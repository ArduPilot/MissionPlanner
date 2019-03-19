using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using log4net;
using Microsoft.Win32.SafeHandles;

namespace MissionPlanner.Comms
{
    public class SerialPort : System.IO.Ports.SerialPort, ICommsSerial
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SerialPort));

        private static readonly object locker = new object();

        private static readonly Dictionary<string, string> comportnamecache = new Dictionary<string, string>();

        private static string portnamenice = "";

        public new bool DtrEnable
        {
            get => base.DtrEnable;
            set
            {
                log.Info(PortName + " DtrEnable " + value);
                if (base.DtrEnable == value) return;
                if (ispx4(PortName)) return;
                base.DtrEnable = value;
            }
        }

        public new bool RtsEnable
        {
            get => base.RtsEnable;
            set
            {
                log.Info(PortName + " RtsEnable " + value);
                if (base.RtsEnable == value) return;
                if (ispx4(PortName)) return;
                base.RtsEnable = value;
            }
        }

        /*
        protected override void Dispose(bool disposing)
        {
            try
            {
                try
                {
                    Type mytype = typeof(System.IO.Ports.SerialPort);
                    FieldInfo field = mytype.GetField("internalSerialStream", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (field != null)
                    {
                        Stream stream = (Stream)field.GetValue(this);

                        if (stream != null)
                        {
                            try
                            {
                                stream.Dispose();
                            }
                            catch (Exception ex) { Console.WriteLine("1 " + ex.ToString()); }
                            stream = null;
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("2 " + ex.ToString()); }

                base.Dispose(disposing);
            }
            catch (Exception ex) { Console.WriteLine("3 " + ex.ToString()); }
        }
        */

        public new void Open()
        {
            // 500ms write timeout - win32 api default
            WriteTimeout = 500;

            if (IsOpen)
                return;

            try
            {
                // this causes element not found with bluetooth devices.
                if (BaudRate > 115200)
                {
                    Console.WriteLine("Doing SerialPortFixer");
                    SerialPortFixer.Execute(PortName);
                    Console.WriteLine("Done SerialPortFixer");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (PortName.StartsWith("/"))
                if (!File.Exists(PortName))
                    throw new Exception("No such device");

            try
            {
                base.Open();
                WriteTimeout = -1;
            }
            catch
            {
                try
                {
                    Close();
                }
                catch
                {
                }

                throw;
            }
        }

        public new void Close()
        {
            log.Info("Closing port " + PortName);
            base.Close();
        }

        public void toggleDTR()
        {
            if (ispx4(PortName))
            {
                Console.WriteLine("PX4 - no DTR");
                return;
            }

            var open = IsOpen;
            Console.WriteLine("toggleDTR " + IsOpen);
            try
            {
                if (!open)
                    Open();
            }
            catch
            {
            }

            base.DtrEnable = false;
            base.RtsEnable = false;

            Thread.Sleep(50);

            base.DtrEnable = true;
            base.RtsEnable = true;

            Thread.Sleep(50);

            try
            {
                if (!open)
                    Close();
            }
            catch
            {
            }

            Console.WriteLine("toggleDTR done " + IsOpen);
        }

        public new static string[] GetPortNames()
        {
            // prevent hammering
            lock (locker)
            {
                var allPorts = new List<string>();

                if (Directory.Exists("/dev/"))
                {
                    // cleanup now
                    GC.Collect();
                    // mono is failing in here on linux "too many open files"
                    try
                    {
                        if (Directory.Exists("/dev/serial/by-id/"))
                            allPorts.AddRange(Directory.GetFiles("/dev/serial/by-id/", "*"));
                    }
                    catch
                    {
                    }

                    try
                    {
                        allPorts.AddRange(Directory.GetFiles("/dev/", "ttyACM*"));
                    }
                    catch
                    {
                    }

                    try
                    {
                        allPorts.AddRange(Directory.GetFiles("/dev/", "ttyUSB*"));
                    }
                    catch
                    {
                    }

                    try
                    {
                        allPorts.AddRange(Directory.GetFiles("/dev/", "rfcomm*"));
                    }
                    catch
                    {
                    }

                    try
                    {
                        allPorts.AddRange(Directory.GetFiles("/dev/", "*usb*"));
                    }
                    catch
                    {
                    }
                }

                string[] ports = null;

                try
                {
                    ports = System.IO.Ports.SerialPort.GetPortNames()
                        .Select(p => p.TrimEnd())
                        .Select(FixBlueToothPortNameBug)
                        .ToArray();
                }
                catch
                {
                }

                if (ports != null)
                    allPorts.AddRange(ports);

                return allPorts.ToArray();
            }
        }

        public static string GetNiceName(string port)
        {
            // make sure we are exclusive
            lock (locker)
            {
                log.Info("start GetNiceName " + port);
                portnamenice = "";

                if (comportnamecache.ContainsKey(port))
                {
                    log.Info("done GetNiceName cache " + port);
                    return comportnamecache[port];
                }

                try
                {
                    CallWithTimeout(GetName, 1000, port);
                }
                catch
                {
                }

                log.Info("done GetNiceName " + port + " = " + portnamenice);

                comportnamecache[port] = portnamenice;

                return (string) portnamenice.Clone();
            }
        }

        private static void GetName(string port)
        {
            try
            {
                /*
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");                // Win32_USBControllerDevice
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject obj2 in searcher.Get())
                    {
                        //DeviceID
                        if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                        {
                            portnamenice = obj2.Properties["Name"].Value.ToString();
                            return;
                        }
                    }
                }
                */
            }
            catch
            {
            }

            portnamenice = "";
        }

        private static void CallWithTimeout(Action<string> action, int timeoutMilliseconds, string data)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action(data);
            };

            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }

        internal bool ispx4(string port)
        {
            try
            {
                /*
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");// Win32_USBControllerDevice
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
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
                */
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

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
            var newPortName = "COM"; // Start over with "COM"
            foreach (var portChar in portName.Substring(3).ToCharArray()
            ) //  Remove "COM", put the rest in a character array
                if (char.IsDigit(portChar))
                    newPortName += portChar.ToString(); // Good character, append to portName
            //  else
            //log.WarnFormat("Bad (Non Numeric) character in port name '{0}' - removing", portName);

            return newPortName;
        }
    }

    public sealed class SerialPortFixer : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SerialPortFixer));

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Handle != null)
            {
                m_Handle.Dispose();
                m_Handle = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        public static void Execute(string portName)
        {
            using (new SerialPortFixer(portName))
            {
            }
        }

        #region Implementation

        private const int DcbFlagAbortOnError = 14;
        private const int CommStateRetries = 10;
        private SafeFileHandle m_Handle;

        private SerialPortFixer(string portName)
        {
            const int dwFlagsAndAttributes = 0x40000000;
            const int dwAccess = unchecked((int) 0xC0000000);
            if (portName == null || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid Serial Port", "portName");
            var hFile = NativeMethods.CreateFile(@"\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes,
                IntPtr.Zero);
            if (hFile.IsInvalid) WinIoError();
            try
            {
                var fileType = NativeMethods.GetFileType(hFile);
                if (fileType != 2 && fileType != 0) throw new ArgumentException("Invalid Serial Port", "portName");
                m_Handle = hFile;
                InitializeDcb();
            }
            catch
            {
                hFile.Dispose();
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
            var dcb = new Dcb();
            GetCommStateNative(ref dcb);
            log.Info("before dcb flags: " + dcb.Flags);
            dcb.Flags &= ~(1u << DcbFlagAbortOnError);
            log.Info("after dcb flags: " + dcb.Flags);
            SetCommStateNative(ref dcb);
        }

        private static string GetMessage(int errorCode)
        {
            var lpBuffer = new StringBuilder(0x200);
            if (
                NativeMethods.FormatMessage(0x3200, new HandleRef(null, IntPtr.Zero), errorCode, 0, lpBuffer,
                    lpBuffer.Capacity,
                    IntPtr.Zero) != 0)
                return lpBuffer.ToString();
            return "Unknown Error";
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return (int) (0x80070000 | (uint) errorCode);
        }

        private static void WinIoError()
        {
            var errorCode = Marshal.GetLastWin32Error();
            throw new IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode));
        }

        private void GetCommStateNative(ref Dcb lpDcb)
        {
            var commErrors = 0;
            var comStat = new Comstat();

            for (var i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat)) WinIoError();
                if (NativeMethods.GetCommState(m_Handle, ref lpDcb)) break;
                if (i == CommStateRetries - 1) WinIoError();
            }
        }

        private void SetCommStateNative(ref Dcb lpDcb)
        {
            var commErrors = 0;
            var comStat = new Comstat();
            for (var i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat)) WinIoError();
                if (NativeMethods.SetCommState(m_Handle, ref lpDcb)) break;
                if (i == CommStateRetries - 1) WinIoError();
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

        #endregion Nested type: COMSTAT

        #region Nested type: DCB

        /*
         * https://msdn.microsoft.com/en-us/library/windows/desktop/aa363214(v=vs.85).aspx
  DWORD fBinary  :1;
  DWORD fParity  :1;
  DWORD fOutxCtsFlow  :1;
  DWORD fOutxDsrFlow  :1;
  DWORD fDtrControl  :2;
  DWORD fDsrSensitivity  :1;
  DWORD fTXContinueOnXoff  :1;
  DWORD fOutX  :1;
  DWORD fInX  :1;
  DWORD fErrorChar  :1;
  DWORD fNull  :1;
  DWORD fRtsControl  :2;
  DWORD fAbortOnError  :1;
  DWORD fDummy2  :17;
         */

        [StructLayout(LayoutKind.Sequential)]
        public struct Dcb
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

        #endregion Nested type: DCB

        #endregion Implementation
    }
}