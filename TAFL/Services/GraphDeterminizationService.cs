using TAFL.Classes.Graph;
using TAFL.Helpers;
using TAFL.Structures;

namespace TAFL.Services;
public static class GraphDeterminizationService
{
    public static Graph GetDeterminizedGraph(Graph graph, out string process, out Graph intermediateState)
    {
        process = "Детерминизация графа\n\n" + graph.ToLongString() + "\n\n";

        var closures = GetEpsilonClosures(graph);
        process += "Эпсилон-замыкания\n" + closures.ToLongString() + "\n\n";

        var slines = GetSLines(graph, closures);
        process += "S-таблица\n" + slines.ToLongString() + "\n\n";

        intermediateState = new Graph();
        foreach (var sline in slines)
        {
            var n = new Node(sline.Name);
            n.SubState = sline.SubState;
            intermediateState.AddNode(n);
        }
        foreach (var sline in slines)
        {
            foreach (var letter in sline.Paths.Keys)
            {
                if (sline.Paths[letter].Count == 0) continue;
                foreach (var path in sline.Paths[letter])
                {
                    intermediateState.Connect(sline.Name, path.Name, letter);
                }
            }
        }

        var plines = GetPLines(graph, slines);
        process += "P-таблица\n" + plines.ToLongString();

        Graph output = new();
        foreach (var pline in plines)
        {
            var n = new Node(pline.Name);
            n.SubState = pline.SubState;
            output.AddNode(n);
        }
        foreach (var pline in plines)
        {
            foreach (var letter in pline.Paths.Keys)
            {
                if (pline.Paths[letter].Count == 0) continue;
                foreach (var path in pline.Paths[letter])
                {
                    output.Connect(pline.Name, path.Name, letter);
                }
            }
        }

        return output;
    }

