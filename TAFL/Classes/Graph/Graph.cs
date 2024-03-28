using Newtonsoft.Json.Linq;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class Graph : IGraph
{
    public List<Node> Nodes;
    public bool IsEmpty => Nodes.Count == 0;

    public Graph()
    {
        Nodes = new();
    }

    public void Connect(string left, string right, string w)
    {
        var l = GetNode(left);
        var r = GetNode(right);
        if (l == null || r == null) return;
        l.Connect(r, w, true);
    }
    public void AddNode(Node node)
    {
        Nodes.Add(node);
        node.Graph = this;
    }
    public void RemoveNode(Node node)
    {
        Nodes.Remove(node);
    }
    public void Clear()
    {
        Nodes.Clear();
    }
    public Node? GetNode(string name)
    {
        foreach (Node node in Nodes)
        {
            if (node.Name == name) return node;
        }
        return null;
    }
    public bool IsConnectionExists(Node node1, Node node2)
    {
        foreach (var edge in node1.Edges)
        {
            if (edge.Right == node2) return true;
        }
        return false;
    }
    public bool IsConnectionExists(string name1, string name2)
    {
        var node1 = GetNode(name1);
        var node2 = GetNode(name2);
        foreach (var edge in node1.Edges)
        {
            if (edge.Right == node2) return true;
        }
        return false;
    }
    public Node? GetStartNode()
    {
        foreach (var node in Nodes)
        {
            if (node.SubState == Enums.NodeSubState.Start) return node;
        }
        return null;
    }
    public Node? GetEndNode()
    {
        foreach (var node in Nodes)
        {
            if (node.SubState == Enums.NodeSubState.End) return node;
        }
        return null;
    }
    public List<string> GetWeightsAlphabet(string separator = ",")
    {
        List<string> alphabet = new();

        foreach (var node in Nodes)
        {
            foreach (var edge in node.Edges)
            {
                var letters = edge.Weight.Split(separator);
                foreach (var letter in letters)
                {
                    if (letter != "ε" && !alphabet.Contains(letter))
                    {
                        alphabet.Add(letter);
                    }
                }
            }
        }

        alphabet.Sort();

        return alphabet;
    }
    public string ToLongString()
    {
        var output = "Граф";

        foreach (var node in Nodes)
        {
            output += $"\n{node.Name}:";
            var sorted_edges = node.Edges;
            sorted_edges.Sort();
            foreach (var edge in sorted_edges)
            {
                output += $" {edge.Right.Name}({edge.Weight});";
            }
        }

        return output;
    }
}
