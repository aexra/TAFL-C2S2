using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab4Page : Page
{
    public Lab4ViewModel ViewModel
    {
        get;
    }

    public Lab4Page()
    {
        ViewModel = App.GetService<Lab4ViewModel>();
        InitializeComponent();
    }
}
