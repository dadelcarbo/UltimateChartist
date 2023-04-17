using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn
{
    public class Candle : Shape
    {
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Candle),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, ScalePropertyChanged));

        private static void ScalePropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var candle = (Candle)d;
            if (candle.geometry != null)
            {
                candle.geometry.Transform = new ScaleTransform((double)e.NewValue, (double)e.NewValue);
            }
        }
        static int index = 0;
        double width = 5;
        double gap = 5;

        public Candle()
        {
            geometry = new PathGeometry();
            PathFigureCollection pathFigureCollection = new PathFigureCollection();

            double offset = index * (2 * width + gap) + gap + width;
            index++;

            PathFigure pathFigure1 = new PathFigure();
            pathFigure1.StartPoint = new Point(offset - width, 50);
            pathFigure1.IsClosed = true;
            pathFigure1.IsFilled = true;
            LineSegment lineSegment1_1 = new LineSegment(new Point(offset - width, 10), true);
            LineSegment lineSegment1_2 = new LineSegment(new Point(offset + width, 10), true);
            LineSegment lineSegment1_3 = new LineSegment(new Point(offset + width, 50), true);
            pathFigure1.Segments.Add(lineSegment1_1);
            pathFigure1.Segments.Add(lineSegment1_2);
            pathFigure1.Segments.Add(lineSegment1_3);

            PathFigure pathFigure2 = new PathFigure();
            pathFigure2.StartPoint = new Point(offset, 50);
            LineSegment lineSegment2_1 = new LineSegment(new Point(offset, 60), true);
            pathFigure2.Segments.Add(lineSegment2_1);

            PathFigure pathFigure3 = new PathFigure();
            pathFigure3.StartPoint = new Point(offset, 0);
            LineSegment lineSegment3_1 = new LineSegment(new Point(offset, 10), true);
            pathFigure3.Segments.Add(lineSegment3_1);

            pathFigureCollection.Add(pathFigure1);
            pathFigureCollection.Add(pathFigure2);
            pathFigureCollection.Add(pathFigure3);

            geometry.Figures = pathFigureCollection;
        }

        private PathGeometry geometry;
        protected override Geometry DefiningGeometry => geometry;


    }
}
