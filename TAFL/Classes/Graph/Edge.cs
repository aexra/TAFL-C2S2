using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class Edge : IEdge, IComparable<Edge>
{
    public Node Left;
    public Node Right;
    public string Weight;
    public bool IsLoop => Left == Right;

    public Edge(Node left, Node right, string weight)
    {
        Left = left;
        Right = right;
        Weight = weight;
    }

    public void Remove()
    {
        
    }

    public int CompareTo(Edge? other)
    {
        if (other == null) return -1;
        return Right.Name.CompareTo(other.Right.Name);
    }
}
