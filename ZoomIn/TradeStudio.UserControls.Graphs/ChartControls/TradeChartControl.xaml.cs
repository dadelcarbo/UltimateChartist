using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Theme;
using TradeStudio.Data.Instruments;
using Label = Telerik.Windows.Controls.Label;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    /// <summary>
    /// Interaction logic for TradeChartControl.xaml
    /// </summary>
    public partial class TradeChartControl : UserControl
    {
        #region BarDuration
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
        #endregion

        #region BarType
        public BarType BarType
        {
            get { return (BarType)GetValue(BarTypeProperty); }
            set { SetValue(BarTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarTypeProperty =
            DependencyProperty.Register("BarType", typeof(BarType), typeof(TradeChartControl), new PropertyMetadata(BarType.Candle, BarTypeChanged));

        private static void BarTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (TradeChartControl)d;
            chart.viewModel.BarType = chart.BarType;
        }
        #endregion

        #region Instrument

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
        #endregion

        public event EventHandler<Bar> CurrentBarChanged;

        Bar previousBar = null;
        protected virtual void OnCurrentBarChanged(Bar selectedBar)
        {
            if (selectedBar != previousBar)
            {
                CurrentBarChanged?.Invoke(this, selectedBar);
            }
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
                child.DataContext = viewModel;
            }
            viewModel.Theme = Persister<TradeTheme>.Instance.Items.First();
            this.DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.Indicators.CollectionChanged += Indicators_CollectionChanged;
        }

        private void Indicators_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var indicatorChartViewModel = (IndicatorChartViewModel)e.NewItems?[0];
            var border = new Border { BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Margin = new Thickness(40, 1, 20, 2) };
            var indicatorControl = new IndicatorControl()
            {
                Height = 100,
                DataContext = indicatorChartViewModel,
                ViewModel = viewModel,
                IndicatorChartViewModel = indicatorChartViewModel
            };
            border.Child = indicatorControl;
            this.indicatorsPanel.Children.Add(border);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MouseIndex":
                    Bar newBar = null;
                    if (viewModel.DataSerie?.Bars != null && viewModel.MouseIndex > -1 && viewModel.MouseIndex < viewModel.DataSerie.Bars.Count)
                    {
                        newBar = viewModel.DataSerie.Bars[viewModel.MouseIndex];
                    }
                    this.OnCurrentBarChanged(newBar);
                    break;
                default:
                    break;
            }
        }

        private void Grid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this.overviewControl.overviewSlider_MouseWheel(sender, e);
        }
    }
}
