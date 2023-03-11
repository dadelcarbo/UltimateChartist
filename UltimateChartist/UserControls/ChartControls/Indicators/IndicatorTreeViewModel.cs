using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorTreeViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public ObservableCollection<IndicatorTreeViewModel> Items { get; set; }
    }
}
