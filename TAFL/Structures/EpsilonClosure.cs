using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
