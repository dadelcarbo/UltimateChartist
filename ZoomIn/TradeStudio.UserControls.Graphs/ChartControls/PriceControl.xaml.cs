using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.UserControls.Graphs.ChartControls.Indicators;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;
using Label = Telerik.Windows.Controls.Label;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    public enum BarType
    {
        Candle,
        BarChart,
        Line
    }
    /// <summary>
    /// Interaction logic for PriceControl.xaml
    /// </summary>
    public partial class PriceControl : ChartControlBase
    {
        #region Dependency Property BarType
        public BarType BarType
        {
            get { return (BarType)GetValue(BarTypeProperty); }
            set { SetValue(BarTypeProperty, value); }
        }
        public static readonly DependencyProperty BarTypeProperty =
            DependencyProperty.Register("BarType", typeof(BarType), typeof(PriceControl), new PropertyMetadata(BarType.Candle, BarTypeChanged));

        private static void BarTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var priceControl = (PriceControl)d;
            priceControl.OnStockSerieChanged();
        }
        #endregion

        public PriceControl()
        {
            InitializeComponent();
        }
        public override void SetViewModel(ChartViewModel viewModel)
        {
            this.ViewModel = viewModel;

            viewModel.PriceIndicators.CollectionChanged += PriceIndicators_CollectionChanged;
        }

        private void PriceIndicators_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.OnStockSerieChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        private List<IChartShapeBase> shapes = new();
        protected override void OnStockSerieChanged()
        {
            var closeSerie = viewModel?.DataSerie?.CloseValues;
            if (closeSerie == null)
                return;

            var lowSerie = viewModel.DataSerie?.LowValues;
            var highSerie = viewModel.DataSerie?.HighValues;

            this.chartCanvas.Children.Clear();
            shapes.Clear();

            foreach (var indicatorSettings in viewModel.PriceIndicators)
            {
                var ivm = new IndicatorViewModel(indicatorSettings, viewModel.DataSerie);
                this.shapes.AddRange(ivm.Shapes);

                ivm.GeometryChanged += Ivm_GeometryChanged;
            }

            #region Price Candle/Barchart...
            switch (BarType)
            {
                case BarType.Candle:
                    GeneratePriceBars<Candle>();
                    break;
                case BarType.BarChart:
                    GeneratePriceBars<BarChart>();
                    break;
                case BarType.Line:
                    var closeCurve = new Curve()
                    {
                        Stroke = Brushes.DarkGreen,
                        StrokeThickness = 1
                    };
                    closeCurve.CreateGeometry(closeSerie);
                    this.shapes.Add(closeCurve);
                    break;
            }
            #endregion

            this.chartCanvas.Children.AddRange(shapes.SelectMany(s => s.Shapes));
            this.OnResize();
        }

        private void Ivm_GeometryChanged(IndicatorViewModel indicatorViewModel)
        {
            foreach (var shape in indicatorViewModel.Shapes)
            {
                shape.ApplyTransform(chartToPixelTransform);
            }
            //this.shapes.RemoveAll(s => (s as Shape).Tag == indicatorViewModel);
            this.shapes.AddRange(indicatorViewModel.Shapes);
            this.chartCanvas.Children.RemoveAll(s => (s as Shape).Tag == indicatorViewModel);
            this.chartCanvas.Children.AddRange(indicatorViewModel.Shapes.SelectMany(s => s.Shapes));
            return;

            //int shapeIndex = 0;
            //for (int i = 0; i < this.chartCanvas.Children.Count; i++)
            //{
            //    var shape = (this.chartCanvas.Children[i] as Shape);
            //    if (shape != null && shape.Tag == indicatorViewModel)
            //    {
            //        this.chartCanvas.Children[i] = indicatorViewModel.Shapes[shapeIndex++].Shapes;
            //    }
            //}
        }

        private void GeneratePriceBars<T>() where T : BarsShapeBase, new()
        {
            for (int i = 0; i < viewModel.DataSerie.Bars.Count; i++)
            {
                var bar = viewModel.DataSerie.Bars[i];
                BarsShapeBase shape = new T() { StrokeThickness = 1 };
                shape.CreateGeometry(bar, i);

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
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MousePos":
                    this.OnMouseIndexChanged();
                    break;
                default:
                    break;
            }
        }

        private Transform chartToPixelTransform;
        protected override void OnResize()
        {
            var canvasWidth = chartCanvas.ActualWidth;
            var canvasHeight = chartCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (viewModel?.ZoomRange == null || viewModel.ZoomRange.End == 0)
                return;
            var curveWidth = (viewModel.ZoomRange.End - viewModel.ZoomRange.Start + 1);
            if (curveWidth == 0)
                return;
            if (viewModel.DataSerie?.Bars == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;
            var lowSerie = viewModel.DataSerie?.LowValues;
            var highSerie = viewModel.DataSerie?.HighValues;
            for (int i = viewModel.ZoomRange.Start; i <= viewModel.ZoomRange.End; i++)
            {
                min = Math.Min(lowSerie[i], min);
                max = Math.Max(highSerie[i], max);
            }
            min *= 0.98;
            max *= 1.02;
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.ZoomRange.Start + 0.5, -max));
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
            tg.Children.Add(new TranslateTransform(0, -max));
            tg.Children.Add(new ScaleTransform(1, -canvasHeight / curveHeight));
            var horizontalGrid = new HorizontalGrid() { StrokeThickness = 1 };
            horizontalGrid.SetBinding(Shape.StrokeProperty, new Binding("GridBrush"));
            horizontalGrid.CreateGeometry(min, max, gridCanvas.ActualWidth);
            horizontalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(horizontalGrid);

            foreach (var legend in horizontalGrid.Legends)
            {
                var location = tg.Transform(legend.Location);
                var label = new Label() { Content = legend.Text, FontFamily = labelFontFamily, FontSize = 10, Foreground = TextBrush };
                label.Measure(gridCanvas.RenderSize);
                Canvas.SetTop(label, location.Y - label.DesiredSize.Height / 2);
                Canvas.SetLeft(label, -label.DesiredSize.Width);
                this.gridCanvas.Children.Add(label);
            }
            #endregion
            #region Vertical Grid
            tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.ZoomRange.Start + 0.5, 0));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, 1));
            var verticalGrid = new VerticalGrid() { StrokeThickness = 1 };
            verticalGrid.SetBinding(Shape.StrokeProperty, new Binding("GridBrush"));

            verticalGrid.CreateGeometry(viewModel.DataSerie, viewModel.ZoomRange.Start, viewModel.ZoomRange.End, gridCanvas.RenderSize, TradeStudio.Data.DataProviders.BarDuration.Daily); // §§§§
            verticalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(verticalGrid);

            foreach (var legend in verticalGrid.Legends)
            {
                var location = tg.Transform(legend.Location);
                var label = new Label() { Content = legend.Text, FontFamily = labelFontFamily, FontSize = 10, Foreground = TextBrush };
                label.Measure(gridCanvas.RenderSize);
                Canvas.SetTop(label, gridCanvas.ActualHeight - 5);
                Canvas.SetLeft(label, location.X - label.DesiredSize.Width / 2);
                this.gridCanvas.Children.Add(label);
            }
            #endregion
            #endregion
        }

        private void OnMouseIndexChanged()
        {
            mouseCanvas.Children.Clear();
            var mouseCross = new MouseCross();
            mouseCross.CreateGeometry(new Point(viewModel.MousePos.X, 0), mouseCanvas.ActualWidth, mouseCanvas.ActualHeight, false);
            mouseCanvas.Children.Add(mouseCross);
        }

        #region Mouse Events
        private void mouseCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseCanvas.Children.Clear();
            var point = e.GetPosition(sender as IInputElement);
            if (point.X < 0 || point.X > mouseCanvas.ActualWidth || this.chartToPixelTransform?.Inverse == null)
            {
                return;
            }
            var p2 = this.chartToPixelTransform.Inverse.Transform(point);
            this.viewModel.MousePos = point;
            this.viewModel.MouseValue = p2;
            this.viewModel.MouseIndex = Math.Min(Math.Max(0, (int)Math.Round(p2.X)), this.viewModel.MaxIndex);

            var mouseCross = new MouseCross();
            mouseCross.CreateGeometry(point, mouseCanvas.ActualWidth, mouseCanvas.ActualHeight);
            mouseCanvas.Children.Add(mouseCross);

            var label = new Label()
            {
                Content = p2.Y.ToString("0.##"),
                BorderThickness = new Thickness(1),
                Background = Brushes.DarkSlateGray,
                FontFamily = labelFontFamily,
                FontSize = 10,
                Padding = new Thickness(1),
                Foreground = TextBrush,
            };
            label.SetBinding(BorderBrushProperty, new Binding("GridBrush"));
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

        private void mouseCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //if (e.Delta > 0)
            //{
            //    this.viewModel.ZoomRange.End += 50;
            //}
            //else
            //{
            //    this.viewModel.ZoomRange.Start -= 50;
            //}
        }
    }
}
