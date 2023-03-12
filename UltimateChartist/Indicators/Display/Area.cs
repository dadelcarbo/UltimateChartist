using System;
using System.Windows.Media;
using Telerik.Windows.Controls;
using UltimateChartist.Helpers;

namespace UltimateChartist.Indicators.Display;

public class Area : ViewModelBase, IDisplayItem
{
    private string name;
    public string Name { get => name; set { if (name != value) { name = value; RaisePropertyChanged(); } } }

    private SolidColorBrush stroke;
    public SolidColorBrush Stroke { get => stroke; set { if (stroke != value) { stroke = value; RaisePropertyChanged(); } } }

    private double thickness;
    public double Thickness { get => thickness; set { if (thickness != value) { thickness = value; RaisePropertyChanged(); } } }

    private SolidColorBrush fill;
    public SolidColorBrush Fill { get => fill; set { if (fill != value) { fill = value; RaisePropertyChanged(); } } }

    public string ToJson()
    {
        return $"{Name}|{Stroke.Color}|{Thickness}|{Fill.Color}";
    }

    public void FromJson(string json)
    {
        try
        {
            var fields = json.Split('|');
            this.Name = fields[0];
            this.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fields[1]));
            this.Thickness = double.Parse(fields[2]);
            this.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fields[3]));
        }
        catch (Exception ex)
        {
            StockLog.Write(ex);
        }
    }
}
