using CanvasedGraph.Raw;

namespace TAFL.Classes;
public class KIteration
{
    public int K;
    public List<Eqlass> SCL;
    public Graph Graph;
    private bool LastIter = true;

    public KIteration(List<Eqlass> start, Graph graph, int k = 0)
    {
        SCL = start;
        Graph = graph;
        K = k;
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

    // generate new graph from this iteration
    public Graph ToGraph()
    {
        Graph graph = new();

        // Создаем вершины из классов эквиваленции
        foreach (var sc in SCL)
        {
            graph.AddNode(new(sc.GetName()));
        }

        // Соединяем исходя из возможностей каждого класса по изначальному графу
        //foreach (var node in Graph.Nodes)
        //{
        //    // Найдем класс, в котором содержится это состояние
        //    var cl = SCL.Find(s => s.Nodes.Contains(node));

        //    // Получим его имя
        //    var cl_name = cl.GetName();

        //    // Найдём вершину НОВОГО графа с этим именем
        //    var m = graph.GetNode(cl_name);

        //    // Найдем все классы, которые достижимы из cl
        //    var dest = SCL.Where(s => );
        //}
        foreach (var cl in SCL)
        {
            // Получим его имя
            var cl_name = cl.GetName();

            // Найдём вершину НОВОГО графа с этим именем
            var m = graph.GetNode(cl_name);

            // Найдем все классы, достижимые из этого
            Dictionary<Eqlass, string> dests = new();
            foreach (var node in cl.Nodes)
            {
                // Найдем класс, в котором содержится это состояние
                var dest = SCL.Find(s => s.Nodes.Contains(node));
                dests.Add(dest, "");
            }

            // Подсчитаем с какими весами можно добраться из m в каждый другой класс
            foreach (var dest_cl in dests.Keys)
            {
                foreach (var node in cl.Nodes)
                {
                    var edge = node.Edges.Find(e => dest_cl.Nodes.Contains(e.Right));
                    if (edge == null) continue;
                    dests[dest_cl] += $"{edge.Weight},";
                }
            }

            // Выполним соединение, если его еще нет
            foreach (var dest_cl in dests.Keys)
            {
                m.Connect(graph.GetNode(dest_cl.GetName()), dests[dest_cl], true);
            }
        }

        return graph;
    }
}
