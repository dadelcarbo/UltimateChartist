using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    class CurveSeries : ChartSeries
    {
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(CurveSeries), new PropertyMetadata(Brushes.Black, OnStrokeChanged));

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartSeries = (CurveSeries)d;
            foreach(var shape in chartSeries.Shapes.Cast<Curve>())
            {
                shape.Stroke = (Brush)e.NewValue;
            }
        }

        public override void OnItemsSourceChanged(IEnumerable oldSource, IEnumerable newSource)
        {
            if (StockChart != null)
            {
                StockChart.Series.Remove(this);
            }
            this.Shapes.Clear();
            this.Shapes.Add(new Curve() { Values = newSource as double[], Stroke = this.Stroke }) ;
        }
    }
}
