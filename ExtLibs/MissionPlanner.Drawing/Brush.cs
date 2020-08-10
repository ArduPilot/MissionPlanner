using System;
using System.Drawing;
using SkiaSharp;

namespace System.Drawing
{
    public abstract class Brush : IDisposable
    {
        internal SKPaint nativeBrush;
        public Color _color = Color.Black;

        public void Dispose()
        {
            nativeBrush?.Dispose();
        }

        public Brush Clone()
        {
            return new SolidBrush() {nativeBrush = nativeBrush?.Clone()};
        }
    }
}