using System.Windows;
using TradeStudio.Data.DataProviders;
using TradeStudio.UserControls.Graphs.ChartControls;

namespace ZoomIn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;
            this.DataContext = MainViewModel.Instance;
            InitializeComponent();

            MainViewModel.Instance.ChartViewModel = this.StockChartControl.ViewModel;
            InstrumentComboBox.SelectedIndex = 0;
        }
    }
}
