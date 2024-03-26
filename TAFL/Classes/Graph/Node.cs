using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class Node : INode
{
    public IGraph? Graph;
    public string Name;
    public List<Edge> Edges;


    public Node(string name)
    {
        Name = name;
        Edges = new();
    }
    
    public void Connect(Node to, string weight, bool isOriented)
    {
        if (isOriented)
        {
            Edges.Add(new OrientedEdge(this, to, weight));
        }
        else
        {
            var edge = new Edge(this, to, weight);
            Edges.Add(edge);
            ((Node)to).Edges.Add(edge);
        }
    }
    public void Disconnect(Node node)
    {
        foreach (var edge in Edges)
        {
            if (edge.Right == node)
            {
                if (edge is OrientedEdge oe)
                {
                    Edges.Remove(oe);
                }
                else
                {
                    Edges.Remove(edge);
                    edge.Right.Edges.Remove(edge);
                }
            }
        }
    }
    public void Delete()
    {
        Graph.RemoveNode(this);
    }

    public override string ToString() => Name;
}
