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
using Windows.Foundation;
using Windows.UI;

namespace TAFL.Classes.Graph;
public class CanvasedEdge
{
    public static readonly int LoopAngleModifier = 60;
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
        var AngleD = angle / Math.PI * 180; // That angle in degrees

        // Create empty path
        var path = GetPath();

        // Create Arc from left vertex to right vertex
        var arcGeometry = new PathGeometry(); // Create geometry that will contain arc figure
        var arcFigure = GetPathFigure(); // Create figure with start point
        var endPoint = GetEndPoint(); // Calculate end point
        var segment = GetArcSegment(endPoint, AngleD); // Create new ArcSegment
        arcFigure.Segments.Add(segment); // Add segment to figure
        arcGeometry.Figures.Add(arcFigure); // Add figure to geometry

        // Create arrow at center of arc
        var asp = GetArrowStartPoint();
        var arrowGeometry = new PathGeometry();
        var arrowFigureL = GetArrowFigure(out var spl);
        var arrowFigureR = GetArrowFigure(out var spr);
        var arrowSegmentL = GetArrowLineSegment(true, spl) ;
        var arrowSegmentR = GetArrowLineSegment(false, spr);
        arrowFigureL.Segments.Add(arrowSegmentL);
        arrowFigureR.Segments.Add(arrowSegmentR);
        arrowGeometry.Figures.Add(arrowFigureL);
        arrowGeometry.Figures.Add(arrowFigureR);
        var tmpa = AngleD + 90;
        if (tmpa > 1 || tmpa < -1)
        {
            var arrowTransformGroup = new TransformGroup();
            arrowTransformGroup.Children.Add(new RotateTransform() { Angle = tmpa, CenterX = asp.X, CenterY = asp.Y - 5 });
            arrowGeometry.Transform = arrowTransformGroup;
        }

        // All figures ready to be set
        var geometryGroup = new GeometryGroup(); // Create geomtry group to contain all geometry objects
        geometryGroup.Children.Add(arcGeometry); // Add arc geomtry group
        geometryGroup.Children.Add(arrowGeometry); // Add arrow geomtry group
        path.Data = geometryGroup; // Set geometryGroup as Data of Path object
        PathObject = path; // Save path object in edge
        return path; // Return path object
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
            Size = new Vector2(20, 30);
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
    private Point GetEndPoint()
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
    private Point CalculateLoopedEndPoint()
    {
        var EndPoint = new Windows.Foundation.Point(Left.Position.X + Left.Radius, Left.Position.Y + Left.Radius);
        var AngleD = LoopAngleModifier * LoopIndex;
        var AnglePI = (double)AngleD / 180 * Math.PI;
        EndPoint.X += Math.Cos(AnglePI);
        EndPoint.Y += Math.Sin(AnglePI);
        return EndPoint;
    }
    private ArcSegment GetArcSegment(Point endPoint, double angle)
    {
        return new ArcSegment() {
            Point = endPoint,
            SweepDirection = SweepDirection.Clockwise,
            Size = new Windows.Foundation.Size(Size.X, Size.Y),
            RotationAngle = IsLoop ? LoopAngleModifier * LoopIndex : angle,
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

    // ARROW METHODS
    private PathFigure GetArrowFigure(out Point sp)
    {
        return new(){ IsClosed = false, StartPoint = sp = GetArrowStartPoint() };
    }
    private Point GetArrowStartPoint()
    {
        if (IsLoop)
        {
            return new(100, 100);
        }
        else
        {
            return new((Left.Center.X + Right.Center.X) / 2, (Left.Center.Y + Right.Center.Y) / 2 + 4);
        }
    }
    private LineSegment GetArrowLineSegment(bool leftSide, Point sp)
    {
        if (leftSide)
        {
            return new() { Point=new(sp.X - 10, sp.Y - 10) };
        }
        else
        {
            return new() { Point=new(sp.X + 10, sp.Y - 10) };
        }
    }
}
