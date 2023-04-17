using System.Windows.Media;
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

        StockSerie stockSerie;
        protected override void OnStockSerieChanged(StockSerie newSerie)
        {
            this.overviewGraph.Children.Clear();
            if (this.stockSerie == newSerie)
                return;
            this.stockSerie = newSerie;

            var closeSerie = stockSerie?.CloseValues;
            if (closeSerie == null)
                return;

            var viewModel = (ChartControlViewModel)this.DataContext;
            viewModel.Serie = stockSerie;

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
    }
}
