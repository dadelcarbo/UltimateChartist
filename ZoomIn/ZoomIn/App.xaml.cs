using System.Globalization;
using System.Threading;
using System.Windows;

namespace ZoomIn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("en-UK");
            culture.NumberFormat.CurrencySymbol = "€";
            culture.DateTimeFormat = new CultureInfo("fr-FR").DateTimeFormat;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            base.OnStartup(e);
        }
    }
}
