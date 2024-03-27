using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Structures;
public struct PLine
{
    public string Name;
    public List<SLine> Slines;
    public Dictionary<string, HashSet<PLine>> Paths;

    public PLine(string name, List<SLine> slines)
    {
        Name = name;
        Slines = slines;
        Paths = new();
    }
}
