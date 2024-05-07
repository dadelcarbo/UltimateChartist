using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.Data.Instruments;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    /// <summary>
    /// Interaction logic for TradeChartControl.xaml
    /// </summary>
    public partial class TradeChartControl : UserControl
    {
        public BarDuration Duration
        {
            get { return (BarDuration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(BarDuration), typeof(TradeChartControl), new PropertyMetadata(BarDuration.Daily, DurationChanged));

        private static void DurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (TradeChartControl)d;
            chart.viewModel.Duration = chart.Duration;
        }

        public TradeInstrument Instrument
        {
            get { return (TradeInstrument)GetValue(InstrumentProperty); }
            set { SetValue(InstrumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Instrument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstrumentProperty =
            DependencyProperty.Register("Instrument", typeof(TradeInstrument), typeof(TradeChartControl), new PropertyMetadata(null, InstrumentChanged));

        private static void InstrumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (TradeChartControl)d;
            chart.viewModel.Instrument = chart.Instrument;
        }

        private ChartViewModel viewModel;
        public ChartViewModel ViewModel => viewModel;
        public TradeChartControl()
        {
            InitializeComponent();

            viewModel = new ChartViewModel();
            foreach (var child in grid.ChildrenOfType<ChartControlBase>())
            {
                child.SetViewModel(viewModel);
            }
            viewModel.Theme = Persister<TradeTheme>.Instance.Items.First();
            this.DataContext = viewModel;
        }
    }
}
