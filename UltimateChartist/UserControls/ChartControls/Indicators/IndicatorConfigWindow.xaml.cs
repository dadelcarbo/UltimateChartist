using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

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
            this.indicatorConfigViewModel = IndicatorConfigViewModel.GetInstance(chartViewModel, this);
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
                this.IndicatorConfigPanel.Children.Add(new IndicatorConfigUserControl(itemViewModel.Indicator));
            }
            else
            {
                this.IndicatorConfigPanel.Children.Add(new ChartConfigUserControl(chartViewModel));
            }
        }

        private void deleteIndicatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            var item = btn?.ParentOfType<TreeListViewRow>()?.Item as IndicatorTreeViewModel;
            if (item == null)
                return;
            this.indicatorConfigViewModel.DeleteItem(item);
        }
    }
}
