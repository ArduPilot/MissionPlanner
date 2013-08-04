using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace MissionPlanner.Comms
{

    public class SerialPort : System.IO.Ports.SerialPort, ICommsSerial
    {
        static bool serialportproblem = false;

        public new bool DtrEnable { get { return base.DtrEnable; } set { if (ispx4(base.PortName)) return; base.DtrEnable = value; } }
        public new bool RtsEnable { get { return base.RtsEnable; } set { if (ispx4(base.PortName)) return; base.RtsEnable = value; } }
        /*
        protected override void Dispose(bool disposing)
        {
            try
            {
                try
                {
                    Type mytype = typeof(System.IO.Ports.SerialPort);
                    FieldInfo field = mytype.GetField("internalSerialStream", BindingFlags.Instance | BindingFlags.NonPublic);
                    Stream stream = (Stream)field.GetValue(this);

                    if (stream != null)
                    {
                        try
                        {
                            stream.Dispose();
                        }
                        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                        stream = null;
                    }
                }
                catch { }

                base.Dispose(disposing);
            }
            catch { }
        }*/

        public new void Open()
        {
            // 500ms write timeout - win32 api default
            this.WriteTimeout = 500;

            if (base.IsOpen)
                return;

            try
            {
              //  Console.WriteLine("Doing SerialPortFixer");
              //  SerialPortFixer.Execute(this.PortName);
              //  Console.WriteLine("Done SerialPortFixer");
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            base.Open();
        }

        public void toggleDTR()
        {
            if (ispx4(this.PortName))
            {
                Console.WriteLine("PX4 - no DTR");
                return;
            }


            bool open = this.IsOpen;
            Console.WriteLine("toggleDTR " + this.IsOpen);
            try
            {
                if (!open)
                    this.Open();
            }
            catch { }


            base.DtrEnable = false;
            base.RtsEnable = false;

            System.Threading.Thread.Sleep(50);

            base.DtrEnable = true;
            base.RtsEnable = true;

            System.Threading.Thread.Sleep(50);

            try
            {
                if (!open)
                    this.Close();
            }
            catch { }
            Console.WriteLine("toggleDTR done " + this.IsOpen);
        }

        public new static string[] GetPortNames()
        {
            List<string> allPorts = new List<string>();

            if (Directory.Exists("/dev/"))
            {
                if (Directory.Exists("/dev/serial/by-id/"))
                    allPorts.AddRange(Directory.GetFiles("/dev/serial/by-id/", "*"));
                allPorts.AddRange(Directory.GetFiles("/dev/", "ttyACM*"));
                allPorts.AddRange(Directory.GetFiles("/dev/", "ttyUSB*"));
                allPorts.AddRange(Directory.GetFiles("/dev/", "rfcomm*"));
            }

            string[] ports = System.IO.Ports.SerialPort.GetPortNames()
            .Select(p => p.TrimEnd())
            .Select(FixBlueToothPortNameBug)
            .ToArray();

            allPorts.AddRange(ports);

            return allPorts.ToArray();
        }

        public static string GetNiceName(string port)
        {
            if (serialportproblem)
                return "";

            DateTime start = DateTime.Now;
            try
            {
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort"); // Win32_USBControllerDevice
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    //DeviceID                     
                    if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                    {
                        DateTime end = DateTime.Now;

                        if ((end - start).TotalSeconds > 5)
                            serialportproblem = true;

                        return obj2.Properties["Name"].Value.ToString();
                    }
                }
            }
            catch { }

            return "";
        }

        internal bool ispx4(string port)
        {
            try
            {
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort"); // Win32_USBControllerDevice
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    //DeviceID                     
                    if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                    {
                        if (obj2.Properties["Name"].Value.ToString().ToLower().Contains("px4"))
                            return true;
                    }
                }
            }
            catch { }

            return false;
        }

        // .NET bug: sometimes bluetooth ports are enumerated with bogus characters 
        // eg 'COM10' becomes 'COM10c' - one workaround is to remove the non numeric  
        // char. Annoyingly, sometimes a numeric char is added, which means this 
        // does not work in all cases. 
        // See http://connect.microsoft.com/VisualStudio/feedback/details/236183/system-io-ports-serialport-getportnames-error-with-bluetooth 
        private static string FixBlueToothPortNameBug(string portName)
        {
            if (!portName.StartsWith("COM"))
                return portName;
            var newPortName = "COM";                                // Start over with "COM" 
            foreach (var portChar in portName.Substring(3).ToCharArray())  //  Remove "COM", put the rest in a character array 
            {
                if (char.IsDigit(portChar))
                    newPortName += portChar.ToString(); // Good character, append to portName 
                //  else
                //log.WarnFormat("Bad (Non Numeric) character in port name '{0}' - removing", portName);
            }

            return newPortName;
        }
    }

    public class SerialPortFixer : IDisposable
    {
        public static void Execute(string portName)
        {
            using (new SerialPortFixer(portName))
            {
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (m_Handle != null)
            {
                m_Handle.Close();
                m_Handle = null;
            }
        }

        #endregion

        #region Implementation

        private const int DcbFlagAbortOnError = 14;
        private const int CommStateRetries = 10;
        private SafeFileHandle m_Handle;

        private SerialPortFixer(string portName)
        {
            const int dwFlagsAndAttributes = 0x40000000;
            const int dwAccess = unchecked((int)0xC0000000); if ((portName == null) || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Serial Port", "portName");
            }
            SafeFileHandle hFile = NativeMethods.CreateFile(@"\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes,
                                              IntPtr.Zero);
            if (hFile.IsInvalid)
            {
                WinIoError();
            }
            try
            {
                int fileType = NativeMethods.GetFileType(hFile);
                if ((fileType != 2) && (fileType != 0))
                {
                    throw new ArgumentException("Invalid Serial Port", "portName");
                }
                m_Handle = hFile;
                InitializeDcb();
            }
            catch
            {
                hFile.Close();
                m_Handle = null;
                throw;
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId,
                                                    StringBuilder lpBuffer, int nSize, IntPtr arguments);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref Comstat lpStat);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
                                                            IntPtr securityAttrs, int dwCreationDisposition,
                                                            int dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern int GetFileType(SafeFileHandle hFile);

        }

        private void InitializeDcb()
        {
            Dcb dcb = new Dcb();
            GetCommStateNative(ref dcb);
            dcb.Flags &= ~(1u << DcbFlagAbortOnError);
            SetCommStateNative(ref dcb);
        }

        private static string GetMessage(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(0x200);
            if (
                NativeMethods.FormatMessage(0x3200, new HandleRef(null, IntPtr.Zero), errorCode, 0, lpBuffer, lpBuffer.Capacity,
                              IntPtr.Zero) != 0)
            {
                return lpBuffer.ToString();
            }
            return "Unknown Error";
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return (int)(0x80070000 | (uint)errorCode);
        }

        private static void WinIoError()
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode));
        }

        private void GetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.GetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }
        private void SetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat(); for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.SetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

        #region Nested type: COMSTAT

        [StructLayout(LayoutKind.Sequential)]
        private struct Comstat
        {
            public readonly uint Flags;
            public readonly uint cbInQue;
            public readonly uint cbOutQue;
        }

        #endregion

        #region Nested type: DCB

        [StructLayout(LayoutKind.Sequential)]
        private struct Dcb
        {
            public readonly uint DCBlength;
            public readonly uint BaudRate;
            public uint Flags;
            public readonly ushort wReserved;
            public readonly ushort XonLim;
            public readonly ushort XoffLim;
            public readonly byte ByteSize;
            public readonly byte Parity;
            public readonly byte StopBits;
            public readonly byte XonChar;
            public readonly byte XoffChar;
            public readonly byte ErrorChar;
            public readonly byte EofChar;
            public readonly byte EvtChar;
            public readonly ushort wReserved1;
        }

        #endregion

        #endregion
    }
}