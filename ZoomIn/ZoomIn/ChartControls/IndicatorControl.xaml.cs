using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using ZoomIn.ChartControls.Shapes;

namespace ZoomIn.ChartControls
{
    /// <summary>
    /// Interaction logic for IndicatorControl.xaml
    /// </summary>
    public partial class IndicatorControl : ChartControlBase
    {
        public IndicatorControl()
        {
            InitializeComponent();
        }

        double[] lowSerie, highSerie;
        protected override void OnStockSerieChanged()
        {
            var closeSerie = viewModel?.Serie?.CloseValues;
            if (closeSerie == null)
                return;

            this.shapes.Clear();
            this.indicatorCanvas.Children.Clear();

            var macd = closeSerie.CalculateMACD(12, 26);
            var macdCurve = new Shapes.Curve()
            {
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 1
            };
            macdCurve.CreateGeometry(macd);
            this.shapes.Add(macdCurve);

            var signal = macd.CalculateEMA(9);
            var signalCurve = new Shapes.Curve()
            {
                Stroke = Brushes.DarkRed,
                StrokeThickness = 1
            };
            signalCurve.CreateGeometry(signal);
            this.shapes.Add(signalCurve);

            this.indicatorCanvas.Children.AddRange(shapes.SelectMany(s => s.Shapes));

            double min = double.MaxValue;
            double max = double.MinValue;
            lowSerie = new double[macd.Length];
            highSerie = new double[macd.Length];
            for (int i = 0; i < macd.Length; i++)
            {
                if (macd[i] < signal[i])
                {
                    lowSerie[i] = macd[i];
                    highSerie[i] = signal[i];
                }
                else
                {
                    lowSerie[i] = signal[i];
                    highSerie[i] = macd[i];
                }
            }
            this.OnResize();
        }

        protected override void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
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

        private List<IStockShapeBase> shapes = new();
        private Transform chartToPixelTransform;
        protected override void OnResize()
        {
            var canvasWidth = indicatorCanvas.ActualWidth;
            var canvasHeight = indicatorCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (viewModel == null || viewModel.Range == null)
                return;
            var curveWidth = (viewModel.ZoomRange.End - viewModel.ZoomRange.Start + 1);
            if (curveWidth == 0)
                return;
            if (viewModel.Serie?.Bars == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = viewModel.ZoomRange.Start; i <= viewModel.ZoomRange.End; i++)
            {
                min = Math.Min(lowSerie[i], min);
                max = Math.Max(highSerie[i], max);
            }
            //min *= 0.98;
            //max *= 1.02;
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.ZoomRange.Start + 0.5, -max));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, -canvasHeight / curveHeight));

            foreach (var shape in shapes)
            {
                shape.ApplyTransform(tg);
            }
            chartToPixelTransform = tg;

            this.gridCanvas.Children.Clear();

            tg = new();
            tg.Children.Add(new TranslateTransform(0, -max));
            tg.Children.Add(new ScaleTransform(1, -canvasHeight / curveHeight));
            var zeroLine = new Path()
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 1,
                Data = new LineGeometry(new Point(0, 0), new Point(canvasWidth, 0), tg)
            };
            this.gridCanvas.Children.Add(zeroLine);

            #region Vertical Grid
            tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.ZoomRange.Start + 0.5, 0));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, 1));
            var verticalGrid = new VerticalGrid() { Stroke = Brushes.LightGray, StrokeThickness = 1 };
            verticalGrid.CreateGeometry(viewModel.Serie, viewModel.ZoomRange.Start, viewModel.ZoomRange.End, gridCanvas.RenderSize);
            verticalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(verticalGrid);
            #endregion
        }

        private void OnMouseIndexChanged()
        {
            mouseCanvas.Children.Clear();
            var mouseCross = new MouseCross() { Stroke = Brushes.Gray, StrokeThickness = 1, StrokeDashArray = { 3, 1 } };
            mouseCross.CreateGeometry(viewModel.Serie, new Point(viewModel.MousePos.X, 0), mouseCanvas.ActualWidth, mouseCanvas.ActualHeight, false);
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
            p2.X = Math.Max(0, Math.Round(p2.X));
            this.viewModel.MousePos = point;
            this.viewModel.MouseValue = p2;
            this.viewModel.MouseIndex = Math.Min(Math.Max(0, (int)Math.Round(p2.X)), this.viewModel.MaxIndex);

            var mouseCross = new MouseCross() { Stroke = Brushes.Gray, StrokeThickness = 1, StrokeDashArray = { 3, 1 } };
            mouseCross.CreateGeometry(viewModel.Serie, point, mouseCanvas.ActualWidth, mouseCanvas.ActualHeight);
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
