using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using UltimateChartist.Indicators;
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
                    Name = "Price Chart",
                    Items = priceTreeViewModel
                }
            };
            int count = 1;
            if (theme != null)
            {
                foreach (var indicator in theme.Indicators)
                {
                    switch (indicator.DisplayType)
                    {
                        case DisplayType.Price:
                        case DisplayType.TrailStop:
                            priceTreeViewModel.Add(new IndicatorTreeViewModel
                            {
                                Name = indicator.DisplayName,
                                Indicator = indicator
                            });
                            break;
                        case DisplayType.Ranged:
                        case DisplayType.NonRanged:
                            var indicatorTreeViewModel = new IndicatorTreeViewModel
                            {
                                Name = $"Indicator{count++} Graph"
                            };
                            indicatorTreeViewModel.Items.Add(new IndicatorTreeViewModel
                            {
                                Name = indicator.DisplayName,
                                Indicator = indicator
                            });
                            root.Add(indicatorTreeViewModel);
                            break;
                        case DisplayType.Volume:
                            throw new NotImplementedException();
                            break;
                    }
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

        private DelegateCommand dropDownClosedCommand;
        public ICommand DropDownClosedCommand => dropDownClosedCommand ??= new DelegateCommand(DropDownClosed);

        private void DropDownClosed(object commandParameter)
        {
            if (NewIndicator != null)
            {
                this.ChartViewModel.Theme.Indicators.Add(NewIndicator);
                this.SetTheme(this.ChartViewModel.Theme);
                this.ChartViewModel.AddIndicator(NewIndicator);
                this.SelectedItem = this.Root.SelectMany(i => i.Items).FirstOrDefault(i => i.Indicator == NewIndicator);
            }
        }

        private DelegateCommand dropDownOpeningCommand;
        public ICommand DropDownOpeningCommand => dropDownOpeningCommand ??= new DelegateCommand(DropDownOpening);

        private void DropDownOpening(object commandParameter)
        {
            this.NewIndicator = null;
        }

        public IIndicator NewIndicator { get; set; }

        private DelegateCommand deleteIndicatorCommand;
        public ICommand DeleteIndicatorCommand => deleteIndicatorCommand ??= new DelegateCommand(DeleteIndicator);

        private void DeleteIndicator(object commandParameter)
        {
            if (this.SelectedItem?.Indicator != null)
            {
                this.ChartViewModel.Theme.Indicators.Remove(this.SelectedItem.Indicator);
                this.ChartViewModel.RemoveIndicator(this.SelectedItem.Indicator);
                this.SetTheme(this.ChartViewModel.Theme);
                this.SelectedItem = this.Root.SelectMany(i => i.Items).FirstOrDefault();
            }
        }

        #region THEME COMMANDS
        private DelegateCommand saveThemeCommand;
        public ICommand SaveThemeCommand => saveThemeCommand ??= new DelegateCommand(SaveTheme);

        private void SaveTheme(object commandParameter)
        {
            if (this.ChartViewModel.Theme.Name.StartsWith("New"))
            {
                RadWindow.Prompt(new DialogParameters()
                {
                    Content = "Enter new theme name",
                    Closed = this.OnSavePromptClosed,
                    DialogStartupLocation = WindowStartupLocation.CenterOwner
                });
            }
            else
            {
                this.ChartViewModel.Theme.Save();
            }
        }
        private void OnSavePromptClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (string.IsNullOrEmpty(e.PromptResult) || e.PromptResult.StartsWith("New"))
                {
                    RadWindow.Alert(new DialogParameters()
                    {
                        Content = "Please enter a valid theme name",
                        DialogStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    return;
                }
                var existingTheme = this.Themes.FirstOrDefault(t=>t.Name == e.PromptResult);
                if (existingTheme != null && existingTheme == this.ChartViewModel.Theme)
                {
                    RadWindow.Alert(new DialogParameters()
                    {
                        Content = "A theme with the same name already exists",
                        DialogStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    return;
                }
                this.ChartViewModel.Theme.Name = e.PromptResult;
                this.ChartViewModel.Theme.Save();
                RaisePropertyChanged("Themes");
            }
        }

        private DelegateCommand newThemeCommand;
        public ICommand NewThemeCommand => newThemeCommand ??= new DelegateCommand(NewTheme);

        private void NewTheme(object commandParameter)
        {
            int count = 1;
            string name = "New";
            while (this.Themes.Any(t => t.Name == name))
            {
                name = $"New ({count++})";
            }
            var newTheme = new StockTheme() { Name = name };
            this.Themes.Add(newTheme);
            this.ChartViewModel.Theme = newTheme;
        }

        private DelegateCommand deleteThemeCommand;
        public ICommand DeleteThemeCommand => deleteThemeCommand ??= new DelegateCommand(DeleteTheme);

        private void DeleteTheme(object commandParameter)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Content = "Are you sure you want to delete theme ?",
                Closed = this.OnConfirmDeleteClosed,
                DialogStartupLocation = WindowStartupLocation.CenterOwner
            });
        }
        private void OnConfirmDeleteClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                var oldTheme = this.ChartViewModel.Theme;
                oldTheme.Delete();
                this.ChartViewModel.Theme = this.Themes.FirstOrDefault(t => t.Name != oldTheme.Name);
                this.Themes.Remove(oldTheme);
            }
        }
        #endregion
    }
}
