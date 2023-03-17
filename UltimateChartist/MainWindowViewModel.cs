using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Persistence.Core;
using UltimateChartist.DataModels;
using UltimateChartist.DataModels.DataProviders;
using UltimateChartist.Helpers;
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

    private MainWindow mainWindow;
    public void SetMainWindow(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    static private MainWindowViewModel instance = null;
    static public MainWindowViewModel Instance => instance ??= new MainWindowViewModel();
    #endregion

    #region Chart Views
    private ChartViewModel currentChartView;
    public ChartViewModel CurrentChartView { get => currentChartView; set { if (currentChartView != value) { currentChartView = value; RaisePropertyChanged(); } } }
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
                this.mainWindow.AddChart(instrument, Themes.FirstOrDefault());
            }
        }
    }
    public ObservableCollection<Instrument> Instruments { get; }

    public ObservableCollection<StockTheme> Themes { get; private set; }

    public void StartUp()
    {
        this.Instruments.AddRange(StockDataProviderBase.InitStockDictionary());

        this.Themes = new ObservableCollection<StockTheme>(Directory.EnumerateFiles(Folders.Theme, "*.thm").Select(f => StockTheme.Load(f)).Where(t => t != null).OrderBy(t => t.Name));
        if (this.Themes.Count == 0)
        {
            this.Themes.Add(new StockTheme() { Name = "New" });
        }

        ApplicationThemes = typeof(Theme).Assembly.ExportedTypes.Where(t => t.IsAssignableTo(typeof(Theme))).Select(t => (Theme)Activator.CreateInstance(t));

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

    public IEnumerable<Theme> ApplicationThemes { get; set; }
    public Theme ApplicationTheme { get { return StyleManager.ApplicationTheme; } set { StyleManager.ApplicationTheme = value; } }
}
