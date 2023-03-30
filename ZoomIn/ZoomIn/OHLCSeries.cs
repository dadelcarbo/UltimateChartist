using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn
{
    class OHLCSeries : Shape
    {
        private Canvas canvas;
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(OHLCSeries),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, StartIndexPropertyChanged));

        private static void StartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (OHLCSeries)d;
            curve.TransformGeometry();
        }
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(OHLCSeries),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, EndIndexPropertyChanged));

        private static void EndIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (OHLCSeries)d;
            curve.TransformGeometry();
        }

        public StockBar[] Values
        {
            get { return (StockBar[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(StockBar[]), typeof(OHLCSeries),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, ValuesPropertyChanged));

        private static void ValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var curve = (OHLCSeries)d;
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
                var values = this.Values;
                var geometryGroup = new GeometryGroup();
                double offset = gap;
                for (int i = 0; i < values.Length; i++)
                {
                    geometryGroup.Children.Add(CreateCandleGeometry(values[i], offset, width));
                    offset += 2 * width + gap;
                }
                this.geometry = geometryGroup;
                this.TransformGeometry();
            }
        }

        private void TransformGeometry()
        {
            if (canvas == null)
                return;
            var canvasWidth = canvas.ActualWidth;
            var canvasHeight = canvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0)
                return;
            var curveWidth = (this.EndIndex - this.StartIndex +1) * (2 * width + gap) + gap;
            if (curveWidth == 0)
                return;
            var values = this.Values;
            if (values == null)
                return;

            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = StartIndex; i <= EndIndex; i++)
            {
                min = Math.Min(values[i].Low, min);
                max = Math.Max(values[i].High, max);
            }
            var curveHeight = max - min;

            TransformGroup tg = new();
            tg.Children.Add(new TranslateTransform(-this.StartIndex * (2 * width + gap) + gap, -min));
            tg.Children.Add(new ScaleTransform(canvasWidth / curveWidth, canvasHeight / curveHeight));
            geometry.Transform = tg;
        }

        double width = 2;
        double gap = 2;

        public OHLCSeries()
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





        private Geometry CreateCandleGeometry(StockBar bar, double offset, double width)
        {
            var geometry = new PathGeometry();
            PathFigureCollection pathFigureCollection = new PathFigureCollection();


            PathFigure pathFigure1 = new PathFigure();
            pathFigure1.StartPoint = new Point(offset - width, bar.Open);
            pathFigure1.IsClosed = true;
            pathFigure1.IsFilled = true;
            pathFigure1.Segments.Add(new LineSegment(new Point(offset - width, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + width, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + width, bar.Open), true));

            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.StartPoint = new Point(offset, Math.Max(bar.Open, bar.Close));
            pathFigure2.Segments.Add(new LineSegment(new Point(offset, bar.High), true));

            PathFigure pathFigure3 = new PathFigure();
            pathFigure3.StartPoint = new Point(offset, Math.Min(bar.Open, bar.Close));
            pathFigure3.Segments.Add(new LineSegment(new Point(offset, bar.Low), true));

            pathFigureCollection.Add(pathFigure1);
            pathFigureCollection.Add(pathFigure2);
            pathFigureCollection.Add(pathFigure3);

            geometry.Figures = pathFigureCollection;
            return geometry;
        }


    }
}
