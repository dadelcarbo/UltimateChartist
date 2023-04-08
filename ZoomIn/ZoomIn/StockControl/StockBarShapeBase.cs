using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public abstract class StockBarShapeBase : StockShapeBase
    {
        public StockBar StockBar { get; private set; }

        public abstract void CreateGeometry(StockBar bar, int index, int gap, int width);
    }
    public abstract class StockShapeBase : Shape
    {
        public void ApplyTranform(Transform transform)
        {
            if (this.geometry == null) return;
            this.geometry.Transform = transform;
        }

        protected Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }
}
