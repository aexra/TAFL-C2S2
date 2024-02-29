using System.Text;
using System.Text.RegularExpressions;
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

    private async void SolveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (await CheckErrorsAsync()) return;

        var outputString = string.Empty;

        var limit = int.Parse(AmountBox.Text);
        var maxDepth = int.Parse(DepthBox.Text);
        var counter = 0;
        var code = 0;

        while (counter < limit && code < maxDepth)
        {
            var s = LexService.Decode(AlphabetBox.Text, (uint)++code, out _);
            if (Regex.IsMatch(s, RegExBox.Text)) outputString += $"{++counter}. {s}\n";
        }

        ResultBlock.Text = outputString;

        if (counter < limit)
        {
            await WarningAsync("Недостаточная глубина поиска для отображения заданного количества строк", "Ошибка поиска");
        }
    }

    private async Task<bool> CheckErrorsAsync()
    {
        if (AlphabetBox.Text == string.Empty)
        {
            await WarningAsync("Введите алфавит");
            return true;
        }

        if (AmountBox.Text == string.Empty)
        {
            await WarningAsync("Введите количество");
            return true;
        }
        if (!int.TryParse(AmountBox.Text, out _))
        {
            await WarningAsync("Количество должно быть натуральным числом");
            return true;
        }
        if (int.Parse(AmountBox.Text) < 1)
        {
            await WarningAsync("Количество должно быть натуральным числом");
            return true;
        }

        if (DepthBox.Text == string.Empty)
        {
            await WarningAsync("Введите глубину поиска");
            return true;
        }
        if (!int.TryParse(DepthBox.Text, out _))
        {
            await WarningAsync("Глубина должна быть натуральным числом");
            return true;
        }
        if (int.Parse(DepthBox.Text) < 1)
        {
            await WarningAsync("Глубина должна быть натуральным числом");
            return true;
        }

        if (RegExBox.Text == string.Empty)
        {
            await WarningAsync("Введите регулярное выражение");
            return true;
        }

        return false;
    }

    private async Task WarningAsync(string content = "", string title = "Неверные исходные данные", string okbtn = "Ок")
    {
        await new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = title,
            Content = content,
            CloseButtonText = okbtn
        }.ShowAsync();
    }
}
