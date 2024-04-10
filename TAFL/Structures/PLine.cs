using CanvasedGraph.Enums;
using TAFL.Helpers;

namespace TAFL.Structures;
public class PLine
{
    public string Name;
    public HashSet<SLine> Slines;
    public Dictionary<string, HashSet<PLine>> Paths;
    public NodeSubState SubState;

    public PLine(string name, HashSet<SLine> slines)
    {
        Name = name;
        Slines = slines;
        Paths = new();
        SubState = NodeSubState.Default;
    }

    public override string ToString() => Name;
    public string ToLongString()
    {
        var output = $"{(SubState == NodeSubState.Start ? "-> " : SubState == NodeSubState.End ? "<- " : SubState == NodeSubState.Universal ? "<>" : "")}{Name}{SetHelper.SetToString(Slines)} =";

        var sorted_keys = Paths.Keys.ToList();
        sorted_keys.Sort();
        foreach (var letter in sorted_keys)
        {
            output += $" {letter}: {SetHelper.SetToString(Paths[letter])};";
        }

        return output;
    }
}
