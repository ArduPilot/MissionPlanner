using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using SharpCompress.Compressors.Xz;

namespace MissionPlanner.ArduPilot.Mavlink
{
    /// <summary>MAVLink Camera Definition File metadata and byte-wise PARAM_EXT encoding.</summary>
    public sealed class CameraDefinition
    {
        public const int MaxSize = 4 * 1024 * 1024;
        public const int MaxParameters = 512;
        public const int MaxOptions = 512;
        public Dictionary<string, CameraParameter> Parameters { get; } = new Dictionary<string, CameraParameter>();
        public string Model { get; }

        public CameraDefinition(byte[] data)
        {
            if (data.Length > MaxSize) throw new InvalidDataException("Camera definition is too large");
            using (var input = new MemoryStream(data))
            {
                if (data.Take(6).SequenceEqual(new byte[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0 }))
                {
                    using (var xz = new XZStream(input)) data = ReadLimited(xz);
                }
            }
            XDocument document;
            using (var input = new MemoryStream(data))
            using (var reader = XmlReader.Create(input, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = MaxSize
            })) document = XDocument.Load(reader);
            var root = document.Root;
            if (root?.Name != "mavlinkcamera" || root.Element("definition") == null)
                throw new InvalidDataException("Not a MAVLink camera definition");
            Model = (string)root.Element("definition").Element("model") ?? "Camera";
            var locales = root.Element("localization")?.Elements("locale").ToList() ?? new List<XElement>();
            var language = CultureInfo.CurrentUICulture.Name.Replace('-', '_');
            var locale = locales.FirstOrDefault(l => string.Equals((string)l.Attribute("name"), language, StringComparison.OrdinalIgnoreCase))
                ?? locales.FirstOrDefault(l => ((string)l.Attribute("name") ?? "").Split('_')[0].Equals(language.Split('_')[0], StringComparison.OrdinalIgnoreCase));
            var translations = new Dictionary<string, string>();
            if (locale != null)
                foreach (var s in locale.Elements("strings"))
                    translations[(string)s.Attribute("original") ?? ""] = (string)s.Attribute("translated") ?? "";
            Func<string, string> translate = s => translations.TryGetValue(s, out var value) ? value : s;
            foreach (var element in root.Element("parameters")?.Elements("parameter") ?? Enumerable.Empty<XElement>())
            {
                if (Parameters.Count >= MaxParameters) throw new InvalidDataException("Camera definition has too many parameters");
                var parameter = new CameraParameter(element, translate);
                if (Parameters.ContainsKey(parameter.Name)) throw new InvalidDataException("Duplicate parameter " + parameter.Name);
                Parameters.Add(parameter.Name, parameter);
            }
            foreach (var parameter in Parameters.Values)
            foreach (var range in parameter.Options.SelectMany(o => o.Ranges))
                if (Parameters.TryGetValue(range.Target, out var target) && target.WireType != 11)
                    foreach (var option in range.Options) option.Value = target.ConvertValue((string)option.Value);
        }

