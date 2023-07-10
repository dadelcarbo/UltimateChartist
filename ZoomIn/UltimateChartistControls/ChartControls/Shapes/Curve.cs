using System.Windows;
using System.Windows.Media;

namespace UltimateChartistControls.ChartControls.Shapes
{
    public class Curve : StockShapeBase
    {
        public void CreateGeometry(double[] values)
        {
            var geometryGroup = new GeometryGroup();
            var streamGeometry = new StreamGeometry();

            using (StreamGeometryContext ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(new Point(0, values[0]), false, false);

                for (int i = 1; i < values.Length; i++)
                {
                    ctx.LineTo(new Point(i, values[i]), true /* is stroked */, false /* is smooth join */);
                }
            }
            geometryGroup.Children.Add(streamGeometry);
            geometry = geometryGroup;
        }
    }
}
