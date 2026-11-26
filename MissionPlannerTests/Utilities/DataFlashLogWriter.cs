using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlannerTests.Utilities
{
    /// <summary>
    /// Writes ArduPilot DataFlash (.bin) logs for tests, independently of MissionPlanner's reader.
    /// Values are given as they are stored: the scaled formats (c, C, e, E, L) take the raw integer.
    /// </summary>
    public class DataFlashLogWriter
    {
        public const byte FmtType = 0x80;

        readonly MemoryStream stream = new MemoryStream();
        readonly Dictionary<string, (byte type, string format)> formats = new Dictionary<string, (byte, string)>();

        public DataFlashLogWriter()
        {
            formats["FMT"] = (FmtType, "BBnNZ");
            WriteFmt(FmtType, 89, "FMT", "BBnNZ", "Type,Length,Name,Format,Columns");
        }

        public static int FieldSize(char format)
        {
            switch (format)
            {
                case 'b': case 'B': case 'M': return 1;
                case 'h': case 'H': case 'c': case 'C': return 2;
                case 'i': case 'I': case 'f': case 'e': case 'E': case 'L': case 'n': return 4;
                case 'q': case 'Q': case 'd': return 8;
                case 'N': return 16;
                case 'Z': case 'a': return 64;
                default: throw new ArgumentException("format " + format);
            }
        }

        public static int MessageLength(string format)
        {
            return 3 + format.Sum(FieldSize);
        }

        /// <summary>
        /// Write the FMT message for a type
        /// </summary>
        public DataFlashLogWriter Define(byte type, string name, string format, string columns)
        {
            formats[name] = (type, format);
            WriteFmt(type, (byte)MessageLength(format), name, format, columns);
            return this;
        }

        void WriteFmt(byte type, byte length, string name, string format, string columns)
        {
            var body = new List<byte> { type, length };
            body.AddRange(Fixed(name, 4));
            body.AddRange(Fixed(format, 16));
            body.AddRange(Fixed(columns, 64));
            WriteRaw(FmtType, body.ToArray());
        }

        public DataFlashLogWriter Write(string name, params object[] values)
        {
            var (type, format) = formats[name];
            if (values.Length != format.Length)
                throw new ArgumentException(name + " takes " + format.Length + " values");

            var body = new List<byte>();
            for (int i = 0; i < format.Length; i++)
                body.AddRange(Encode(format[i], values[i]));
            WriteRaw(type, body.ToArray());
            return this;
        }

        /// <summary>
        /// a message header and body as given, for broken logs
        /// </summary>
        public DataFlashLogWriter WriteRaw(byte type, byte[] body)
        {
            stream.WriteByte(0xA3);
            stream.WriteByte(0x95);
            stream.WriteByte(type);
            stream.Write(body, 0, body.Length);
            return this;
        }

        public DataFlashLogWriter WriteBytes(params byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
            return this;
        }

        public long Length => stream.Length;

        public byte[] ToArray() => stream.ToArray();

        static byte[] Fixed(string text, int size)
        {
            var bytes = new byte[size];
            var source = Encoding.ASCII.GetBytes(text);
            Array.Copy(source, bytes, Math.Min(size, source.Length));
            return bytes;
        }

        static byte[] Encode(char format, object value)
        {
            switch (format)
            {
                case 'b': return new[] { (byte)Convert.ToSByte(value) };
                case 'B': case 'M': return new[] { Convert.ToByte(value) };
                case 'h': case 'c': return BitConverter.GetBytes(Convert.ToInt16(value));
                case 'H': case 'C': return BitConverter.GetBytes(Convert.ToUInt16(value));
                case 'i': case 'e': case 'L': return BitConverter.GetBytes(Convert.ToInt32(value));
                case 'I': case 'E': return BitConverter.GetBytes(Convert.ToUInt32(value));
                case 'q': return BitConverter.GetBytes(Convert.ToInt64(value));
                case 'Q': return BitConverter.GetBytes(Convert.ToUInt64(value));
                case 'f': return BitConverter.GetBytes(Convert.ToSingle(value));
                case 'd': return BitConverter.GetBytes(Convert.ToDouble(value));
                case 'n': return Fixed((string)value, 4);
                case 'N': return Fixed((string)value, 16);
                case 'Z': return Fixed((string)value, 64);
                case 'a':
                    var shorts = (short[])value;
                    var bytes = new byte[64];
                    Buffer.BlockCopy(shorts, 0, bytes, 0, Math.Min(64, shorts.Length * 2));
                    return bytes;
                default: throw new ArgumentException("format " + format);
            }
        }

        /// <summary>
        /// The metadata ArduPilot writes at the start of a log: UNIT, MULT and FMTU
        /// </summary>
        public DataFlashLogWriter DefineUnits()
        {
            Define(177, "UNIT", "QbZ", "TimeUS,Id,Label");
            Define(178, "MULT", "Qbd", "TimeUS,Id,Mult");
            Define(179, "FMTU", "QBNN", "TimeUS,FmtType,UnitIds,MultIds");
            foreach (var unit in new[] { ('s', "s"), ('v', "V"), ('A', "A"), ('d', "deg"), ('m', "m"), ('#', "instance"), ('-', "") })
                Write("UNIT", 0L, (sbyte)unit.Item1, unit.Item2);
            foreach (var mult in new[] { ('F', 1e-6), ('0', 1.0), ('B', 0.01), ('-', 0.0) })
                Write("MULT", 0L, (sbyte)mult.Item1, mult.Item2);
            return this;
        }
    }
}
