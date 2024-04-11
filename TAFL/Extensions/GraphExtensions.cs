using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanvasedGraph.Raw;

namespace TAFL.Extensions;
public static class GraphExtensions
{
    public static List<Node> RemoveUnreachableNodesByAlphabetTransitions(this Graph graph, int depth = 10)
    {
        var removed = new List<Node>();
        var alphabet = graph.GetWeightsAlphabet();

        void PushLetter(Node node, string letter, ref HashSet<Node> visited, int depth)
        {
            visited.Add(node);
            if (depth < 1) return;
            foreach (var next in node.Edges.Where(e => e.Weight.Contains(letter)).Select(e => e.Right))
            {
                PushLetter(next, letter, ref visited, depth - 1);
            }
        }

        var visited = new HashSet<Node>();
        foreach (var letter in alphabet)
        {
            foreach (var start_node in graph.Nodes.Where(n => n.SubState == CanvasedGraph.Enums.NodeSubState.Start))
            {
                PushLetter(start_node, letter, ref visited, depth);
            }
        }

        foreach (var node in graph.Nodes)
        {
            if (!visited.Contains(node))
            {
                removed.Add(node);
            }
        }

        foreach (var remove in removed)
        {
            remove.Delete();
        }

        return removed;
    }
    public static List<Node> RemoveUnreachableNodesByTransitions(this Graph graph, int depth = 10)
    {
        var removed = new List<Node>();
        var alphabet = graph.GetWeightsAlphabet();

        void PushLetter(Node node, ref HashSet<Node> visited, int depth)
        {
            visited.Add(node);
            if (depth < 1) return;
            foreach (var next in node.Edges.Select(e => e.Right))
            {
                PushLetter(next, ref visited, depth - 1);
            }
        }

        var visited = new HashSet<Node>();
        foreach (var letter in alphabet)
        {
            foreach (var start_node in graph.Nodes.Where(n => n.SubState == CanvasedGraph.Enums.NodeSubState.Start))
            {
                PushLetter(start_node, ref visited, depth);
            }
        }

        foreach (var node in graph.Nodes)
        {
            if (!visited.Contains(node))
            {
                removed.Add(node);
            }
        }

        foreach (var remove in removed)
        {
            remove.Delete();
        }

        return removed;
    }
}
