using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ZoomIn.ChartControls;

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
        static private StockChartViewModel instance;
        static public StockChartViewModel Instance => instance;

        private ChartControlViewModel chartControlViewModel;
        public ChartControlViewModel ChartControlViewModel { get { return chartControlViewModel; } set { if (chartControlViewModel != value) { chartControlViewModel = value; RaisePropertyChanged(); } } }


        public StockChartViewModel()
        {
            instance = this;
            this.chartControlViewModel = new ChartControlViewModel();
            this.Bars = StockBar.Load(@"C:\ProgramData\UltimateChartist_new\Data\Archive\ABC\Daily\CAC_FR0003500008.csv").ToArray();
            this.Series = new List<StockSerie>
            {
                new StockSerie { Name = "CAC40", Bars = Bars}
            };
            this.Series.AddRange(this.GenerateSeries(2000));
            this.Serie = this.Series[0];

            this.Values = Bars.Select(b => b.Close).ToArray();
        }

        List<StockSerie> GenerateSeries(int nbBars)
        {
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
                var serie = new StockSerie() { Name = duration.ToString(), Bars = new StockBar[nbBars] };
                double value = 100;
                DateTime date = DateTime.Today;
                for (int i = 0; i < nbBars; i++)
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

        public double[] Values { get; set; }

        public StockBar[] Bars { get; set; }

        public List<StockSerie> Series { get; set; }

        private StockSerie serie;
        public StockSerie Serie { get { return serie; } set { if (serie != value) { serie = value; this.ChartControlViewModel.Serie = value; RaisePropertyChanged(); } } }

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
