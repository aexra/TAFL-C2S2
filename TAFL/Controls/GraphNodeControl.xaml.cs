using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using TAFL.Enums;
using TAFL.Helpers;
using TAFL.Services;
using TAFL.Views;

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
    public Page Page;
    public int Loops = 0;
    public Vector2 Center => new(Position.X + Radius, Position.Y + Radius);

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
    private NodeControlState state = NodeControlState.Default;
    public NodeControlState State
    {
        get => state;
        set
        {
            if (value != state)
            {
                state = value;
                NotifyPropertyChanged(nameof(SelectedBrush));
            } 
        }
    }
    private readonly Color DefaultSelectionColor = Color.DarkOrange;
    private readonly Color DefaultMovingColor = Color.CornflowerBlue;
    public Brush SelectedBrush
    {
        get
        {
            switch (State)
            {
                case NodeControlState.Selected:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(DefaultSelectionColor.A, DefaultSelectionColor.R, DefaultSelectionColor.G, DefaultSelectionColor.B));
                case NodeControlState.Moving:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(DefaultMovingColor.A, DefaultMovingColor.R, DefaultMovingColor.G, DefaultMovingColor.B));
                default:
                    Resources.ThemeDictionaries.TryGetValue("ApplicationPageBackgroundThemeBrush", out var brush);
                    return brush != null ? brush is Brush ? (Brush)brush : null : null;
            }
        }
    }

    private Lab5Page Page5 => (Lab5Page)Page;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public GraphNodeControl(Vector2 position, Canvas canva)
    {
        Position = new Vector2(position.X - Radius, position.Y - Radius);
        state = NodeControlState.Default;
        Canva = canva;
        this.InitializeComponent();
    }

    public void Select()
    {
        if (Page5.SelectionMode == Enums.SelectionMode.None) return;
        State = NodeControlState.Selected;
    }
    public void Deselect()
    {
        State = NodeControlState.Default;
    }
    public void ToggleSelection()
    {
        if (State == NodeControlState.Selected)
        {
            Deselect();
        }
        else if (State == NodeControlState.Default) 
        {
            Select();
        }
    }

    private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        var props = e.GetCurrentPoint(Canva).Properties;
        if (props.IsLeftButtonPressed) ToggleSelection();
        if (props.IsRightButtonPressed)
        {
            FlyoutBase.ShowAttachedFlyout(this);
        }
    }
    private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (Page5.SelectionMode == Enums.SelectionMode.None)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }
    private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var props = e.GetCurrentPoint(null).Properties;
        if (props.IsLeftButtonPressed)
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

                    ((Lab5Page)Page).UpdateConnectedEdges(this);

                    State = NodeControlState.Moving;
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
        if (State != NodeControlState.Selected)
        {
            State = NodeControlState.Default;
        }
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
        Page5.RemoveVertex(this);
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
            var isBackwards = content.Flag;

            var node = ((Lab5Page)Page).GetNode(toConnectName, Canva);

            if (node != null)
            {
                if (((Lab5Page)Page).IsConnectionExists(this, node) && this != node)
                {
                    await DialogHelper.ShowErrorDialogAsync("Ребро уже существует", XamlRoot);
                    return;
                }
                ((Lab5Page)Page).ConnectVertrices(this, node, weight);
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
        Page5.ConnectVertrices(this, this, weight);
    }
}
