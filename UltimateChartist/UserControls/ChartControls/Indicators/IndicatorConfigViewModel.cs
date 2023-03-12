using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Telerik.Windows.Controls;
using UltimateChartist.Helpers;
using UltimateChartist.Indicators.Theme;

namespace UltimateChartist.UserControls.ChartControls.Indicators
{
    public class IndicatorConfigViewModel
    {
        public IndicatorConfigViewModel()
        {
            this.Themes = new ObservableCollection<Theme>();
            this.Themes.AddRange(Directory.EnumerateFiles(Folders.Theme, "*.thm").Select(f => StockTheme.Load(f)).Where(t => t != null));
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

        public ObservableCollection<Theme> Themes { get; set; }
    }
}
