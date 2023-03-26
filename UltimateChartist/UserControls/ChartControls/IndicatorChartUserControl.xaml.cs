using System.Windows.Controls;
using UltimateChartist.Indicators;

namespace UltimateChartist.UserControls.ChartControls;

/// <summary>
/// Interaction logic for IndicatorChartUserControl.xaml
/// </summary>
public partial class IndicatorChartUserControl : UserControl
{
    private IndicatorChartViewModel viewModel;
    public IndicatorChartUserControl(IndicatorChartViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
        this.viewModel = viewModel;

        foreach (var series in viewModel.Indicator.CartesianSeries)
        {
            this.indicatorChart.Series.Insert(0, series);
        }
        var rangedIndicator = viewModel.Indicator.Indicator as IRangedIndicator;
        if (rangedIndicator != null)
        {
            verticalAxis.Minimum = (double)rangedIndicator.Minimum;
            verticalAxis.Maximum = (double)rangedIndicator.Maximum;
        }
    }
}