        public static byte[] ReadLimited(Stream stream, Action<int> progress = null)
        {
            using (var output = new MemoryStream())
            {
                var buffer = new byte[8192];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (output.Length + count > MaxSize) throw new InvalidDataException("Camera definition is too large");
                    output.Write(buffer, 0, count);
                    progress?.Invoke((int)output.Length);
                }
                return output.ToArray();
            }
        }

        public Dictionary<string, List<CameraParameter.Option>> Controls(IDictionary<string, object> values)
        {
            var excluded = new HashSet<string>();
            var limited = new HashSet<string>();
            var choices = Parameters.ToDictionary(p => p.Key, p => p.Value.Options);
            foreach (var p in Parameters.Values)
            {
                if (!values.TryGetValue(p.Name, out var value)) continue;
                foreach (var option in p.Options.Where(o => Equals(o.Value, value)))
                {
                    excluded.UnionWith(option.Exclusions);
                    foreach (var range in option.Ranges)
                        if (!limited.Contains(range.Target) && Condition(range.Condition, values))
                        {
                            choices[range.Target] = range.Options;
                            limited.Add(range.Target);
                        }
                }
            }
            return Parameters.Values.Where(p => p.Control && !excluded.Contains(p.Name))
                .ToDictionary(p => p.Name, p => choices[p.Name]);
        }

        private bool Condition(string condition, IDictionary<string, object> values)
        {
            if (string.IsNullOrWhiteSpace(condition)) return true;
            var parts = Regex.Split(condition.Trim(), @"\s+(AND|OR)\s+", RegexOptions.IgnoreCase);
            bool result = false;
            for (int i = 0; i < parts.Length; i += 2)
            {
                var match = Regex.Match(parts[i], @"^\s*([A-Za-z0-9_]+)\s*(!=|==|>=|<=|=|>|<)\s*(\S+)\s*$");
                var name = match.Groups[1].Value;
                if (!match.Success || !values.TryGetValue(name, out var value) || !Parameters.TryGetValue(name, out var p)) return false;
                int comparison;
                try { comparison = ((IComparable)value).CompareTo(p.ConvertValue(match.Groups[3].Value)); }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentException) { return false; }
                bool test;
                switch (match.Groups[2].Value)
                {
                    case "=": case "==": test = comparison == 0; break;
                    case "!=": test = comparison != 0; break;
                    case ">": test = comparison > 0; break;
                    case "<": test = comparison < 0; break;
                    case ">=": test = comparison >= 0; break;
                    default: test = comparison <= 0; break;
                }
                result = i == 0 ? test : parts[i - 1].Equals("AND", StringComparison.OrdinalIgnoreCase) ? result && test : result || test;
            }
            return result;
        }
    }

    public sealed class CameraParameter
    {
        public sealed class Option
        {
            public string Label;
            public object Value;
            public List<string> Exclusions = new List<string>();
            public List<Range> Ranges = new List<Range>();
            public override string ToString() => Label;
        }
        public sealed class Range
        {
            public string Target, Condition;
            public List<Option> Options = new List<Option>();
        }

        public string Name { get; }
        public string Description { get; }
        public string Type { get; }
        public byte WireType { get; }
        public bool Control { get; }
        public bool ReadOnly { get; }
        public bool WriteOnly { get; }
        public object Minimum { get; }
        public object Maximum { get; }
        public string Step { get; }
        public List<Option> Options { get; } = new List<Option>();
        public List<string> Updates { get; }
        private static readonly string[] Types = { "uint8", "int8", "uint16", "int16", "uint32", "int32", "uint64", "int64", "float", "double", "custom" };
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        internal CameraParameter(XElement element, Func<string, string> translate)
        {
            Name = (string)element.Attribute("name") ?? "";
            if (Name.Length == 0 || Name.Length > 16 || Name.Any(c => c == 0 || c > 127))
                throw new InvalidDataException("Invalid camera parameter name");
            Type = ((string)element.Attribute("type") ?? "").ToLowerInvariant();
            WireType = (byte)(Array.IndexOf(Types, Type == "bool" ? "uint8" : Type) + 1);
            if (WireType == 0) throw new InvalidDataException("Unsupported parameter type: " + Type);
            Description = translate(((string)element.Element("description") ?? Name).Trim());
            Control = !Flag(element, "control", false) && WireType != 11;
            ReadOnly = Flag(element, "readonly", true);
            WriteOnly = Flag(element, "writeonly", true);
            Updates = element.Element("updates")?.Elements("update").Select(e => e.Value.Trim()).ToList() ?? new List<string>();
            if (WireType == 11) return; // Vendor-specific binary structures require a custom controller.
            if (element.Attribute("min") != null) Minimum = ConvertValue((string)element.Attribute("min"));
            if (element.Attribute("max") != null) Maximum = ConvertValue((string)element.Attribute("max"));
            if (Minimum != null && Maximum != null && ((IComparable)Minimum).CompareTo(Maximum) > 0)
                throw new InvalidDataException("Invalid limits for " + Name);
            Step = (string)element.Attribute("step");
            if (Step != null && (!(double.Parse(Step, Invariant) > 0) || double.IsInfinity(double.Parse(Step, Invariant))))
                throw new InvalidDataException("Invalid step for " + Name);
            foreach (var o in element.Element("options")?.Elements("option") ?? Enumerable.Empty<XElement>())
            {
                if (Options.Count >= CameraDefinition.MaxOptions) throw new InvalidDataException("Too many options for " + Name);
                var option = new Option { Label = translate((string)o.Attribute("name") ?? ""), Value = ConvertValue((string)o.Attribute("value")) };
                option.Exclusions.AddRange(o.Descendants("exclude").Select(e => e.Value.Trim()));
                foreach (var r in o.Element("parameterranges")?.Elements("parameterrange") ?? Enumerable.Empty<XElement>())
                {
                    if (r.Elements("roption").Take(CameraDefinition.MaxOptions + 1).Count() > CameraDefinition.MaxOptions)
                        throw new InvalidDataException("Too many range options for " + Name);
                    option.Ranges.Add(new Range
                    {
                        Target = (string)r.Attribute("parameter") ?? "", Condition = (string)r.Attribute("condition") ?? "",
                        Options = r.Elements("roption").Select(e => new Option
                        { Label = translate((string)e.Attribute("name") ?? ""), Value = (string)e.Attribute("value") }).ToList()
                    });
                }
                Options.Add(option);
            }
            if (Type == "bool" && Options.Count == 0)
            {
                Options.Add(new Option { Label = translate("Off"), Value = (byte)0 });
                Options.Add(new Option { Label = translate("On"), Value = (byte)1 });
            }
        }

        private static bool Flag(XElement e, string name, bool expected)
        {
            var value = ((string)e.Attribute(name) ?? "").ToLowerInvariant();
            return expected ? value == "1" || value == "true" : value == "0" || value == "false";
        }

        public object ConvertValue(string text)
        {
            object value;
            switch (WireType)
            {
                case 1: value = byte.Parse(text, Invariant); break;
                case 2: value = sbyte.Parse(text, Invariant); break;
                case 3: value = ushort.Parse(text, Invariant); break;
                case 4: value = short.Parse(text, Invariant); break;
                case 5: value = uint.Parse(text, Invariant); break;
                case 6: value = int.Parse(text, Invariant); break;
                case 7: value = ulong.Parse(text, Invariant); break;
                case 8: value = long.Parse(text, Invariant); break;
                case 9: value = float.Parse(text, Invariant); break;
                case 10: value = double.Parse(text, Invariant); break;
                default: throw new ArgumentException("Custom parameter has no numeric editor");
            }
            if (WireType >= 9 && (double.IsNaN(Convert.ToDouble(value)) || double.IsInfinity(Convert.ToDouble(value))))
                throw new ArgumentException("Value must be finite");
            if (Type == "bool" && (byte)value > 1) throw new ArgumentException("Boolean must be 0 or 1");
            return value;
        }

        public object Validate(string text, List<Option> options)
        {
            var value = ConvertValue(text);
            if (Minimum != null && ((IComparable)value).CompareTo(Minimum) < 0) throw new ArgumentException("Minimum: " + Minimum);
            if (Maximum != null && ((IComparable)value).CompareTo(Maximum) > 0) throw new ArgumentException("Maximum: " + Maximum);
            if ((Options.Count > 0 || options.Count > 0) && !options.Any(o => Equals(o.Value, value)))
                throw new ArgumentException("Value is not an available option");
            if (Step != null && Minimum != null)
            {
                bool valid;
                if (WireType <= 8) valid = (Convert.ToDecimal(value) - Convert.ToDecimal(Minimum)) % decimal.Parse(Step, Invariant) == 0;
                else
                {
                    double v = Convert.ToDouble(value), min = Convert.ToDouble(Minimum), step = double.Parse(Step, Invariant);
                    double offset = Math.Round((v - min) / step) * step;
                    // Account for wire rounding without accepting adjacent float options as equal.
                    double tolerance = Math.Max(Math.Abs(v), Math.Abs(min)) * (WireType == 9 ? 1.2e-7 : 4.5e-16);
                    valid = Math.Abs(v - (min + offset)) <= Math.Max(tolerance, WireType == 9 ? 1.4e-45 : double.Epsilon);
                }
                if (!valid) throw new ArgumentException("Value must use steps of " + Step);
            }
            return value;
        }

        public byte[] Encode(object value)
        {
            byte[] bytes;
            switch (WireType)
            {
                case 1: bytes = new[] { (byte)value }; break;
                case 2: bytes = new[] { unchecked((byte)(sbyte)value) }; break;
                case 3: bytes = BitConverter.GetBytes((ushort)value); break;
                case 4: bytes = BitConverter.GetBytes((short)value); break;
                case 5: bytes = BitConverter.GetBytes((uint)value); break;
                case 6: bytes = BitConverter.GetBytes((int)value); break;
                case 7: bytes = BitConverter.GetBytes((ulong)value); break;
                case 8: bytes = BitConverter.GetBytes((long)value); break;
                case 9: bytes = BitConverter.GetBytes((float)value); break;
                case 10: bytes = BitConverter.GetBytes((double)value); break;
                default: throw new ArgumentException("Unsupported parameter type");
            }
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            var result = new byte[128];
            Array.Copy(bytes, result, bytes.Length);
            return result;
        }

        public object Decode(byte[] bytes, byte type)
        {
            if (type != WireType || bytes == null || bytes.Length != 128) throw new ArgumentException("Unexpected parameter type or size");
            if (WireType == 11) return bytes;
            using (var reader = new BinaryReader(new MemoryStream(bytes)))
            {
                object value;
                switch (WireType)
                {
                    case 1: value = reader.ReadByte(); break;
                    case 2: value = reader.ReadSByte(); break;
                    case 3: value = reader.ReadUInt16(); break;
                    case 4: value = reader.ReadInt16(); break;
                    case 5: value = reader.ReadUInt32(); break;
                    case 6: value = reader.ReadInt32(); break;
                    case 7: value = reader.ReadUInt64(); break;
                    case 8: value = reader.ReadInt64(); break;
                    case 9: value = reader.ReadSingle(); break;
                    default: value = reader.ReadDouble(); break;
                }
                return ConvertValue(Format(value));
            }
        }

        public static string Format(object value) => value is float f ? f.ToString("R", Invariant)
            : value is double d ? d.ToString("R", Invariant) : Convert.ToString(value, Invariant);
        public byte[] Id => Encoding.ASCII.GetBytes(Name.PadRight(16, '\0'));
    }
}
