using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomIn
{
    static class MathExtension
    {
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
                for (int i = 1; i < values.Count(); i++)
                {
                    ema[i] = ema[i - 1] + alpha * (values[i] - ema[i - 1]);
                }
            }
            return ema;
        }

    }
}
