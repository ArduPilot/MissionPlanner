using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using static MissionPlanner.Utilities.Mission.CommandUtils;

namespace MissionPlanner.Utilities.Mission
{
    /// <summary>
    /// Resolved visual properties for a single segment. Populated by applying
    /// style rules in order; later rules override earlier ones.
    /// </summary>
    public sealed class SegmentStyle
    {
        public Color StrokeColor { get; set; } = Color.Yellow;
        public float StrokeWidth { get; set; } = 3f;
        public DashStyle DashStyle { get; set; } = DashStyle.Solid;
        public bool ShowArrow { get; set; } = true;
    }

    /// <summary>
    /// Serializable style rule where null fields inherit from earlier rules.
    /// </summary>
    public sealed class SegmentStyleRuleConfig
    {
        [Category("Filter")]
        public string Description { get; set; }
        [Category("Filter")]
        public SegmentKind? KindFilter { get; set; }
        [Category("Filter")]
        public SegmentFlags? RequiredFlags { get; set; }
        [Category("Filter")]
        public SegmentFlags? ExcludedFlags { get; set; }

        [Category("Style")]
        public Color? StrokeColor { get; set; }
        [Category("Style")]
        public float? StrokeWidth { get; set; }
        [Category("Style")]
        public DashStyle? DashStyle { get; set; }
        [Category("Style")]
        public bool? ShowArrow { get; set; }

        public SegmentStyleRuleConfig Clone()
        {
            return (SegmentStyleRuleConfig)MemberwiseClone();
        }
    }

    /// <summary>
    /// Matches segments by kind/flags and applies style overrides from a
    /// <see cref="SegmentStyleRuleConfig"/>.
    /// </summary>
    public sealed class SegmentStyleRule
    {
        readonly SegmentStyleRuleConfig _config;

        public SegmentStyleRule(SegmentStyleRuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool Applies(SegmentKind kind, SegmentFlags flags)
        {
            if (_config.KindFilter.HasValue && kind != _config.KindFilter.Value)
                return false;

            if (_config.RequiredFlags.HasValue &&
                (flags & _config.RequiredFlags.Value) != _config.RequiredFlags.Value)
                return false;

            if (_config.ExcludedFlags.HasValue && (flags & _config.ExcludedFlags.Value) != 0)
                return false;

            return true;
        }

        public void Apply(SegmentStyle style)
        {
            if (_config.StrokeColor.HasValue) style.StrokeColor = _config.StrokeColor.Value;
            if (_config.StrokeWidth.HasValue) style.StrokeWidth = _config.StrokeWidth.Value;
            if (_config.DashStyle.HasValue) style.DashStyle = _config.DashStyle.Value;
            if (_config.ShowArrow.HasValue) style.ShowArrow = _config.ShowArrow.Value;
        }
    }

    /// <summary>
    /// Resolved visual properties for a single waypoint marker.
    /// </summary>
    public sealed class MarkerStyle
    {
        public GMarkerGoogleType MarkerType { get; set; } = GMarkerGoogleType.green;
        public Color CircleColor { get; set; } = Color.White;
    }

    /// <summary>
    /// Which commands a marker style rule targets: a predefined group
    /// (All, Loiter, Landing, etc.) or a custom list of command IDs.
    /// </summary>
    public enum MarkerFilter
    {
        All,
        SpecificIds,
        Home,
        Loiters,
        Landings,
        Bookmarks,
        RegionsOfInterest,
    }

    /// <summary>
    /// Serializable marker style rule. The PropertyGrid hides RawCommandIds
    /// unless Filter is SpecificIds.
    /// </summary>
    public sealed class MarkerStyleRuleConfig : ICustomTypeDescriptor
    {
        [Category("Filter")]
        public string Description { get; set; }
        [Category("Filter")]
        public MarkerFilter Filter { get; set; }
        [Category("Filter")]
        public ushort[] RawCommandIds { get; set; }

        [Category("Style")]
        public GMarkerGoogleType? MarkerType { get; set; }
        [Category("Style")]
        public Color? CircleColor { get; set; }

