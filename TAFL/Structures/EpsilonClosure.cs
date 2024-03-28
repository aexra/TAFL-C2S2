using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ABI.System.Collections.Generic;
using TAFL.Classes.Graph;

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
    
    public override string ToString()
    {
        var output = $"E({Origin.Name}) = {{ {Origin.Name}";
        foreach (var node in Nodes)
        {
            output += ", " + node.ToString();
        }
        output += " }";
        return output;
    }
}
