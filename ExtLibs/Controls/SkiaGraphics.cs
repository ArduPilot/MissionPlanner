using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using MissionPlanner.Utilities;
using SkiaSharp;
using Color = System.Drawing.Color;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace System
{
    public static class Extension
    {
        public static SKColor SKColor(this Color color)
        {
            var skcol = SkiaSharp.SKColor.Empty.WithAlpha(color.A).WithRed(color.R).WithGreen(color.G).WithBlue(color.B);
            return skcol;
        }

        public static SKPaint SKPaint(this Pen pen)
        {
            return new SKPaint() { Color = pen.Color.SKColor(), StrokeWidth = pen.Width, IsAntialias = true, Style = SKPaintStyle.Stroke, BlendMode = SKBlendMode.SrcOver, FilterQuality = SKFilterQuality.High};
        }

        public static SKPaint SKPaint(this Font font)
        {
            return new SKPaint()
            {
                Typeface = SKTypeface.FromFamilyName(font.SystemFontName),
                TextSize = font.Size
            };
        }

        public static SKPaint SKPaint(this Brush brush)
        {
            if (brush is SolidBrush)
            {
                return new SKPaint() { Color = ((SolidBrush)brush).Color.SKColor(), IsAntialias = true, Style = SKPaintStyle.Fill };
            }

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush)brush;
                return new SKPaint()
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Shader = SKShader.CreateLinearGradient(new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Y), new SKPoint(lgb.Rectangle.X, lgb.Rectangle.Bottom),
                        new SKColor[] {
                            ((LinearGradientBrush)brush).LinearColors[0].SKColor() ,
                            ((LinearGradientBrush)brush).LinearColors[1].SKColor()
                        }
                        , null, SKShaderTileMode.Clamp, SKMatrix.MakeIdentity())

                };
            }

            return new SKPaint();
        }
    }

    public class Graphics: IDisposable
    {
        private SKSurface _surface = null;

        private SKCanvas _image
        {
            get { return _surface.Canvas; }
        }

        private SKPaint _paint =
            new SKPaint() {IsAntialias = true, FilterQuality = SKFilterQuality.High, StrokeWidth = 0};

        public override SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            SKRect bound = new SKRect();
            font.SKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width, bound.Height);
        }

        public override SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            SKRect bound = new SKRect();
            font.SKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width/* + origin.X*/, bound.Height/* + origin.Y*/);
        }

        public override void Clear(Color color)
        {
            _image.Clear(color.SKColor());
        }

        public override void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x,y,x+width,y+height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.SKPaint());
        }

        public override void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.SKPaint());
        }

        public override void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            DrawArc(pen, x, y, width, height, 0, 360);
        }

        public override void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize,
            CopyPixelOperation copyPixelOperation)
        {
            //fixme

        }

        public void DrawImage(Image img, RectangleF rect)
        {
            DrawImage(img, (long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height);
        }

        public void DrawImage(Image img, long i, long i1, long width, long height)
        {
            var data = ((Bitmap) img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            using (var skbmp = new SKBitmap(new SKImageInfo(img.Width, img.Height, SKColorType.Bgra8888)))
            {
                skbmp.SetPixels(data.Scan0);

                //skbmp.InstallPixels(new SKPixmap(skbmp.Info, data.Scan0));

                _image.DrawBitmap(skbmp, new SKRect(i, i1, i + width, i1 + height), _paint);
            }

            ((Bitmap) img).UnlockBits(data);

            data = null;
        }

        public override void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), pen.SKPaint());
        }

        public override void ResetTransform()
        {
            _image.ResetMatrix();
        }

        public override void RotateTransform(float angle, MatrixOrder order)
        {
            if(order == MatrixOrder.Prepend)
                _image.RotateDegrees(angle);

            if (order == MatrixOrder.Append)
            {
                //checkthis
                var old = _image.TotalMatrix;
                var extra = SKMatrix.MakeRotation(angle);
                SKMatrix.PreConcat(ref old, extra);
                _image.SetMatrix(old);
            }
        }

        public override void TranslateTransform(float f, float f1, MatrixOrder order)
        {
            if(order == MatrixOrder.Prepend)
            _image.Translate(f, f1);

            if (order == MatrixOrder.Append)
            {
                var old = _image.TotalMatrix;
                old.SetScaleTranslate(old.ScaleX, old.ScaleY, f, f1);
                _image.SetMatrix(old);
            }
        }

        public override void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _image.Scale(sx, sy);

            if (order == MatrixOrder.Append)
            {
                var old = _image.TotalMatrix;
                old.SetScaleTranslate(sx, sy, old.TransX, old.TransY);
                _image.SetMatrix(old);
            }
        }

        public void FillPath(Brush fill, GraphicsPath graphicsPath)
        {
            if (graphicsPath.PointCount == 0)
            {
                return;
            }

            _image.DrawPath(pathtopintfarr(graphicsPath), fill.SKPaint());
        }

        SKPath pathtopintfarr(GraphicsPath path)
        {
            var ans = new SKPath();

            var cubic = new List<PointF>();

            for (int i = 0; i < path.PointCount; i++)
            {
                if (path.PathTypes[i] == 0)
                {
                    ans.MoveTo(path.PathPoints[i].X, path.PathPoints[i].Y);
                    continue;
                }

                if ((path.PathTypes[i] & 0x7) == 1)
                {
                    ans.LineTo(path.PathPoints[i].X, path.PathPoints[i].Y);
                }

                if ((path.PathTypes[i] & 0x7) == 3)
                {
                    /* first bezier requires 4 points, other 3 more points */
                    cubic.Add(path.PathPoints[i]);
                    if (cubic.Count == 3)
                    {
                        ans.CubicTo(cubic[0].X, cubic[0].Y, cubic[1].X, cubic[1].Y, cubic[2].X, cubic[2].Y);
                        cubic.Clear();
                    }
                }

                //if ((path.PathTypes[i] & 0x20) > 0)
                    //list.Add(path.PathPoints[i]);

                if ((path.PathTypes[i] & 0x80) > 0)
                {
                    ans.LineTo(path.PathPoints[i].X, path.PathPoints[i].Y);
                    ans.Close();
                }
            }

            return ans;
        }

        public void DrawPath(Pen stroke, GraphicsPath graphicsPath)
        {
            if (graphicsPath.PointCount==0)
            {
                return;
            }

            _image.DrawPath(pathtopintfarr(graphicsPath), stroke.SKPaint());
        }

        public override void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, brush.SKPaint());
        }

        public override void DrawPolygon(Pen pen, Point[] points)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, pen.SKPaint());
        }

        public override void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), brush.SKPaint());
        }

        public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            _image.DrawLine(x1, y1, x2, y2, pen.SKPaint());
        }

        public Matrix Transform
        {
            get { return new Matrix(_image.TotalMatrix.Persp0, _image.TotalMatrix.Persp1, _image.TotalMatrix.ScaleX, _image.TotalMatrix.ScaleY, _image.TotalMatrix.TransX, _image.TotalMatrix.TransY); }
            set { }
        }

        public static SkiaGraphics FromImage(Bitmap bmp)
        {
            var ans = new SkiaGraphics();

            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            SKBitmap bm =
                new SKBitmap(new SKImageInfo(bmp.Width, bmp.Height, SKColorType.Bgra8888, SKAlphaType.Premul),
                    bmp.Width * 4);
            bm.SetPixels(data.Scan0);

            ans._surface = SKSurface.Create(bm.Info, data.Scan0, bmp.Width * 4);

            bmp.UnlockBits(data);

            return ans;
        }

        public static implicit operator SkiaGraphics(Graphics g)
        {
            var ret = new SkiaGraphics();

            var imageWidth = (int) g.VisibleClipBounds.Width;
            var imageHeight = (int) g.VisibleClipBounds.Height;

            var hdc = g.GetHdc();

            ret._surface = SKSurface.Create(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            return ret;
        }

        public static implicit operator Image(SkiaGraphics sk)
        {
            using (var snap = sk._surface.Snapshot())
            using (var rast = snap.ToRasterImage())
            using (var pxmap = rast.PeekPixels())
            {
                var bmpdata = pxmap.GetPixels();
                return new Bitmap(rast.Width, rast.Height, rast.Width * 4, PixelFormat.Format32bppArgb, bmpdata);
            }
        }

        public void FillPolygon(Brush brushh, PointF[] list)
        {
            var path = new SKPath();
            path.AddPoly(list.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, brushh.SKPaint());
        }

        public override void DrawPolygon(Pen pen, PointF[] points)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, pen.SKPaint());
        }

        public void Dispose()
        {
            _surface?.Dispose();
            _paint?.Dispose();
        }

        public override void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            var pnt = brush.SKPaint();
            // Find the text bounds
            SKRect textBounds = SKRect.Empty;
            var fnt = font.SKPaint();
            fnt.MeasureText(s, ref textBounds);

            pnt.TextSize = fnt.TextSize;
            pnt.Typeface = fnt.Typeface;

            pnt.StrokeWidth = 1;
            _image.DrawText(s, layoutRectangle.X, layoutRectangle.Y, pnt);
        }

        public void DrawImage(Image image, Rectangle rectangle, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes tileFlipXYAttributes)
        {
            DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public void DrawImage(Image image, Rectangle rectangle, int p1, int p2, long p3, long p4, GraphicsUnit graphicsUnit, ImageAttributes TileFlipXYAttributes)
        {
            DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public override void SetClip(RectangleF rect, CombineMode mode)
        {
            _image.ClipRect(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), SKClipOperation.Intersect,
                false);
        }

        public override void ResetClip()
        {
            _image.ClipRect(_image.LocalClipBounds);
            //_image.ClipRect(SKRect.Create(0, 0, 10000, 10000), SKClipOperation.Intersect, false);
        }
        
    }
}