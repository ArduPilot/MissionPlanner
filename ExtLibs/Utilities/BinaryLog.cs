using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8_t = System.Byte;

namespace MissionPlanner.Utilities
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

        public struct log_format_cache
        {
            public uint8_t type;
            public uint8_t length;
            public string name;
            public string format;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct UnionArray
        {
            public UnionArray(byte[] bytes)
            {
                this.Shorts = null;
                this.Bytes = bytes;
            }

            [FieldOffset(0)]
            public byte[] Bytes;

            [FieldOffset(0)]
            public short[] Shorts;

            public override string ToString()
            {
                return "[" + String.Join(" ", Shorts.Take((Bytes.Length / 2)).ToList()) + "]";
            }
        }

        object locker = new object();
 
        Dictionary<string, log_Format> logformat = new Dictionary<string, log_Format>();

        public static event getFlightMode onFlightMode;

        public delegate string getFlightMode(string firmware, int modeno);

        public static void ConvertBin(string inputfn, string outputfn, Action<int> progress = null)
        {
            new BinaryLog().ConvertBini(inputfn, outputfn, progress);
        }

        void ConvertBini(string inputfn, string outputfn, Action<int> progress = null)
        {
            using (var stream = File.Open(outputfn, FileMode.Create))
            {
                using (BinaryReader br = new BinaryReader(new BufferedStream(File.OpenRead(inputfn), 1024*1024)))
                {
                    DateTime displaytimer = DateTime.MinValue;

                    var length = br.BaseStream.Length;

                    while (br.BaseStream.Position < length)
                    {
                        if (displaytimer.Second != DateTime.Now.Second)
                        {
                            if (progress != null)
                                progress((int) ((br.BaseStream.Position / (float) br.BaseStream.Length) * 100));

                            Console.WriteLine("ConvertBin " + (br.BaseStream.Position/(float) br.BaseStream.Length)*100);
                            displaytimer = DateTime.Now;
                        }
                        byte[] data = ASCIIEncoding.ASCII.GetBytes(ReadMessage(br.BaseStream, length));
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        private string _firmware = "";

        public string ReadMessage(Stream br, long length)
        {
            lock (locker)
            {
                int log_step = 0;

                while (br.Position < length)
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
                                string line = String.Join(", ", logEntryObjects(data, br).Select((a) =>
                                              {
                                                  if (a.IsNumber())
                                                      return (((IConvertible)a).ToString(CultureInfo.InvariantCulture));
                                                  else
                                                      return a.ToString();
                                              })) + "\r\n";

                                // we need to know the mav type to use the correct mode list.
                                if (line.Contains("PARM, RATE_RLL_P") || line.Contains("ArduCopter") ||
                                    line.Contains("Copter"))
                                {
                                    _firmware = "ArduCopter2";
                                }
                                else if ((line.Contains("PARM, H_SWASH_PLATE")) || line.Contains("ArduCopter"))
                                {
                                    _firmware = "ArduCopter2";
                                }
                                else if (line.Contains("PARM, PTCH2SRV_P") || line.Contains("ArduPlane") ||
                                         line.Contains("Plane"))
                                {
                                    _firmware ="ArduPlane";
                                }
                                else if (line.Contains("PARM, SKID_STEER_OUT") || line.Contains("ArduRover") ||
                                         line.Contains("Rover"))
                                {
                                    _firmware = "ArduRover";
                                }
                                else if (line.Contains("AntennaTracker") || line.Contains("Tracker"))
                                {
                                    _firmware = "ArduTracker";
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
        }


        internal (byte MsgType, long Offset) ReadMessageTypeOffset(Stream br, long length)
        {
            int log_step = 0;

            while (br.Position < length)
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
                            long pos = br.Position - 3;
                            // read fmt or seek length of packet
                            logEntryFMT(data, br);

                            return (data, pos);
                        }
                        catch
                        {
                            Console.WriteLine("Bad Binary log line {0}", data);
                        }

                        break;
                }
            }

            return (0, 0);

        }

        void logEntryFMT(byte packettype, Stream br)
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
                    //string lgformat = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] {'\0'});
                    //string lglabels = ASCIIEncoding.ASCII.GetString(logfmt.labels).Trim(new char[] {'\0'});

                    logformat[lgname] = logfmt;

                    packettypecache[logfmt.type] = new log_format_cache()
                    {
                        length = logfmt.length,
                        type = logfmt.type,
                        name = ASCIIEncoding.ASCII.GetString(logfmt.name).Trim(new char[] { '\0' }),
                        format = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] { '\0' }),
                    };

                    return;

                default:
                    //string format = "";
                    //string name = "";

                    int size = 0;

                    if (packettypecache[packettype].length != 0)
                    {
                        var fmt = packettypecache[packettype];
                        //name = fmt.name;
                        //format = fmt.format;
                        size = fmt.length;
                    }

                    // didnt find a match, return unknown packet type
                    if (size == 0)
                        return;

                    //byte[] buf = new byte[size - 3];
                    //br.Read(buf, 0, buf.Length);

                    br.Seek(br.Position + size - 3, SeekOrigin.Begin);
                    break;
            }
        }

        public object[] ReadMessageObjects(Stream br, long length)
        {
            lock (locker)
            {
                int log_step = 0;

                while (br.Position < length)
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
        }

        object[] logEntryObjects(byte packettype, Stream br)
        {
            lock (locker)
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

                        return new object[] {"FMT", logfmt.type, logfmt.length, lgname, lgformat, lglabels};

                    default:
                        string format = "";
                        string name = "";
                        int size = 0;

                        if (packettypecache[packettype].length != 0)
                        {
                            var fmt = packettypecache[packettype];
                            name = fmt.name;
                            format = fmt.format;
                            size = fmt.length;
                        }
                        else
                        {
                            foreach (log_Format fmt in logformat.Values)
                            {
                                packettypecache[fmt.type] = new log_format_cache()
                                {
                                    length = fmt.length,
                                    type = fmt.type,
                                    name = ASCIIEncoding.ASCII.GetString(fmt.name).Trim(new char[] {'\0'}),
                                    format = ASCIIEncoding.ASCII.GetString(fmt.format).Trim(new char[] {'\0'}),
                                };

                                if (fmt.type == packettype)
                                {
                                    name = packettypecache[fmt.type].name;
                                    format = packettypecache[fmt.type].format;
                                    size = fmt.length;
                                    //break;
                                }
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
        }

        private object[] ProcessMessageObjects(byte[] message, string name, string format)
        {
            char[] form = format.ToCharArray();

            int offset = 0;

            List<object> answer = new List<object>();

            answer.Add(name);

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
                        string mode = onFlightMode?.Invoke(_firmware, modeno);
                        if (mode == null)
                            mode = modeno.ToString();
                        answer.Add(mode);
                        offset++;
                        break;
                    case 'Z':
                        answer.Add(ASCIIEncoding.ASCII.GetString(message, offset, 64).Trim(new char[] {'\0'}));
                        offset += 64;
                        break;
                    case 'a':
                        answer.Add(new UnionArray(message.Skip(offset).Take(64).ToArray()));
                        offset += 2 * 32;
                        break;
                    default:
                        return null;
                }
            }
            return answer.ToArray();
        }

        private log_format_cache[] packettypecache = new log_format_cache[256];

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
    a : short[32]
    119    + */
    }
}