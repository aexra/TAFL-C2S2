using System.Collections.Immutable;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using TAFL.Classes.Graph;
using TAFL.Controls;
using TAFL.Helpers;
using TAFL.Interfaces;
using TAFL.Services;
using TAFL.Structures;
using TAFL.ViewModels;
using Windows.Storage.Pickers;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    private readonly CanvasedGraph Constructor;
    private readonly CanvasedGraph InterOutput;
    private readonly CanvasedGraph Output;
    public Lab5ViewModel ViewModel
    {
        get;
    }

    // CONSTRUCTOR
    public Lab5Page()
    {
        ViewModel = App.GetService<Lab5ViewModel>();
        InitializeComponent();

        Constructor = new(ConstructorCanvas);
        InterOutput = new(InterOutputCanvas);
        Output = new(OutputCanvas);
        //Output = new(OutputCanvas) { ReadOnly=true };

        Constructor.NodeCreated += Constructor_NodeCreated;
        Constructor.NodeRemoved += Constructor_NodeRemoved;
        Constructor.EdgeCreated += Constructor_EdgeCreated;
        Constructor.EdgeRemoved += Constructor_EdgeRemoved;
        Constructor.GraphCleared += Constructor_GraphCleared;
        Constructor.Loaded += Constructor_Loaded;
    }

    private void Constructor_Loaded(string path)
    {
        LogService.Log($"Граф загружен из файла: {path}");
    }
    private void Constructor_NodeCreated(Controls.GraphNodeControl node)
    {
        LogService.Log($"Создана вершина {node.Title}");
    }
    private void Constructor_NodeRemoved(Controls.GraphNodeControl node)
    {
        LogService.Log($"Удалена вершина {node.Title}");
    }
    private void Constructor_EdgeCreated(CanvasedEdge edge)
    {
        LogService.Log(edge.Left != edge.Right ? $"Соединены вершины {edge.Left.Title} и {edge.Right.Title}" : $"Создана петля в {edge.Left.Title}");
    }
    private void Constructor_EdgeRemoved(CanvasedEdge edge)
    {
        LogService.Log(edge.Left != edge.Right? $"Удалено ребро между {edge.Left.Title} и {edge.Right.Title}" : $"Удалена петля в {edge.Left.Title}");
    }
    private void Constructor_GraphCleared()
    {
        LogService.Log($"Граф очищен");
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Constructor.Clear();
        InterOutput.Clear();
        Output.Clear();
    }
    private async void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        if (Constructor.GetStartNode() == null || Constructor.GetEndNode() == null)
        {
            await DialogHelper.ShowErrorDialogAsync("Определите начальную и конечную вершины", XamlRoot);
            return;
        }

        var graph = Constructor.GetRawGraph();

        var dgraph = GraphDeterminizationService.GetDeterminizedGraph(graph, out var process, out var sgraph);

        LogService.Log(process);

        InterOutput.Clear();
        InterOutput.FromRaw(sgraph);

        Output.Clear();
        Output.FromRaw(dgraph);
    }
    private void CheckWordButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
    {
        var filePicker = new FileOpenPicker();

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

        filePicker.FileTypeFilter.Add(".graph");
        var file = await filePicker.PickSingleFileAsync();

        if (file != null)
        {
            Constructor.FromJson(file);
        }
        else
        {
            LogService.Error("Не выбран подходящий файл для загрузки");
        }
    }
    private void DownloadGraphFileButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
