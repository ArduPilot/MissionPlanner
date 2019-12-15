using System;
using SkiaSharp;

namespace MissionPlanner.Drawing
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
            Name = genericSansSerif;
            Size = size;

            try
            {
                nativeFont = new SKPaint() {Typeface = SKTypeface.Default, TextSize = size, TextAlign = SKTextAlign.Left};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Font(Font genericSansSerif, FontStyle style)
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

        public FontFamily FontFamily { get; set; }

        private float _size = 7;
        public float Size
        {
            get { return _size; }
            set
            {
                _size = value;
                try
                {
                    if(nativeFont != null)
                        nativeFont.TextSize = value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
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

        public IntPtr ToHfont()
        {
            return nativeFont.Handle;
        }
    }
}
