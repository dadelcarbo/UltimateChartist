using System.Configuration;
using System.Windows.Media;
using UltimateChartist.DataModels;
using UltimateChartist.Indicators.Display;
using UltimateChartist.Indicators.Events;

namespace UltimateChartist.Indicators;

public enum EmaType
{
    EMA,
    MA,
    MID
}

public abstract class MovingAverageBase : IndicatorBase
{
    public MovingAverageBase()
    {
        var series = new IndicatorLineSeries();
        series.Curve.Stroke = Brushes.Blue;
        series.Curve.Thickness = 1;
        series.Curve.Name = "Moving Average";

        this.Series = series;
    }

    public override void Initialize(StockSerie stockSerie)
    {
        this.InitializeMA(stockSerie);
        var close = stockSerie.CloseValues;
        bool isAbove, previousIsAbove = false;
        for (int i = this.Period; i < stockSerie.CloseValues.Length; i++)
        {
            var value = this.Series.Values[i] as IndicatorLineValue;
            isAbove = close[i] > value.Value;
            value.Events = new StockEvent_MA { IsAbove = isAbove, CrossAbove = !previousIsAbove && isAbove, CrossBelow = previousIsAbove && !isAbove };
            previousIsAbove = isAbove;
        }
    }

    protected abstract void InitializeMA(StockSerie stockSerie);

    public override DisplayType DisplayType => DisplayType.Price;
    public override string DisplayName => $"{ShortName}({Period})";

    private int period = 20;
    [IndicatorParameterInt("Period", 1, 500)]
    public int Period { get => period; set { if (period != value) { period = value; RaiseParameterChanged(); } } }
}