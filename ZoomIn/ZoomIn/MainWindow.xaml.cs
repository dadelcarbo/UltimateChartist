using System.Windows;
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
            InitializeComponent();

            this.chartControlDebug.DataContext = MainWindowViewModel.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.contentBorder.Child = new StockChartControl(MainWindowViewModel.Instance.ChartControlViewModel);
        }
    }
}
