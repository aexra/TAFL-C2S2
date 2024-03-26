using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Structures;
public struct SLine
{
    public string Name;
    public EpsilonClosure Closure;
    public Dictionary<string, List<SLine>> Paths;

    public SLine(string name, EpsilonClosure closure)
    {
        Name = name;
        Closure = closure;
        Paths = new();
    }
}
