using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.DataProviders.ABCDataProvider;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Instruments;
using TradeStudio.UserControls.Graphs.ChartControls;

namespace ZoomIn
{
    internal class DumbProgress : INotifyProgress
    {
        public void NotifyProgressContent(string text) { }
        public void NotifyProgressFooter(string footer, string text) { }
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        static private MainViewModel instance;
        static public MainViewModel Instance => instance ??= new MainViewModel();

        private MainViewModel()
        {
            instance = this;

            Folders.Initialize("C:\\Users\\david\\OneDrive\\TradeStudio", "C:\\ProgramData\\TradeStudio");
            ABCDataProvider.Instance.InitializeAsync(new DumbProgress(), false).Wait();
        }

        private ChartViewModel chartViewModel;
        public ChartViewModel ChartViewModel { get { return chartViewModel; } set { if (chartViewModel != value) { chartViewModel = value; RaisePropertyChanged(); } } }

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

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
        #endregion

        public System.Collections.IEnumerable Instruments => TradeInstrument.Instruments;

        public List<BarDuration> Durations => new() { BarDuration.Daily, BarDuration.Weekly, BarDuration.Monthly };

        public BarDuration Duration
        {
            get => ChartViewModel == null ? BarDuration.Daily : ChartViewModel.Duration;
            set { ChartViewModel.Duration = value; }
        }

        public List<BarType> BarTypes => new() { BarType.BarChart, BarType.Line, BarType.Candle };

        public BarType BarType
        {
            get => ChartViewModel == null ? BarType.Candle : ChartViewModel.BarType;
            set { ChartViewModel.BarType = value; }
        }

        private int emaPeriod = 20;

        public int EmaPeriod { get => emaPeriod; set => SetProperty(ref emaPeriod, value); }

        private DelegateCommand addIndicator;
        public ICommand AddIndicator => addIndicator ??= new DelegateCommand(PerformAddIndicator);

        private void PerformAddIndicator(object commandParameter)
        {
            var indicator = new TradeIndicator_EMA { Period = emaPeriod };
            ChartViewModel.AddIndicator(indicator);
        }
    }
}
