namespace MissionPlanner.Utilities.Drawing
{
    public enum PixelFormat
    {
        /// <summary>The pixel data contains color-indexed values, which means the values are an index to colors in the system color table, as opposed to individual color values.</summary>
        Indexed = 0x10000,
        /// <summary>The pixel data contains GDI colors.</summary>
        Gdi = 0x20000,
        /// <summary>The pixel data contains alpha values that are not premultiplied.</summary>
        Alpha = 0x40000,
        /// <summary>The pixel format contains premultiplied alpha values.</summary>
        PAlpha = 0x80000,
        /// <summary>Reserved.</summary>
        Extended = 0x100000,
        /// <summary>The default pixel format of 32 bits per pixel. The format specifies 24-bit color depth and an 8-bit alpha channel.</summary>
        Canonical = 0x200000,
        /// <summary>The pixel format is undefined.</summary>
        Undefined = 0,
        /// <summary>No pixel format is specified.</summary>
        DontCare = 0,
        /// <summary>Specifies that the pixel format is 1 bit per pixel and that it uses indexed color. The color table therefore has two colors in it.</summary>
        Format1bppIndexed = 196865,
        /// <summary>Specifies that the format is 4 bits per pixel, indexed.</summary>
        Format4bppIndexed = 197634,
        /// <summary>Specifies that the format is 8 bits per pixel, indexed. The color table therefore has 256 colors in it.</summary>
        Format8bppIndexed = 198659,
        /// <summary>The pixel format is 16 bits per pixel. The color information specifies 65536 shades of gray.</summary>
        Format16bppGrayScale = 1052676,
        /// <summary>Specifies that the format is 16 bits per pixel; 5 bits each are used for the red, green, and blue components. The remaining bit is not used.</summary>
        Format16bppRgb555 = 135173,
        /// <summary>Specifies that the format is 16 bits per pixel; 5 bits are used for the red component, 6 bits are used for the green component, and 5 bits are used for the blue component.</summary>
        Format16bppRgb565 = 135174,
        /// <summary>The pixel format is 16 bits per pixel. The color information specifies 32,768 shades of color, of which 5 bits are red, 5 bits are green, 5 bits are blue, and 1 bit is alpha.</summary>
        Format16bppArgb1555 = 397319,
        /// <summary>Specifies that the format is 24 bits per pixel; 8 bits each are used for the red, green, and blue components.</summary>
        Format24bppRgb = 137224,
        /// <summary>Specifies that the format is 32 bits per pixel; 8 bits each are used for the red, green, and blue components. The remaining 8 bits are not used.</summary>
        Format32bppRgb = 139273,
        /// <summary>Specifies that the format is 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue components.</summary>
        Format32bppArgb = 2498570,
        /// <summary>Specifies that the format is 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue components. The red, green, and blue components are premultiplied, according to the alpha component.</summary>
        Format32bppPArgb = 925707,
        /// <summary>Specifies that the format is 48 bits per pixel; 16 bits each are used for the red, green, and blue components.</summary>
        Format48bppRgb = 1060876,
        /// <summary>Specifies that the format is 64 bits per pixel; 16 bits each are used for the alpha, red, green, and blue components.</summary>
        Format64bppArgb = 3424269,
        /// <summary>Specifies that the format is 64 bits per pixel; 16 bits each are used for the alpha, red, green, and blue components. The red, green, and blue components are premultiplied according to the alpha component.</summary>
        Format64bppPArgb = 1851406,
        /// <summary>The maximum value for this enumeration.</summary>
        Max = 0xF
    }
}