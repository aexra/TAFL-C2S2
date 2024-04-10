using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ABI.System.Collections.Generic;
using CanvasedGraph.Raw;
using TAFL.Services;

namespace TAFL.Structures;
public struct EpsilonClosure
{
    public string Name;
    public Node Origin;
    public List<Node> Nodes;

    public EpsilonClosure(string name, Node origin, List<Node> nodes)
    {
        Name = name;
        Origin = origin;
        Nodes = nodes;
    }
    public List<Node> GetAllNodes()
    {
        List<Node> nodes = new();
        foreach (var node in Nodes)
        {
            nodes.Add(node);
        }
        nodes.Add(Origin);
        return nodes;
    }

    public override string ToString() => Name;
    public string ToLongString()
    {
        var output = $"E({Origin.Name}) = {{ ";
        var sorted_nodes = GetAllNodes();
        sorted_nodes.Sort();
        var counter = 0;
        foreach (var node in sorted_nodes)
        {
            output += counter == 0 ? node.ToString() : (", " + node.ToString());
            counter++;
        }
        output += " }";
        return output;
    }
}
