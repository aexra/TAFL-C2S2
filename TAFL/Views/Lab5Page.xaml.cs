using Microsoft.UI.Xaml.Controls;
using TAFL.Services;
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

    private void Canva_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(Canva).Position;

        

        //Canva.Children.Add();

        //Canvas.SetLeft(tb, pos.X);
        //Canvas.SetTop(tb, pos.Y);
    }

    private void Canva_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        
    }

    private void Canva_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        
    }
}
