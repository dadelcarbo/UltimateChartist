using System;
using System.Windows;
using System.Windows.Data;

namespace UltimateChartist.UserControls.Converters;

public class ThicknessToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return ((Thickness)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        try
        {
            var fields = value.ToString().Split(',');
            return new Thickness(Double.Parse(fields[0]), Double.Parse(fields[1]), Double.Parse(fields[2]), Double.Parse(fields[3]));
        }
        catch
        {
            MessageBox.Show("Please, format the input value like this: \"number,number,number,number\".");
            return new Thickness(0);
        }
    }
}
