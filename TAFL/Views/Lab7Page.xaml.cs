using Microsoft.UI.Xaml.Controls;

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
}
