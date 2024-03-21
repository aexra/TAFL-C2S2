using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
