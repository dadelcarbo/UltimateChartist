using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZoomIn.ChartControls
{
    public abstract class ChartControlBase : UserControl
    {
        protected FontFamily labelFontFamily = new FontFamily("Calibri");

        protected ChartControlViewModel viewModel;
        public ChartControlViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                if (viewModel != value)
                {
                    if (viewModel != null)
                    {
                        viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    }
                    viewModel = value;
                    if (viewModel != null)
                    {
                        viewModel.PropertyChanged += ViewModel_PropertyChanged;
                    }
                    OnStockSerieChanged();
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.OnResize();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Range":
                    this.OnResize();
                    return;
                case "Serie":
                    this.OnStockSerieChanged();
                    return;
                default:
                    this.OnViewModelPropertyChanged(sender, e);
                    break;
            }
        }

        protected abstract void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e);
        protected abstract void OnResize();
        protected abstract void OnStockSerieChanged();
    }
}
