using System.ComponentModel;
using System.Globalization;

namespace System.Drawing
{
    /// <summary>
    /// Parses the resx font form "Name, 8.25pt[, style=Bold, Italic]", for preserialized resx
    /// resources (see ImageConverter).
    /// </summary>
    public class FontConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string text))
                return base.ConvertFrom(context, culture, value);

            text = text.Trim();
            if (text.Length == 0)
                return null;

            var parts = text.Split(',');
            var name = parts[0].Trim();
            float size = 8.25f;
            var unit = GraphicsUnit.Point;
            var style = FontStyle.Regular;
            var inStyle = false;

            for (int i = 1; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (part.StartsWith("style=", StringComparison.OrdinalIgnoreCase))
                {
                    inStyle = true;
                    part = part.Substring(6).Trim();
                }
                else if (inStyle && part.Contains("="))
                {
                    // another named field (e.g. GdiCharSet=0) ends the style list
                    inStyle = false;
                }

                if (inStyle)
                {
                    if (Enum.TryParse(part, true, out FontStyle flag))
                        style |= flag;
                }
                else if (i == 1)
                {
                    ParseSize(part, ref size, ref unit);
                }
            }

            return new Font(name, size, style, unit);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string) && value is Font font)
            {
                var text = string.Format(CultureInfo.InvariantCulture, "{0}, {1}{2}", font.Name, font.Size,
                    UnitSuffix(font.Unit));
                if (font.Style != FontStyle.Regular)
                    text += ", style=" + font.Style;
                return text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static void ParseSize(string part, ref float size, ref GraphicsUnit unit)
        {
            var digits = part.Length;
            while (digits > 0 && char.IsLetter(part[digits - 1]))
                digits--;

            switch (part.Substring(digits).ToLowerInvariant())
            {
                case "px": unit = GraphicsUnit.Pixel; break;
                case "in": unit = GraphicsUnit.Inch; break;
                case "mm": unit = GraphicsUnit.Millimeter; break;
                case "world": unit = GraphicsUnit.World; break;
                case "display": unit = GraphicsUnit.Display; break;
                case "doc": unit = GraphicsUnit.Document; break;
                default: unit = GraphicsUnit.Point; break;
            }

            if (float.TryParse(part.Substring(0, digits), NumberStyles.Float, CultureInfo.InvariantCulture,
                    out var parsed))
                size = parsed;
        }

        private static string UnitSuffix(GraphicsUnit unit)
        {
            switch (unit)
            {
                case GraphicsUnit.Pixel: return "px";
                case GraphicsUnit.Inch: return "in";
                case GraphicsUnit.Millimeter: return "mm";
                case GraphicsUnit.World: return "world";
                case GraphicsUnit.Display: return "display";
                case GraphicsUnit.Document: return "doc";
                default: return "pt";
            }
        }
    }
}
