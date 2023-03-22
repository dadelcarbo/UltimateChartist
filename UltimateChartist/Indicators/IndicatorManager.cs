using System.Collections.Generic;
using System;
using UltimateChartist.Indicators.Display;
using System.ComponentModel;
using UltimateChartist.DataModels;
using System.Security.Policy;
using System.Diagnostics;

namespace UltimateChartist.Indicators;

[DebuggerDisplay("Name={Name}")]
public class IndicatorDescriptor
{
    private IIndicator instance;

    public IndicatorDescriptor(IIndicator instance)
    {
        this.instance = instance;
    }

    public Type Type => instance.GetType();
    public string ShortName => instance.ShortName;

    public string DisplayName => instance.DisplayName;

    public string Description => instance.Description;

    public DisplayType DisplayType => instance.DisplayType;

    internal IIndicator CreateInstance()
    {
        return IndicatorManager.CreateIndicator(this);
    }
}
public static class IndicatorManager
{
    static private List<IndicatorDescriptor> indicatorList = null;
    static private List<IndicatorDescriptor> GetIndicatorList()
    {
        indicatorList = new List<IndicatorDescriptor>();
        var assembly = typeof(IndicatorManager).Assembly;
        foreach (Type t in assembly.GetTypes())
        {
            Type st = t.GetInterface("IIndicator");
            if (st != null)
            {
                if (!(t.Name.EndsWith("Base") || t.Name.Contains("StockTrail")))
                {
                    var instance = assembly.CreateInstance(t.FullName);
                    indicatorList.Add(new(instance as IIndicator));
                }
            }
        }
        return indicatorList;
    }
    static public List<IndicatorDescriptor> Indicators => indicatorList ??= GetIndicatorList();

    static public IIndicator CreateIndicator(IndicatorDescriptor indicatorDescriptor)
    {
        var assembly = typeof(IndicatorManager).Assembly;
        return (IIndicator)Activator.CreateInstance(indicatorDescriptor.Type);
    }

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