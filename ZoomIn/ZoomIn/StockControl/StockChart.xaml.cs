using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Documents.Model.Drawing.Charts;

namespace ZoomIn.StockControl
{
    /// <summary>
    /// Interaction logic for StockChart.xaml
    /// </summary>
    public partial class StockChart : UserControl
    {
        public Brush GridStroke
        {
            get { return (Brush)GetValue(FillShapeProperty); }
            set { SetValue(FillShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillShape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillShapeProperty =
            DependencyProperty.Register("GridStroke", typeof(Brush), typeof(StockChart), new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));


        private ObservableCollection<ChartSeries> series = new ObservableCollection<ChartSeries>();
        public ObservableCollection<ChartSeries> Series { get => series; set { if (series != value) { if (series != null) { series.CollectionChanged -= Series_CollectionChanged; } series = value; series.CollectionChanged += Series_CollectionChanged; } } }

        #region StockBars DependencyProperty
        public StockSerie StockSerie
        {
            get { return (StockSerie)GetValue(StockBarsProperty); }
            set { SetValue(StockBarsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StockBars.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StockBarsProperty =
            DependencyProperty.Register("StockSerie", typeof(StockSerie), typeof(StockChart), new PropertyMetadata(null, OnStockSerieChanged));

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
            stockChart.OnResize();
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
            stockChart.OnResize();
        }
        #endregion

        private Transform chartToPixelTransform;
        private void OnResize()
        {
            var canvasWidth = mainCanvas.ActualWidth;
            var canvasHeight = mainCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (EndIndex == 0)
                return;
            var curveWidth = (this.EndIndex - this.StartIndex + 1) * (2 * width + gap);
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
            //min *= 0.98;
            //max *= 1.02;
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-this.StartIndex * (2 * width + gap) + gap, -max));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, -canvasHeight / curveHeight));

            foreach (var shape in shapes)
            {
                shape.ApplyTransform(tg);
            }
            chartToPixelTransform = tg;

            #region Grid
            this.gridCanvas.Children.Clear();
            #region Horizontal Grid
            tg = new();
            tg.Children.Add(new TranslateTransform(gap, -max));
            tg.Children.Add(new ScaleTransform(1, -canvasHeight / curveHeight));
            var horizontalGrid = new HorizontalGrid() { Stroke = Brushes.LightGray, StrokeThickness = 1 };
            horizontalGrid.CreateGeometry(stockSerie, min, max, gridCanvas.ActualWidth);
            horizontalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(horizontalGrid);

            foreach (var legend in horizontalGrid.Legends)
            {
                var location = tg.Transform(legend.Location);
                var label = new System.Windows.Controls.Label() { Content = legend.Text, FontFamily = labelFontFamily, FontSize = 10 };
                label.Measure(gridCanvas.RenderSize);
                Canvas.SetTop(label, location.Y - label.DesiredSize.Height / 2);
                Canvas.SetLeft(label, -label.DesiredSize.Width);
                this.gridCanvas.Children.Add(label);
            }
            #endregion
            #region Vertical Grid
            tg = new();
            tg.Children.Add(new TranslateTransform(-this.StartIndex * (2 * width + gap) + gap, 0));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, 1));
            var verticalGrid = new VerticalGrid() { Stroke = Brushes.LightGray, StrokeThickness = 1 };
            verticalGrid.CreateGeometry(stockSerie, gap, width, StartIndex, EndIndex, gridCanvas.ActualHeight);
            verticalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(verticalGrid);

