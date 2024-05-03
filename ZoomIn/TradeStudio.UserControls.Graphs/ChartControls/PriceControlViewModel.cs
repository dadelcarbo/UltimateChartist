using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Theme;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    public class PriceControlViewModel : ViewModelBase
    {
        public ObservableCollection<IIndicator> Indicators { get; set; } = new();


        private DelegateCommand deleteIndicatorCommand;
        public ICommand DeleteIndicatorCommand => deleteIndicatorCommand ??= new DelegateCommand(DeleteIndicator);

        private void DeleteIndicator(object commandParameter)
        {
            this.Indicators.Remove(commandParameter as IIndicator);
        }

    }
}
