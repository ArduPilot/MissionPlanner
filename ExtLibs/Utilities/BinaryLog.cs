using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8_t = System.Byte;
using System.Diagnostics;

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
                this._shorts = null;
                this._bytes = bytes;
            }

            [FieldOffset(0)]
            byte[] _bytes;

            [FieldOffset(0)]
            short[] _shorts;

            public byte[] Bytes
            {
                get
                {
                    return _bytes;
                }
                set
                {
                    _bytes = value;
                }
            }

            public ReadOnlySpan<short> Shorts
            {
                get
                {
                    return new ReadOnlySpan<short>(_shorts, 0, _bytes.Length / 2);
                }
            }

            public override string ToString()
            {
                return "[" + String.Join(" ", _shorts.Take((_bytes.Length / 2)).ToList()) + "]";
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
                                var objects = logEntryObjects(data, br);
                                // only text fields can name the vehicle: message names are 4 characters
                                // at most and the other fields are numbers
                                bool hastext = false;
                                for (int a = 1; a < objects.Length; a++)
                                {
                                    if (objects[a] is string || objects[a] is byte[])
                                    {
                                        hastext = true;
                                        break;
                                    }
                                }

                                string line = String.Join(", ", objects.Select((a) =>
                                              {
                                                  if (a.IsNumber())
                                                      return (((IConvertible)a).ToString(CultureInfo.InvariantCulture));
                                                  else if (a is System.Byte[])
                                                      return EscapeText(Encoding.ASCII.GetString(a as byte[]).Trim('\0'));
                                                  else
                                                      return a?.ToString();
                                              })) + "\r\n";

                                if (!hastext)
                                    return line;

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


        /// <summary>
        /// Escape a text field for the text form of a log line
        /// </summary>
        static string EscapeText(string str)
        {
            int a = 0;
            for (; a < str.Length; a++)
            {
                char c = str[a];
                if (c < 32 || c > 127 || c == '\\')
                    break;
            }

            // nothing to escape
            if (a == str.Length)
                return str;

            var sb = new StringBuilder(str.Length + 8);
            sb.Append(str, 0, a);
            for (; a < str.Length; a++)
            {
                char c = str[a];
                switch (c)
                {
                    // Escape \ as \\
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    // Escape certain whitespace characters
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        // Escape all other non-printable characters
                        if (c < 32 || c > 127)
                            sb.Append("\\x").Append(Convert.ToByte(c).ToString("X2"));
                        else
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        // header, type and the largest message, the length of a message is one byte
        internal const int MaxMessageSize = 256;

        // size of log_Format, the body of a FMT message
        const int FormatSize = 86;

        /// <summary>
        /// Find every message from the current position of the stream to length, in file order.
        /// FMT messages met on the way set the sizes of the messages that follow them.
        /// </summary>
        /// <param name="onMessage">called with the type and the offset of the header of each message</param>
        internal void ScanMessages(Stream br, long length, Action<byte, long> onMessage)
        {
            byte[] buffer = new byte[1024 * 1024];
            byte[] fmt = new byte[FormatSize];
            // file offset of buffer[0], bytes held in buffer, next byte to look at
            long bufferpos = br.Position;
            int bufferlen = 0;
            int p = 0;
            int log_step = 0;

            while (true)
            {
                if (p >= bufferlen)
                {
                    bufferpos += bufferlen;
                    bufferlen = 0;
                    p = 0;
                    if (bufferpos >= length)
                        break;
                    bufferlen = br.Read(buffer, 0, (int)Math.Min(buffer.Length, length - bufferpos));
                    if (bufferlen <= 0)
                        break;
                }

                byte data = buffer[p++];

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
                        long pos = bufferpos + p - 3;

                        if (data == 0x80) // FMT
                        {
                            // bring the whole body into the buffer
                            if (bufferlen - p < FormatSize)
                            {
                                int keep = bufferlen - p;
                                Buffer.BlockCopy(buffer, p, buffer, 0, keep);
                                bufferpos += p;
                                p = 0;
                                bufferlen = keep;
                                while (bufferlen < FormatSize && bufferpos + bufferlen < length)
                                {
                                    int read = br.Read(buffer, bufferlen,
                                        (int)Math.Min(buffer.Length - bufferlen, length - bufferpos - bufferlen));
                                    if (read <= 0)
                                        break;
                                    bufferlen += read;
                                }
                            }

                            // a body cut short by the end of the file is taken as zeros
                            int have = Math.Min(FormatSize, bufferlen - p);
                            Array.Clear(fmt, 0, fmt.Length);
                            Buffer.BlockCopy(buffer, p, fmt, 0, have);
                            p += have;

                            var logfmt = ToLogFormat(fmt, 0);

                            string lgname = ASCIIEncoding.ASCII.GetString(logfmt.name).Trim(new char[] {'\0'});

                            logformat[lgname] = logfmt;

                            packettypecache[logfmt.type] = new log_format_cache()
                            {
                                length = logfmt.length,
                                type = logfmt.type,
                                name = lgname,
                                format = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] {'\0'})
                            };
                        }
                        else
                        {
                            // 0 is an unknown packet type, it is still reported
                            int size = packettypecache[data].length;

                            if (size != 0)
                            {
                                if (size < 3)
                                {
                                    Console.WriteLine("Bad Binary log line {0}", data);
                                    break;
                                }

                                // step over the body
                                long skip = size - 3;
                                if (skip <= bufferlen - p)
                                {
                                    p += (int)skip;
                                }
                                else
                                {
                                    bufferpos = Math.Min(bufferpos + p + skip, length);
                                    br.Seek(bufferpos, SeekOrigin.Begin);
                                    bufferlen = 0;
                                    p = 0;
                                }
                            }
                        }

                        if (data == 0 && pos == 0)
                            break;

                        onMessage(data, pos);
                        break;
                }
            }
        }

        static log_Format ToLogFormat(byte[] message, int offset)
        {
            var logfmt = new log_Format()
            {
                type = message[offset],
                length = message[offset + 1],
                name = new byte[4],
                format = new byte[16],
                labels = new byte[64]
            };
            Buffer.BlockCopy(message, offset + 2, logfmt.name, 0, 4);
            Buffer.BlockCopy(message, offset + 6, logfmt.format, 0, 16);
            Buffer.BlockCopy(message, offset + 22, logfmt.labels, 0, 64);
            return logfmt;
        }

        /// <summary>
        /// Decode a message held in memory, as ReadMessageObjects does from a stream
        /// </summary>
        /// <param name="buffer">holds the message</param>
        /// <param name="offset">where the header of the message starts in buffer</param>
        /// <param name="count">the bytes of the log that follow offset in buffer</param>
        /// <param name="items">the fields of the message</param>
        /// <param name="size">the size of the message, when it could not be decoded</param>
        /// <returns>
        /// 1: items is the message. 0: not decoded, read it from the stream. -1: the message is bad, carry
        /// on from the stream after its size bytes
        /// </returns>
        internal int TryReadMessageObjects(byte[] buffer, int offset, int count, out object[] items, out int size)
        {
            items = null;
            size = 0;

            lock (locker)
            {
                if (count < 3 || buffer[offset] != HEAD_BYTE1 || buffer[offset + 1] != HEAD_BYTE2)
                    return 0;

                byte packettype = buffer[offset + 2];

                // FMT, and types not read as objects yet, are left to the stream
                if (packettype == 0x80)
                    return 0;

                size = packettypecache[packettype].length;

                // so is a message that is not all here, or whose format describes more than its body holds
                if (size < 3 || size > count || FormatSizeOf(packettype) > size - 3)
                    return 0;

                var fmt = packettypecache[packettype];
                try
                {
                    items = ProcessMessageObjects(buffer, offset + 3, fmt.name, fmt.format);
                    return 1;
                }
                catch
                {
                    Console.WriteLine("Bad Binary log line {0}", packettype);
                    return -1;
                }
            }
        }

        // the bytes each type's format describes, and the format that was worked out for
        int[] formatsize = new int[256];
        string[] formatsizeof = new string[256];

        int FormatSizeOf(byte packettype)
        {
            string format = packettypecache[packettype].format;

            if (format == null || !ReferenceEquals(formatsizeof[packettype], format))
            {
                int size = 0;
                if (format == null)
                {
                    size = int.MaxValue;
                }
                else
                {
                    foreach (char ch in format)
                        size += FieldSize(ch);
                }

                formatsize[packettype] = size;
                formatsizeof[packettype] = format;
            }

            return formatsize[packettype];
        }

        /// <summary>
        /// the bytes GetObjectFromMessage takes for a format character
        /// </summary>
        static int FieldSize(char type)
        {
            switch (type)
            {
                case 'b':
                case 'B':
                case 'M':
                    return 1;
                case 'h':
                case 'H':
                case 'g':
                case 'c':
                case 'C':
                    return 2;
                case 'i':
                case 'I':
                case 'f':
                case 'e':
                case 'E':
                case 'L':
                case 'n':
                    return 4;
                case 'q':
                case 'Q':
                case 'd':
                    return 8;
                case 'N':
                    return 16;
                case 'Z':
                case 'a':
                    return 64;
                default:
                    return 0;
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

                        return ProcessMessageObjects(data, 0, name, format);
                }
            }
        }
        
        private object[] ProcessMessageObjects(byte[] message, int offset, string name, string format)
        {
            object[] answer = new object[format.Length + 1];

            answer[0] = name;

            int a = 1;

            foreach (char ch in format)
            {
                var temp = GetObjectFromMessage(ch, message, offset);
                answer[a] = temp.item;
                offset += temp.size;
                a++;
            }
            return answer;
        }

        public (object item, int size) GetObjectFromMessage(char type, byte[] message, int offset)
        {
            switch (type)
            {
                case 'b':
                    return ((sbyte) message[offset], 1);
                case 'B':
                    return (message[offset], 1);

                case 'h':
                    return (BitConverter.ToInt16(message, offset), 2);

                case 'H':
                    return (BitConverter.ToUInt16(message, offset), 2);

                case 'i':
                    return (BitConverter.ToInt32(message, offset), 4);

                case 'I':
                    return (BitConverter.ToUInt32(message, offset), 4);

                case 'q':
                    return (BitConverter.ToInt64(message, offset), 8);

                case 'Q':
                    return (BitConverter.ToUInt64(message, offset), 8);

                case 'g':
                    return (HalfHelper.HalfushortToSingle(BitConverter.ToUInt16(message, offset)), 2);

                case 'f':
                    return (BitConverter.ToSingle(message, offset), 4);

                case 'd':
                    return (BitConverter.ToDouble(message, offset), 8);

                case 'c':
                    return (BitConverter.ToInt16(message, offset) / 100.0, 2);

                case 'C':
                    return (BitConverter.ToUInt16(message, offset) / 100.0, 2);

                case 'e':
                    return (BitConverter.ToInt32(message, offset) / 100.0, 4);

                case 'E':
                    return (BitConverter.ToUInt32(message, offset) / 100.0, 4);

                case 'L':
                    return (BitConverter.ToInt32(message, offset) / 10000000.0, 4);

                case 'n':
                    return (Encoding.ASCII.GetString(message, offset, 4).Trim('\0'), 4);

                case 'N':
                    return (Encoding.ASCII.GetString(message, offset, 16).Trim('\0'), 16);

                case 'M':
                    int modeno = message[offset];
                    var mode = onFlightMode?.Invoke(_firmware, modeno);
                    if (mode == null)
                        mode = modeno.ToString();
                    return (mode, 1);

                case 'Z':
                    return (new ReadOnlySpan<byte>(message, offset, 64).ToArray(), 64);

                case 'a':
                    return (new UnionArray(new ReadOnlySpan<byte>(message, offset, 64).ToArray()), 2 * 32);

                default:
                    return (null, 0);
            }
        }


        private log_format_cache[] packettypecache = new log_format_cache[256];

        /*
         https://github.com/ArduPilot/ardupilot/blob/master/libraries/AP_Logger/LogStructure.h
      Format characters in the format string for binary log messages
        a   : int16_t[32]
        b   : int8_t
        B   : uint8_t
        c   : int16_t * 100
        C   : uint16_t * 100
        d   : double
        e   : int32_t * 100
        E   : uint32_t * 100
        f   : float
        h   : int16_t
        H   : uint16_t
        i   : int32_t
        I   : uint32_t
        L   : int32_t latitude/longitude
        M   : uint8_t flight mode
        N   : char[16]
        n   : char[4]
        q   : int64_t
        Q   : uint64_t
        Z   : char[64]
       */
    }
}