        public MarkerStyleRuleConfig Clone()
        {
            var clone = (MarkerStyleRuleConfig)MemberwiseClone();
            if (RawCommandIds != null)
                clone.RawCommandIds = (ushort[])RawCommandIds.Clone();
            return clone;
        }

        #region ICustomTypeDescriptor - hide RawCommandIds unless Filter == SpecificIds

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(this, true);
        string ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(this, true);
        string ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(this, true);
        TypeConverter ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(this, true);
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(this, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => ((ICustomTypeDescriptor)this).GetProperties(null);
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => this;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            var props = TypeDescriptor.GetProperties(this, attributes, true);
            if (Filter == MarkerFilter.SpecificIds)
                return props;

            var filtered = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor p in props)
            {
                if (p.Name != nameof(RawCommandIds))
                    filtered.Add(p);
            }
            return new PropertyDescriptorCollection(filtered.ToArray());
        }

        #endregion
    }

    public sealed class MarkerStyleRule
    {
        readonly MarkerStyleRuleConfig _config;

        public MarkerStyleRule(MarkerStyleRuleConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool Applies(ushort cmd)
        {
            switch (_config.Filter)
            {
                case MarkerFilter.All: return true;
                case MarkerFilter.SpecificIds:
                    return _config.RawCommandIds != null &&
                           Array.Exists(_config.RawCommandIds, id => id == cmd);
                case MarkerFilter.Home: return cmd == 0;
                case MarkerFilter.Loiters: return IsLoiter(cmd);
                case MarkerFilter.Landings: return IsLand(cmd);
                case MarkerFilter.Bookmarks: return IsBookmark(cmd);
                case MarkerFilter.RegionsOfInterest: return IsRegionOfInterest(cmd);
                default: return false;
            }
        }

        public void Apply(MarkerStyle style)
        {
            if (_config.MarkerType.HasValue) style.MarkerType = _config.MarkerType.Value;
            if (_config.CircleColor.HasValue) style.CircleColor = _config.CircleColor.Value;
        }
    }

