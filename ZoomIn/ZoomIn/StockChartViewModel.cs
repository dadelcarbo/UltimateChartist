using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ZoomIn
{
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
    public class StockChartViewModel : INotifyPropertyChanged
    {
        public StockChartViewModel()
        {
            this.Bars = StockBar.Load(@"C:\ProgramData\UltimateChartist_new\Data\Archive\ABC\Daily\CAC_FR0003500008.csv").ToArray();
            this.Series = new List<StockSerie>
            {
                new StockSerie { Name = "CAC40", Bars = Bars}
            };
            this.Series.AddRange(this.GenerateSeries());
            this.Serie = this.Series[0];

            this.Values = Bars.Select(b => b.Close).ToArray();
            this.Values2 = Bars.Select(b => b.Close).Reverse().ToArray();
            this.startIndex = 0;
            this.endIndex = Values.Length - 1;
            this.maxIndex = Values.Length - 1;
        }

        List<StockSerie> GenerateSeries()
        {
            double degToRad = Math.PI / 180.0;
            var rnd = new Random();
            var series = new List<StockSerie>();
            foreach (var duration in Enum.GetValues<BarDuration>())
            {
                TimeSpan timeSpan = TimeSpan.FromDays(1);
                switch (duration)
                {
                    case BarDuration.M_1:
                        timeSpan = TimeSpan.FromMinutes(1);
                        break;
                    case BarDuration.M_2:
                        timeSpan = TimeSpan.FromMinutes(2);
                        break;
                    case BarDuration.M_5:
                        timeSpan = TimeSpan.FromMinutes(5);
                        break;
                    case BarDuration.M_15:
                        timeSpan = TimeSpan.FromMinutes(15);
                        break;
                    case BarDuration.M_30:
                        timeSpan = TimeSpan.FromMinutes(30);
                        break;
                    case BarDuration.H_1:
                        timeSpan = TimeSpan.FromHours(1);
                        break;
                    case BarDuration.H_2:
                        timeSpan = TimeSpan.FromHours(2);
                        break;
                    case BarDuration.H_4:
                        timeSpan = TimeSpan.FromHours(4);
                        break;
                    case BarDuration.Daily:
                        timeSpan = TimeSpan.FromDays(1);
                        break;
                    case BarDuration.Weekly:
                        timeSpan = TimeSpan.FromDays(7);
                        break;
                    case BarDuration.Monthly:
                        timeSpan = TimeSpan.FromDays(31);
                        break;
                    default:
                        break;
                }
                var serie = new StockSerie() { Name = duration.ToString(), Bars = new StockBar[500] };
                double value = 100;
                DateTime date = DateTime.Today;
                for (int i = 0; i < 500; i++)
                {
                    var open = value * (1 + 0.02 * (rnd.NextDouble() - 0.5));
                    var close = open * (1 + 0.02 * (rnd.NextDouble() - 0.5));
                    var high = Math.Max(open, close) * (1 + 0.01 * rnd.NextDouble());
                    var low = Math.Min(open, close) * (1 - 0.01 * rnd.NextDouble());
                    var volume = rnd.Next(100000);
                    serie.Bars[i] = new StockBar(date, open, high, low, close, volume);

                    date = date + timeSpan;
                    value = close;
                }
                series.Add(serie);
            }
            return series;
        }

        private int startIndex;
        public int StartIndex
        {
            get { return startIndex; }
            set { if (startIndex != value) { startIndex = value; RaisePropertyChanged(); } }
        }

        private int endIndex;
        public int EndIndex
        {
            get { return endIndex; }
            set { if (endIndex != value) { endIndex = value; RaisePropertyChanged(); } }
        }

        private int maxIndex;

        public int MaxIndex
        {
            get { return maxIndex; }
            set { if (maxIndex != value) { maxIndex = value; RaisePropertyChanged(); } }
        }

        public double[] Values { get; set; }
        public double[] Values2 { get; set; }

        public StockBar[] Bars { get; set; }

        public List<StockSerie> Series { get; set; }

        private StockSerie serie;
        public StockSerie Serie { get { return serie; } set { if (serie != value) { serie = value; RaisePropertyChanged(); } } }

        private Point mousePoint;
        public Point MousePoint
        {
            get => mousePoint;
            set { if (mousePoint != value) { mousePoint = value; RaisePropertyChanged(); } }
        }

        private Point canvasMousePoint;
        public Point CanvasMousePoint
        {
            get => canvasMousePoint;
            set { if (canvasMousePoint != value) { canvasMousePoint = value; RaisePropertyChanged(); } }
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }


    public enum BarDuration
    {
        M_1,
        M_2,
        M_5,
        M_15,
        M_30,
        H_1,
        H_2,
        H_4,
        Daily,
        Weekly,
        Monthly
    }
}
