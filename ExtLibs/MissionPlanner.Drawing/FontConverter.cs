using System.ComponentModel;
using System.Globalization;

namespace System.Drawing
{
    /// <summary>
    /// Minimal <see cref="Font"/> converter so fonts serialized as strings in WinForms .resx
    /// designer resources (e.g. "Microsoft Sans Serif, 8.25pt, style=Bold") deserialize under
    /// the SkiaSharp shim instead of throwing "TypeConverter cannot convert from System.String".
    /// </summary>
    public class FontConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s && s.Trim().Length > 0)
            {
                if (culture == null)
                    culture = CultureInfo.CurrentCulture;

                char sep = culture.TextInfo.ListSeparator.Length > 0 ? culture.TextInfo.ListSeparator[0] : ',';
                string[] parts = s.Split(sep);

                string family = parts.Length > 0 ? parts[0].Trim() : "Microsoft Sans Serif";
                if (family.Length == 0)
                    family = "Microsoft Sans Serif";

                float size = 8.25f;
                if (parts.Length > 1)
                {
                    string sizePart = parts[1].Trim();
                    int i = 0;
                    while (i < sizePart.Length &&
                           (char.IsDigit(sizePart[i]) || sizePart[i] == '.' || sizePart[i] == ','))
                        i++;
                    if (i > 0 && float.TryParse(sizePart.Substring(0, i), NumberStyles.Float, culture, out float parsed) && parsed > 0)
                        size = parsed;
                }

                FontStyle style = FontStyle.Regular;
                for (int p = 2; p < parts.Length; p++)
                {
                    string sp = parts[p].Trim();
                    if (sp.StartsWith("style=", StringComparison.OrdinalIgnoreCase))
                        sp = sp.Substring("style=".Length);

                    foreach (var token in sp.Split(' '))
                    {
                        switch (token.Trim().ToLowerInvariant())
                        {
                            case "bold": style |= FontStyle.Bold; break;
                            case "italic": style |= FontStyle.Italic; break;
                            case "underline": style |= FontStyle.Underline; break;
                            case "strikeout": style |= FontStyle.Strikeout; break;
                        }
                    }
                }

                return new Font(family, size, style, GraphicsUnit.Point);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
