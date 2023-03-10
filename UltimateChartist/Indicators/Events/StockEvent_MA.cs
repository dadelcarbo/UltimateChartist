using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
