using Microsoft.UI.Xaml.Controls;
using TAFL.Misc;
using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab7Page : Page
{
    public Lab7ViewModel ViewModel
    {
        get;
    }

    public Lab7Page()
    {
        ViewModel = App.GetService<Lab7ViewModel>();
        InitializeComponent();
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
}
