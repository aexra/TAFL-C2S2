using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using TAFL.Controls;

namespace TAFL.Helpers;
public static class DialogHelper
{
    public static async Task ShowErrorDialogAsync(string title, XamlRoot root)
    {
        ContentDialog errorDialog = new ContentDialog();

        errorDialog.XamlRoot = root;
        errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        errorDialog.Title = title;
        errorDialog.CloseButtonText = "Ок";
        errorDialog.DefaultButton = ContentDialogButton.Close;

        await errorDialog.ShowAsync();
    }
    public static async Task<string?> ShowSingleInputDialogAsync(XamlRoot root, string title = "Заголовок", string placeholder = "Введите чо то", string primaryText = "Ок", string closeText = "Отмена")
    {
        var content = new StringInputDialog();
        var dialog = new ContentDialog();

        dialog.XamlRoot = root;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = title;
        dialog.PrimaryButtonText = primaryText;
        dialog.CloseButtonText = closeText;
        dialog.DefaultButton = ContentDialogButton.Primary;
        content.Placeholder = placeholder;
        dialog.Content = content;

        await dialog.ShowAsync();

        return content.Input ?? null;
    }
}
