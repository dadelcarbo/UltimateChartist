using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.ChartControls.Shapes
{
    public class OverviewCurve : Shape
    {
        private Canvas canvas;
        public double[] Values
        {
            get { return (double[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(double[]), typeof(OverviewCurve),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, ValuesPropertyChanged));

        private static void ValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (OverviewCurve)d;
            curve.CreateGeometry();
        }

        double min, max;
        private void CreateGeometry()
        {
            if (Values == null || Values.Length < 2)
            {
                var text = new FormattedText("No Data To Display", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 16, Brushes.Black, 1);
                geometry = text.BuildGeometry(new Point(5, 5));
                geometry.Transform = new MatrixTransform(1, 0, 0, 1, 0, 0);
            }
            else
            {
                var geometryGroup = new GeometryGroup();
                var streamGeometry = new StreamGeometry();

                var values = Values;
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    min = values[0];
                    max = values[0];
                    ctx.BeginFigure(new Point(0, values[0]), false, false);

                    for (int i = 1; i < values.Length; i++)
                    {
                        min = Math.Min(values[i], min);
                        max = Math.Max(values[i], max);
                        ctx.LineTo(new Point(i, values[i]), true /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
                geometry = geometryGroup;
                TransformGeometry();
            }
        }

        private void TransformGeometry()
        {
            if (canvas == null || this.Values == null)
                return;
            var canvasWidth = canvas.ActualWidth;
            var canvasHeight = canvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            var curveWidth = (this.Values.Length - 1);
            if (curveWidth == 0)
                return;
            var values = Values;
            if (values == null)
                return;

            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(0, -max));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, -canvasHeight / curveHeight));
            geometry.Transform = tg;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (Parent != null)
            {
                canvas = (Canvas)Parent;
                TransformGeometry();
                canvas.SizeChanged += Curve_SizeChanged;
            }
            else
            {
                var oldCanvas = (Canvas)oldParent;
                if (oldCanvas != null)
                    oldCanvas.SizeChanged -= Curve_SizeChanged;
            }
        }

        private void Curve_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TransformGeometry();
        }

        private Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }
}
