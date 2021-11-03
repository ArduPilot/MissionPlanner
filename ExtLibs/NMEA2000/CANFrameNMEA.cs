using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

namespace NMEA2000
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    class Program
    {
        static void Main(string[] args)
        {
            msggen.run();
        }
    }
    public partial class Pgns
    {
        [JsonProperty("Comment")]
        public string Comment { get; set; }

        [JsonProperty("CreatorCode")]
        public string CreatorCode { get; set; }

        [JsonProperty("License")]
        public string License { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("PGNs")]
        public List<Pgn> PgNs { get; set; }
    }

    public partial class Pgn
    {
        [JsonProperty("PGN")]
        public long PgnPgn { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Type")]
        public PgnType Type { get; set; }

        [JsonProperty("Complete")]
        public bool Complete { get; set; }

        [JsonProperty("Length")]
        public long Length { get; set; }

        [JsonProperty("RepeatingFields", NullValueHandling = NullValueHandling.Ignore)]
        public long? RepeatingFields { get; set; }

        [JsonProperty("Fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<Field> Fields { get; set; }

        [JsonProperty("Missing", NullValueHandling = NullValueHandling.Ignore)]
        public List<Missing> Missing { get; set; }

        [JsonProperty("RepeatingFieldSet1", NullValueHandling = NullValueHandling.Ignore)]
        public long? RepeatingFieldSet1 { get; set; }

        [JsonProperty("RepeatingFieldSet2", NullValueHandling = NullValueHandling.Ignore)]
        public long? RepeatingFieldSet2 { get; set; }
    }

    public partial class Field
    {
        [JsonProperty("Order")]
        public long Order { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("BitLength")]
        public long BitLength { get; set; } = 0;

        [JsonProperty("BitOffset", NullValueHandling = NullValueHandling.Ignore)]
        public long? BitOffset { get; set; } = 0;

        [JsonProperty("BitStart")]
        public long BitStart { get; set; } = 0;

        [JsonProperty("Type", NullValueHandling = NullValueHandling.Ignore)]
        public FieldType? Type { get; set; }

        [JsonProperty("Signed")]
        public bool Signed { get; set; }

        [JsonProperty("EnumValues", NullValueHandling = NullValueHandling.Ignore)]
        public List<EnumValue> EnumValues { get; set; }

        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("Resolution", NullValueHandling = NullValueHandling.Ignore)]
        public double? Resolution { get; set; }

        [JsonProperty("Match", NullValueHandling = NullValueHandling.Ignore)]
        public long? Match { get; set; }

        [JsonProperty("Units")]
        public string Units { get; set; }

        [JsonProperty("Offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Offset { get; set; } = 0;

        [JsonProperty("EnumBitValues", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dictionary<string, string>> EnumBitValues { get; set; }
    }

    public partial class EnumValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Value { get; set; }
    }

    public enum FieldType { AsciiOrUnicodeStringStartingWithLengthAndControlByte, AsciiStringStartingWithLengthByte, AsciiText, BinaryData, Bitfield, Date, DecimalEncodedNumber, IeeeFloat, Integer, Latitude, Longitude, LookupTable, ManufacturerCode, Pressure, PressureHires, StringWithStartStopByte, Temperature, TemperatureHires, Time };

    public enum Missing { FieldLengths, Fields, Lookups, Precision, SampleData };

    public enum PgnType { Fast, Iso, Single };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                FieldTypeConverter.Singleton,
                MissingConverter.Singleton,
                PgnTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class FieldTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FieldType) || t == typeof(FieldType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ASCII or UNICODE string starting with length and control byte":
                    return FieldType.AsciiOrUnicodeStringStartingWithLengthAndControlByte;
                case "ASCII string starting with length byte":
                    return FieldType.AsciiStringStartingWithLengthByte;
                case "ASCII text":
                    return FieldType.AsciiText;
                case "Binary data":
                    return FieldType.BinaryData;
                case "Bitfield":
                    return FieldType.Bitfield;
                case "Date":
                    return FieldType.Date;
                case "Decimal encoded number":
                    return FieldType.DecimalEncodedNumber;
                case "IEEE Float":
                    return FieldType.IeeeFloat;
                case "Integer":
                    return FieldType.Integer;
                case "Latitude":
                    return FieldType.Latitude;
                case "Longitude":
                    return FieldType.Longitude;
                case "Lookup table":
                    return FieldType.LookupTable;
                case "Manufacturer code":
                    return FieldType.ManufacturerCode;
                case "Pressure":
                    return FieldType.Pressure;
                case "Pressure (hires)":
                    return FieldType.PressureHires;
                case "String with start/stop byte":
                    return FieldType.StringWithStartStopByte;
                case "Temperature":
                    return FieldType.Temperature;
                case "Temperature (hires)":
                    return FieldType.TemperatureHires;
                case "Time":
                    return FieldType.Time;
            }
            throw new Exception("Cannot unmarshal type FieldType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (FieldType)untypedValue;
            switch (value)
            {
                case FieldType.AsciiOrUnicodeStringStartingWithLengthAndControlByte:
                    serializer.Serialize(writer, "ASCII or UNICODE string starting with length and control byte");
                    return;
                case FieldType.AsciiStringStartingWithLengthByte:
                    serializer.Serialize(writer, "ASCII string starting with length byte");
                    return;
                case FieldType.AsciiText:
                    serializer.Serialize(writer, "ASCII text");
                    return;
                case FieldType.BinaryData:
                    serializer.Serialize(writer, "Binary data");
                    return;
                case FieldType.Bitfield:
                    serializer.Serialize(writer, "Bitfield");
                    return;
                case FieldType.Date:
                    serializer.Serialize(writer, "Date");
                    return;
                case FieldType.DecimalEncodedNumber:
                    serializer.Serialize(writer, "Decimal encoded number");
                    return;
                case FieldType.IeeeFloat:
                    serializer.Serialize(writer, "IEEE Float");
                    return;
                case FieldType.Integer:
                    serializer.Serialize(writer, "Integer");
                    return;
                case FieldType.Latitude:
                    serializer.Serialize(writer, "Latitude");
                    return;
                case FieldType.Longitude:
                    serializer.Serialize(writer, "Longitude");
                    return;
                case FieldType.LookupTable:
                    serializer.Serialize(writer, "Lookup table");
                    return;
                case FieldType.ManufacturerCode:
                    serializer.Serialize(writer, "Manufacturer code");
                    return;
                case FieldType.Pressure:
                    serializer.Serialize(writer, "Pressure");
                    return;
                case FieldType.PressureHires:
                    serializer.Serialize(writer, "Pressure (hires)");
                    return;
                case FieldType.StringWithStartStopByte:
                    serializer.Serialize(writer, "String with start/stop byte");
                    return;
                case FieldType.Temperature:
                    serializer.Serialize(writer, "Temperature");
                    return;
                case FieldType.TemperatureHires:
                    serializer.Serialize(writer, "Temperature (hires)");
                    return;
                case FieldType.Time:
                    serializer.Serialize(writer, "Time");
                    return;
            }
            throw new Exception("Cannot marshal type FieldType");
        }

        public static readonly FieldTypeConverter Singleton = new FieldTypeConverter();
    }

    internal class MissingConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Missing) || t == typeof(Missing?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "FieldLengths":
                    return Missing.FieldLengths;
                case "Fields":
                    return Missing.Fields;
                case "Lookups":
                    return Missing.Lookups;
                case "Precision":
                    return Missing.Precision;
                case "SampleData":
                    return Missing.SampleData;
            }
            throw new Exception("Cannot unmarshal type Missing");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Missing)untypedValue;
            switch (value)
            {
                case Missing.FieldLengths:
                    serializer.Serialize(writer, "FieldLengths");
                    return;
                case Missing.Fields:
                    serializer.Serialize(writer, "Fields");
                    return;
                case Missing.Lookups:
                    serializer.Serialize(writer, "Lookups");
                    return;
                case Missing.Precision:
                    serializer.Serialize(writer, "Precision");
                    return;
                case Missing.SampleData:
                    serializer.Serialize(writer, "SampleData");
                    return;
            }
            throw new Exception("Cannot marshal type Missing");
        }

        public static readonly MissingConverter Singleton = new MissingConverter();
    }

    internal class PgnTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PgnType) || t == typeof(PgnType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Fast":
                    return PgnType.Fast;
                case "ISO":
                    return PgnType.Iso;
                case "Single":
                    return PgnType.Single;
            }
            throw new Exception("Cannot unmarshal type PgnType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (PgnType)untypedValue;
            switch (value)
            {
                case PgnType.Fast:
                    serializer.Serialize(writer, "Fast");
                    return;
                case PgnType.Iso:
                    serializer.Serialize(writer, "ISO");
                    return;
                case PgnType.Single:
                    serializer.Serialize(writer, "Single");
                    return;
            }
            throw new Exception("Cannot marshal type PgnType");
        }

        public static readonly PgnTypeConverter Singleton = new PgnTypeConverter();
    }


    public class msggen
    {
        /*
12:02:25.513 T 0DF11300 00 BD 36 FF FF F9 13 56
2021-10-27T12:02:25.513 3   0 255 127251 Rate of Turn:  SID = 0; Rate = -0.09225 deg/s
2021-10-27T12:02:25.513 3 000 255 127251 : 00 bd 36 ff ff f9 13 56
  
12:02:25.513 T 09F80102 41 B8 3D E9 EB F9 13 56
2021-10-27T12:02:25.513 2   2 255 129025 Position, Rapid Update:(-381831103)   Latitude = -38.1831103(1444149739) ; Longitude = 144.4149739
2021-10-27T12:02:25.513 2 002 255 129025 : 41 b8 3d e9 eb f9 13 56

12:02:25.513 T 0DF80502 E0 2B 00 ED 49 00 5C 73
12:02:25.513 T 0DF80502 E1 0D C0 DB 1F 72 A2 76
12:02:25.513 T 0DF80502 E2 B3 FA 00 08 49 79 89
12:02:25.513 T 0DF80502 E3 A6 0A 14 30 4C 75 FF
12:02:25.513 T 0DF80502 E4 FF FF FF FF 12 FC 13
12:02:25.513 T 0DF80502 E5 46 00 8C 00 8A FD FF
12:02:25.513 T 0DF80502 E6 FF 00 FF FF FF FF FF
2021-10-27T12:02:25.513 3 002 255 129029 : e0 2b 00 ed 49 00 5c 73
2021-10-27T12:02:25.513 3 002 255 129029 : e1 0d c0 db 1f 72 a2 76
2021-10-27T12:02:25.513 3 002 255 129029 : e2 b3 fa 00 08 49 79 89
2021-10-27T12:02:25.513 3 002 255 129029 : e3 a6 0a 14 30 4c 75 ff
2021-10-27T12:02:25.513 3 002 255 129029 : e4 ff ff ff ff 12 fc 13
2021-10-27T12:02:25.513 3 002 255 129029 : e5 46 00 8c 00 8a fd ff
2021-10-27T12:02:25.513 3   2 255 129029 GNSS Position Data:  SID = 0(date 49ed = 18925) ; Date = 2021.10.25(time d735c00 = 225664000) ; Time = 06:16:06.04000(fab376a2721fdbc0 = -381831103324890176) (-381831103) ; Latitude = -38.1831103(140aa68979490800 = 1444149739901224960) (1444149739) ; Longitude = 144.4149739; Altitude = -9.090000 m; GNSS type = GPS+GLONASS; Method = GNSS fix; Integrity = No integrity checking; Number of SVs = 19; HDOP = 0.70; PDOP = 1.40; Geoidal Separation = -6.30 m; Reference Stations = 0
2021-10-27T12:02:25.513 3 002 255 129029 : e6 ff 00 ff ff ff ff ff

12:02:25.513 T 1DF11A02 00 F8 FF FF F6 07 FF FF
2021-10-27T12:02:25.513 7   2 255 127258 Magnetic Variation:  SID = 0; Source = WMM 2020; Age of service = Unknown; Variation = 11.7 deg
2021-10-27T12:02:25.513 7 002 255 127258 : 00 f8 ff ff f6 07 ff ff
         */

        public static object Process(byte[] frame) 
        {
            var cf = new CANFrameNMEA(frame.Take(4).Reverse().ToArray());

            var msginfo = NMEA2000msgs.msgs.Where(a => a.Item1 == cf.PDU);

            var msg = Activator.CreateInstance(msginfo.First().Item2) as INMEA2000;

            msg.SetPacketData(frame.Skip(4).ToArray());

            return null;
        }

        public static void run()
        {
            var pgns = JsonConvert.DeserializeObject<Pgns>(File.ReadAllText("Resources/pgns.json"), Converter.Settings);

            Console.WriteLine(@"using System;
using MissionPlanner.Utilities;
using System.Collections.Generic;

namespace NMEA2000
{
    public class NMEA2000msgs
    {
        public static readonly (int, Type)[] msgs = {
");
            foreach (var pgn in pgns.PgNs)
                Console.WriteLine(@"                ("+pgn.PgnPgn+", typeof("+pgn.Id+")),");

            Console.WriteLine(@"};
    }

    public interface INMEA2000
    {
        void SetPacketData(byte[] packet);
    }
");


            List<string> msgnames = new List<string>();

            foreach (var pgn in pgns.PgNs)
            {
                var pgnno = pgn.PgnPgn;
                var desc = pgn.Description;
                var fields = pgn.Fields;
                var type = pgn.Type;
                var length = pgn.Length;

                if (msgnames.Contains(pgn.Id))
                    continue;
                msgnames.Add(pgn.Id);

                Console.WriteLine(" /// " + desc);
                Console.WriteLine(" /// " + type);
                Console.WriteLine(" /// " + pgnno);
                Console.WriteLine(" public class " + pgn.Id + " : INMEA2000 { ");
                Console.WriteLine(" byte[] _packet;");
                Console.WriteLine(" public void SetPacketData(byte[] packet) { _packet = packet; }");
                Console.WriteLine(" public " + pgn.Id + "(byte[] packet) { _packet = packet; }");

                // event
                // override
                // reserved

                fields?
                    .Distinct(new MissionPlanner.Utilities.EqualityComparer<Field>((a, b) => a.Id == b.Id))
                    .ForEach(a =>
                {
                    // name same as class
                    if (a.Id.Equals(pgn.Id))
                        a.Id += "_";
                    Console.WriteLine($"    public {ConvertType(a.Type)} {ConvertName(a.Id)} {{ get {{ return _packet.GetBitOffsetLength<{ConvertType(a.Type)}>({a.BitStart}, {a.BitOffset}, {a.BitLength}, {a.Signed.ToString().ToLower()}, {a.Resolution ?? 0}); }} set {{ }}  }}");
                });
                

                Console.WriteLine("}");
            }

            Console.WriteLine(@"
}
");
        }

        public static string ConvertName(string no)
        {
            return no.Replace("event", "@event")
                .Replace("override", "@override")
                .Replace("1stTelecommand", "_1stTelecommand")
                .Replace("reserved", "@reserved")
                 .Replace("unused", "@unused");
        }

        private static string ConvertType(FieldType? jToken)
        {
            if(jToken == null)
                return "int";

            switch (jToken)
            {
                case FieldType.AsciiOrUnicodeStringStartingWithLengthAndControlByte:
                    return "string";
                case FieldType.AsciiStringStartingWithLengthByte:
                    return "string";
                case FieldType.AsciiText:
                    return "string";
                case FieldType.BinaryData:
                    return "byte[]";
                case FieldType.Bitfield:
                     return "long";
                case FieldType.Date:
                    return "DateTime";
                case FieldType.DecimalEncodedNumber:
                    return "decimal";
                case FieldType.IeeeFloat:
                   return "float";
                case FieldType.Integer:
                   return "int";
                case FieldType.Latitude:
                    return "double";
                case FieldType.Longitude:
                    return "double";
                case FieldType.LookupTable:
                    return "string";
                case FieldType.ManufacturerCode:
                    return "long";
                case FieldType.Pressure:
                    return "double";
                case FieldType.PressureHires:
                    return "double";
                case FieldType.StringWithStartStopByte:
                    return "string";
                case FieldType.Temperature:
                    return "double";
                case FieldType.TemperatureHires:
                    return "double";
                case FieldType.Time:
                     return "DateTime";
            }

            throw new Exception();
        }
    }



    public class ConvertCANLogToYDWG
    {
        public static void runme(string file = @"C:\Users\mich1\OneDrive\2021-10-25 17-16-05.can")
        {
            Regex reg = new Regex("^T(.{8})(.)(.+)(....)$");

            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                var match = reg.Match(line);
                if (match.Success)
                {
                    // ts prio src dst  pgn
                    var cf = new CANFrameNMEA((match.Groups[1].Value.HexStringToByteArray().Reverse().ToArray()));

                    //       {4} {5} {6} {7}
                    Console.WriteLine("{0} {1} {2} {3}", DateTime.Now.ToString("hh:mm:ss.fff"), "T", match.Groups[1].Value, (match.Groups[3].Value.HexStringToSpacedHex()), cf.Priority, cf.DataPage, cf.PDU, cf.SourceAddress);
                }
            }
        }
    }

    public class CANFrameNMEA
    {
        private byte[] packet_data;

        public CANFrameNMEA(Span<byte> packet_data)
        {
            this.packet_data = packet_data.ToArray();
        }

        //0-256
        public byte SourceAddress
        {
            get { return (byte) (packet_data[0]); }
            set { packet_data[0] = (byte) ((packet_data[0]) | (value)); }
        }

        public byte DestAddress
        {
            get
            {
                if (packet_data[2] < 240)
                    return packet_data[1];
                else return 0xff;
            }
        }

        // 0 - 65535    anon 0-3
        public UInt32 PDU //  PGN
        {
            get
            {
                var pf = packet_data[2];
                var ps = packet_data[1];

                if(pf < 240)
                {
                    return (UInt32)((DataPage << 16) + (pf << 8));
                } else
                {
                    return (UInt32)((DataPage << 16) + (pf << 8) + ps);
                }
            }
            set
            {
                packet_data[1] = (byte)(value >> 8);
                packet_data[2] = (byte)value; 
            }
        }

        // 0 = high
        public byte Priority
        {
            get { return (byte) ((packet_data[3] & 0x1c) >> 2); }
            set { packet_data[3] = (byte)((packet_data[3] & (~0x1c)) | ((value<<2) & 0x1c)); }
        }

        public byte DataPage
        {
            get { return (byte)(packet_data[3] & 3); }
        }

        public string ToHex()
        {
            var ans = "";
            foreach (var b in packet_data.Reverse())
            {
                ans += b.ToString("X2");
            }

            return ans;
        }
    }
}