using System;
using System.Collections.Generic;
using UltimateChartist.Indicators.Events;

namespace UltimateChartist.Indicators.Display;

public abstract class IndicatorValueBase
{
    public DateTime Date { get; set; }

    public IStockEvents Events { get; set; }
}

public interface IIndicatorSeries
{
    public IndicatorValueBase[] Values { get; set; }
}
