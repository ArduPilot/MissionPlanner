using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MissionPlanner.Utilities.Drawing
{
    public class GraphicsPath: IDisposable
    {
        public void AddLines(Point[] toArray)
        {
            throw new NotImplementedException();
        }

        public void AddPolygon(Point[] toArray)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public int PointCount { get; set; }
        public PointF[] PathPoints { get; set; }

        public bool IsVisible(int i, int i1)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void AddLine(float p2X, float p2Y, float p2, float p3)
        {
            throw new NotImplementedException();
        }

        public void AddArc(float p0, float p1, float p2, float p3, int p4, int p5)
        {
            throw new NotImplementedException();
        }

        public void CloseFigure()
        {
            throw new NotImplementedException();
        }

        public PointF GetLastPoint()
        {
            throw new NotImplementedException();
        }

        public bool IsOutlineVisible(int i, int i1, Pen stroke)
        {
            throw new NotImplementedException();
        }
    }
}
