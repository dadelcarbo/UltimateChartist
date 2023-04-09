﻿using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZoomIn.StockControl
{
    public abstract class StockBarShapeBase : StockShapeBase
    {
        public StockBar StockBar { get; private set; }

        public abstract void CreateGeometry(StockBar bar, int index, int gap, int width);
    }
    public abstract class StockShapeBase : Shape, IStockShapeBase
    {
        IEnumerable<Shape> shapes;
        public StockShapeBase()
        {
            this.shapes = new List<Shape>() { this };
        }
        public void ApplyTranform(Transform transform)
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
        public void ApplyTranform(Transform transform);
        public IEnumerable<Shape> Shapes { get; }
    }
}