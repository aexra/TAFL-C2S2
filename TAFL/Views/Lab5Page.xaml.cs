using System.Numerics;
using Microsoft.UI.Xaml.Controls;
using TAFL.Controls;
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
        NewNode(pos.X, pos.Y);
    }

    private void Canva_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        
    }

    private void Canva_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        
    }

    private void NewNode(double x, double y)
    {
        var node = new GraphNodeControl(new Vector2((float)x, (float)y), Canva);
        node.Page = this;

        if (CheckNodeCollisions(node)) return;

        if (node.Position.X < 0)
        {
            node.Position = new Vector2(0, node.Position.Y);
        }
        if (node.Position.Y < 0)
        {
            node.Position = new Vector2(node.Position.X, 0);
        }
        if (node.Position.X > Canva.ActualWidth - 40)
        {
            node.Position = new Vector2((float)Canva.ActualWidth - 40, node.Position.Y);
        }
        if (node.Position.Y > Canva.ActualHeight - 40)
        {
            node.Position = new Vector2(node.Position.X, (float)Canva.ActualHeight - 40);
        }

        Canva.Children.Add(node);

        Canvas.SetLeft(node, node.Position.X);
        Canvas.SetTop(node, node.Position.Y);

        LogService.Log($"Новая вершина в [{node.Position.X}, {node.Position.Y}]");
    }
    public bool CheckNodeCollisions(GraphNodeControl c)
    {
        foreach (GraphNodeControl node in Canva.Children)
        {
            if (node == c) continue;
            var d = Math.Sqrt(Math.Pow(c.Position.X - node.Position.X, 2) + Math.Pow(c.Position.Y - node.Position.Y, 2));
            var mind = c.Radius + node.Radius;
            if (d < mind)
            {
                return true;
            }
        }
        return false;
    }
}
