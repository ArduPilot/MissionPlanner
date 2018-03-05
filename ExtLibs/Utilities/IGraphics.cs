using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;

namespace MissionPlanner.Utilities
{
    //
    // Summary:
    //     Encapsulates a GDI+ drawing surface. This class cannot be inherited.
    public abstract class IGraphics
    {
        //
        // Summary:
        //     Gets a value that specifies how composited images are drawn to this System.Drawing.Graphics.
        //
        // Returns:
        //     This property specifies a member of the System.Drawing.Drawing2D.CompositingMode
        //     enumeration. The default is System.Drawing.Drawing2D.CompositingMode.SourceOver.
        public CompositingMode CompositingMode { get; set; }

        //
        // Summary:
        //     Gets or sets the unit of measure used for page coordinates in this System.Drawing.Graphics.
        //
        // Returns:
        //     One of the System.Drawing.GraphicsUnit values other than System.Drawing.GraphicsUnit.World.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     System.Drawing.Graphics.PageUnit is set to System.Drawing.GraphicsUnit.World,
        //     which is not a physical unit.
        public GraphicsUnit PageUnit { get; set; }

        //
        // Summary:
        //     Gets or sets a copy of the geometric world transformation for this System.Drawing.Graphics.
        //
        // Returns:
        //     A copy of the System.Drawing.Drawing2D.Matrix that represents the geometric world
        //     transformation for this System.Drawing.Graphics.
        public System.Drawing.Drawing2D.Matrix Transform { get; set; }

        //
        // Summary:
        //     Gets or sets the interpolation mode associated with this System.Drawing.Graphics.
        //
        // Returns:
        //     One of the System.Drawing.Drawing2D.InterpolationMode values.
        public InterpolationMode InterpolationMode { get; set; }

        //
        // Summary:
        //     Gets the vertical resolution of this System.Drawing.Graphics.
        //
        // Returns:
        //     The value, in dots per inch, for the vertical resolution supported by this System.Drawing.Graphics.
        public float DpiY { get; }

        //
        // Summary:
        //     Gets or sets a System.Drawing.Region that limits the drawing region of this System.Drawing.Graphics.
        //
        // Returns:
        //     A System.Drawing.Region that limits the portion of this System.Drawing.Graphics
        //     that is currently available for drawing.
        public Region Clip { get; set; }

        //
        // Summary:
        //     Gets or set a value specifying how pixels are offset during rendering of this
        //     System.Drawing.Graphics.
        //
        // Returns:
        //     This property specifies a member of the System.Drawing.Drawing2D.PixelOffsetMode
        //     enumeration
        public PixelOffsetMode PixelOffsetMode { get; set; }

        //
        // Summary:
        //     Gets or sets the scaling between world units and page units for this System.Drawing.Graphics.
        //
        // Returns:
        //     This property specifies a value for the scaling between world units and page
        //     units for this System.Drawing.Graphics.
        public float PageScale { get; set; }

        //
        // Summary:
        //     Gets a System.Drawing.RectangleF structure that bounds the clipping region of
        //     this System.Drawing.Graphics.
        //
        // Returns:
        //     A System.Drawing.RectangleF structure that represents a bounding rectangle for
        //     the clipping region of this System.Drawing.Graphics.
        public RectangleF ClipBounds { get; }

        //
        // Summary:
        //     Gets the bounding rectangle of the visible clipping region of this System.Drawing.Graphics.
        //
        // Returns:
        //     A System.Drawing.RectangleF structure that represents a bounding rectangle for
        //     the visible clipping region of this System.Drawing.Graphics.
        public RectangleF VisibleClipBounds { get; }

        //
        // Summary:
        //     Gets a value indicating whether the visible clipping region of this System.Drawing.Graphics
        //     is empty.
        //
        // Returns:
        //     true if the visible portion of the clipping region of this System.Drawing.Graphics
        //     is empty; otherwise, false.
        public bool IsVisibleClipEmpty { get; }

        //
        // Summary:
        //     Gets or sets the rendering quality for this System.Drawing.Graphics.
        //
        // Returns:
        //     One of the System.Drawing.Drawing2D.SmoothingMode values.
        public SmoothingMode SmoothingMode { get; set; }

        //
        // Summary:
        //     Gets or sets the gamma correction value for rendering text.
        //
        // Returns:
        //     The gamma correction value used for rendering antialiased and ClearType text.
        public int TextContrast { get; set; }

        //
        // Summary:
        //     Gets or sets the rendering mode for text associated with this System.Drawing.Graphics.
        //
        // Returns:
        //     One of the System.Drawing.Text.TextRenderingHint values.
        public TextRenderingHint TextRenderingHint { get; set; }

        //
        // Summary:
        //     Gets or sets the rendering quality of composited images drawn to this System.Drawing.Graphics.
        //
        // Returns:
        //     This property specifies a member of the System.Drawing.Drawing2D.CompositingQuality
        //     enumeration. The default is System.Drawing.Drawing2D.CompositingQuality.Default.
        public CompositingQuality CompositingQuality { get; set; }

        //
        // Summary:
        //     Gets or sets the rendering origin of this System.Drawing.Graphics for dithering
        //     and for hatch brushes.
        //
        // Returns:
        //     A System.Drawing.Point structure that represents the dither origin for 8-bits-per-pixel
        //     and 16-bits-per-pixel dithering and is also used to set the origin for hatch
        //     brushes.
        public Point RenderingOrigin { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether the clipping region of this System.Drawing.Graphics
        //     is empty.
        //
        // Returns:
        //     true if the clipping region of this System.Drawing.Graphics is empty; otherwise,
        //     false.
        public bool IsClipEmpty { get; }

        //
        // Summary:
        //     Gets the horizontal resolution of this System.Drawing.Graphics.
        //
        // Returns:
        //     The value, in dots per inch, for the horizontal resolution supported by this
        //     System.Drawing.Graphics.
        public float DpiX { get; }
        /*
        //
        // Summary:
        //     Creates a new System.Drawing.Graphics from the specified handle to a device context.
        //
        // Parameters:
        //   hdc:
        //     Handle to a device context.
        //
        // Returns:
        //     This method returns a new System.Drawing.Graphics for the specified device context.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract static Graphics FromHdc(IntPtr hdc);

        //
        // Summary:
        //     Creates a new System.Drawing.Graphics from the specified handle to a device context
        //     and handle to a device.
        //
        // Parameters:
        //   hdc:
        //     Handle to a device context.
        //
        //   hdevice:
        //     Handle to a device.
        //
        // Returns:
        //     This method returns a new System.Drawing.Graphics for the specified device context
        //     and device.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract static Graphics FromHdc(IntPtr hdc, IntPtr hdevice);

        //
        // Summary:
        //     Returns a System.Drawing.Graphics for the specified device context.
        //
        // Parameters:
        //   hdc:
        //     Handle to a device context.
        //
        // Returns:
        //     A System.Drawing.Graphics for the specified device context.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract static Graphics FromHdcInternal(IntPtr hdc);

        //
        // Summary:
        //     Creates a new System.Drawing.Graphics from the specified handle to a window.
        //
        // Parameters:
        //   hwnd:
        //     Handle to a window.
        //
        // Returns:
        //     This method returns a new System.Drawing.Graphics for the specified window handle.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract static Graphics FromHwnd(IntPtr hwnd);

        //
        // Summary:
        //     Creates a new System.Drawing.Graphics for the specified windows handle.
        //
        // Parameters:
        //   hwnd:
        //     Handle to a window.
        //
        // Returns:
        //     A System.Drawing.Graphics for the specified window handle.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract static Graphics FromHwndInternal(IntPtr hwnd);

        //
        // Summary:
        //     Creates a new System.Drawing.Graphics from the specified System.Drawing.Image.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image from which to create the new System.Drawing.Graphics.
        //
        // Returns:
        //     This method returns a new System.Drawing.Graphics for the specified System.Drawing.Image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        //
        //   T:System.Exception:
        //     image has an indexed pixel format or its format is undefined.
        public abstract static Graphics FromImage(Image image);

        //
        // Summary:
        //     Gets a handle to the current Windows halftone palette.
        //
        // Returns:
        //     Internal pointer that specifies the handle to the palette.
        public abstract static IntPtr GetHalftonePalette();
        */
        //
        // Summary:
        //     Adds a comment to the current System.Drawing.Imaging.Metafile.
        //
        // Parameters:
        //   data:
        //     Array of bytes that contains the comment.
        //public abstract void AddMetafileComment(byte[] data);

        //
        // Summary:
        //     Saves a graphics container with the current state of this System.Drawing.Graphics
        //     and opens and uses a new graphics container with the specified scale transformation.
        //
        // Parameters:
        //   dstrect:
        //     System.Drawing.RectangleF structure that, together with the srcrect parameter,
        //     specifies a scale transformation for the new graphics container.
        //
        //   srcrect:
        //     System.Drawing.RectangleF structure that, together with the dstrect parameter,
        //     specifies a scale transformation for the new graphics container.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure for the container.
        //
        // Returns:
        //     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
        //     the state of this System.Drawing.Graphics at the time of the method call.
        //public abstract GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit);

        //
        // Summary:
        //     Saves a graphics container with the current state of this System.Drawing.Graphics
        //     and opens and uses a new graphics container.
        //
        // Returns:
        //     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
        //     the state of this System.Drawing.Graphics at the time of the method call.
        //public abstract GraphicsContainer BeginContainer();

        //
        // Summary:
        //     Saves a graphics container with the current state of this System.Drawing.Graphics
        //     and opens and uses a new graphics container with the specified scale transformation.
        //
        // Parameters:
        //   dstrect:
        //     System.Drawing.Rectangle structure that, together with the srcrect parameter,
        //     specifies a scale transformation for the container.
        //
        //   srcrect:
        //     System.Drawing.Rectangle structure that, together with the dstrect parameter,
        //     specifies a scale transformation for the container.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure for the container.
        //
        // Returns:
        //     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
        //     the state of this System.Drawing.Graphics at the time of the method call.
        //public abstract GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit);

        //
        // Summary:
        //     Clears the entire drawing surface and fills it with the specified background
        //     color.
        //
        // Parameters:
        //   color:
        //     System.Drawing.Color structure that represents the background color of the drawing
        //     surface.
        public abstract void Clear(Color color);

        //
        // Summary:
        //     Performs a bit-block transfer of color data, corresponding to a rectangle of
        //     pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
        //
        // Parameters:
        //   upperLeftSource:
        //     The point at the upper-left corner of the source rectangle.
        //
        //   upperLeftDestination:
        //     The point at the upper-left corner of the destination rectangle.
        //
        //   blockRegionSize:
        //     The size of the area to be transferred.
        //
        //   copyPixelOperation:
        //     One of the System.Drawing.CopyPixelOperation values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     copyPixelOperation is not a member of System.Drawing.CopyPixelOperation.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     The operation failed.
        protected void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
        }

