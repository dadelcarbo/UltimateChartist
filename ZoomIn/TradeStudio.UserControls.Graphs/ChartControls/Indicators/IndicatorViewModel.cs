using System;
using System.Collections.Generic;
using System.Windows.Data;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.DataProviders;
using System.Windows.Shapes;
using TradeStudio.Data.Indicators.Display;
using Curve = TradeStudio.UserControls.Graphs.ChartControls.Shapes.Curve;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators;

public delegate void GeometryChangedEventHandler(IndicatorViewModel indicatorViewModel);
public class IndicatorViewModel : ViewModelBase
{
    public event GeometryChangedEventHandler GeometryChanged;

    public IndicatorSettings IndicatorSettings { get; }

    public List<IChartShapeBase> Shapes { get; } = new();
    public IndicatorViewModel() { }

    private DataSerie dataSerie;
    private IIndicator indicator;
    public IndicatorViewModel(IndicatorSettings indicatorSettings, DataSerie dataSerie)
    {
        this.IndicatorSettings = indicatorSettings;
        indicator = indicatorSettings.GetIndicator();
        this.dataSerie = dataSerie;
        if (dataSerie != null)
            indicator.Initialize(dataSerie);

        this.GetIndicatorGeometry(indicator);

        IndicatorSettings.ParameterChanged += IndicatorSettings_ParameterChanged;
        IndicatorSettings.DisplayChanged += IndicatorSettings_DisplayChanged;
    }

    private void IndicatorSettings_DisplayChanged(DisplaySettings displaySettings)
    {
        var seriesType = indicator.Series.GetType();
        var prop = seriesType.GetProperty(displaySettings.Name);
        var displayItem = prop.GetValue(indicator.Series) as IDisplayItem;
        displayItem.FromJson(displaySettings.Json);
    }

    private void IndicatorSettings_ParameterChanged(IndicatorSettings indicatorSettings)
    {
        indicator = indicatorSettings.GetIndicator();
        if (indicator != null && dataSerie != null)
        {
            indicator.Initialize(dataSerie);
        }
        this.GetIndicatorGeometry(indicator);
        this.GeometryChanged?.Invoke(this);
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
                        var lineSeries = new Curve() { Tag = this };

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
