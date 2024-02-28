using Microsoft.UI.Xaml.Controls;
using TAFL.Misc;
using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab2Page : Page
{
    public Lab2ViewModel ViewModel
    {
        get;
    }

    public Lab2Page()
    {
        ViewModel = App.GetService<Lab2ViewModel>();
        InitializeComponent();
    }



    private void AlphabetBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var distinct = StringUtils.StringToDistinctString(sender.Text);
        if (distinct != sender.Text)
        {
            var pointer = sender.SelectionStart - 1;
            sender.Text = distinct;
            sender.SelectionStart = pointer;
        }
    }

    private void SolveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
