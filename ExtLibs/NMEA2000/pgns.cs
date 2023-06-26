using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
    using System.Diagnostics;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

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


}