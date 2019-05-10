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
            throw new System.NotImplementedException();
        }

        public RectangleF GetBounds(Graphics graphics)
        {
            throw new System.NotImplementedException();
        }

        public void Exclude(Rectangle peClipRectangle)
        {
            throw new System.NotImplementedException();
        }

        public void MakeEmpty()
        {
            throw new System.NotImplementedException();
        }

        public void Union(Rectangle clientRectangle)
        {
            throw new System.NotImplementedException();
        }

        public void Intersect(Region clipRegion)
        {
            throw new System.NotImplementedException();
        }

        public bool IsVisible(Rectangle clipRectangle)
        {
            throw new System.NotImplementedException();
        }

        public bool IsInfinite(Graphics graphics)
        {
            throw new NotImplementedException();
        }

        public void Intersect(Rectangle columnsArea)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(Graphics dc)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(Point point)
        {
            throw new NotImplementedException();
        }

        public void Translate(int paddingLeft, int paddingTop)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RectangleF> GetRegionScans(Matrix dcTransform)
        {
            throw new NotImplementedException();
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