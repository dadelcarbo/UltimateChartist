﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using TradeStudio.UserControls.Graphs.ChartControls.Shapes;

namespace TradeStudio.UserControls.Graphs.ChartControls
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
            this.overviewSlider.Selection = new SelectionRange<double>(viewModel.ZoomRange.Start, viewModel.ZoomRange.End);

            this.overviewGraph.Children.Clear();

            var closeSerie = viewModel?.DataSerie?.CloseValues;
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
            this.viewModel.ZoomRange = new SelectionRange<int>((int)Math.Floor(overviewSlider.Selection.Start), (int)Math.Ceiling(overviewSlider.Selection.End));
        }

        protected override void OnResize()
        {
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public void overviewSlider_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var dir = Math.Sign(e.Delta) * this.viewModel.MaxIndex / 20;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                this.overviewSlider.Selection = new SelectionRange<double>(Math.Max(0, this.viewModel.ZoomRange.Start + dir), Math.Min(this.viewModel.ZoomRange.End + dir, this.ViewModel.MaxIndex));
            }
            else
            {
                this.overviewSlider.Selection = new SelectionRange<double>(Math.Max(0, this.viewModel.ZoomRange.Start + dir), this.viewModel.ZoomRange.End);
            }
        }
    }
}
