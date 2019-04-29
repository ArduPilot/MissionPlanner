using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SkiaSharp;

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

        public int PointCount {
            get { return PathPoints.Length; } }
        public PointF[] PathPoints { get; set; } = new PointF[0];

        public bool IsVisible(int i, int i1)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            PathPoints = new PointF[0];
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

        public void AddString(string s, object fontFontFamily, int i, float f, Point point, object genericTypographic)
        {
            var path = new SKPaint().GetTextPath(s, point.X, point.Y);

            PathPoints = path.Points.Select(a => new PointF(a.X, a.Y)).ToArray();
        }

        public void AddEllipse(Rectangle p0)
        {
           // throw new NotImplementedException();
        }

        public void AddPie(Rectangle p0, float rangeStartAngle, float rangeSweepAngle)
        {
          //  throw new NotImplementedException();
        }

        public void Reverse()
        {
            //throw new NotImplementedException();
        }
    }
}
