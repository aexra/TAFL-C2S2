using CanvasedGraph;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Classes;
using TAFL.Helpers;
using TAFL.Services;
using TAFL.ViewModels;
using Windows.Storage.Pickers;

namespace TAFL.Views;

public sealed partial class Lab6Page : Page
{
    private readonly Constructor Constructor;
    private readonly Constructor Output;


    public Lab6ViewModel ViewModel
    {
        get;
    }

    public Lab6Page()
    {
        ViewModel = App.GetService<Lab6ViewModel>();
        InitializeComponent();

        Constructor = new(ConstructorCanvas);
        Output = new(OutputCanvas);
    }

    private KIteration GetInitialIteration(CanvasedGraph.Raw.Graph graph)
    {
        Eqlass nf = new();
        Eqlass f = new();

        foreach (var node in graph.Nodes)
        {
            if (node.SubState == CanvasedGraph.Enums.NodeSubState.End)
            {
                f.Add(node);
            }
            else
            {
                nf.Add(node);
            }
        }

        LogService.Log($"final: {f}");
        LogService.Log($"not final: {nf}");

        return new(new List<Eqlass>() { nf, f }, graph);
    }

    private void ClearCanvasButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Constructor.Clear();
        Output.Clear();
    }
    private async void SolveLabButton_Click(object sender, RoutedEventArgs e)
    {
        if (Constructor.GetStartNode() == null || Constructor.GetEndNode() == null)
        {
            await DialogHelper.ShowErrorDialogAsync("Определите начальную и конечную вершины", XamlRoot);
            return;
        }

        var graph = Constructor.ToRaw();

        var it = GetInitialIteration(graph);
        while (true)
        {
            LogService.Log(it);
            var next = it.Next();
            if (next == it)
            {
                break;
            }
            it = next;
        }

        Output.Clear();
        Output.FromRaw(it.ToGraph());
    }
    private async void CheckWordButton_Click(object sender, RoutedEventArgs e)
    {
        var w = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "Проверить", "Введите слово");
        if (w == null)
        {
            LogService.Warning("Не введено слово для проверки");
            return;
        }
        var graph = Output.ToRaw();
        if (graph.Match(w)) LogService.Log($"{w} - подходит");
        else LogService.Log($"{w} - не подходит");
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
            if (!Constructor.FromJson(file))
            {
                LogService.Error($"Ошибка парсинга файла графа: {file.Path}");
            }
        }
        else
        {
            LogService.Error("Не выбран подходящий файл для загрузки");
        }
    }
    private async void DownloadGraphFileButton_Click(object sender, RoutedEventArgs e)
    {
        var json = Constructor.ToJson();
        if (json == null)
        {
            LogService.Error("Ошибка чтения графа в JSON");
            return;
        }

        var folderPicker = new FolderPicker();

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

        folderPicker.FileTypeFilter.Add(".graph");
        var folder = await folderPicker.PickSingleFolderAsync();

        if (folder == null)
        {
            LogService.Warning("Не выбрана папка для сохранения");
            return;
        }

        var name = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "Сохранить", "Введите название файла");
        if (name == null)
        {
            LogService.Warning("Не выбрано название файла");
            return;
        }

        var file = await folder.CreateFileAsync(name + ".graph", Windows.Storage.CreationCollisionOption.ReplaceExisting);
        await Windows.Storage.FileIO.WriteTextAsync(file, json);

        LogService.Log($"Граф успешно сохранен в файл: {file.Path}");
    }
}
