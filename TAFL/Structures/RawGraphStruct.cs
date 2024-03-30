using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Structures;
public struct RawGraphStruct
{
    public Dictionary<string, float[]> Nodes;
    public List<string[]> Edges;
    public Dictionary<string, string> States;
}
