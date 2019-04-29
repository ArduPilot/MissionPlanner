using System;
using System.Drawing;
using SkiaSharp;
using System.Geometry.Text;

namespace MissionPlanner.Utilities.Drawing
{
    public interface IGraphics: IDisposable
    {
        Region Clip { get; set; }
        RectangleF ClipBounds { get; }
        CompositingMode CompositingMode { get; set; }
        CompositingQuality CompositingQuality { get; set; }
        float DpiX { get; }
        float DpiY { get; }
        InterpolationMode InterpolationMode { get; set; }
        bool IsClipEmpty { get; }
        bool IsVisibleClipEmpty { get; }
        float PageScale { get; set; }
        GraphicsUnit PageUnit { get; set; }
        PixelOffsetMode PixelOffsetMode { get; set; }
        Point RenderingOrigin { get; set; }
        SmoothingMode SmoothingMode { get; set; }
        int TextContrast { get; set; }
        TextRenderingHint TextRenderingHint { get; set; }
        Matrix Transform { get; set; }
        RectangleF VisibleClipBounds { get; }
        void AddMetafileComment(Byte[] data);

        //   GraphicsContainer BeginContainer();

        //   GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit);

        //   GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit);

        void Clear(Color color);

     //   void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize);

      //  void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize);

      //  void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation);

      //  void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation);


        void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle);

