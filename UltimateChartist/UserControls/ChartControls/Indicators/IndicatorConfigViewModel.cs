using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Data.CardView;
using UltimateChartist.Helpers;
using UltimateChartist.Indicators.Theme;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorConfigViewModel : ViewModelBase
    {
        private ObservableCollection<IndicatorTreeViewModel> root;
        private readonly IndicatorConfigWindow indicatorConfigWindow;

        public ChartViewModel ChartViewModel { get; }
        public IndicatorConfigViewModel(ChartViewModel chartViewModel, IndicatorConfigWindow indicatorConfigWindow)
        {
            this.ChartViewModel = chartViewModel;
            this.indicatorConfigWindow = indicatorConfigWindow;
            SetTheme(ChartViewModel.Theme);

            this.ChartViewModel.PropertyChanged += ChartViewModel_PropertyChanged;
        }

        private void ChartViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Theme":
                    this.SetTheme(ChartViewModel.Theme);
                    break;
            }
        }

        private void SetTheme(StockTheme theme)
        {
            var priceTreeViewModel = new ObservableCollection<IndicatorTreeViewModel>
            {
            };
            var root = new ObservableCollection<IndicatorTreeViewModel>
            {
                new IndicatorTreeViewModel {
                    Name = "Price",
                    Items = priceTreeViewModel
                }
            };
            int count = 1;
            foreach (var indicator in theme.Indicators)
            {
                switch (indicator.DisplayType)
                {
                    case UltimateChartist.Indicators.DisplayType.Price:
                    case UltimateChartist.Indicators.DisplayType.TrailStop:
                        priceTreeViewModel.Add(new IndicatorTreeViewModel
                        {
                            Name = indicator.DisplayName,
                            Indicator = indicator
                        });
                        break;
                    case UltimateChartist.Indicators.DisplayType.Ranged:
                    case UltimateChartist.Indicators.DisplayType.NonRanged:
                        var indicatorTreeViewModel = new IndicatorTreeViewModel
                        {
                            Name = $"Indicator{count++}"
                        };
                        indicatorTreeViewModel.Items.Add(new IndicatorTreeViewModel
                        {
                            Name = indicator.DisplayName,
                            Indicator = indicator
                        });
                        root.Add(indicatorTreeViewModel);
                        break;
                }
            }
            this.Root = root;
        }

        public ObservableCollection<IndicatorTreeViewModel> Root { get => root; set { if (root != value) { root = value; RaisePropertyChanged(); } } }

        public ObservableCollection<StockTheme> Themes => MainWindowViewModel.Instance.Themes;


        private IndicatorTreeViewModel selectedItem;

        public IndicatorTreeViewModel SelectedItem
        {
            get => selectedItem; set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    this.indicatorConfigWindow.DisplayConfigItem(selectedItem);
                    RaisePropertyChanged();
                }
            }
        }
    }
}
