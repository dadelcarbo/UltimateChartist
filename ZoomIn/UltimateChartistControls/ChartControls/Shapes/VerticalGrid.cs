using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using UltimateChartistLib;

namespace UltimateChartistControls.ChartControls.Shapes
{
    public class Legend
    {
        public Point Location { get; set; }
        public string Text { get; set; }
    }
    public class VerticalGrid : StockShapeBase
    {
        static CultureInfo frCulture = new CultureInfo("fr-FR");
        public List<Legend> Legends { get; } = new List<Legend>();
        public void CreateGeometry(StockSerie serie, int startIndex, int endIndex, Size canvasSize)
        {
            if (!Enum.TryParse(serie.Name, out BarDuration duration))
                duration = BarDuration.Daily;

            switch (duration)
            {
                case BarDuration.M_1:
                    GenerateMinuteGrid(serie, startIndex, endIndex, canvasSize.Height, 15);
                    break;
                case BarDuration.M_2:
                    GenerateMinuteGrid(serie, startIndex, endIndex, canvasSize.Height, 30);
                    break;
                case BarDuration.M_5:
                    GenerateMinuteGrid(serie, startIndex, endIndex, canvasSize.Height, 60);
                    break;
                case BarDuration.M_15:
                    GenerateMinuteGrid(serie, startIndex, endIndex, canvasSize.Height, 240);
                    break;
                case BarDuration.M_30:
                    GenerateMinuteGrid(serie, startIndex, endIndex, canvasSize.Height, 480);
                    break;
                case BarDuration.H_1:
                    GenerateHourGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                case BarDuration.H_2:
                    GenerateHourGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                case BarDuration.H_4:
                    GenerateHourGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                case BarDuration.Daily:
                    GenerateDailyGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                case BarDuration.Weekly:
                    GenerateWeeklyGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                case BarDuration.Monthly:
                    GenerateMonthlyGrid(serie, startIndex, endIndex, canvasSize.Height);
                    break;
                default:
                    break;
            }

            RenderOptions.SetEdgeMode((DependencyObject)this, EdgeMode.Aliased);
        }


        private void GenerateMinuteGrid(StockSerie serie, int startIndex, int endIndex, double height, int barFrequency)
        {
            var geometryGroup = new GeometryGroup();

            var previousDay = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Day;
            int previousHour = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Hour;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                var minutes = (bar.Date - bar.Date.Date).TotalMinutes;
                if (minutes % barFrequency == 0)
                {
                    previousHour = bar.Date.Hour;
                    var line = new LineGeometry(new Point(i, 0), new Point(i, height));
                    geometryGroup.Children.Add(line);
                    if (bar.Date.Day != previousDay)
                    {
                        previousDay = bar.Date.Day;
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("HH:mm", frCulture) + Environment.NewLine + bar.Date.ToString("dd/MM") });
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("HH:mm", frCulture) });
                    }
                }
            }
            geometry = geometryGroup;
        }

        private void GenerateHourGrid(StockSerie serie, int startIndex, int endIndex, double height)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;

            var geometryGroup = new GeometryGroup();

            var previousDay = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.DayOfYear;
            int previousWeek = startIndex == 0 ? -1 : calendar.GetWeekOfYear(serie.Bars[startIndex].Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                var week = calendar.GetWeekOfYear(bar.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                if (week != previousWeek)
                {
                    previousWeek = week;
                    var line = new LineGeometry(new Point(i, 0), new Point(i, height));
                    geometryGroup.Children.Add(line);
                    if (bar.Date.Day != previousDay)
                    {
                        previousDay = bar.Date.Day;
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("HH:mm", frCulture) + Environment.NewLine + bar.Date.ToString("dd/MM") });
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("HH:mm", frCulture) });
                    }
                }
            }
            geometry = geometryGroup;
        }
        private void GenerateDailyGrid(StockSerie serie, int startIndex, int endIndex, double height)
        {
            var geometryGroup = new GeometryGroup();

            int previousMonth = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Month;
            int previousYear = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Year;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                if (bar.Date.Month != previousMonth)
                {
                    previousMonth = bar.Date.Month;
                    var line = new LineGeometry(new Point(i, 0), new Point(i, height));
                    geometryGroup.Children.Add(line);
                    if (previousYear != bar.Date.Year)
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd/MM") + Environment.NewLine + bar.Date.ToString("yyyy") });
                        previousYear = bar.Date.Year;
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd/MM") });
                    }
                }
            }
            geometry = geometryGroup;
        }

        private void GenerateWeeklyGrid(StockSerie serie, int startIndex, int endIndex, double height)
        {
            var geometryGroup = new GeometryGroup();

            int previousMonth = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Month;
            int previousYear = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Year;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];
                if (bar.Date.Month != previousMonth && (bar.Date.Month - 1) % 3 == 0) // Every other month
                {
                    previousMonth = bar.Date.Month;
                    var line = new LineGeometry(new Point(i, 0), new Point(i, height));
                    geometryGroup.Children.Add(line);
                    if (previousYear != bar.Date.Year)
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd MMM") + Environment.NewLine + bar.Date.ToString("yyyy") });
                        previousYear = bar.Date.Year;
                    }
                    else
                    {
                        this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd MMM") });
                    }
                }
            }
            geometry = geometryGroup;
        }

        private void GenerateMonthlyGrid(StockSerie serie, int startIndex, int endIndex, double height)
        {
            var geometryGroup = new GeometryGroup();

            int previousYear = startIndex == 0 ? -1 : serie.Bars[startIndex].Date.Year;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var bar = serie.Bars[i];

                if (previousYear != bar.Date.Year)
                {
                    var line = new LineGeometry(new Point(i, 0), new Point(i, height));
                    geometryGroup.Children.Add(line);
                    this.Legends.Add(new Legend { Location = new Point(i, 0), Text = bar.Date.ToString("dd MMM") + Environment.NewLine + bar.Date.ToString("yyyy") });
                    previousYear = bar.Date.Year;
                }
            }
            geometry = geometryGroup;
        }
    }
}
