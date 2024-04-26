using System;
using System.Windows;
using Telerik.Windows.Controls;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Instruments;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    public class ChartControlViewModel : ViewModelBase
    {
        private SelectionRange<int> zoomRange;
        public SelectionRange<int> ZoomRange { get { return zoomRange; } set { if (zoomRange != value) { zoomRange = value; if (!changingSerie) RaisePropertyChanged(); } } }

        private int minRange = 25;
        public int MinRange
        {
            get { return minRange; }
            set { if (minRange != value) { minRange = value; RaisePropertyChanged(); } }
        }
        private int maxRange = 500;
        public int MaxRange
        {
            get { return maxRange; }
            set { if (maxRange != value) { maxRange = value; RaisePropertyChanged(); } }
        }

        public int MaxIndex => serie?.Bars == null ? 0 : serie.Bars.Count - 1;

        private TradeInstrument instrument;
        public TradeInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; this.Serie = instrument.GetDataSerie(duration); }
        }

        private BarDuration duration = BarDuration.Daily;
        public BarDuration Duration
        {
            get { return duration; }
            set { duration = value; this.Serie = instrument?.GetDataSerie(duration); }
        }

        bool changingSerie = false;
        private DataSerie serie;
        public DataSerie Serie
        {
            get { return serie; }
            set
            {
                if (serie != value)
                {
                    changingSerie = true;
                    serie = value;
                    OnPropertyChanged("MaxIndex");

                    var endIndex = serie.Bars == null ? 0 : serie.Bars.Count - 1;
                    var startIndex = Math.Max(0, endIndex - (MinRange + MaxRange) / 2);

                    this.zoomRange = new SelectionRange<int> { Start = startIndex, End = endIndex };

                    RaisePropertyChanged();
                    OnPropertyChanged("ChartVisibility");
                    changingSerie = false;
                }
            }
        }

        private Point mousePos;
        public Point MousePos { get { return mousePos; } set { if (mousePos != value) { mousePos = value; RaisePropertyChanged(); } } }

        private Point mouseValue;
        public Point MouseValue { get { return mouseValue; } set { if (mouseValue != value) { mouseValue = value; RaisePropertyChanged(); } } }

        private int mouseIndex;
        public int MouseIndex { get { return mouseIndex; } set { if (mouseIndex != value) { mouseIndex = value; this.CurrentBar = serie?.Bars[mouseIndex]; RaisePropertyChanged(); } } }

        private Bar currentBar;
        public Bar CurrentBar { get { return currentBar; } set { if (currentBar != value) { currentBar = value; RaisePropertyChanged(); } } }

        private BarType type = BarType.Candle;
        public BarType BarType { get { return type; } set { if (type != value) { type = value; RaisePropertyChanged(); } } }
    }
}
