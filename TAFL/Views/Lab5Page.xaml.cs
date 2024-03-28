using System.Collections.Immutable;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using TAFL.Classes.Graph;
using TAFL.Controls;
using TAFL.Helpers;
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
        Output = new(OutputCanvas);
        //Output = new(OutputCanvas) { ReadOnly=true };

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
        Output.Clear();
    }

    private async void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        if (Constructor.GetStartNode() == null || Constructor.GetEndNode() == null)
        {
            await DialogHelper.ShowErrorDialogAsync("Определите начальную и конечную вершины", XamlRoot);
            return;
        }

        var graph = Constructor.GetRawGraph();
        if (graph.IsEmpty) return;

        GraphDeterminizationService.GetDeterminizedGraph(graph, out var process);

        LogService.Log(process);

        //GetTransitionsE(out var ets, out var closures);
        //LogService.Log(ets);

        //var alphabet = GetAlphabet();
        //alphabet.Sort();
        //var tableS = GetSTable(graph, closures, alphabet, out var slines);
        //LogService.Log(tableS);

        //var tableP = GetPTable(graph, slines, out var plines);
        //LogService.Log(tableP);

        //var offset = 0;
        //Output.Clear();
        //foreach (var pline in plines)
        //{
        //    Output.NewNode(offset += 60, offset, pline.Name);
        //}

        //foreach (var pline in plines)
        //{
        //    foreach (var letter in pline.Paths.Keys)
        //    {
        //        if (pline.Paths[letter].Count == 0) continue;
        //        Output.ConnectNodes(Output.GetNode(pline.Name) , Output.GetNode(pline.Paths[letter].First().Name),letter);
        //    }
        //}
    }

    private string GetQTable(Graph graph)
    {
        return graph.ToString();
    }
    private string GetSTable(Graph graph, List<EpsilonClosure> closures, List<string> alphabet, out List<SLine> slines)
    {
        /// Создаем пустую строку для вывода таблички
        var output = "Таблица S:";

        /// Создаем пустой список записей S таблицы
        slines = new();

        /// Заполняем пустыми S записями эквивалентными замыканиям
        foreach (var closure in closures)
        {
            slines.Add(new SLine($"S{slines.Count}", closure));
        }

        /// Заполняем пути в каждой S записи
        for (var i = 0; i < slines.Count; i++)
        {
            var sline = slines[i];
            output += $"\n";
            var localOutput = $"{sline.Name}{SetHelper.SetToString(sline.Closure.GetAllNodes().ToHashSet())} = ";
            foreach (var letter in alphabet)
            {
                /// Создадим в sline список SLine для этой литеры
                sline.Paths.Add(letter, new());

                /// Вершины которые доступны по этой литере
                HashSet<Node> destinations = new();

                /// Пройдем по всем начальным вершинам и соберем списки достижимых вершин
                foreach (var start in sline.Closure.GetAllNodes())
                {
                    /// Список вершин в которые можно попасть через letter из start
                    GetDestinations(graph, start, letter, ref destinations);
                }

                // Куда аткуда
                //output += $"{letter}: " + SetToString(destinations) + "; ";

                /// Теперь найдем какие SLine соответствуют этим вершинам
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

                /// Выведем полученные штуки
                localOutput += $"{letter}: " + SetHelper.SetToString(sline.Paths[letter]) + "; ";
            }

            /// Если эта S является начальной, отметим это
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
            if (allF) slines[i] = new SLine(slines[i].Name, slines[i].Closure) { Paths = slines[i].Paths, IsStart = true };
            output += (slines[i].IsStart? "-> " : GetEndSLines(graph, slines).Contains(sline) ? "<- " : "     ") + localOutput;
        }
        return output;
    }
    private string GetPTable(Graph graph, List<SLine> slines, out List<PLine> plines)
    {
        var output = $"Таблица P:";

        plines = new();
        var alphabet = GetAlphabet();
        alphabet.Sort();

        /// Получим начальные S
        var starts = GetStartSLines(graph, slines);

        ///  Создание начальной P-вершины
        var p0 = new PLine("P0", starts);
        plines.Add(p0);

        /// Заполняем список вершин P
        while (true)
        {
            var changed = false;

            foreach (var pline in plines)
            {
                foreach (var letter in alphabet)
                {
                    if (!pline.Paths.ContainsKey(letter))
                    {
                        FillPLinesList(ref plines, pline, slines);
                        changed = true;
                        break;
                    }
                }
                if (changed) break; 
            }

            if (!changed) break;
        }

        /// Получим начальные P
        var starts_p = GetStartPLines(graph, slines, plines);
        var ends_p = GetEndPLines(graph, slines, plines);

        /// Формирование таблицы P вершин
        foreach (var pline in plines)
        {
            output += $"\n{(starts_p.Contains(pline) ? "-> " : ends_p.Contains(pline) ? "<- " : "")}{pline.Name}{SetHelper.SetToString(pline.Slines)} = ";
            var keys = pline.Paths.Keys.ToList();
            keys.Sort();
            foreach (var letter in keys)
            {
                if (pline.Paths.ContainsKey(letter)) output += $"{letter}: {SetHelper.SetToString(pline.Paths[letter])}; ";
            }
        }

        return output;
    }

    private HashSet<SLine> GetStartSLines(Graph graph, List<SLine> slines)
    {
        HashSet<SLine> starts = new();
        foreach (var sline in slines)
        {
            if (sline.IsStart) starts.Add(sline);
        }
        return starts;
    }
    private HashSet<PLine> GetStartPLines(Graph graph, List<SLine> slines, List<PLine> plines)
    {
        HashSet<PLine> starts_p = new();

        var starts_s = GetStartSLines(graph, slines);
        foreach (var pline in plines)
        {
            var good = true;
            foreach (var sline in pline.Slines)
            {
                if (!starts_s.Contains(sline))
                {
                    good = false;
                    break;
                }
            }
            if (good) starts_p.Add(pline);
        }

        return starts_p;
    }

    private HashSet<SLine> GetEndSLines(Graph graph, List<SLine> slines)
    {
        HashSet<SLine> ends = new();
        var end = graph.GetEndNode();
        if (end == null) return ends;
        foreach (var sline in slines)
        {
            if (sline.Closure.GetAllNodes().Exists(x => x.Name == end.Name))
            {
                ends.Add(sline);
            }
        }
        return ends;
    }
    private HashSet<PLine> GetEndPLines(Graph graph, List<SLine> slines, List<PLine> plines)
    {
        HashSet<PLine> ends_p = new();
        var ends_s = GetEndSLines(graph, slines);
        foreach (var pline in plines)
        {
            var found = false;
            foreach (var end in ends_s)
            {
                if (pline.Slines.Contains(end))
                {
                    found = true;
                    break;
                }
            }
            if (found) ends_p.Add(pline);
        }
        return ends_p;
    }

    private void FillPLinesList(ref List<PLine> plines, PLine start_p, List<SLine> allSlines)
    {
        /// Получение алфавита
        var alphabet = GetAlphabet();
        alphabet.Sort();

        /// Получает все достижимые SLines для всех литер в этом PLine
        foreach (var letter in alphabet)
        {
            /// Получает все достижимые SLines для литеры letter в этом PLine
            HashSet<SLine> dists = new();
            foreach (var start_s in start_p.Slines)
            {
                var this_start_dests = GetSLineDestinations(start_s, letter, allSlines);
                if (this_start_dests != null) this_start_dests.ToList().ForEach(x => dists.Add(x));
            }

            /// Проверим существует ли такой PLine в plines
            if (SLinesSetExists(plines, dists, out var target))
            {
                if (!start_p.Paths.ContainsKey(letter)) start_p.Paths.Add(letter, new());
                start_p.Paths[letter].Add(target);
            }
            else
            {
                if (dists.Count == 0)
                {
                    if (!start_p.Paths.ContainsKey(letter)) start_p.Paths.Add(letter, new());
                    continue;
                }
                var new_p = new PLine($"P{plines.Count}", dists);
                plines.Add(new_p);
                FillPLinesList(ref plines, new_p, allSlines);
            }
        }
    }
    private bool SLinesSetExists(List<PLine> plines, HashSet<SLine> slines, out PLine? target)
    {
        target = null;
        foreach (var pline in plines)
        {
            if (pline.Slines.Count != slines.Count) continue;
            var foundAllSlines = true;
            foreach (var sline in slines)
            {
                if (!pline.Slines.Contains(sline))
                {
                    foundAllSlines = false;
                    break;
                }
            }
            if (foundAllSlines)
            {
                target = pline;
                return true;
            }
        }
        return false;
    }
    private HashSet<SLine>? GetSLineDestinations(SLine start, string letter, List<SLine> slines)
    {
        foreach (var sline in slines)
        {
            if (sline == start)
            {
                return sline.Paths[letter];
            }
        }
        return null;
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
        var globalStartNode = graph.GetStartNode();

        // Эпсилон-замыкание начальной вершины
        var startClosure = closures.Where(x => x.Origin.Name == globalStartNode.Name).First();

        // Вершины эпсилон-замыкания начальной вершины также являются начальными
        var startNodes = startClosure.GetAllNodes();

        return startNodes;
    }

    private void CheckWordButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
