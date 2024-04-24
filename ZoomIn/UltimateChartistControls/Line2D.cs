using System.Windows;

namespace UltimateChartistControls;

public class Line2D
{
    public Point Point1 { get; set; }
    public Point Point2 { get; set; }

    public double VX { get { return Point2.X - Point1.X; } }
    public double VY { get { return Point2.Y - Point1.Y; } }

    public double a
    {
        get
        {
            if (VX == 0.0f) { return double.MaxValue; }
            else { return VY / VX; }
        }
    }
    public Line2D(Point p1, Point p2)
    {
        Point1 = p1;
        Point2 = p2;
    }
    public Point Intersection(Line2D line)
    {
        var k = line.VX * VY - line.VY * VX;
        if (k == 0) return new Point();
        k = (VY * (Point1.X - line.Point1.X) + VX * (line.Point1.Y - Point1.Y)) / k;
        return new Point(line.Point1.X + k * line.VX, line.Point1.Y + k * line.VY);
    }

    static public Point Intersection(Point Line1P1, Point Line1P2, Point Line2P1, Point Line2P2)
    {
        var line1 = new Line2D(Line1P1, Line1P2);
        var line2 = new Line2D(Line2P1, Line2P2);

        return line1.Intersection(line2);
    }
}
