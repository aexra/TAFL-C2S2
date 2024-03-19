using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using TAFL.Controls;

namespace TAFL.Classes.Graph;
public class CanvasedEdge
{
    public GraphNodeControl Left;
    public GraphNodeControl Right;
    public bool ToRight;
    public string Weight; // a,b,c,...,e

    public Microsoft.UI.Xaml.Shapes.Path PathObject;

    public CanvasedEdge(GraphNodeControl left, GraphNodeControl right, bool toRight, string weight)
    {
        Left = left;
        Right = right;
        ToRight = toRight;
        Weight = weight;
    }

    public Microsoft.UI.Xaml.Shapes.Path UpdatePath()
    {
        var path = new Microsoft.UI.Xaml.Shapes.Path();
        var pd = new PathGeometry();
        var pf = new PathFigure() {
            StartPoint = ToRight ?
                new Windows.Foundation.Point(Left.Position.X, Left.Position.Y) :
                new Windows.Foundation.Point(Right.Position.X, Right.Position.Y),
            IsClosed = false
        };
        pf.Segments.Add(new ArcSegment() { 
            Point = ToRight ?
                new Windows.Foundation.Point(Right.Position.X, Right.Position.Y) :
                new Windows.Foundation.Point(Left.Position.X, Left.Position.Y),
            SweepDirection = ToRight ? SweepDirection.Clockwise : SweepDirection.Counterclockwise 
        });
        pd.Figures.Add(pf);
        path.Data = pd;
        PathObject = path;
        return path;
    }
}
