using System;
using System.Drawing;
using MissionPlanner.Utilities;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Graphics:  IDisposable
    {
        internal SKSurface nativeSurface;

        public static Graphics FromImage(Bitmap bmpDestination)
        {
            var bmpdata = bmpDestination.LockBits(new Rectangle(0, 0, bmpDestination.Width, bmpDestination.Height),
                null, SKColorType.Bgra8888);
            return new Graphics()
            {
                nativeSurface = SKSurface.Create(bmpDestination.nativeSkBitmap.Info, bmpdata.Scan0)
            };
        }

        public void Dispose()
        {
            nativeSurface.Dispose();
        }

        public void DrawString(string text, SKPaint defaultFont, Brush red, int x, int y, StringFormat genericDefault)
        {
            var paint = red.nativeBrush;
            paint.TextSize = defaultFont.TextSize;
            paint.Typeface = defaultFont.Typeface;
            paint.StrokeWidth = 1;

            var bounds = new SKRect();
            paint.MeasureText(text,ref bounds);
            nativeSurface.Canvas.DrawText(text, x, y + bounds.Height, paint);
        }

        public CompositingMode CompositingMode { get; set; }

        public void DrawImage(Image image, long x, long y, long width, long height)
        {
            nativeSurface.Canvas.DrawImage(SKImage.FromBitmap(image.nativeSkBitmap), x, y,
                new SKPaint()
                    {IsAntialias = true, BlendMode = SKBlendMode.SrcOver, FilterQuality = SKFilterQuality.Medium});
        }

        public void DrawLine(Pen pen, PointF lastpoint, PointF newpoint)
        {
            nativeSurface.Canvas.DrawLine(lastpoint.ToSKPoint(), newpoint.ToSKPoint(), pen.nativePen);
        }

        public void DrawImage(Image original, Rectangle rectangle, int x, int y, int originalWidth, int originalHeight, GraphicsUnit pixel, ImageAttributes attributes)
        {
            DrawImage(original, x, y, originalWidth, originalHeight);
        }
    }
}