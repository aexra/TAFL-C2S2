﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Classes.Graph;
using TAFL.Services;
using TAFL.Structures;
using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    private readonly CanvasedGraph Constructor;
    private readonly CanvasedGraph Output;
    public Lab5ViewModel ViewModel
    {
        get;
    }

    // CONSTRUCTOR
    public Lab5Page()
    {
        ViewModel = App.GetService<Lab5ViewModel>();
        InitializeComponent();

        Constructor = new(ConstructorCanvas);
        Output = new(OutputCanvas) { ReadOnly=true };

        Constructor.NodeCreated += Constructor_NodeCreated;
        Constructor.NodeRemoved += Constructor_NodeRemoved;
        Constructor.EdgeCreated += Constructor_EdgeCreated;
        Constructor.EdgeRemoved += Constructor_EdgeRemoved;
        Constructor.GraphCleared += Constructor_GraphCleared;
    }

    private void Constructor_NodeCreated(Controls.GraphNodeControl node)
    {
        LogService.Log($"Создана вершина {node.Title}");
    }
    private void Constructor_NodeRemoved(Controls.GraphNodeControl node)
    {
        LogService.Log($"Удалена вершина {node.Title}");
    }
    private void Constructor_EdgeCreated(CanvasedEdge edge)
    {
        LogService.Log(edge.Left != edge.Right ? $"Соединены вершины {edge.Left.Title} и {edge.Right.Title}" : $"Создана петля в {edge.Left.Title}");
    }
    private void Constructor_EdgeRemoved(CanvasedEdge edge)
    {
        LogService.Log(edge.Left != edge.Right? $"Удалено ребро между {edge.Left.Title} и {edge.Right.Title}" : $"Удалена петля в {edge.Left.Title}");
    }
    private void Constructor_GraphCleared()
    {
        LogService.Log($"Граф очищен");
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Constructor.Clear();
    }

    private void BuildQButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(GetQTable());
    }
    private void BuildSButton_Click(object sender, RoutedEventArgs e)
    {

    }
    private void BuildPButton_Click(object sender, RoutedEventArgs e)
    {

    }
    private void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(GetQTable());
        GetTransitionsE(out var ets, out var locks);
        LogService.Log(ets);

        var alphabet = GetAlphabet();
        var tableS = GetSTable(locks, alphabet);
    }

    private string GetQTable()
    {
        return Constructor.GetRawGraph().ToString();
    }
    private string GetSTable(List<EpsilonClosure> closures, List<string> alphabet)
    {
        return string.Empty;
    }
    private string GetPTable()
    {
        return string.Empty;
    }

    private List<Edge> GetTransitionsE(out string output, out List<EpsilonClosure> closures)
    {
        List<Edge> edgesE = new();
        output = "Эпсилон замыкания";
        closures = new();

        var graph = Constructor.GetRawGraph();
        var counter = -1;
        foreach (var node in graph.Nodes)
        {
            closures.Add(new($"S{++counter}", node, new()));
            output += "\n( " + node.Name + " ) = { " + node.Name;
            foreach (var edge in node.Edges)
            {
                if (ParseWeights(edge.Weight).Contains("ε"))
                {
                    closures.Last().Nodes.Add(edge.Right);
                    edgesE.Add(edge);
                    output += $", {edge.Right.Name}";
                }
            }
            output += " }";
        }

        return edgesE;
    }
    private string[] ParseWeights(string weight) => weight.Split(',');
    private List<string> GetAlphabet()
    {
        List<string> alphabet = new();

        foreach (var node in Constructor.GetRawGraph().Nodes)
        {
            foreach (var edge in node.Edges)
            {
                var letters = ParseWeights(edge.Weight);
                foreach (var letter in letters)
                {
                    if (letter != "ε" && !alphabet.Contains(letter))
                    {
                        alphabet.Add(letter);
                    }
                }
            }
        }

        return alphabet;
    }
}
