using System;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public abstract class Brush: IDisposable
    {
        internal SKPaint nativeBrush;

        public void Dispose()
        {
            nativeBrush?.Dispose();
        }
    }
}