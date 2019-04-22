using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Font: IDisposable
    {
        internal SKPaint nativeFont;

        public Font(FontFamily genericSansSerif, int size, FontStyle bold, GraphicsUnit pixel = GraphicsUnit.Pixel)
        {
            nativeFont = new SKPaint() {Typeface = SKTypeface.Default, TextSize = size, TextAlign = SKTextAlign.Left};
        }

        public float Height
        {
            get { return nativeFont.TextSize; }
            set { nativeFont.TextSize = value; }
        }

        public void Dispose()
        {
            nativeFont?.Dispose();
        }
    }
}
