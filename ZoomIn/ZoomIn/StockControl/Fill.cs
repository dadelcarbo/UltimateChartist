using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Charting;
using ZoomIn.StockDrawing;

namespace ZoomIn.StockControl
{
    public class Fill : DependencyObject, IStockShapeBase
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
        public static readonly DependencyProperty BullFillProperty = DependencyProperty.Register("BullFill", typeof(Brush), typeof(Fill), new FrameworkPropertyMetadata(DefaultBullFillBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BullStroke
        {
            get { return (Brush)GetValue(BullStrokeProperty); }
            set { SetValue(BullStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullStrokeProperty = DependencyProperty.Register("BullStroke", typeof(Brush), typeof(Fill), new FrameworkPropertyMetadata(DefaultBullStrokeBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BullStrokeThickness
        {
            get { return (double)GetValue(BullStrokeThicknessProperty); }
            set { SetValue(BullStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullStrokeThicknessProperty = DependencyProperty.Register("BullStrokeThickness", typeof(double), typeof(Fill), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BearFill
        {
            get { return (Brush)GetValue(BearFillProperty); }
            set { SetValue(BearFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearFillProperty = DependencyProperty.Register("BearFill", typeof(Brush), typeof(Fill), new PropertyMetadata(DefaultBearFillBrush));


        public Brush BearStroke
        {
            get { return (Brush)GetValue(BearStrokeProperty); }
            set { SetValue(BearStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearStrokeProperty = DependencyProperty.Register("BearStroke", typeof(Brush), typeof(Fill), new FrameworkPropertyMetadata(DefaultBearStrokeBrush, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BearStrokeThickness
        {
            get { return (double)GetValue(BearStrokeThicknessProperty); }
            set { SetValue(BearStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearStrokeThicknessProperty = DependencyProperty.Register("BearStrokeThickness", typeof(double), typeof(Fill), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));


        private Geometry? bullGeometry = null, bearGeometry = null;
        public void CreateGeometry(double[] fast, double[] slow, double gap, double width)
        {
            List<List<Point>> bullPoints = new List<List<Point>>();
            List<List<Point>> bearPoints = new List<List<Point>>();

            bool bull = fast[0] > slow[0];

            double x = gap;
            double step = 2 * width + gap;
            List<Point> fastPoints = new List<Point>() { new Point(x, fast[0]) };
            List<Point> slowPoints = new List<Point>() { new Point(x, slow[0]) };
            for (int i = 1; i < fast.Length; i++)
            {
                x += step;
                if (bull)
                {
                    if (fast[i] > slow[i])
                    {
                        fastPoints.Add(new Point(x, fast[i]));
                        slowPoints.Add(new Point(x, slow[i]));
                    }
                    else
                    {
                        if (i < fast.Length - 1)
                        {
                            var pivot = Line2D.Intersection(fastPoints.Last(), new Point(x, fast[i]), slowPoints.Last(), new Point(x, slow[i]));
                            fastPoints.Add(pivot);
                            slowPoints.Reverse();
                            fastPoints.AddRange(slowPoints);
                            bullPoints.Add(fastPoints);

                            fastPoints = new List<Point>() { pivot, new Point(x, fast[i]) };
                        }
                        else
                        {
                            fastPoints = new List<Point>() { new Point(x, fast[i]) };
                        }
                        slowPoints = new List<Point>() { new Point(x, slow[i]) };
                        bull = false;
                    }
                }
                else
                {
                    if (fast[i] < slow[i])
                    {
                        fastPoints.Add(new Point(x, fast[i]));
                        slowPoints.Add(new Point(x, slow[i]));

                    }
                    else
                    {
                        if (i < fast.Length - 1)
                        {
                            var pivot = Line2D.Intersection(fastPoints.Last(), new Point(x, fast[i]), slowPoints.Last(), new Point(x, slow[i]));
                            fastPoints.Add(pivot);
                            slowPoints.Reverse();
                            fastPoints.AddRange(slowPoints);
                            bearPoints.Add(fastPoints);

                            fastPoints = new List<Point>() { pivot, new Point(x, fast[i]) };
                        }
                        else
                        {
                            fastPoints = new List<Point>() { new Point(x, fast[i]) };
                        }
                        slowPoints = new List<Point>() { new Point(x, slow[i]) };
                        bull = true;
                    }
                }
            }
            if (bull)
            {
                slowPoints.Reverse();
                fastPoints.AddRange(slowPoints);
                bullPoints.Add(fastPoints);
            }
            else
            {
                slowPoints.Reverse();
                fastPoints.AddRange(slowPoints);
                bearPoints.Add(fastPoints);
            }

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
