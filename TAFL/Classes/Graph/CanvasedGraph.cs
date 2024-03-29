using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TAFL.Controls;
using TAFL.Services;
using Windows.Storage;
using SelectionMode = TAFL.Enums.SelectionMode;

namespace TAFL.Classes.Graph;
public class CanvasedGraph
{
    // COMPTIME CONSTANTS
    public static readonly int VertexZ = 10;
    public static readonly int EdgeZ = 0;

    // INPUT PROPS
    public Canvas Canvas;

    // FLAGS
    public bool ReadOnly = false;

    // OTHERS
    public List<GraphNodeControl> Nodes => Canvas.Children.Where(x => x is GraphNodeControl).Cast<GraphNodeControl>().ToList();
    public List<CanvasedEdge> Edges = new();
    public GraphNodeControl? SelectedNode => GetSelectedNode();
    public List<GraphNodeControl>? SelectedNodes => GetSelectedNodes();
    public SelectionMode SelectionMode = SelectionMode.None;
    public Queue<Action<GraphNodeControl, bool>> SelectionRequests = new();

    // EVENT HANDLERS
    public delegate void NodeCreatedHandler(GraphNodeControl node);
    public delegate void NodeRemovedHandler(GraphNodeControl node);
    public delegate void NodeSelectedHandler(GraphNodeControl node);
    public delegate void NodeRenamedHandler(GraphNodeControl node, string oldName);
    public delegate void EdgeCreatedHandler(CanvasedEdge edge);
    public delegate void EdgeRemovedHandler(CanvasedEdge edge);
    public delegate void GraphClearedHandler();
    public delegate void LoadedHandler(string path);

