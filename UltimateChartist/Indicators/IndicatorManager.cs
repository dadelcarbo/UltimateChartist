using System.Collections.Generic;
using System;

namespace UltimateChartist.Indicators;

public static class IndicatorManager
{
    static private List<IndicatorBase> indicatorList = null;
    static private List<IndicatorBase> GetIndicatorList()
    {
        indicatorList = new List<IndicatorBase>();
        var assembly = typeof(IndicatorManager).Assembly;
        foreach (Type t in assembly.GetTypes())
        {
            Type st = t.GetInterface("IIndicator");
            if (st != null)
            {
                if (!(t.Name.EndsWith("Base") || t.Name.Contains("StockTrail")))
                {
                    var instance = assembly.CreateInstance(t.FullName);
                    indicatorList.Add(instance as IndicatorBase);
                }
            }
        }
        return indicatorList;
    }
    static public List<IndicatorBase> Indicators => indicatorList ??= GetIndicatorList();

    //static public IIndicator CreateIndicator(string fullName)
    //{
    //    using (MethodLogger ml = new MethodLogger(typeof(IndicatorManager)))
    //    {
    //        IndicatorBase indicator = null;
    //        if (indicatorList == null)
    //        {
    //            GetIndicatorList();
    //        }

    //        try
    //        {
    //            int paramStartIndex = fullName.IndexOf('(') + 1;
    //            string name = fullName;
    //            int paramLength = 0;
    //            if (paramStartIndex != 0) // Else we are creating an empty indicator for the dianlog window
    //            {
    //                paramLength = fullName.LastIndexOf(')') - paramStartIndex;
    //                name = fullName.Substring(0, paramStartIndex - 1);
    //            }

    //            if (indicatorList.Contains(name))
    //            {
    //                IndicatorManager sm = new IndicatorManager();
    //                indicator = (IndicatorBase)sm.GetType().Assembly.CreateInstance("UltimateChartist.Indicators.StockIndicator_" + name);
    //                if (indicator != null)
    //                {
    //                    if (paramLength > 0)
    //                    {
    //                        string parameters = fullName.Substring(paramStartIndex, paramLength);
    //                        indicator.Initialise(parameters.Split(','));
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                throw new StockAnalyzerException("Indicator " + name + " doesn't not exist ! ");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            //if (e is StockAnalyzerException) throw e;
    //            indicator = null;
    //            StockLog.Write(e);
    //        }
    //        return indicator;
    //    }
    //}

}