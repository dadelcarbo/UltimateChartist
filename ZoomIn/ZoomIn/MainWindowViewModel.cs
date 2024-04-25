using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.DataProviders.ABCDataProvider;
using TradeStudio.Data.Instruments;
using TradeStudio.UserControls.Graphs.ChartControls;

namespace ZoomIn
{
    internal class DumbProgress : INotifyProgress
    {
        public void NotifyProgressContent(string text) { }
        public void NotifyProgressFooter(string footer, string text) { }
    }
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        static private MainWindowViewModel instance;
        static public MainWindowViewModel Instance => instance ??= new MainWindowViewModel();

        private MainWindowViewModel()
        {
            instance = this;

            Folders.Initialize("C:\\Users\\david\\OneDrive\\TradeStudio", "C:\\ProgramData\\TradeStudio");
            ABCDataProvider.Instance.InitializeAsync(new DumbProgress(), false).Wait();
        }

        public ChartControlViewModel ChartControlViewModel { get; set; }

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

    }
}
