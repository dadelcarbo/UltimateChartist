using System;
using System.Windows;
using Telerik.Windows.Controls;

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

        public int MaxIndex => this.Bars == null ? 0 : this.Bars.Length - 1;


        private StockBar[] bars;
        public StockBar[] Bars
        {
            get => bars; set
            {
                if (bars == value) return;
                bars = value;
                this.EndIndex = this.Bars == null ? 0 : this.Bars.Length - 1;
                this.StartIndex = Math.Max(0, this.EndIndex - (MinRange + MaxRange) / 2);
                RaisePropertyChanged("MaxIndex");
            }
        }

        private Point mousePos;
        public Point MousePos { get { return mousePos; } set { if (mousePos != value) { mousePos = value; RaisePropertyChanged(); } } }

        private Point mouseValue;
        public Point MouseValue { get { return mouseValue; } set { if (mouseValue != value) { mouseValue = value; RaisePropertyChanged(); } } }

        private int mouseIndex;
        public int MouseIndex { get { return mouseIndex; } set { if (mouseIndex != value) { mouseIndex = value; RaisePropertyChanged(); } } }

        private StockBar currentBar;
        public StockBar CurrentBar { get { return currentBar; } set { if (currentBar != value) { currentBar = value; RaisePropertyChanged(); } } }

    }
}
