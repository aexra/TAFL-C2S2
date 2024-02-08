using Microsoft.UI.Xaml.Controls;

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
}