    // P
    private static List<PLine> GetPLines(Graph graph, List<SLine> slines)
    {
        List<PLine> plines = new();
        var alphabet = graph.GetWeightsAlphabet();

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
                        FillPLinesList(graph, ref plines, pline, slines);
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

        /// Отметим если P является начальным или конечным
        for (var i = 0; i < plines.Count; i++)
        {
            var startable = starts_p.Contains(plines[i]);
            var endable = ends_p.Contains(plines[i]);
            if (startable && endable) plines[i].SubState = Enums.NodeSubState.Universal;
            else if (startable) plines[i].SubState = Enums.NodeSubState.Start;
            else if (endable) plines[i].SubState = Enums.NodeSubState.End;
        }

        return plines;
    }
    private static void FillPLinesList(Graph graph, ref List<PLine> plines, PLine start_p, List<SLine> allSlines)
    {
        /// Получение алфавита
        var alphabet = graph.GetWeightsAlphabet();
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
            if (SLinesSetExists(graph, plines, dists, out var target))
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
                FillPLinesList(graph, ref plines, new_p, allSlines);
            }
        }
    }
    private static HashSet<PLine> GetStartPLines(Graph graph, List<SLine> slines, List<PLine> plines)
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
    private static HashSet<PLine> GetEndPLines(Graph graph, List<SLine> slines, List<PLine> plines)
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

    // S
    private static List<SLine> GetSLines(Graph graph, List<EpsilonClosure>? closures = null)
    {
        /// Пустой список для заполнения
        List<SLine> slines = new();

        /// Алфавит
        var alphabet = graph.GetWeightsAlphabet();
        
        /// Если не передали заранее, то нужно вычислить все замыкания
        closures ??= GetEpsilonClosures(graph);

        /// Заполняем пустыми S записями эквивалентными замыканиям
        foreach (var closure in closures)
        {
            slines.Add(new SLine($"S{slines.Count}", closure));
        }

        /// Заполняем пути в каждой S записи
        for (var i = 0; i < slines.Count; i++)
        {
            foreach (var letter in alphabet)
            {
                /// Создадим в sline список SLine для этой литеры
                slines[i].Paths.Add(letter, new());

                /// Вершины которые доступны по этой литере
                HashSet<Node> destinations = new();

                /// Пройдем по всем начальным вершинам и соберем списки достижимых вершин
                foreach (var start in slines[i].Closure.GetAllNodes())
                {
                    /// Список вершин в которые можно попасть через letter из start
                    GetDestinations(graph, start, letter, ref destinations);
                }

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
                        slines[i].Paths[letter].Add(sline_);
                    }
                }
            }

            /// Если эта S является начальной, отметим это
            var starts = GetStartNodes(graph, closures);
            var allF = true;
            foreach (var node in slines[i].Closure.GetAllNodes())
            {
                if (!starts.Contains(node))
                {
                    allF = false;
                    break;
                }
            }
            var isEnd = GetEndSLines(graph, slines).Contains(slines[i]);
            if (allF && isEnd) slines[i].SubState = Enums.NodeSubState.Universal;
            else if (allF) slines[i].SubState = Enums.NodeSubState.Start;
            else if (isEnd) slines[i].SubState = Enums.NodeSubState.End;
        }

        return slines;
    }
    private static void GetDestinations(Graph graph, Node start, string letter, ref HashSet<Node> visited)
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
    private static HashSet<SLine> GetStartSLines(Graph graph, List<SLine> slines)
    {
        HashSet<SLine> starts = new();
        foreach (var sline in slines)
        {
            if (sline.SubState == Enums.NodeSubState.Start || sline.SubState == Enums.NodeSubState.Universal) starts.Add(sline);
        }
        return starts;
    }
    private static HashSet<SLine> GetEndSLines(Graph graph, List<SLine> slines)
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
    private static HashSet<SLine>? GetSLineDestinations(SLine start, string letter, List<SLine> slines)
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
    private static bool SLinesSetExists(Graph graph, List<PLine> plines, HashSet<SLine> slines, out PLine? target)
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

    // EPS
    private static List<EpsilonClosure> GetEpsilonClosures(Graph graph)
    {
        List<EpsilonClosure> closures = new();
        var counter = -1;
        foreach (var node in graph.Nodes)
        {
            closures.Add(new($"E{++counter}", node, new()));
            foreach (var edge in node.Edges)
            {
                if (ParseWeights(edge.Weight).Contains("ε"))
                {
                    closures.Last().Nodes.Add(edge.Right);
                }
            }
        }
        return closures;
    }

    // NODES
    private static List<Node> GetStartNodes(Graph graph, List<EpsilonClosure> closures)
    {
        // Начальная вершина
        var globalStartNode = graph.GetStartNode();

        // Эпсилон-замыкание начальной вершины
        var startClosure = closures.Where(x => x.Origin.Name == globalStartNode.Name).First();

        // Вершины эпсилон-замыкания начальной вершины также являются начальными
        var startNodes = startClosure.GetAllNodes();

        return startNodes;
    }

    // HELPERS
    private static string[] ParseWeights(string weight, string separator = ",") => weight.Split(separator);

    // ToString extensions
    private static string ToLongString(this List<EpsilonClosure> closures)
    {
        var output = "";

        var counter = 0;
        foreach (var cl in closures) 
        {
            output += counter == 0 ? cl.ToLongString() : ('\n' + cl.ToLongString());
            counter++;
        }

        return output;
    }
    private static string ToLongString(this List<SLine> slines)
    {
        var output = "";
        
        var counter = 0;
        foreach (var sline in slines)
        {
            output += counter == 0? sline.ToLongString() : ('\n' + sline.ToLongString());
            counter++;
        }

        return output;
    }
    private static string ToLongString(this List<PLine> plines)
    {
        var output = "";

        var counter = 0;
        foreach (var pline in plines)
        {
            output += counter == 0 ? pline.ToLongString() : ('\n' + pline.ToLongString());
            counter++;
        }

        return output;
    }
}
