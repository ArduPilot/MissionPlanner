using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClipperLib;
using SkiaSharp;

namespace System.Drawing
{
    // Clipper lib definitions
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public class Region : SKRegion
    {
        internal static Path PointFArrayToIntArray(PointF[] points, float scale)
        {
            Path result = new Path();
            for (int i = 0; i < points.Length; ++i)
            {
                result.Add(new IntPoint((int) points[i].X * scale, (int) points[i].Y * scale));
            }

            return result;
        }


        internal static PointF[] PathToPointFArray(Path pg, float scale)
        {
            PointF[] result = new PointF[pg.Count];
            for (int i = 0; i < pg.Count; ++i)
            {
                result[i].X = (float) pg[i].X / scale;
                result[i].Y = (float) pg[i].Y / scale;
            }

            return result;
        }

        public Region()
        {
        }

        public Region(Rectangle parentClientRectangle)
        {
            base.SetRect(parentClientRectangle.ToSKRectI());
        }

        public Region(GraphicsPath parentClientRectangle)
        {
            base.SetPath(parentClientRectangle);
        }

        public RectangleF GetBounds(Graphics graphics)
        {
            return new RectangleF(base.Bounds.Left, base.Bounds.Top, base.Bounds.Width, base.Bounds.Height);
        }

        public void Exclude(Rectangle peClipRectangle)
        {
            base.Op(peClipRectangle.ToSKRectI(), SKRegionOperation.Difference);
        }

        public void MakeEmpty()
        {
            base.SetRect(SKRectI.Empty);
        }

        public void Union(Rectangle clientRectangle)
        {
            base.Op(clientRectangle.ToSKRectI(), SKRegionOperation.Union);
        }

        public void Intersect(Region clipRegion)
        {
            throw new System.NotImplementedException();
        }

        public bool IsVisible(Rectangle clipRectangle)
        {
            return (base.Contains(clipRectangle.Location.X, clipRectangle.Location.Y) ||
                    base.Contains(clipRectangle.Location.X + clipRectangle.Width / 2,
                        clipRectangle.Location.Y + clipRectangle.Height / 2) ||
                    base.Contains(clipRectangle.Location.X + clipRectangle.Width,
                        clipRectangle.Location.Y + clipRectangle.Height) ||
                    base.Contains(clipRectangle.Location.X + clipRectangle.Width / 2, clipRectangle.Location.Y) ||
                    base.Contains(clipRectangle.Location.X, clipRectangle.Location.Y + clipRectangle.Height / 2));
        }

        public bool IsInfinite(Graphics graphics)
        {
            throw new NotImplementedException();
        }

        public void Intersect(Rectangle columnsArea)
        {
            base.Op(columnsArea.ToSKRectI(), SKRegionOperation.Intersect);
        }

        public bool IsEmpty(Graphics dc)
        {
            return base.Bounds.IsEmpty;
        }

        public bool IsVisible(Point point)
        {
            return base.Contains(point.X, point.Y);
        }

        public void Translate(float paddingLeft, float paddingTop)
        {
            base.Op((int) paddingLeft, (int) paddingTop, base.Bounds.Right + (int) paddingLeft,
                base.Bounds.Bottom + (int) paddingTop,
                SKRegionOperation.Replace);
        }

        public IEnumerable<RectangleF> GetRegionScans(Matrix dcTransform)
        {
            return new RectangleF[]
                {RectangleF.FromLTRB(base.Bounds.Left, base.Bounds.Top, base.Bounds.Right, base.Bounds.Bottom)};
        }

        public void Union(Region clientRectangle)
        {
            base.Op(clientRectangle, SKRegionOperation.Union);
        }

        public IntPtr GetHrgn(Graphics graphics)
        {
            throw new NotImplementedException();
        }

        public static Region FromHrgn(IntPtr retval)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(PointF clipRectangle)
        {
            return base.Contains((int)clipRectangle.X, (int)clipRectangle.Y);
        }

        public Region Clone()
        {
            var rect = new Region();
            rect.SetRect(this.Bounds);
            return rect;
        }

        public void Exclude(GraphicsPath peClipRectangle)
        {
            base.Op(peClipRectangle, SKRegionOperation.Difference);
        }
    }
}