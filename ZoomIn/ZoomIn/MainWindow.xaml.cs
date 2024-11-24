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

            InstrumentComboBox.SelectedIndex = 0;


            MainViewModel.Instance.ChartViewModel = tradeChartControl.ViewModel;
        }

        private void TradeChartControl_CurrentBarChanged(object sender, Bar bar)
        {
            MainViewModel.Instance.CurrentBar = bar;
        }
    }
}
