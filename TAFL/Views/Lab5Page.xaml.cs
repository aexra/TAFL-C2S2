﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TAFL.Helpers;
using TAFL.Services;
using TAFL.ViewModels;
using Windows.Storage.Pickers;
using CanvasedGraph;

namespace TAFL.Views;

public sealed partial class Lab5Page : Page
{
    private readonly Constructor Constructor;
    private readonly Constructor InterOutput;
    private readonly Constructor Output;
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
    private void Constructor_NodeCreated(Vertex node)
    {
        LogService.Log($"Создана вершина {node.Title}");
    }
    private void Constructor_NodeRemoved(Vertex node)
    {
        LogService.Log($"Удалена вершина {node.Title}");
    }
    private void Constructor_EdgeCreated(Edge edge)
    {
        LogService.Log(edge.Left != edge.Right ? $"Соединены вершины {edge.Left.Title} и {edge.Right.Title}" : $"Создана петля в {edge.Left.Title}");
    }
    private void Constructor_EdgeRemoved(Edge edge)
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

        var graph = Constructor.ToRaw();

        var dgraph = GraphDeterminizationService.GetDeterminizedGraph(graph, out var process, out var sgraph);

        LogService.Log(process);

        InterOutput.Clear();
        InterOutput.FromRaw(sgraph);

        Output.Clear();
        Output.FromRaw(dgraph);
    }
    private async void CheckWordButton_Click(object sender, RoutedEventArgs e)
    {
        var w = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "Проверить", "Введите слово");
        if (w == null)
        {
            LogService.Warning("Не введено слово для проверки");
            return;
        }
        var dgraph = GraphDeterminizationService.GetDeterminizedGraph(Constructor.ToRaw(), out var process, out var sgraph);
        if (dgraph.Match(w)) LogService.Log($"{w} - подходит");
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
