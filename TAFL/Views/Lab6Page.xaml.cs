using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab6Page : Page
{
    public Lab6ViewModel ViewModel
    {
        get;
    }

    public Lab6Page()
    {
        ViewModel = App.GetService<Lab6ViewModel>();
        InitializeComponent();
    }
}
