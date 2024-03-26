﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Classes.Graph;
using TAFL.Controls;
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
        var graph = Constructor.GetRawGraph();
        LogService.Log(GetQTable(graph));
        GetTransitionsE(out var ets, out var closures);
        LogService.Log(ets);

        var alphabet = GetAlphabet();
        var tableS = GetSTable(graph, closures, alphabet);
        LogService.Log(tableS);
    }

    private string GetQTable(Graph graph)
    {
        return graph.ToString();
    }
    private string GetSTable(Graph graph, List<EpsilonClosure> closures, List<string> alphabet)
    {
        // Создаем пустой список записей S таблицы
        List<SLine> slines = new();

        // Заполняем пустыми S записями эквивалентными замыканиям
        foreach (var closure in closures)
        {
            slines.Add(new SLine($"S{slines.Count}", closure));
        }

        // Заполняем пути в каждой S записи
        foreach (var sline in slines)
        {
            foreach (var letter in alphabet)
            {
                // Создадим пустой список путей для текущего веса
                List<SLine> paths = new();

                // Пройдем по всем начальным вершинам и соберем списки достижимых вершин
                foreach (var start in sline.Closure.Nodes)
                {
                    // Список вершин в которые можно попасть через letter из start
                    var nodes = GetDestinations(graph, start, letter, null);

                }

                // Добавим к путям этой записи все пути по весу letter
                sline.Paths.Add(letter, paths);
            }
        }

        return string.Empty;
    }
    private string GetPTable()
    {
        return string.Empty;
    }

    private HashSet<Node> GetDestinations(Graph graph, Node start, string letter, HashSet<Node>? visited)
    {
        visited ??= new HashSet<Node>();
        foreach (var edge in start.Edges)
        {
            if (!visited.Contains(edge.Right) && (ParseWeights(edge.Weight).Contains(letter) || ParseWeights(edge.Weight).Contains("ε")))
            {
                visited.Union(GetDestinations(graph, edge.Right, letter, visited));
            }
        }
        return visited;
    }
    private EpsilonClosure? GetEpsilonClosure(List<EpsilonClosure> closures, List<Node> nodes)
    {
        foreach (var closure in closures)
        {
            if (closure.Nodes.Count == nodes.Count)
            {
                var ok = true;
                foreach (var node in nodes)
                {
                    if (!closure.Nodes.Contains(node))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) return closure;
            }
        }
        return null;
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
