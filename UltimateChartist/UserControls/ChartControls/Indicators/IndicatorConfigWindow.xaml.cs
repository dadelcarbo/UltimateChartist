using System.Windows;
using Telerik.Windows.Controls.Data.CardView;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    /// <summary>
    /// Interaction logic for IndicatorConfigWindow.xaml
    /// </summary>
    public partial class IndicatorConfigWindow : Window
    {
        IndicatorConfigViewModel indicatorConfigViewModel;
        ChartViewModel chartViewModel;
        public IndicatorConfigWindow(ChartViewModel chartViewModel)
        {
            this.chartViewModel = chartViewModel;
            this.indicatorConfigViewModel = new IndicatorConfigViewModel(chartViewModel, this);
            this.DataContext = indicatorConfigViewModel;
            InitializeComponent();
        }

        public void DisplayConfigItem(IndicatorTreeViewModel itemViewModel)
        {
            this.IndicatorConfigPanel.Children.Clear();
            if (itemViewModel == null)
                return;

            if (itemViewModel.Indicator != null)
            {
                this.IndicatorConfigPanel.Children.Add(new IndicatorConfigUserControl(new IndicatorViewModel(itemViewModel.Indicator, chartViewModel.StockSerie)));
            }
        }
    }
}
