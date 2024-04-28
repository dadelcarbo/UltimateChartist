using System.Windows.Controls;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators
{
    /// <summary>
    /// Interaction logic for ChartConfigUserControl.xaml
    /// </summary>
    public partial class ChartConfigUserControl : UserControl
    {
        public ChartConfigUserControl(ChartViewModel chartViewModel)
        {
            InitializeComponent();
            // §§§§ propertyGrid.Item = chartViewModel.ChartProperties;
        }
    }
}
