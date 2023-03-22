using System.Windows.Media;
using Telerik.Windows.Controls;

namespace UltimateChartist.UserControls.ChartControls
{
    public class ChartProperties : ViewModelBase
    {
        #region CHART VISUAL
        private Brush background = Brushes.White;
        public Brush Background { get { return background; } set { if (background != value) { background = value; RaisePropertyChanged(); } } }

        Brush gridLineBrush = Brushes.LightGray;
        public Brush GridLineBrush { get { return gridLineBrush; } set { if (gridLineBrush != value) { gridLineBrush = value; RaisePropertyChanged(); } } }

        #endregion
    }
}