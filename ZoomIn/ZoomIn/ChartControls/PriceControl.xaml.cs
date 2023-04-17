using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using ZoomIn.ChartControls.Shapes;

namespace ZoomIn.ChartControls
{
    /// <summary>
    /// Interaction logic for PriceControl.xaml
    /// </summary>
    public partial class PriceControl : ChartControlBase
    {
        ChartControlViewModel viewModel;

        public PriceControl()
        {
            InitializeComponent();

            this.SizeChanged += PriceControl_SizeChanged;
        }

        private void PriceControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.OnResize();
        }
        private List<IStockShapeBase> shapes = new();
        private StockSerie stockSerie;
        protected override void OnStockSerieChanged(StockSerie newSerie)
        {
            if (viewModel == null)
            {
                viewModel = (ChartControlViewModel)this.DataContext;
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            if (this.stockSerie == newSerie)
                return;
            this.stockSerie = newSerie;

            var closeSerie = stockSerie?.CloseValues;
            if (closeSerie == null)
                return;

            var lowSerie = stockSerie?.LowValues;
            var highSerie = stockSerie?.HighValues;

            this.chartCanvas.Children.Clear();
            shapes.Clear();

            #region Price Indicators (EMA, Cloud, Trail...)

            var fill = new Fill() { };
            fill.CreateGeometry(closeSerie.CalculateEMA(12), closeSerie.CalculateEMA(26));
            this.shapes.Add(fill);

            #endregion

            #region Price Candle/Barchart...

            for (int i = 0; i < stockSerie.Bars.Length; i++)
            {
                var bar = stockSerie.Bars[i];
                var shape = new Shapes.Candle() { StrokeThickness = 1 };
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
            #endregion

            this.chartCanvas.Children.AddRange(shapes.SelectMany(s => s.Shapes));
            this.OnResize();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StartIndex":
                case "EndIndex":
                    this.OnResize();
                    break;
                case "MousePos":
                    this.OnMouseIndexChanged();
                    break;
                case "Serie":
                    this.OnStockSerieChanged(viewModel.Serie);
                    break;
                default:
                    break;
            }
        }

        private Transform chartToPixelTransform;
        private void OnResize()
        {
            var canvasWidth = chartCanvas.ActualWidth;
            var canvasHeight = chartCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (viewModel == null || viewModel.EndIndex == 0)
                return;
            var curveWidth = (viewModel.EndIndex - viewModel.StartIndex + 1);
            if (curveWidth == 0)
                return;
            if (this.StockSerie?.Bars == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;
            var lowSerie = stockSerie?.LowValues;
            var highSerie = stockSerie?.HighValues;
            for (int i = viewModel.StartIndex; i <= viewModel.EndIndex; i++)
            {
                min = Math.Min(lowSerie[i], min);
                max = Math.Max(highSerie[i], max);
            }
            //min *= 0.98;
            //max *= 1.02;
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.StartIndex + 0.5, -max));
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
            tg.Children.Add(new TranslateTransform(-viewModel.StartIndex + 0.5, 0));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, 1));
            var verticalGrid = new VerticalGrid() { Stroke = Brushes.LightGray, StrokeThickness = 1 };
            verticalGrid.CreateGeometry(stockSerie, viewModel.StartIndex, viewModel.EndIndex, gridCanvas.ActualHeight);
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

        private void OnMouseIndexChanged()
        {
            mouseCanvas.Children.Clear();
            var mouseCross = new MouseCross() { Stroke = Brushes.Gray, StrokeThickness = 1, StrokeDashArray = { 3, 1 } };
            mouseCross.CreateGeometry(this.StockSerie, new Point(viewModel.MousePos.X, 0), mouseCanvas.ActualWidth, mouseCanvas.ActualHeight, false);
            mouseCanvas.Children.Add(mouseCross);
        }

        #region Mouse Events
        private void mouseCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseCanvas.Children.Clear();
            var point = e.GetPosition(sender as IInputElement);
            if (point.X < 0 || point.X > mouseCanvas.ActualWidth)
            {
                return;
            }
            var p2 = this.chartToPixelTransform.Inverse.Transform(point);
            this.viewModel.MousePos = point;
            this.viewModel.MouseValue = p2;
            this.viewModel.MouseIndex = Math.Min(Math.Max(0, (int)Math.Round(p2.X)), this.viewModel.MaxIndex);

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
                FontSize = 10,
                Padding = new Thickness(1)
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
    }
}
