using CanvasedGraph.Raw;
using TAFL.Services;
using Windows.ApplicationModel.Activation;

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

    // return list of destinable eqlasses from state node by weight letter
    public List<Eqlass> GetDestinations(Node node, string letter)
    {
        // Получим список всех состояний в которые можно попасть из node по letter
        var dest_nodes = node.Edges.Where(e => e.Weight == letter).Select(e => e.Right);
        return SCL.Where(sc => sc.Nodes.Exists(n => dest_nodes.Contains(n))).ToList();
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

        //LogService.Log(new KIteration(next, Graph, -1));

        // Пройдем по всем состониям исходной итерации и заполним (или добавим новые) классы эквивалентности в next в соответствии с теоремой
        foreach (var sc in SCL)
        {
            // Мы должны разместить каждое состояние которое еще не разместили
            foreach (var node in sc.Nodes)
            {
                // Проверяем что этого состояния еще нет в новых классах
                if (next.Exists(c => c.Nodes.Contains(node))) continue;

                // Нужно понять, создаем ли мы для него новый класс
                var splittable = false;

                // Пройдем по всем буквам алфавита, и если этот класс делим, т.е. существует такая буква, что node и другая подходящая ведут в разные классы, то установим splittable -> true
                foreach (var letter in alphabet)
                {
                    // А есть вообще что сравнивать?
                    var comparable = sc.Nodes.Where(n => n.Edges.Exists(e => e.Weight.Contains(letter))).ToList();
                    if (comparable.Count < 2 || !comparable.Contains(node)) continue; // Этим выполняется случай когда есть только одно ребро для сравнения или нет вообще или среди них нет node

                    // Получим классы в которые можно попасть из этих состояний 
                    List<Eqlass> dests = new();
                    foreach (var c_node in comparable)
                    {
                        GetDestinations(c_node, letter).ForEach(d =>
                        {
                            if (!dests.Contains(d)) dests.Add(d);
                        });
                    }

                    // Проверяем их количество (если > 1, то node нужно отделить)
                    if (dests.Count > 1)
                    {
                        splittable = true;
                    }
                }

                // Если класс делим, то засунем node в новый класс, иначе засунем node в существующий (в тот, в котором он был изначально)
                if (splittable)
                {
                    // Добавим новый класс с этим состоянием
                    next.Add(new Eqlass().Add(node));

                    // Объявим, что эта итерация не финальная
                    LastIter = false;
                    
                    LogService.Log($"Создал новый класс: {node.Name} -> {next.Last()}");
                }
                else
                {
                    // Найдем состояния из того класса, в котором был node
                    var nodes = SCL.Find(sc => sc.Nodes.Contains(node)).Nodes.Where(n => n != node);

                    // Найдем новый класс, в котором есть хотя бы одно из состояний nodes
                    var clas = next.Find(cl => cl.Nodes.Exists(n => nodes.Contains(n)));

                    // Добавим в него node
                    clas.Add(node);
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

        LogService.Log($"{K}, {this}");
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
                if (dests.Keys.Contains(dest)) continue;
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

    // simple string equivalent
    public override string ToString()
    {
        var output = "";

        foreach (var cl in SCL)
        {
            output += $"{cl} ";
        }

        return output[..^1];
    }
}
