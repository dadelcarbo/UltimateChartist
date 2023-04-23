using System.ComponentModel;
using System.Windows.Media;
using Telerik.Windows.Controls.Calendar;
using ZoomIn.StockControl;

namespace ZoomIn.ChartControls
{
    /// <summary>
    /// Interaction logic for OverviewControl.xaml
    /// </summary>
    public partial class OverviewControl : ChartControlBase
    {
        public OverviewControl()
        {
            InitializeComponent();
        }
        protected override void OnStockSerieChanged()
        {
            this.overviewSlider.Maximum = viewModel.MaxIndex;
            this.overviewSlider.Selection = viewModel.Range;

            this.overviewGraph.Children.Clear();

            var closeSerie = viewModel?.Serie?.CloseValues;
            if (closeSerie == null)
                return;

            #region Create overview
            var overviewCurve = new OverviewCurve()
            {
                Values = closeSerie,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2
            };

            this.overviewGraph.Children.Add(overviewCurve);
            #endregion
        }

        private void overviewSlider_SelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (viewModel == null)
                return;
            this.viewModel.Range = overviewSlider.Selection;
        }

        protected override void OnResize()
        {
        }

        protected override void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }
    }
}
