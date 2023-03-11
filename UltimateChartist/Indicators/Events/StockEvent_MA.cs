namespace UltimateChartist.Indicators.Events
{
    public class StockEvent_MA : IStockEvents
    {
        public bool CrossAbove { get; set; }
        public bool CrossBelow { get; set; }
        public bool IsAbove { get; set; }
        public bool IsBelow => !IsAbove;
    }
}
