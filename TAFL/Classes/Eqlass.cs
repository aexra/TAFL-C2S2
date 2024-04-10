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
}
