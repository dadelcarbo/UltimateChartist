using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ZoomIn.StockControl
{
    public class Legend
    {
        public Point Location { get; set; }
        public string Text { get; set; }
    }
    public class VerticalGrid : StockShapeBase
    {
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(StockSerie serie, int gap, int width, int startIndex, int endIndex, double height)
        {
            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);
            var geometryGroup = new GeometryGroup();

            double step = 2 * width + gap;
            double x = gap + startIndex * step;
            int previousMonth = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Month;
            int previousYear = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Year;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                if (bar.Date.Month != previousMonth)
                {
                    previousMonth = bar.Date.Month;
                    var line = new LineGeometry(new Point(x, 0), new Point(x, height));
                    geometryGroup.Children.Add(line);
                    if (previousYear != bar.Date.Year)
                    {
                        this.Legends.Add(new Legend { Location = new Point(x, 0), Text = bar.Date.ToString("yyyy") + Environment.NewLine + bar.Date.ToString("dd/MM") });
                        previousYear = bar.Date.Year;
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(x, 0), Text = bar.Date.ToString("dd/MM") });
                    }
                }
                x += step;
            }
            geometry = geometryGroup;
        }
    }
}
