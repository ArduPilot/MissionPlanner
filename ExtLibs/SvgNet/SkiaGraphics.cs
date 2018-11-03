using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;
using SvgNet.SvgGdi;
using Color = System.Drawing.Color;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Pen = System.Drawing.Pen;
using Font = System.Drawing.Font;
using Brush = System.Drawing.Brush;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

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
                TextSize = (font.Size * 1.5f)
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

    public class SkiaGraphics: IGraphics, IDisposable
    {
        private SKSurface _surface = null;

        private SKCanvas _image
        {
            get { return _surface.Canvas; }
        }

        private SKPaint _paint =
            new SKPaint()
            {
                IsAntialias = true, FilterQuality = SKFilterQuality.High, StrokeWidth = 0,
                BlendMode = SKBlendMode.SrcOver
            };

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            throw new NotImplementedException();
        }

        public  SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            SKRect bound = new SKRect();
            font.SKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width, bound.Height);
        }

        public SizeF MeasureString(string text, Font font)
        {
            return MeasureString(text, font, SizeF.Empty, StringFormat.GenericDefault);
        }

        public SizeF MeasureString(string text, Font font, int width)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            throw new NotImplementedException();
        }

        public void MultiplyTransform(Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted,
            out int linesFilled)
        {
            throw new NotImplementedException();
        }

        public  SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            SKRect bound = new SKRect();
            font.SKPaint().MeasureText(text, ref bound);
            return new SizeF(bound.Width/* + origin.X*/, bound.Height/* + origin.Y*/);
        }

        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public  void Clear(Color color)
        {
            _image.Clear(color.SKColor());
        }

        public  void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x,y,x+width,y+height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.SKPaint());
        }

        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public  void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.SKPaint());
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, (int)startAngle, (int)sweepAngle);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            throw new NotImplementedException();
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public  void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            DrawArc(pen, x, y, width, height, 0, 360);
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            throw new NotImplementedException();
        }

        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF point)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y)
        {
            throw new NotImplementedException();
        }

        public  void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize,
            CopyPixelOperation copyPixelOperation)
        {
            //fixme

        }

        public void DrawImage(Image img, RectangleF rect)
        {
            DrawImage(img, (long)rect.X, (long)rect.Y, (long)rect.Width, (long)rect.Height);
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImage(image, (long)x, (long)y, (long)width, (long)height);
            //throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point point)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            DrawImage(image, (long)x, (long)y, (long)width, (long)height);
        }

        public void DrawImage(Image image, PointF[] destPoints)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr,
            Graphics.DrawImageAbort callback)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image img, long i, long i1, long width, long height)
        {
            ((Bitmap)img).MakeTransparent(Color.Transparent);
            var data = ((Bitmap) img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            using (var skbmp = new SKBitmap(new SKImageInfo(img.Width, img.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul)))
            {
                skbmp.SetPixels(data.Scan0);

                //skbmp.InstallPixels(new SKPixmap(skbmp.Info, data.Scan0));

                _paint.BlendMode = SKBlendMode.SrcOver;
                //_paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Transparent, SKBlendMode.SrcOver);

                _image.DrawBitmap(skbmp, new SKRect(i, i1, i + width, i1 + height), _paint);
            }

            ((Bitmap) img).UnlockBits(data);

            data = null;
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public  void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), pen.SKPaint());
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            DrawRectangle(pen, (float)x, y, width, height);
        }

        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0, 0), StringFormat.GenericDefault);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            throw new NotImplementedException();
        }

        public  void ResetTransform()
        {
            _image.ResetMatrix();
        }

        public void Restore(GraphicsState gstate)
        {
            throw new NotImplementedException();
        }

        public void RotateTransform(float angle)
        {
            _surface.Canvas.RotateDegrees(angle);
        }

        public  void RotateTransform(float angle, MatrixOrder order)
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

        public GraphicsState Save()
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        public  void TranslateTransform(float f, float f1, MatrixOrder order)
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

        public  void ScaleTransform(float sx, float sy, MatrixOrder order)
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

        public void SetClip(Graphics g)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Graphics g, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void FillPath(Brush fill, GraphicsPath graphicsPath)
        {
            if (graphicsPath.PointCount == 0)
            {
                return;
            }

            _image.DrawPath(pathtopintfarr(graphicsPath), fill.SKPaint());
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, brush.SKPaint());
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, brush.SKPaint());
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

        public void DrawLines(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawPath(Pen stroke, GraphicsPath graphicsPath)
        {
            if (graphicsPath.PointCount==0)
            {
                return;
            }

            _image.DrawPath(pathtopintfarr(graphicsPath), stroke.SKPaint());
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var path = new SKPath();
            path.AddArc(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle);
            _image.DrawPath(path, pen.SKPaint());
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public  void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, brush.SKPaint());
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public  void DrawPolygon(Pen pen, Point[] points)
        {
            var path = new SKPath();
            path.AddPoly(points.Select(a => new SKPoint(a.X, a.Y)).ToArray());
            _image.DrawPath(path, pen.SKPaint());
        }

        public  void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            _image.DrawRect(new SKRect(x, y, x + width, y + height), brush.SKPaint());
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            FillRectangle(brush, (float)x, y, width, height);
        }

        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            throw new NotImplementedException();
        }

        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            throw new NotImplementedException();
        }

        public void FillRegion(Brush brush, Region region)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Flush(FlushIntention intention)
        {
            throw new NotImplementedException();
        }

        public Color GetNearestColor(Color color)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void IntersectClip(Region region)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(Point point)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(float x, float y)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(PointF point)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public  void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            _image.DrawLine(x1, y1, x2, y2, pen.SKPaint());
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, (float)x1, y1, x2, y2);
            //throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public Region Clip { get; set; }
        public RectangleF ClipBounds { get; }
        public CompositingMode CompositingMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }
        public float DpiX { get; }
        public float DpiY { get; }
        public InterpolationMode InterpolationMode { get; set; }
        public bool IsClipEmpty { get; }
        public bool IsVisibleClipEmpty { get; }
        public float PageScale { get; set; }
        public GraphicsUnit PageUnit { get; set; }
        public PixelOffsetMode PixelOffsetMode { get; set; }
        public Point RenderingOrigin { get; set; }
        public SmoothingMode SmoothingMode { get; set; }
        public int TextContrast { get; set; }
        public TextRenderingHint TextRenderingHint { get; set; }

        public Matrix Transform
        {
            get { return new Matrix(_image.TotalMatrix.Persp0, _image.TotalMatrix.Persp1, _image.TotalMatrix.ScaleX, _image.TotalMatrix.ScaleY, _image.TotalMatrix.TransX, _image.TotalMatrix.TransY); }
            set { }
        }

        public RectangleF VisibleClipBounds { get; }
        public void AddMetafileComment(byte[] data)
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer()
        {
            throw new NotImplementedException();
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
        /*
        public static implicit operator SkiaGraphics(IGraphics g)
        {
            var ret = new SkiaGraphics();

            var imageWidth = (int) g.VisibleClipBounds.Width;
            var imageHeight = (int) g.VisibleClipBounds.Height;

            //var hdc = g.GetHdc();

            ret._surface = SKSurface.Create(imageWidth, imageHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            return ret;
        }
        */

        public SkiaGraphics()
        {

        }

        public SkiaGraphics(IntPtr handle, int width, int height)
        {
            _surface = SKSurface.Create(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
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

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            throw new NotImplementedException();
        }

        public  void DrawPolygon(Pen pen, PointF[] points)
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

        public  void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            var pnt = brush.SKPaint();
            // Find the text bounds
            SKRect textBounds = SKRect.Empty;
            var fnt = font.SKPaint();
            fnt.MeasureText(s, ref textBounds);

            pnt.TextSize = fnt.TextSize;
            pnt.Typeface = fnt.Typeface;

            pnt.StrokeWidth = 1;
            _image.DrawText(s, layoutRectangle.X, layoutRectangle.Y + textBounds.Height, pnt);
        }

        public void EndContainer(GraphicsContainer container)
        {
            throw new NotImplementedException();
        }

        public void ExcludeClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void ExcludeClip(Region region)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Brush brush, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle rectangle, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes tileFlipXYAttributes)
        {
            DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle rectangle, int p1, int p2, long p3, long p4, GraphicsUnit graphicsUnit, ImageAttributes TileFlipXYAttributes)
        {
            DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public  void SetClip(RectangleF rect, CombineMode mode)
        {
            _image.ClipRect(new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), SKClipOperation.Intersect,
                false);
        }

        public void SetClip(GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
            throw new NotImplementedException();
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
        {
            throw new NotImplementedException();
        }

        public void TranslateClip(float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public void TranslateClip(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        public  void ResetClip()
        {
            _image.ClipRect(_image.LocalClipBounds);
            //_image.ClipRect(SKRect.Create(0, 0, 10000, 10000), SKClipOperation.Intersect, false);
        }
        
    }
}