        void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);

        void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle);

        void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4,
            float y4);

        void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4);

        void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4);

        void DrawBeziers(Pen pen, PointF[] points);

        void DrawBeziers(Pen pen, Point[] points);

        void DrawClosedCurve(Pen pen, PointF[] points);

        void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode);

        void DrawClosedCurve(Pen pen, Point[] points);

        void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode);

        void DrawCurve(Pen pen, PointF[] points);

        void DrawCurve(Pen pen, PointF[] points, float tension);

        void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments);

        void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);

        void DrawCurve(Pen pen, Point[] points);

        void DrawCurve(Pen pen, Point[] points, float tension);

        void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension);

        void DrawEllipse(Pen pen, RectangleF rect);

        void DrawEllipse(Pen pen, float x, float y, float width, float height);

        void DrawEllipse(Pen pen, Rectangle rect);

        void DrawEllipse(Pen pen, int x, int y, int width, int height);

        void DrawIcon(Icon icon, int x, int y);

        void DrawIcon(Icon icon, Rectangle targetRect);

        void DrawIconUnstretched(Icon icon, Rectangle targetRect);

        void DrawImage(Image image, int x, int y);

        void DrawImage(Image image, Rectangle rect);

        void DrawImage(Image image, int x, int y, int width, int height);

        void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttr);

        //     void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);

        void DrawImage(Image image, PointF point);

        void DrawImage(Image image, float x, float y);

        void DrawImage(Image image, RectangleF rect);

        void DrawImage(Image image, float x, float y, float width, float height);

        void DrawImage(Image image, Point point);

        void DrawImage(Image image, PointF[] destPoints);

        void DrawImage(Image image, Point[] destPoints);

        void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr);

        //   void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);

        //   void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);

        void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit);

        void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr);

        //   void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);

        //   void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);

        void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit);

        void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs);

        //   void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback);

        //    void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);

        void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit);

        //   void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);

        void DrawImageUnscaled(Image image, Point point);

        void DrawImageUnscaled(Image image, int x, int y);

        void DrawImageUnscaled(Image image, Rectangle rect);

        void DrawImageUnscaled(Image image, int x, int y, int width, int height);

       // void DrawImageUnscaledAndClipped(Image image, Rectangle rect);

        void DrawLine(Pen pen, int x1, int y1, int x2, int y2);

        void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

        void DrawLine(Pen pen, PointF pt1, PointF pt2);

        void DrawLine(Pen pen, Point pt1, Point pt2);

        void DrawLines(Pen pen, PointF[] points);

        void DrawLines(Pen pen, Point[] points);

        void DrawPath(Pen pen, GraphicsPath path);

        void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle);

        void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle);

        void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);

        void DrawPolygon(Pen pen, PointF[] points);

        void DrawPolygon(Pen pen, Point[] points);

        void DrawRectangle(Pen pen, Rectangle rect);

        void DrawRectangle(Pen pen, float x, float y, float width, float height);

        void DrawRectangle(Pen pen, int x, int y, int width, int height);

        void DrawRectangles(Pen pen, RectangleF[] rects);

        void DrawRectangles(Pen pen, Rectangle[] rects);

        void DrawString(String s, Font font, Brush brush, float x, float y);

        void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle);

        void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format);

        void DrawString(String s, Font font, Brush brush, PointF point);

        void DrawString(String s, Font font, Brush brush, float x, float y, StringFormat format);

        void DrawString(String s, Font font, Brush brush, PointF point, StringFormat format);
        /*
        void EndContainer(GraphicsContainer container);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);

        void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
        */

        void ExcludeClip(Region region);

        void ExcludeClip(Rectangle rect);

        void FillClosedCurve(Brush brush, PointF[] points);

        void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode);

        void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension);

        void FillClosedCurve(Brush brush, Point[] points);

        void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode);

        void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension);

        void FillEllipse(Brush brush, float x, float y, float width, float height);

        void FillEllipse(Brush brush, RectangleF rect);

        void FillEllipse(Brush brush, Rectangle rect);

        void FillEllipse(Brush brush, int x, int y, int width, int height);

        void FillPath(Brush brush, GraphicsPath path);

        void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle);

        void FillPie(Brush brush, float x, float y, float width, float height, float startAngle,
            float sweepAngle);

        void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle);

        void FillPolygon(Brush brush, Point[] points);

        void FillPolygon(Brush brush, PointF[] points);

        void FillPolygon(Brush brush, PointF[] points, FillMode fillMode);

        void FillPolygon(Brush brush, Point[] points, FillMode fillMode);

        void FillRectangle(Brush brush, float x, float y, float width, float height);

        void FillRectangle(Brush brush, Rectangle rect);

        void FillRectangle(Brush brush, int x, int y, int width, int height);

        void FillRectangle(Brush brush, RectangleF rect);

        void FillRectangles(Brush brush, Rectangle[] rects);

        void FillRectangles(Brush brush, RectangleF[] rects);

        void FillRegion(Brush brush, Region region);

        void Flush();

        void Flush(FlushIntention intention);

     //   Graphics FromHdc(IntPtr hdc);
     //   Graphics FromHdc(IntPtr hdc, IntPtr hdevice);

     //   Graphics FromHdcInternal(IntPtr hdc);
     //   Graphics FromHwnd(IntPtr hwnd);

     //   Graphics FromHwndInternal(IntPtr hwnd);
     //   Graphics FromImage(Image image);


     //   Object GetContextInfo();

     //   IntPtr GetHalftonePalette();

     //   IntPtr GetHdc();

    //    Object GetLifetimeService();

        Color GetNearestColor(Color color);

    //    Type GetType();

     //   Object InitializeLifetimeService();

        void IntersectClip(Rectangle rect);

        void IntersectClip(RectangleF rect);

        void IntersectClip(Region region);

        bool IsVisible(Rectangle rect);

        bool IsVisible(int x, int y);

        bool IsVisible(Point point);

        bool IsVisible(float x, float y);

        bool IsVisible(PointF point);

        bool IsVisible(int x, int y, int width, int height);

        bool IsVisible(float x, float y, float width, float height);

        bool IsVisible(RectangleF rect);

        Region[] MeasureCharacterRanges(String text, Font font, RectangleF layoutRect, StringFormat stringFormat);

        SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat);

        SizeF MeasureString(String text, Font font);

        SizeF MeasureString(String text, Font font, int width, StringFormat format);

        SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat, int charactersFitted,
            int linesFilled);

        SizeF MeasureString(String text, Font font, PointF origin, StringFormat stringFormat);

        SizeF MeasureString(String text, Font font, SizeF layoutArea);

        SizeF MeasureString(String text, Font font, int width);

        void MultiplyTransform(Matrix matrix);

        void MultiplyTransform(Matrix matrix, MatrixOrder order);

       // void ReleaseHdc(IntPtr hdc);
       // void ReleaseHdc();

       // void ReleaseHdcInternal(IntPtr hdc);
        void ResetClip();

        void ResetTransform();

        //    void Restore(GraphicsState gstate);

        void RotateTransform(float angle);

        void RotateTransform(float angle, MatrixOrder order);

        //     GraphicsState Save();

        void ScaleTransform(float sx, float sy);

        void ScaleTransform(float sx, float sy, MatrixOrder order);

        void SetClip(Rectangle rect);

        void SetClip(Region region, CombineMode combineMode);

        void SetClip(Graphics g);

        void SetClip(Graphics g, CombineMode combineMode);

        void SetClip(Rectangle rect, CombineMode combineMode);

        void SetClip(RectangleF rect);

        void SetClip(RectangleF rect, CombineMode combineMode);

        void SetClip(GraphicsPath path);

        void SetClip(GraphicsPath path, CombineMode combineMode);



        void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts);

        void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts);

        void TranslateClip(int dx, int dy);

        void TranslateClip(float dx, float dy);

        void TranslateTransform(float dx, float dy);
        void TranslateTransform(float dx, float dy, MatrixOrder order);
    }
}