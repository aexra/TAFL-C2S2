using CanvasedGraph.Raw;

namespace TAFL.Classes;
public class KIteration
{
    public int K;
    public List<Eqlass> SC;
    public List<Eqlass> FC;

    public KIteration(int k, List<Eqlass> start)
    {
        K = k;
        SC = start;
        FC = new();
    }
}
