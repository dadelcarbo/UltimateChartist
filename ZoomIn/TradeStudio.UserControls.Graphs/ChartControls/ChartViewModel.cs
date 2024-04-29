using System;
using System.Collections.ObjectModel;
using System.Windows;
using Telerik.Windows.Controls;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Instruments;
using System.Windows.Media;
using System.Linq;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    public class ChartViewModel : ViewModelBase
    {
        #region ZOOMING
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
        #endregion    #region THEME & INDICATORS

        public ObservableCollection<IIndicator> PriceIndicators { get; set; } = new ObservableCollection<IIndicator>();
        public ObservableCollection<IndicatorChartViewModel> Indicators { get; } = new ObservableCollection<IndicatorChartViewModel>();

        public void RemoveIndicator(IndicatorChartViewModel indicatorViewModel)
        {
            Indicators.Remove(indicatorViewModel);
        }

        private TradeTheme theme = TradeTheme.Themes.First();
        public TradeTheme Theme
        {
            get { return theme; }
            set
            {
                if (theme != value)
                {
                    theme = value;
                    Indicators.Clear();
                    PriceIndicators.Clear();
                    if (theme != null)
                    {
                        foreach (var indicator in theme.Indicators)
                        {
                            AddIndicator(indicator);
                        }
                    }

                    RaisePropertyChanged();
                }
            }
        }

        public void AddIndicator(IIndicator indicator)
        {
            switch (indicator.DisplayType)
            {
                case DisplayType.Price:
                case DisplayType.TrailStop:
                    PriceIndicators.Add(indicator);
                    break;
                case DisplayType.Volume:
                case DisplayType.Ranged:
                case DisplayType.NonRanged:
                    Indicators.Add(new IndicatorChartViewModel(this, indicator));
                    break;
                default:
                    throw new NotImplementedException($"DisplayType {indicator.DisplayType} not implemented !");
            }
        }
        public void RemoveIndicator(IIndicator indicator)
        {
            switch (indicator.DisplayType)
            {
                case DisplayType.Price:
                case DisplayType.TrailStop:
                    PriceIndicators.Remove(indicator);
                    break;
                case DisplayType.Ranged:
                case DisplayType.NonRanged:
                    Indicators.RemoveAll(i => i.Indicator?.Indicator == indicator);
                    break;
                case DisplayType.Volume:
                    Indicators.RemoveAll(i => i.Indicator == indicator);
                    break;
                default:
                    break;
            }
        }



        public int MaxIndex => serie?.Bars == null ? 0 : serie.Bars.Count - 1;

        private TradeInstrument instrument;
        public TradeInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; this.DataSerie = instrument.GetDataSerie(duration); }
        }

        private BarDuration duration = BarDuration.Daily;
        public BarDuration Duration
        {
            get { return duration; }
            set { duration = value; this.DataSerie = instrument?.GetDataSerie(duration); }
        }

        bool changingSerie = false;
        private DataSerie serie;
        public DataSerie DataSerie
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


        private Brush mouseBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
        public Brush MouseBrush { get { return mouseBrush; } set { if (mouseBrush != value) { mouseBrush = value; RaisePropertyChanged(); } } }

        private Brush gridBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20));
        public Brush GridBrush { get { return gridBrush; } set { if (gridBrush != value) { gridBrush = value; RaisePropertyChanged(); } } }

        private Brush textBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0XB0, 0xB0, 0xB0));
        public Brush TextBrush { get { return textBrush; } set { if (textBrush != value) { textBrush = value; RaisePropertyChanged(); } } }

    }
}
