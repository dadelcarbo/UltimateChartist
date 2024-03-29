﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UltimateChartistLib;

public enum BarDuration
{
    M_1,
    M_2,
    M_5,
    M_15,
    M_30,
    H_1,
    H_2,
    H_4,
    Daily,
    Weekly,
    Monthly
}

[DebuggerDisplay("Date={Date}")]
public class StockBar
{
    public DateTime Date { get; private set; }
    public double Open { get; private set; }
    public double High { get; private set; }
    public double Low { get; private set; }
    public double Close { get; private set; }
    public double Volume { get; private set; }

    public double BodyHigh => Math.Max(Open, Close);
    public double BodyLow => Math.Min(Open, Close);

    public bool IsComplete { get; set; } = true;

    public StockBar(DateTime date, double open, double high, double low, double close, double volume)
    {
        Date = date;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    public static List<StockBar> GenerateWeeklyBarsFomDaily(List<StockBar> dailyBars)
    {
        if (dailyBars == null)
            return null;
        var newBars = new List<StockBar>();
        StockBar newBar = null;
        DayOfWeek previousDayOfWeek = DayOfWeek.Sunday;

        foreach (var bar in dailyBars)
        {
            if (newBar == null)
            {
                newBar = new StockBar(bar.Date, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume);
                previousDayOfWeek = bar.Date.DayOfWeek;
                newBar.IsComplete = false;
            }
            else
            {
                if (previousDayOfWeek < bar.Date.DayOfWeek)
                {
                    // We are in the week
                    newBar.High = Math.Max(newBar.High, bar.High);
                    newBar.Low = Math.Min(newBar.Low, bar.Low);
                    newBar.Close = bar.Close;
                    newBar.Volume += bar.Volume;
                    previousDayOfWeek = bar.Date.DayOfWeek;
                }
                else
                {
                    // We switched to next week
                    newBar.IsComplete = true;
                    newBars.Add(newBar);
                    newBar = new StockBar(bar.Date, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume);
                    previousDayOfWeek = bar.Date.DayOfWeek;
                    newBar.IsComplete = false;
                }
            }
        }
        if (newBar != null)
        {
            var lastDailyValue = dailyBars.Last();
            if (lastDailyValue.Date.DayOfWeek == DayOfWeek.Friday)
                newBar.IsComplete = lastDailyValue.IsComplete;
            newBars.Add(newBar);
        }
        return newBars;
    }
    public static List<StockBar> GenerateMonthlyBarsFomDaily(List<StockBar> dailyBars)
    {
        if (dailyBars == null)
            return null;

        var newBars = new List<StockBar>();
        StockBar newValue = null;

        foreach (StockBar bar in dailyBars)
        {
            if (newValue == null)
            {
                newValue = new StockBar(bar.Date, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume);
                newValue.IsComplete = false;
            }
            else
            {
                if (newValue.Date.Month == bar.Date.Month)
                {
                    // We are in the month
                    newValue.High = Math.Max(newValue.High, bar.High);
                    newValue.Low = Math.Min(newValue.Low, bar.Low);
                    newValue.Volume += bar.Volume;
                    newValue.Close = bar.Close;
                }
                else
                {
                    // We switched to next month
                    newValue.IsComplete = true;
                    newBars.Add(newValue);
                    newValue = new StockBar(bar.Date, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume);
                    newValue.IsComplete = false;
                }
            }
        }
        if (newValue != null)
        {
            // Check if bar complete
            var lastDailyValue = dailyBars.Last().Date;
            if (lastDailyValue.DayOfWeek == DayOfWeek.Friday && lastDailyValue.AddDays(3).Month != newValue.Date.Month)
                newValue.IsComplete = true;
            newBars.Add(newValue);
        }
        return newBars;
    }

    #region CSV FILE IO

    const char SEPARATOR = ';';
    const string Header = "Date;Open;High;Low;Close;Volume";

    public static StockBar Parse(string csvLine, DateTime? startDate = null)
    {
        try
        {
            var row = csvLine.Split(SEPARATOR);
            DateTime date = DateTime.Parse(row[0]);
            if (startDate != null && date < startDate)
                return null;
            return new StockBar(date,
               double.Parse(row[1]),
               double.Parse(row[2]),
               double.Parse(row[3]),
               double.Parse(row[4]),
               long.Parse(row[5]));
        }
        catch (Exception) { return null; }
    }

    /// <summary>
    /// Load a csv file containg bar data. First line is a header
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public static List<StockBar> Load(string fileName, DateTime? startDate = null)
    {
        if (File.Exists(fileName))
        {
            List<StockBar> bars = new List<StockBar>();
            foreach (var line in File.ReadAllLines(fileName).Skip(1))
            {
                var readValue = StockBar.Parse(line, startDate);
                if (readValue != null)
                {
                    bars.Add(readValue);
                }
            }
            return bars;
        }
        else
        {
            return null;
        }
    }

    public static void SaveCsv(IEnumerable<StockBar> bars, string fileName, DateTime? startDate = null, DateTime? endDate = null)
    {
        var selectedBars = bars.AsEnumerable();
        if (startDate != null)
        {
            selectedBars = selectedBars.Where(b => b.Date >= startDate);
        }
        if (endDate != null)
        {
            selectedBars = selectedBars.Where(b => b.Date < endDate);
        }
        if (selectedBars.Any())
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(StockBar.Header);
                sw.Write(selectedBars.Select(b => b.ToCsvString()).Aggregate((i, j) => i + Environment.NewLine + j));
            }
        }
        else
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }

    public string ToCsvString()
    {
        return $"{this.Date}{SEPARATOR}{this.Open}{SEPARATOR}{this.High}{SEPARATOR}{this.Low}{SEPARATOR}{this.Close}{SEPARATOR}{this.Volume}";
    }

    #endregion

}
