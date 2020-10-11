using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using SkiaSharp;

namespace System.Drawing
{
    [Serializable]
    public class Font : ISerializable, IDisposable, ICloneable
    {
        internal SKPaint nativeFont;

        /// <summary>Gets style information for this <see cref="T:System.Drawing.Font" />.</summary>
        /// <returns>A <see cref="T:System.Drawing.FontStyle" /> enumeration that contains style information for this <see cref="T:System.Drawing.Font" />.</returns>
        [Browsable(false)]
        public FontStyle Style { get; set; }

        private Font(SerializationInfo info, StreamingContext context)
        {
            string familyName = null;
            float emSize = -1f;
            FontStyle style = FontStyle.Regular;
            GraphicsUnit unit = GraphicsUnit.Point;
            SingleConverter singleConverter = new SingleConverter();
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (string.Equals(enumerator.Name, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    familyName = (string)enumerator.Value;
                }
                else if (string.Equals(enumerator.Name, "Size", StringComparison.OrdinalIgnoreCase))
                {
                    emSize = ((!(enumerator.Value is string)) ? ((float)enumerator.Value) : ((float)singleConverter.ConvertFrom(enumerator.Value)));
                }
                else if (string.Compare(enumerator.Name, "Style", ignoreCase: true, CultureInfo.InvariantCulture) == 0)
                {
                    style = (FontStyle)enumerator.Value;
                }
                else if (string.Compare(enumerator.Name, "Unit", ignoreCase: true, CultureInfo.InvariantCulture) == 0)
                {
                    unit = (GraphicsUnit)enumerator.Value;
                }
            }
            Initialize(familyName, emSize, style, unit, 1, IsVerticalName(familyName));
        }

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("Name", Name);
            si.AddValue("Size", Size);
            si.AddValue("Style", Style);
            si.AddValue("Unit", Unit);
        }

        private byte gdiCharSet = 1;

        private bool gdiVerticalFont;


