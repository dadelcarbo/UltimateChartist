using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UltimateChartistControls.ChartControls.Shapes
{
    public abstract class BarsShapeBase : Shape, IStockShapeBase
    {
        IEnumerable<Shape> shapes;
        public BarsShapeBase()
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

    public interface IStockShapeBase
    {
        public void ApplyTransform(Transform transform);
        public IEnumerable<Shape> Shapes { get; }
    }
}
