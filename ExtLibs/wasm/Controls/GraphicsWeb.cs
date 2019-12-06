﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Blazor.Extensions.Canvas.Canvas2D;
using MissionPlanner.Drawing;
using MissionPlanner.Drawing.Drawing2D;
using MissionPlanner.Utilities;
using Brush = MissionPlanner.Drawing.Brush;
using CombineMode = MissionPlanner.Drawing.CombineMode;
using CompositingMode = MissionPlanner.Drawing.CompositingMode;
using CompositingQuality = MissionPlanner.Drawing.CompositingQuality;
using CoordinateSpace = MissionPlanner.Drawing.CoordinateSpace;
using DashStyle = MissionPlanner.Drawing.DashStyle;
using FillMode = MissionPlanner.Drawing.FillMode;
using FlushIntention = MissionPlanner.Drawing.FlushIntention;
using Font = MissionPlanner.Drawing.Font;
using Graphics = MissionPlanner.Drawing.Graphics;
using GraphicsPath = MissionPlanner.Drawing.Drawing2D.GraphicsPath;
using GraphicsState = MissionPlanner.Drawing.GraphicsState;
using GraphicsUnit = MissionPlanner.Drawing.GraphicsUnit;
using Icon = MissionPlanner.Drawing.Icon;
using IGraphics = MissionPlanner.Drawing.IGraphics;
using Image = MissionPlanner.Drawing.Image;
using InterpolationMode = MissionPlanner.Drawing.InterpolationMode;
using Matrix = MissionPlanner.Drawing.Drawing2D.Matrix;
using MatrixOrder = MissionPlanner.Drawing.MatrixOrder;
using Pen = MissionPlanner.Drawing.Pen;
using PixelOffsetMode = MissionPlanner.Drawing.PixelOffsetMode;
using Region = MissionPlanner.Drawing.Region;
using SmoothingMode = MissionPlanner.Drawing.SmoothingMode;
using StringFormat = MissionPlanner.Drawing.StringFormat;
using Color = System.Drawing.Color;
using Pens = MissionPlanner.Drawing.Pens;
using Rectangle = System.Drawing.Rectangle;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using Point = System.Drawing.Point;

namespace MissionPlanner.Controls
{
    public class GraphicsWeb : IGraphics
    {
        private readonly Canvas2DContext _context;

        public GraphicsWeb(Canvas2DContext context)
        {
            _context = context;
        }

        public Region Clip
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public RectangleF ClipBounds => throw new NotImplementedException();

        public CompositingMode CompositingMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public CompositingQuality CompositingQuality
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public float DpiX => throw new NotImplementedException();

        public float DpiY => throw new NotImplementedException();

        public InterpolationMode InterpolationMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool IsClipEmpty => throw new NotImplementedException();

        public bool IsVisibleClipEmpty => throw new NotImplementedException();

        public float PageScale
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public GraphicsUnit PageUnit
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public PixelOffsetMode PixelOffsetMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Point RenderingOrigin
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public SmoothingMode SmoothingMode
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int TextContrast
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public TextRenderingHint TextRenderingHint
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Matrix Transform
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public RectangleF VisibleClipBounds => throw new NotImplementedException();

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

        public async void Clear(Color color)
        {
            await _context.ClearRectAsync(0, 0, 9999, 9999);
        }

