using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using UltimateChartist.DataModels;
using UltimateChartist.Indicators;
using UltimateChartist.Indicators.Theme;
using UltimateChartist.UserControls.ChartControls.Indicators;

namespace UltimateChartist.UserControls.ChartControls;


[DebuggerDisplay("Name={Instrument?.Name} Duration={BarDuration} Theme={Theme?.Name}")]
public class ChartViewModel : ViewModelBase
{
    const double ZOOM_MARGIN = 0.025; // %

    public ChartViewModel(Instrument instrument)
    {
        Instrument = instrument;
    }
    public string Name => this.Instrument?.Name;

    #region THEME & INDICATORS
    public ObservableCollection<IndicatorChartViewModel> Indicators { get; } = new ObservableCollection<IndicatorChartViewModel>();

    public void RemoveIndicator(IndicatorChartViewModel indicatorViewModel)
    {
        Indicators.Remove(indicatorViewModel);
    }

    private StockTheme theme;
    public StockTheme Theme
    {
        get { return theme; }
        set
        {
            if (theme != value)
            {
                theme = value;
                this.Indicators.Clear();
                this.PriceIndicators.Clear();
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
                this.Indicators.Add(new IndicatorChartViewModel(this, indicator));
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
                this.Indicators.RemoveAll(i => i.Indicator?.Indicator == indicator);
                break;
            case DisplayType.Volume:
                this.Indicators.RemoveAll(i => i.Indicator == indicator);
                break;
            default:
                break;
        }
    }
    #endregion


    private Instrument instrument;
    public Instrument Instrument
    {
        get => instrument;
        set
        {
            if (value != null && instrument != value)
            {
                instrument = value;
                if (!instrument.SupportedBarDurations.Contains(barDuration))
                {
                    this.BarDuration = instrument.DataProvider.DefaultBarDuration;
                }
                StockSerie = instrument.GetStockSerie(barDuration);
                Data = StockSerie.Bars;

                RaisePropertyChanged();
            }
        }
    }

    private int nbBar = 100;
    private void ResetZoom()
    {
        horizontalZoomRangeEnd = 1;
        HorizontalZoomRangeStart = Math.Max(0, 1 - (double)nbBar / Data.Count);
    }

    private Size maxZoom = new Size(100, 100);

    public Size MaxZoom
    {
        get { return maxZoom; }
        set { if (value != maxZoom) { maxZoom = value; RaisePropertyChanged(); } }
    }


    private BarDuration barDuration = BarDuration.Daily;
    public BarDuration BarDuration
    {
        get => barDuration;
        set
        {
            if (barDuration != value)
            {
                barDuration = value;
                StockSerie = instrument.GetStockSerie(barDuration);
                Data = StockSerie?.Bars;

                OnPropertyChanged(nameof(AxisLabelTemplate));
                RaisePropertyChanged();
            }
        }
    }

    #region Chart Data

    public StockSerie StockSerie { get; set; }

    private List<StockBar> data;
    public List<StockBar> Data
    {
        get => data;
        set
        {
            if (data != value)
            {
                data = value;
                if (data == null)
                {
                    RaisePropertyChanged();
                    return;
                }
                //MaxZoom = new Size(Math.Max(1, data.Count / nbBar), 100);

                var max = 0m;
                if (data != null && data.Count > 0)
                {
                    max = data.Max(d => d.High);
                    foreach (var indicator in PriceIndicators)
                    {
                        indicator.Initialize(StockSerie);
                    }
                }
                this.Maximum = (1 + ZOOM_MARGIN) * (double)max;

                ResetZoom();

                RaisePropertyChanged();
            }
        }
    }

    private double maximum;
    public double Maximum { get { return maximum; } set { if (maximum != value) { maximum = value; RaisePropertyChanged(); } } }

    private double horizontalZoomRangeStart;
    public double HorizontalZoomRangeStart
    {
        get => horizontalZoomRangeStart;
        set
        {
            if (horizontalZoomRangeStart != value)
            {
                horizontalZoomRangeStart = value;
                CalculateVerticalZoom();
                RaisePropertyChanged();
            }
        }
    }

    private double horizontalZoomRangeEnd;
    public double HorizontalZoomRangeEnd
    {
        get => horizontalZoomRangeEnd;
        set
        {
            if (horizontalZoomRangeEnd != value)
            {
                horizontalZoomRangeEnd = value;
                CalculateVerticalZoom();
                RaisePropertyChanged();
            }
        }
    }

    private void CalculateVerticalZoom()
    {
        int startIndex = (int)Math.Floor(horizontalZoomRangeStart * Data.Count);
        int endIndex = (int)Math.Ceiling(horizontalZoomRangeEnd * Data.Count) - 1;
        if (startIndex == endIndex)
            return;
        var visibleData = Data.Skip(startIndex).Take(endIndex - startIndex).ToList();
        var min = (double)visibleData.Min(f => f.Low);
        var max = (double)visibleData.Max(f => f.High);
        var margin = ZOOM_MARGIN * (max - min);
        this.VerticalZoomRangeStart = (min - margin) / Maximum;
        this.VerticalZoomRangeEnd = (max + margin) / Maximum;
    }

    private double verticalZoomRangeStart;
    public double VerticalZoomRangeStart { get => verticalZoomRangeStart; set { if (verticalZoomRangeStart != value) { verticalZoomRangeStart = value; RaisePropertyChanged(); } } }

    private double verticalZoomRangeEnd;
    public double VerticalZoomRangeEnd { get => verticalZoomRangeEnd; set { if (verticalZoomRangeEnd != value) { verticalZoomRangeEnd = value; RaisePropertyChanged(); } } }

    private SeriesType seriesType;
    public SeriesType SeriesType { get => seriesType; set { if (seriesType != value) { seriesType = value; RaisePropertyChanged(); } } }

    public DataTemplate AxisLabelTemplate => BarDuration switch
    {
        BarDuration.Daily => App.Current.FindResource($"axisDailyLabelTemplate") as DataTemplate,
        BarDuration.Weekly => App.Current.FindResource($"axisDailyLabelTemplate") as DataTemplate,
        BarDuration.Monthly => App.Current.FindResource($"axisDailyLabelTemplate") as DataTemplate,
        _ => App.Current.FindResource($"axisIntradayLabelTemplate") as DataTemplate
    };

    public ObservableCollection<IIndicator> PriceIndicators { get; set; } = new ObservableCollection<IIndicator>();
    #endregion

    public ChartProperties ChartProperties => new ChartProperties();

}
