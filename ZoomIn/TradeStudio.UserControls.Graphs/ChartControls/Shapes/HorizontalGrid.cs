using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class HorizontalGrid : BarsShapeBase
    {
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(double min, double max, double width)
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            var geometryGroup = new GeometryGroup();

            var step = Math.Pow(10, Math.Floor(Math.Log10((max - min))));
            if ((max - min) / step < 3)
            {
                step /= 4;
            }
            else if ((max - min) / step < 6)
            {
                step /= 2;
            }
            else
            {
                if ((max - min) / step > 13)
                {
                    step *= 4;
                }
                else if ((max - min) / step > 7)
                {
                    step *= 2;
                }
            }
            var val = min < 0 ? -Math.Pow(10, Math.Ceiling(Math.Log10((Math.Abs(min))))) : Math.Pow(10, Math.Floor(Math.Log10((min))));

            if (val > 0 && step > val) val = step;
            if (val < 0 && step > Math.Abs(val)) val = -step;

            while (val < max)
            {
                if (val > min)
                {
                    var line = new LineGeometry(new Point(0, val), new Point(width, val));
                    geometryGroup.Children.Add(line);
                    this.Legends.Add(new Legend { Location = new Point(0, val), Text = val.ToString("0.#####") });
                }
                val += step;
            }
            geometry = geometryGroup;
        }
    }
}
