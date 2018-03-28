/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace SvgNet.SvgGdi
{
    /// <summary>
    /// An IGraphics implementation that simply passes every call through to a GDI+ <c>Graphics</c> object.
    /// </summary>
    public class GdiGraphics : IGraphics
    {
        public GdiGraphics(Graphics g)
        {
            _g = g;
        }

        public Region Clip { get { return _g.Clip; } set { _g.Clip = value; } }
        public RectangleF ClipBounds => _g.ClipBounds;
        public CompositingMode CompositingMode { get { return _g.CompositingMode; } set { _g.CompositingMode = value; } }
        public CompositingQuality CompositingQuality { get { return _g.CompositingQuality; } set { _g.CompositingQuality = value; } }
        public float DpiX => _g.DpiX;
        public float DpiY => _g.DpiY;
        public InterpolationMode InterpolationMode { get { return _g.InterpolationMode; } set { _g.InterpolationMode = value; } }
        public bool IsClipEmpty => _g.IsClipEmpty;
        public bool IsVisibleClipEmpty => _g.IsVisibleClipEmpty;
        public float PageScale { get { return _g.PageScale; } set { _g.PageScale = value; } }
        public GraphicsUnit PageUnit { get { return _g.PageUnit; } set { _g.PageUnit = value; } }
        public PixelOffsetMode PixelOffsetMode { get { return _g.PixelOffsetMode; } set { _g.PixelOffsetMode = value; } }
        public Point RenderingOrigin { get { return _g.RenderingOrigin; } set { _g.RenderingOrigin = value; } }
        public SmoothingMode SmoothingMode { get { return _g.SmoothingMode; } set { _g.SmoothingMode = value; } }
        public int TextContrast { get { return _g.TextContrast; } set { _g.TextContrast = value; } }
        public TextRenderingHint TextRenderingHint { get { return _g.TextRenderingHint; } set { _g.TextRenderingHint = value; } }
        public Matrix Transform { get { return _g.Transform; } set { _g.Transform = value; } }
        public RectangleF VisibleClipBounds => _g.VisibleClipBounds;

        public void AddMetafileComment(byte[] data)
        {
            _g.AddMetafileComment(data);
        }

        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit) => _g.BeginContainer(dstrect, srcrect, unit);

        public GraphicsContainer BeginContainer() => _g.BeginContainer();

        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit) => _g.BeginContainer(dstrect, srcrect, unit);

        public void Clear(Color color)
        {
            _g.Clear(color);
        }

        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            _g.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
        }

        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            _g.DrawArc(pen, rect, startAngle, sweepAngle);
        }

        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            _g.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            _g.DrawArc(pen, rect, startAngle, sweepAngle);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            _g.DrawBezier(pen, x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            _g.DrawBezier(pen, pt1, pt2, pt3, pt4);
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            _g.DrawBezier(pen, pt1, pt2, pt3, pt4);
        }

        public void DrawBeziers(Pen pen, PointF[] points)
        {
            _g.DrawBeziers(pen, points);
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            _g.DrawBeziers(pen, points);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            _g.DrawClosedCurve(pen, points);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            _g.DrawClosedCurve(pen, points, tension, fillmode);
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            _g.DrawClosedCurve(pen, points);
        }

        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            _g.DrawClosedCurve(pen, points, tension, fillmode);
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            _g.DrawCurve(pen, points);
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            _g.DrawCurve(pen, points, tension);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            _g.DrawCurve(pen, points, offset, numberOfSegments);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            _g.DrawCurve(pen, points, offset, numberOfSegments, tension);
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            _g.DrawCurve(pen, points);
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            _g.DrawCurve(pen, points, tension);
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            _g.DrawCurve(pen, points, offset, numberOfSegments, tension);
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            _g.DrawEllipse(pen, rect);
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            _g.DrawEllipse(pen, x, y, width, height);
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            _g.DrawEllipse(pen, rect);
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            _g.DrawEllipse(pen, x, y, width, height);
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            _g.DrawIcon(icon, x, y);
        }

        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            _g.DrawIcon(icon, targetRect);
        }

        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            _g.DrawIconUnstretched(icon, targetRect);
        }

        public void DrawImage(Image image, PointF point)
        {
            _g.DrawImage(image, point);
        }

        public void DrawImage(Image image, float x, float y)
        {
            _g.DrawImage(image, x, y);
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            _g.DrawImage(image, rect);
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            _g.DrawImage(image, x, y, width, height);
        }

        public void DrawImage(Image image, Point point)
        {
            _g.DrawImage(image, point);
        }

        public void DrawImage(Image image, int x, int y)
        {
            _g.DrawImage(image, x, y);
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            _g.DrawImage(image, rect);
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            _g.DrawImage(image, x, y, width, height);
        }

        public void DrawImage(Image image, PointF[] destPoints)
        {
            _g.DrawImage(image, destPoints);
        }

        public void DrawImage(Image image, Point[] destPoints)
        {
            _g.DrawImage(image, destPoints);
        }

        public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, x, y, srcRect, srcUnit);
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, x, y, srcRect, srcUnit);
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destRect, srcRect, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destRect, srcRect, srcUnit);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destPoints, srcRect, srcUnit);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            _g.DrawImage(image, destPoints, srcRect, srcUnit);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
        {
            _g.DrawImage(image, destPoints, srcRect, srcUnit);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destPoints, srcRect, srcUnit);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            _g.DrawImage(image, destPoints, srcRect, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            _g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            _g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            _g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            _g.DrawImageUnscaled(image, point);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            _g.DrawImageUnscaled(image, x, y);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            _g.DrawImageUnscaled(image, rect);
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            _g.DrawImageUnscaled(image, x, y, width, height);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            _g.DrawLine(pen, x1, y1, x2, y2);
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            _g.DrawLine(pen, pt1, pt2);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            _g.DrawLine(pen, x1, y1, x2, y2);
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            _g.DrawLine(pen, pt1, pt2);
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            _g.DrawLines(pen, points);
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            _g.DrawLines(pen, points);
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            _g.DrawPath(pen, path);
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            _g.DrawPie(pen, rect, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            _g.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            _g.DrawPie(pen, rect, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            _g.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            _g.DrawPolygon(pen, points);
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            _g.DrawPolygon(pen, points);
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            _g.DrawRectangle(pen, rect);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            _g.DrawRectangle(pen, x, y, width, height);
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            _g.DrawRectangle(pen, x, y, width, height);
        }

        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            _g.DrawRectangles(pen, rects);
        }

        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            _g.DrawRectangles(pen, rects);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            _g.DrawString(s, font, brush, x, y);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            _g.DrawString(s, font, brush, point);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            _g.DrawString(s, font, brush, x, y, format);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            _g.DrawString(s, font, brush, point, format);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            _g.DrawString(s, font, brush, layoutRectangle);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            _g.DrawString(s, font, brush, layoutRectangle, format);
        }

        public void EndContainer(GraphicsContainer container)
        {
            _g.EndContainer(container);
        }

        public void ExcludeClip(Rectangle rect)
        {
            _g.ExcludeClip(rect);
        }

        public void ExcludeClip(Region region)
        {
            _g.ExcludeClip(region);
        }

        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            _g.FillClosedCurve(brush, points);
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            _g.FillClosedCurve(brush, points, fillmode);
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            _g.FillClosedCurve(brush, points, fillmode, tension);
        }

        public void FillClosedCurve(Brush brush, Point[] points)
        {
            _g.FillClosedCurve(brush, points);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            _g.FillClosedCurve(brush, points, fillmode);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            _g.FillClosedCurve(brush, points, fillmode, tension);
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            _g.FillEllipse(brush, rect);
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            _g.FillEllipse(brush, x, y, width, height);
        }

        public void FillEllipse(Brush brush, Rectangle rect)
        {
            _g.FillEllipse(brush, rect);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            _g.FillEllipse(brush, x, y, width, height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            _g.FillPath(brush, path);
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            _g.FillPie(brush, rect, startAngle, sweepAngle);
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            _g.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            _g.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            _g.FillPolygon(brush, points);
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            _g.FillPolygon(brush, points, fillMode);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            _g.FillPolygon(brush, points);
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            _g.FillPolygon(brush, points, fillMode);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            _g.FillRectangle(brush, rect);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            _g.FillRectangle(brush, x, y, width, height);
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            _g.FillRectangle(brush, rect);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            _g.FillRectangle(brush, x, y, width, height);
        }

        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            _g.FillRectangles(brush, rects);
        }

        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            _g.FillRectangles(brush, rects);
        }

        public void FillRegion(Brush brush, Region region)
        {
            _g.FillRegion(brush, region);
        }

        public void Flush()
        {
            _g.Flush();
        }

        public void Flush(FlushIntention intention)
        {
            _g.Flush(intention);
        }

        public Color GetNearestColor(Color color) => _g.GetNearestColor(color);

        public void IntersectClip(Rectangle rect)
        {
            _g.IntersectClip(rect);
        }

        public void IntersectClip(RectangleF rect)
        {
            _g.IntersectClip(rect);
        }

        public void IntersectClip(Region region)
        {
            _g.IntersectClip(region);
        }

        public bool IsVisible(int x, int y) => _g.IsVisible(x, y);

        public bool IsVisible(Point point) => _g.IsVisible(point);

        public bool IsVisible(float x, float y) => _g.IsVisible(x, y);

        public bool IsVisible(PointF point) => _g.IsVisible(point);

        public bool IsVisible(int x, int y, int width, int height) => _g.IsVisible(x, y, width, height);

        public bool IsVisible(Rectangle rect) => _g.IsVisible(rect);

        public bool IsVisible(float x, float y, float width, float height) => _g.IsVisible(x, y, width, height);

        public bool IsVisible(RectangleF rect) => _g.IsVisible(rect);

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat) => _g.MeasureCharacterRanges(text, font, layoutRect, stringFormat);

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
        {
            int a, b; var siz = _g.MeasureString(text, font, layoutArea, stringFormat, out a, out b); charactersFitted = a; linesFilled = b; return siz;
        }

        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat) => _g.MeasureString(text, font, origin, stringFormat);

        public SizeF MeasureString(string text, Font font, SizeF layoutArea) => _g.MeasureString(text, font, layoutArea);

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat) => _g.MeasureString(text, font, layoutArea, stringFormat);

        public SizeF MeasureString(string text, Font font) => _g.MeasureString(text, font);

        public SizeF MeasureString(string text, Font font, int width) => _g.MeasureString(text, font, width);

        public SizeF MeasureString(string text, Font font, int width, StringFormat format) => _g.MeasureString(text, font, width, format);

        public void MultiplyTransform(Matrix matrix)
        {
            _g.MultiplyTransform(matrix);
        }

        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            _g.MultiplyTransform(matrix, order);
        }

        public void ResetClip()
        {
            _g.ResetClip();
        }

        public void ResetTransform()
        {
            _g.ResetTransform();
        }

        public void Restore(GraphicsState gstate)
        {
            _g.Restore(gstate);
        }

        public void RotateTransform(float angle)
        {
            _g.RotateTransform(angle);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            _g.RotateTransform(angle, order);
        }

        public GraphicsState Save() => _g.Save();

        public void ScaleTransform(float sx, float sy)
        {
            _g.ScaleTransform(sx, sy);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            _g.ScaleTransform(sx, sy);
        }

        public void SetClip(Graphics g)
        {
            _g.SetClip(g);
        }

        public void SetClip(Graphics g, CombineMode combineMode)
        {
            _g.SetClip(g, combineMode);
        }

        public void SetClip(Rectangle rect)
        {
            _g.SetClip(rect);
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            _g.SetClip(rect, combineMode);
        }

        public void SetClip(RectangleF rect)
        {
            _g.SetClip(rect);
        }

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            _g.SetClip(rect, combineMode);
        }

        public void SetClip(GraphicsPath path)
        {
            _g.SetClip(path);
        }

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            _g.SetClip(path, combineMode);
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            _g.SetClip(region, combineMode);
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
            _g.TransformPoints(destSpace, srcSpace, pts);
        }

        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
        {
            _g.TransformPoints(destSpace, srcSpace, pts);
        }

        public void TranslateClip(float dx, float dy)
        {
            _g.TranslateClip(dx, dy);
        }

        public void TranslateClip(int dx, int dy)
        {
            _g.TranslateClip(dx, dy);
        }

        public void TranslateTransform(float dx, float dy)
        {
            _g.TranslateTransform(dx, dy);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            _g.TranslateTransform(dx, dy, order);
        }

        private readonly Graphics _g;
    }
}
