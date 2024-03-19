using System.Collections.ObjectModel;
using TAFL.Controls;
using TAFL.Enums;
using TAFL.Helpers;
using TAFL.Structures;

namespace TAFL.Services;
public class LogService
{
    private static List<LogMessageManifest> logs = new();

    public static ObservableCollection<LogMessageControl> LogMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> InfoMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> WarningMessages { get; private set; } = new();
    public static ObservableCollection<LogMessageControl> ErrorMessages { get; private set; } = new();

    public static bool UpdateRequired = false;

    public static void Log(string msg)
    {
        TryLog(msg, LogSeverity.Info);
    }
    public static void Log(object obj)
    {
        TryLog(obj.ToString(), LogSeverity.Info);
    }
    public static void Warning(string msg)
    {
        TryLog(msg, LogSeverity.Warning);
    }
    public static void Warning(object obj)
    {
        TryLog(obj.ToString(), LogSeverity.Warning);
    }
    public static void Error(string msg)
    {
        TryLog(msg, LogSeverity.Error);
    }
    public static void Error(object obj)
    {
        TryLog(obj.ToString(), LogSeverity.Error);
    }
    public static void ForceUpdateControlsCollections()
    {
        try
        {
            _UpdateControlsCollections_();
        }
        catch { }
    }

    private static void TryLog(string msg, LogSeverity type)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;
        try
        {
            logs.Add(new LogMessageManifest() { Text = msg, Type = type, Time = TimeHelper.GetNowString(), Id = (ulong)logs.Count });
            _UpdateControlsCollections_();
        }
        catch (Exception e) { UpdateRequired = true; return; }
        UpdateRequired = false;
    }

    private static void AddLog(LogMessageManifest log)
    {
        LogMessageControl lmc = new()
        {
            Text = log.Text,
            Type = log.Type,
            Time = log.Time,
            Id = log.Id,
        };
        LogMessages.Add(lmc);

        LogMessageControl lmce = new()
        {
            Text = log.Text,
            Type = log.Type,
            Time = log.Time,
            Id = log.Id,
        };
        switch (log.Type)
        {
            case LogSeverity.Warning:
                WarningMessages.Add(lmce);
                break;
            case LogSeverity.Error:
                ErrorMessages.Add(lmce);
                break;
            default:
                InfoMessages.Add(lmce);
                break;
        }
    }

    private static void _UpdateControlsCollections_()
    {
        foreach (var log in logs)
        {
            var found = false;
            foreach (var control in LogMessages)
            {
                if (control.Id == log.Id)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AddLog(log);
            }
        }
    }
}