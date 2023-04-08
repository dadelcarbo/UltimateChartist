using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace ZoomIn.StockControl
{
    /// <summary>
    /// Interaction logic for StockChart.xaml
    /// </summary>
    public partial class StockChart : UserControl
    {
        private ObservableCollection<ChartSeries> series = new ObservableCollection<ChartSeries>();
        public ObservableCollection<ChartSeries> Series { get => series; set { if (series != value) { if (series != null) { series.CollectionChanged -= Series_CollectionChanged; } series = value; series.CollectionChanged += Series_CollectionChanged; } } }

        #region StockBars DependencyProperty
        public StockSerie StockSerie
        {
            get { return (StockSerie)GetValue(StockBarsProperty); }
            set { SetValue(StockBarsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StockBars.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StockBarsProperty = DependencyProperty.Register("StockSerie", typeof(StockSerie), typeof(StockChart), new PropertyMetadata(null, OnStockSerieChanged));

        private static void OnStockSerieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (StockChart)d;
            stockChart.OnStockSerieChanged((StockSerie)e.NewValue);
        }
        #endregion

        #region StartIndex & EndIndex 

        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(StockChart),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, StartIndexPropertyChanged));

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
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EndIndexPropertyChanged));

        private static void EndIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stockChart = (StockChart)d;
            stockChart.TransformGeometry();
        }
        #endregion

        private void TransformGeometry()
        {
            var canvasWidth = mainGraph.ActualWidth;
            var canvasHeight = mainGraph.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (EndIndex == 0) 
                return;
            var curveWidth = (this.EndIndex - this.StartIndex + 1) * (2 * width + gap) + gap;
            if (curveWidth == 0)
                return;
            if (this.StockSerie?.Bars == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = StartIndex; i <= EndIndex; i++)
            {
                min = Math.Min(lowSerie[i], min);
                max = Math.Max(highSerie[i], max);
            }
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-this.StartIndex * (2 * width + gap) + gap, -min));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, canvasHeight / curveHeight));

            foreach (var shape in shapes.OfType<ChartShapeBase>())
            {
                shape.ApplyTranform(tg);
            }
        }
        int gap = 2;
        int width = 2;

        private StockSerie stockSerie = null;
        private List<Shape> shapes = new List<Shape>();
        private double[] lowSerie;
        private double[] highSerie;
        private void OnStockSerieChanged(StockSerie stockSerie)
        {
            if (this.stockSerie == stockSerie)
                return;
            this.stockSerie = stockSerie;

            var closeSerie = stockSerie?.CloseValues;
            if (closeSerie == null)
                return;
            lowSerie = stockSerie?.LowValues;
            highSerie = stockSerie?.HighValues;

            var viewModel = (StockChartViewModel)this.Resources["viewModel"];
            viewModel.Bars = stockSerie.Bars;

            this.EndIndex = viewModel.EndIndex;
            this.StartIndex = viewModel.StartIndex;


            #region Create overview
            var curve = new OverviewCurve()
            {
                Values = closeSerie,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2
            };

            this.overviewGraph.Children.Clear();
            this.overviewGraph.Children.Add(curve);
            #endregion

            this.mainGraph.Children.Clear();
            shapes.Clear();

            int offset = gap;
            for (int i = 0; i < stockSerie.Bars.Length; i++)
            {
                var bar = stockSerie.Bars[i];
                var shape = new Candle() { StrokeThickness = 1 };
                shape.CreateGeometry(bar, i, gap, width);

                if (bar.Close >= bar.Open)
                {
                    shape.Stroke = Brushes.DarkGreen;
                    shape.Fill = Brushes.Green;
                }
                else
                {
                    shape.Stroke = Brushes.DarkRed;
                    shape.Fill = Brushes.Red;
                }
                shapes.Add(shape);
            }
            this.mainGraph.Children.AddRange(shapes);

            this.TransformGeometry();
        }

        public StockChart()
        {
            series.CollectionChanged += Series_CollectionChanged;
            InitializeComponent();

            this.SizeChanged += StockChart_SizeChanged; ;
        }

        private void StockChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TransformGeometry();
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
