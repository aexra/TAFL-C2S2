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
    public override string ToString()
    {
        var output = "Граф";

        foreach (var node in Nodes)
        {
            output += $"\n{node.Name}: ";
            foreach (var edge in node.Edges)
            {
                output += $"{edge.Right.Name}[{edge.Weight}] ";
            }
        }

        return output;
    }
}
