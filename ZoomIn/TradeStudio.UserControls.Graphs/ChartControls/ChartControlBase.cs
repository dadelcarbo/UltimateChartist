using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TradeStudio.UserControls.Graphs.ChartControls
{
    public abstract class ChartControlBase : UserControl
    {
        protected FontFamily labelFontFamily = new FontFamily("Calibri");
        protected static Brush TextBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0XB0, 0xB0, 0xB0));

        protected ChartViewModel viewModel;
        public ChartViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                if (viewModel != value)
                {
                    if (viewModel != null)
                    {
                        viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                        throw new InvalidOperationException("Change of viewModel unexpected");
                    }
                    viewModel = value;
                    if (viewModel != null)
                    {
                        viewModel.PropertyChanged += ViewModel_PropertyChanged;
                    }
                    this.DataContext = viewModel;
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.OnResize();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ZoomRange":
                    this.OnResize();
                    return;
                case "PriceIndicators":
                    this.OnIndicatorsChanged();
                    break;
                case "DataSerie":
                    this.OnStockSerieChanged();
                    return;
                default:
                    this.OnViewModelPropertyChanged(sender, e);
                    break;
            }
        }

        protected virtual void OnIndicatorsChanged() { }

        protected abstract void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e);
        protected abstract void OnResize();
        protected abstract void OnStockSerieChanged();

        public virtual void SetViewModel(ChartViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }
    }
}
