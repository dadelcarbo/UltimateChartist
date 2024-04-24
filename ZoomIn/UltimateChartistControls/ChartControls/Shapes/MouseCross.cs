using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class MouseCross : BarsShapeBase
    {
        public void CreateGeometry(Point point, double width, double height, bool addHorizontal = true)
        {
            this.IsHitTestVisible = false;
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

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
