﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using UltimateChartist.Helpers;

namespace UltimateChartist.Indicators.Theme
{
    [DebuggerDisplay("Name={Name}")]
    public class StockTheme
    {
        public StockTheme()
        {
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
        public void Reload()
        {
            var path = Path.Combine(Folders.Theme, this.Name + ".thm");
            if (File.Exists(path))
            {
                var theme = JsonSerializer.Deserialize<StockTheme>(File.ReadAllText(path));
                this.IndicatorSettings = theme.IndicatorSettings;
                this.Indicators = theme.IndicatorSettings.Select(i => i.GetIndicator()).ToList();
            }
        }
        public void Delete()
        {
            var path = Path.Combine(Folders.Theme, this.Name + ".thm");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static StockTheme Load(string path)
        {
            try
            {
                var theme = JsonSerializer.Deserialize<StockTheme>(File.ReadAllText(path));
                theme.Indicators = theme.IndicatorSettings.Select(i => i.GetIndicator()).ToList();
                return theme;
            }
            catch (Exception ex)
            {
                StockLog.Write(ex);
            }
            return null;
        }
    }
}
