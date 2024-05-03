using System;
using System.Collections.Generic;
using System.Windows.Data;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.DataProviders;
using System.Windows.Shapes;
using TradeStudio.Data.Indicators.Display;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;
using Range = TradeStudio.UserControls.Graphs.ChartControls.Shapes.Range;
using Curve = TradeStudio.UserControls.Graphs.ChartControls.Shapes.Curve;

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
                        var lineSeries = new Curve { Tag = this, DataContext = indicator };

                        var binding = new Binding($"Series.Curve.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);

                        binding = new Binding($"Series.Curve.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry((indicator.Series.Values as IndicatorLineValues).Values);

                        Shapes.Add(lineSeries);
                    }
                    break;
                case "IndicatorLineSignalSeries":
                    {
                        var lineSeries = new Curve { Tag = this, DataContext = indicator };

                        var binding = new Binding($"Series.Signal.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);
                        binding = new Binding($"Series.Signal.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry((indicator.Series.Values as IndicatorLineValues).Values);

                        Shapes.Add(lineSeries);

                        lineSeries = new Curve { Tag = this, DataContext = indicator };

                        binding = new Binding($"Series.Curve.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);
                        binding = new Binding($"Series.Curve.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry((indicator.Series.Values as IndicatorLineValues).Values);

                        Shapes.Add(lineSeries);
                    }
                    break;
                case "IndicatorRangeSeries":
                    {
                        var rangeSeries = new Range { Tag = this, DataContext = indicator };

                        var binding = new Binding($"Series.Area.Fill");
                        rangeSeries.SetBinding(Shape.FillProperty, binding);
                        binding = new Binding($"Series.Area.Thickness");
                        rangeSeries.SetBinding(Shape.StrokeThicknessProperty, binding);
                        binding = new Binding($"Series.Area.Stroke");
                        rangeSeries.SetBinding(Shape.StrokeProperty, binding);

                        var rangeValues = (IndicatorRangeValues)indicator.Series.Values;
                        rangeSeries.CreateGeometry(rangeValues.Low, rangeValues.High);

                        Shapes.Add(rangeSeries);
                    }
                    break;
                case "IndicatorBandSeries":
                    {
                        var rangeSeries = new Range { Tag = this, DataContext = indicator };

                        var binding = new Binding($"Series.Area.Fill");
                        rangeSeries.SetBinding(Shape.FillProperty, binding);
                        binding = new Binding($"Series.Area.Thickness");
                        rangeSeries.SetBinding(Shape.StrokeThicknessProperty, binding);
                        binding = new Binding($"Series.Area.Stroke");
                        rangeSeries.SetBinding(Shape.StrokeProperty, binding);

                        var bandValues = (IndicatorBandValues)indicator.Series.Values;
                        rangeSeries.CreateGeometry(bandValues.Low, bandValues.High);

                        Shapes.Add(rangeSeries);

                        var lineSeries = new Curve { Tag = this, DataContext = indicator };

                        binding = new Binding($"Series.MidLine.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);
                        binding = new Binding($"Series.MidLine.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry(bandValues.Mid);

                        Shapes.Add(lineSeries);
                    }
                    break;
                case "IndicatorTrailSeries":
                    {
                        var trailValues = (IndicatorTrailValues)indicator.Series.Values;

                        var lineSeries = new Curve { Tag = this, DataContext = indicator };

                        var binding = new Binding($"Series.LongReentry.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);
                        binding = new Binding($"Series.LongReentry.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry(trailValues.LongReentry);

                        Shapes.Add(lineSeries);

                        lineSeries = new Curve { Tag = this, DataContext = indicator };

                        binding = new Binding($"Series.ShortReentry.Stroke");
                        lineSeries.SetBinding(Shape.StrokeProperty, binding);
                        binding = new Binding($"Series.ShortReentry.Thickness");
                        lineSeries.SetBinding(Shape.StrokeThicknessProperty, binding);

                        lineSeries.CreateGeometry(trailValues.ShortReentry);

                        Shapes.Add(lineSeries);

                        var rangeSeries = new Range { Tag = this, DataContext = indicator };

                        binding = new Binding($"Series.Long.Fill");
                        rangeSeries.SetBinding(Shape.FillProperty, binding);
                        binding = new Binding($"Series.Long.Thickness");
                        rangeSeries.SetBinding(Shape.StrokeThicknessProperty, binding);
                        binding = new Binding($"Series.Long.Stroke");
                        rangeSeries.SetBinding(Shape.StrokeProperty, binding);

                        rangeSeries.CreateGeometry(trailValues.Long, trailValues.High);
                        Shapes.Add(rangeSeries);

                        rangeSeries = new Range { Tag = this, DataContext = indicator };

                        binding = new Binding($"Series.Short.Fill");
                        rangeSeries.SetBinding(Shape.FillProperty, binding);
                        binding = new Binding($"Series.Short.Thickness");
                        rangeSeries.SetBinding(Shape.StrokeThicknessProperty, binding);
                        binding = new Binding($"Series.Short.Stroke");
                        rangeSeries.SetBinding(Shape.StrokeProperty, binding);

                        rangeSeries.CreateGeometry(trailValues.Short, trailValues.Low);

                        Shapes.Add(rangeSeries);

                    }
                    break;
                default: // §§§§
                    throw new NotImplementedException($"Series type not implemented {indicatorSeries.GetType().Name} in IndicatorViewModel");
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
