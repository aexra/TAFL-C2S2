using TAFL.Classes.Graph;

namespace TAFL.Interfaces;
public interface IGraph
{
    public void AddNode(Node node);
    public void RemoveNode(Node node);
    public void Clear();
    public Node? GetNode(string name);
    public bool IsConnectionExists(Node node1, Node node2);
    public bool IsConnectionExists(string name1, string name2);
}
