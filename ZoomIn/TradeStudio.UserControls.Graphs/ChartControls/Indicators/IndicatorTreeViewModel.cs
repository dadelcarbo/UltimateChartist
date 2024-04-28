using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators
{
    public class IndicatorTreeViewModel : ViewModelBase, IDisposable
    {
        public IndicatorTreeViewModel(string name)
        {
            Name = name;
        }
        public IndicatorTreeViewModel(IIndicator indicator)
        {
            Name = indicator.DisplayName;
            Indicator = indicator;
            indicator.ParameterChanged += Indicator_ParameterChanged;
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
        public ObservableCollection<IndicatorTreeViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeViewModel>();
        public IIndicator Indicator { get; private set; }
    }
}