            foreach (var legend in verticalGrid.Legends)
            {
                var location = tg.Transform(legend.Location);
                var label = new System.Windows.Controls.Label() { Content = legend.Text, FontFamily = labelFontFamily, FontSize = 10 };
                label.Measure(gridCanvas.RenderSize);
                Canvas.SetTop(label, gridCanvas.ActualHeight - 5);
                Canvas.SetLeft(label, location.X - label.DesiredSize.Width / 2);
                this.gridCanvas.Children.Add(label);
            }
            #endregion
            #endregion
        }

        double gap = 0.2;
        double width = 0.4;

        private StockSerie stockSerie = null;
        private List<IStockShapeBase> shapes = new();
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
            var overviewCurve = new OverviewCurve()
            {
                Values = closeSerie,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2
            };

            this.overviewGraph.Children.Clear();
            this.overviewGraph.Children.Add(overviewCurve);
            #endregion

            this.mainCanvas.Children.Clear();
            shapes.Clear();

            #region Price Indicators (EMA, Cloud, Trail...)

            var fill = new Fill() { };
            fill.CreateGeometry(closeSerie.CalculateEMA(10), closeSerie.CalculateEMA(20), gap, width);
            this.shapes.Add(fill);

            //var range = new Range()
            //{
            //    StrokeThickness = 1,
            //    Stroke = Brushes.Blue,
            //    Fill = Brushes.LightBlue
            //};

            //range.CreateGeometry(closeSerie.CalculateEMA(10), closeSerie.CalculateEMA(20), gap, width);
            //this.shapes.Add(range);

            //var curve = new Curve()
            //{
            //    StrokeThickness = 1,
            //    Stroke = Brushes.DarkOliveGreen
            //};
            //curve.CreateGeometry(closeSerie.CalculateEMA(10), gap, width);
            //this.shapes.Add(curve);
            //curve = new Curve()
            //{
            //    StrokeThickness = 1,
            //    Stroke = Brushes.DarkRed
            //};
            //curve.CreateGeometry(closeSerie.CalculateEMA(20), gap, width);
            //this.shapes.Add(curve);
            #endregion

            #region Price Candle/Barchart...

            double offset = gap;
            for (int i = 0; i < stockSerie.Bars.Length; i++)
            {
                var bar = stockSerie.Bars[i];
                var shape = new Candle() { StrokeThickness = 1, Style = Resources["barStyle"] as Style };
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
            #endregion

            this.mainCanvas.Children.AddRange(shapes.SelectMany(s => s.Shapes));
            this.OnResize();
        }


        public StockChart()
        {
            series.CollectionChanged += Series_CollectionChanged;
            InitializeComponent();

            this.SizeChanged += StockChart_SizeChanged; ;
        }

        private void StockChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.OnResize();
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
                            this.mainCanvas.Children.Add(shape);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var series in e.OldItems.Cast<ChartSeries>())
                    {
                        foreach (var shape in series.Shapes)
                        {
                            this.mainCanvas.Children.Remove(shape);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.mainCanvas.Children.Clear();
                    break;
                default:
                    break;
            }
        }

        #region Mouse Events
        FontFamily labelFontFamily = new FontFamily("Calibri");
        private void mouseCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.GetPosition(sender as IInputElement);
            var p2 = this.chartToPixelTransform.Inverse.Transform(point);
            p2.X = Math.Round(p2.X - gap);
            this.MousePos = point;
            this.MouseValue = p2;

            mouseCanvas.Children.Clear();
            var mouseCross = new MouseCross() { Stroke = Brushes.Gray, StrokeThickness = 1, StrokeDashArray = { 3, 1 } };
            mouseCross.CreateGeometry(this.StockSerie, point, mouseCanvas.ActualWidth, mouseCanvas.ActualHeight);
            mouseCanvas.Children.Add(mouseCross);

            var label = new System.Windows.Controls.Label()
            {
                Content = p2.Y.ToString("0.##"),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Background = Brushes.Goldenrod,
                FontFamily = labelFontFamily,
                FontSize = 10, Padding = new Thickness(1)
            };
            label.Measure(mouseCanvas.RenderSize);
            Canvas.SetTop(label, point.Y - label.DesiredSize.Height / 2);
            Canvas.SetLeft(label, -label.DesiredSize.Width);
            this.mouseCanvas.Children.Add(label);
        }

        private void mouseCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseCanvas.Children.Clear();
        }

        private void mouseCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void mouseCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        #endregion



        public Point MousePos
        {
            get { return (Point)GetValue(MousePosProperty); }
            set { SetValue(MousePosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MousePos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePosProperty =
            DependencyProperty.Register("MousePos", typeof(Point), typeof(StockChart), new PropertyMetadata(new Point()));

        public Point MouseValue
        {
            get { return (Point)GetValue(MouseValueProperty); }
            set { SetValue(MouseValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseValueProperty =
            DependencyProperty.Register("MouseValue", typeof(Point), typeof(StockChart), new PropertyMetadata(new Point()));


    }
}
