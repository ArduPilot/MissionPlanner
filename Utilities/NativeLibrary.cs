using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

        [DllImport("libdl.so")]
        public static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so")]
        public static extern StringBuilder dlerror();

        [DllImport("libdl.so")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        public const int RTLD_NOW = 2; // for dlopen's flags 

        public static string GetLibraryPathname(string filename)
        {
            // If 64-bit process, load 64-bit DLL
            bool is64bit = System.Environment.Is64BitProcess;
            var arch = RuntimeInformation.OSArchitecture;

            // incase we are in 32bit mode on x64
            if (!is64bit && arch == Architecture.X64)
                arch = Architecture.X86;

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var lib1 = dir + Path.DirectorySeparatorChar + arch.ToString().ToLower() + Path.DirectorySeparatorChar +
                       filename;

            return lib1;
        }
    }
}
