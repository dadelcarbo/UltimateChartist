using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
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

        private void Indicator_ParameterChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Name = Indicator.DisplayName;
            RaisePropertyChanged(nameof(Name));
        }

        public string Name { get; set; }
        public ObservableCollection<IndicatorTreeItemViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeItemViewModel>();
        public IIndicator Indicator { get; private set; }

        public IndicatorSettings IndicatorSettings { get; private set; }
    }
}
