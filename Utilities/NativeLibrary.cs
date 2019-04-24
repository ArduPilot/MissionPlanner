using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public static class NativeLibrary
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public static string GetLibraryPathname(string filename)
        {
            // If 64-bit process, load 64-bit DLL
            bool is64bit = System.Environment.Is64BitProcess;

            string prefix = "x86";

            if (is64bit)
            {
                prefix = "x64";
            }

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var lib1 = dir + Path.DirectorySeparatorChar + prefix + Path.DirectorySeparatorChar + filename;

            return lib1;
        }
    }
}
