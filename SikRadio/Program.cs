/*
 * Release notes:
 * 
 * 2.4 - Terminal window now logs in user's documents folder instead of program files folder, to prevent
 *          crash caused by folder not being writable.  RFD900x net ID range now defaults to 16 bit for
 *          p2p firmware and 8 bit for async firmware, if range isn't specified by modem.
 * 2.5 - Added TCP port capability.
 * 2.6 - Can now generate a random encryption key.
 */

using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace SikRadio
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            log.Info("App Start");

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());
        }
    }
}