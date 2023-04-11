using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ZoomIn.StockControl
{
    public class MouseCross : StockShapeBase
    {
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(StockSerie serie, Point point, double width, double height)
        {
            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);
            var geometryGroup = new GeometryGroup();

            var line = new LineGeometry(new Point(0, point.Y), new Point(width, point.Y));
            geometryGroup.Children.Add(line);
            line = new LineGeometry(new Point(point.X, 0), new Point(point.X, height));
            geometryGroup.Children.Add(line);
            //this.Legends.Add(new Legend { Location = new Point(0, val), Text = val.ToString("0.##") });
            geometry = geometryGroup;
        }
    }
}
