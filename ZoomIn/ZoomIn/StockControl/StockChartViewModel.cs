using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZoomIn.StockControl
{
    class StockChartViewModel : INotifyPropertyChanged
    {
        private int minRange = 50;
        public int MinRange
        {
            get { return minRange; }
            set { if (minRange != value) { minRange = value; RaisePropertyChanged(); } }
        }
        private int maxRange = 100;
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
}
