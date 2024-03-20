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
    public static readonly int LoopAngleModifier = 10;
    public GraphNodeControl Left;
    public GraphNodeControl Right;
    public string Weight => wb.Text; // a,b,c,...,e
    public bool IsLoop => Left == Right;
    public int LoopIndex = 0;

    public Vector2 Size;
    private TextBox wb;

    public Microsoft.UI.Xaml.Shapes.Path PathObject;

    private readonly System.Drawing.Color DefaultPathStrokeColor = System.Drawing.Color.Gray;

    public CanvasedEdge(GraphNodeControl left, GraphNodeControl right, string weight)
    {
        Left = left;
        Right = right;
        wb = new() { Text=weight };

        CalculateArcSize();
    }

    public Microsoft.UI.Xaml.Shapes.Path UpdatePath()
    {
        // Angle between two vertices
        var angle = Math.Atan2(Left.Position.Y - Right.Position.Y, Left.Position.X - Right.Position.X);

        // That angle in degrees
        var _a = angle / Math.PI * 180;

        // Create path constructor
        var path = GetPath();

        // Create geometry that will contain ArcSegment
        var pg = new PathGeometry();

        // Create figure with start point
        var pf = GetPathFigure();

        // Calculate end point
        var endPoint = GetEndPoint();

        // Create new ArcSegment
        var segment = GetArcSegment(endPoint, _a);

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
    public void ToArc()
    {
        Size.Y = 0.1f;
    }
    public void ToLine()
    {
        Size.Y = 0;
    }
    private void CalculateArcSize()
    {
        if (IsLoop)
        {
            Size = new Vector2(30, 20);
            LoopIndex = Left.Loops;
            Left.Loops++;
        }
        else
        {
            Size = new Vector2(0.3f, 0);
        }
    }
    private Microsoft.UI.Xaml.Shapes.Path GetPath()
    {
        return new Microsoft.UI.Xaml.Shapes.Path() {
            Stroke = new SolidColorBrush(Color.FromArgb(
            DefaultPathStrokeColor.A,
            DefaultPathStrokeColor.R,
            DefaultPathStrokeColor.G,
            DefaultPathStrokeColor.B )), StrokeThickness = 4 
        };
    }
    private Windows.Foundation.Point GetEndPoint()
    {
        if (IsLoop)
        {
            return CalculateLoopedEndPoint();
        }
        else
        {
            return new(Right.Position.X + Right.Radius, Right.Position.Y + Right.Radius);
        }
    }
    private Windows.Foundation.Point CalculateLoopedEndPoint()
    {
        var EndPoint = new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius);
        var AngleD = LoopAngleModifier * LoopIndex;
        var AnglePI = AngleD / 180 * Math.PI;
        EndPoint.X += Math.Cos(AnglePI);
        EndPoint.Y += Math.Sin(AnglePI);
        return EndPoint;
    }
    private ArcSegment GetArcSegment(Windows.Foundation.Point endPoint, double angle)
    {
        return new ArcSegment() {
            Point = endPoint,
            SweepDirection = SweepDirection.Clockwise,
            Size = new Windows.Foundation.Size(Size.X, Size.Y),
            RotationAngle = IsLoop ? 0 : angle,
            IsLargeArc = IsLoop
        };
    }
    private PathFigure GetPathFigure()
    {
        return new PathFigure()
        {
            StartPoint = new(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius),
            IsClosed = false
        };
    }
}
