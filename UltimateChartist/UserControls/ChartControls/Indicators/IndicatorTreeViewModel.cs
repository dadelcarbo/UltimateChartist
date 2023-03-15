using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using UltimateChartist.Indicators;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorTreeViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public ObservableCollection<IndicatorTreeViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeViewModel>();
        public IIndicator Indicator { get; set; }
    }
}
