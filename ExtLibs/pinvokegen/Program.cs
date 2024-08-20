using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace pinvokegen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Windows.Win32.PInvoke.FindWindow("Notepad", null);
            Windows.Win32.PInvoke.GetConsoleWindow();
        }
    }
}
