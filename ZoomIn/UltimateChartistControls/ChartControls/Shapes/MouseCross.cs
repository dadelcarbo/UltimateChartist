using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using UltimateChartistLib;

namespace UltimateChartistControls.ChartControls.Shapes
{
    public class MouseCross : StockShapeBase
    {
        public void CreateGeometry(StockSerie serie, Point point, double width, double height, bool addHorizontal = true)
        {
            this.IsHitTestVisible = false;
            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);

            var geometryGroup = new GeometryGroup();
            LineGeometry line;

            if (addHorizontal)
            {
                line = new LineGeometry(new Point(0, point.Y), new Point(width, point.Y));
                geometryGroup.Children.Add(line);
            }
            line = new LineGeometry(new Point(point.X, 0), new Point(point.X, height));
            geometryGroup.Children.Add(line);

            geometry = geometryGroup;
        }
    }
}
