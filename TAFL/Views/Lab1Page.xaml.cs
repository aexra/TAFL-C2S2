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
        // TODO: Initiate encoding sequence
        EncodeProcessBlock.Text = "Решено кодирование!";
        EncodeResultBlock.Text = "Код";
    }

    private void DecodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO: Initiate decoding sequence
        DecodeProcessBlock.Text = "Решено декодирование!";
        DecodeResultBlock.Text = "Слово";
    }
}