        public async void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            await _context.SetLineWidthAsync(pen.Width);
            await _context.SetStrokeStyleAsync(pen.Color.ToString());
            await _context.ArcAsync(x, y, Math.Min(width, height) / 2, startAngle, sweepAngle - startAngle);
        }


        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }


        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawArc(pen, x, y, width, height, startAngle, (float)sweepAngle);
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            DrawBeziers(pen,
                new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4) });
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
             DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
             DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }


        public async void DrawBeziers(Pen pen, PointF[] points)
        {
            await _context.SetLineWidthAsync(pen.Width);
            await _context.SetStrokeStyleAsync(pen.Color.ToString());
            await _context.MoveToAsync(points[0].X, points[0].Y);
            await _context.BezierCurveToAsync(
                points[1].X, points[1].Y,
                points[2].X, points[2].Y,
                points[3].X, points[3].Y);
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            var pts = Point2PointF(points);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
             DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, false, .5f);
             DrawBeziers(pen, pts);
        }

        public  void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, false, tension);
             DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            var pts = Spline2Bez(points, offset, numberOfSegments, false, .5f);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            var pts = Spline2Bez(points, offset, numberOfSegments, false, tension);
            DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, .5f);
             DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, false, tension);
             DrawBeziers(pen, pts);
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), offset, numberOfSegments, false, tension);
             DrawBeziers(pen, pts);
        }
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
             DrawArc(pen, x, y, width, height, 0, 360);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
             DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, Rectangle rect)
        {
             DrawEllipse(pen, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
             DrawEllipse(pen, x, y, width, (float)height);
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
            DrawImage(image, point.X, point.Y);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, float x, float y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImage(image, (long)x, (long)y, (long)width, (long)height);
            //throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point point)
        {
            DrawImage(image, point.X, (float)point.Y);
        }

        public void DrawImage(Image image, int x, int y)
        {
            DrawImage(image, x, (float)y);
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            DrawImage(image, (float)x, y, width, height);
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

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

     

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image img, Rectangle rectangle, float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes tileFlipXYAttributes)
        {
            //_context.DrawImageAsync(null, srcX, srcY, srcWidth, srcHeight);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight,
            GraphicsUnit srcUnit,
            ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Image img, long i, long i1, long width, long height)
        {
            DrawImage(img, new Rectangle((int)i, (int)i1, (int)width, (int)height), 0.0f, 0, width, height,
                GraphicsUnit.Pixel,
                new ImageAttributes());
        }

        public void DrawImage(Image image, Rectangle rectangle, int p1, int p2, long p3, long p4,
            GraphicsUnit graphicsUnit, ImageAttributes TileFlipXYAttributes)
        {
            DrawImage(image, rectangle, (float)rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, new ImageAttributes());
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImage(image, point.X, (float)point.Y);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            _context.SetLineWidthAsync(pen.Width);
            _context.SetStrokeStyleAsync(pen.Color.ToString());
            _context.BeginPath();
            _context.MoveTo(x1, y1);
            _context.LineTo(x2, y2);
            _context.Stroke();
            _context.ClosePath();
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLine(pen, x1, y1, x2, (float)y2);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, (float)pt2.Y);
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            var start = points.First();
            foreach (var pointF in points.Skip(1))
            {
                DrawLine(pen, start.X, start.Y, pointF.X, pointF.Y);
                start = pointF;
            }
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void DrawLines(Pen pen, Point[] points)
        {
            var pts = Point2PointF(points);
            DrawLines(pen, pts);
        }


        /// <summary>
        ///     Implemented
        /// </summary>
        /// <remarks>
        ///     Mainly based on the libgdi+ implementation: https://github.com/mono/libgdiplus/blob/master/src/graphics-cairo.c
        ///     and this SO question reply:
        ///     https://stackoverflow.com/questions/1790862/how-to-determine-endpoints-of-arcs-in-graphicspath-pathpoints-and-pathtypes-arra
        ///     from SiliconMind.
        /// </remarks>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            float x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0;
            var points = path.PathPoints;
            var types = path.PathTypes;
            int bidx = 0;

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var type = (PathPointType)types[i];

                switch (type & PathPointType.PathTypeMask)
                {
                    case PathPointType.Start:
                        _context.MoveToAsync(point.X, point.Y);
                        break;

                    case PathPointType.Line:
                        _context.LineToAsync(point.X, point.Y);
                        break;

                    case PathPointType.Bezier3:
                        // collect 3 points
                        switch (bidx++)
                        {
                            case 0:
                                x1 = point.X;
                                y1 = point.Y;
                                break;
                            case 1:
                                x2 = point.X;
                                y2 = point.Y;
                                break;
                            case 2:
                                x3 = point.X;
                                y3 = point.Y;
                                break;
                        }
                        if (bidx == 3)
                        {
                            //_context.AddCurveToPoint(x1, y1, x2, y2, x3, y3);
                            DrawBezier(pen, x1, y1, x2, y2, x3, y3, point.X, point.Y);
                            bidx = 0;
                        }
                        break;
                    default:
                        throw new Exception("Inconsistent internal state, path type=" + type);
                }
                if ((type & PathPointType.CloseSubpath) != 0)
                    _context.ClosePath();
            }
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
            DrawPie(pen, rect.X, rect.X, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawPie(pen, x, y, width, height, startAngle, (float)sweepAngle);
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
        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            var pts = Spline2Bez(points, 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, FillMode.Alternate);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, .5f);
            FillBeziers(brush, pts, fillmode);
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            var pts = Spline2Bez(Point2PointF(points), 0, points.Length - 1, true, tension);
            FillBeziers(brush, pts, fillmode);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Implemented
        /// </summary>
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, (float)rect.Height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            Console.WriteLine("FillPath");
            var subpaths = new GraphicsPathIterator(path);
            var subpath = new GraphicsPath(path.FillMode);
            subpaths.Rewind();
            for (var s = 0; s < subpaths.SubpathCount; s++)
            {
                bool isClosed;
                if (subpaths.NextSubpath(subpath, out isClosed) < 2) continue;
                if (!isClosed)
                {
                    //subpath.CloseAllFigures();
                }

                var PathPoints = subpath.PathPoints;

                var lastType = (PathPointType)subpath.PathTypes[PathPoints.Length - 1];
                if (subpath.PathTypes.Any(pt =>
                    ((PathPointType)pt & PathPointType.PathTypeMask) == PathPointType.Line))
                    FillPolygon(brush, PathPoints, path.FillMode);
                else
                    FillBeziers(brush, PathPoints, path.FillMode);
            }

            subpath.Dispose();
            subpaths.Dispose();
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle,
            float sweepAngle)
        {
            _context.BeginPathAsync();
            DrawPie(Pens.Transparent, x, y, width, height, startAngle, sweepAngle);
            _context.FillAsync();
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            FillPie(brush, (float)x, y, width, height, startAngle, sweepAngle);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            var path = new GraphicsPath();path.AddPolygon(points);
            FillPath(brush, path);
        }

        public async void FillPolygon(Brush brush, PointF[] points)
        {
            //_context.SetFillStyleAsync(((Drawing.SolidBrush) brush).Color.ToString());
             await _context.BeginPathAsync();
             await _context.MoveToAsync(points[0].X, points[0].Y);
            points.ForEach((a) => _context.LineToAsync(a.X, a.Y));
            await _context.ClosePathAsync();
            await _context.FillAsync();
            Console.WriteLine("FillPolygon done");
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            FillPolygon(brush, points);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            var path = new GraphicsPath(); 
            path.AddRectangle(RectangleF.FromLTRB(x, y, width, height));
            FillPath(brush, path);
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

        public async void ResetTransform()
        {
            await _context.SetTransformAsync(0, 0, 0, 0, 0, 0);
        }

        public void Restore(GraphicsState gstate)
        {
            throw new NotImplementedException();
        }

        public void RotateTransform(float angle)
        {
            _context.RotateAsync(angle);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _context.RotateAsync(angle);

            if (order == MatrixOrder.Append)
            {
                //checkthis
                //var old = _image.TotalMatrix;
                //var extra = SKMatrix.MakeRotation(angle);
                //SKMatrix.PreConcat(ref old, extra);
                //_image.SetMatrix(old);
            }
        }

        public GraphicsState Save()
        {
            throw new NotImplementedException();
        }

        public void ScaleTransform(float sx, float sy)
        {
            _context.ScaleAsync(sx, sy);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
                _context.Scale(sx, sy);

            if (order == MatrixOrder.Append)
            {
                //var old = _image.TotalMatrix;
                //old.SetScaleTranslate(sx, sy, old.TransX, old.TransY);
                //_image.SetMatrix(old);
            }
        }

        public void SetClip(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            _context.ClipAsync();
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
            _context.TranslateAsync(dx, dy);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            if (MatrixOrder.Append == order)
            {
                //_context.GetTransform();
                //_context.TranslateAsync()
                /*
            float[] model = new float[16];
            _context.GetFloat(GetPName.ModelviewMatrix, model);

            var current = new Matrix4(model[0], model[1], model[2], model[3],
                model[4], model[5], model[6], model[7],
                model[8], model[9], model[10], model[11],
                model[12], model[13], model[14], model[15]);

            var matrix = current * Matrix4.CreateTranslation(dx, dy, 0);
            _context.LoadIdentity();
            _context.LoadMatrix(ref matrix);
            //GL.GetFloat(GetPName.ModelviewMatrix, model);
            */
            }
            else
            {
                TranslateTransform(dx, dy);
            }
        }

        private static PointF ControlPoint(PointF l, PointF pt, float t)
        {
            var v = new PointF(l.X - pt.X, l.Y - pt.Y);

            var vlen = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            v.X /= (float)Math.Sqrt(vlen / (10 * t * t));
            v.Y /= (float)Math.Sqrt(vlen / (10 * t * t));

            return new PointF(pt.X + v.X, pt.Y + v.Y);
        }

        private static PointF[] ControlPoints(PointF l, PointF r, PointF pt, float t)
        {
            //points to vectors
            var lv = new PointF(l.X - pt.X, l.Y - pt.Y);
            var rv = new PointF(r.X - pt.X, r.Y - pt.Y);

            var nlv = new PointF(lv.X - rv.X, lv.Y - rv.Y);
            var nrv = new PointF(rv.X - lv.X, rv.Y - lv.Y);

            var nlvlen = (float)Math.Sqrt(nlv.X * nlv.X + nlv.Y * nlv.Y);
            nlv.X /= (float)Math.Sqrt(nlvlen / (10 * t * t));
            nlv.Y /= (float)Math.Sqrt(nlvlen / (10 * t * t));

            var nrvlen = (float)Math.Sqrt(nrv.X * nrv.X + nrv.Y * nrv.Y);
            nrv.X /= (float)Math.Sqrt(nrvlen / (10 * t * t));
            nrv.Y /= (float)Math.Sqrt(nrvlen / (10 * t * t));

            var ret = new PointF[2];

            ret[0] = new PointF(pt.X + nlv.X, pt.Y + nlv.Y);
            ret[1] = new PointF(pt.X + nrv.X, pt.Y + nrv.Y);

            return ret;
        }

        private static PointF[] Point2PointF(Point[] p)
        {
            var pf = new PointF[p.Length];
            for (var i = 0; i < p.Length; ++i) pf[i] = new PointF(p[i].X, p[i].Y);

            return pf;
        }

        //This seems to be a very good approximation.  GDI must be using a similar simplistic method for some odd reason.
        //If a curve is closed, it uses all points, and ignores start and num.
        private static PointF[] Spline2Bez(PointF[] points, int start, int num, bool closed, float tension)
        {
            var cp = new ArrayList();
            var res = new ArrayList();

            var l = points.Length - 1;

            res.Add(points[0]);
            res.Add(ControlPoint(points[1], points[0], tension));

            for (var i = 1; i < l; ++i)
            {
                var pts = ControlPoints(points[i - 1], points[i + 1], points[i], tension);
                res.Add(pts[0]);
                res.Add(points[i]);
                res.Add(pts[1]);
            }

            res.Add(ControlPoint(points[l - 1], points[l], tension));
            res.Add(points[l]);

            if (closed)
            {
                //adjust rh cp of point 0
                var pts = ControlPoints(points[l], points[1], points[0], tension);
                res[1] = pts[1];

                //adjust lh cp of point l and add rh cp
                pts = ControlPoints(points[l - 1], points[0], points[l], tension);
                res[res.Count - 2] = pts[0];
                res.Add(pts[1]);

                //add new end point and its lh cp
                pts = ControlPoints(points[l], points[1], points[0], tension);
                res.Add(pts[0]);
                res.Add(points[0]);

                return (PointF[])res.ToArray(typeof(PointF));
            }

            var subset = new ArrayList();

            for (var i = start * 3; i < (start + num) * 3; ++i) subset.Add(res[i]);

            subset.Add(res[(start + num) * 3]);

            return (PointF[])subset.ToArray(typeof(PointF));
        }

        private void FillBeziers(Brush brush, PointF[] points, FillMode fillmode)
        {
            throw new NotImplementedException();
        }
    }
}