using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using UltimateChartist.Indicators;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorTreeViewModel : ViewModelBase, IDisposable
    {
        public IndicatorTreeViewModel(string name)
        {
            this.Name = name;
        }
        public IndicatorTreeViewModel(IIndicator indicator)
        {
            this.Name = indicator.DisplayName;
            this.Indicator = indicator;
            indicator.ParameterChanged += Indicator_ParameterChanged;
        }

        protected new void Dispose()
        {
            this.Indicator.ParameterChanged -= Indicator_ParameterChanged;
            foreach (var item in Items)
            {
                item.Dispose();
            }
            base.Dispose();
        }


        private void Indicator_ParameterChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Name = this.Indicator.DisplayName;
            RaisePropertyChanged("Name");
        }

        public string Name { get; set; }
        public ObservableCollection<IndicatorTreeViewModel> Items { get; set; } = new ObservableCollection<IndicatorTreeViewModel>();
        public IIndicator Indicator { get; private set; }
    }
}
