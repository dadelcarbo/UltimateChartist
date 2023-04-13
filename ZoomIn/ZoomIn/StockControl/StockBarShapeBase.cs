using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public abstract class StockBarShapeBase : StockShapeBase
    {
        public StockBar StockBar { get; private set; }

        public abstract void CreateGeometry(StockBar bar, int index, double gap, double width);
    }
    public abstract class StockShapeBase : Shape, IStockShapeBase
    {
        IEnumerable<Shape> shapes;
        public StockShapeBase()
        {
            this.shapes = new List<Shape>() { this };
        }
        public void ApplyTransform(Transform transform)
        {
            if (this.geometry == null) return;
            this.geometry.Transform = transform;
        }

        public IEnumerable<Shape> Shapes => this.shapes;

        protected Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }

    public interface IStockShapeBase
    {
        public void ApplyTransform(Transform transform);
        public IEnumerable<Shape> Shapes { get; }
    }
}
