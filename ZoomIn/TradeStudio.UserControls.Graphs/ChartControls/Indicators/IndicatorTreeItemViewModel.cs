using System;
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
            this.name = name;
        }
        public IndicatorTreeItemViewModel(IndicatorSettings indicatorSettings)
        {
            IndicatorSettings = indicatorSettings;
            Indicator = indicatorSettings.GetIndicator();
            this.name = Indicator.DisplayName;
            Indicator.ParameterChanged += Indicator_ParameterChanged;
        }


        protected new void Dispose()
        {
            Indicator.ParameterChanged -= Indicator_ParameterChanged;
            foreach (var item in Items)
            {
                item.Dispose();
            }
            base.Dispose();
        }

        private void Indicator_ParameterChanged(IIndicator indicator, ParameterValue parameterValue)
        {
            Name = indicator.DisplayName;
        }
        private string name;
        public string Name { get { return name; } set { if (name != value) { name = value; RaisePropertyChanged(); } } }

        public ObservableCollection<IndicatorTreeItemViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeItemViewModel>();
        public IIndicator Indicator { get; private set; }

        public IndicatorSettings IndicatorSettings { get; private set; }
    }
}
