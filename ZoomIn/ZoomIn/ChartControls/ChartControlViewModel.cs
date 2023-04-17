using System;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;

namespace ZoomIn.ChartControls
{
    public class ChartControlViewModel : ViewModelBase
    {
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

        public int MaxIndex => serie?.Bars == null ? 0 : serie.Bars.Length - 1;



        private StockSerie serie;
        public StockSerie Serie
        {
            get { return serie; }
            set
            {
                if (serie != value)
                {
                    serie = value;

                    this.endIndex = serie.Bars == null ? 0 : serie.Bars.Length - 1;
                    this.startIndex = Math.Max(0, this.EndIndex - (MinRange + MaxRange) / 2);
                    RaisePropertyChanged();
                }
            }
        }



        //private StockBar[] bars;
        //public StockBar[] Bars
        //{
        //    get => bars; set
        //    {
        //        if (bars == value) return;
        //        bars = value;
        //        this.endIndex = this.Bars == null ? 0 : this.Bars.Length - 1;
        //        this.startIndex = Math.Max(0, this.EndIndex - (MinRange + MaxRange) / 2);
        //        RaisePropertyChanged("MaxIndex");
        //        RaisePropertyChanged("StartIndex");
        //        RaisePropertyChanged("EndIndex");
        //    }
        //}

        private Point mousePos;
        public Point MousePos { get { return mousePos; } set { if (mousePos != value) { mousePos = value; RaisePropertyChanged(); } } }

        private Point mouseValue;
        public Point MouseValue { get { return mouseValue; } set { if (mouseValue != value) { mouseValue = value; RaisePropertyChanged(); } } }

        private int mouseIndex;
        public int MouseIndex { get { return mouseIndex; } set { if (mouseIndex != value) { mouseIndex = value; this.CurrentBar = serie?.Bars[mouseIndex]; RaisePropertyChanged(); } } }

        private StockBar currentBar;
        public StockBar CurrentBar { get { return currentBar; } set { if (currentBar != value) { currentBar = value; RaisePropertyChanged(); } } }

    }
}
