using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TAFL.Classes.Graph;
using TAFL.Structures;
using Windows.Graphics.Display;

namespace TAFL.Services;
public static class GraphDeterminizationService
{
    public static Graph GetDeterminizedGraph(Graph graph, out string process)
    {
        process = "Детерминизация графа\n\n" + graph.ToString() + "\n\n";

        var closures = GetEpsilonClosures(graph);
        process += "Эпсилон-замыкания\n" + closures.ToString(true) + "\n\n";


        return new();
    }

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
    private static string[] ParseWeights(string weight, string separator = ",") => weight.Split(separator);

    public static string ToString(this List<EpsilonClosure> closures, bool inline)
    {
        var output = "";
        var counter = 0;
        foreach (var cl in closures) 
        {
            output += counter == 0 ? cl.ToString() : ('\n' + cl.ToString());
            counter++;
        }
        return output;
    }
}
