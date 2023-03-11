using System.Collections.ObjectModel;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorConfigViewModel
    {
        public IndicatorConfigViewModel()
        {
            this.Root = new ObservableCollection<IndicatorTreeViewModel>
            {
                new IndicatorTreeViewModel {
                    Name = "Price",
                    Items = new ObservableCollection<IndicatorTreeViewModel>
                    {
                         new IndicatorTreeViewModel {Name= "Item1"},
                         new IndicatorTreeViewModel {Name= "Item2"},
                    }
                },
                new IndicatorTreeViewModel {
                    Name = "Indicator1",
                    Items = new ObservableCollection<IndicatorTreeViewModel>
                    {
                         new IndicatorTreeViewModel {Name= "Item3"},
                         new IndicatorTreeViewModel {Name= "Item4"},
                    }
                },
                new IndicatorTreeViewModel {
                    Name = "Indicator2",
                    Items = new ObservableCollection<IndicatorTreeViewModel>
                    {
                         new IndicatorTreeViewModel {Name= "Item5"},
                         new IndicatorTreeViewModel {Name= "Item6"},
                         new IndicatorTreeViewModel {Name= "Item7"},
                    }
                }
            };
        }
        public ObservableCollection<IndicatorTreeViewModel> Root { get; set; }
    }
}
