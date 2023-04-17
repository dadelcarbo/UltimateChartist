using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ZoomIn.ChartControls.Shapes
{
    public class Legend
    {
        public Point Location { get; set; }
        public string Text { get; set; }
    }
    public class VerticalGrid : StockShapeBase
    {
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(StockSerie serie, int startIndex, int endIndex, double height)
        {
            BarDuration duration = BarDuration.Daily;
            if (!Enum.TryParse<BarDuration>(serie.Name, out duration))
                duration = BarDuration.Daily;

            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);
            var geometryGroup = new GeometryGroup();

            int previousMonth = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Month;
            int previousYear = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Year;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];

                if (bar.Date.Month != previousMonth)
                {
                    previousMonth = bar.Date.Month;
                    var line = new LineGeometry(new Point(i , 0), new Point(i , height));
                    geometryGroup.Children.Add(line);
                    if (previousYear != bar.Date.Year)
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd/MM") + Environment.NewLine + bar.Date.ToString("yyyy") });
                        previousYear = bar.Date.Year;
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd/MM") });
                    }
                }
            }
            geometry = geometryGroup;
        }
    }
}