    sealed class ColorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color) || objectType == typeof(Color?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var s = (string)reader.Value;
            if (s.StartsWith("#"))
                return Color.FromArgb(Convert.ToInt32(s.Substring(1), 16));
            return Color.FromName(s);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            if (color.IsNamedColor)
                writer.WriteValue(color.Name);
            else
                writer.WriteValue($"#{color.ToArgb():X8}");
        }
    }

    /// <summary>
    /// Ordered lists of segment and marker style rules, serialized as JSON.
    /// Later rules override earlier ones.
    /// </summary>
    public class MissionStyleConfig
    {
        public List<SegmentStyleRuleConfig> SegmentRules { get; set; } = new List<SegmentStyleRuleConfig>();
        public List<MarkerStyleRuleConfig> MarkerRules { get; set; } = new List<MarkerStyleRuleConfig>();

        public static MissionStyleConfig CreateDefault()
        {
            return new MissionStyleConfig
            {
                SegmentRules = new List<SegmentStyleRuleConfig>
                {
                    new SegmentStyleRuleConfig
                    {
                        Description = "Base defaults",
                        StrokeColor = Color.Yellow,
                        StrokeWidth = 3f,
                        DashStyle   = DashStyle.Solid,
                        ShowArrow   = true,
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Jumps/Alternates",
                        RequiredFlags = SegmentFlags.Alternate,
                        StrokeWidth   = 2f,
                        DashStyle     = DashStyle.Dash
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Unknown commands",
                        RequiredFlags = SegmentFlags.UnknownCommand,
                        StrokeColor   = Color.Red,
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Takeoff segments",
                        RequiredFlags = SegmentFlags.FromTakeoff,
                        StrokeWidth   = 2f,
                        DashStyle     = DashStyle.Dash
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Bookmark lines",
                        RequiredFlags = SegmentFlags.FromBookmark,
                        StrokeColor   = Color.Orange
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Return path",
                        RequiredFlags = SegmentFlags.ReturnPath,
                        StrokeColor   = Color.LightSkyBlue
                    },
                    new SegmentStyleRuleConfig
                    {
                        Description   = "Landing sequence",
                        RequiredFlags = SegmentFlags.LandSequence,
                        StrokeColor   = Color.FromArgb(180, 255, 0),
                    },
                },
                MarkerRules = new List<MarkerStyleRuleConfig>
                {
                    new MarkerStyleRuleConfig
                    {
                        Description = "Base defaults",
                        Filter      = MarkerFilter.All,
                        MarkerType  = GMarkerGoogleType.green,
                        CircleColor = Color.FromArgb(64, 255, 255, 255)
                    },
                    new MarkerStyleRuleConfig
                    {
                        Description = "Loiters",
                        Filter      = MarkerFilter.Loiters,
                        CircleColor = Color.Transparent
                    },
                    new MarkerStyleRuleConfig
                    {
                        Description = "Landings",
                        Filter      = MarkerFilter.Landings,
                        CircleColor = Color.Transparent
                    },
                    new MarkerStyleRuleConfig
                    {
                        Description = "Bookmarks",
                        Filter      = MarkerFilter.Bookmarks,
                        MarkerType  = GMarkerGoogleType.orange,
                        CircleColor = Color.Transparent
                    },
                    new MarkerStyleRuleConfig
                    {
                        Description = "ROI",
                        Filter      = MarkerFilter.RegionsOfInterest,
                        MarkerType  = GMarkerGoogleType.purple,
                        CircleColor = Color.Transparent
                    },
                }
            };
        }

        public MissionStyleConfig Clone()
        {
            return new MissionStyleConfig
            {
                SegmentRules = SegmentRules.Select(r => r.Clone()).ToList(),
                MarkerRules = MarkerRules.Select(r => r.Clone()).ToList(),
            };
        }
    }

    /// <summary>
    /// Resolves the final style for any segment or marker by applying
    /// rules from a <see cref="MissionStyleConfig"/> in order. Rules are
    /// compiled once at construction; this object is immutable.
    /// </summary>
    public sealed class MissionStyle
    {
        static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter(), new ColorJsonConverter() },
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        public MissionStyleConfig Config { get; }

        readonly List<SegmentStyleRule> _segmentRules = new List<SegmentStyleRule>();
        readonly List<MarkerStyleRule> _markerRules = new List<MarkerStyleRule>();

        public MissionStyle(MissionStyleConfig config = null)
        {
            Config = config ?? MissionStyleConfig.CreateDefault();
            foreach (var cfg in Config.SegmentRules)
                _segmentRules.Add(new SegmentStyleRule(cfg));
            foreach (var cfg in Config.MarkerRules)
                _markerRules.Add(new MarkerStyleRule(cfg));
        }

        public SegmentStyle GetSegmentStyle(MissionSegmentizer.Segment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            var style = new SegmentStyle();
            foreach (var rule in _segmentRules)
            {
                if (rule.Applies(segment.Kind, segment.Flags))
                    rule.Apply(style);
            }
            return style;
        }

        public MarkerStyle GetMarkerStyle(ushort cmd)
        {
            var style = new MarkerStyle();
            foreach (var rule in _markerRules)
            {
                if (rule.Applies(cmd))
                    rule.Apply(style);
            }
            return style;
        }

        /// <summary>
        /// Loads a <see cref="MissionStyleConfig"/> from a JSON file.
        /// Returns null if the path is empty, the file doesn't exist, or parsing fails.
        /// </summary>
        public static MissionStyleConfig LoadConfig(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<MissionStyleConfig>(json, JsonSettings);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveConfig(string path, MissionStyleConfig config)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var json = JsonConvert.SerializeObject(config, JsonSettings);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Loads a style from a JSON file, falling back to defaults if the path
        /// is empty or the file is missing/invalid.
        /// </summary>
        public static MissionStyle LoadFromConfig(string path)
        {
            return new MissionStyle(LoadConfig(path));
        }
    }
}
