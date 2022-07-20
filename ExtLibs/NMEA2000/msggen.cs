using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace NMEA2000
{
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

        public static void run()
        {
            var pgns = JsonConvert.DeserializeObject<Pgns>(File.ReadAllText("Resources/pgns.json"), Converter.Settings);

            List<string> ans = new List<string>();

            ans.Add(@"using System;
using MissionPlanner.Utilities;
using System.Collections.Generic;

namespace NMEA2000
{
    public class NMEA2000msgs
    {
        public static readonly (int, Type, PgnType, int)[] msgs = {
");
            foreach (var pgn in pgns.PgNs)
                ans.Add(@"                (" + pgn.PgnPgn + ", typeof(" + pgn.Id + "), PgnType." + pgn.Type + ", " + pgn.Length + "),");

            ans.Add(@"};
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

                ans.Add("/// Description: " + desc);
                ans.Add("/// Type: " + type);
                ans.Add("/// PGNNO: " + pgnno);
                ans.Add("/// Length: " + length);
                ans.Add("public class " + pgn.Id + " : INMEA2000 {");
                ans.Add("    byte[] _packet;");
                ans.Add("    public void SetPacketData(byte[] packet) { _packet = packet; }");
                ans.Add("    public " + pgn.Id + "(byte[] packet) { _packet = packet; }");

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
                        ans.Add($"    public {ConvertTypeOUTPUT(a)} {ConvertName(a.Id)} {{ get {{ return _packet.GetBitOffsetLength<{ConvertType(a.Type, a.BitLength)}, {ConvertTypeOUTPUT(a)}>({a.BitStart}, {a.BitOffset}, {a.BitLength}, {a.Signed.ToString().ToLower()},  {a.Resolution ?? 0}); }} set {{ }}  }}");
                    });

                fields?
                    .Distinct(new MissionPlanner.Utilities.EqualityComparer<Field>((a, b) => a.Id == b.Id)).Where(a => a.Type == FieldType.LookupTable)
                    .ForEach(a =>
                    {
                        if (a.EnumValues == null)
                            return;
                        // name same as class
                        if (a.Id.Equals(pgn.Id))
                            a.Id += "_";

                        // ans.Add($"    public enum {ConvertName(a.Id)}_enum {{ " + $"{a.EnumValues.Aggregate("", (q, w) => q + "" + w.Name.Replace(" ", "_").Replace("/", "_").Replace("-", "minus").Replace("+", "plus").Replace("(", "_").Replace(")", "_").Replace("#", "_").Replace(".", "_").Replace(":", "_") + " = " + w.Value + ",\n")} }}");
                    });


                ans.Add("}");


            }

            ans.Add(@"
}
");

            File.WriteAllLines("..\\..\\..\\NMEA2000pgns.cs", ans);
        }

        public static string ConvertName(string no)
        {
            return no.Replace("event", "@event")
                .Replace("override", "@override")
                .Replace("1stTelecommand", "_1stTelecommand")
                .Replace("reserved", "@reserved")
                .Replace("unused", "@unused");
        }

        private static string ConvertType(FieldType? jToken, long bitwidth = 0)
        {
            if (jToken == null)
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
                    return "int";
                case FieldType.DecimalEncodedNumber:
                    return "decimal";
                case FieldType.IeeeFloat:
                    return "float";
                case FieldType.Integer:
                    return "int";
                case FieldType.Latitude:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.Longitude:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.LookupTable:
                    return "string";
                case FieldType.ManufacturerCode:
                    return "long";
                case FieldType.Pressure:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.PressureHires:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.StringWithStartStopByte:
                    return "string";
                case FieldType.Temperature:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.TemperatureHires:
                    if (bitwidth == 64)
                        return "long";
                    return "int";
                case FieldType.Time:
                    return "int";
            }

            throw new Exception();
        }

        private static string ConvertTypeOUTPUT(Field jToken)
        {
            if (jToken.Type == null)
            {
                if (jToken.Resolution == null)
                    if (jToken.BitLength <= 32)
                        return "int";
                    else
                        return "long";
                if (jToken.Resolution != 1 && jToken.Resolution != 0)
                    return "double";
                return "int";
            }

            switch (jToken.Type)
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
                    return "int";
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
                    return "int";
            }

            throw new Exception();
        }
    }
}