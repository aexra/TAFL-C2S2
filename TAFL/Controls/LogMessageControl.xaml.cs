using Microsoft.UI.Xaml.Controls;
using TAFL.Enums;

namespace TAFL.Controls;
public sealed partial class LogMessageControl : UserControl
{
    public LogSeverity Type
    {
        get; set;
    }
    public string Text
    {
        get; set;
    }
    public string Time
    {
        get; set;
    }
    public ulong Id;

    public string IconSource
    {
        get
        {
            switch (Type)
            {
                case LogSeverity.Warning:
                    return "ms-appx:///Assets/LogMessageWarningIcon.png";
                case LogSeverity.Error:
                    return "ms-appx:///Assets/LogMessageErrorIcon.png";
                default:
                    return "ms-appx:///Assets/LogMessageInfoIcon.png";
            }
        }
        set => IconSource = value;
    }

    public LogMessageControl()
    {
        this.InitializeComponent();
    }
}