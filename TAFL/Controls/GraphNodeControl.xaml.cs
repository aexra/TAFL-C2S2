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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

    private Canvas Canva;
    private bool isSelected;
    private readonly Color SelectionColor = Color.DarkOrange;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Brush SelectedBrush
    {
        get
        {
            if (isSelected)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(SelectionColor.A, SelectionColor.R, SelectionColor.G, SelectionColor.B));
            }
            else
            {
                Resources.ThemeDictionaries.TryGetValue("ApplicationPageBackgroundThemeBrush", out var brush);
                return brush != null? brush is Brush? (Brush)brush : null : null;
            }
        }
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
        Select();
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
                if (l5.CheckNodeCollisions(this))
                {
                    Canvas.SetLeft(this, Position.X);
                    Canvas.SetTop(this, Position.Y);
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
}
