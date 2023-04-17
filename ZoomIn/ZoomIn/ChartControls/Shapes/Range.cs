using System.Windows;
using System.Windows.Media;

namespace ZoomIn.ChartControls.Shapes
{
    public class Range : StockShapeBase
    {
        public void CreateGeometry(double[] lows, double[] highs)
        {
            var geometryGroup = new GeometryGroup();
            var streamGeometry = new StreamGeometry();

            using (StreamGeometryContext ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(new Point(0, lows[0]), true, true);

                for (int i = 1; i < lows.Length; i++)
                {
                    ctx.LineTo(new Point(i, lows[i]), true /* is stroked */, false /* is smooth join */);
                }
                ctx.LineTo(new Point(highs.Length - 1, highs[highs.Length - 1]), false /* is stroked */, false /* is smooth join */);
                for (int i = highs.Length - 2; i >= 0; i--)
                {
                    ctx.LineTo(new Point(i, highs[i]), true /* is stroked */, false /* is smooth join */);
                }
            }
            geometryGroup.Children.Add(streamGeometry);
            geometry = geometryGroup;
        }
    }

}
