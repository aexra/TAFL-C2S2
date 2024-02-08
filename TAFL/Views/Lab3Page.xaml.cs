using Microsoft.UI.Xaml.Controls;

using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab3Page : Page
{
    public Lab3ViewModel ViewModel
    {
        get;
    }

    public Lab3Page()
    {
        ViewModel = App.GetService<Lab3ViewModel>();
        InitializeComponent();
    }
}