        /// <summary>Returns a human-readable string representation of this <see cref="T:System.Drawing.Font" />.</summary>
        /// <returns>A string that represents this <see cref="T:System.Drawing.Font" />.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}, Size={2}, Units={3}, GdiCharSet={4}, GdiVerticalFont={5}]", GetType().Name, FontFamily.Name, Size, (int)Unit, gdiCharSet, gdiVerticalFont);
        }


        private static bool IsVerticalName(string familyName)
        {
            if (familyName != null && familyName.Length > 0)
            {
                return familyName[0] == '@';
            }

            return false;
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> that uses the specified existing <see cref="T:System.Drawing.Font" /> and <see cref="T:System.Drawing.FontStyle" /> enumeration.</summary>
        /// <param name="prototype">The existing <see cref="T:System.Drawing.Font" /> from which to create the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="newStyle">The <see cref="T:System.Drawing.FontStyle" /> to apply to the new <see cref="T:System.Drawing.Font" />. Multiple values of the <see cref="T:System.Drawing.FontStyle" /> enumeration can be combined with the <see langword="OR" /> operator.</param>
        public Font(Font prototype, FontStyle newStyle)
        {
            Initialize(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, 1, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, and unit.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="family" /> is <see langword="null" />.</exception>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
        {
            Initialize(family, emSize, style, unit, 1, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a  
        ///  GDI character set to use for the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="family" /> is <see langword="null" />.</exception>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
        {
            Initialize(family, emSize, style, unit, gdiCharSet, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a  
        ///  GDI character set to use for this font.</param>
        /// <param name="gdiVerticalFont">A Boolean value indicating whether the new font is derived from a GDI vertical font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="family" /> is <see langword="null" /></exception>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet,
            bool gdiVerticalFont)
        {
            Initialize(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, unit, and character set.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a GDI character set to use for this font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
        {
            Initialize(familyName, emSize, style, unit, gdiCharSet, IsVerticalName(familyName));
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using the specified size, style, unit, and character set.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <param name="gdiCharSet">A <see cref="T:System.Byte" /> that specifies a GDI character set to use for this font.</param>
        /// <param name="gdiVerticalFont">A Boolean value indicating whether the new <see cref="T:System.Drawing.Font" /> is derived from a GDI vertical font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet,
            bool gdiVerticalFont)
        {
            if (float.IsNaN(emSize) || float.IsInfinity(emSize) || emSize <= 0f)
            {
                throw new ArgumentException(emSize.ToString(), "emSize");
            }

            Initialize(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and style.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="family" /> is <see langword="null" />.</exception>
        public Font(FontFamily family, float emSize, FontStyle style)
        {
            Initialize(family, emSize, style, GraphicsUnit.Point, 1, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and unit. Sets the style to <see cref="F:System.Drawing.FontStyle.Regular" />.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="family" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(FontFamily family, float emSize, GraphicsUnit unit)
        {
            Initialize(family, emSize, FontStyle.Regular, unit, 1, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size.</summary>
        /// <param name="family">The <see cref="T:System.Drawing.FontFamily" /> of the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(FontFamily family, float emSize)
        {
            Initialize(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, gdiVerticalFont: false);
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size, style, and unit.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity or is not a valid number.</exception>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit)
        {
            Initialize(familyName, emSize, style, unit, 1, IsVerticalName(familyName));
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and style.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <param name="style">The <see cref="T:System.Drawing.FontStyle" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(string familyName, float emSize, FontStyle style)
        {
            Initialize(familyName, emSize, style, GraphicsUnit.Point, 1, IsVerticalName(familyName));
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size and unit. The style is set to <see cref="F:System.Drawing.FontStyle.Regular" />.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size of the new font in the units specified by the <paramref name="unit" /> parameter.</param>
        /// <param name="unit">The <see cref="T:System.Drawing.GraphicsUnit" /> of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity, or is not a valid number.</exception>
        public Font(string familyName, float emSize, GraphicsUnit unit)
        {
            Initialize(familyName, emSize, FontStyle.Regular, unit, 1, IsVerticalName(familyName));
        }

        /// <summary>Initializes a new <see cref="T:System.Drawing.Font" /> using a specified size.</summary>
        /// <param name="familyName">A string representation of the <see cref="T:System.Drawing.FontFamily" /> for the new <see cref="T:System.Drawing.Font" />.</param>
        /// <param name="emSize">The em-size, in points, of the new font.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="emSize" /> is less than or equal to 0, evaluates to infinity or is not a valid number.</exception>
        public Font(string familyName, float emSize)
        {
            Initialize(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, IsVerticalName(familyName));
        }


        internal void Initialize(FontFamily genericSansSerif, float size, FontStyle bold = FontStyle.Regular,
            GraphicsUnit pixel = GraphicsUnit.Point, byte gdiCharSet = 0,
            bool gdiVerticalFont = false)
        {
            nativeFont = new SKPaint()
            {
                Typeface = SKTypeface.FromFamilyName(genericSansSerif?.Name), TextSize = size,
                TextAlign = SKTextAlign.Left
            };

            FontFamily = new FontFamily() {Name = nativeFont.Typeface.FamilyName};
            Name = nativeFont.Typeface.FamilyName;
            Style = bold;
            Unit = pixel;
            Size = size;
        }

        internal void Initialize(string genericSansSerif, float size, FontStyle bold = FontStyle.Regular,
            GraphicsUnit pixel = GraphicsUnit.Point, byte gdiCharSet = 0,
            bool gdiVerticalFont = false)
        {
            Initialize(FontFamily.GenericSansSerif, size, bold, pixel, gdiCharSet, gdiVerticalFont);
        }


        /// <summary>Gets the line spacing of this font.</summary>
        /// <returns>The line spacing, in pixels, of this font.</returns>
        [Browsable(false)]
        public int Height
        {
            get { return (int) Math.Ceiling(GetHeight()); }
        }

        public FontFamily FontFamily { get; set; } = new FontFamily();

        private float _size = 12;
        /// <summary>Gets the em-size of this <see cref="T:System.Drawing.Font" /> measured in the units specified by the <see cref="P:System.Drawing.Font.Unit" /> property.</summary>
        /// <returns>The em-size of this <see cref="T:System.Drawing.Font" />.</returns>
        public float Size
        {
            get { return _size; }
            set
            {
                _size = value;
                try
                {
                    if (nativeFont != null)
                    {
                        if (Unit == GraphicsUnit.Pixel)
                            nativeFont.TextSize = value;
                        else
                            nativeFont.TextSize = value * 1.33334f;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public bool Bold => (Style & FontStyle.Bold) != 0;
        public string SystemFontName { get; set; } = "Arial";
        public bool Italic => (Style & FontStyle.Italic) != 0;

        public string Name { get; set; } = "Arial";
        public bool Strikeout => (Style & FontStyle.Strikeout) != 0;

        public bool Underline => (Style & FontStyle.Underline) != 0;
        public GraphicsUnit Unit { get; set; } = GraphicsUnit.Pixel;

        /// <summary>
        /// WinForms (GDI) fonts are points (pt)
        /// SkiaSharp is pixels (px)
        /// 12pt = 16px
        /// 
        /// Gets the em-size, in points, of this <see cref="T:System.Drawing.Font" />.</summary>
        /// <returns>The em-size, in points, of this <see cref="T:System.Drawing.Font" />.</returns>
        public float SizeInPoints
        {
            get
            {
                if (Unit == GraphicsUnit.Pixel)
                    return Size * 1.33334f;
                return Size;
            }
        }

        public void Dispose()
        {
            nativeFont?.Dispose();
        }

        public object Clone()
        {
            return nativeFont.Clone();
        }

        public IntPtr ToHfont()
        {
            return nativeFont.Handle;
        }

        /// <summary>Returns the line spacing, in pixels, of this font.</summary>
        /// <returns>The line spacing, in pixels, of this font.</returns>
        public float GetHeight()
        {
            return nativeFont.FontSpacing;
        }

        public Font FromHfont(IntPtr hfont)
        {
            return new Font(FontFamily.GenericMonospace, 8);
        }
    }
}