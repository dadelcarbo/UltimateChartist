using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using UltimateChartist.Helpers;
using UltimateChartist.Indicators.Display;

namespace UltimateChartist.Indicators.Theme
{
    public class StockTheme
    {
        public StockTheme()
        {
            this.Name = "Test";
            this.Indicators.Add(new StockIndicator_EMA());
            this.Indicators.Add(new StockIndicator_TrailATR());
        }

        public string Name { get; set; }
        public IEnumerable<IndicatorSettings> IndicatorSettings { get; set; } = new List<IndicatorSettings>();

        [JsonIgnore]
        public List<IIndicator> Indicators { get; set; } = new List<IIndicator>();

        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        public void Save()
        {
            IndicatorSettings = this.Indicators.Select(i => new IndicatorSettings(i));
            var path = Path.Combine(Folders.Theme, this.Name + ".thm");
            var themeString = JsonSerializer.Serialize(this, jsonOptions);
            File.WriteAllText(path, themeString);
        }
        public static StockTheme Load(string path)
        {
            try
            {
                return JsonSerializer.Deserialize<StockTheme>(File.ReadAllText(path));
            }
            catch(Exception ex)
            {
                StockLog.Write(ex);
            }
            return null;
        }
    }
}
