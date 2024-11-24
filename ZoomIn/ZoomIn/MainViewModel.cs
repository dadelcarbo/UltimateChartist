using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model.Themes;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.DataProviders.ABCDataProvider;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.Data.Instruments;
using TradeStudio.UserControls.Graphs.ChartControls;
using TradeStudio.UserControls.Graphs.ChartControls.Indicators;

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

            Folders.Initialize($"C:\\Users\\{Environment.UserName}\\OneDrive\\TradeStudio", "C:\\ProgramData\\TradeStudio");
            ABCDataProvider.Instance.InitializeAsync(new DumbProgress(), false).Wait();

            Persister<TradeTheme>.Instance.Initialize(Path.Combine(Folders.UserFolder, "Themes"), "thm");
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

        private Bar currentBar;
        public Bar CurrentBar { get { return currentBar; } set { if (currentBar != value) { currentBar = value; RaisePropertyChanged(); } } }


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

        public List<BarType> BarTypes => new() { BarType.Line, BarType.Candle, BarType.BarChart };

        private DelegateCommand graphSettingsCommand;
        public ICommand GraphSettingsCommand => graphSettingsCommand ??= new DelegateCommand(GraphSettings);

        private void GraphSettings(object commandParameter)
        {
            var indicatorSelectorWindow = new RadWindow()
            {
                Content = new IndicatorConfigWindow(this.ChartViewModel),
                Owner = MainWindow.Instance,
                Header = "Theme configuration"
            };
            indicatorSelectorWindow.ShowDialog();
        }

        #region Add Indicator Drop Down Button

        private DelegateCommand dropDownClosedCommand;
        public ICommand DropDownClosedCommand => dropDownClosedCommand ??= new DelegateCommand(DropDownClosed);

        private void DropDownClosed(object commandParameter)
        {
            if (NewIndicator != null)
            {
                ChartViewModel.AddIndicator(new IndicatorSettings(NewIndicator.CreateInstance()));
                NewIndicator = null;
            }
        }

        private DelegateCommand dropDownOpeningCommand;
        public ICommand DropDownOpeningCommand => dropDownOpeningCommand ??= new DelegateCommand(DropDownOpening);

        private void DropDownOpening(object commandParameter)
        {
            NewIndicator = null;
        }

        private IndicatorDescriptor newIndicator;
        public IndicatorDescriptor NewIndicator { get => newIndicator; set { if (newIndicator != value) { newIndicator = value; RaisePropertyChanged(); } } }

        #endregion
    }
}
