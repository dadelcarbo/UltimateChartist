using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZoomIn.ChartControls
{
    public abstract class ChartControlBase : UserControl
    {
        protected FontFamily labelFontFamily = new FontFamily("Calibri");
        #region StockSerie DependencyProperty
        public StockSerie StockSerie
        {
            get { return (StockSerie)GetValue(StockBarsProperty); }
            set { SetValue(StockBarsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StockBars.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StockBarsProperty =
            DependencyProperty.Register("StockSerie", typeof(StockSerie), typeof(ChartControlBase), new PropertyMetadata(null, OnStockSerieChanged));

        private static void OnStockSerieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (ChartControlBase)d;
            stockChart.OnStockSerieChanged((StockSerie)e.NewValue);
        }

        protected abstract void OnStockSerieChanged(StockSerie newSerie);
        #endregion
    }
}
