using System;
using System.Windows;
using System.Windows.Media;

namespace ZoomIn.StockControl
{
    public class Candle : ChartShapeBase
    {
        public override void CreateGeometry(StockBar bar, int index, int gap, int width)
        {
            var pathGeometry = new PathGeometry();
            PathFigureCollection pathFigureCollection = new PathFigureCollection();

            var offset = gap + index * (2 * width + gap);
            PathFigure pathFigure1 = new PathFigure();
            pathFigure1.StartPoint = new Point(offset - width, bar.Open);
            pathFigure1.IsClosed = true;
            pathFigure1.IsFilled = true;
            pathFigure1.Segments.Add(new LineSegment(new Point(offset - width, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + width, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + width, bar.Open), true));

            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.StartPoint = new Point(offset, Math.Max(bar.Open, bar.Close));
            pathFigure2.Segments.Add(new LineSegment(new Point(offset, bar.High), true));

            PathFigure pathFigure3 = new PathFigure();
            pathFigure3.StartPoint = new Point(offset, Math.Min(bar.Open, bar.Close));
            pathFigure3.Segments.Add(new LineSegment(new Point(offset, bar.Low), true));

            pathFigureCollection.Add(pathFigure1);
            pathFigureCollection.Add(pathFigure2);
            pathFigureCollection.Add(pathFigure3);

            pathGeometry.Figures = pathFigureCollection;

            geometry = pathGeometry;
        }
    }
}
