namespace UltimateChartist.Indicators.Display
{
    internal interface IDisplayItem
    {
        string Name { get; set; }
        string ToJson();
        void FromJson(string json);
    }
}