        //
        // Summary:
        //     Performs a bit-block transfer of color data, corresponding to a rectangle of
        //     pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
        //
        // Parameters:
        //   upperLeftSource:
        //     The point at the upper-left corner of the source rectangle.
        //
        //   upperLeftDestination:
        //     The point at the upper-left corner of the destination rectangle.
        //
        //   blockRegionSize:
        //     The size of the area to be transferred.
        //
        // Exceptions:
        //   T:System.ComponentModel.Win32Exception:
        //     The operation failed.
        protected void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
        {
            this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize);
        }

        //
        // Summary:
        //     Performs a bit-block transfer of the color data, corresponding to a rectangle
        //     of pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
        //
        // Parameters:
        //   sourceX:
        //     The x-coordinate of the point at the upper-left corner of the source rectangle.
        //
        //   sourceY:
        //     The y-coordinate of the point at the upper-left corner of the source rectangle.
        //
        //   destinationX:
        //     The x-coordinate of the point at the upper-left corner of the destination rectangle.
        //
        //   destinationY:
        //     The y-coordinate of the point at the upper-left corner of the destination rectangle.
        //
        //   blockRegionSize:
        //     The size of the area to be transferred.
        //
        // Exceptions:
        //   T:System.ComponentModel.Win32Exception:
        //     The operation failed.
        protected void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
        {
            this.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
        }

