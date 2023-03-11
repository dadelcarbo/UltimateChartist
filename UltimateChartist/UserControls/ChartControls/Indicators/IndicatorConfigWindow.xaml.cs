using System.Windows;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    /// <summary>
    /// Interaction logic for IndicatorConfigWindow.xaml
    /// </summary>
    public partial class IndicatorConfigWindow : Window
    {
        public IndicatorConfigWindow()
        {
            InitializeComponent();
        }

        private void AddIndicatorBtn_Click(object sender, RoutedEventArgs e)
        {
            var indicatorPicker = new IndicatorPicker();
            indicatorPicker.Show();
        }
    }
}
