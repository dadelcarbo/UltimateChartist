using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class MouseCross : ChartShapeBase
    {
        public MouseCross()
        {
            this.SetBinding(StrokeProperty, new Binding("MouseBrush"));
            StrokeThickness = 1;
            StrokeDashArray = new DoubleCollection() { 3, 2 };
            this.IsHitTestVisible = false;
            this.SnapsToDevicePixels = false;
        }
        public MouseCross(Brush brush)
        {
            Stroke = brush;
            StrokeThickness = 1;
            StrokeDashArray = new DoubleCollection() { 3, 2 };
            this.IsHitTestVisible = false;
            this.SnapsToDevicePixels = false;
        }
        public void CreateGeometry(Point point, double width, double height, bool addHorizontal = true)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            var geometryGroup = new GeometryGroup();
            LineGeometry line;

            line = new LineGeometry(new Point(point.X, 0), new Point(point.X, height));
            geometryGroup.Children.Add(line);
            if (addHorizontal)
            {
                line = new LineGeometry(new Point(0, point.Y), new Point(width, point.Y));
                geometryGroup.Children.Add(line);
            }

            geometry = geometryGroup;
        }
    }
}
