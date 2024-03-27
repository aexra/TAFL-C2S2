using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Structures;
public struct SLine
{
    public string Name;
    public bool IsStarting;
    public EpsilonClosure Closure;
    public Dictionary<string, HashSet<SLine>> Paths;

    public SLine(string name, EpsilonClosure closure)
    {
        Name = name;
        IsStarting = false;
        Closure = closure;
        Paths = new();
    }

    public override string ToString() => Name;
    public void MakeStarting()
    {
        IsStarting = true;
    }

    public static bool operator == (SLine left, SLine right)
    {
        return left.Name == right.Name;
    }
    public static bool operator != (SLine left, SLine right)
    {
        return left.Name != right.Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is SLine sline && sline.Name == this.Name) {
            return true;
        }
        return false;
    }
}
