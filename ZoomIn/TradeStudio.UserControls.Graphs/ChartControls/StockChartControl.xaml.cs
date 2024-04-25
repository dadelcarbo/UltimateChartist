﻿using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using TradeStudio.Data.DataProviders;
using TradeStudio.Data.Instruments;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    /// <summary>
    /// Interaction logic for StockChartControl.xaml
    /// </summary>
    public partial class StockChartControl : UserControl
    {
        public BarDuration Duration
        {
            get { return (BarDuration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(BarDuration), typeof(StockChartControl), new PropertyMetadata(BarDuration.Daily, DurationChanged));

        private static void DurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (StockChartControl)d;
            chart.viewModel.Duration = chart.Duration;
        }

        public TradeInstrument Instrument
        {
            get { return (TradeInstrument)GetValue(InstrumentProperty); }
            set { SetValue(InstrumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Instrument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstrumentProperty =
            DependencyProperty.Register("Instrument", typeof(TradeInstrument), typeof(StockChartControl), new PropertyMetadata(null, InstrumentChanged));

        private static void InstrumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (StockChartControl)d;
            chart.viewModel.Instrument = chart.Instrument;
        }

        private ChartControlViewModel viewModel;
        public ChartControlViewModel ViewModel => viewModel;
        public StockChartControl()
        {
            InitializeComponent();

            viewModel = new ChartControlViewModel();
            foreach (var child in grid.ChildrenOfType<ChartControlBase>())
            {
                child.ViewModel = viewModel;
            }
        }
    }
}