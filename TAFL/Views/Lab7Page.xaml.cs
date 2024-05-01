using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using TAFL.Misc;
using TAFL.ViewModels;
using AexraUI.Controls;
using TAFL.Enums;
using TAFL.Extensions;

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
        var type = GrammarType.Type0;



        return type;
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
        Ruleset.Add(rule);
    }
    private void SolveBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var type = AnalyzeGrammar();

        switch (type)
        {
            case GrammarType.Type1:
                Logger.Log("Контекстно-зависимая грамматика");
                break;
            case GrammarType.Type2:
                Logger.Log("Контекстно-свободная грамматика");
                break;
            case GrammarType.Type3:
                Logger.Log("Регулярная грамматика");
                break;
            default:
                Logger.Log("Грамматика фразовой структуры (грамматика без ограничений)");
                break;
        }
    }
}
