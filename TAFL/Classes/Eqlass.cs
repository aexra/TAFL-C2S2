using CanvasedGraph.Raw;

namespace TAFL.Classes;
public class Eqlass
{
    public List<Node> Nodes;

    public Eqlass()
    {
        Nodes = new();
    }

    public void Add(Node node)
    {
        Nodes.Add(node);
    }

    public void Remove(Node node)
    {
        Nodes.Remove(node);
    }
}
