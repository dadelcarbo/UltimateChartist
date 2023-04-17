using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.ChartControls.Shapes
{
    public abstract class StockBarShapeBase : StockShapeBase
    {
        public StockBar StockBar { get; private set; }

        public abstract void CreateGeometry(StockBar bar, int index);
    }
    public abstract class StockShapeBase : Shape, IStockShapeBase
    {
        IEnumerable<Shape> shapes;
        public StockShapeBase()
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
