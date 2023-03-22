namespace UltimateChartist.Indicators.Events
{
    public class StockEvent_TrailStop : IStockEvents
    {
        public bool BrokenUp { get; set; }
        public bool BrokenDown { get; set; }
        public bool Bullish { get; set; }
        public bool Bearish { get; set; }

        public bool LongReentry { get; set; }
        public bool FirstLongReentry { get; set; }

        public bool ShortReentry { get; set; }
        public bool FirstShortReentry { get; set; }

        public bool NewHigh { get; set; }
        public bool NewLow { get; set; }
    }
}
