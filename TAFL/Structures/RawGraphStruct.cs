using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Structures;
public struct RawGraphStruct
{
    public Dictionary<string, int[]> Nodes;
    public string[][] Edges;
    public Dictionary<string, string> States;
}
