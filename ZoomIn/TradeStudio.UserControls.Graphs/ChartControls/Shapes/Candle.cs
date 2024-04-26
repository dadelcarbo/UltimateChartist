﻿using System;
using System.Windows;
using System.Windows.Media;
using TradeStudio.Data.DataProviders;

namespace TradeStudio.UserControls.Graphs.ChartControls.Shapes
{
    public class Candle : BarsShapeBase
    {
        protected double gap = 0.3;
        protected double width = 0.7;
        /// <summary>
        /// Create a bar geometry.
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="index"></param>
        /// <param name="width">Width of the bar itself</param>
        public override void CreateGeometry(Bar bar, int index)
        {
            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);

            var pathGeometry = new PathGeometry();
            PathFigureCollection pathFigureCollection = new PathFigureCollection();

            var halfWidth = width / 2.0;
            var offset = index * (width + gap);
            PathFigure pathFigure1 = new PathFigure();
            pathFigure1.StartPoint = new Point(offset - halfWidth, bar.Open);
            pathFigure1.IsClosed = true;
            pathFigure1.IsFilled = true;
            pathFigure1.Segments.Add(new LineSegment(new Point(offset - halfWidth, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + halfWidth, bar.Close), true));
            pathFigure1.Segments.Add(new LineSegment(new Point(offset + halfWidth, bar.Open), true));

            PathFigure pathFigure2 = new PathFigure() { IsClosed = false };
            pathFigure2.StartPoint = new Point(offset, Math.Max(bar.Open, bar.Close));
            pathFigure2.Segments.Add(new LineSegment(new Point(offset, bar.High), true));

            PathFigure pathFigure3 = new PathFigure() { IsClosed = false };
            pathFigure3.StartPoint = new Point(offset, Math.Min(bar.Open, bar.Close));
            pathFigure3.Segments.Add(new LineSegment(new Point(offset, bar.Low), true));

            pathFigureCollection.Add(pathFigure1);
            pathFigureCollection.Add(pathFigure2);
            pathFigureCollection.Add(pathFigure3);
            pathGeometry.Figures = pathFigureCollection;

            geometry = pathGeometry;
        }
    }
}
