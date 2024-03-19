using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TAFL.Controls;
public sealed partial class GraphNodeControl : UserControl
{
    private readonly float DEFAULT_SIZE_X = 40;
    private readonly float DEFAULT_SIZE_Y = 40;

    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    public GraphNodeControl(Vector2 position)
    {
        Position = new Vector2(position.X - DEFAULT_SIZE_X / 2, position.Y - DEFAULT_SIZE_Y / 2);
        Size = new Vector2(DEFAULT_SIZE_X, DEFAULT_SIZE_Y);

        this.InitializeComponent();
    }
}
