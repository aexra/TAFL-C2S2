using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
public sealed partial class StringInputDialog2 : UserControl
{
    public string Placeholder1 = "Input value";
    public string Placeholder2 = "Input value";
    public string CheckBoxContent = "CheckBox title";

    public string First => InputBox1.Text;
    public string Second => InputBox2.Text;
    public bool Flag => Check.IsChecked ?? false;
    public string GetFirst() => InputBox1.Text;
    public string GetSecond() => InputBox2.Text;
    public bool GetFlag() => Check.IsChecked ?? false;

    public StringInputDialog2()
    {
        this.InitializeComponent();
    }
}
