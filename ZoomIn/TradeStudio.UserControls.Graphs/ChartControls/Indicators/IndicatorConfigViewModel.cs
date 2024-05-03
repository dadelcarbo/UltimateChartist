using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using TradeStudio.Common.Helpers;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Theme;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators
{
    public class IndicatorConfigViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<IndicatorTreeItemViewModel> root;
        private readonly IndicatorConfigWindow indicatorConfigWindow;

        public static IndicatorConfigViewModel Instance { get; private set; }

        public static IndicatorConfigViewModel GetInstance(ChartViewModel chartViewModel, IndicatorConfigWindow indicatorConfigWindow)
        {
            if (Instance != null)
            {
                Instance.Dispose();
            }
            Instance = new IndicatorConfigViewModel(chartViewModel, indicatorConfigWindow);
            return Instance;
        }

        protected new void Dispose()
        {
            ChartViewModel.PropertyChanged -= ChartViewModel_PropertyChanged;
            base.Dispose();
        }

        public ChartViewModel ChartViewModel { get; }
        private IndicatorConfigViewModel(ChartViewModel chartViewModel, IndicatorConfigWindow indicatorConfigWindow)
        {
            ChartViewModel = chartViewModel;
            this.indicatorConfigWindow = indicatorConfigWindow;
            SetTheme(ChartViewModel.Theme);

            ChartViewModel.PropertyChanged += ChartViewModel_PropertyChanged;
        }

        private void ChartViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Theme":
                    SetTheme(ChartViewModel.Theme);
                    break;
            }
        }

        private void SetTheme(TradeTheme theme)
        {
            if (Root != null)
            {
                foreach (var item in Root)
                {
                    item.Dispose();
                }
            }
            var root = new ObservableCollection<IndicatorTreeItemViewModel>
            {
                new IndicatorTreeItemViewModel("Price Chart")
            };
            var priceTreeViewModel = root.First().Items;
            int count = 1;
            if (theme != null)
            {
                foreach (var indicatorSettings in theme.IndicatorSettings)
                {
                    switch (indicatorSettings.DisplayTarget)
                    {
                        case DisplayTarget.Price:
                            priceTreeViewModel.Add(new IndicatorTreeItemViewModel(indicatorSettings));
                            break;
                        default:
                            var indicatorTreeViewModel = new IndicatorTreeItemViewModel($"Indicator{count++} Graph");
                            indicatorTreeViewModel.Items.Add(new IndicatorTreeItemViewModel(indicatorSettings));
                            root.Add(indicatorTreeViewModel);
                            break;
                    }
                }
            }
            Root = root;
        }

        public ObservableCollection<IndicatorTreeItemViewModel> Root { get => root; set { if (root != value) { root = value; RaisePropertyChanged(); } } }

        public ObservableCollection<TradeTheme> Themes => Persister<TradeTheme>.Instance.Items;

        private IndicatorTreeItemViewModel selectedItem;
        public IndicatorTreeItemViewModel SelectedItem
        {
            get => selectedItem; set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    indicatorConfigWindow.DisplayConfigItem(selectedItem);
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
                var indicator = NewIndicator.CreateInstance();
                var indicatorSettings = new IndicatorSettings(indicator);

                ChartViewModel.Theme.IndicatorSettings.Add(indicatorSettings);
                SetTheme(ChartViewModel.Theme);
                ChartViewModel.AddIndicator(indicatorSettings);
                SelectedItem = Root.SelectMany(i => i.Items).FirstOrDefault(i => i.IndicatorSettings == indicatorSettings);
                NewIndicator = null;
            }
        }

        private DelegateCommand dropDownOpeningCommand;
        public ICommand DropDownOpeningCommand => dropDownOpeningCommand ??= new DelegateCommand(DropDownOpening);

        private void DropDownOpening(object commandParameter)
        {
            NewIndicator = null;
        }

        private IndicatorDescriptor newIndicator;
        public IndicatorDescriptor NewIndicator { get => newIndicator; set { if (newIndicator != value) { newIndicator = value; RaisePropertyChanged(); } } }

        private DelegateCommand deleteIndicatorCommand;
        public ICommand DeleteIndicatorCommand => deleteIndicatorCommand ??= new DelegateCommand(DeleteIndicator);

        private void DeleteIndicator(object commandParameter)
        {
            if (SelectedItem?.Indicator != null)
            {
                ChartViewModel.Theme.IndicatorSettings.Remove(SelectedItem.IndicatorSettings);
                ChartViewModel.RemoveIndicator(SelectedItem.IndicatorSettings);
                SetTheme(ChartViewModel.Theme);
                SelectedItem = Root.SelectMany(i => i.Items).FirstOrDefault();
            }
        }

        internal void DeleteItem(IndicatorTreeItemViewModel treeItem)
        {
            if (treeItem.Indicator != null)
            {
                ChartViewModel.Theme.IndicatorSettings.Remove(treeItem.IndicatorSettings);
                ChartViewModel.RemoveIndicator(treeItem.IndicatorSettings);
            }
            else
            {
                foreach (var item in treeItem.Items)
                {
                    ChartViewModel.Theme.IndicatorSettings.Remove(item.IndicatorSettings);
                    ChartViewModel.RemoveIndicator(item.IndicatorSettings);
                }
            }
            SetTheme(ChartViewModel.Theme);
            var selectedItem = Root.SelectMany(i => i.Items).FirstOrDefault();
            SelectedItem = selectedItem != null ? selectedItem : Root.FirstOrDefault();
        }

        #region THEME COMMANDS

        private DelegateCommand reloadThemeCommand;
        public ICommand ReloadThemeCommand => reloadThemeCommand ??= new DelegateCommand(ReloadTheme);
        private void ReloadTheme(object commandParameter)
        {
            ChartViewModel.Theme.Reload();
            SetTheme(ChartViewModel.Theme);
        }

        private DelegateCommand saveThemeCommand;
        public ICommand SaveThemeCommand => saveThemeCommand ??= new DelegateCommand(SaveTheme);

        private void SaveTheme(object commandParameter)
        {
            if (ChartViewModel.Theme.Name.StartsWith("New"))
            {
                RadWindow.Prompt(new DialogParameters()
                {
                    Header = "Save Theme",
                    Content = "Enter new theme name",
                    Closed = OnSavePromptClosed,
                    DialogStartupLocation = WindowStartupLocation.CenterOwner
                });
            }
            else
            {
                ChartViewModel.Theme.Save();
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
                        Header = "Save Theme",
                        Content = "Please enter a valid theme name",
                        DialogStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    return;
                }
                var existingTheme = Themes.FirstOrDefault(t => t.Name == e.PromptResult);
                if (existingTheme != null && existingTheme == ChartViewModel.Theme)
                {
                    RadWindow.Alert(new DialogParameters()
                    {
                        Content = "A theme with the same name already exists",
                        DialogStartupLocation = WindowStartupLocation.CenterOwner
                    });
                    return;
                }


                ChartViewModel.Theme.Name = e.PromptResult;
                ChartViewModel.Theme.Save();

                ChartViewModel.Theme = null;
                ChartViewModel.Theme = Themes.FirstOrDefault(t => t.Name == e.PromptResult);

                RaisePropertyChanged(nameof(Themes));
            }
        }

        private DelegateCommand newThemeCommand;
        public ICommand NewThemeCommand => newThemeCommand ??= new DelegateCommand(NewTheme);

        private void NewTheme(object commandParameter)
        {
            int count = 1;
            string name = "New";
            while (Themes.Any(t => t.Name == name))
            {
                name = $"New ({count++})";
            }
            var newTheme = new TradeTheme() { Name = name };
            Themes.Add(newTheme);
            ChartViewModel.Theme = newTheme;
        }


        private DelegateCommand deleteThemeCommand;
        public ICommand DeleteThemeCommand => deleteThemeCommand ??= new DelegateCommand(DeleteTheme);

        private void DeleteTheme(object commandParameter)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Content = $"Are you sure you want to delete theme {ChartViewModel.Theme.Name}?",
                Closed = OnConfirmDeleteClosed,
                DialogStartupLocation = WindowStartupLocation.CenterOwner
            });
        }
        private void OnConfirmDeleteClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true && ChartViewModel.Theme != null)
            {
                var oldTheme = ChartViewModel.Theme;
                oldTheme.Delete();
                ChartViewModel.Theme = Themes.FirstOrDefault(t => t.Name != oldTheme.Name);
                Themes.Remove(oldTheme);
            }
        }
        #endregion
    }
}
