using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System
{
    public class Application11
    {
        public static string ExecutablePath
        {
            get { return System.Windows.Forms.Application.ExecutablePath; }
        }

        public static string StartupPath
        {
            get { return System.Windows.Forms.Application.StartupPath; }
        }

        public static void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
        }

        public static string ProductVersion
        {
            get { return System.Windows.Forms.Application.ProductVersion; }
        }

        public static void Exit()
        {
            System.Windows.Forms.Application.Exit();
        }

        public static void EnableVisualStyles()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
        }

        public static void SetCompatibleTextRenderingDefault(bool p)
        {
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(p);
        }

        public static void Run(Form mainV2)
        {
            System.Windows.Forms.Application.Run(mainV2);
        }
    }
}