using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TAFL.Controls;
using TAFL.Services;
using Windows.UI;

namespace TAFL.Classes.Graph;
public class CanvasedEdge
{
    public static readonly int LoopAngleModifier = 40;
    public GraphNodeControl Left;
    public GraphNodeControl Right;
    public bool ToRight;
    public string Weight => wb.Text; // a,b,c,...,e
    public bool IsArc = false;
    public bool IsLoop => Left == Right;
    public int Angle = 0;

    private Vector2 Size;
    private TextBox wb;

    public Microsoft.UI.Xaml.Shapes.Path PathObject;

    private readonly System.Drawing.Color DefaultPathStrokeColor = System.Drawing.Color.Gray;

    public CanvasedEdge(GraphNodeControl left, GraphNodeControl right, bool toRight, string weight)
    {
        Left = left;
        Right = right;
        ToRight = toRight;
        wb = new() { Text=weight };

        //if (left == right)
        //{
        //    Size = new Vector2(30, 20);
        //    left.Loops++;
        //    Angle = LoopAngleModifier * (left.Loops - 1);
        //}
        //else
        //{
        //    Size = new Vector2(0.3f, 0.1f);
        //}
    }

    public Microsoft.UI.Xaml.Shapes.Path UpdatePath()
    {
        // Angle between two vertices
        var angle = Math.Atan2(Left.Position.Y - Right.Position.Y, Left.Position.X - Right.Position.X);

        // That angle in degrees
        var _a = angle / Math.PI * 180;

        // Create path constructor
        var path = new Microsoft.UI.Xaml.Shapes.Path() { Stroke = new SolidColorBrush(Color.FromArgb(
            DefaultPathStrokeColor.A,
            DefaultPathStrokeColor.R,
            DefaultPathStrokeColor.G,
            DefaultPathStrokeColor.B
        )), StrokeThickness = 4 };

        // Create geometry that will contain ArcSegment
        var pg = new PathGeometry();

        // Create figure with start point
        var pf = new PathFigure() {
            StartPoint = ToRight ?
                new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius) :
                new Windows.Foundation.Point(Right.Position.X + Right.Radius, Right.Position.Y + Right.Radius),
            IsClosed = false
        };

        // Calculate end point
        Windows.Foundation.Point endPoint;
        if (IsLoop)
        {
            endPoint = ToRight ?
                new Windows.Foundation.Point(Right.Position.X + Right.Radius, Right.Position.Y + Right.Radius) :
                new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius);
        }
        else
        {
            endPoint = new();
        }

        // Create new ArcSegment
        var segment = new ArcSegment() {
            Point = endPoint,
            SweepDirection = ToRight ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
            Size = new Windows.Foundation.Size(Size.X, IsArc ? Size.Y : 0),
            RotationAngle = Left == Right ? Angle : _a,
            IsLargeArc = Left == Right
        };

        // Add segment to figure
        pf.Segments.Add(segment);

        // Add figure to geometry
        pg.Figures.Add(pf);

        // Set geometry as Data of Path object
        path.Data = pg;

        // Save path object in edge
        PathObject = path;

        // Return path object
        return path;
    }
}
