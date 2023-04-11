using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public class Curve : StockShapeBase
    {
        public void CreateGeometry(double[] values, double gap, double width)
        {
            var geometryGroup = new GeometryGroup();
            var streamGeometry = new StreamGeometry();

            double x = gap;
            double step = 2 * width + gap;
            using (StreamGeometryContext ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(new Point(x, values[0]), false, false);

                for (int i = 1; i < values.Length; i++)
                {
                    x += step;
                    ctx.LineTo(new Point(x, values[i]), true /* is stroked */, false /* is smooth join */);
                }
            }
            geometryGroup.Children.Add(streamGeometry);
            geometry = geometryGroup;
        }
    }
}
