using System;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Drawing
{
    public abstract class Brush: IDisposable
    {
        internal SKPaint nativeBrush;
        public Color _color = Color.Black;

        public void Dispose()
        {
            nativeBrush?.Dispose();
        }
    }
}