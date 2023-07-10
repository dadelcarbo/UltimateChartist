using System.Linq;

namespace UltimateChartistLib;

public class StockSerie
{
    public StockBar[] Bars { get; set; }
    public string Name { get; set; }

    private double[] closeValues;
    public double[] CloseValues => closeValues ??= this.Bars?.Select(b => b.Close).ToArray();

    private double[] lowValues;
    public double[] LowValues => lowValues ??= this.Bars?.Select(b => b.Low).ToArray();

    private double[] highValues;
    public double[] HighValues => highValues ??= this.Bars?.Select(b => b.High).ToArray();
}
