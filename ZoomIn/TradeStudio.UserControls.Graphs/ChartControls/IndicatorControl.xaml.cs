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
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using TradeStudio.Common.Extensions;
using TradeStudio.Data.Indicators;
using TradeStudio.UserControls.Graphs.ChartControls.Indicators;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;
using Label = Telerik.Windows.Controls.Label;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    /// <summary>
    /// Interaction logic for IndicatorControl.xaml
    /// </summary>
    public partial class IndicatorControl : ChartControlBase
    {
        IndicatorChartViewModel indicatorChartViewModel;
        public IndicatorChartViewModel IndicatorChartViewModel
        {
            get { return indicatorChartViewModel; }
            set
            {
                if (value == indicatorChartViewModel)
                    return;
                indicatorChartViewModel = value;
                this.OnStockSerieChanged();
            }
        }

        public IndicatorControl()
        {
            InitializeComponent();
        }

        double[] lowSerie, highSerie;
        protected override void OnStockSerieChanged()
        {
            var closeSerie = viewModel?.DataSerie?.CloseValues;
            if (closeSerie == null)
                return;

            this.shapes.Clear();
            this.chartCanvas.Children.Clear();

            this.IndicatorChartViewModel.Indicator.GeometryChanged -= Ivm_GeometryChanged;

            this.IndicatorChartViewModel.Indicator.SetDataSerie(viewModel.DataSerie);
            this.shapes.AddRange(this.IndicatorChartViewModel.Indicator.Shapes);

            this.IndicatorChartViewModel.Indicator.GeometryChanged += Ivm_GeometryChanged;

            this.chartCanvas.Children.AddRange(shapes);

            //var macd = closeSerie.CalculateMACD(12, 26);
            //var macdCurve = new Shapes.Curve()
            //{
            //    Stroke = Brushes.DarkGreen,
            //    StrokeThickness = 1
            //};
            //macdCurve.CreateGeometry(macd);
            //this.shapes.Add(macdCurve);

            //var signal = macd.CalculateEMA(9);
            //var signalCurve = new Shapes.Curve()
            //{
            //    Stroke = Brushes.DarkRed,
            //    StrokeThickness = 1
            //};
            //signalCurve.CreateGeometry(signal);
            //this.shapes.Add(signalCurve);

            //var fill = new Shapes.Fill();
            //fill.CreateGeometry(macd, signal);
            //this.shapes.Add(fill);

            //this.chartCanvas.Children.AddRange(shapes.SelectMany(s => s.Shapes));

            //lowSerie = new double[macd.Length];
            //highSerie = new double[macd.Length];
            //for (int i = 0; i < macd.Length; i++)
            //{
            //    if (macd[i] < signal[i])
            //    {
            //        lowSerie[i] = macd[i];
            //        highSerie[i] = signal[i];
            //    }
            //    else
            //    {
            //        lowSerie[i] = signal[i];
            //        highSerie[i] = macd[i];
            //    }
            //}

            if (this.indicatorChartViewModel.Indicator.Indicator.DisplayType == DisplayType.NonRanged)
            {
                lowSerie = this.IndicatorChartViewModel.Indicator.Indicator.Series.Values.GetLow();
                highSerie = this.IndicatorChartViewModel.Indicator.Indicator.Series.Values.GetHigh();
            }

            this.OnResize();
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


        private void Ivm_GeometryChanged(IndicatorViewModel indicatorViewModel)
        {
            foreach (var shape in indicatorViewModel.Shapes)
            {
                shape.ApplyTransform(chartToPixelTransform);
            }
            //this.shapes.RemoveAll(s => (s as Shape).Tag == indicatorViewModel);
            this.shapes.AddRange(indicatorViewModel.Shapes);
            this.chartCanvas.Children.RemoveAll(s => (s as Shape).Tag == indicatorViewModel);
            this.chartCanvas.Children.AddRange(indicatorViewModel.Shapes);
            return;
        }

        private List<IChartShapeBase> shapes = new();
        private Transform chartToPixelTransform;

        protected override void OnResize()
        {
            var canvasWidth = chartCanvas.ActualWidth;
            var canvasHeight = chartCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            if (viewModel == null)
                return;
            var curveWidth = (viewModel.ZoomRange.End - viewModel.ZoomRange.Start + 1);
            if (curveWidth == 0)
                return;
            if (viewModel.DataSerie?.Bars == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;
            switch (this.indicatorChartViewModel.Indicator.Indicator.DisplayType)
            {
                case DisplayType.Ranged:
                    var rangedIndicator = this.indicatorChartViewModel.Indicator.Indicator as IRangedIndicator;
                    min = rangedIndicator.Minimum;
                    max = rangedIndicator.Maximum;
                    break;
                case DisplayType.NonRanged:

                    for (int i = viewModel.ZoomRange.Start; i <= viewModel.ZoomRange.End; i++)
                    {
                        min = Math.Min(lowSerie[i], min);
                        max = Math.Max(highSerie[i], max);
                    }

                    break;
                default:
                    throw new NotSupportedException($"Indicator display type '{this.indicatorChartViewModel.Indicator.Indicator.DisplayType}'");
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
                StrokeThickness = 1,
                Data = new LineGeometry(new Point(0, 0), new Point(canvasWidth, 0), tg)
            };
            zeroLine.SetBinding(Path.StrokeProperty, new Binding("ChartViewModel.GridBrush"));

            this.gridCanvas.Children.Add(zeroLine);

            #region Vertical Grid
            tg = new();
            tg.Children.Add(new TranslateTransform(-viewModel.ZoomRange.Start + 0.5, 0));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, 1));
            var verticalGrid = new VerticalGrid() { StrokeThickness = 1 };
            verticalGrid.SetBinding(Shape.StrokeProperty, new Binding("ChartViewModel.GridBrush"));
            verticalGrid.CreateGeometry(viewModel.DataSerie, viewModel.ZoomRange.Start, viewModel.ZoomRange.End, gridCanvas.RenderSize, TradeStudio.Data.DataProviders.BarDuration.Daily); // §§§§
            verticalGrid.ApplyTransform(tg);
            this.gridCanvas.Children.Add(verticalGrid);
            #endregion
        }

        private void OnMouseIndexChanged()
        {
            mouseCanvas.Children.Clear();
            var mouseCross = new MouseCross(this.ViewModel.MouseBrush);
            mouseCross.CreateGeometry(new Point(viewModel.MousePos.X, 0), mouseCanvas.ActualWidth, mouseCanvas.ActualHeight, false);
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

            var mouseCross = new MouseCross(this.ViewModel.MouseBrush);
            mouseCross.CreateGeometry(point, mouseCanvas.ActualWidth, mouseCanvas.ActualHeight);
            mouseCanvas.Children.Add(mouseCross);

            var label = new Label()
            {
                Content = p2.Y.ToString("0.##"),
                BorderThickness = new Thickness(1),
                FontFamily = labelFontFamily,
                FontSize = 10,
                Padding = new Thickness(1)
            };
            label.SetBinding(Label.BorderBrushProperty, new Binding("ChartViewModel.GridBrush"));


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
