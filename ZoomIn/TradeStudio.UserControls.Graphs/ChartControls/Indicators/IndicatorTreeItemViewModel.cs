﻿using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Display;
using TradeStudio.Data.Indicators.Theme;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators
{
    public class IndicatorTreeItemViewModel : ViewModelBase, IDisposable
    {
        public IndicatorTreeItemViewModel(string name)
        {
            Name = name;
        }
        public IndicatorTreeItemViewModel(IndicatorSettings indicatorSettings)
        {
            IndicatorSettings = indicatorSettings;
            Indicator = indicatorSettings.GetIndicator();
            Name = Indicator.DisplayName;
            Indicator.ParameterChanged += Indicator_ParameterChanged;
            Indicator.DisplayChanged += Indicator_DisplayChanged; ;
        }


        protected new void Dispose()
        {
            Indicator.ParameterChanged -= Indicator_ParameterChanged;
            Indicator.DisplayChanged -= Indicator_DisplayChanged;
            foreach (var item in Items)
            {
                item.Dispose();
            }
            base.Dispose();
        }

        private void Indicator_ParameterChanged(IIndicator indicator, ParameterValue parameterValue)
        {
            Name = indicator.DisplayName;

            this.IndicatorSettings.UpdateParameter(parameterValue);
        }
        private void Indicator_DisplayChanged(IIndicator indicator, IDisplayItem displayItem)
        {
            this.IndicatorSettings.UpdateDisplay(displayItem);
        }

        public string Name { get; set; }
        public ObservableCollection<IndicatorTreeItemViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeItemViewModel>();
        public IIndicator Indicator { get; private set; }

        public IndicatorSettings IndicatorSettings { get; private set; }
    }
}
