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
        EncodeProcessBlock.Text = "Решено кодирование!";
    }

    private void DecodeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        DecodeProcessBlock.Text = "Решено декодирование!";
    }
}
