using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using UltimateChartist.DataModels;
using UltimateChartist.DataModels.DataProviders;
using UltimateChartist.UserControls.ChartControls;

namespace UltimateChartist;

public class MainWindowViewModel : ViewModelBase
{
    #region Singleton
    private MainWindowViewModel()
    {
        this.Instruments = new ObservableCollection<Instrument>();
    }
    static private MainWindowViewModel instance = null;
    static public MainWindowViewModel Instance => instance ??= new MainWindowViewModel();
    #endregion

    #region Chart Views
    private ChartViewModel currentChartView;
    public ChartViewModel CurrentChartView { get => currentChartView; set { if (currentChartView != value) { currentChartView = value; RaisePropertyChanged(); } } }
    #endregion

    public ObservableCollection<Instrument> Instruments { get; }

    public void StartUp()
    {
        this.Instruments.AddRange(StockDataProviderBase.InitStockDictionary());

        // @@@@ Daily alerts
        //foreach (var instrument in this.Instruments.Where(i => i.Group == StockGroup.EURO_A))
        //{
        //    var serie = instrument.GetStockSerie(BarDuration.Daily);
        //    if (serie != null)
        //    {
        //        var emaIndicator = new StockIndicator_EMA() { Period = 85 };
        //        emaIndicator.Initialize(serie);
        //        var events = emaIndicator.Series.Values.Last().Events as StockEvent_MA;
        //        if (events != null)
        //        {
        //            if (events.IsAbove)
        //            {
        //                StockLog.Write($"Instrument: {instrument.Name} is above EMA(85)");
        //            }
        //            else
        //            {
        //                StockLog.Write($"Instrument: {instrument.Name} is below EMA(85)");
        //            }
        //        }
        //    }
        //}
    }
}
