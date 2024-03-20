using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TAFL.Classes.Graph;
using TAFL.Helpers;
using TAFL.Services;
using TAFL.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TAFL.Controls;
public sealed partial class GraphNodeControl : UserControl, INotifyPropertyChanged
{
    public Vector2 Position;
    public float Radius = 40;
    public float Diameter => Radius * 2;
    public float SelectionRadius = 37;
    public float SelectionDiameter => SelectionRadius * 2;
    public float InnerRadius = 34;
    public float InnerDiameter => InnerRadius * 2;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (value != isSelected)
            {
                isSelected = value;
                NotifyPropertyChanged("SelectedBrush");
            }
        }
    }
    public Page Page;
    public int Loops = 0;

    public List<CanvasedEdge> Edges = new();

    private string title = "A";
    public string Title
    {
        get => title;
        set
        {
            if (value != title)
            {
                title = value;
                NotifyPropertyChanged();
            }
        }
    }

    private readonly Canvas Canva;
    private bool isSelected;
    private readonly Color DefaultSelectionColor = Color.DarkOrange;
    public Brush SelectedBrush
    {
        get
        {
            if (isSelected)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(DefaultSelectionColor.A, DefaultSelectionColor.R, DefaultSelectionColor.G, DefaultSelectionColor.B));
            }
            else
            {
                Resources.ThemeDictionaries.TryGetValue("ApplicationPageBackgroundThemeBrush", out var brush);
                return brush != null ? brush is Brush ? (Brush)brush : null : null;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public GraphNodeControl(Vector2 position, Canvas canva)
    {
        Position = new Vector2(position.X - Radius, position.Y - Radius);
        isSelected = false;
        Canva = canva;
        this.InitializeComponent();
    }

    public void Select()
    {
        IsSelected = true;
    }
    public void Deselect()
    {
        IsSelected = false;
    }
    public bool ToggleSelection()
    {
        IsSelected = !IsSelected;
        return isSelected;
    }

    private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var props = e.GetCurrentPoint(Canva).Properties;
        if (props.IsLeftButtonPressed) Select();
        if (props.IsRightButtonPressed)
        {
            FlyoutBase.ShowAttachedFlyout(this);
        }
    }
    private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        Deselect();
    }
    private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
    { 
        if (IsSelected)
        {
            var pos = e.GetCurrentPoint(Canva).Position;
            var tr_pos = new Vector2((float)pos.X - Radius, (float)pos.Y - Radius);

            var last_pos = Position;
            Position = tr_pos;

            if (Page != null && Page is Lab5Page l5)
            {
                if (!l5.CheckNodeCollisions(this, Canva))
                {
                    Canvas.SetLeft(this, Position.X);
                    Canvas.SetTop(this, Position.Y);

                    ((Lab5Page)Page).UpdateEdges1(this);
                }
                else
                {
                    Position = last_pos;
                }
            }
            else
            {
                return;
            }
        }
    }
    private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        Deselect();
    }

    private async void FlyoutRenameButton_Click(object sender, RoutedEventArgs e)
    {
        var content = new StringInputDialog();
        var dialog = new ContentDialog();

        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Переименовать вершину";
        dialog.PrimaryButtonText = "Готово";
        dialog.CloseButtonText = "Отмена";
        dialog.DefaultButton = ContentDialogButton.Primary;
        content.Placeholder = "Введите новое имя";
        dialog.Content = content;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            if (((Lab5Page)Page).IsNameUnique(content.Input, Canva))
            {
                Title = content.Input;
            }
            else
            {
                ContentDialog errorDialog = new ContentDialog();

                errorDialog.XamlRoot = this.XamlRoot;
                errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                errorDialog.Title = "Вершина с таким именем уже существует";
                errorDialog.CloseButtonText = "Ок";
                errorDialog.DefaultButton = ContentDialogButton.Close;

                await errorDialog.ShowAsync();
            }
        }
    }
    private void FlyoutDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Canva.Children.Remove(this);
        foreach (var edge in Edges)
        {
            ((Lab5Page)Page).RemoveEdge(edge);
        }
    }
    private async void FlyoutConnectButton_Click(object sender, RoutedEventArgs e)
    {
        var content = new StringInputDialog2();
        var dialog = new ContentDialog();

        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = $"Присоединить вершину";
        dialog.PrimaryButtonText = "Соединить";
        dialog.CloseButtonText = "Отмена";
        dialog.DefaultButton = ContentDialogButton.Primary;
        content.Placeholder1 = "Введите имя вершины";
        content.Placeholder2 = "Введите вес ребра";
        content.CheckBoxContent = "От указанной к этой?";
        dialog.Content = content;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var toConnectName = content.First;
            var weight = content.Second;
            var flag = content.Flag;

            var node = ((Lab5Page)Page).GetNode(toConnectName, Canva);

            if (node != null)
            {
                var edge = new CanvasedEdge(this, node, flag, weight);
                if (((Lab5Page)Page).IsEdgeExists(edge) && !edge.IsLoop)
                {
                    await DialogHelper.ShowErrorDialogAsync("Ребро уже существует", XamlRoot);
                    return;
                }
                ((Lab5Page)Page).AddEdge(edge, Canva);
                Edges.Add(edge);
            }
            else
            {
                await DialogHelper.ShowErrorDialogAsync("Не найдена вершина с именем " + toConnectName, XamlRoot);
            }
        }
    }
    private async void FlyoutLoopButton_Click(object sender, RoutedEventArgs e)
    {
        var weight = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "Создать петлю", "Введите вес");
        weight ??= string.Empty;
        AddLoop(weight);
    }

    public void AddLoop(string weight)
    {
        var edge = new CanvasedEdge(this, this, false, weight);
        ((Lab5Page)Page).AddEdge(edge, Canva);
        Edges.Add(edge);
    }
}
