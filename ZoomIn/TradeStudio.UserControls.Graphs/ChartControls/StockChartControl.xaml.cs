using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    /// <summary>
    /// Interaction logic for StockChartControl.xaml
    /// </summary>
    public partial class StockChartControl : UserControl
    {
        public StockChartControl(ChartControlViewModel viewModel)
        {
            InitializeComponent();

            foreach (var child in grid.ChildrenOfType<ChartControlBase>())
            {
                child.ViewModel = viewModel;
            }
        }
    }
}
