using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Font : IDisposable, ICloneable
    {
        internal SKPaint nativeFont;
        public FontStyle Style { get; set; }

        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
        {
            nativeFont = new SKPaint() { Typeface = SKTypeface.Default, TextSize = emSize, TextAlign = SKTextAlign.Left };
        }

        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet,
            bool gdiVerticalFont = false)
        {
            nativeFont = new SKPaint() { Typeface = SKTypeface.Default, TextSize = emSize, TextAlign = SKTextAlign.Left };
        }

        public Font(FontFamily genericSansSerif, float size, FontStyle bold, GraphicsUnit pixel = GraphicsUnit.Pixel)
        {
            nativeFont = new SKPaint() {Typeface = SKTypeface.Default, TextSize = size, TextAlign = SKTextAlign.Left};
        }

        public Font(FontFamily genericSansSerif, float size)
        {
            nativeFont = new SKPaint() { Typeface = SKTypeface.Default, TextSize = size, TextAlign = SKTextAlign.Left };
        }

        public Font(string genericSansSerif, float size)
        {
            nativeFont = new SKPaint() {Typeface = SKTypeface.Default, TextSize = size, TextAlign = SKTextAlign.Left};
        }

        public Font(Font genericSansSerif, FontStyle stle)
        {
            nativeFont = genericSansSerif.nativeFont;
         
        }

        public Font(string microsoftSansSerif, float captionRectHeight, FontStyle bold = FontStyle.Regular, GraphicsUnit pixel = GraphicsUnit.Pixel): this(microsoftSansSerif, captionRectHeight)
        {
           
        }


        /// <summary>
        /// pixels
        /// </summary>
        public int Height
        {
            get { return (int) nativeFont.TextSize; }
            set { nativeFont.TextSize = value; }
        }

        public String FontFamily { get; set; }

        public float Size
        {
            get { return nativeFont.TextSize; }
            set { nativeFont.TextSize = value; }
        }

        public bool Bold { get; set; }
        public string SystemFontName { get; set; } = "Arial";
        public bool Italic { get; set; }
        public string Name { get; set; } = "Arial";
        public bool Strikeout { get; set; }
        public bool Underline { get; set; }
        public GraphicsUnit Unit
        { get; set; }

        public int SizeInPoints { get; set; }

        public void Dispose()
        {
            nativeFont?.Dispose();
        }

        public object Clone()
        {
            return nativeFont.Clone();
        }
    }
}
