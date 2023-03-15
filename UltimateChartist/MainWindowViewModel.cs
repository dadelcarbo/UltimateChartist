using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model.Themes;
using UltimateChartist.DataModels;
using UltimateChartist.DataModels.DataProviders;
using UltimateChartist.Helpers;
using UltimateChartist.Indicators;
using UltimateChartist.Indicators.Theme;
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


    public ObservableCollection<StockTheme> Themes { get; private set; }
    public StockTheme EmptyTheme { get; private set; }


    public void StartUp()
    {
        this.Instruments.AddRange(StockDataProviderBase.InitStockDictionary());

        this.Themes = new ObservableCollection<StockTheme>(Directory.EnumerateFiles(Folders.Theme, "*.thm").Select(f => StockTheme.Load(f)).Where(t => t != null));
        this.EmptyTheme = this.Themes.FirstOrDefault(t => t.Name == "Empty" );
        if (EmptyTheme==null)
        {
            this.EmptyTheme = new StockTheme { Name = "Empty" };
            this.EmptyTheme.Save();
        }

        //var theme = new StockTheme()
        //{
        //    Name = "Test2"
        //};
        //theme.Indicators.Add(new StockIndicator_EMA());
        //theme.Indicators.Add(new StockIndicator_MACD());
        //theme.Indicators.Add(new StockIndicator_STOCK());
        //theme.Save();

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
