using System.Diagnostics.Metrics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using TAFL.Classes.Graph;
using TAFL.Controls;
using TAFL.Services;
using TAFL.ViewModels;
using Windows.AI.MachineLearning.Preview;
using Windows.Devices.Bluetooth;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    List<CanvasedEdge> Edges1 = new();

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
        var props = e.GetCurrentPoint(Canva).Properties;

        if (props.IsLeftButtonPressed) NewNode(pos.X, pos.Y);
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

        if (CheckNodeCollisions(node, Canva)) return;

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

        node.Title = GetUniqueName(Canva);

        Canva.Children.Add(node);

        Canvas.SetLeft(node, node.Position.X);
        Canvas.SetTop(node, node.Position.Y);
        Canvas.SetZIndex(node, 10);

        LogService.Log($"Новая вершина в [{node.Position.X}, {node.Position.Y}]");
    }
    public bool CheckNodeCollisions(GraphNodeControl c, Canvas canv)
    {
        foreach (var element in canv.Children)
        {
            if (element is GraphNodeControl node)
            {
                if (node == c) continue;
                var d = Math.Sqrt(Math.Pow(c.Position.X - node.Position.X, 2) + Math.Pow(c.Position.Y - node.Position.Y, 2));
                var mind = c.Radius + node.Radius;
                if (d < mind)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public string GetUniqueName(Canvas canv)
    {
        var counter = 0;
        while (true)
        {
            var found = false;
            foreach (var element in canv.Children)
            {
                if (element is GraphNodeControl node && node.Title == $"p{counter}")
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                counter++;
            }
            else
            {
                return "p" + counter.ToString();
            }
        } 
    }
    public bool IsNameUnique(string name, Canvas canv)
    {
        foreach (var element in canv.Children)
        {
            if (element is GraphNodeControl node && node.Title == name) return false;
        }
        return true;
    }

    public GraphNodeControl? GetNode(string name, Canvas canv)
    {
        foreach (var element in canv.Children)
        {
            if (element is GraphNodeControl node)
            {
                if (node.Title == name) return node;
            }
        }
        return null;
    }
    public void AddEdge(CanvasedEdge edge, Canvas canv)
    {
        foreach (var edgee in Edges1)
        {
            if (edgee.Left == edge.Right && edgee.Right == edge.Left)
            {
                edgee.IsArc = true;
                edge.IsArc = true;
                break;
            }
        }
        Edges1.Add(edge);
        edge.UpdatePath();
        UpdateEdges1(edge.Left);
        canv.Children.Add(edge.PathObject);
        Canvas.SetZIndex(edge.PathObject, 0);
    }
    public void RemoveEdge(CanvasedEdge edge)
    {
        Edges1.Remove(edge);
        foreach (var child in Canva.Children)
        {
            if (child is Microsoft.UI.Xaml.Shapes.Path path && edge.PathObject == path)
            {
                Canva.Children.Remove(child);
                break;
            }
        }
    }
    public void UpdateEdges1(GraphNodeControl node)
    {
        foreach (var edge in Edges1)
        {
            if (edge.Left == node || edge.Right == node)
            {
                foreach (var child in Canva.Children)
                {
                    if (child is Microsoft.UI.Xaml.Shapes.Path path && edge.PathObject == path)
                    {
                        Canva.Children.Remove(child);
                        Canva.Children.Add(edge.UpdatePath());
                        break;
                    }
                }
            }
        }
    }
    public bool IsEdgeExists(CanvasedEdge edge)
    {
        foreach (var edgee in Edges1)
        {
            if (edgee.Left == edge.Left && edgee.Right == edge.Right) return true;
        }
        return false;
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Canva.Children.Clear();
    }
}
