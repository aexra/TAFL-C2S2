using CanvasedGraph.Enums;
using TAFL.Helpers;

namespace TAFL.Structures;
public class SLine
{
    public string Name;
    public NodeSubState SubState;
    public EpsilonClosure Closure;
    public Dictionary<string, HashSet<SLine>> Paths;

    public SLine(string name, EpsilonClosure closure)
    {
        Name = name;
        SubState = NodeSubState.Default;
        Closure = closure;
        Paths = new();
    }

    public override string ToString() => Name;
    public string ToLongString()
    {
        var output = $"{(SubState == NodeSubState.Start ? "-> " : SubState == NodeSubState.End ? "<- " : SubState == NodeSubState.Universal ? "<>" : "")}{Name}";

        output += SetHelper.SetToString(Closure.GetAllNodes().ToHashSet()) + " =";
        foreach (var letter in Paths.Keys)
        {
            output += $" {letter}: {SetHelper.SetToString(Paths[letter])};";
        }

        return output;
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
