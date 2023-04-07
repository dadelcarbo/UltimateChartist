using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ZoomIn.StockControl
{
    /// <summary>
    /// Interaction logic for StockChart.xaml
    /// </summary>
    public partial class StockChart : UserControl
    {
        private ObservableCollection<ChartSeries> series = new ObservableCollection<ChartSeries>();
        public ObservableCollection<ChartSeries> Series { get => series; set { if (series != value) { if (series != null) { series.CollectionChanged -= Series_CollectionChanged; } series = value; series.CollectionChanged += Series_CollectionChanged; } } }

        private ChartSeries mainSeries;

        #region StockBars DependencyProperty
        public StockBar[] StockBars
        {
            get { return (StockBar[])GetValue(StockBarsProperty); }
            set { SetValue(StockBarsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StockBars.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StockBarsProperty =
            DependencyProperty.Register("StockBars", typeof(StockBar[]), typeof(StockChart), new PropertyMetadata(null, OnStockBarsChanged));

        private static void OnStockBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (StockChart)d;
            stockChart.OnStockBarsChanged((StockBar[])e.NewValue);
        }
        #endregion

        #region

        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(StockChart),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, StartIndexPropertyChanged));

        private static void StartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (StockChart)d;
            stockChart.TransformGeometry();
        }
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(StockChart),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, EndIndexPropertyChanged));

        private static void EndIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (StockChart)d;
            stockChart.TransformGeometry();
        }
        #endregion

        private void TransformGeometry()
        {
            throw new NotImplementedException();
        }

        private void OnStockBarsChanged(StockBar[] stockBars)
        {
            var closeSerie = stockBars?.Select(b => b.Close)?.ToArray();
            if (closeSerie == null)
                return;

            var viewModel = (StockChartViewModel)this.Resources["viewModel"];
            viewModel.Bars = stockBars;

            var curve = new Curve()
            {
                Values = closeSerie,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2,
                StartIndex = 0
            };
            curve.SetBinding(Curve.EndIndexProperty, new Binding("MaxIndex") { Mode = BindingMode.OneWay });

            this.overviewGraph.Children.Clear();
            this.overviewGraph.Children.Add(curve); 
            
            curve = new Curve()
            {
                Values = closeSerie,
                Stroke = Brushes.DarkRed,
                StrokeThickness = 2
            };
            curve.SetBinding(Curve.StartIndexProperty, new Binding("StartIndex") { Mode = BindingMode.OneWay });
            curve.SetBinding(Curve.EndIndexProperty, new Binding("EndIndex") { Mode = BindingMode.OneWay });
            this.mainGraph.Children.Clear();
            this.mainGraph.Children.Add(curve);


            var ohlc = new OHLCSeries()
            {
                Values = stockBars,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2
            };
            ohlc.SetBinding(OHLCSeries.StartIndexProperty, new Binding("StartIndex") { Mode = BindingMode.OneWay });
            ohlc.SetBinding(OHLCSeries.EndIndexProperty, new Binding("EndIndex") { Mode = BindingMode.OneWay });
            this.mainGraph.Children.Add(ohlc);
        }

        public StockChart()
        {
            series.CollectionChanged += Series_CollectionChanged;
            InitializeComponent();
        }

        private void Series_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var series in e.NewItems.Cast<ChartSeries>())
                    {
                        foreach (var shape in series.Shapes)
                        {
                            this.mainGraph.Children.Add(shape);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var series in e.OldItems.Cast<ChartSeries>())
                    {
                        foreach (var shape in series.Shapes)
                        {
                            this.mainGraph.Children.Remove(shape);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.mainGraph.Children.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
