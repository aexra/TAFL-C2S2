using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class Graph : IGraph
{
    public List<Node> Nodes;

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
}
