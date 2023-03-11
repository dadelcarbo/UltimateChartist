using System.Windows.Media;
using UltimateChartist.DataModels;
using UltimateChartist.Indicators.Display;
using UltimateChartist.Indicators.Events;

namespace UltimateChartist.Indicators;


public abstract class TrailStopBase : IndicatorBase
{
    public TrailStopBase()
    {
        this.Series = new IndicatorTrailSeries();
    }

    public override void Initialize(StockSerie stockSerie)
    {
        this.InitializeTrailStop(stockSerie);
        var previousEvent = new StockEvent_TrailStop();
        for (int i = this.ReentryPeriod; i < stockSerie.CloseValues.Length; i++)
        {
            var value = this.Series.Values[i] as IndicatorTrailValue;
            value.Events = new StockEvent_TrailStop
            {
                Bullish = value.Long != null,
                Bearish = value.Short != null,
                BrokenUp = value.Long != null && !previousEvent.Bullish,
                BrokenDown = value.Short != null && !previousEvent.Bearish
            };

        }
    }

    protected abstract void InitializeTrailStop(StockSerie stockSerie);

    private int reentryPeriod = 20;
    [IndicatorParameterInt("ReentryPeriod", 1, 500)]
    public int ReentryPeriod { get => reentryPeriod; set { if (reentryPeriod != value) { reentryPeriod = value; RaiseParameterChanged(); } } }
}