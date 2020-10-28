namespace component_information
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Schema for COMP_METADATA_TYPE_PARAMETER
    /// </summary>
    public partial class Parameter
    {
        [JsonProperty("parameters")]
        public ParametersUnion? Parameters { get; set; }

        /// <summary>
        /// Scope to which this metadata applies. Firmware: Any vehicle running this same vehicles
        /// firmware type. VehicleGroup: Any vehicle running this same firmware and this vehicles
        /// group type (Fixed Wing, Multi-Rotor, VTOL, Rover). VehicleType: Any vehicle match this
        /// vehicles firmware type and specific vehicle type. Vehicle: Only applies to this specific
        /// vehicle.
        /// </summary>
        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public Scope? Scope { get; set; }

        /// <summary>
        /// Unique id for this metadata. Same as ```COMPONENT_INFORMATION. comp_metadata_uid```.
        /// </summary>
        [JsonProperty("uid", NullValueHandling = NullValueHandling.Ignore)]
        public long? Uid { get; set; }

        /// <summary>
        /// Version number for the format of this file.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public long? Version { get; set; }
    }

    public partial class ParametersClass
    {
        /// <summary>
        /// User readable name for a 'type' of parameter. For example 'Developer', 'System', or
        /// 'Advanced'.
        /// </summary>
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        /// <summary>
        /// Number of decimal places to show for user facing display.
        /// </summary>
        [JsonProperty("decimalPlaces", NullValueHandling = NullValueHandling.Ignore)]
        public long? DecimalPlaces { get; set; }

        /// <summary>
        /// Default value for parameter.
        /// </summary>
        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public double? Default { get; set; }

        /// <summary>
        /// User readable name for a group of parameters which are commonly modified together. For
        /// example a GCS can shows params in a hierarchical display based on group
        /// </summary>
        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public string Group { get; set; }

        /// <summary>
        /// Increment to use for user facing UI which increments a value
        /// </summary>
        [JsonProperty("increment", NullValueHandling = NullValueHandling.Ignore)]
        public double? Increment { get; set; }

        /// <summary>
        /// Long user facing documentation of how the parameters works.
        /// </summary>
        [JsonProperty("longDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string LongDesc { get; set; }

        /// <summary>
        /// Maximum valid value
        /// </summary>
        [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
        public double? Max { get; set; }

        /// <summary>
        /// Minimum valid value
        /// </summary>
        [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
        public double? Min { get; set; }

        /// <summary>
        /// Parameter Name.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// true: Vehicle must be rebooted if this value is changed
        /// </summary>
        [JsonProperty("rebootRequired", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RebootRequired { get; set; }

        /// <summary>
        /// Short user facing description/name for parameter. Used in UI intead of internal parameter
        /// name.
        /// </summary>
        [JsonProperty("shortDesc", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortDesc { get; set; }

        /// <summary>
        /// Parameter type.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public TypeEnum? Type { get; set; }

        /// <summary>
        /// Units for parameter value.
        /// </summary>
        [JsonProperty("units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; }

        /// <summary>
        /// Array of values and textual descriptions for use by GCS ui.
        /// </summary>
        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> Values { get; set; }

        /// <summary>
        /// true: value is volatile. Should not be included in creation of a CRC over param values
        /// for example.
        /// </summary>
        [JsonProperty("volatile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Volatile { get; set; }
    }

    /// <summary>
    /// Parameter type.
    /// </summary>
    public enum TypeEnum { Float, Int16, Int32, Int8, Uint16, Uint32, Uint8 };

    /// <summary>
    /// Scope to which this metadata applies. Firmware: Any vehicle running this same vehicles
    /// firmware type. VehicleGroup: Any vehicle running this same firmware and this vehicles
    /// group type (Fixed Wing, Multi-Rotor, VTOL, Rover). VehicleType: Any vehicle match this
    /// vehicles firmware type and specific vehicle type. Vehicle: Only applies to this specific
    /// vehicle.
    /// </summary>
    public enum Scope { Firmware, Vehicle, VehicleGroup, VehicleType };

    public partial struct ParametersUnion
    {
        public List<dynamic> AnythingArray;
        public bool? Bool;
        public double? Double;
        public long? Integer;
        public ParametersClass ParametersClass;
        public string String;

        public static implicit operator ParametersUnion(List<dynamic> AnythingArray) => new ParametersUnion { AnythingArray = AnythingArray };
        public static implicit operator ParametersUnion(bool Bool) => new ParametersUnion { Bool = Bool };
        public static implicit operator ParametersUnion(double Double) => new ParametersUnion { Double = Double };
        public static implicit operator ParametersUnion(long Integer) => new ParametersUnion { Integer = Integer };
        public static implicit operator ParametersUnion(ParametersClass ParametersClass) => new ParametersUnion { ParametersClass = ParametersClass };
        public static implicit operator ParametersUnion(string String) => new ParametersUnion { String = String };
        public bool IsNull => AnythingArray == null && Bool == null && ParametersClass == null && Double == null && Integer == null && String == null;
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ParametersUnionConverter.Singleton,
                TypeEnumConverter.Singleton,
                ScopeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParametersUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ParametersUnion) || t == typeof(ParametersUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new ParametersUnion { };
                case JsonToken.Integer:
                    var integerValue = serializer.Deserialize<long>(reader);
                    return new ParametersUnion { Integer = integerValue };
                case JsonToken.Float:
                    var doubleValue = serializer.Deserialize<double>(reader);
                    return new ParametersUnion { Double = doubleValue };
                case JsonToken.Boolean:
                    var boolValue = serializer.Deserialize<bool>(reader);
                    return new ParametersUnion { Bool = boolValue };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ParametersUnion { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ParametersClass>(reader);
                    return new ParametersUnion { ParametersClass = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<dynamic>>(reader);
                    return new ParametersUnion { AnythingArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ParametersUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ParametersUnion)untypedValue;
            if (value.IsNull)
            {
                serializer.Serialize(writer, null);
                return;
            }
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value);
                return;
            }
            if (value.Double != null)
            {
                serializer.Serialize(writer, value.Double.Value);
                return;
            }
            if (value.Bool != null)
            {
                serializer.Serialize(writer, value.Bool.Value);
                return;
            }
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.AnythingArray != null)
            {
                serializer.Serialize(writer, value.AnythingArray);
                return;
            }
            if (value.ParametersClass != null)
            {
                serializer.Serialize(writer, value.ParametersClass);
                return;
            }
            throw new Exception("Cannot marshal type ParametersUnion");
        }

        public static readonly ParametersUnionConverter Singleton = new ParametersUnionConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Float":
                    return TypeEnum.Float;
                case "Int16":
                    return TypeEnum.Int16;
                case "Int32":
                    return TypeEnum.Int32;
                case "Int8":
                    return TypeEnum.Int8;
                case "Uint16":
                    return TypeEnum.Uint16;
                case "Uint32":
                    return TypeEnum.Uint32;
                case "Uint8":
                    return TypeEnum.Uint8;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Float:
                    serializer.Serialize(writer, "Float");
                    return;
                case TypeEnum.Int16:
                    serializer.Serialize(writer, "Int16");
                    return;
                case TypeEnum.Int32:
                    serializer.Serialize(writer, "Int32");
                    return;
                case TypeEnum.Int8:
                    serializer.Serialize(writer, "Int8");
                    return;
                case TypeEnum.Uint16:
                    serializer.Serialize(writer, "Uint16");
                    return;
                case TypeEnum.Uint32:
                    serializer.Serialize(writer, "Uint32");
                    return;
                case TypeEnum.Uint8:
                    serializer.Serialize(writer, "Uint8");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class ScopeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Scope) || t == typeof(Scope?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Firmware":
                    return Scope.Firmware;
                case "Vehicle":
                    return Scope.Vehicle;
                case "VehicleGroup":
                    return Scope.VehicleGroup;
                case "VehicleType":
                    return Scope.VehicleType;
            }
            throw new Exception("Cannot unmarshal type Scope");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Scope)untypedValue;
            switch (value)
            {
                case Scope.Firmware:
                    serializer.Serialize(writer, "Firmware");
                    return;
                case Scope.Vehicle:
                    serializer.Serialize(writer, "Vehicle");
                    return;
                case Scope.VehicleGroup:
                    serializer.Serialize(writer, "VehicleGroup");
                    return;
                case Scope.VehicleType:
                    serializer.Serialize(writer, "VehicleType");
                    return;
            }
            throw new Exception("Cannot marshal type Scope");
        }

        public static readonly ScopeConverter Singleton = new ScopeConverter();
    }
}
