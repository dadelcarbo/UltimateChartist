using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public abstract class ChartShapeBase : Shape, IChartShapeBase
    {
        IEnumerable<Shape> shapes;
        public ChartShapeBase()
        {
            shapes = new List<Shape>() { this };
        }

        public void ApplyTransform(Transform transform)
        {
            if (geometry == null) return;
            geometry.Transform = transform;
        }

        public IEnumerable<Shape> Shapes => shapes;

        protected Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }

    public abstract class BarsShapeBase : Shape, IChartShapeBase
    {
        IEnumerable<Shape> shapes;
        public BarsShapeBase()
        {
            shapes = new List<Shape>() { this };
        }
        public abstract void CreateGeometry(Bar bar, int index);

        public void ApplyTransform(Transform transform)
        {
            if (geometry == null) return;
            geometry.Transform = transform;
        }

        public IEnumerable<Shape> Shapes => shapes;

        protected Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }

    public interface IChartShapeBase
    {
        public void ApplyTransform(Transform transform);
        public IEnumerable<Shape> Shapes { get; }
    }
}
