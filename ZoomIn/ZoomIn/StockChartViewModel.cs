﻿using System;
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
            this.Bars = StockBar.Load(@"C:\ProgramData\UltimateChartist_new\Data\Archive\ABC\Daily\CAC_FR0003500008.csv").Take(500).ToArray();
            this.Series = new StockSerie[]
            {
                new StockSerie { Name = "Test1", Bars = Bars.Take(250).ToArray()},
                new StockSerie { Name = "Test2", Bars = Bars.Skip(10).Take(250).ToArray()}
            };
            this.Serie = this.Series[0];

            this.Values = Bars.Select(b => b.Close).ToArray();
            this.Values2 = Bars.Select(b => b.Close).Reverse().ToArray();
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
        public double[] Values2 { get; set; }

        public StockBar[] Bars { get; set; }

        public StockSerie[] Series { get; set; }
        public StockSerie Serie { get; set; }

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
