using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Bson;
using TAFL.Classes.Graph;
using TAFL.Enums;
using TAFL.Helpers;
using TAFL.Services;
using TAFL.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TAFL.Controls;
public sealed partial class GraphNodeControl : UserControl, INotifyPropertyChanged
{
    // INPUT PROPS
    public Vector2 Position;
    private readonly CanvasedGraph Graph;

    // COUNTERS
    public int Loops = 0;
    
    // ENUMS
    private NodeSubState subState = NodeSubState.Default;
    public NodeSubState SubState
    {
        get => subState;
        set
        {
            if (value != subState)
            {
                subState = value;
                NotifyPropertyChanged(nameof(SubStateBrush));
            } 
        }
    }

    // VARIABLE FIELDS
    private string title = "A";
    public float SelectionRadius = 40;
    public float Radius = 38;
    public float SubStateRadius = 36;
    public float InnerRadius = 34;
    public float SelectionDiameter => SelectionRadius * 2;
    public float Diameter => Radius * 2;
    public float SubStateDiameter => SubStateRadius * 2;
    public float InnerDiameter => InnerRadius * 2;

    // FLAGS
    private bool isSelected = false;
    private bool isDragging = false;
    private bool isDeselectable = true;
    private bool isDraggable = true;
    
    // PUBLIC GET-SET ABSTRACTIONS
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (value != isSelected)
            {
                isSelected = value;
                NotifyPropertyChanged(nameof(SelectionBrush));
            }
        }
    }
    public bool IsDragging
    {
        get => isDragging;
        set
        {
            if (value != isDragging)
            {
                isDragging = value;
                NotifyPropertyChanged(nameof(SelectionBrush));
            }
        }
    }
    public bool IsDraggable
    {
        get => isDraggable;
        set
        {
            if (value != isDraggable)
            {
                isDraggable = value;
                NotifyPropertyChanged();
            }
        }
    }
    public bool IsDeselectable
    {
        get => isDeselectable;
        set
        {
            if (value != isDeselectable)
            {
                isDeselectable = value;
                NotifyPropertyChanged();
            }
        }
    }
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

    // Костыль чтобы прерывать первое перемещение вызовом события PointerMove
    private bool IsFirstInteraction = true;

    // OTHER
    public Vector2 Center => new(Position.X + Radius, Position.Y + Radius);

    // COLORS
    private readonly Color DefaultSubStateColor = Color.Transparent;
    private readonly Color StartSubStateColor = Color.Lime;
    private readonly Color EndSubStateColor = Color.Red;
    private readonly Color UniversalSubStateColor = Color.Yellow;
    private readonly Color SelectionColor = Color.OrangeRed;
    private readonly Color DraggingColor = Color.Blue;
    
    // BRUSHES
    public Brush SelectionBrush
    {
        get
        {
            if (IsDragging)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(DraggingColor.A, DraggingColor.R, DraggingColor.G, DraggingColor.B));
            }
            else if (IsSelected)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(SelectionColor.A, SelectionColor.R, SelectionColor.G, SelectionColor.B));
            }
            else
            {
                Resources.ThemeDictionaries.TryGetValue("ApplicationPageBackgroundThemeBrush", out var brush);
                return brush != null ? brush is Brush ? (Brush)brush : null : null;
            }
        }
    }
    public Brush SubStateBrush
    {
        get
        {
            switch (SubState)
            {
                case NodeSubState.Start:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(StartSubStateColor.A, StartSubStateColor.R, StartSubStateColor.G, StartSubStateColor.B));
                case NodeSubState.End:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(EndSubStateColor.A, EndSubStateColor.R, EndSubStateColor.G, EndSubStateColor.B));
                case NodeSubState.Universal:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(UniversalSubStateColor.A, UniversalSubStateColor.R, UniversalSubStateColor.G, UniversalSubStateColor.B));
                default:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(DefaultSubStateColor.A, DefaultSubStateColor.R, DefaultSubStateColor.G, DefaultSubStateColor.B));
            }
        }
    }

    // NotifyPropertyChanged event for OneWay (TwoWay) bindings
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // CONSTRUCTORS
    public GraphNodeControl(Vector2 position, CanvasedGraph graph)
    {
        Position = new Vector2(position.X - Radius, position.Y - Radius);
        subState = NodeSubState.Default;
        Graph = graph;
        this.InitializeComponent();
    }

    // SELECTION METHODS
    public void Select(bool deselectable = true, bool ephemeral = false)
    {
        if (Graph.SelectionMode == Enums.SelectionMode.None) return;
        Graph._NodeSelecting_(this, ephemeral);
        isDeselectable = deselectable;
    }
    public void Deselect()
    {
        IsSelected = false;
    }
    public void ToggleSelection()
    {
        if (IsSelected)
        {
            if (IsDeselectable)
            {
                Deselect();
            }
            else
            {
                Select(true, true);
            }
        }
        else 
        {
            Select();
        }
    }

    // POINTER EVENTS
    private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        if (Graph.ReadOnly) return;
        var props = e.GetCurrentPoint(Graph.Canvas).Properties;
        if (props.IsLeftButtonPressed) ToggleSelection();
        if (props.IsRightButtonPressed)
        {
            FlyoutBase.ShowAttachedFlyout(this);
        }
    }
    private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (Graph.ReadOnly) return;
        if (Graph.SelectionMode == Enums.SelectionMode.None)
        {
            Deselect();
        }
        IsDragging = false;
    }
    private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (Graph.ReadOnly) return;
        if (IsFirstInteraction)
        {
            IsFirstInteraction = false;
            return;
        }
        if (!IsDraggable) return;
        var props = e.GetCurrentPoint(null).Properties;
        if (props.IsLeftButtonPressed)
        {
            var pos = e.GetCurrentPoint(Graph.Canvas).Position;
            var tr_pos = new Vector2((float)pos.X - Radius, (float)pos.Y - Radius);

            if (Position - tr_pos == Vector2.Zero)
            {
                IsDragging = false;
                return;
            }

            var last_pos = Position;
            Position = tr_pos;

            if (!Graph.CheckNodeCollisions(this))
            {
                Move(Position.X, Position.Y);
                Graph.UpdateConnectedEdges(this);
                IsDragging = true;
                Select();
            }
            else
            {
                Position = last_pos;
            }
        }
        else
        {
            IsDragging = false;
        }
    }
    private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (Graph.ReadOnly) return;
        IsDragging = false;
    }
    private void Border_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        ConnectFromSelection();
    }

    // FLYOUT EVENTS
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
            await Graph.RenameNode(this.Title, content.Input);
        }

        ContextFlyout.Hide();
    }
    private void FlyoutDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Graph.RemoveNode(this);
        ContextFlyout.Hide();
    }
    private void FlyoutConnectButton_Click(object sender, RoutedEventArgs e)
    {
        ContextFlyout.Hide();
        ConnectFromSelection();
    }
    private async void FlyoutLoopButton_Click(object sender, RoutedEventArgs e)
    {
        var weight = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "Создать петлю", "Введите вес");
        weight ??= string.Empty;
        AddLoop(weight);
        ContextFlyout.Hide();
    }
    private void MakeMeDefault_Click(object sender, RoutedEventArgs e)
    {
        this.SubState = NodeSubState.Default;
        ContextFlyout.Hide();
    }
    private void MakeMeStart_Click(object sender, RoutedEventArgs e)
    {
        this.SubState = NodeSubState.Start;
        ContextFlyout.Hide();
    }
    private void MakeMeEnd_Click(object sender, RoutedEventArgs e)
    {
        this.SubState = NodeSubState.End;
        ContextFlyout.Hide();
    }

    // NODE MANIPULATION METHODS
    public void AddLoop(string weight)
    {
        Graph.ConnectNodes(this, this, weight);
    }
    public void Move(float x, float y)
    {
        Canvas.SetLeft(this, Position.X);
        Canvas.SetTop(this, Position.Y);
    }
    public void LockPosition()
    {
        IsDraggable = false;
    }
    public void UnlockPosition()
    {
        IsDraggable = true;
    }
    public async Task ConnectFromDialogAsync()
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
            var isBackwards = content.Flag;

            var node = Graph.GetNode(toConnectName);

            if (node != null)
            {
                if (Graph.IsEdgeExists(this, node) && this != node)
                {
                    await DialogHelper.ShowErrorDialogAsync("Ребро уже существует", XamlRoot);
                    return;
                }
                Graph.ConnectNodes(this, node, weight);
            }
            else
            {
                await DialogHelper.ShowErrorDialogAsync("Не найдена вершина с именем " + toConnectName, XamlRoot);
            }
        }
    }
    public void ConnectFromSelection()
    {
        Graph.SelectionMode = Enums.SelectionMode.Multiple;
        Graph.LockAllNodesPosition();
        Select(false);
        Graph.RequestSelection(async (node, ephemeral) => {
            if (Graph.IsEdgeExists(this, node))
            {
                await DialogHelper.ShowErrorDialogAsync("Ребро уже существует", XamlRoot);
            }
            else
            {
                Graph.ConnectNodes(this, node, "");
            }
            Graph.DeselectAllNodes();
            Graph.SelectionMode = Enums.SelectionMode.None;
            Graph.UnlockAllNodesPosition();
        });
    }

    // NODE EVENTS
    public void Selected(bool ephemeral = false)
    {
        IsSelected = true;
        if (Graph.SelectionRequests.Count > 0)
        {
            Graph.SelectionRequests.Dequeue().Invoke(this, ephemeral);
        }
    }
}