    // EVENTS
    public event NodeCreatedHandler? NodeCreated;
    public event NodeRemovedHandler? NodeRemoved;
    public event NodeSelectedHandler? NodeSelected;
    public event NodeRenamedHandler? NodeRenamed;
    public event EdgeCreatedHandler? EdgeCreated;
    public event EdgeRemovedHandler? EdgeRemoved;
    public event GraphClearedHandler? GraphCleared;
    public event LoadedHandler? Loaded;


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
        if (ReadOnly) return;
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
        Edges.Clear();
        SelectionRequests.Clear();
        GraphCleared?.Invoke();
    }
    public void RequestSelection(Action<GraphNodeControl, bool> selected)
    {
        SelectionRequests.Enqueue(selected);
    }

    // NODES MANIPULATION METHODS
    public GraphNodeControl? NewNode(double x, double y, string? name = null)
    {
        var node = new GraphNodeControl(new Vector2((float)x, (float)y), this);

        if (CheckNodeCollisions(node)) return null;

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

        if (name == null) node.Title = GetUniqueName();
        else node.Title = name;

        Canvas.Children.Add(node);

        Canvas.SetLeft(node, node.Position.X);
        Canvas.SetTop(node, node.Position.Y);
        Canvas.SetZIndex(node, VertexZ);

        NodeCreated?.Invoke(node);

        return node;
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
        NodeRemoved?.Invoke(node);
    }
    public void ConnectNodes(GraphNodeControl left, GraphNodeControl right, string weight)
    {
        NewEdge(left, right, weight);
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
    public void LockAllNodesPosition()
    {
        foreach (var node in Nodes)
        {
            node.LockPosition();
        }
    }
    public void UnlockAllNodesPosition()
    {
        foreach (var node in Nodes)
        {
            node.UnlockPosition();
        }
    }
    public async Task<bool> RenameNode(string target, string name)
    {
        var node = GetNode(target);
        if (node != null)
        {
            if (IsNameValid(name))
            {
                var oldName = node.Title;
                node.Title = name;
                NodeRenamed?.Invoke(node, oldName);
                return true;
            }
            else
            {
                ContentDialog errorDialog = new ContentDialog();

                errorDialog.XamlRoot = Canvas.XamlRoot;
                errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                errorDialog.Title = "Недопустимое имя вершины: повторяется или пустое";
                errorDialog.CloseButtonText = "Ок";
                errorDialog.DefaultButton = ContentDialogButton.Close;

                await errorDialog.ShowAsync();

                return false;
            }
        }
        else
        {
            ContentDialog errorDialog = new ContentDialog();

            errorDialog.XamlRoot = Canvas.XamlRoot;
            errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            errorDialog.Title = "Вершина с таким именем не найдена";
            errorDialog.CloseButtonText = "Ок";
            errorDialog.DefaultButton = ContentDialogButton.Close;

            await errorDialog.ShowAsync();

            return false;
        }
    }

    // EDGES MANIPULATION METHODS
    public void NewEdge(GraphNodeControl left, GraphNodeControl right, string weight)
    {
        var edge = new CanvasedEdge(left, right, weight, this);
        Edges.Add(edge);
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
        EdgeCreated?.Invoke(edge);
    }
    public void RemoveEdge(CanvasedEdge edge)
    {
        Edges.Remove(edge);
        if (edge.IsLoop)
        {
            var deletedIndex = edge.LoopIndex;
            edge.Left.Loops--;
            foreach (var edgee in Edges)
            {
                if (edgee.IsLoop && edgee.Left == edge.Left && edgee.LoopIndex > deletedIndex)
                {
                    edgee.LoopIndex--;
                }
            }
            UpdateConnectedEdges(edge.Left);
        }
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
        if (edge.Left != edge.Right)
        {
            foreach (var edgee in Edges)
            {
                if (edgee.Left == edge.Right && edgee.Right == edge.Left)
                {
                    edgee.ToLine();
                    UpdateEdge(edgee);
                    break;
                }
            }
        }
        EdgeRemoved?.Invoke(edge);
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
    public bool IsNameValid(string name)
    {
        return IsNameUnique(name) && !string.IsNullOrWhiteSpace(name);
    }
    public bool IsEdgeExists(CanvasedEdge edge)
    {
        foreach (var edgee in Edges)
        {
            if (edgee.Left == edge.Left && edgee.Right == edge.Right) return true;
        }
        return false;
    }
    public bool IsEdgeExists(GraphNodeControl left, GraphNodeControl right)
    {
        foreach (var edgee in Edges)
        {
            if (edgee.Left == left && edgee.Right == right) return true;
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
    public GraphNodeControl? GetStartNode()
    {
        foreach (var node in Nodes)
        {
            if (node.SubState == Enums.NodeSubState.Start) return node;
        }
        return null;
    }
    public GraphNodeControl? GetEndNode()
    {
        foreach (var node in Nodes)
        {
            if (node.SubState == Enums.NodeSubState.End) return node;
        }
        return null;
    }

    // RAW GRAPH THINGS
    public Graph GetRawGraph()
    {
        var graph = new Graph();
        var start = GetStartNode();
        var end = GetEndNode();

        // Формируем граф
        // Добавим все вершины
        foreach (var node in Nodes)
        {
            graph.AddNode(new Node(node.Title));
            if (node == start)
            {
                graph.Nodes.Last().SubState = Enums.NodeSubState.Start;
            }
            else if (node == end)
            {
                graph.Nodes.Last().SubState = Enums.NodeSubState.End;
            }
        }
        // Добавим все связи
        foreach (var edge in Edges)
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
    public void FromRaw(Graph graph)
    {
        var offset = 0;
        foreach (var node in graph.Nodes)
        {
            var n = NewNode(offset += 60, offset, node.Name);
            if (n != null) n.SubState = node.SubState;
        }
        foreach (var node in graph.Nodes)
        {
            foreach (var edge in node.Edges)
            {
                var l = GetNode(node.Name);
                var r = GetNode(edge.Right.Name);
                if (l == null || r == null) continue;
                if (!IsEdgeExists(l, r))
                    ConnectNodes(l, r, edge.Weight);
                else
                {
                    var e = Edges.Find(x => x.Left == l && x.Right == r);
                    if (e != null)
                    {
                        var ww = new string[2] { e.Weight, edge.Weight };
                        ww.ToList().Sort();
                        e.Weight = $"{string.Join(",", ww)}";
                    }
                }
            }
        }
    }

    // GRAPH EVENTS
    public bool _NodeSelecting_(GraphNodeControl node, bool ephemeral = false)
    {
        switch (SelectionMode)
        {
            case SelectionMode.Single:
                DeselectAllNodes();
                _NodeSelected_(node, ephemeral); 
                return true;
            case SelectionMode.Multiple:
                _NodeSelected_(node, ephemeral);
                return true;
            default:
                return false;
        }
    }
    public void _NodeSelected_(GraphNodeControl node, bool ephemeral = false)
    {
        node.Selected(ephemeral);
        NodeSelected?.Invoke(node);
    }

    // JSON CONVERTER
    public bool FromJson(StorageFile file)
    {
        Clear();



        Loaded?.Invoke(file.Path);
        return true;
    }
}
