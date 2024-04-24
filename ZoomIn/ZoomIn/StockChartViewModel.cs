using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Instruments;
using TradeStudio.UserControls.Graphs.ChartControls;

namespace ZoomIn
{
    public class StockChartViewModel : INotifyPropertyChanged
    {
        static private StockChartViewModel instance;
        static public StockChartViewModel Instance => instance ??= new StockChartViewModel();

        private ChartControlViewModel chartControlViewModel;
        public ChartControlViewModel ChartControlViewModel { get { return chartControlViewModel; } set { if (chartControlViewModel != value) { chartControlViewModel = value; RaisePropertyChanged(); } } }

        private StockChartViewModel()
        {
            instance = this;
            this.chartControlViewModel = new ChartControlViewModel();
            this.Bars = Bar.Load(@"C:\ProgramData\UltimateChartist_new\Data\Archive\ABC\Daily\CAC_FR0003500008.csv").ToArray();
            this.Series = new List<DataSerie>
            {
                new DataSerie(new TradeInstrument{ Name = "CAC40" }, BarDuration.Daily, Bars.ToList())
            };
            this.Series.AddRange(this.GenerateSeries(2000));
            this.Serie = this.Series[0];

            this.Values = Bars.Select(b => b.Close).ToArray();
        }

        List<DataSerie> GenerateSeries(int nbBars)
        {
            var rnd = new Random();
            var series = new List<DataSerie>();
            foreach (var duration in Enum.GetValues<BarDuration>())
            {
                TimeSpan timeSpan = TimeSpan.FromDays(1);
                switch (duration)
                {
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


                var serie = new DataSerie(new TradeInstrument { Name = duration.ToString() }, duration, new List<Bar>());
                double value = 100;
                DateTime date = DateTime.Today;
                for (int i = 0; i < nbBars; i++)
                {
                    var open = value * (1 + 0.02 * (rnd.NextDouble() - 0.5));
                    var close = open * (1 + 0.02 * (rnd.NextDouble() - 0.5));
                    var high = Math.Max(open, close) * (1 + 0.01 * rnd.NextDouble());
                    var low = Math.Min(open, close) * (1 - 0.01 * rnd.NextDouble());
                    var volume = rnd.Next(100000);
                    serie.Bars.Add(new Bar(date, open, high, low, close, volume));

                    date = date + timeSpan;
                    value = close;
                }
                series.Add(serie);
            }
            return series;
        }

        public double[] Values { get; set; }

        public Bar[] Bars { get; set; }

        public List<DataSerie> Series { get; set; }

        private DataSerie serie;
        public DataSerie Serie { get { return serie; } set { if (serie != value) { serie = value; this.ChartControlViewModel.Serie = value; RaisePropertyChanged(); } } }

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
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
