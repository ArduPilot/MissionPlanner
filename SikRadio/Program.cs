using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace SikRadio
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");
        private const string MANUFACTURER_PASSWORD = "--manufacturing";
        private const string ALLOW_DIFF_PROG = "--allowdiffprog";
        private static bool _Manufacturer = false;
        private static bool _AllowDiffProg = false;

        public static bool Manufacturer
        {
            get
            {
                return _Manufacturer;
            }
        }

        public static bool AllowDiffProg
        {
            get
            {
                return _AllowDiffProg;
            }
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (RFDLib.Text.Contains(arg, MANUFACTURER_PASSWORD))
                {
                    _Manufacturer = true;
                }
                if (RFDLib.Text.Contains(arg, ALLOW_DIFF_PROG))
                {
                    _AllowDiffProg = true;
                }
            }

            log.Info("App Start");

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());
        }
    }
}