using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace ZoomIn
{
    public class Curve : Shape
    {
        private Canvas canvas;
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(Curve),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, StartIndexPropertyChanged));

        private static void StartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (Curve)d;
            curve.TransformGeometry();
        }
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(Curve),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, EndIndexPropertyChanged));

        private static void EndIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (Curve)d;
            curve.TransformGeometry();
        }

        public double[] Values
        {
            get { return (double[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(double[]), typeof(Curve),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, ValuesPropertyChanged));

        private static void ValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (Curve)d;
            curve.CreateGeometry();
        }

        private void CreateGeometry()
        {
            if (Values == null || this.Values.Length < 2)
            {
                var text = new FormattedText("No Data To Display", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 16, Brushes.Black, 1);
                geometry = text.BuildGeometry(new Point(5, 5));
                geometry.Transform = new MatrixTransform(1, 0, 0, -1, 0, 0);
            }
            else
            {
                var geometryGroup = new GeometryGroup();
                var streamGeometry = new StreamGeometry();

                double x = 0;
                var values = this.Values;
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    ctx.BeginFigure(new Point(x, values[0]), false, false);

                    for (int i = 1; i < values.Length; i++)
                    {
                        x += width;
                        ctx.LineTo(new Point(x, values[i]), true /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
                this.geometry = geometryGroup;
                this.TransformGeometry();
            }
        }

        private void TransformGeometry()
        {
            var canvasWidth = canvas.ActualWidth;
            var canvasHeight = canvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            var curveWidth = (this.EndIndex - this.StartIndex) * width;
            if (curveWidth == 0)
                return;
            var values = this.Values;
            if (values == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = StartIndex; i <= EndIndex; i++)
            {
                min = Math.Min(values[i], min);
                max = Math.Max(values[i], max);
            }
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-this.StartIndex * width, -min));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, canvasHeight / curveHeight));
            geometry.Transform = tg;
        }

        double width = 1;
        double gap = 5;

        public Curve()
        {
            this.CreateGeometry();
        }
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (this.Parent != null && oldParent == null)
            {
                canvas = (Canvas)this.Parent;
                canvas.SizeChanged += Curve_SizeChanged;
                canvas.MouseMove += Canvas_MouseMove;
            }
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var viewModel = this.DataContext as ChartViewModel;
            var position = Mouse.GetPosition(canvas);
            var inverseTransform = geometry.Transform.Inverse;
            viewModel.MousePoint = inverseTransform.Transform(position);
        }

        private void Curve_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TransformGeometry();
        }

        private Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }
}
