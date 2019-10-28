using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Microsoft.AspNetCore.Components.Web;
using MissionPlanner.Drawing;
using MissionPlanner.Drawing.Drawing2D;

namespace wasm
{
    public class CanvasGraphics: BECanvas, IGraphics<Region,CompositingMode,CompositingQuality,InterpolationMode, GraphicsUnit, PixelOffsetMode, SmoothingMode, TextRenderingHint, Matrix, object, Pen, FillMode, Icon, Image, ImageAttributes, GraphicsPath, Font,Brush,StringFormat, FlushIntention, MatrixOrder, GraphicsState, CombineMode, CoordinateSpace>
    {
        public new int Width
        {
            get { return (int)base.Width; }
            set { }
        }
        public new int Height
        {
            get { return (int)base.Height; }
            set { }
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
        public Matrix Transform { get; set; }
        public RectangleF VisibleClipBounds { get; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool VSync { get; set; }
        public bool DesignMode { get; set; }
        public bool IsHandleCreated { get; set; }

        public void AddMetafileComment(byte[] data)
        {
            throw new NotImplementedException();
        }

        public object BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public object BeginContainer()
        {
            throw new NotImplementedException();
        }

        public object BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        public void Clear(Color color)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
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

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
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

        public void DrawImage(Image image, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            throw new NotImplementedException();
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            throw new NotImplementedException();
        }

        public void EndContainer(object container)
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

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
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

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted,
            out int linesFilled)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        public SizeF MeasureString(string text, Font font)
        {
            throw new NotImplementedException();
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

        public void ResetClip()
        {
            throw new NotImplementedException();
        }

        public void ResetTransform()
        {
            throw new NotImplementedException();
        }

        public void Restore(GraphicsState gstate)
        {
            throw new NotImplementedException();
        }

        public void RotateTransform(float angle)
        {
            throw new NotImplementedException();
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            throw new NotImplementedException();
        }

        public GraphicsState Save()
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
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

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            throw new NotImplementedException();
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

        public void TranslateTransform(float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            throw new NotImplementedException();
        }

        public virtual void Refresh()
        {
            throw new NotImplementedException();
        }

        protected void Invalidate()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnHandleCreated(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnHandleDestroyed(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnMouseClick(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnResize(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLoad(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
