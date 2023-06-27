using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;
using UltimateChartist.Indicators;
using UltimateChartist.Indicators.Display;

namespace UltimateChartist.UserControls.ChartControls.Indicators;

/// <summary>
/// Interaction logic for IndicatorConfigUserControl.xaml
/// </summary>
public partial class IndicatorConfigUserControl : UserControl
{
    public IndicatorConfigUserControl(IIndicator indicator)
    {
        InitializeComponent();

        this.DataContext = indicator;

        // Add Parameters 
        foreach (PropertyInfo prop in indicator.GetType().GetProperties())
        {
            var attribute = prop.GetCustomAttributes(typeof(IndicatorParameterAttribute), true).FirstOrDefault() as IndicatorParameterAttribute;
            if (attribute == null)
                continue;
            switch (attribute.Type.Name)
            {
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

    private void CreateBoolParameter(IIndicatorParameterViewModel parameter)
    {
        var label = new System.Windows.Controls.Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
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
        var label = new System.Windows.Controls.Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
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
        var label = new System.Windows.Controls.Label() { Content = parameter.Parameter.Name, Width = 90, Margin = new Thickness(2) };
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
