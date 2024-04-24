using System.Windows;
using UltimateChartistControls.ChartControls;

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

            this.chartControlDebug.DataContext = StockChartViewModel.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.contentBorder.Child = new StockChartControl(StockChartViewModel.Instance.ChartControlViewModel);
        }
    }
}
