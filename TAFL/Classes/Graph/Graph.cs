using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class Graph : IGraph
{
    public List<IGraphNode> Nodes { private set; get; }
}
