using System.Globalization;
using System.Threading;
using System.Windows;
using Telerik.Windows.Controls;

namespace ZoomIn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VisualStudio2019Palette.LoadPreset(VisualStudio2019Palette.ColorVariation.Dark);
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
