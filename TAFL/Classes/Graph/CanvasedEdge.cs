using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using TAFL.Controls;
using TAFL.Services;
using Windows.UI;

namespace TAFL.Classes.Graph;
public class CanvasedEdge
{
    public GraphNodeControl Left;
    public GraphNodeControl Right;
    public bool ToRight;
    public string Weight; // a,b,c,...,e

    public Microsoft.UI.Xaml.Shapes.Path PathObject;

    private readonly System.Drawing.Color DefaultPathStroke = System.Drawing.Color.Gray;

    public CanvasedEdge(GraphNodeControl left, GraphNodeControl right, bool toRight, string weight)
    {
        Left = left;
        Right = right;
        ToRight = toRight;
        Weight = weight;
    }

    public Microsoft.UI.Xaml.Shapes.Path UpdatePath()
    {
        var halfPi = Math.PI / 2;
        var angle = Math.Atan2(Left.Position.Y - Right.Position.Y, Left.Position.X - Right.Position.X);
        var absAngle = Math.Abs(angle);
        var _a = absAngle;

        absAngle = NormalizeAngle(absAngle);
        var b = absAngle / halfPi;
        absAngle = NormalizeAngle(absAngle + halfPi);
        var a = absAngle / halfPi;
        if (a < 0.1) a = 0.1;
        if (b < 0.1) b = 0.1;
        LogService.Error($"{a}, {b}");

        var path = new Microsoft.UI.Xaml.Shapes.Path() { Stroke = new SolidColorBrush(Color.FromArgb(
            DefaultPathStroke.A,
            DefaultPathStroke.R,
            DefaultPathStroke.G,
            DefaultPathStroke.B
        )), StrokeThickness = 4 };
        var pd = new PathGeometry();
        var pf = new PathFigure() {
            StartPoint = ToRight ?
                new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius) :
                new Windows.Foundation.Point(Right.Position.X + Right.Radius, Right.Position.Y + Right.Radius),
            IsClosed = false
        };
        pf.Segments.Add(new ArcSegment() { 
            Point = ToRight ?
                new Windows.Foundation.Point(Right.Position.X + Right.Radius, Right.Position.Y + Right.Radius) :
                new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius),
            SweepDirection = ToRight ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
            Size = new Windows.Foundation.Size(a, b),
            RotationAngle = _a
        });
        pd.Figures.Add(pf);
        path.Data = pd;
        PathObject = path;
        return path;
    }
    private double NormalizeAngle(double angle)
    {
        if (angle > Math.PI / 2) angle = Math.PI - angle;
        return angle;
    }
}
