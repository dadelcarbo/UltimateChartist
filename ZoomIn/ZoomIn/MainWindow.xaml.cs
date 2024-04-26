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
        public MainWindow()
        {
            this.DataContext = MainWindowViewModel.Instance;
            InitializeComponent();

            MainWindowViewModel.Instance.ChartControlViewModel = this.StockChartControl.ViewModel;
        }
    }
}
