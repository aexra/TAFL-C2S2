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

    public GraphNodeControl(Vector2 position)
    {
        Position = new Vector2(position.X - Radius, position.Y - Radius);
        isSelected = false;
        this.InitializeComponent();
    }

    public bool ToggleSelection()
    {
        IsSelected = !IsSelected;
        return isSelected;
    }

    private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        ToggleSelection();
    }
}
