using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab1Page : Page
{
    public Lab1ViewModel ViewModel
    {
        get;
    }

    public Lab1Page()
    {
        ViewModel = App.GetService<Lab1ViewModel>();
        InitializeComponent();
    }

    private void EncodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!CheckErrors(EncodeAlphabetBox.Text, EncodeWordBox.Text))
        {
            // TODO: Show Error Box
            return;
        }
        // TODO: Initiate encoding sequence
        EncodeProcessBlock.Text = "Решено кодирование!";
        EncodeResultBlock.Text = "Код";
    }

    private void DecodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!CheckErrors(DecodeAlphabetBox.Text, DecodeNumberBox.Text))
        {
            // TODO: Show Error Box
            return;
        }
        // TODO: Initiate decoding sequence
        DecodeProcessBlock.Text = "Решено декодирование!";
        DecodeResultBlock.Text = "Слово";
    }

    private void AlphabetBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        var distinct = StringToDistinctString(sender.Text);
        if (distinct != sender.Text) 
        {
            var pointer = sender.SelectionStart - 1;
            sender.Text = distinct;
            sender.SelectionStart = pointer;
        }
    }

    private string StringToDistinctString(string text)
    {
        return String.Join("", text.Distinct());
    }

    private bool CheckErrors(string alphabet, string task)
    {
        return CheckEmptyFields(alphabet, task) && CompareAlphabets(alphabet, task);
    }

    private bool CheckEmptyFields(string alphabet, string task)
    {
        return !(alphabet == String.Empty || task == String.Empty);
    }

    private bool CompareAlphabets(string alphabet, string word)
    {
        foreach (var item in word) 
        { 
            if (!alphabet.Contains(item))
            {
                return false;
            }
        }
        return true;
    }
}
