using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public abstract class ChartSeries : Control
    {
        public StockChart StockChart { get; private set; }

        public List<Shape> Shapes { get; protected set; } = new List<Shape>();

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ChartSeries), new PropertyMetadata(null, OnItemsSourceChanged));


        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries chartSeries = (ChartSeries)d;
            chartSeries.OnItemsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
        }
        public abstract void OnItemsSourceChanged(IEnumerable oldSource, IEnumerable newSource);

    }
}
