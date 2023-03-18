using System;
using System.Linq;
using System.Windows.Media;
using Telerik.Windows.Controls.FieldList;
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
    public override DisplayType DisplayType => DisplayType.TrailStop;

    public override void Initialize(StockSerie stockSerie)
    {
        this.InitializeTrailStop(stockSerie);
        var previousEvent = new StockEvent_TrailStop();
        var series = this.Series.Values.Cast<IndicatorTrailValue>().ToArray();

        var highValues = stockSerie.HighValues;
        var closeValues = stockSerie.CloseValues;
        decimal? high = null;
        for (int i = this.ReentryPeriod; i < stockSerie.CloseValues.Length; i++)
        {
            var value = series[i];
            var events = new StockEvent_TrailStop
            {
                Bullish = value.Long != null,
                Bearish = value.Short != null,
                BrokenUp = value.Long != null && !previousEvent.Bullish,
                BrokenDown = value.Short != null && !previousEvent.Bearish
            };

            if (events.Bullish)
            {
                if (high == null)
                {
                    if (highValues[i - 1] > highValues[i])
                    {
                        high = highValues[i - 1];
                    }
                }
                else if (closeValues[i] > high)
                {
                    high = null;
                }
                value.LongReentry = high;
            }
            else { high = null; }

            value.Events = events;
        }
    }

    protected abstract void InitializeTrailStop(StockSerie stockSerie);

    private int reentryPeriod = 20;
    [IndicatorParameterInt("ReentryPeriod", 1, 500)]
    public int ReentryPeriod { get => reentryPeriod; set { if (reentryPeriod != value) { reentryPeriod = value; RaiseParameterChanged(); } } }
}