using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TAFL.Controls;
using TAFL.Services;
using SelectionMode = TAFL.Enums.SelectionMode;

namespace TAFL.Classes.Graph;
public class CanvasedGraph
{
    // COMPTIME CONSTANTS
    public static readonly int VertexZ = 10;
    public static readonly int EdgeZ = 0;

    // INPUT PROPS
    public Canvas Canvas;

    // OTHERS
    public List<CanvasedEdge> Edges => Canvas.Children.Where(x => x is CanvasedEdge).Cast<CanvasedEdge>().ToList();
    public GraphNodeControl? SelectedNode => GetSelectedNode();
    public List<GraphNodeControl>? SelectedNodes => GetSelectedNodes();
    public SelectionMode SelectionMode = SelectionMode.Single;
    public Queue<Action<GraphNodeControl>> SelectionRequests = new();

    // CONSTRUCTORS
    public CanvasedGraph(Canvas canvas)
    {
        Canvas = canvas;

        var transparentColor = Color.Transparent;
        Canvas.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(transparentColor.A, transparentColor.R, transparentColor.G, transparentColor.B));

        Canvas.PointerPressed += PointerPressed;
    }

    // POINTER EVENTS
    private void PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (SelectedNode != null)
        {
            DeselectAllNodes();
            return;
        }
        var pos = e.GetCurrentPoint(Canvas).Position;
        var props = e.GetCurrentPoint(Canvas).Properties;

        if (props.IsLeftButtonPressed) NewNode(pos.X, pos.Y);

    }

    // GRAPH MANIPULATION METHODS
    public void Clear()
    {
        Canvas.Children.Clear();
    }

    // NODES MANIPULATION METHODS
    public void NewNode(double x, double y)
    {
        var node = new GraphNodeControl(new Vector2((float)x, (float)y), this);

        if (CheckNodeCollisions(node)) return;

        if (node.Position.X < 0)
        {
            node.Position = new Vector2(0, node.Position.Y);
        }
        if (node.Position.Y < 0)
        {
            node.Position = new Vector2(node.Position.X, 0);
        }
        if (node.Position.X > Canvas.ActualWidth - 40)
        {
            node.Position = new Vector2((float)Canvas.ActualWidth - 40, node.Position.Y);
        }
        if (node.Position.Y > Canvas.ActualHeight - 40)
        {
            node.Position = new Vector2(node.Position.X, (float)Canvas.ActualHeight - 40);
        }

        node.Title = GetUniqueName();

        Canvas.Children.Add(node);

        Canvas.SetLeft(node, node.Position.X);
        Canvas.SetTop(node, node.Position.Y);
        Canvas.SetZIndex(node, VertexZ);

        LogService.Log($"Новая вершина в [{node.Position.X}, {node.Position.Y}]");
    }
    public void RemoveNode(GraphNodeControl node)
    {
        List<CanvasedEdge> toDelete = new();
        foreach (var edge in Edges)
        {
            if (edge.Left == node || edge.Right == node)
            {
                toDelete.Add(edge);
            }
        }
        toDelete.ForEach(RemoveEdge);
        Canvas.Children.Remove(node);
    }
    public void ConnectNodes(GraphNodeControl node1, GraphNodeControl node2, string weight)
    {
        var edge = new CanvasedEdge(node1, node2, weight, this);
        NewEdge(edge);
        Edges.Add(edge);
    }
    public void DeselectAllNodes()
    {
        foreach (var child in Canvas.Children)
        {
            if (child is GraphNodeControl node)
            {
                node.Deselect();
            }
        }
    }

    // EDGES MANIPULATION METHODS
    public void NewEdge(CanvasedEdge edge)
    {
        foreach (var edgee in Edges)
        {
            if (edgee.Left == edge.Right && edgee.Right == edge.Left && !edge.IsLoop)
            {
                edgee.ToArc();
                edge.ToArc();
                break;
            }
        }
        edge.UpdatePath();
        UpdateConnectedEdges(edge.Left);
        Canvas.Children.Add(edge.PathObject);
        Canvas.Children.Add(edge.WeightBox);
        Canvas.SetZIndex(edge.PathObject, EdgeZ);
    }
    public void RemoveEdge(CanvasedEdge edge)
    {
        Edges.Remove(edge);
        foreach (var child in Canvas.Children)
        {
            if (child is Microsoft.UI.Xaml.Shapes.Path path && edge.PathObject == path)
            {
                Canvas.Children.Remove(child);
                break;
            }
        }
        foreach (var child in Canvas.Children)
        {
            if (child is TextBox box && edge.WeightBox == box)
            {
                Canvas.Children.Remove(child);
                break;
            }
        }
    }
    public void UpdateAllEdges()
    {
        foreach (var edge in Edges)
        {
            UpdateEdge(edge);
        }
    }
    public void UpdateConnectedEdges(GraphNodeControl node)
    {
        foreach (var edge in Edges)
        {
            if (edge.Left == node || edge.Right == node)
            {
                UpdateEdge(edge);
            }
        }
    }
    public void UpdateEdge(CanvasedEdge edge)
    {
        foreach (var child in Canvas.Children)
        {
            if (child is Microsoft.UI.Xaml.Shapes.Path path && edge.PathObject == path)
            {
                Canvas.Children.Remove(child);
                Canvas.Children.Add(edge.UpdatePath());
                Canvas.SetZIndex(edge.PathObject, EdgeZ);
                break;
            }
        }
    }

    // HELPERS
    public GraphNodeControl? GetSelectedNode()
    {
        if (SelectionMode != SelectionMode.Single) return null;
        foreach (var child in Canvas.Children)
        {
            if (child is GraphNodeControl node)
            {
                if (node.IsSelected)
                {
                    return node;
                }
            }
        }
        return null;
    }
    public List<GraphNodeControl>? GetSelectedNodes()
    {
        if (SelectionMode != SelectionMode.Multiple) return null;
        List<GraphNodeControl> nodes = new();
        foreach (var child in Canvas.Children)
        {
            if (child is GraphNodeControl node)
            {
                if (node.IsSelected)
                {
                    nodes.Add(node);
                }
            }
        }
        return nodes;
    }
    public GraphNodeControl? GetNode(string name)
    {
        foreach (var element in Canvas.Children)
        {
            if (element is GraphNodeControl node)
            {
                if (node.Title == name) return node;
            }
        }
        return null;
    }
    public string GetUniqueName()
    {
        var counter = 0;
        while (true)
        {
            var found = false;
            foreach (var element in Canvas.Children)
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
    public bool IsNameUnique(string name)
    {
        foreach (var element in Canvas.Children)
        {
            if (element is GraphNodeControl node && node.Title == name) return false;
        }
        return true;
    }
    public bool IsEdgeExists(CanvasedEdge edge)
    {
        foreach (var edgee in Edges)
        {
            if (edgee.Left == edge.Left && edgee.Right == edge.Right) return true;
        }
        return false;
    }
    public bool IsEdgeExists(GraphNodeControl node1, GraphNodeControl node2)
    {
        foreach (var edgee in Edges)
        {
            if (edgee.Left == node1 && edgee.Right == node2) return true;
        }
        return false;
    }
    public bool CheckNodeCollisions(GraphNodeControl c)
    {
        foreach (var element in Canvas.Children)
        {
            if (element is GraphNodeControl node)
            {
                if (node == c) continue;
                var d = Math.Sqrt(Math.Pow(c.Position.X - node.Position.X, 2) + Math.Pow(c.Position.Y - node.Position.Y, 2));
                if (d < c.Radius + node.Radius)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // RAW GRAPH THINGS
    public Graph GetRawGraph()
    {
        var graph = new Graph();

        // Получаем списоки всех вершин и ребер
        List<GraphNodeControl> nodes = new();
        List<CanvasedEdge> edges = new();
        foreach (var child in Canvas.Children)
        {
            if (child is GraphNodeControl gnc)
            {
                nodes.Add(gnc);
            }
            else if (child is Microsoft.UI.Xaml.Shapes.Path path)
            {
                foreach (var edge in Edges)
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
        // Добавим все вершины
        foreach (var node in nodes)
        {
            graph.AddNode(new Node(node.Title));
        }
        // Добавим все связи
        foreach (var edge in edges)
        {
            var node1 = graph.GetNode(edge.Left.Title);
            var node2 = graph.GetNode(edge.Right.Title);
            if (!graph.IsConnectionExists(node1, node2))
            {
                node1.Connect(node2, edge.Weight, true);
            }
        }

        return graph;
    }
}
