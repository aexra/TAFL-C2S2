using TAFL.Helpers;

namespace TAFL.Structures;
public class PLine
{
    public string Name;
    public HashSet<SLine> Slines;
    public Dictionary<string, HashSet<PLine>> Paths;
    public bool IsStart;
    public bool IsEnd;

    public PLine(string name, HashSet<SLine> slines)
    {
        Name = name;
        Slines = slines;
        Paths = new();
        IsStart = false;
        IsEnd = false;
    }

    public override string ToString() => Name;
    public string ToLongString()
    {
        var output = $"{(IsStart ? "-> " : IsEnd ? "<- " : "")}{Name}{SetHelper.SetToString(Slines)} =";

        var sorted_keys = Paths.Keys.ToList();
        sorted_keys.Sort();
        foreach (var letter in sorted_keys)
        {
            output += $" {letter}: {SetHelper.SetToString(Paths[letter])};";
        }

        return output;
    }
}
