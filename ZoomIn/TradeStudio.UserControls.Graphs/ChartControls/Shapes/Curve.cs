using TradeStudio.Common.Extensions;
using System.Windows;
using System.Windows.Media;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class Curve : ChartShapeBase
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
        public void CreateGeometry(double?[] values)
        {
            var geometryGroup = new GeometryGroup();

            var indexes = values.GetNotNullIndexes();
            foreach (var item in indexes)
            {
                var streamGeometry = new StreamGeometry();
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    ctx.BeginFigure(new Point(item.Start, values[item.Start].Value), false, false);

                    for (int i = item.Start + 1; i <= item.End; i++)
                    {
                        ctx.LineTo(new Point(i, values[i].Value), true /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
            }
            geometry = geometryGroup;
        }
    }
}
