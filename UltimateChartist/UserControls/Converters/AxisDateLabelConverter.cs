﻿using System;
using System.Windows.Data;
using UltimateChartist.Helpers;

namespace UltimateChartist.UserControls.Converters;
public class AxisDateLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        try
        {
            var date = (DateTime)value;
            if (parameter.ToString() == "Daily")
            {
                return date.ToString("dd/MM") + Environment.NewLine + date.ToString("yyyy");
            }
            else
            {
                return date.ToString("G");
            }
        }
        catch (Exception ex)
        {
            StockLog.Write(ex);
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
