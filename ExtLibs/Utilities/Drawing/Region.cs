using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Region: SKRegion
    {
        public Region()
        {
        }

        public Region(Rectangle parentClientRectangle)
        {
            base.SetRect(parentClientRectangle.ToSKRectI());
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
            base.Op((int)paddingLeft, (int)paddingTop, base.Bounds.Right + (int)paddingLeft, base.Bounds.Bottom + (int)paddingTop,
                SKRegionOperation.Replace);
        }

        public IEnumerable<RectangleF> GetRegionScans(Matrix dcTransform)
        {
            return new RectangleF[0];
        }

        public void Union(Region clientRectangle)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetHrgn(Graphics graphics)
        {
            throw new NotImplementedException();
        }
    }
}