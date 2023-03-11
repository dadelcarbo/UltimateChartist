using System.Windows;
using Instrument = UltimateChartist.DataModels.Instrument;

namespace UltimateChartist.UserControls.InstrumentControls
{
    /// <summary>
    /// Interaction logic for InstrumentWindow.xaml
    /// </summary>
    public partial class InstrumentWindow : Window
    {
        public InstrumentWindow()
        {
            InitializeComponent();
        }

        private void RadGridView_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            MainWindowViewModel.Instance.CurrentChartView.Instrument = e.AddedItems[0] as Instrument;
        }
    }
}
