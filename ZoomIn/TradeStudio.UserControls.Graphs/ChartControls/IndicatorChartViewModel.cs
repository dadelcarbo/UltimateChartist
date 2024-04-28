using System;
using System.Windows.Input;
using Telerik.Windows.Controls;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Indicators;
using TradeStudio.UserControls.Graphs.ChartControls.Indicators;

namespace TradeStudio.UserControls.Graphs.ChartControls;

public class IndicatorChartViewModel : ViewModelBase
{
    public ChartViewModel ChartViewModel { get; }
    public IndicatorViewModel Indicator { get; }

    public IndicatorChartViewModel(ChartViewModel chartViewModel, IIndicator indicator)
    {
        ChartViewModel = chartViewModel;
        Indicator = new IndicatorViewModel(indicator, chartViewModel.DataSerie);
    }

    #region Commands

    private DelegateCommand deleteIndicatorCommand;
    public ICommand DeleteIndicatorCommand => deleteIndicatorCommand ??= new DelegateCommand(DeleteIndicator);

    private void DeleteIndicator(object commandParameter)
    {
        ChartViewModel.RemoveIndicator(this);
    }

    internal void Initialize(DataSerie dataSerie)
    {
        this.Indicator.Indicator.Initialize(dataSerie);
    }
    #endregion
}
