using System;
using System.Linq;
using UltimateChartist.DataModels;
using UltimateChartist.Indicators.Display;

namespace UltimateChartist.Indicators;

public class StockIndicator_TrailATRBOX : TrailStopBase
{
    public override string Description => "Trail Stop based on ATR";

    public override string DisplayName => $"{ShortName}({AtrPeriod},{UpWidth},{downWidth})";

    private int atrPeriod = 20;
    [IndicatorParameterInt("ATR Period", 1, 500)]
    public int AtrPeriod { get => atrPeriod; set { if (atrPeriod != value) { atrPeriod = value; RaiseParameterChanged(); } } }

    private decimal upWidth = 1;
    [IndicatorParameterDecimal("Up Width", 0, 50, 0.1, "{0:F2}")]
    public decimal UpWidth { get => upWidth; set { if (upWidth != value) { upWidth = value; RaiseParameterChanged(); } } }

    private decimal downWidth = 1;
    [IndicatorParameterDecimal("Down Width", 0, 50, 0.1, "{0:F2}")]
    public decimal DownWidth { get => downWidth; set { if (downWidth != value) { downWidth = value; RaiseParameterChanged(); } } }

    protected override IndicatorTrailValue[] InitializeTrailStop(StockSerie stockSerie)
    {
        var values = new IndicatorTrailValue[stockSerie.Bars.Count];
        var atrSerie = stockSerie.Bars.CalculateATR().CalculateEMA(AtrPeriod).Mult(upWidth);

        var longStop = new decimal?[stockSerie.Bars.Count];
        var shortStop = new decimal?[stockSerie.Bars.Count];

        bool upTrend = false;
        bool downTrend = false;
        var previousLow = stockSerie.Bars.First().Low;
        var previousHigh = stockSerie.Bars.First().High;

        int i = 1;
        foreach (var currentBar in stockSerie.Bars.Skip(1))
        {
            if (upTrend)
            {
                if (currentBar.Close < longStop[i - 1])
                { // Trailing stop has been broken => reverse trend
                    upTrend = false;
                    previousLow = currentBar.Low;
                }
                else
                {
                    // UpTrend still in place
                    var nbBox = (int)Math.Floor((currentBar.Close - longStop[i - 1].Value) / atrSerie[i - 1]) -1;
                    if (nbBox >= 1)
                        longStop[i] = longStop[i - 1] + atrSerie[i - 1] * nbBox;
                    else
                        longStop[i] = longStop[i - 1];
                }
            }
            else
            {
                if (currentBar.Close > previousLow + atrSerie[i - 1])
                {  // Up trend staring
                    upTrend = true;
                    longStop[i] = previousLow;
                }
                else
                {
                    // Up trend not started yet
                    previousLow = Math.Min(previousLow, currentBar.Low);
                }
            }

            if (downTrend)
            {
                if (currentBar.Close > shortStop[i - 1])
                { // Trailing stop has been broken => reverse trend
                    downTrend = false;
                    previousHigh = currentBar.High;
                }
                else
                {
                    // downTrend still in place
                    var nbBox = (int)Math.Floor((shortStop[i - 1].Value - currentBar.Close) / atrSerie[i - 1]) - 1;
                    if (nbBox >= 1)
                        shortStop[i] = shortStop[i - 1] - atrSerie[i - 1] * nbBox;
                    else
                        shortStop[i] = shortStop[i - 1];
                }
            }
            else
            {
                if (currentBar.Close < previousHigh - atrSerie[i - 1])
                {  // Down trend staring
                    downTrend = true;
                    shortStop[i] = previousHigh;
                }
                else
                {
                    // Down trend not started yet
                    previousHigh= Math.Max(previousHigh, currentBar.High);
                }
            }
            i++;
        }


        i = 0;
        foreach (var bar in stockSerie.Bars)
        {
            values[i] = new IndicatorTrailValue()
            {
                Date = bar.Date,
                High = longStop[i] == null ? null : bar.Close,
                Low = shortStop[i] == null ? null : bar.Close,
                Long = longStop[i],
                Short = shortStop[i]
            };
            i++;
        }

        return values;
    }
}
