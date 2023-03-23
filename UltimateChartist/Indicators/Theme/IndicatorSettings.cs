using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using UltimateChartist.Helpers;
using UltimateChartist.Indicators.Display;

namespace UltimateChartist.Indicators.Theme
{
    public class IndicatorSettings
    {
        public IndicatorSettings() { }
        public IndicatorSettings(IIndicator indicator)
        {
            this.TypeName = indicator.GetType().FullName;
            this.Parameters = new List<ParameterValue>();
            foreach (PropertyInfo prop in indicator.GetType().GetProperties())
            {
                var attribute = prop.GetCustomAttributes(typeof(IndicatorParameterAttribute), true).FirstOrDefault() as IndicatorParameterAttribute;
                if (attribute == null)
                    continue;
                this.Parameters.Add(new ParameterValue { Name = prop.Name, Value = prop.GetValue(indicator).ToString() });
            }
            this.DisplaySettings = new List<DisplaySettings>();
            foreach (PropertyInfo prop in indicator.Series.GetType().GetProperties())
            {
                Type st = prop.PropertyType.GetInterface("IDisplayItem");
                if (st == null)
                    continue;
                var displayItem = (IDisplayItem)prop.GetValue(indicator.Series);
                this.DisplaySettings.Add(new DisplaySettings { Name = prop.Name, Type = prop.PropertyType.Name, Json = displayItem.ToJson() });
            }
        }

        public IIndicator GetIndicator()
        {
            var indicator = typeof(IIndicator).Assembly.CreateInstance(this.TypeName) as IIndicator;
            var indicatorType = typeof(IIndicator).Assembly.GetType(this.TypeName);
            foreach (var parameter in this.Parameters)
            {
                var prop = indicatorType.GetProperty(parameter.Name);
                switch (prop.PropertyType.Name)
                {
                    case "Int32":
                        prop.SetValue(indicator, int.Parse(parameter.Value));
                        break;
                    case "Double":
                        prop.SetValue(indicator, double.Parse(parameter.Value));
                        break;
                    case "Decimal":
                        prop.SetValue(indicator, decimal.Parse(parameter.Value));
                        break;
                    case "Boolean":
                        prop.SetValue(indicator, bool.Parse(parameter.Value));
                        break;
                    default:
                        throw new NotImplementedException($"Parameters of type:{prop.PropertyType.Name} not supported");
                };
            }
            foreach (var settings in this.DisplaySettings)
            {
                try
                {
                    var seriesType = indicator.Series.GetType();
                    var prop = seriesType.GetProperty(settings.Name);
                    var displayItem = prop.GetValue(indicator.Series) as IDisplayItem;
                    displayItem.FromJson(settings.Json);
                }
                catch (Exception ex)
                {
                    StockLog.Write(ex);
                }
            }
            return indicator;
        }

        public string TypeName { get; set; }
        public List<ParameterValue> Parameters { get; set; }
        public List<DisplaySettings> DisplaySettings { get; set; }
    }
}
