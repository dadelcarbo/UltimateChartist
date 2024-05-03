using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;
using TradeStudio.Data.Indicators;
using TradeStudio.Data.Indicators.Display;
using Label = Telerik.Windows.Controls.Label;

namespace TradeStudio.UserControls.Graphs.ChartControls.Indicators;

/// <summary>
/// Interaction logic for IndicatorConfigUserControl.xaml
/// </summary>
public partial class IndicatorConfigUserControl : UserControl
{


    public IIndicator Indicator
    {
        get { return (IIndicator)GetValue(IndicatorProperty); }
        set { SetValue(IndicatorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Indicator.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IndicatorProperty =
        DependencyProperty.Register("Indicator", typeof(IIndicator), typeof(IndicatorConfigUserControl), new PropertyMetadata(null, OnIndicatorChanged));

    private static void OnIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var indicatorConfigUserControl = (IndicatorConfigUserControl)d;
        indicatorConfigUserControl.SetIndicator(indicatorConfigUserControl.Indicator, true);
    }

    public IndicatorConfigUserControl()
    {
        InitializeComponent();
    }

    public void SetIndicator(IIndicator indicator, bool showDisplay = true)
    {
        this.DataContext = indicator;

        // Parameters control
        parameterPanel.Children.Clear();
        foreach (PropertyInfo prop in indicator.GetType().GetProperties())
        {
            var attribute = prop.GetCustomAttributes(typeof(IndicatorParameterAttribute), true).FirstOrDefault() as IndicatorParameterAttribute;
            if (attribute == null)
                continue;
            switch (attribute.Type.Name)
            {
                case "Double":
                    CreateDecimalParameter(new IndicatorParameterViewModel<double>(prop.Name)
                    {
                        Value = (double)prop.GetValue(indicator),
                        Parameter = attribute
                    });
                    break;
                case "Decimal":
                    CreateDecimalParameter(new IndicatorParameterViewModel<decimal>(prop.Name)
                    {
                        Value = (decimal)prop.GetValue(indicator),
                        Parameter = attribute
                    });
                    break;
                case "Int32":
                    CreateIntParameter(new IndicatorParameterViewModel<int>(prop.Name)
                    {
                        Value = (int)prop.GetValue(indicator),
                        Parameter = attribute
                    });
                    break;
                case "Boolean":
                    CreateBoolParameter(new IndicatorParameterViewModel<bool>(prop.Name)
                    {
                        Value = (bool)prop.GetValue(indicator),
                        Parameter = attribute
                    });
                    break;
                default:
                    throw new NotImplementedException($"Attribute type not implemented {attribute.Type.Name} in IndicatorViewModel");
            }
        }

        // Graphic Curves
        curvePanel.Children.Clear();
        if (showDisplay)
        {
            switch (indicator.Series.GetType().Name)
            {
                case "IndicatorLineSeries":
                    {
                        var curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = (indicator.Series as IndicatorLineSeries).Curve;
                        this.curvePanel.Children.Add(curveConfig);
                    }
                    break;
                case "IndicatorLineSignalSeries":
                    {
                        var curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = (indicator.Series as IndicatorLineSignalSeries).Curve;
                        this.curvePanel.Children.Add(curveConfig);

                        curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = (indicator.Series as IndicatorLineSignalSeries).Signal;
                        this.curvePanel.Children.Add(curveConfig);
                    }
                    break;
                case "IndicatorRangeSeries":
                    {
                        var rangeConfig = new RangeConfigUserControl();
                        rangeConfig.DataContext = (indicator.Series as IndicatorRangeSeries).Area;
                        this.curvePanel.Children.Add(rangeConfig);
                    }
                    break;
                case "IndicatorBandSeries":
                    {
                        var curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = (indicator.Series as IndicatorBandSeries).MidLine;
                        this.curvePanel.Children.Add(curveConfig);
                        var rangeConfig = new RangeConfigUserControl();
                        rangeConfig.DataContext = (indicator.Series as IndicatorBandSeries).Area;
                        this.curvePanel.Children.Add(rangeConfig);
                    }
                    break;
                case "IndicatorTrailSeries":
                    {
                        var trailSerie = indicator.Series as IndicatorTrailSeries;
                        var rangeConfig = new RangeConfigUserControl();
                        rangeConfig.DataContext = trailSerie.Long;
                        this.curvePanel.Children.Add(rangeConfig);

                        rangeConfig = new RangeConfigUserControl();
                        rangeConfig.DataContext = trailSerie.Short;
                        this.curvePanel.Children.Add(rangeConfig);

                        var curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = trailSerie.LongReentry;
                        this.curvePanel.Children.Add(curveConfig);

                        curveConfig = new CurveConfigUserControl();
                        curveConfig.DataContext = trailSerie.ShortReentry;
                        this.curvePanel.Children.Add(curveConfig);
                    }
                    break;
            }
        }
        else
        {
            this.curveTabItem.Visibility = Visibility.Collapsed;
        }
    }

    private void CreateBoolParameter(IIndicatorParameterViewModel parameter)
    {
        var label = new Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
        var upDown = new CheckBox() { VerticalAlignment = VerticalAlignment.Center };

        var binding = new Binding(parameter.PropertyName) { Mode = BindingMode.TwoWay };
        upDown.SetBinding(CheckBox.IsCheckedProperty, binding);

        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(upDown);
        this.parameterPanel.Children.Add(stackPanel);
    }

    private void CreateIntParameter(IIndicatorParameterViewModel parameter)
    {
        var intParameter = parameter.Parameter as IndicatorParameterIntAttribute;
        var label = new Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
        var upDown = new RadNumericUpDown()
        {
            Minimum = intParameter.Min,
            Maximum = intParameter.Max,
            Margin = new Thickness(2),
            NumberDecimalDigits = 0,
            Width = 80,
            Height = 22
        };

        var binding = new Binding(parameter.PropertyName) { Mode = BindingMode.TwoWay };
        upDown.SetBinding(RadNumericUpDown.ValueProperty, binding);

        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(upDown);
        this.parameterPanel.Children.Add(stackPanel);
    }
    private void CreateDecimalParameter(IIndicatorParameterViewModel parameter)
    {
        var doubleParameter = parameter.Parameter as IndicatorParameterDecimalAttribute;
        var label = new Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
        var upDown = new RadNumericUpDown()
        {
            Minimum = (double)doubleParameter.Min,
            Maximum = (double)doubleParameter.Max,
            SmallChange = (double)doubleParameter.Step,
            LargeChange = (double)doubleParameter.Step * 10,
            Margin = new Thickness(2),
            NumberDecimalDigits = -(int)Math.Round(Math.Log10((double)doubleParameter.Step)),
            Width = 80,
            Height = 22
        };

        var binding = new Binding(parameter.PropertyName) { Mode = BindingMode.TwoWay, StringFormat = doubleParameter.Format };
        upDown.SetBinding(RadNumericUpDown.ValueProperty, binding);

        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(upDown);
        this.parameterPanel.Children.Add(stackPanel);
    }
}
