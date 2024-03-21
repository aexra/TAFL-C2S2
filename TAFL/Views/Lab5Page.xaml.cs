﻿using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using TAFL.Classes.Graph;
using TAFL.Controls;
using TAFL.Services;
using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    private readonly List<CanvasedEdge> Edges1 = new();
    private static readonly int VertexZ = 10;
    private static readonly int EdgeZ = 0;

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
        Canvas.SetZIndex(node, VertexZ);

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
                if (element is GraphNodeControl node && node.Title == $"q{counter}")
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
                return "q" + counter.ToString();
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
            if (edgee.Left == edge.Right && edgee.Right == edge.Left && !edge.IsLoop)
            {
                edgee.ToArc();
                edge.ToArc();
                break;
            }
        }
        Edges1.Add(edge);
        edge.Page5 = this;
        edge.UpdatePath();
        UpdateConnectedEdges(edge.Left);
        canv.Children.Add(edge.PathObject);
        canv.Children.Add(edge.WeightBox);
        Canvas.SetZIndex(edge.PathObject, EdgeZ);
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
        foreach (var child in Canva.Children)
        {
            if (child is TextBox box && edge.WeightBox == box)
            {
                Canva.Children.Remove(child);
                break;
            }
        }
    }
    public void UpdateAllEdges()
    {
        foreach (var edge in Edges1)
        {
            UpdateEdge(edge);
        }
    }
    public void UpdateConnectedEdges(GraphNodeControl node)
    {
        foreach (var edge in Edges1)
        {
            if (edge.Left == node || edge.Right == node)
            {
                UpdateEdge(edge);
            }
        }
    }
    private void UpdateEdge(CanvasedEdge edge)
    {
        foreach (var child in Canva.Children)
        {
            if (child is Microsoft.UI.Xaml.Shapes.Path path && edge.PathObject == path)
            {
                Canva.Children.Remove(child);
                Canva.Children.Add(edge.UpdatePath());
                Canvas.SetZIndex(edge.PathObject, EdgeZ);
                break;
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
    public bool IsConnectionExists(GraphNodeControl node1, GraphNodeControl node2)
    {
        foreach (var edgee in Edges1)
        {
            if (edgee.Left == node1 && edgee.Right == node2) return true;
        }
        return false;
    }
    public void ConnectVertrices(GraphNodeControl node1, GraphNodeControl node2, string weight)
    {
        var edge = new CanvasedEdge(node1, node2, weight);
        AddEdge(edge, Canva);
        Edges1.Add(edge);
    }
    public void RemoveVertex(GraphNodeControl node)
    {
        List<CanvasedEdge> toDelete = new();
        foreach (var edge in Edges1)
        {
            if (edge.Left == node || edge.Right == node)
            {
                toDelete.Add(edge);
            }
        }
        toDelete.ForEach(RemoveEdge);
        Canva.Children.Remove(node);
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Canva.Children.Clear();
    }

    private void BuildQButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BuildSButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BuildSButton_Click_1(object sender, RoutedEventArgs e)
    {

    }

    private void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private string GetQTable()
    {
        return string.Empty;
    }
    private string GetSTable()
    {
        return string.Empty;
    }
    private string GetPTable()
    {
        return string.Empty;
    }

    private Graph GetRawGraph()
    {
        var graph = new Graph();

        // Получаем списоки всех вершин и ребер
        List<GraphNodeControl> nodes = new();
        List<CanvasedEdge> edges = new();
        foreach (var child in Canva.Children)
        {
            if (child is GraphNodeControl gnc)
            {
                nodes.Add(gnc);
            }
            else if (child is Microsoft.UI.Xaml.Shapes.Path path)
            {
                foreach (var edge in Edges1)
                {
                    if (edge.PathObject == path)
                    {
                        edges.Add(edge);
                        break;
                    }
                }
            }
        }

        // Формируем граф
        foreach (var node in nodes)
        {
            graph.AddNode(new Node(node.Name));
        }

        return graph;
    }
}
