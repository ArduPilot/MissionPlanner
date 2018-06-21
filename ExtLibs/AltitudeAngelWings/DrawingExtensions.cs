using System;
using System.Globalization;
using AltitudeAngelWings.Extra;
using GeoJSON.Net.Feature;

namespace AltitudeAngelWings
{
    public static class DrawingExtensions
    {
        public static ColorInfo ToColorInfo(this Feature feature)
        {
            uint fillColor = ToARGB((string)feature.Properties.Get("fillColor"), (string)feature.Properties.Get("fillOpacity"));
            uint strokeColor = ToARGB((string)feature.Properties.Get("strokeColor"), (string)feature.Properties.Get("strokeOpacity"));
            int strokeWidth;
            if (!int.TryParse((string)feature.Properties["strokeWidth"], out strokeWidth))
            {
                strokeWidth = 1;
            }

            return new ColorInfo
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeWidth = strokeWidth
            };
        }

        public static uint ToARGB(string color, string opacity = null)
        {
            byte opacityByte;
            uint colorARGB = 0x00;

            if (color.StartsWith("#"))
            {
                color = color.Substring(1);
            }
            else if (color.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                color = color.Substring(2);
            }

            if (color.Length == 6)
            {
                float opacityAmount;
                if (!float.TryParse(opacity, out opacityAmount))
                {
                    opacityAmount = 1F;
                }

                opacityByte = (byte)(opacityAmount * 100);
            }
            else if (color.Length == 8)
            {
                if (opacity != null)
                {
                    throw new InvalidOperationException("Color cannot contain an ARGB value if the opacity is specified.");
                }

                if (!byte.TryParse(color.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out opacityByte))
                {
                    opacityByte = 0xFF;
                }

                color = color.Substring(2);
            }
            else
            {
                throw new ArgumentException(nameof(color), "Color must be either #AARRGGBB, #RRGGBB, 0xAARRGGBB or 0xRRGGBB");
            }

            colorARGB = (uint)opacityByte << 24;

            for (int i = 0; i < 6; i += 2)
            {
                byte colorByte;
                byte.TryParse(color.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out colorByte);
                colorARGB |= (uint)colorByte << (((4 - i) / 2) * 8);
            }

            return colorARGB;
        }
    }
}
