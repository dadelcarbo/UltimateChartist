using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class Trail : DependencyObject, IStockShapeBase
    {
        static public Brush DefaultBullFillBrush = new SolidColorBrush(Colors.Green) { Opacity = 0.5 };
        static public Brush DefaultBearFillBrush = new SolidColorBrush(Colors.Red) { Opacity = 0.5 };
        static public Brush DefaultBullStrokeBrush = new SolidColorBrush(Colors.Green) { Opacity = 1 };
        static public Brush DefaultBearStrokeBrush = new SolidColorBrush(Colors.Red) { Opacity = 1 };

        public Brush BullFill
        {
            get { return (Brush)GetValue(BullFillProperty); }
            set { SetValue(BullFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullFillProperty = DependencyProperty.Register("BullFill", typeof(Brush), typeof(Trail), new FrameworkPropertyMetadata(DefaultBullFillBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BullStroke
        {
            get { return (Brush)GetValue(BullStrokeProperty); }
            set { SetValue(BullStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullStrokeProperty = DependencyProperty.Register("BullStroke", typeof(Brush), typeof(Trail), new FrameworkPropertyMetadata(DefaultBullStrokeBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BullStrokeThickness
        {
            get { return (double)GetValue(BullStrokeThicknessProperty); }
            set { SetValue(BullStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullStrokeThicknessProperty = DependencyProperty.Register("BullStrokeThickness", typeof(double), typeof(Trail), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BearFill
        {
            get { return (Brush)GetValue(BearFillProperty); }
            set { SetValue(BearFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearFillProperty = DependencyProperty.Register("BearFill", typeof(Brush), typeof(Trail), new PropertyMetadata(DefaultBearFillBrush));


        public Brush BearStroke
        {
            get { return (Brush)GetValue(BearStrokeProperty); }
            set { SetValue(BearStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearStrokeProperty = DependencyProperty.Register("BearStroke", typeof(Brush), typeof(Trail), new FrameworkPropertyMetadata(DefaultBearStrokeBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BearStrokeThickness
        {
            get { return (double)GetValue(BearStrokeThicknessProperty); }
            set { SetValue(BearStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearStrokeThicknessProperty = DependencyProperty.Register("BearStrokeThickness", typeof(double), typeof(Trail), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));


        private Geometry bullGeometry = null, bearGeometry = null;

        public void CreateGeometry(double[] close, double?[] longStop, double?[] shortStop)
        {
            var bullPoints = GetTrailAreas(close, longStop);
            var bearPoints = GetTrailAreas(close, shortStop);

            var geometryGroup = new GeometryGroup();
            foreach (var points in bullPoints)
            {
                var streamGeometry = new StreamGeometry();
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    ctx.BeginFigure(points.First(), true, true);
                    foreach (var point in points)
                    {
                        ctx.LineTo(point, true /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
            }
            this.bullGeometry = geometryGroup;
            this.BullShape = new Path { Data = bullGeometry };
            this.BullShape.SetBinding(Path.FillProperty, new Binding("BullFill") { Source = this });
            this.BullShape.SetBinding(Path.StrokeProperty, new Binding("BullStroke") { Source = this });
            this.BullShape.SetBinding(Path.StrokeThicknessProperty, new Binding("BullStrokeThickness") { Source = this });

            geometryGroup = new GeometryGroup();
            foreach (var points in bearPoints)
            {
                var streamGeometry = new StreamGeometry();
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    ctx.BeginFigure(points.First(), true, true);
                    foreach (var point in points)
                    {
                        ctx.LineTo(point, true /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
            }
            this.bearGeometry = geometryGroup;
            this.BearShape = new Path { Data = bearGeometry };
            this.BearShape.SetBinding(Path.FillProperty, new Binding("BearFill") { Source = this });
            this.BearShape.SetBinding(Path.StrokeProperty, new Binding("BearStroke") { Source = this });
            this.BearShape.SetBinding(Path.StrokeThicknessProperty, new Binding("BearStrokeThickness") { Source = this });
        }

        private static List<List<Point>> GetTrailAreas(double[] close, double?[] trail)
        {
            var pointList = new List<List<Point>>();
            int i = 0;
            do
            {
                while (i < close.Length && trail[i] == null) { i++; }
                if (i == trail.Length) break;

                List<Point> closePoints = new() { new Point(i, close[i]) };
                List<Point> trailPoints = new() { new Point(i, trail[i].Value) };
                i++;
                while (i < close.Length && trail[i] != null)
                {
                    closePoints.Add(new Point(i, close[i]));
                    trailPoints.Insert(0, new Point(i, trail[i].Value));
                    i++;
                }
                closePoints.AddRange(trailPoints);
                pointList.Add(closePoints);
            }
            while (i < close.Length);
            return pointList;
        }

        public void ApplyTransform(Transform transform)
        {
            if (this.bullGeometry != null)
                this.bullGeometry.Transform = transform;
            if (this.bearGeometry != null)
                this.bearGeometry.Transform = transform;
        }

        public Path BullShape { get; set; }
        public Path BearShape { get; set; }


        public IEnumerable<Shape> Shapes => new List<Shape>() { this.BullShape, this.BearShape };
    }
}
