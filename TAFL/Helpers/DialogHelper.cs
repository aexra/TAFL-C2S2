using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace TAFL.Helpers;
public static class DialogHelper
{
    public static async Task ErrorDialog(string title, XamlRoot root)
    {
        ContentDialog errorDialog = new ContentDialog();

        errorDialog.XamlRoot = root;
        errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        errorDialog.Title = title;
        errorDialog.CloseButtonText = "Ок";
        errorDialog.DefaultButton = ContentDialogButton.Close;

        await errorDialog.ShowAsync();
    }
}
