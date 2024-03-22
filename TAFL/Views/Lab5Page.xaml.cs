using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Classes.Graph;
using TAFL.Services;
using TAFL.Structures;
using TAFL.ViewModels;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    private readonly CanvasedGraph Constructor;
    private readonly CanvasedGraph Output;
    public Lab5ViewModel ViewModel
    {
        get;
    }

    public Lab5Page()
    {
        ViewModel = App.GetService<Lab5ViewModel>();
        InitializeComponent();

        Constructor = new(ConstructorCanvas);
        Output = new(OutputCanvas);
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Constructor.Clear();
    }

    private void BuildQButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(GetQTable());
    }
    private void BuildSButton_Click(object sender, RoutedEventArgs e)
    {

    }
    private void BuildPButton_Click(object sender, RoutedEventArgs e)
    {

    }
    private void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        LogService.Log(GetQTable());
        GetTransitionsE(out var ets, out var locks);
        LogService.Log(ets);

        var alphabet = GetAlphabet();
        var tableS = GetSTable(locks, alphabet);
    }

    private string GetQTable()
    {
        return Constructor.GetRawGraph().ToString();
    }
    private string GetSTable(List<EpsLock> locks, List<string> alphabet)
    {
        return string.Empty;
    }
    private string GetPTable()
    {
        return string.Empty;
    }

    private List<Edge> GetTransitionsE(out string output, out List<EpsLock> locks)
    {
        List<Edge> edgesE = new();
        output = "Эпсилон замыкания";
        locks = new();

        var graph = Constructor.GetRawGraph();
        var counter = -1;
        foreach (var node in graph.Nodes)
        {
            locks.Add(new() { Name=$"S{++counter}", Origin=node, Nodes=new() });
            output += "\n( " + node.Name + " ) = { " + node.Name;
            foreach (var edge in node.Edges)
            {
                if (ParseWeights(edge.Weight).Contains("ε"))
                {
                    locks.Last().Nodes.Add(edge.Right);
                    edgesE.Add(edge);
                    output += $", {edge.Right.Name}";
                }
            }
            output += " }";
        }

        return edgesE;
    }
    private string[] ParseWeights(string weight) => weight.Split(',');
    private List<string> GetAlphabet()
    {
        List<string> alphabet = new();

        foreach (var node in Constructor.GetRawGraph().Nodes)
        {
            foreach (var edge in node.Edges)
            {
                var letters = ParseWeights(edge.Weight);
                foreach (var letter in letters)
                {
                    if (letter != "ε" && !alphabet.Contains(letter))
                    {
                        alphabet.Add(letter);
                    }
                }
            }
        }

        return alphabet;
    }
}
