using System.Collections.Generic;
using System.Drawing;
using MissionPlanner.Drawing.Drawing2D;
using SkiaSharp;

namespace MissionPlanner.Drawing
{
    public static class Extension
    {
        public static SKColor ToSKColor(this Color color)
        {
            var skcol = SkiaSharp.SKColor.Empty.WithAlpha(color.A).WithRed(color.R).WithGreen(color.G)
                .WithBlue(color.B);
            return skcol;
        }

        public static Color ToColor(this SKColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static SKRect ToSKRect(this Rectangle rect)
        {
            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        public static SKRectI ToSKRectI(this Rectangle rect)
        {
            return new SKRectI(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static SKPaint ToSKPaint(this Pen pen)
        {
            pen.nativePen.StrokeWidth = pen.Width;
            pen.nativePen.Color = pen.Color.ToSKColor();
            pen.nativePen.Style = SKPaintStyle.Stroke; 
            if (pen.DashStyle != DashStyle.Solid)
                pen.nativePen.PathEffect = SKPathEffect.CreateDash(pen.DashPattern, 0);
            return pen.nativePen;
        }


        static Dictionary<string, SKTypeface> fontcache = new Dictionary<string, SKTypeface>();
        public static SKPaint ToSKPaint(this Font font)
        {
            lock (fontcache)
            {
                if (!fontcache.ContainsKey(font.SystemFontName))
                    fontcache[font.SystemFontName] = SKTypeface.FromFamilyName(font.SystemFontName);
            }

            return new SKPaint
            {
                Typeface = fontcache[font.SystemFontName],
                TextSize = font.Size * 1.4f,
                StrokeWidth = 2
            };
        }

        public static SKPoint ToSKPoint(this PointF pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }

        public static SKPoint ToSKPoint(this Point pnt)
        {
            return new SKPoint(pnt.X, pnt.Y);
        }

        static Dictionary<Color, SKPaint> brushcache = new Dictionary<Color, SKPaint>();
        public static SKPaint ToSKPaint(this Brush brush)
        {
            if (brush is SolidBrush)
            {
                lock(brushcache)
                    if (!brushcache.ContainsKey(((SolidBrush) brush).Color))
                        brushcache[((SolidBrush) brush).Color] = new SKPaint
                        {
                            Color = ((SolidBrush) brush).Color.ToSKColor(), IsAntialias = true,
                            Style = SKPaintStyle.Fill
                        };

                return brushcache[((SolidBrush) brush).Color];
            }

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush)brush;
                return new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Shader = SKShader.CreateLinearGradient(new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Y),
                        new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Bottom),
                        new[]
                        {
                            ((LinearGradientBrush) brush).LinearColors[0].ToSKColor(),
                            ((LinearGradientBrush) brush).LinearColors[1].ToSKColor()
                        }
                        , null, SKShaderTileMode.Clamp, SKMatrix.MakeIdentity())
                };
            }

            return new SKPaint();
        }
    }
}