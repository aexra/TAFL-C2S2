using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TAFL.Helpers;

namespace TAFL.Structures;
public struct SLine
{
    public string Name;
    public bool IsStart;
    public bool IsEnd;
    public EpsilonClosure Closure;
    public Dictionary<string, HashSet<SLine>> Paths;

    public SLine(string name, EpsilonClosure closure)
    {
        Name = name;
        IsStart = false;
        IsEnd = false;
        Closure = closure;
        Paths = new();
    }

    public override string ToString() => Name;
    public string ToLongString()
    {
        var output = $"{(IsStart ? "-> " : IsEnd ? "<- " : "")}{Name}";

        output += SetHelper.SetToString(Closure.GetAllNodes().ToHashSet()) + " =";
        foreach (var letter in Paths.Keys)
        {
            output += $" {letter}: {SetHelper.SetToString(Paths[letter])};";
        }

        return output;
    }
    public void MakeStarting()
    {
        IsStart = true;
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
