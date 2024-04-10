using CanvasedGraph.Raw;

namespace TAFL.Classes;
public class Eqlass
{
    public List<Node> Nodes;

    public Eqlass()
    {
        Nodes = new();
    }

    public Eqlass Add(Node node)
    {
        Nodes.Add(node);
        return this;
    }
    public Eqlass Remove(Node node)
    {
        Nodes.Remove(node);
        return this;
    }
    public string GetName()
    {
        var name = "";
        foreach (var node in Nodes)
        {
            name += node.Name + "-";
        }
        name = name[..^1];
        return name;
    }
    public override string ToString()
    {
        var s = "{";

        foreach (var node in Nodes)
        {
            s += node.Name + ", ";
        }

        s = s[..^2];
        s += "}";

        return s;
    }
    public bool IsSplittable(List<Eqlass> eqs, string letter, out List<List<Node>>? splitting)
    {
        Dictionary<Node, Eqlass> transitions = new();

        foreach (var node in Nodes)
        {
            var edge = node.Edges.Find(e => e.Weight.Contains(letter));
            if (edge != null) transitions.Add(node, eqs.Find(x => x.Nodes.Contains(edge.Right)));
            else
            {
                splitting = null;
                return false;
            }
        }

        Dictionary<Eqlass, List<Node>> class_splitting = new();

        foreach (var kv in transitions)
        {
            if (!class_splitting.Keys.Contains(kv.Value))
            {
                class_splitting.Add(kv.Value, new());
            }

            class_splitting[kv.Value].Add(kv.Key);
        }

        if (class_splitting.Keys.Count > 1)
        {
            splitting = class_splitting.Values.ToList();
            return true;
        }

        splitting = null;
        return false;
    }
    public List<Eqlass> Split(List<List<Node>> splitting)
    {
        List<Eqlass> eqs = new();
        foreach (var split in splitting)
        {
            eqs.Add(new());
            split.ForEach(x => eqs.Last().Add(x));
        }
        return eqs;
    }
}
