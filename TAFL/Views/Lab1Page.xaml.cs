﻿using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using TAFL.Misc;
using TAFL.ViewModels;
using Windows.UI.Popups;

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

    private async void EncodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!CheckErrors(EncodeAlphabetBox.Text, EncodeWordBox.Text))
        {
            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Неверные исходные данные",
                Content = "Поля пустые или в словаре недостаточно символов для кодирования слова",
                CloseButtonText = "Ок"
            }.ShowAsync();
            return;
        }
        
        EncodeResultBlock.Text = LexService.Encode(EncodeAlphabetBox.Text, EncodeWordBox.Text, out var process).ToString();
        EncodeProcessBlock.Text = process;
    }
    private async void DecodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!CheckEmptyFields(DecodeAlphabetBox.Text, DecodeNumberBox.Text) || !uint.TryParse(DecodeNumberBox.Text, out var _) || uint.Parse(DecodeNumberBox.Text) < 1)
        {
            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Неверные исходные данные",
                Content = "Поле кода должно содержать натуральное число",
                CloseButtonText = "Ок"
            }.ShowAsync();
            return;
        }

        DecodeResultBlock.Text = LexService.Decode(DecodeAlphabetBox.Text, uint.Parse(DecodeNumberBox.Text), out var process);
        DecodeProcessBlock.Text = process;
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
