using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public interface IChartShapeBase
    {
        public void ApplyTransform(Transform transform);
        public IEnumerable<Shape> Shapes { get; }
    }
    public abstract class ChartShapeBase : Shape, IChartShapeBase
    {
        IEnumerable<Shape> shapes = new List<Shape>();

        public void ApplyTransform(Transform transform)
        {
            if (geometry == null)
                return;
            geometry.Transform = transform;
        }

        public IEnumerable<Shape> Shapes => shapes;

        protected Geometry geometry;
        protected override Geometry DefiningGeometry => geometry;
    }

    public abstract class BarsShapeBase : ChartShapeBase, IChartShapeBase
    {
        public abstract void CreateGeometry(Bar bar, int index);
        public abstract void CreateGeometry(List<Bar> bars, bool bullBars);
    }

}
