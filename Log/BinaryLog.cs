using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using MissionPlanner.Controls;
using uint8_t = System.Byte;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    /// <summary>
    /// Convert a binary log to an assci log
    /// </summary>
    public class BinaryLog
    {
        public const byte HEAD_BYTE1 = 0xA3; // Decimal 163  
        public const byte HEAD_BYTE2 = 0x95; // Decimal 149  

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct log_Format
        {
            public uint8_t type;
            public uint8_t length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] format;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] labels;
        }

        private ProgressReporterDialogue prd;
        private string inputfn;
        private string outputfn;
        private event convertProgress convertstatus;

        private delegate void convertProgress(ProgressReporterDialogue prd, float progress);

        Dictionary<string, log_Format> logformat = new Dictionary<string, log_Format>();

        public static void ConvertBin(string inputfn, string outputfn, bool showui = true)
        {
            if (!showui)
            {
                new BinaryLog().ConvertBini(inputfn, outputfn, false);
                return;
            }

            new BinaryLog().doUI(inputfn, outputfn, true);
        }

        void doUI(string inputfn, string outputfn, bool showui = true)
        {
            this.inputfn = inputfn;
            this.outputfn = outputfn;

            prd = new ProgressReporterDialogue();

            prd.DoWork += prd_DoWork;

            prd.UpdateProgressAndStatus(-1, Strings.Converting_bin_to_log);

            this.convertstatus += BinaryLog_convertstatus;

            ThemeManager.ApplyThemeTo(prd);

            prd.RunBackgroundOperationAsync();

            prd.Dispose();
        }

        void BinaryLog_convertstatus(ProgressReporterDialogue prd, float progress)
        {
            prd.UpdateProgressAndStatus((int) progress, Strings.Converting_bin_to_log);
        }

        void prd_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            this.ConvertBini(inputfn, outputfn, true);
        }

        void ConvertBini(string inputfn, string outputfn, bool showui = true)
        {
            using (var stream = File.Open(outputfn, FileMode.Create))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(inputfn)))
                {
                    DateTime displaytimer = DateTime.MinValue;

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        if (displaytimer.Second != DateTime.Now.Second)
                        {
                            if (convertstatus != null && prd != null)
                                convertstatus(prd, (br.BaseStream.Position/(float) br.BaseStream.Length)*100);

                            Console.WriteLine("ConvertBin " + (br.BaseStream.Position/(float) br.BaseStream.Length)*100);
                            displaytimer = DateTime.Now;
                        }
                        byte[] data = ASCIIEncoding.ASCII.GetBytes(ReadMessage(br.BaseStream));
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        public string ReadMessage(Stream br)
        {
            int log_step = 0;

            while (br.Position < br.Length)
            {
                byte data = (byte) br.ReadByte();

                switch (log_step)
                {
                    case 0:
                        if (data == HEAD_BYTE1)
                        {
                            log_step++;
                        }
                        break;

                    case 1:
                        if (data == HEAD_BYTE2)
                        {
                            log_step++;
                        }
                        else
                        {
                            log_step = 0;
                        }
                        break;

                    case 2:
                        log_step = 0;
                        try
                        {
                            string line = logEntry(data, br);

                            // we need to know the mav type to use the correct mode list.
                            if (line.Contains("PARM, RATE_RLL_P") || line.Contains("ArduCopter") || line.Contains("Copter"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;
                            }
                            else if ((line.Contains("PARM, H_SWASH_PLATE")) || line.Contains("ArduCopter"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;
                            }
                            else if (line.Contains("PARM, PTCH2SRV_P") || line.Contains("ArduPlane") || line.Contains("Plane"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduPlane;
                            }
                            else if (line.Contains("PARM, SKID_STEER_OUT") || line.Contains("ArduRover") || line.Contains("Rover"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduRover;
                            }
                            else if (line.Contains("AntennaTracker") || line.Contains("Tracker"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduTracker;
                            }

                            return line;
                        }
                        catch
                        {
                            Console.WriteLine("Bad Binary log line {0}", data);
                        }
                        break;
                }
            }

            return "";
        }

        public object[] ReadMessageObjects(Stream br)
        {
            int log_step = 0;

            while (br.Position < br.Length)
            {
                byte data = (byte) br.ReadByte();

                switch (log_step)
                {
                    case 0:
                        if (data == HEAD_BYTE1)
                        {
                            log_step++;
                        }
                        break;

                    case 1:
                        if (data == HEAD_BYTE2)
                        {
                            log_step++;
                        }
                        else
                        {
                            log_step = 0;
                        }
                        break;

                    case 2:
                        log_step = 0;
                        try
                        {
                            var line = logEntryObjects(data, br);

                            return line;
                        }
                        catch
                        {
                            Console.WriteLine("Bad Binary log line {0}", data);
                        }
                        break;
                }
            }

            return null;
        }

        object[] logEntryObjects(byte packettype, Stream br)
        {
            switch (packettype)
            {
                case 0x80: // FMT

                    log_Format logfmt = new log_Format();

                    object obj = logfmt;

                    int len = Marshal.SizeOf(obj);

                    byte[] bytearray = new byte[len];

                    br.Read(bytearray, 0, bytearray.Length);

                    IntPtr i = Marshal.AllocHGlobal(len);

                    // create structure from ptr
                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    // copy byte array to ptr
                    Marshal.Copy(bytearray, 0, i, len);

                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    Marshal.FreeHGlobal(i);

                    logfmt = (log_Format) obj;

                    string lgname = ASCIIEncoding.ASCII.GetString(logfmt.name).Trim(new char[] {'\0'});
                    string lgformat = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] {'\0'});
                    string lglabels = ASCIIEncoding.ASCII.GetString(logfmt.labels).Trim(new char[] {'\0'});

                    logformat[lgname] = logfmt;

                    return null;

                default:
                    string format = "";
                    string name = "";
                    int size = 0;

                    foreach (log_Format fmt in logformat.Values)
                    {
                        if (fmt.type == packettype)
                        {
                            name = ASCIIEncoding.ASCII.GetString(fmt.name).Trim(new char[] {'\0'});
                            format = ASCIIEncoding.ASCII.GetString(fmt.format).Trim(new char[] {'\0'});
                            size = fmt.length;
                            break;
                        }
                    }

                    // didnt find a match, return unknown packet type
                    if (size == 0)
                        return null;

                    byte[] data = new byte[size - 3]; // size - 3 = message - messagetype - (header *2)

                    br.Read(data, 0, data.Length);

                    return ProcessMessageObjects(data, name, format);
            }
        }

        private object[] ProcessMessageObjects(byte[] message, string name, string format)
        {
            char[] form = format.ToCharArray();

            int offset = 0;

            List<object> answer = new List<object>();

            foreach (char ch in form)
            {
                switch (ch)
                {
                    case 'b':
                        answer.Add((sbyte) message[offset]);
                        offset++;
                        break;
                    case 'B':
                        answer.Add(message[offset]);
                        offset++;
                        break;
                    case 'h':
                        answer.Add(BitConverter.ToInt16(message, offset));
                        offset += 2;
                        break;
                    case 'H':
                        answer.Add(BitConverter.ToUInt16(message, offset));
                        offset += 2;
                        break;
                    case 'i':
                        answer.Add(BitConverter.ToInt32(message, offset));
                        offset += 4;
                        break;
                    case 'I':
                        answer.Add(BitConverter.ToUInt32(message, offset));
                        offset += 4;
                        break;
                    case 'q':
                        answer.Add(BitConverter.ToInt64(message, offset));
                        offset += 8;
                        break;
                    case 'Q':
                        answer.Add(BitConverter.ToUInt64(message, offset));
                        offset += 8;
                        break;
                    case 'f':
                        answer.Add(BitConverter.ToSingle(message, offset));
                        offset += 4;
                        break;
                    case 'd':
                        answer.Add(BitConverter.ToDouble(message, offset));
                        offset += 8;
                        break;
                    case 'c':
                        answer.Add((BitConverter.ToInt16(message, offset)/100.0));
                        offset += 2;
                        break;
                    case 'C':
                        answer.Add((BitConverter.ToUInt16(message, offset)/100.0));
                        offset += 2;
                        break;
                    case 'e':
                        answer.Add((BitConverter.ToInt32(message, offset)/100.0));
                        offset += 4;
                        break;
                    case 'E':
                        answer.Add((BitConverter.ToUInt32(message, offset)/100.0));
                        offset += 4;
                        break;
                    case 'L':
                        answer.Add(((double) BitConverter.ToInt32(message, offset)/10000000.0));
                        offset += 4;
                        break;
                    case 'n':
                        answer.Add(ASCIIEncoding.ASCII.GetString(message, offset, 4).Trim(new char[] {'\0'}));
                        offset += 4;
                        break;
                    case 'N':
                        answer.Add(ASCIIEncoding.ASCII.GetString(message, offset, 16).Trim(new char[] {'\0'}));
                        offset += 16;
                        break;
                    case 'M':
                        int modeno = message[offset];
                        answer.Add(modeno);
                        offset++;
                        break;
                    case 'Z':
                        answer.Add(ASCIIEncoding.ASCII.GetString(message, offset, 64).Trim(new char[] {'\0'}));
                        offset += 64;
                        break;
                    default:
                        return null;
                }
            }
            return answer.ToArray();
        }

        /// <summary>
        /// Process each log entry
        /// </summary>
        /// <param name="packettype">packet type</param>
        /// <param name="br">input file</param>
        /// <returns>string of converted data</returns>
        string logEntry(byte packettype, Stream br)
        {
            switch (packettype)
            {
                case 0x80: // FMT

                    log_Format logfmt = new log_Format();

                    object obj = logfmt;

                    int len = Marshal.SizeOf(obj);

                    byte[] bytearray = new byte[len];

                    br.Read(bytearray, 0, bytearray.Length);

                    IntPtr i = Marshal.AllocHGlobal(len);

                    // create structure from ptr
                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    // copy byte array to ptr
                    Marshal.Copy(bytearray, 0, i, len);

                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    Marshal.FreeHGlobal(i);

                    logfmt = (log_Format) obj;

                    string lgname = ASCIIEncoding.ASCII.GetString(logfmt.name).Trim(new char[] {'\0'});
                    string lgformat = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] {'\0'});
                    string lglabels = ASCIIEncoding.ASCII.GetString(logfmt.labels).Trim(new char[] {'\0'});

                    logformat[lgname] = logfmt;

                    string line = String.Format("FMT, {0}, {1}, {2}, {3}, {4}\r\n", logfmt.type, logfmt.length, lgname,
                        lgformat, lglabels);

                    return line;

                default:
                    string format = "";
                    string name = "";
                    int size = 0;

                    foreach (log_Format fmt in logformat.Values)
                    {
                        if (fmt.type == packettype)
                        {
                            name = ASCIIEncoding.ASCII.GetString(fmt.name).Trim(new char[] {'\0'});
                            format = ASCIIEncoding.ASCII.GetString(fmt.format).Trim(new char[] {'\0'});
                            size = fmt.length;
                            break;
                        }
                    }

                    // didnt find a match, return unknown packet type
                    if (size == 0)
                        return "UNKW, " + packettype;

                    byte[] data = new byte[size - 3]; // size - 3 = message - messagetype - (header *2)

                    br.Read(data, 0, data.Length);

                    return ProcessMessage(data, name, format);
            }
        }


        /*  
    105    +Format characters in the format string for binary log messages  
    106    +  b   : int8_t  
    107    +  B   : uint8_t  
    108    +  h   : int16_t  
    109    +  H   : uint16_t  
    110    +  i   : int32_t  
    111    +  I   : uint32_t  
    112    +  f   : float  
         *     d   : double
    113    +  N   : char[16]  
    114    +  c   : int16_t * 100  
    115    +  C   : uint16_t * 100  
    116    +  e   : int32_t * 100  
    117    +  E   : uint32_t * 100  
    118    +  L   : uint32_t latitude/longitude  
    119    + */


        /// <summary>
        /// Convert to ascii based on the existing format message
        /// </summary>
        /// <param name="message">raw binary message</param>
        /// <param name="name">Message type name</param>
        /// <param name="format">format string containing packet structure</param>
        /// <returns>formated ascii string</returns>
        string ProcessMessage(byte[] message, string name, string format)
        {
            char[] form = format.ToCharArray();

            int offset = 0;

            StringBuilder line = new StringBuilder(name);

            foreach (char ch in form)
            {
                switch (ch)
                {
                    case 'b':
                        line.Append(", " + (sbyte) message[offset]);
                        offset++;
                        break;
                    case 'B':
                        line.Append(", " + message[offset]);
                        offset++;
                        break;
                    case 'h':
                        line.Append(", " +
                                    BitConverter.ToInt16(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 2;
                        break;
                    case 'H':
                        line.Append(", " +
                                    BitConverter.ToUInt16(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 2;
                        break;
                    case 'i':
                        line.Append(", " +
                                    BitConverter.ToInt32(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'I':
                        line.Append(", " +
                                    BitConverter.ToUInt32(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'q':
                        line.Append(", " +
                                    BitConverter.ToInt64(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 8;
                        break;
                    case 'Q':
                        line.Append(", " +
                                    BitConverter.ToUInt64(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 8;
                        break;
                    case 'f':
                        line.Append(", " +
                                    BitConverter.ToSingle(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'd':
                        line.Append(", " +
                                    BitConverter.ToDouble(message, offset)
                                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
                        offset += 8;
                        break;
                    case 'c':
                        line.Append(", " +
                                    (BitConverter.ToInt16(message, offset)/100.0).ToString("0.00",
                                        System.Globalization.CultureInfo.InvariantCulture));
                        offset += 2;
                        break;
                    case 'C':
                        line.Append(", " +
                                    (BitConverter.ToUInt16(message, offset)/100.0).ToString("0.00",
                                        System.Globalization.CultureInfo.InvariantCulture));
                        offset += 2;
                        break;
                    case 'e':
                        line.Append(", " +
                                    (BitConverter.ToInt32(message, offset)/100.0).ToString("0.00",
                                        System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'E':
                        line.Append(", " +
                                    (BitConverter.ToUInt32(message, offset)/100.0).ToString("0.00",
                                        System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'L':
                        line.Append(", " +
                                    ((double) BitConverter.ToInt32(message, offset)/10000000.0).ToString(
                                        System.Globalization.CultureInfo.InvariantCulture));
                        offset += 4;
                        break;
                    case 'n':
                        line.Append(", " + ASCIIEncoding.ASCII.GetString(message, offset, 4).Trim(new char[] {'\0'}));
                        offset += 4;
                        break;
                    case 'N':
                        line.Append(", " + ASCIIEncoding.ASCII.GetString(message, offset, 16).Trim(new char[] {'\0'}));
                        offset += 16;
                        break;
                    case 'M':
                        int modeno = message[offset];
                        var modes = Common.getModesList(MainV2.comPort.MAV.cs);
                        string currentmode = "";

                        foreach (var mode in modes)
                        {
                            if (mode.Key == modeno)
                            {
                                currentmode = mode.Value;
                                break;
                            }
                        }

                        line.Append(", " + currentmode);
                        offset++;
                        break;
                    case 'Z':
                        line.Append(", " + ASCIIEncoding.ASCII.GetString(message, offset, 64).Trim(new char[] {'\0'}));
                        offset += 64;
                        break;
                    default:
                        return "Bad Conversion";
                }
            }

            line.Append("\r\n");
            return line.ToString();
        }
    }
}