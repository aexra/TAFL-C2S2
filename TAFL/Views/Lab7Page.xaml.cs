using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using TAFL.Misc;
using TAFL.ViewModels;
using AexraUI.Controls;
using TAFL.Enums;
using TAFL.Extensions;
using System.Text.RegularExpressions;

namespace TAFL.Views;

public sealed partial class Lab7Page : Page
{
    public ObservableCollection<DynamicOption> Ruleset { get; set; } = new();

    public Lab7ViewModel ViewModel
    {
        get;
    }
    public Lab7Page()
    {
        ViewModel = App.GetService<Lab7ViewModel>();
        InitializeComponent();
    }

    private GrammarType AnalyzeGrammar()
    {
        var based = BasedSymbolsBox.Text.ToArray();
        var start = StartSymbolsBox.Text.ToArray();
        var terminal = EndSymbolsBox.Text.ToArray();

        Logger.Log(
            $"Не терминальные символы: {{{string.Join(",", based)}}}" + '\n' +   
            $"Начальные символы: {{{string.Join(",", start)}}}" + '\n' +
            $"Терминальные символы: {{{string.Join(",", terminal)}}}"
        );

        // ε

        // Проверим что это тип 3
        var flag = true;
        foreach (var rule in Ruleset)
        {
            if (rule.Key.Length != 1 || terminal.Contains(rule.Key[0]) || !Regex.Match(rule.Value, "(^[A-Z][a-z]+$)|(^[a-z]+[A-Z]$)|(^[ε]{1}$)").Success)
            {
                flag = false;
                break;
            }
        }

        if ( flag )
        {
            return GrammarType.Type3;
        }

        // Проверим что это тип 2
        flag = true;
        foreach (var rule in Ruleset)
        {
            if (rule.Key.Length != 1 || terminal.Contains(rule.Key[0]) || rule.Value.ToList().Exists(symbol => based.Contains(symbol)))
            {
                flag = false;
                break;
            }
        }

        if (flag)
        {
            return GrammarType.Type2;
        }

        // Проверим что это тип 1
        flag = true;
        foreach (var rule in Ruleset)
        {
            // Проверим условие длины
            if (!(rule.Key.Length <= rule.Value.ToList().Where(symbol => symbol != 'ε').Count()))
            {
                flag = false;
                break;
            }

            // Если длина ключа 2
            if (rule.Key.Length == 2)
            {
                if (!(rule.Key[0] == rule.Value[0] || rule.Key[^1] == rule.Value[^1]))
                {
                    flag = false;
                    break;
                }
            }
            // Если длина ключа любая другая
            else if (rule.Key.Length > 2)
            {
                if (!(rule.Key[0] == rule.Value[0] && rule.Key[^1] == rule.Value[^1]))
                {
                    flag = false;
                    break;
                }
            }
        }

        if (flag)
        {
            return GrammarType.Type1;
        }

        return GrammarType.Type0;
    }

    private void BasedSymbolsBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var distinct = StringUtils.StringToDistinctString(sender.Text);
        if (distinct != sender.Text)
        {
            var pointer = sender.SelectionStart - 1;
            sender.Text = distinct;
            sender.SelectionStart = pointer;
        }
    }
    private void StartSymbolsBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var distinct = StringUtils.StringToDistinctString(sender.Text);
        if (distinct != sender.Text)
        {
            var pointer = sender.SelectionStart - 1;
            sender.Text = distinct;
            sender.SelectionStart = pointer;
        }
    }
    private void EndSymbolsBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var distinct = StringUtils.StringToDistinctString(sender.Text);
        if (distinct != sender.Text)
        {
            var pointer = sender.SelectionStart - 1;
            sender.Text = distinct;
            sender.SelectionStart = pointer;
        }
    }
    private void AddRuleBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var rule = new DynamicOption() { KeyPlaceholder = "Это", ValuePlaceholder = "Вот в это" };
        rule.RemoveRequested += (s) => { Ruleset.Remove(s); };
        rule.ValueChanging += (s, e) => {
            if (s.Text == "eps")
            {
                s.Text = "ε";
                s.SelectionStart = 1;
            }
        };
        Ruleset.Add(rule);
    }
    private void SolveBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var type = AnalyzeGrammar();

        switch (type)
        {
            case GrammarType.Type1:
                Logger.Log("Тип 1 - Контекстно-зависимая грамматика");
                break;
            case GrammarType.Type2:
                Logger.Log("Тип 2 - Контекстно-свободная грамматика");
                break;
            case GrammarType.Type3:
                Logger.Log("Тип 3 - Регулярная грамматика");
                break;
            default:
                Logger.Log("Тип 0 - Грамматика фразовой структуры (грамматика без ограничений)");
                break;
        }
    }
}
