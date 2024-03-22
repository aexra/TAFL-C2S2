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
    public float SubStateRadius = 37;
    public float InnerRadius = 34;
    public float SelectionDiameter => SelectionRadius * 2;
    public float Diameter => Radius * 2;
    public float SubStateDiameter => SubStateRadius * 2;
    public float InnerDiameter => InnerRadius * 2;

    // FLAGS
    private bool isSelected = false;
    private bool isDragging = false;
    
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

    // ������� ����� ��������� ������ ����������� ������� ������� PointerMove
    private bool IsFirstInteraction = true;

    // OTHER
    public Vector2 Center => new(Position.X + Radius, Position.Y + Radius);

    // COLORS
    private readonly Color DefaultSubStateColor = Color.Transparent;
    private readonly Color SelectionColor = Color.OrangeRed;
    private readonly Color DraggingColor = Color.Lime;
    
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
                case NodeSubState.Final:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(SelectionColor.A, SelectionColor.R, SelectionColor.G, SelectionColor.B));
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
    public void Select()
    {
        if (Graph.SelectionMode == Enums.SelectionMode.None) return;
        Graph.NodeSelecting(this);
    }
    public void Deselect()
    {
        IsSelected = false;
    }
    public void ToggleSelection()
    {
        if (IsSelected)
        {
            Deselect();
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
        var props = e.GetCurrentPoint(Graph.Canvas).Properties;
        if (props.IsLeftButtonPressed) ToggleSelection();
        if (props.IsRightButtonPressed)
        {
            FlyoutBase.ShowAttachedFlyout(this);
        }
    }
    private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (Graph.SelectionMode == Enums.SelectionMode.None)
        {
            Deselect();
        }
        IsDragging = false;
    }
    private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (IsFirstInteraction)
        {
            IsFirstInteraction = false;
            return;
        }
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
        IsDragging = false;
    }

    // FLYOUT EVENTS
    private async void FlyoutRenameButton_Click(object sender, RoutedEventArgs e)
    {
        var content = new StringInputDialog();
        var dialog = new ContentDialog();

        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "������������� �������";
        dialog.PrimaryButtonText = "������";
        dialog.CloseButtonText = "������";
        dialog.DefaultButton = ContentDialogButton.Primary;
        content.Placeholder = "������� ����� ���";
        dialog.Content = content;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            if (Graph.IsNameUnique(content.Input))
            {
                Title = content.Input;
            }
            else
            {
                ContentDialog errorDialog = new ContentDialog();

                errorDialog.XamlRoot = this.XamlRoot;
                errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                errorDialog.Title = "������� � ����� ������ ��� ����������";
                errorDialog.CloseButtonText = "��";
                errorDialog.DefaultButton = ContentDialogButton.Close;

                await errorDialog.ShowAsync();
            }
        }
    }
    private void FlyoutDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Graph.RemoveNode(this);
    }
    private void FlyoutConnectButton_Click(object sender, RoutedEventArgs e)
    {
        Graph.SelectionMode = Enums.SelectionMode.Multiple;
        Select();
        Graph.RequestSelection((node) => {
            Graph.ConnectNodes(this, node, "");
            Graph.DeselectAllNodes();
            Graph.SelectionMode = Enums.SelectionMode.None;
        });

        //var content = new StringInputDialog2();
        //var dialog = new ContentDialog();

        //dialog.XamlRoot = this.XamlRoot;
        //dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        //dialog.Title = $"������������ �������";
        //dialog.PrimaryButtonText = "���������";
        //dialog.CloseButtonText = "������";
        //dialog.DefaultButton = ContentDialogButton.Primary;
        //content.Placeholder1 = "������� ��� �������";
        //content.Placeholder2 = "������� ��� �����";
        //content.CheckBoxContent = "�� ��������� � ����?";
        //dialog.Content = content;

        //var result = await dialog.ShowAsync();

        //if (result == ContentDialogResult.Primary)
        //{
        //    var toConnectName = content.First;
        //    var weight = content.Second;
        //    var isBackwards = content.Flag;

        //    var node = Graph.GetNode(toConnectName);

        //    if (node != null)
        //    {
        //        if (Graph.IsEdgeExists(this, node) && this != node)
        //        {
        //            await DialogHelper.ShowErrorDialogAsync("����� ��� ����������", XamlRoot);
        //            return;
        //        }
        //        Graph.ConnectNodes(this, node, weight);
        //    }
        //    else
        //    {
        //        await DialogHelper.ShowErrorDialogAsync("�� ������� ������� � ������ " + toConnectName, XamlRoot);
        //    }
        //}
    }
    private async void FlyoutLoopButton_Click(object sender, RoutedEventArgs e)
    {
        var weight = await DialogHelper.ShowSingleInputDialogAsync(XamlRoot, "������� �����", "������� ���");
        weight ??= string.Empty;
        AddLoop(weight);
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

    // NODE EVENTS
    public void Selected()
    {
        IsSelected = true;
        if (Graph.SelectionRequests.Count > 0)
        {
            Graph.SelectionRequests.Dequeue().Invoke(this);
        }
    }
}
