
static public class MathExtension
{
    public static double[] CalculateMACD(this double[] values, int fastPeriod, int slowPeriod)
    {
        var macd = new double[values.Length];
        double fastEma = values[0];
        double slowEma = values[0];
        double fastAlpha = 2.0 / (fastPeriod + 1);
        double slowAlpha = 2.0 / (slowPeriod + 1);

        macd[0] = 0;
        for (int i = 1; i < values.Length; i++)
        {
            fastEma = fastEma + fastAlpha * (values[i] - fastEma);
            slowEma = slowEma + slowAlpha * (values[i] - slowEma);
            macd[i] = fastEma - slowEma;
        }
        return macd;
    }
    public static double[] CalculateEMA(this double[] values, int period)
    {
        var ema = new double[values.Length];
        if (period <= 1)
        {
            for (int i = 0; i < values.Length; i++)
            {
                ema[i] = values[i];
            }
        }
        else
        {
            double alpha = 2.0 / (period + 1);

            ema[0] = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                ema[i] = ema[i - 1] + alpha * (values[i] - ema[i - 1]);
            }
        }
        return ema;
    }
}
