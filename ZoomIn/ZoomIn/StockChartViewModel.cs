using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ZoomIn
{
    public class StockChartViewModel : INotifyPropertyChanged
    {
        public StockChartViewModel()
        {
            this.Bars = StockBar.Load(@"C:\ProgramData\UltimateChartist_new\Data\Archive\ABC\Daily\CAC_FR0003500008.csv").Skip(20).Take(25).ToArray();
            this.Values = Bars.Select(b=>b.Close).ToArray();
            this.startIndex = 0;
            this.endIndex = Values.Length - 1;
            this.maxIndex = Values.Length - 1;
            //Random rnd = new();

            //double value = 100;
            //this.Values = new double[size];
            //for (int i = 0; i < size; i++)
            //{
            //    Values[i] = value;
            //    value += 10 * (rnd.NextDouble() - 0.5);
            //}

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

        public StockBar[] Bars { get; set; }


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
}
