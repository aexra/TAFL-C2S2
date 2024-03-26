﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Classes.Graph;
using TAFL.Controls;
using TAFL.Interfaces;
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

    private void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        var graph = Constructor.GetRawGraph();
        if (graph.IsEmpty) return;

        LogService.Log(GetQTable(graph));
        GetTransitionsE(out var ets, out var closures);
        LogService.Log(ets);

        var alphabet = GetAlphabet();
        alphabet.Sort();
        var tableS = GetSTable(graph, closures, alphabet, out var slines);
        LogService.Log(tableS);

        

        
    }

    private string GetQTable(Graph graph)
    {
        return graph.ToString();
    }
    private string GetSTable(Graph graph, List<EpsilonClosure> closures, List<string> alphabet, out List<SLine> slines)
    {
        // Создаем пустую строку для вывода таблички
        var output = "Таблица S:";

        // Создаем пустой список записей S таблицы
        slines = new();

        // Заполняем пустыми S записями эквивалентными замыканиям
        foreach (var closure in closures)
        {
            slines.Add(new SLine($"S{slines.Count}", closure));
        }

        // Заполняем пути в каждой S записи
        foreach (var sline in slines)
        {
            output += $"\n";
            var localOutput = $"{sline.Name} = ";
            foreach (var letter in alphabet)
            {
                // Создадим в sline список SLine для этой литеры
                sline.Paths.Add(letter, new());

                // Вершины которые доступны по этой литере
                HashSet<Node> destinations = new();

                // Пройдем по всем начальным вершинам и соберем списки достижимых вершин
                foreach (var start in sline.Closure.GetAllNodes())
                {
                    // Список вершин в которые можно попасть через letter из start
                    GetDestinations(graph, start, letter, ref destinations);
                }

                // Куда аткуда
                //output += $"{letter}: " + SetToString(destinations) + "; ";

                // Теперь найдем какие SLine соответствуют этим вершинам
                foreach (var sline_ in slines)
                {
                    // Проверим что все вершины sline_ находятся в destination
                    var allFound = true;
                    foreach (var node in sline_.Closure.GetAllNodes())
                    {
                        allFound = destinations.Contains(node);
                    }
                    if (allFound)
                    {
                        sline.Paths[letter].Add(sline_);
                    }
                }

                // Выведем полученные штуки
                localOutput += $"{letter}: " + SetToString(sline.Paths[letter]) + "; ";
            }

            // Если эта S является начальной, отметим это
            var starts = GetStartNodes(graph, closures);
            var allF = true;
            foreach (var node in sline.Closure.GetAllNodes())
            {
                if (!starts.Contains(node))
                {
                    allF = false;
                    break;
                }
            }
            if (allF) sline.MakeStarting();

            output += (sline.IsStarting? "-> " : "     ") + localOutput;
        }

        return output;
    }
    private string GetPTable()
    {
        return string.Empty;
    }

    private void GetDestinations(Graph graph, Node start, string letter, ref HashSet<Node> visited)
    {
        foreach (var edge in start.Edges)
        {
            if (ParseWeights(edge.Weight).Contains(letter) || ParseWeights(edge.Weight).Contains("ε"))
            {
                if (visited.Contains(edge.Right)) continue;
                visited.Add(edge.Right);
                GetDestinations(graph, edge.Right, letter, ref visited);
            }
        }
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
            output += "\nE( " + node.Name + " ) = { " + node.Name;
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
    private string SetToString<T>(HashSet<T> set)
    {
        var list = new List<string>();
        set.ToList().ForEach(x => list.Add(x.ToString()));
        list.Sort();
        var s = "{ ";
        if (list.Count > 0)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                s += list.ElementAt(i).ToString() + ", ";
            }
            s += list.Last().ToString();
        }
        s += " }";
        return s;
    }
    private List<string> GetNodesNames(Graph graph)
    {
        // Отсортированный список имен вершин (первая будет начальной)
        List<string> names = new();
        graph.Nodes.ForEach(x => names.Add(x.Name));
        names.Sort();
        return names;
    }
    private List<Node> GetStartNodes(Graph graph, List<EpsilonClosure> closures)
    {
        // Начальная вершина
        var globalStartNode = graph.GetNode(GetNodesNames(graph).First());

        // Эпсилон-замыкание начальной вершины
        var startClosure = closures.Where(x => x.Origin.Name == globalStartNode.Name).First();

        // Вершины эпсилон-замыкания начальной вершины также являются начальными
        var startNodes = startClosure.GetAllNodes();

        return startNodes;
    }
}
