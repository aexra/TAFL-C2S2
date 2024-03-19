using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Controls;

namespace TAFL.Classes.Graph;
public class CanvasedEdge
{
    public GraphNodeControl Left;
    public GraphNodeControl Right;
    public bool ToRight;
    public string Weight; // a,b,c,...,e

    public CanvasedEdge(GraphNodeControl left, GraphNodeControl right, bool toRight, string weight)
    {
        Left = left;
        Right = right;
        ToRight = toRight;
        Weight = weight;
    }
}
