using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace ZoomIn.ChartControls
{
    public class ChartControlViewModel : ViewModelBase
    {
        private SelectionRange<int> zoomRange;
        public SelectionRange<int> ZoomRange { get { return zoomRange; } set { if (zoomRange != value) { zoomRange = value; RaisePropertyChanged(); } } }

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

                    var endIndex = serie.Bars == null ? 0 : serie.Bars.Length - 1;
                    var startIndex = Math.Max(0, endIndex - (MinRange + MaxRange) / 2);

                    this.zoomRange = new SelectionRange<int> { Start = startIndex, End = endIndex };
                    RaisePropertyChanged();
                }
            }
        }

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
