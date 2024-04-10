using CanvasedGraph.Raw;

namespace TAFL.Classes;
public class KIteration
{
    public int K;
    public List<Eqlass> SCL;
    public List<Eqlass> FCL;
    public Graph Graph;
    private bool LastIter = true;

    public KIteration(List<Eqlass> start, Graph graph, int k = 0)
    {
        SCL = start;
        FCL = new();
        Graph = graph;
        K = k;
    }

    public bool IsLast()
    {
        return true;
    }

    // returns this if no NEW equivalence classes available
    public KIteration Next()
    {
        // Получим алфавит переходов графа
        var alphabet = Graph.GetWeightsAlphabet();

        // Создадим новый список классов эквивалентности и заполним классами с одним состоянием в каждом
        List<Eqlass> next = new();
        foreach (var sc in SCL)
        {
            var c = new Eqlass();
            c.Add(sc.Nodes.First());
            next.Add(c);
        }

        // Пройдем по всем состониям исходной итерации и заполним (или добавим новые) классы эквивалентности в next в соответствии с теоремой
        foreach (var sc in SCL)
        {
            // Если найдется хотя бы один входной символ letter, такой, что состояния \delta(q_1,a) и \delta(q_2,a) окажутся в разных классах (к-1)-эквивалентности (рис. 7.24), то состояния q_1 и q_2 уже не будут k-эквивалентными
            foreach (var letter in alphabet)
            {
                // А есть вообще что сравнивать?
                var comparable = sc.Nodes.Where(n => n.Edges.Exists(e => e.Weight.Contains(letter))).ToList();
                if (comparable.Count < 2) continue;

                // Идем по всем подходящим состояниям текущего sc
                foreach (var node in comparable)
                {
                    // Проверяем что этого состояния еще нет в новых классах
                    if (next.Exists(c => c.Nodes.Contains(node))) continue;

                    // Получим состояние, которое достижимо из данного через letter (ТЕОРЕТИЧЕСКИ ЭТО НЕ NULL)
                    var destination = node.Edges.Find(e => e.Weight.Contains(letter)).Right;

                    // Получим исходный класс, в котором есть node (ЭТО sc)
                    //var node_class = SCL.Find(_sc => _sc.Nodes.Contains(node));

                    // Получим класс, в котором есть destination
                    var dest_class = SCL.Find(_sc => _sc.Nodes.Contains(destination));

                    // Проверим, что в next есть такой класс, в котором есть состояние, которое содержится в dest_class
                    var search = next.Find(c => c.Nodes.Exists(n => dest_class.Nodes.Contains(n)));
                    if (search != null)
                    {
                        search.Add(node);
                    }
                    else
                    {
                        next.Add(new Eqlass().Add(node));
                        LastIter = false;
                    }
                }
            }
        }

        // Если новые классы равны прошлым, вернем эту же итерацию, иначе создадим новую итерацию с номером + 1
        KIteration it;
        if (LastIter)
        {
            it = this;
        }
        else
        {
            it = new KIteration(next, Graph, K + 1);
        }

        return it;
    }
}
