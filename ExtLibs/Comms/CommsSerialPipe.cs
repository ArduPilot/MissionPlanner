using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using log4net;
using Microsoft.Win32.SafeHandles;

namespace MissionPlanner.Comms
{
    public class CommsSerialPipe : ICommsSerial
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommsSerialPipe));
        private static readonly object locker = new object();
        private COMMTIMEOUTS commTimeouts = default(COMMTIMEOUTS);
        private SafeFileHandle safeFileHandle;

        public void DiscardInBuffer()
        {
            ReadExisting();
        }

        public void Open()
        {
            // 500ms write timeout - win32 api default
            WriteTimeout = 500;

            if (IsOpen)
                return;

            if (PortName.StartsWith("/"))
                if (!File.Exists(PortName))
                    throw new Exception("No such device");

            try
            {
                IsOpen = false;
                BaseStream = new MemoryStream(20000);

                var dwFlagsAndAttributes = 128;
                safeFileHandle = CreateFile("\\\\.\\" + PortName,
                    -1073741824, 0, IntPtr.Zero, 3, dwFlagsAndAttributes, IntPtr.Zero);


                if (safeFileHandle.IsInvalid)
                {
                    throw new Exception("Invalid Port");
                }

                var num1 = 0;
                var commProp = default(COMMPROP);
                if (!GetCommProperties(safeFileHandle, ref commProp) || !GetCommModemStatus(safeFileHandle, ref num1))
                {
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 87 || lastWin32Error == 6)
                    {
                        //throw new ArgumentException(SR.GetString("Arg_InvalidSerialPortExtended"), "portName");
                    }
                    //WinIOError(lastWin32Error, string.Empty);
                }

                //var comStat = default(COMSTAT);
                var dcb = default(DCB);

                if (!GetCommState(safeFileHandle, ref dcb))
                {
                }

                dcb.BaudRate = (uint) BaudRate;
                dcb.ByteSize = 8;
                dcb.StopBits = 0;
                dcb.Parity = 0;
                //https://msdn.microsoft.com/en-us/library/windows/desktop/aa363214(v=vs.85).aspx
                SetDcbFlag(dcb, 0, 1); //fBinary  
                SetDcbFlag(dcb, 1, 0); //fParity  
                SetDcbFlag(dcb, 2, 1); //fOutxCtsFlow  
                SetDcbFlag(dcb, 3, 0); // fOutxDsrFlow  
                SetDcbFlag(dcb, 4, 0); //fDtrControl  
                SetDcbFlag(dcb, 5, 0); //fDsrSensitivity  
                SetDcbFlag(dcb, 6, 0); //fTXContinueOnXoff  
                SetDcbFlag(dcb, 7, 0); //fOutX  
                SetDcbFlag(dcb, 8, 0); //fInX  
                SetDcbFlag(dcb, 9, 0); //fErrorChar  
                SetDcbFlag(dcb, 10, 0); //fNull  
                SetDcbFlag(dcb, 11, 1); //fRtsControl  
                SetDcbFlag(dcb, 14, 0); //fAbortOnError  

                if (!SetCommState(safeFileHandle, ref dcb))
                {
                }

                commTimeouts.ReadTotalTimeoutConstant = 0;
                commTimeouts.ReadTotalTimeoutMultiplier = 0;
                commTimeouts.ReadIntervalTimeout = -1;

                if (!SetCommTimeouts(safeFileHandle, ref commTimeouts))
                {
                }

                //ThreadPool.BindHandle(safeFileHandle);

                //

                SetCommMask(safeFileHandle, 507);


                IsOpen = true;

                var th = new Thread(() =>
                {
                    while (IsOpen)
                    {
                        var buffer = new byte[4096];

                        uint read = 0;

                        ReadFile(safeFileHandle, buffer, (uint) buffer.Length, out read, IntPtr.Zero);

                        if (!IsOpen)
                            continue;

                        lock (locker)
                        {
                            var pos = BaseStream.Position;

                            BaseStream.Write(buffer, 0, (int) read);

                            BaseStream.Seek(pos, SeekOrigin.Begin);
                        }

                    }

                    safeFileHandle.Dispose();
                });

                th.IsBackground = true;
                th.Name = "CommsSerialPipe reader";
                th.Start();
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

        public int Read(byte[] buffer, int offset, int count)
        {
            var toread = Math.Min(count, BytesToRead);

            return BaseStream.Read(buffer, offset, toread);
        }

        public int ReadByte()
        {
            var count = 0;
            while (BytesToRead == 0)
            {
                Thread.Sleep(1);
                if (count > ReadTimeout)
                    throw new Exception("CommsSerialPipe Timeout on read");
                count++;
            }
            var buffer = new byte[1];
            Read(buffer, 0, 1);
            return buffer[0];
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadLine()
        {
            var temp = new byte[4000];
            var count = 0;
            var timeout = 0;

            while (timeout <= 100)
            {
                if (!IsOpen)
                {
                    break;
                }
                if (BytesToRead > 0)
                {
                    var letter = (byte) ReadByte();

                    temp[count] = letter;

                    if (letter == '\n') // normal line
                    {
                        break;
                    }


                    count++;
                    if (count == temp.Length)
                        break;
                    timeout = 0;
                }
                else
                {
                    timeout++;
                    Thread.Sleep(5);
                }
            }

            Array.Resize(ref temp, count + 1);

            return Encoding.ASCII.GetString(temp, 0, temp.Length);
        }

        public string ReadExisting()
        {
            var data = new byte[BytesToRead];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            var line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public void WriteLine(string line)
        {
            line = line + "\n";
            Write(line);
        }

        public void Write(string line)
        {
            var data = new ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var result = 0;
            if (offset != 0)
            {
                WriteFile(safeFileHandle, buffer.Skip(offset).ToArray(), count, out result, IntPtr.Zero);
            }
            else
            {
                WriteFile(safeFileHandle, buffer, count, out result, IntPtr.Zero);
            }
        }

        public void Close()
        {
            IsOpen = false;
            log.Info("Closing port " + PortName);
            BaseStream.Dispose();
            safeFileHandle.Dispose();
        }

        public void toggleDTR()
        {
        }

        public Stream BaseStream { get; internal set; }
        public int BaudRate { get; set; }

        public int BytesToRead
        {
            get
            {
                /*
                int num = 0;
                if (!ClearCommError(safeFileHandle, ref num, ref this.comStat))
                {
                    var err = Marshal.GetLastWin32Error();
                    //InternalResources.WinIOError();
                }
                return (int)this.comStat.cbInQue;
                */

                lock (locker)
                {
                    if (!BaseStream.CanRead)
                        return 0;

                    var left = (BaseStream.Length - BaseStream.Position);

                    if (left == 0)
                        BaseStream.SetLength(0);

                    return (int) left;
                }
            }
        }

        public int BytesToWrite { get; }
        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }
        public bool IsOpen { get; internal set; }
        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public StopBits StopBits { get; set; }
        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
            IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool DeviceIoControl(SafeHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer,
            uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned,
           [In] NativeOverlapped lpOverlapped);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WaitCommEvent(SafeFileHandle hFile, [In] int lpEvtMask,
           [In] NativeOverlapped lpOverlapped);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetCommState(SafeFileHandle hFile, ref DCB lpDCB);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern  bool GetOverlappedResult(SafeFileHandle hFile, [In] NativeOverlapped lpOverlapped,
            ref int lpNumberOfBytesTransferred, bool bWait);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetCommProperties(SafeFileHandle hFile, ref COMMPROP lpCommProp);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetCommTimeouts(SafeFileHandle hFile, ref COMMTIMEOUTS lpCommTimeouts);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetCommMask(SafeFileHandle hFile, int dwEvtMask);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetCommModemStatus(SafeFileHandle hFile, ref int lpModemStat);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref COMSTAT lpStat);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer,uint nNumberOfBytesToRead, 
            out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead, [In] ref System.Threading.NativeOverlapped lpOverlapped);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetCommState(SafeFileHandle hFile, ref DCB lpDCB);

        internal void SetDcbFlag(DCB dcb, int whichFlag, int setting)
        {
            setting <<= whichFlag;
            uint num;
            if (whichFlag == 4 || whichFlag == 12)
            {
                num = 3u;
            }
            else if (whichFlag == 15)
            {
                num = 131071u;
            }
            else
            {
                num = 1u;
            }
            dcb.Flags = (dcb.Flags & ~(num << whichFlag));
            dcb.Flags = (dcb.Flags | (uint) setting);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int WriteFile(SafeFileHandle handle, byte[] bytes, int numBytesToWrite,
    out int numBytesWritten, IntPtr lpOverlapped);

        public void Dispose()
        {
            Close();
        }

        internal struct COMMTIMEOUTS
        {
            public int ReadIntervalTimeout;
            public int ReadTotalTimeoutConstant;
            public int ReadTotalTimeoutMultiplier;
            public int WriteTotalTimeoutConstant;
            public int WriteTotalTimeoutMultiplier;
        }

        internal struct COMMPROP
        {
            public int dwCurrentRxQueue;
            public int dwCurrentTxQueue;
            public int dwMaxBaud;
            public int dwMaxRxQueue;
            public int dwMaxTxQueue;
            public int dwProvCapabilities;
            public int dwProvSpec1;
            public int dwProvSpec2;
            public int dwProvSubType;
            public int dwReserved1;
            public int dwServiceMask;
            public int dwSettableBaud;
            public int dwSettableParams;
            public char wcProvChar;
            public ushort wPacketLength;
            public ushort wPacketVersion;
            public ushort wSettableData;
            public ushort wSettableStopParity;
        }

        internal struct COMSTAT
        {
            public uint cbInQue;
            public uint cbOutQue;
            public uint Flags;
        }

        internal struct DCB
        {
            public uint BaudRate;
            public byte ByteSize;
            public uint DCBlength;
            public byte EofChar;
            public byte ErrorChar;
            public byte EvtChar;
            public uint Flags;
            public byte Parity;
            public byte StopBits;
            public ushort wReserved;
            public ushort wReserved1;
            public byte XoffChar;
            public ushort XoffLim;
            public byte XonChar;
            public ushort XonLim;
        }
    }
}