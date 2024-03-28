using TAFL.Classes.Graph;
using TAFL.Structures;

namespace TAFL.Services;
public static class GraphDeterminizationService
{
    public static Graph GetDeterminizedGraph(Graph graph, out string process)
    {
        process = "Детерминизация графа\n\n" + graph.ToString() + "\n\n";

        var closures = GetEpsilonClosures(graph);
        process += "Эпсилон-замыкания\n" + closures.ToString(true) + "\n\n";

        var slines = GetSLines(graph, closures);
        process += "S-таблица\n" + slines.ToString(true) + "\n\n";

        return new();
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
            if (allF) slines[i] = new SLine(slines[i].Name, slines[i].Closure) { Paths = slines[i].Paths, IsStart = true };
            if (GetEndSLines(graph, slines).Contains(slines[i])) slines[i] = new SLine(slines[i].Name, slines[i].Closure) { Paths = slines[i].Paths, IsEnd = true };
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
    private static string ToString(this List<EpsilonClosure> closures, bool enumerate)
    {
        var output = "";

        var counter = 0;
        foreach (var cl in closures) 
        {
            output += counter == 0 ? cl.ToLongString() : ('\n' + cl.ToString());
            counter++;
        }

        return output;
    }
    private static string ToString(this List<SLine> slines, bool enumerate)
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
}
