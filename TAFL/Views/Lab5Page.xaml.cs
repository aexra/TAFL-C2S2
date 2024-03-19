using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    public Lab5ViewModel ViewModel
    {
        get;
    }

    public Lab5Page()
    {
        ViewModel = App.GetService<Lab5ViewModel>();
        InitializeComponent();
    }

    private void Canva_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {

    }
}
