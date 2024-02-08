using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab9Page : Page
{
    public Lab9ViewModel ViewModel
    {
        get;
    }

    public Lab9Page()
    {
        ViewModel = App.GetService<Lab9ViewModel>();
        InitializeComponent();
    }
}
