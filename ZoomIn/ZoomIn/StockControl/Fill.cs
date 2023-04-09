using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public class Fill : DependencyObject, IStockShapeBase
    {
        static public Brush DefaultBullFillBrush = new SolidColorBrush(Colors.LightGreen) { Opacity = 0.25 };
        static public Brush DefaultBearFillBrush = new SolidColorBrush(Colors.Red) { Opacity = 0.25 };

        public Brush BullFill
        {
            get { return (Brush)GetValue(BullFillProperty); }
            set { SetValue(BullFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullFillProperty = DependencyProperty.Register("BullFill", typeof(Brush), typeof(Fill), new PropertyMetadata(DefaultBullFillBrush));

        public Brush BearFill
        {
            get { return (Brush)GetValue(BearFillProperty); }
            set { SetValue(BearFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BearFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BearFillProperty = DependencyProperty.Register("BearFill", typeof(Brush), typeof(Fill), new PropertyMetadata(DefaultBearFillBrush));


        private Geometry bullGeometry = null, bearGeometry = null;
        public void CreateGeometry(double[] fast, double[] slow, int gap, int width)
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
                            var pivot = new Point(x + step / 2, (fast[i] + fast[i + 1]) / 2);
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
                            var pivot = new Point(x + step / 2, (fast[i] + fast[i + 1]) / 2);
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
                        ctx.LineTo(point, false /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
            }
            this.bullGeometry = geometryGroup;
            this.BullShape = new Path { Data = bullGeometry };
            var binding = new Binding("BullFill") { Source = this };
            this.BullShape.SetBinding(Path.FillProperty, binding);

            geometryGroup = new GeometryGroup();
            foreach (var points in bearPoints)
            {
                var streamGeometry = new StreamGeometry();
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    ctx.BeginFigure(points.First(), true, true);
                    foreach (var point in points)
                    {
                        ctx.LineTo(point, false /* is stroked */, false /* is smooth join */);
                    }
                }
                geometryGroup.Children.Add(streamGeometry);
            }
            this.bearGeometry = geometryGroup;
            this.BearShape = new Path { Data = bearGeometry };
            binding = new Binding("BearFill") { Source = this };
            this.BearShape.SetBinding(Path.FillProperty, binding);
        }

        public void ApplyTranform(Transform transform)
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
