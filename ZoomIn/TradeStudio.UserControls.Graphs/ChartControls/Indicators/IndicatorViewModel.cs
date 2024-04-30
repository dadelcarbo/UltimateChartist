﻿using System;
using System.Collections.Generic;
using System.Windows.Data;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.DataProviders;
using System.Windows.Shapes;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;
using System.Linq;
using TradeStudio.Data.Indicators.Display;
using Curve = TradeStudio.UserControls.Graphs.ChartControls.Shapes.Curve;
using System.ComponentModel;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators;

public delegate void GeometryChangedEventHandler(IndicatorViewModel indicatorViewModel);
public class IndicatorViewModel : ViewModelBase
{
    public event GeometryChangedEventHandler GeometryChanged;

    public IIndicator Indicator { get; }

    private void Indicator_ParameterChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var indicator = sender as TradeStudio.Data.Indicators.IndicatorBase;
        if (indicator != null && dataSerie != null)
        {
            indicator.Initialize(dataSerie);
        }
        this.GetIndicatorGeometry(indicator);
    }

    public List<Shape> Shapes { get; } = new();
    public IndicatorViewModel() { }

    private DataSerie dataSerie;
    public IndicatorViewModel(IIndicator indicator, DataSerie dataSerie)
    {
        Indicator = indicator;
        this.dataSerie = dataSerie;
        if (dataSerie != null)
            indicator.Initialize(dataSerie);

        this.GetIndicatorGeometry(indicator);

        indicator.ParameterChanged += Indicator_ParameterChanged;
    }

    void GetIndicatorGeometry(IIndicator indicator)
    {
        this.Shapes.Clear();

        // Create GraphSeries from instrospection
        var indicatorSeries = indicator.Series;
        if (indicatorSeries != null)
        {
            switch (indicatorSeries.GetType().Name)
            {
                case "IndicatorLineSeries":
                    {
                        var lineSeries = new Curve();

                        var binding = new Binding($"Series.Curve.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);

                        binding = new Binding($"Series.Curve.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.DataContext = indicator;
                        lineSeries.CreateGeometry((indicator.Series.Values as IndicatorLineValues).Values);

                        Shapes.Add(lineSeries);
                    }
                    break;
                //case "IndicatorLineSignalSeries":
                //    {
                //        var lineSeries = new LineSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            ValueBinding = new PropertyNameDataPointBinding("Value")
                //        };
                //        var binding = new Binding($"Series.Curve.Stroke");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeProperty, binding);
                //        binding = new Binding($"Series.Curve.Thickness");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Values");
                //        lineSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        lineSeries.DataContext = indicator;

                //        CartesianSeries.Add(lineSeries);

                //        lineSeries = new LineSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            ValueBinding = new PropertyNameDataPointBinding("Signal")
                //        };
                //        binding = new Binding($"Series.Signal.Stroke");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeProperty, binding);
                //        binding = new Binding($"Series.Signal.Thickness");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Values");
                //        lineSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        lineSeries.DataContext = indicator;

                //        CartesianSeries.Add(lineSeries);
                //    }
                //    break;
                //case "IndicatorRangeSeries":
                //    {
                //        var rangeSeries = new RangeSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            HighBinding = new PropertyNameDataPointBinding("High"),
                //            LowBinding = new PropertyNameDataPointBinding("Low"),
                //            StrokeMode = RangeSeriesStrokeMode.LowAndHighPoints,
                //        };

                //        var binding = new Binding($"Series.Area.Fill");
                //        rangeSeries.SetBinding(RangeSeries.FillProperty, binding);
                //        binding = new Binding($"Series.Area.Thickness");
                //        rangeSeries.SetBinding(RangeSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Area.Stroke");
                //        rangeSeries.SetBinding(RangeSeries.StrokeProperty, binding);

                //        binding = new Binding($"Series.Values");
                //        rangeSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        rangeSeries.DataContext = indicator;

                //        CartesianSeries.Add(rangeSeries);
                //    }
                //    break;
                //case "IndicatorBandSeries":
                //    {
                //        var lineSeries = new LineSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            ValueBinding = new PropertyNameDataPointBinding("Mid")
                //        };
                //        var binding = new Binding($"Series.MidLine.Stroke");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeProperty, binding);
                //        binding = new Binding($"Series.MidLine.Thickness");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Values");
                //        lineSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        lineSeries.DataContext = indicator;

                //        CartesianSeries.Add(lineSeries);

                //        var rangeSeries = new RangeSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            HighBinding = new PropertyNameDataPointBinding("High"),
                //            LowBinding = new PropertyNameDataPointBinding("Low"),
                //            StrokeMode = RangeSeriesStrokeMode.LowAndHighPoints,
                //        };

                //        binding = new Binding($"Series.Area.Fill");
                //        rangeSeries.SetBinding(RangeSeries.FillProperty, binding);
                //        binding = new Binding($"Series.Area.Thickness");
                //        rangeSeries.SetBinding(RangeSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Area.Stroke");
                //        rangeSeries.SetBinding(RangeSeries.StrokeProperty, binding);

                //        binding = new Binding($"Series.Values");
                //        rangeSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        rangeSeries.DataContext = indicator;

                //        CartesianSeries.Add(rangeSeries);
                //    }
                //    break;
                //case "IndicatorTrailSeries":
                //    {

                //        var lineSeries = new LineSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            ValueBinding = new PropertyNameDataPointBinding("LongReentry")
                //        };
                //        var binding = new Binding($"Series.LongReentry.Stroke");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeProperty, binding);
                //        binding = new Binding($"Series.LongReentry.Thickness");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Values");
                //        lineSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        lineSeries.DataContext = indicator;

                //        CartesianSeries.Add(lineSeries);

                //        lineSeries = new LineSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            ValueBinding = new PropertyNameDataPointBinding("ShortReentry")
                //        };
                //        binding = new Binding($"Series.ShortReentry.Stroke");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeProperty, binding);
                //        binding = new Binding($"Series.ShortReentry.Thickness");
                //        lineSeries.SetBinding(CategoricalStrokedSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Values");
                //        lineSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        lineSeries.DataContext = indicator;

                //        CartesianSeries.Add(lineSeries);

                //        var rangeSeries = new RangeSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            HighBinding = new PropertyNameDataPointBinding("High"),
                //            LowBinding = new PropertyNameDataPointBinding("Long"),
                //            StrokeMode = RangeSeriesStrokeMode.LowPoints
                //        };

                //        binding = new Binding($"Series.Long.Fill");
                //        rangeSeries.SetBinding(RangeSeries.FillProperty, binding);
                //        binding = new Binding($"Series.Long.Thickness");
                //        rangeSeries.SetBinding(RangeSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Long.Stroke");
                //        rangeSeries.SetBinding(RangeSeries.StrokeProperty, binding);

                //        binding = new Binding($"Series.Values");
                //        rangeSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        rangeSeries.DataContext = indicator;

                //        CartesianSeries.Add(rangeSeries);

                //        rangeSeries = new RangeSeries()
                //        {
                //            CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" },
                //            HighBinding = new PropertyNameDataPointBinding("Short"),
                //            LowBinding = new PropertyNameDataPointBinding("Low"),
                //            StrokeMode = RangeSeriesStrokeMode.HighPoints
                //        };

                //        binding = new Binding($"Series.Short.Fill");
                //        rangeSeries.SetBinding(RangeSeries.FillProperty, binding);
                //        binding = new Binding($"Series.Short.Thickness");
                //        rangeSeries.SetBinding(RangeSeries.StrokeThicknessProperty, binding);
                //        binding = new Binding($"Series.Short.Stroke");
                //        rangeSeries.SetBinding(RangeSeries.StrokeProperty, binding);

                //        binding = new Binding($"Series.Values");
                //        rangeSeries.SetBinding(ChartSeries.ItemsSourceProperty, binding);
                //        rangeSeries.DataContext = indicator;

                //        CartesianSeries.Add(rangeSeries);

                //    }
                //    break;
                //default: §§§§
                //    throw new NotImplementedException($"Series type not implemented {indicatorSeries.GetType().Name} in IndicatorViewModel");
            }
        }
    }


}

public interface IIndicatorParameterViewModel
{
    IndicatorParameterAttribute Parameter { get; }
    string PropertyName { get; }
}
public class IndicatorParameterViewModel<T> : ViewModelBase, IIndicatorParameterViewModel
{
    public IndicatorParameterViewModel(string propertyName)
    {
        PropertyName = propertyName;
    }
    public T Value { get; set; }

    public string PropertyName { get; }

    public IndicatorParameterAttribute Parameter { get; set; }
}
