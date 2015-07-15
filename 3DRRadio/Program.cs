using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace _3DRRadio
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            log.Info("App Start");

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());
        }
    }
}
