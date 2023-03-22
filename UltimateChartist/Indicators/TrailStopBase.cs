using System.Linq;
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
        var lowValues = stockSerie.LowValues;
        var closeValues = stockSerie.CloseValues; ;
        decimal alpha = 2.0m / (ReentryPeriod + 1.0m);
        decimal? highReentry = null;
        bool firstLongReentry = true;
        decimal? low = null;
        decimal? lowReentry = null;
        bool firstShortReentry = true;
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
                if (highReentry == null)
                {
                    if (highValues[i - 1] > highValues[i])
                    {
                        highReentry = highValues[i - 1];
                        value.LongReentry = highReentry;
                    }
                }
                else
                {
                    highReentry += alpha * (closeValues[i] - highReentry);
                    if (closeValues[i] > highReentry)
                    {
                        highReentry = null;
                        events.LongReentry = true;
                        events.FirstLongReentry = firstLongReentry;
                        firstLongReentry = false;
                    }
                    else
                    {
                        value.LongReentry = highReentry;
                    }
                }
            }
            else { highReentry = null; }

            if (events.Bearish)
            {
                if (low == null)
                {
                    if (lowValues[i - 1] < lowValues[i])
                    {
                        lowReentry = low = lowValues[i - 1];
                    }
                }
                else
                {
                    if (closeValues[i] < lowReentry)
                    {
                        lowReentry = null;
                        events.ShortReentry = true;
                        events.FirstShortReentry = firstShortReentry;
                        firstShortReentry = false;
                    }
                    else
                    {
                        lowReentry += alpha * (closeValues[i] - lowReentry);
                    }
                    if (closeValues[i] < low)
                    {
                        low = closeValues[i];
                        events.NewLow = true;
                    }
                }

                value.ShortReentry = lowReentry;
            }
            else { low = null; lowReentry = null; }

            value.Events = events;
        }
    }

    protected abstract void InitializeTrailStop(StockSerie stockSerie);

    private int reentryPeriod = 20;
    [IndicatorParameterInt("ReentryPeriod", 1, 500)]
    public int ReentryPeriod { get => reentryPeriod; set { if (reentryPeriod != value) { reentryPeriod = value; RaiseParameterChanged(); } } }
}