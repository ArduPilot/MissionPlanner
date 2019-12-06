using System;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Drawing
{
    public abstract class Brush: IDisposable
    {
        internal SKPaint nativeBrush;
        internal Color _color;

        public void Dispose()
        {
            nativeBrush?.Dispose();
        }
    }
}