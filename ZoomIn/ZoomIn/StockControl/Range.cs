using System.Windows;
using System.Windows.Media;

namespace ZoomIn.StockControl
{
    public class Range : StockShapeBase
    {
        public void CreateGeometry(double[] lows, double[] highs, double gap, double width)
        {
            var geometryGroup = new GeometryGroup();
            var streamGeometry = new StreamGeometry();

            double x = gap;
            double step = 2 * width + gap;
            using (StreamGeometryContext ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(new Point(x, lows[0]), true, true);

                for (int i = 1; i < lows.Length; i++)
                {
                    x += step;
                    ctx.LineTo(new Point(x, lows[i]), true /* is stroked */, false /* is smooth join */);
                }
                ctx.LineTo(new Point(x, highs[highs.Length - 1]), false /* is stroked */, false /* is smooth join */);
                for (int i = highs.Length - 2; i >= 0; i--)
                {
                    x -= step;
                    ctx.LineTo(new Point(x, highs[i]), true /* is stroked */, false /* is smooth join */);
                }
            }
            geometryGroup.Children.Add(streamGeometry);
            geometry = geometryGroup;
        }
    }

}