        //
        // Summary:
        //     Performs a bit-block transfer of the color data, corresponding to a rectangle
        //     of pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
        //
        // Parameters:
        //   sourceX:
        //     The x-coordinate of the point at the upper-left corner of the source rectangle.
        //
        //   sourceY:
        //     The y-coordinate of the point at the upper-left corner of the source rectangle
        //
        //   destinationX:
        //     The x-coordinate of the point at the upper-left corner of the destination rectangle.
        //
        //   destinationY:
        //     The y-coordinate of the point at the upper-left corner of the destination rectangle.
        //
        //   blockRegionSize:
        //     The size of the area to be transferred.
        //
        //   copyPixelOperation:
        //     One of the System.Drawing.CopyPixelOperation values.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     copyPixelOperation is not a member of System.Drawing.CopyPixelOperation.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     The operation failed.
        public abstract void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize,
            CopyPixelOperation copyPixelOperation);

        //
        // Summary:
        //     Releases all resources used by this System.Drawing.Graphics.
        //public abstract void Dispose();

        //
        // Summary:
        //     Draws an arc representing a portion of an ellipse specified by a pair of coordinates,
        //     a width, and a height.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the arc.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        //
        //   width:
        //     Width of the rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the rectangle that defines the ellipse.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the starting point of
        //     the arc.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to ending point
        //     of the arc.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-rects is null.
        //
        //   T:System.ArgumentNullException:
        //     rects is a zero-length array.
        public abstract void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);

        //
        // Summary:
        //     Draws an arc representing a portion of an ellipse specified by a System.Drawing.Rectangle
        //     structure.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the arc.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the starting point of
        //     the arc.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to ending point
        //     of the arc.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            this.DrawArc(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
        }
        //
        // Summary:
        //     Draws an arc representing a portion of an ellipse specified by a pair of coordinates,
        //     a width, and a height.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the arc.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        //
        //   width:
        //     Width of the rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the rectangle that defines the ellipse.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the starting point of
        //     the arc.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to ending point
        //     of the arc.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        //
        // Summary:
        //     Draws an arc representing a portion of an ellipse specified by a System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the arc.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the starting point of
        //     the arc.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to ending point
        //     of the arc.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null
        protected void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            this.DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        //
        // Summary:
        //     Draws a Bézier spline defined by four ordered pairs of coordinates that represent
        //     points.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   x1:
        //     The x-coordinate of the starting point of the curve.
        //
        //   y1:
        //     The y-coordinate of the starting point of the curve.
        //
        //   x2:
        //     The x-coordinate of the first control point of the curve.
        //
        //   y2:
        //     The y-coordinate of the first control point of the curve.
        //
        //   x3:
        //     The x-coordinate of the second control point of the curve.
        //
        //   y3:
        //     The y-coordinate of the second control point of the curve.
        //
        //   x4:
        //     The x-coordinate of the ending point of the curve.
        //
        //   y4:
        //     The y-coordinate of the ending point of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

        //
        // Summary:
        //     Draws a Bézier spline defined by four System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   pt1:
        //     System.Drawing.PointF structure that represents the starting point of the curve.
        //
        //   pt2:
        //     System.Drawing.PointF structure that represents the first control point for the
        //     curve.
        //
        //   pt3:
        //     System.Drawing.PointF structure that represents the second control point for
        //     the curve.
        //
        //   pt4:
        //     System.Drawing.PointF structure that represents the ending point of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            this.DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        //
        // Summary:
        //     Draws a Bézier spline defined by four System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen structure that determines the color, width, and style of the
        //     curve.
        //
        //   pt1:
        //     System.Drawing.Point structure that represents the starting point of the curve.
        //
        //   pt2:
        //     System.Drawing.Point structure that represents the first control point for the
        //     curve.
        //
        //   pt3:
        //     System.Drawing.Point structure that represents the second control point for the
        //     curve.
        //
        //   pt4:
        //     System.Drawing.Point structure that represents the ending point of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            this.DrawBezier(pen, (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y, (float)pt3.X, (float)pt3.Y, (float)pt4.X, (float)pt4.Y);
        }
        //
        // Summary:
        //     Draws a series of Bézier splines from an array of System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the points that determine
        //     the curve. The number of points in the array should be a multiple of 3 plus 1,
        //     such as 4, 7, or 10.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawBeziers(Pen pen, PointF[] points);

        //
        // Summary:
        //     Draws a series of Bézier splines from an array of System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that represent the points that determine
        //     the curve. The number of points in the array should be a multiple of 3 plus 1,
        //     such as 4, 7, or 10.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawBeziers(Pen pen, Point[] points);

        //
        // Summary:
        //     Draws a closed cardinal spline defined by an array of System.Drawing.PointF structures
        //     using a specified tension.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and height of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled. This parameter is required but is ignored.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode);

        //
        // Summary:
        //     Draws a closed cardinal spline defined by an array of System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and height of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawClosedCurve(Pen pen, PointF[] points);

        //
        // Summary:
        //     Draws a closed cardinal spline defined by an array of System.Drawing.Point structures
        //     using a specified tension.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and height of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled. This parameter is required but ignored.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode);

        //
        // Summary:
        //     Draws a closed cardinal spline defined by an array of System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and height of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawClosedCurve(Pen pen, Point[] points);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.PointF structures
        //     using a specified tension.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the points that define
        //     the curve.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, PointF[] points, float tension);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.Point structures
        //     using a specified tension.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        //   offset:
        //     Offset from the first element in the array of the points parameter to the starting
        //     point in the curve.
        //
        //   numberOfSegments:
        //     Number of segments after the starting point to include in the curve.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.Point structures
        //     using a specified tension.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, Point[] points, float tension);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and height of the curve.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, Point[] points);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.PointF structures
        //     using a specified tension. The drawing begins offset from the beginning of the
        //     array.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        //   offset:
        //     Offset from the first element in the array of the points parameter to the starting
        //     point in the curve.
        //
        //   numberOfSegments:
        //     Number of segments after the starting point to include in the curve.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);

        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.PointF structures.
        //     The drawing begins offset from the beginning of the array.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        //   offset:
        //     Offset from the first element in the array of the points parameter to the starting
        //     point in the curve.
        //
        //   numberOfSegments:
        //     Number of segments after the starting point to include in the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        protected void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            this.DrawCurve(pen, points, offset, numberOfSegments, 0.5f);
        }
        //
        // Summary:
        //     Draws a cardinal spline through a specified array of System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the curve.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawCurve(Pen pen, PointF[] points);

        //
        // Summary:
        //     Draws an ellipse defined by a bounding System.Drawing.RectangleF.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the ellipse.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawEllipse(Pen pen, RectangleF rect)
        {
            this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        //
        // Summary:
        //     Draws an ellipse defined by a bounding rectangle specified by a pair of coordinates,
        //     a height, and a width.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the ellipse.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawEllipse(Pen pen, float x, float y, float width, float height);

        //
        // Summary:
        //     Draws an ellipse specified by a bounding System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the ellipse.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that defines the boundaries of the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawEllipse(Pen pen, Rectangle rect)
        {
            this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        //
        // Summary:
        //     Draws an ellipse defined by a bounding rectangle specified by coordinates for
        //     the upper-left corner of the rectangle, a height, and a width.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the ellipse.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawEllipse(Pen pen, int x, int y, int width, int height);

        //
        // Summary:
        //     Draws the image represented by the specified System.Drawing.Icon at the specified
        //     coordinates.
        //
        // Parameters:
        //   icon:
        //     System.Drawing.Icon to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     icon is null.
        protected void DrawIcon(Icon icon, int x, int y)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }
           // if (this.backingImage != null)
            {
                this.DrawImage(icon.ToBitmap(), x, y);
            }
           // else
            {
               // icon.Draw(this, x, y);
            }
        }

        //
        // Summary:
        //     Draws the image represented by the specified System.Drawing.Icon within the area
        //     specified by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   icon:
        //     System.Drawing.Icon to draw.
        //
        //   targetRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     resulting image on the display surface. The image contained in the icon parameter
        //     is scaled to the dimensions of this rectangular area.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     icon is null.
        protected void DrawIcon(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }
            //if (this.backingImage != null)
            {
                this.DrawImage(icon.ToBitmap(), targetRect);
            }
            //else
            {
               // icon.Draw(this, targetRect);
            }
        }

        //
        // Summary:
        //     Draws the image represented by the specified System.Drawing.Icon without scaling
        //     the image.
        //
        // Parameters:
        //   icon:
        //     System.Drawing.Icon to draw.
        //
        //   targetRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     resulting image. The image is not scaled to fit this rectangle, but retains its
        //     original size. If the image is larger than the rectangle, it is clipped to fit
        //     inside it.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     icon is null.
        protected void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }
           // if (this.backingImage != null)
            {
                this.DrawImageUnscaled(icon.ToBitmap(), targetRect);
            }
            //else
            {
               // icon.DrawUnstretched(this, targetRect);
            }
        }
        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image, using its original physical size, at
        //     the specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   point:
        //     System.Drawing.PointF structure that represents the upper-left corner of the
        //     drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImage(Image image, PointF point)
        {
            this.DrawImage(image, point.X, point.Y);
        }

        //
        // Summary:
        //     Draws the specified System.Drawing.Image, using its original physical size, at
        //     the specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, float x, float y);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, RectangleF rect);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        //   width:
        //     Width of the drawn image.
        //
        //   height:
        //     Height of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, float x, float y, float width, float height);

        //
        // Summary:
        //     Draws the specified image, using its original physical size, at the location
        //     specified by a coordinate pair.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, int x, int y);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImage(Image image, Rectangle rect)
        {
            this.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        //   width:
        //     Width of the drawn image.
        //
        //   height:
        //     Height of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, int x, int y, int width, int height);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified shape and size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, PointF[] destPoints);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image at the specified location and with the
        //     specified shape and size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Point[] destPoints);

        //
        // Summary:
        //     Draws a portion of an image at a specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the System.Drawing.Image
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws a portion of an image at a specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
        //     method according to application-determined criteria.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr = null, DrawImageAbort callback = null);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttrs:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)
        //     method according to application-determined criteria.
        //
        //   callbackData:
        //     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
        //     delegate to use when checking whether to stop execution of the DrawImage method.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttr);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttrs:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)
        //     method according to application-determined criteria.
        //
        //   callbackData:
        //     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
        //     delegate to use when checking whether to stop execution of the DrawImage method.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttrs:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
        //     method according to application-determined criteria.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttrs:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttrs);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
        //     method according to application-determined criteria.
        //
        //   callbackData:
        //     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
        //     delegate to use when checking whether to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
        //     method.
        public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
        //     method according to application-determined criteria.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr, DrawImageAbort callback);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the image object
        //     to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used by the srcRect parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for the image object.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
        //     method according to application-determined criteria.
        //
        //   callbackData:
        //     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
        //     delegate to use when checking whether to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
        //     method.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);

        //
        // Summary:
        //     Draws the specified portion of the specified System.Drawing.Image at the specified
        //     location and with the specified size.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn image. The image is scaled to fit the rectangle.
        //
        //   srcX:
        //     The x-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcY:
        //     The y-coordinate of the upper-left corner of the portion of the source image
        //     to draw.
        //
        //   srcWidth:
        //     Width of the portion of the source image to draw.
        //
        //   srcHeight:
        //     Height of the portion of the source image to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
        //     of measure used to determine the source rectangle.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
        //     for image.
        //
        //   callback:
        //     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
        //     during the drawing of the image. This method is called frequently to check whether
        //     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
        //     method according to application-determined criteria.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);

        //
        // Summary:
        //     Draws the specified System.Drawing.Image, using its original physical size, at
        //     the specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   point:
        //     System.Drawing.Point structure that represents the location of the upper-left
        //     corner of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        public abstract void DrawImage(Image image, Point point);

        //
        // Summary:
        //     Draws a specified image using its original physical size at a specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   point:
        //     System.Drawing.Point structure that specifies the upper-left corner of the drawn
        //     image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImageUnscaled(Image image, Point point)
        {
            this.DrawImage(image, point.X, point.Y);
        }

        //
        // Summary:
        //     Draws the specified image using its original physical size at the location specified
        //     by a coordinate pair.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImageUnscaled(Image image, int x, int y)
        {
            this.DrawImage(image, x, y);
        }

        //
        // Summary:
        //     Draws a specified image using its original physical size at a specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   rect:
        //     System.Drawing.Rectangle that specifies the upper-left corner of the drawn image.
        //     The X and Y properties of the rectangle specify the upper-left corner. The Width
        //     and Height properties are ignored.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImageUnscaled(Image image, Rectangle rect)
        {
            this.DrawImage(image, rect.X, rect.Y);
        }
        //
        // Summary:
        //     Draws a specified image using its original physical size at a specified location.
        //
        // Parameters:
        //   image:
        //     System.Drawing.Image to draw.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn image.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn image.
        //
        //   width:
        //     Not used.
        //
        //   height:
        //     Not used.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            this.DrawImage(image, x, y);
        }
        //
        // Summary:
        //     Draws the specified image without scaling and clips it, if necessary, to fit
        //     in the specified rectangle.
        //
        // Parameters:
        //   image:
        //     The System.Drawing.Image to draw.
        //
        //   rect:
        //     The System.Drawing.Rectangle in which to draw the image.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     image is null.
        protected void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            int srcWidth = Math.Min(rect.Width, image.Width);
            int srcHeight = Math.Min(rect.Height, image.Height);
            this.DrawImage(image, rect, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
        }
        //
        // Summary:
        //     Draws a line connecting the two points specified by the coordinate pairs.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line.
        //
        //   x1:
        //     The x-coordinate of the first point.
        //
        //   y1:
        //     The y-coordinate of the first point.
        //
        //   x2:
        //     The x-coordinate of the second point.
        //
        //   y2:
        //     The y-coordinate of the second point.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

        //
        // Summary:
        //     Draws a line connecting two System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line.
        //
        //   pt1:
        //     System.Drawing.PointF structure that represents the first point to connect.
        //
        //   pt2:
        //     System.Drawing.PointF structure that represents the second point to connect.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            this.DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        //
        // Summary:
        //     Draws a line connecting the two points specified by the coordinate pairs.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line.
        //
        //   x1:
        //     The x-coordinate of the first point.
        //
        //   y1:
        //     The y-coordinate of the first point.
        //
        //   x2:
        //     The x-coordinate of the second point.
        //
        //   y2:
        //     The y-coordinate of the second point.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawLine(Pen pen, int x1, int y1, int x2, int y2);

        //
        // Summary:
        //     Draws a line connecting two System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line.
        //
        //   pt1:
        //     System.Drawing.Point structure that represents the first point to connect.
        //
        //   pt2:
        //     System.Drawing.Point structure that represents the second point to connect.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            this.DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        //
        // Summary:
        //     Draws a series of line segments that connect an array of System.Drawing.PointF
        //     structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line segments.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the points to connect.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawLines(Pen pen, PointF[] points);

        //
        // Summary:
        //     Draws a series of line segments that connect an array of System.Drawing.Point
        //     structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the line segments.
        //
        //   points:
        //     Array of System.Drawing.Point structures that represent the points to connect.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawLines(Pen pen, Point[] points);

        //
        // Summary:
        //     Draws a System.Drawing.Drawing2D.GraphicsPath.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the path.
        //
        //   path:
        //     System.Drawing.Drawing2D.GraphicsPath to draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-path is null.
        public abstract void DrawPath(Pen pen, GraphicsPath path);

        //
        // Summary:
        //     Draws a pie shape defined by an ellipse specified by a coordinate pair, a width,
        //     a height, and two radial lines.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the pie shape.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie shape comes.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie shape comes.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse from which the pie shape
        //     comes.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse from which the pie
        //     shape comes.
        //
        //   startAngle:
        //     Angle measured in degrees clockwise from the x-axis to the first side of the
        //     pie shape.
        //
        //   sweepAngle:
        //     Angle measured in degrees clockwise from the startAngle parameter to the second
        //     side of the pie shape.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        //
        // Summary:
        //     Draws a pie shape defined by an ellipse specified by a System.Drawing.Rectangle
        //     structure and two radial lines.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the pie shape.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that represents the bounding rectangle that
        //     defines the ellipse from which the pie shape comes.
        //
        //   startAngle:
        //     Angle measured in degrees clockwise from the x-axis to the first side of the
        //     pie shape.
        //
        //   sweepAngle:
        //     Angle measured in degrees clockwise from the startAngle parameter to the second
        //     side of the pie shape.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            this.DrawPie(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
        }

        //
        // Summary:
        //     Draws a pie shape defined by an ellipse specified by a coordinate pair, a width,
        //     a height, and two radial lines.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the pie shape.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie shape comes.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie shape comes.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse from which the pie shape
        //     comes.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse from which the pie
        //     shape comes.
        //
        //   startAngle:
        //     Angle measured in degrees clockwise from the x-axis to the first side of the
        //     pie shape.
        //
        //   sweepAngle:
        //     Angle measured in degrees clockwise from the startAngle parameter to the second
        //     side of the pie shape.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);

        //
        // Summary:
        //     Draws a pie shape defined by an ellipse specified by a System.Drawing.RectangleF
        //     structure and two radial lines.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the pie shape.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that represents the bounding rectangle that
        //     defines the ellipse from which the pie shape comes.
        //
        //   startAngle:
        //     Angle measured in degrees clockwise from the x-axis to the first side of the
        //     pie shape.
        //
        //   sweepAngle:
        //     Angle measured in degrees clockwise from the startAngle parameter to the second
        //     side of the pie shape.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            this.DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        //
        // Summary:
        //     Draws a polygon defined by an array of System.Drawing.PointF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the polygon.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the vertices of the
        //     polygon.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-points is null.
        public abstract void DrawPolygon(Pen pen, PointF[] points);

        //
        // Summary:
        //     Draws a polygon defined by an array of System.Drawing.Point structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the polygon.
        //
        //   points:
        //     Array of System.Drawing.Point structures that represent the vertices of the polygon.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawPolygon(Pen pen, Point[] points);

        //
        // Summary:
        //     Draws a rectangle specified by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   pen:
        //     A System.Drawing.Pen that determines the color, width, and style of the rectangle.
        //
        //   rect:
        //     A System.Drawing.Rectangle structure that represents the rectangle to draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        protected void DrawRectangle(Pen pen, Rectangle rect)
        {
            this.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        //
        // Summary:
        //     Draws a rectangle specified by a coordinate pair, a width, and a height.
        //
        // Parameters:
        //   pen:
        //     A System.Drawing.Pen that determines the color, width, and style of the rectangle.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to draw.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to draw.
        //
        //   width:
        //     The width of the rectangle to draw.
        //
        //   height:
        //     The height of the rectangle to draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawRectangle(Pen pen, float x, float y, float width, float height);

        //
        // Summary:
        //     Draws a rectangle specified by a coordinate pair, a width, and a height.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the rectangle.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to draw.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to draw.
        //
        //   width:
        //     Width of the rectangle to draw.
        //
        //   height:
        //     Height of the rectangle to draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.
        public abstract void DrawRectangle(Pen pen, int x, int y, int width, int height);

        //
        // Summary:
        //     Draws a series of rectangles specified by System.Drawing.Rectangle structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the outlines
        //     of the rectangles.
        //
        //   rects:
        //     Array of System.Drawing.Rectangle structures that represent the rectangles to
        //     draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-rects is null.
        //
        //   T:System.ArgumentException:
        //     rects is a zero-length array.
        public abstract void DrawRectangles(Pen pen, Rectangle[] rects);

        //
        // Summary:
        //     Draws a series of rectangles specified by System.Drawing.RectangleF structures.
        //
        // Parameters:
        //   pen:
        //     System.Drawing.Pen that determines the color, width, and style of the outlines
        //     of the rectangles.
        //
        //   rects:
        //     Array of System.Drawing.RectangleF structures that represent the rectangles to
        //     draw.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     pen is null.-or-rects is null.
        //
        //   T:System.ArgumentException:
        //     rects is a zero-length array.
        public abstract void DrawRectangles(Pen pen, RectangleF[] rects);

        //
        // Summary:
        //     Draws the specified text string in the specified rectangle with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
        //     of the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   layoutRectangle:
        //     System.Drawing.RectangleF structure that specifies the location of the drawn
        //     text.
        //
        //   format:
        //     System.Drawing.StringFormat that specifies formatting attributes, such as line
        //     spacing and alignment, that are applied to the drawn text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        public abstract void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format);

        //
        // Summary:
        //     Draws the specified text string in the specified rectangle with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   layoutRectangle:
        //     System.Drawing.RectangleF structure that specifies the location of the drawn
        //     text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        protected void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            this.DrawString(s, font, brush, layoutRectangle, null);
        }
        //
        // Summary:
        //     Draws the specified text string at the specified location with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
        //     of the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   point:
        //     System.Drawing.PointF structure that specifies the upper-left corner of the drawn
        //     text.
        //
        //   format:
        //     System.Drawing.StringFormat that specifies formatting attributes, such as line
        //     spacing and alignment, that are applied to the drawn text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        protected void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
        }
        //
        // Summary:
        //     Draws the specified text string at the specified location with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
        //     of the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn text.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn text.
        //
        //   format:
        //     System.Drawing.StringFormat that specifies formatting attributes, such as line
        //     spacing and alignment, that are applied to the drawn text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        protected void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), format);
        }

        //
        // Summary:
        //     Draws the specified text string at the specified location with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   point:
        //     System.Drawing.PointF structure that specifies the upper-left corner of the drawn
        //     text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        protected void DrawString(string s, Font font, Brush brush, PointF point)
        {
            this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
        }
        //
        // Summary:
        //     Draws the specified text string at the specified location with the specified
        //     System.Drawing.Brush and System.Drawing.Font objects.
        //
        // Parameters:
        //   s:
        //     String to draw.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   brush:
        //     System.Drawing.Brush that determines the color and texture of the drawn text.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the drawn text.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the drawn text.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-s is null.
        protected void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
        }
        //
        // Summary:
        //     Closes the current graphics container and restores the state of this System.Drawing.Graphics
        //     to the state saved by a call to the System.Drawing.Graphics.BeginContainer method.
        //
        // Parameters:
        //   container:
        //     System.Drawing.Drawing2D.GraphicsContainer that represents the container this
        //     method restores.
        public abstract void EndContainer(GraphicsContainer container);
        /*
        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point using specified image
        //     attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point using specified image
        //     attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram
        //     using specified image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram
        //     using specified image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified parallelogram.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structures that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle using
        //     specified image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.PointF structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle using
        //     specified image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.Point structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.Rectangle structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback,
            IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   unit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit,
            EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display at a specified point.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoint:
        //     System.Drawing.PointF structure that specifies the location of the upper-left
        //     corner of the drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback);

        //
        // Summary:
        //     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified parallelogram using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destPoints:
        //     Array of three System.Drawing.Point structures that define a parallelogram that
        //     determines the size and location of the drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);

        //
        // Summary:
        //     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
        //     one at a time, to a callback method for display in a specified rectangle.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.RectangleF structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   srcRect:
        //     System.Drawing.RectangleF structure that specifies the portion of the metafile,
        //     relative to its upper-left corner, to draw.
        //
        //   srcUnit:
        //     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
        //     of measure used to determine the portion of the metafile that the rectangle specified
        //     by the srcRect parameter contains.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit,
            EnumerateMetafileProc callback, IntPtr callbackData);

        //
        // Summary:
        //     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
        //     time, to a callback method for display in a specified rectangle using specified
        //     image attributes.
        //
        // Parameters:
        //   metafile:
        //     System.Drawing.Imaging.Metafile to enumerate.
        //
        //   destRect:
        //     System.Drawing.Rectangle structure that specifies the location and size of the
        //     drawn metafile.
        //
        //   callback:
        //     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
        //     to which the metafile records are sent.
        //
        //   callbackData:
        //     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
        //     for this parameter.
        //
        //   imageAttr:
        //     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
        //     for the drawn image.
        public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback,
            IntPtr callbackData, ImageAttributes imageAttr);
            */
        //
        // Summary:
        //     Updates the clip region of this System.Drawing.Graphics to exclude the area specified
        //     by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.Rectangle structure that specifies the rectangle to exclude from
        //     the clip region.
        public abstract void ExcludeClip(Rectangle rect);

        //
        // Summary:
        //     Updates the clip region of this System.Drawing.Graphics to exclude the area specified
        //     by a System.Drawing.Region.
        //
        // Parameters:
        //   region:
        //     System.Drawing.Region that specifies the region to exclude from the clip region.
        public abstract void ExcludeClip(Region region);

        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
        //     structures using the specified fill mode.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        protected void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            this.FillClosedCurve(brush, points, fillmode, 0.5f);
        }

        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
        //     structures using the specified fill mode and tension.
        //
        // Parameters:
        //   brush:
        //     A System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension);

        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillClosedCurve(Brush brush, Point[] points);

        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
        //     structures using the specified fill mode and tension.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled.
        //
        //   tension:
        //     Value greater than or equal to 0.0F that specifies the tension of the curve.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension);

        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
        //     structures using the specified fill mode.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.Point structures that define the spline.
        //
        //   fillmode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
        //     the curve is filled.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        protected void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            this.FillClosedCurve(brush, points, fillmode, 0.5f);
        }
        //
        // Summary:
        //     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that define the spline.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillClosedCurve(Brush brush, PointF[] points);

        //
        // Summary:
        //     Fills the interior of an ellipse defined by a bounding rectangle specified by
        //     a System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that represents the bounding rectangle that
        //     defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        protected void FillEllipse(Brush brush, RectangleF rect)
        {
            this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        //
        // Summary:
        //     Fills the interior of an ellipse defined by a bounding rectangle specified by
        //     a pair of coordinates, a width, and a height.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillEllipse(Brush brush, float x, float y, float width, float height);

        //
        // Summary:
        //     Fills the interior of an ellipse defined by a bounding rectangle specified by
        //     a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that represents the bounding rectangle that
        //     defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        protected void FillEllipse(Brush brush, Rectangle rect)
        {
            this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        //
        // Summary:
        //     Fills the interior of an ellipse defined by a bounding rectangle specified by
        //     a pair of coordinates, a width, and a height.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillEllipse(Brush brush, int x, int y, int width, int height);

        //
        // Summary:
        //     Fills the interior of a System.Drawing.Drawing2D.GraphicsPath.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   path:
        //     System.Drawing.Drawing2D.GraphicsPath that represents the path to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-path is null.
        public abstract void FillPath(Brush brush, GraphicsPath path);

        //
        // Summary:
        //     Fills the interior of a pie section defined by an ellipse specified by a pair
        //     of coordinates, a width, a height, and two radial lines.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie section comes.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie section comes.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse from which the pie section
        //     comes.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse from which the pie
        //     section comes.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the first side of the
        //     pie section.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to the second
        //     side of the pie section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle);

        //
        // Summary:
        //     Fills the interior of a pie section defined by an ellipse specified by a System.Drawing.RectangleF
        //     structure and two radial lines.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that represents the bounding rectangle that
        //     defines the ellipse from which the pie section comes.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the first side of the
        //     pie section.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to the second
        //     side of the pie section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        protected void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            this.FillPie(brush, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
        }
        //
        // Summary:
        //     Fills the interior of a pie section defined by an ellipse specified by a pair
        //     of coordinates, a width, a height, and two radial lines.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie section comes.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the bounding rectangle that defines
        //     the ellipse from which the pie section comes.
        //
        //   width:
        //     Width of the bounding rectangle that defines the ellipse from which the pie section
        //     comes.
        //
        //   height:
        //     Height of the bounding rectangle that defines the ellipse from which the pie
        //     section comes.
        //
        //   startAngle:
        //     Angle in degrees measured clockwise from the x-axis to the first side of the
        //     pie section.
        //
        //   sweepAngle:
        //     Angle in degrees measured clockwise from the startAngle parameter to the second
        //     side of the pie section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillPie(Brush brush, float x, float y, float width, float height, float startAngle,
            float sweepAngle);

        //
        // Summary:
        //     Fills the interior of a polygon defined by an array of points specified by System.Drawing.PointF
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the vertices of the
        //     polygon to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        protected void FillPolygon(Brush brush, PointF[] points)
        {
            this.FillPolygon(brush, points, FillMode.Alternate);
        }
        //
        // Summary:
        //     Fills the interior of a polygon defined by an array of points specified by System.Drawing.PointF
        //     structures using the specified fill mode.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.PointF structures that represent the vertices of the
        //     polygon to fill.
        //
        //   fillMode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the
        //     style of the fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillPolygon(Brush brush, PointF[] points, FillMode fillMode);

        //
        // Summary:
        //     Fills the interior of a polygon defined by an array of points specified by System.Drawing.Point
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.Point structures that represent the vertices of the polygon
        //     to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        protected void FillPolygon(Brush brush, Point[] points)
        {
            this.FillPolygon(brush, points, FillMode.Alternate);
        }
        //
        // Summary:
        //     Fills the interior of a polygon defined by an array of points specified by System.Drawing.Point
        //     structures using the specified fill mode.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   points:
        //     Array of System.Drawing.Point structures that represent the vertices of the polygon
        //     to fill.
        //
        //   fillMode:
        //     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the
        //     style of the fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-points is null.
        public abstract void FillPolygon(Brush brush, Point[] points, FillMode fillMode);

        //
        // Summary:
        //     Fills the interior of a rectangle specified by a pair of coordinates, a width,
        //     and a height.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to fill.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to fill.
        //
        //   width:
        //     Width of the rectangle to fill.
        //
        //   height:
        //     Height of the rectangle to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillRectangle(Brush brush, float x, float y, float width, float height);

        //
        // Summary:
        //     Fills the interior of a rectangle specified by a pair of coordinates, a width,
        //     and a height.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to fill.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to fill.
        //
        //   width:
        //     Width of the rectangle to fill.
        //
        //   height:
        //     Height of the rectangle to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        public abstract void FillRectangle(Brush brush, int x, int y, int width, int height);

        //
        // Summary:
        //     Fills the interior of a rectangle specified by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rect:
        //     System.Drawing.Rectangle structure that represents the rectangle to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        protected void FillRectangle(Brush brush, Rectangle rect)
        {
            this.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        //
        // Summary:
        //     Fills the interior of a rectangle specified by a System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rect:
        //     System.Drawing.RectangleF structure that represents the rectangle to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.
        protected void FillRectangle(Brush brush, RectangleF rect)
        {
            this.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
        //
        // Summary:
        //     Fills the interiors of a series of rectangles specified by System.Drawing.Rectangle
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rects:
        //     Array of System.Drawing.Rectangle structures that represent the rectangles to
        //     fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-rects is null.
        //
        //   T:System.ArgumentException:
        //     rects is a zero-length array.
        public abstract void FillRectangles(Brush brush, Rectangle[] rects);

        //
        // Summary:
        //     Fills the interiors of a series of rectangles specified by System.Drawing.RectangleF
        //     structures.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   rects:
        //     Array of System.Drawing.RectangleF structures that represent the rectangles to
        //     fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-rects is null.
        //
        //   T:System.ArgumentException:
        //     Rects is a zero-length array.
        public abstract void FillRectangles(Brush brush, RectangleF[] rects);

        //
        // Summary:
        //     Fills the interior of a System.Drawing.Region.
        //
        // Parameters:
        //   brush:
        //     System.Drawing.Brush that determines the characteristics of the fill.
        //
        //   region:
        //     System.Drawing.Region that represents the area to fill.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     brush is null.-or-region is null.
        public abstract void FillRegion(Brush brush, Region region);

        //
        // Summary:
        //     Forces execution of all pending graphics operations with the method waiting or
        //     not waiting, as specified, to return before the operations finish.
        //
        // Parameters:
        //   intention:
        //     Member of the System.Drawing.Drawing2D.FlushIntention enumeration that specifies
        //     whether the method returns immediately or waits for any existing operations to
        //     finish.
        public abstract void Flush(FlushIntention intention);

        //
        // Summary:
        //     Forces execution of all pending graphics operations and returns immediately without
        //     waiting for the operations to finish.
        protected void Flush()
        {
            this.Flush(FlushIntention.Flush);
        }
        //
        // Summary:
        //     Gets the cumulative graphics context.
        //
        // Returns:
        //     An System.Object representing the cumulative graphics context.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected object GetContextInfo()
        {
            Region clip = this.Clip;
            var transform = this.Transform;
            PointF pointF = PointF.Empty;
            PointF empty = PointF.Empty;
            if (!transform.IsIdentity)
            {
                float[] elements = transform.Elements;
                pointF.X = elements[4];
                pointF.Y = elements[5];
            }
         /*   GraphicsContext previous = this.previousContext;
            while (previous != null)
            {
                PointF transformOffset = previous.TransformOffset;
                if (!transformOffset.IsEmpty)
                {
                    Matrix matrix = transform;
                    transformOffset = previous.TransformOffset;
                    float x = transformOffset.X;
                    transformOffset = previous.TransformOffset;
                    matrix.Translate(x, transformOffset.Y);
                }
                if (!pointF.IsEmpty)
                {
                    clip.Translate(pointF.X, pointF.Y);
                    empty.X += pointF.X;
                    empty.Y += pointF.Y;
                }
                if (previous.Clip != null)
                {
                    clip.Intersect(previous.Clip);
                }
                pointF = previous.TransformOffset;
                do
                {
                    previous = previous.Previous;
                }
                while (previous != null && previous.Next.IsCumulative && previous.IsCumulative);
            }*/
            if (!empty.IsEmpty)
            {
                clip.Translate(0f - empty.X, 0f - empty.Y);
            }
            return new object[2]
            {
                clip,
                transform
            };
        }
        //
        // Summary:
        //     Gets the handle to the device context associated with this System.Drawing.Graphics.
        //
        // Returns:
        //     Handle to the device context associated with this System.Drawing.Graphics.
        public abstract IntPtr GetHdc();

        //
        // Summary:
        //     Gets the nearest color to the specified System.Drawing.Color structure.
        //
        // Parameters:
        //   color:
        //     System.Drawing.Color structure for which to find a match.
        //
        // Returns:
        //     A System.Drawing.Color structure that represents the nearest color to the one
        //     specified with the color parameter.
        public abstract Color GetNearestColor(Color color);

        //
        // Summary:
        //     Updates the clip region of this System.Drawing.Graphics to the intersection of
        //     the current clip region and the specified System.Drawing.Region.
        //
        // Parameters:
        //   region:
        //     System.Drawing.Region to intersect with the current region.
        public abstract void IntersectClip(Region region);

        //
        // Summary:
        //     Updates the clip region of this System.Drawing.Graphics to the intersection of
        //     the current clip region and the specified System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.Rectangle structure to intersect with the current clip region.
        public abstract void IntersectClip(Rectangle rect);

        //
        // Summary:
        //     Updates the clip region of this System.Drawing.Graphics to the intersection of
        //     the current clip region and the specified System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.RectangleF structure to intersect with the current clip region.
        public abstract void IntersectClip(RectangleF rect);

        //
        // Summary:
        //     Indicates whether the point specified by a pair of coordinates is contained within
        //     the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the point to test for visibility.
        //
        //   y:
        //     The y-coordinate of the point to test for visibility.
        //
        // Returns:
        //     true if the point defined by the x and y parameters is contained within the visible
        //     clip region of this System.Drawing.Graphics; otherwise, false.
        protected bool IsVisible(int x, int y)
        {
            return this.IsVisible(new Point(x, y));
        }
        //
        // Summary:
        //     Indicates whether the specified System.Drawing.PointF structure is contained
        //     within the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   point:
        //     System.Drawing.PointF structure to test for visibility.
        //
        // Returns:
        //     true if the point specified by the point parameter is contained within the visible
        //     clip region of this System.Drawing.Graphics; otherwise, false.
        public abstract bool IsVisible(PointF point);

        //
        // Summary:
        //     Indicates whether the rectangle specified by a pair of coordinates, a width,
        //     and a height is contained within the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to test for visibility.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to test for visibility.
        //
        //   width:
        //     Width of the rectangle to test for visibility.
        //
        //   height:
        //     Height of the rectangle to test for visibility.
        //
        // Returns:
        //     true if the rectangle defined by the x, y, width, and height parameters is contained
        //     within the visible clip region of this System.Drawing.Graphics; otherwise, false.
        protected bool IsVisible(int x, int y, int width, int height)
        {
            return this.IsVisible(new Rectangle(x, y, width, height));
        }
        //
        // Summary:
        //     Indicates whether the rectangle specified by a System.Drawing.Rectangle structure
        //     is contained within the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.Rectangle structure to test for visibility.
        //
        // Returns:
        //     true if the rectangle specified by the rect parameter is contained within the
        //     visible clip region of this System.Drawing.Graphics; otherwise, false.
        public abstract bool IsVisible(Rectangle rect);

        //
        // Summary:
        //     Indicates whether the rectangle specified by a System.Drawing.RectangleF structure
        //     is contained within the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.RectangleF structure to test for visibility.
        //
        // Returns:
        //     true if the rectangle specified by the rect parameter is contained within the
        //     visible clip region of this System.Drawing.Graphics; otherwise, false.
        public abstract bool IsVisible(RectangleF rect);

        //
        // Summary:
        //     Indicates whether the rectangle specified by a pair of coordinates, a width,
        //     and a height is contained within the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle to test for visibility.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle to test for visibility.
        //
        //   width:
        //     Width of the rectangle to test for visibility.
        //
        //   height:
        //     Height of the rectangle to test for visibility.
        //
        // Returns:
        //     true if the rectangle defined by the x, y, width, and height parameters is contained
        //     within the visible clip region of this System.Drawing.Graphics; otherwise, false.
        protected bool IsVisible(float x, float y, float width, float height)
        {
            return this.IsVisible(new RectangleF(x, y, width, height));
        }
        //
        // Summary:
        //     Indicates whether the specified System.Drawing.Point structure is contained within
        //     the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   point:
        //     System.Drawing.Point structure to test for visibility.
        //
        // Returns:
        //     true if the point specified by the point parameter is contained within the visible
        //     clip region of this System.Drawing.Graphics; otherwise, false.
        public abstract bool IsVisible(Point point);

        //
        // Summary:
        //     Indicates whether the point specified by a pair of coordinates is contained within
        //     the visible clip region of this System.Drawing.Graphics.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the point to test for visibility.
        //
        //   y:
        //     The y-coordinate of the point to test for visibility.
        //
        // Returns:
        //     true if the point defined by the x and y parameters is contained within the visible
        //     clip region of this System.Drawing.Graphics; otherwise, false.
        protected bool IsVisible(float x, float y)
        {
            return this.IsVisible(new PointF(x, y));
        }
        //
        // Summary:
        //     Gets an array of System.Drawing.Region objects, each of which bounds a range
        //     of character positions within the specified string.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   layoutRect:
        //     System.Drawing.RectangleF structure that specifies the layout rectangle for the
        //     string.
        //
        //   stringFormat:
        //     System.Drawing.StringFormat that represents formatting information, such as line
        //     spacing, for the string.
        //
        // Returns:
        //     This method returns an array of System.Drawing.Region objects, each of which
        //     bounds a range of character positions within the specified string.
        public abstract Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect,
            StringFormat stringFormat);

        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font
        //     and formatted with the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   layoutArea:
        //     System.Drawing.SizeF structure that specifies the maximum layout area for the
        //     text.
        //
        //   stringFormat:
        //     System.Drawing.StringFormat that represents formatting information, such as line
        //     spacing, for the string.
        //
        //   charactersFitted:
        //     Number of characters in the string.
        //
        //   linesFilled:
        //     Number of text lines in the string.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size
        //     of the string, in the units specified by the System.Drawing.Graphics.PageUnit
        //     property, of the text parameter as drawn with the font parameter and the stringFormat
        //     parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        //public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat,
            //out int charactersFitted, out int linesFilled);

        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font that defines the format of the string.
        //
        //   width:
        //     Maximum width of the string in pixels.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified in the text parameter as drawn with the font parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        protected SizeF MeasureString(string text, Font font, int width)
        {
            return this.MeasureString(text, font, new SizeF((float)width, 999999f));
        }

        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font
        //     and formatted with the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        //   width:
        //     Maximum width of the string.
        //
        //   format:
        //     System.Drawing.StringFormat that represents formatting information, such as line
        //     spacing, for the string.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified in the text parameter as drawn with the font parameter and the
        //     stringFormat parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        protected SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            return this.MeasureString(text, font, new SizeF((float)width, 999999f), format);
        }
        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font
        //     and formatted with the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font defines the text format of the string.
        //
        //   origin:
        //     System.Drawing.PointF structure that represents the upper-left corner of the
        //     string.
        //
        //   stringFormat:
        //     System.Drawing.StringFormat that represents formatting information, such as line
        //     spacing, for the string.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified by the text parameter as drawn with the font parameter and the
        //     stringFormat parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        public abstract SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat);

        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font
        //     and formatted with the specified System.Drawing.StringFormat.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font defines the text format of the string.
        //
        //   layoutArea:
        //     System.Drawing.SizeF structure that specifies the maximum layout area for the
        //     text.
        //
        //   stringFormat:
        //     System.Drawing.StringFormat that represents formatting information, such as line
        //     spacing, for the string.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified in the text parameter as drawn with the font parameter and the
        //     stringFormat parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);

        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font that defines the text format of the string.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified by the text parameter as drawn with the font parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        protected SizeF MeasureString(string text, Font font)
        {
            return this.MeasureString(text, font, new SizeF(0f, 0f));
        }
        //
        // Summary:
        //     Measures the specified string when drawn with the specified System.Drawing.Font
        //     within the specified layout area.
        //
        // Parameters:
        //   text:
        //     String to measure.
        //
        //   font:
        //     System.Drawing.Font defines the text format of the string.
        //
        //   layoutArea:
        //     System.Drawing.SizeF structure that specifies the maximum layout area for the
        //     text.
        //
        // Returns:
        //     This method returns a System.Drawing.SizeF structure that represents the size,
        //     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
        //     string specified by the text parameter as drawn with the font parameter.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     font is null.
        protected SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            return this.MeasureString(text, font, layoutArea, null);
        }
        //
        // Summary:
        //     Multiplies the world transformation of this System.Drawing.Graphics and specified
        //     the System.Drawing.Drawing2D.Matrix.
        //
        // Parameters:
        //   matrix:
        //     4x4 System.Drawing.Drawing2D.Matrix that multiplies the world transformation.
        protected void MultiplyTransform(Matrix matrix)
        {
            this.MultiplyTransform(matrix, MatrixOrder.Prepend);
        }
        //
        // Summary:
        //     Multiplies the world transformation of this System.Drawing.Graphics and specified
        //     the System.Drawing.Drawing2D.Matrix in the specified order.
        //
        // Parameters:
        //   matrix:
        //     4x4 System.Drawing.Drawing2D.Matrix that multiplies the world transformation.
        //
        //   order:
        //     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that determines
        //     the order of the multiplication.
        public abstract void MultiplyTransform(Matrix matrix, MatrixOrder order);

        //
        // Summary:
        //     Releases a device context handle obtained by a previous call to the System.Drawing.Graphics.GetHdc
        //     method of this System.Drawing.Graphics.
        //public abstract void ReleaseHdc();

        //
        // Summary:
        //     Releases a device context handle obtained by a previous call to the System.Drawing.Graphics.GetHdc
        //     method of this System.Drawing.Graphics.
        //
        // Parameters:
        //   hdc:
        //     Handle to a device context obtained by a previous call to the System.Drawing.Graphics.GetHdc
        //     method of this System.Drawing.Graphics.
        //[EditorBrowsable(EditorBrowsableState.Advanced)]
        //public abstract void ReleaseHdc(IntPtr hdc);

        //
        // Summary:
        //     Releases a handle to a device context.
        //
        // Parameters:
        //   hdc:
        //     Handle to a device context.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public abstract void ReleaseHdcInternal(IntPtr hdc);

        //
        // Summary:
        //     Resets the clip region of this System.Drawing.Graphics to an infinite region.
        public abstract void ResetClip();

        //
        // Summary:
        //     Resets the world transformation matrix of this System.Drawing.Graphics to the
        //     identity matrix.
        public abstract void ResetTransform();

        //
        // Summary:
        //     Restores the state of this System.Drawing.Graphics to the state represented by
        //     a System.Drawing.Drawing2D.GraphicsState.
        //
        // Parameters:
        //   gstate:
        //     System.Drawing.Drawing2D.GraphicsState that represents the state to which to
        //     restore this System.Drawing.Graphics.
        //public abstract void Restore(GraphicsState gstate);

        //
        // Summary:
        //     Applies the specified rotation to the transformation matrix of this System.Drawing.Graphics.
        //
        // Parameters:
        //   angle:
        //     Angle of rotation in degrees.
        protected void RotateTransform(float angle)
        {
            this.RotateTransform(angle, MatrixOrder.Prepend);
        }

        //
        // Summary:
        //     Applies the specified rotation to the transformation matrix of this System.Drawing.Graphics
        //     in the specified order.
        //
        // Parameters:
        //   angle:
        //     Angle of rotation in degrees.
        //
        //   order:
        //     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
        //     whether the rotation is appended or prepended to the matrix transformation.
        public abstract void RotateTransform(float angle, MatrixOrder order);

        //
        // Summary:
        //     Saves the current state of this System.Drawing.Graphics and identifies the saved
        //     state with a System.Drawing.Drawing2D.GraphicsState.
        //
        // Returns:
        //     This method returns a System.Drawing.Drawing2D.GraphicsState that represents
        //     the saved state of this System.Drawing.Graphics.
        public abstract GraphicsState Save();

        //
        // Summary:
        //     Applies the specified scaling operation to the transformation matrix of this
        //     System.Drawing.Graphics by prepending it to the object's transformation matrix.
        //
        // Parameters:
        //   sx:
        //     Scale factor in the x direction.
        //
        //   sy:
        //     Scale factor in the y direction.
        protected void ScaleTransform(float sx, float sy)
        {
            this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
        }
        //
        // Summary:
        //     Applies the specified scaling operation to the transformation matrix of this
        //     System.Drawing.Graphics in the specified order.
        //
        // Parameters:
        //   sx:
        //     Scale factor in the x direction.
        //
        //   sy:
        //     Scale factor in the y direction.
        //
        //   order:
        //     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
        //     whether the scaling operation is prepended or appended to the transformation
        //     matrix.
        public abstract void ScaleTransform(float sx, float sy, MatrixOrder order);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the result of the
        //     specified operation combining the current clip region and the rectangle specified
        //     by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.Rectangle structure to combine.
        //
        //   combineMode:
        //     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
        //     the combining operation to use.
        public abstract void SetClip(Rectangle rect, CombineMode combineMode);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the rectangle specified
        //     by a System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.RectangleF structure that represents the new clip region.
        protected void SetClip(RectangleF rect)
        {
            this.SetClip(rect, CombineMode.Replace);
        }

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the result of the
        //     specified operation combining the current clip region and the rectangle specified
        //     by a System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.RectangleF structure to combine.
        //
        //   combineMode:
        //     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
        //     the combining operation to use.
        public abstract void SetClip(RectangleF rect, CombineMode combineMode);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the specified System.Drawing.Drawing2D.GraphicsPath.
        //
        // Parameters:
        //   path:
        //     System.Drawing.Drawing2D.GraphicsPath that represents the new clip region.
        protected void SetClip(GraphicsPath path)
        {
            this.SetClip(path, CombineMode.Replace);
        }
        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the result of the
        //     specified operation combining the current clip region and the specified System.Drawing.Drawing2D.GraphicsPath.
        //
        // Parameters:
        //   path:
        //     System.Drawing.Drawing2D.GraphicsPath to combine.
        //
        //   combineMode:
        //     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
        //     the combining operation to use.
        public abstract void SetClip(GraphicsPath path, CombineMode combineMode);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the result of the
        //     specified operation combining the current clip region and the specified System.Drawing.Region.
        //
        // Parameters:
        //   region:
        //     System.Drawing.Region to combine.
        //
        //   combineMode:
        //     Member from the System.Drawing.Drawing2D.CombineMode enumeration that specifies
        //     the combining operation to use.
        public abstract void SetClip(Region region, CombineMode combineMode);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the Clip property
        //     of the specified System.Drawing.Graphics.
        //
        // Parameters:
        //   g:
        //     System.Drawing.Graphics from which to take the new clip region.
        protected void SetClip(Graphics g)
        {
            this.SetClip(g, CombineMode.Replace);
        }
        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the result of the
        //     specified combining operation of the current clip region and the System.Drawing.Graphics.Clip
        //     property of the specified System.Drawing.Graphics.
        //
        // Parameters:
        //   g:
        //     System.Drawing.Graphics that specifies the clip region to combine.
        //
        //   combineMode:
        //     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
        //     the combining operation to use.
        public abstract void SetClip(Graphics g, CombineMode combineMode);

        //
        // Summary:
        //     Sets the clipping region of this System.Drawing.Graphics to the rectangle specified
        //     by a System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   rect:
        //     System.Drawing.Rectangle structure that represents the new clip region.
        protected void SetClip(Rectangle rect)
        {
            this.SetClip(rect, CombineMode.Replace);
        }
        //
        // Summary:
        //     Transforms an array of points from one coordinate space to another using the
        //     current world and page transformations of this System.Drawing.Graphics.
        //
        // Parameters:
        //   destSpace:
        //     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
        //     the destination coordinate space.
        //
        //   srcSpace:
        //     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
        //     the source coordinate space.
        //
        //   pts:
        //     Array of System.Drawing.Point structures that represents the points to transformation.
        //public abstract void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts);

        //
        // Summary:
        //     Transforms an array of points from one coordinate space to another using the
        //     current world and page transformations of this System.Drawing.Graphics.
        //
        // Parameters:
        //   destSpace:
        //     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
        //     the destination coordinate space.
        //
        //   srcSpace:
        //     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
        //     the source coordinate space.
        //
        //   pts:
        //     Array of System.Drawing.PointF structures that represent the points to transform.
        //public abstract void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts);

        //
        // Summary:
        //     Translates the clipping region of this System.Drawing.Graphics by specified amounts
        //     in the horizontal and vertical directions.
        //
        // Parameters:
        //   dx:
        //     The x-coordinate of the translation.
        //
        //   dy:
        //     The y-coordinate of the translation.
        //public abstract void TranslateClip(float dx, float dy);

        //
        // Summary:
        //     Translates the clipping region of this System.Drawing.Graphics by specified amounts
        //     in the horizontal and vertical directions.
        //
        // Parameters:
        //   dx:
        //     The x-coordinate of the translation.
        //
        //   dy:
        //     The y-coordinate of the translation.
        //public abstract void TranslateClip(int dx, int dy);

        //
        // Summary:
        //     Changes the origin of the coordinate system by applying the specified translation
        //     to the transformation matrix of this System.Drawing.Graphics in the specified
        //     order.
        //
        // Parameters:
        //   dx:
        //     The x-coordinate of the translation.
        //
        //   dy:
        //     The y-coordinate of the translation.
        //
        //   order:
        //     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
        //     whether the translation is prepended or appended to the transformation matrix.
        public abstract void TranslateTransform(float dx, float dy, MatrixOrder order);

        //
        // Summary:
        //     Changes the origin of the coordinate system by prepending the specified translation
        //     to the transformation matrix of this System.Drawing.Graphics.
        //
        // Parameters:
        //   dx:
        //     The x-coordinate of the translation.
        //
        //   dy:
        //     The y-coordinate of the translation.
        public void TranslateTransform(float dx, float dy)
        {
            this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }
        //
        // Summary:
        //     Provides a callback method for deciding when the Overload:System.Drawing.Graphics.DrawImage
        //     method should prematurely cancel execution and stop drawing an image.
        //
        // Parameters:
        //   callbackdata:
        //     Internal pointer that specifies data for the callback method. This parameter
        //     is not passed by all Overload:System.Drawing.Graphics.DrawImage overloads. You
        //     can test for its absence by checking for the value System.IntPtr.Zero.
        //
        // Returns:
        //     This method returns true if it decides that the Overload:System.Drawing.Graphics.DrawImage
        //     method should prematurely stop execution. Otherwise it returns false to indicate
        //     that the Overload:System.Drawing.Graphics.DrawImage method should continue execution.
        public delegate bool DrawImageAbort(IntPtr callbackdata);

        //
        // Summary:
        //     Provides a callback method for the Overload:System.Drawing.Graphics.EnumerateMetafile
        //     method.
        //
        // Parameters:
        //   recordType:
        //     Member of the System.Drawing.Imaging.EmfPlusRecordType enumeration that specifies
        //     the type of metafile record.
        //
        //   flags:
        //     Set of flags that specify attributes of the record.
        //
        //   dataSize:
        //     Number of bytes in the record data.
        //
        //   data:
        //     Pointer to a buffer that contains the record data.
        //
        //   callbackData:
        //     Not used.
        //
        // Returns:
        //     Return true if you want to continue enumerating records; otherwise, false.
        public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data,
            PlayRecordCallback callbackData);
    }
}