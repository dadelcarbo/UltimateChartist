using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.FieldList;
using UltimateChartist.Indicators.Display;
using UltimateChartist.UserControls.ChartControls.Indicators;

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
                this.Parameters.Add(new ParameterValue { Name = prop.Name, Value = prop.GetValue(indicator) });
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
            return null;
        }

        public string TypeName { get; set; }
        public List<ParameterValue> Parameters { get; set; }
        public List<DisplaySettings> DisplaySettings { get; set; }
    }
}
