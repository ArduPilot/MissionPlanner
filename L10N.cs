using log4net;
using MissionPlanner.Utilities;
using System.Globalization;
using System.Threading;

namespace MissionPlanner
{
    public class L10N
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static CultureInfo ConfigLang;

        static L10N()
        {
            ConfigLang = GetConfigLang();
            Strings.Culture = ConfigLang;
            Controls.HUDT.Culture = ConfigLang;

            // Set CurrentUICulture so L10NU loads the correct language strings
            if (ConfigLang != null && !Thread.CurrentThread.CurrentUICulture.Equals(ConfigLang))
            {
                Thread.CurrentThread.CurrentUICulture = ConfigLang;
            }
        }

        public static CultureInfo GetConfigLang()
        {
            if (Settings.Instance["language"] == null)
                return CultureInfo.CurrentUICulture;
            else
                return CultureInfoEx.GetCultureInfo(Settings.Instance["language"]);
        }
    }
}