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
}
