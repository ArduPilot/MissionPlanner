using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MissionPlanner.Comms
{
    public class SerialTest
    {
        public static void Main(string[] args)
        {
            string portName = @"\\.\"+ args[0];
            IntPtr handle = CreateFile(portName, 0, 0, IntPtr.Zero, 3, 0x80, IntPtr.Zero);
            if (handle == (IntPtr)(-1))
            {
                Console.WriteLine("Could not open " + portName + ": " + new Win32Exception().Message);
                Console.ReadKey();
                return;
            }

            FileType type = GetFileType(handle);
            Console.WriteLine("File " + portName + " reports its type as: " + type);

            Console.ReadKey();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr SecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern FileType GetFileType(IntPtr hFile);

        enum FileType : uint
        {
            UNKNOWN = 0x0000,
            DISK = 0x0001,
            CHAR = 0x0002,
            PIPE = 0x0003,
            REMOTE = 0x8000,
        }
    }
}
