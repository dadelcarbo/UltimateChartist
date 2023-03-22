using System.Windows.Controls;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    /// <summary>
    /// Interaction logic for ChartConfigUserControl.xaml
    /// </summary>
    public partial class ChartConfigUserControl : UserControl
    {
        public ChartConfigUserControl(ChartViewModel chartViewModel)
        {
            InitializeComponent();
            propertyGrid.Item = chartViewModel.ChartProperties;
        }
    }
}
