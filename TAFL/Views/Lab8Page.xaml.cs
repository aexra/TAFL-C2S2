using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab8Page : Page
{
    public Lab8ViewModel ViewModel
    {
        get;
    }

    public Lab8Page()
    {
        ViewModel = App.GetService<Lab8ViewModel>();
        InitializeComponent();
    }
}
