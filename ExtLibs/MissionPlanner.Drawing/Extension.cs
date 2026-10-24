using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace System.Drawing
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

        public static Rectangle ToRectangle(this RectangleF rect)
        {
            return new Rectangle((int)rect.X,(int)rect.Y,(int)rect.Width,(int)rect.Height);
        }

        public static RectangleF ToRectangleF(this Rectangle rect)
        {
            return new Rectangle(rect.X,rect.Y,rect.Width,rect.Height);
        }

        public static SKPaint ToSKPaint(this Pen pen)
        {
            pen.nativePen.StrokeWidth = pen.Width;
            pen.nativePen.Color = pen.Color.ToSKColor();
            pen.nativePen.Style = SKPaintStyle.Stroke;
            if (pen.DashStyle != DashStyle.Solid)
            {
                pen.nativePen.PathEffect = pen.GetDashEffect();
                pen.dashApplied = true;
            }
            else if (pen.dashApplied)
            {
                pen.nativePen.PathEffect = null;
                pen.dashApplied = false;
            }
            return pen.nativePen;
        }

        public static Bitmap ToBitmap(this SKImage skiaImage)
        {
            return new Bitmap() {nativeSkBitmap = SKBitmap.FromImage(skiaImage)};
        }

        /// <summary>
        /// Typeface for drawing text in this family. For a Chinese, Japanese or Korean UI, a typeface that
        /// has those glyphs is used instead, whatever the family. Cached per family/language.
        /// </summary>
        public static SKTypeface ToSKTypeface(this FontFamily ff)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var cjk = language == "zh" || language == "ja" || language == "ko";
            var id = cjk ? language : "family:" + ff?.Name;

            lock (fontcache)
            {
                if (fontcache.TryGetValue(id, out var typeface))
                    return typeface;

                var fm = SKFontManager.Default;
                if (language == "zh")
                    typeface = fm.MatchCharacter("YaHei", new[] { "zh" }, '飞');
                else if (language == "ja")
                    typeface = fm.MatchCharacter("", new[] { "ja" }, 'フ');
                else if (language == "ko")
                    typeface = fm.MatchCharacter("", new[] { "ko" }, '비');
                else
                    typeface = SKTypeface.FromFamilyName(ff?.Name);

                fontcache[id] = typeface;
                return typeface;
            }
        }

        static Dictionary<string, SKTypeface> fontcache = new Dictionary<string, SKTypeface>();

        public static SKPaint ToSKPaint(this Font font)
        {
            return new SKPaint
            {
                Typeface = font.FontFamily.ToSKTypeface(),
                TextSize = font.SizeInPoints * 1.33334f,
                StrokeWidth = 2,
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
                lock (brushcache)
                    if (!brushcache.ContainsKey(((SolidBrush) brush).Color))
                        brushcache[((SolidBrush) brush).Color] = new SKPaint
                        {
                            Color = ((SolidBrush) brush).Color.ToSKColor(), IsAntialias = true,
                            Style = SKPaintStyle.Fill
                        };

                return brushcache[((SolidBrush) brush).Color];
            }

            if (brush is HatchBrush)
            {
                return brush.nativeBrush;
            }

            if (brush is TextureBrush)
            {
                return brush.nativeBrush;
            }

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush) brush;
                if (lgb._gradMode == LinearGradientMode.Horizontal || lgb._gradMode == LinearGradientMode.Vertical)
                    return lgb.GetPaint();
            }

            return new SKPaint();
        }
    }
}