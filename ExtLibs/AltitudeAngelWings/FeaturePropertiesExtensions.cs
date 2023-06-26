using System;
using System.Globalization;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;

namespace AltitudeAngelWings
{
    public static class FeaturePropertiesExtensions
    {
        public static ColorInfo ToColorInfo(this FeatureProperties properties, float opacityAdjust = 1f)
        {
            var fillColor = ToARGB(properties.FillColor, properties.FillOpacity, opacityAdjust);
            var strokeColor = ToARGB(properties.StrokeColor, properties.StrokeOpacity, opacityAdjust);
            if (!int.TryParse(properties.StrokeWidth, out var strokeWidth))
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

        private static uint ToARGB(string color, string opacity = null, float opacityAdjust = 1f)
        {
            if (color.StartsWith("#"))
            {
                color = color.Substring(1);
            }
            else if (color.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                color = color.Substring(2);
            }

            byte opacityByte;
            switch (color.Length)
            {
                case 6:
                {
                    if (!float.TryParse(opacity, out var opacityAmount))
                    {
                        opacityAmount = 1f;
                    }

                    opacityByte = (byte)(opacityAmount * byte.MaxValue);
                    break;
                }
                case 8 when opacity != null:
                    throw new InvalidOperationException("Color cannot contain an ARGB value if the opacity is specified.");
                case 8:
                {
                    if (!byte.TryParse(color.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out opacityByte))
                    {
                        opacityByte = 0xFF;
                    }

                    color = color.Substring(2);
                    break;
                }
                default:
                    throw new ArgumentException(nameof(color), "Color must be either #AARRGGBB, #RRGGBB, 0xAARRGGBB or 0xRRGGBB");
            }

            opacityByte = (byte)Math.Min(opacityAdjust * opacityByte, byte.MaxValue);

            var colorArgb = (uint)opacityByte << 24;

            for (var i = 0; i < 6; i += 2)
            {
                byte.TryParse(color.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorByte);
                colorArgb |= (uint)colorByte << ((4 - i) / 2 * 8);
            }

            return colorArgb;
        }
    }
}
