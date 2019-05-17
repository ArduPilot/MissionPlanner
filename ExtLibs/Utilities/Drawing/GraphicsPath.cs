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
        private List<PointF> _pathPoints= new List<PointF>();

        public void AddLines(Point[] toArray)
        {
            _pathPoints.AddRange(toArray.Select(a => (PointF) a));
        }

        public void AddPolygon(Point[] toArray)
        {
            _pathPoints.AddRange(toArray.Select(a => (PointF)a));
        }

        public void Dispose()
        {
            
        }

        public int PointCount {
            get { return PathPoints.Length; } }

        public PointF[] PathPoints
        {
            get => _pathPoints.ToArray();
        }

        public int[] PathTypes {
            get { return Enumerable.Repeat(1, PointCount).ToArray(); }
            
         }

        public bool IsVisible(int i, int i1)
        {
            return true;
        }

        public void Reset()
        {
            _pathPoints.Clear();
        }

        public void AddLine(float p2X, float p2Y, float p2, float p3)
        {
            _pathPoints.Add(new PointF(p2X, p2Y));
            _pathPoints.Add(new PointF(p2, p3));
        }

        public void AddArc(float p0, float p1, float p2, float p3, int p4, int p5)
        {
            throw new NotImplementedException();
        }

        public void CloseFigure()
        {
            _pathPoints.Add(_pathPoints[0]);
        }

        public PointF GetLastPoint()
        {
            return _pathPoints.Last();
        }

        public bool IsOutlineVisible(int i, int i1, Pen stroke)
        {
            throw new NotImplementedException();
        }

        public void AddString(string s, object fontFontFamily, int i, float f, Point point, object genericTypographic)
        {
            var path = new SKPaint().GetTextPath(s, point.X, point.Y);

            path.Points.ForEach(a => _pathPoints.Add(new PointF(a.X, a.Y)));
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
            _pathPoints.Reverse();
            //throw new NotImplementedException();
        }
    }
}
