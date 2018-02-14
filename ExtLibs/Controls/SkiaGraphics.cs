using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using SkiaSharp;
using Color = System.Drawing.Color;
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
            return new SKPaint() { Color = pen.Color.SKColor(), StrokeWidth = pen.Width, Style = SKPaintStyle.Stroke };
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
                return new SKPaint() { Color = ((SolidBrush)brush).Color.SKColor(), Style = SKPaintStyle.Fill };
            }

            if (brush is LinearGradientBrush)
            {
                var lgb = (LinearGradientBrush)brush;
                return new SKPaint()
                {
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

    public class SkiaGraphics: IDisposable
    {
        private SKSurface _surface = null;

        private SKCanvas _image
        {
            get { return _surface.Canvas; }
        }

        private SKPaint _paint =
            new SKPaint() {IsAntialias = true, FilterQuality = SKFilterQuality.High, StrokeWidth = 0};

        public SizeF MeasureString(string toolTipText, Font font)
        {
            SKRect bound = new SKRect();
            font.SKPaint().MeasureText(toolTipText, ref bound);
            return new SizeF(bound.Width, bound.Height);
        }

        public void Clear(Color color)
        {
            _image.Clear(color.SKColor());
        }

        public void DrawArc(Pen penn, RectangleF rect, float start, float degrees)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), start, degrees);
            _image.DrawPath(path, penn.SKPaint());
        }

        public void DrawEllipse(Pen penn, RectangleF rect)
        {
            DrawArc(penn, rect, 0, 360);
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

        public void DrawRectangle(Pen stroke, Rectangle rect)
        {
            DrawRectangle(stroke, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void ResetTransform()
        {
            _image.ResetMatrix();
        }

        public void RotateTransform(float angle)
        {
            _image.RotateDegrees(angle);
        }

        public void TranslateTransform(float f, float f1, MatrixOrder order = MatrixOrder.Prepend)
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

        public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
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

            for (int i = 0; i < path.PointCount; i++)
            {
                if (path.PathTypes[i] == 0)
                {
                    ans.MoveTo(path.PathPoints[i].X, path.PathPoints[i].Y);
                    continue;
                }

                if (path.PathTypes[i] <= 3)
                    ans.LineTo(path.PathPoints[i].X, path.PathPoints[i].Y);

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

        public void FillPolygon(Brush brushh, Point[] list)
        {
            var path = new SKPath();
            path.AddPoly(list.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, brushh.SKPaint());
        }

        public void DrawPolygon(Pen penn, Point[] list)
        {
            var path = new SKPath();
            path.AddPoly(list.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, penn.SKPaint());
        }

        public void FillRectangle(Brush brushh, long x, long y, long width, long height)
        {
            FillRectangle(brushh, new RectangleF(x, y, width, height));
        }

        public void FillRectangle(Brush brushh, RectangleF rectf)
        {
            _image.DrawRect(new SKRect(rectf.Left, rectf.Top, rectf.Right, rectf.Bottom), brushh.SKPaint());
        }

        public void DrawRectangle(Pen penn, float x1, float y1, float width, float height)
        {
            _image.DrawRect(new SKRect(x1, y1, x1 + width, y1 + height), penn.SKPaint());
        }

        public void DrawLine(Pen penn, float x1, float y1, float x2, float y2)
        {
            _image.DrawLine(x1, y1, x2, y2, penn.SKPaint());
        }

        public SmoothingMode SmoothingMode { get; set; }
        public InterpolationMode InterpolationMode { get; set; }
        public CompositingMode CompositingMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }
        public PixelOffsetMode PixelOffsetMode { get; set; }
        public TextRenderingHint TextRenderingHint { get; set; }
        public Matrix Transform
        {
            get { return new Matrix(_image.TotalMatrix.Persp0, _image.TotalMatrix.Persp1, _image.TotalMatrix.ScaleX, _image.TotalMatrix.ScaleY, _image.TotalMatrix.TransX, _image.TotalMatrix.TransY); }
            set { }
        }
        public Region Clip { get;  set; }

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

        public void DrawPolygon(Pen penn, PointF[] list)
        {
            var path = new SKPath();
            path.AddPoly(list.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, penn.SKPaint());
        }

        public void Dispose()
        {
            _surface?.Dispose();
            _paint?.Dispose();
        }


        public void DrawString(string p0, Font missingDataFont, Brush red, RectangleF p3)
        {
            missingDataFont.SKPaint().Color = red.SKPaint().Color;
            _image.DrawText(p0, p3.X, p3.Y, _paint);
        }

        public void DrawString(string v1, Font copyrightFont, Brush blue, int v2, int height)
        {
            _paint.Color = blue.SKPaint().Color;
            _image.DrawText(v1, v2, height, _paint);
        }

        public void DrawString(string EmptyTileText, System.Drawing.Font MissingDataFont, Brush brush, RectangleF rectangleF, StringFormat CenterFormat)
        {
            float textWidth = _paint.MeasureText(EmptyTileText);
            // Find the text bounds
            SKRect textBounds= SKRect.Empty;
            _paint.MeasureText(EmptyTileText, ref textBounds);

            _paint.StrokeWidth = 1;

            _paint.Color = brush.SKPaint().Color;
            _image.DrawText(EmptyTileText, rectangleF.X, rectangleF.Y, _paint);
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

        public void SetClip(RectangleF rect, CombineMode mode = CombineMode.Replace)
        {
            _image.ClipRect(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), SKClipOperation.Intersect,
                false);
        }

        public void SetClip(Region rect1, CombineMode mode = CombineMode.Replace)
        {
            
        }

        public void ResetClip()
        {
            _image.ClipRect(SKRect.Create(0, 0, 10000, 10000), SKClipOperation.Intersect, false);
        }
        
    }
}