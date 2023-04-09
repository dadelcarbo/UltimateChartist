using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls.FieldList;
using ZoomIn.StockDrawing;

namespace ZoomIn.StockControl
{
    public class Legend
    {
        public Point Location { get; set; }
        public string Text { get; set; }
    }
    public class Grid : StockShapeBase
    {
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(StockSerie serie, int gap, int width, int startIndex, int endIndex, double height)
        {
            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);
            var geometryGroup = new GeometryGroup();

            double step = 2 * width + gap;
            double x = gap + startIndex * step;
            int previousMonth = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Month;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                if (bar.Date.Month != previousMonth)
                {
                    previousMonth = bar.Date.Month;
                    var line = new LineGeometry(new Point(x, 0), new Point(x, height));
                    geometryGroup.Children.Add(line);
                    this.Legends.Add(new Legend { Location = new Point(x, 0), Text = bar.Date.ToString("MM/dd") });
                }
                x += step;
                geometry = geometryGroup;
            }
        }
    }
}
