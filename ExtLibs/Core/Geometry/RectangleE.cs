using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Core.Geometry
{
    public class RectangleE : AShapeE, ICloneable, IComparable<RectangleE>
    {//RectangleC
        #region Private Fields
        private double m_X;
        private double m_Y;
        private double m_Width;
        private double m_Height;
        #endregion

        #region Constructors
        public RectangleE(double x, double y, double width, double height) {
            m_X = x;
            m_Width = width;
            m_Y = y;
            m_Height = height;
        }
        public RectangleE(PointE location, SizeE size) {
            m_X = location.X;
            m_Width = size.Width;
            m_Y = location.Y;
            m_Height = size.Height;
        }
        public RectangleE(IntervalE xInt, IntervalE yInt) {
            m_X = xInt.Minimum;
            m_Width = xInt.Dimension;
            m_Y = yInt.Minimum;
            m_Height = yInt.Dimension;
        }
        public RectangleE(RectangleE r) {
            m_X = r.X;
            m_Width = r.Width;
            m_Y = r.Y;
            m_Height = r.Height;
        }

        public RectangleE() {
        }
        #endregion



        #region GetSet
        [XmlAttribute()]
        public double X {
            get {
                return m_X;
            }
            set {
                m_X = value;
                CallGeometryChanged();
            }
        }

        [XmlAttribute()]
        public double Y {
            get {
                return m_Y;
            }
            set {
                m_Y = value;
                CallGeometryChanged();
            }
        }

        [XmlAttribute()]
        public double Width {
            get {
                return m_Width;
            }
            set {
                m_Width = value;
                CallGeometryChanged();
            }
        }


        [XmlAttribute()]
        public double Height {
            get {
                return m_Height;
            }
            set {
                m_Height = value;
                CallGeometryChanged();
            }
        }

        [XmlIgnore()]
        public double Top {
            get {
                return Y + Height;
            }
            set {
                Y = value - Height;
            }
        }

        [XmlIgnore()]
        public double Bottom {
            get {
                return Y;
            }
            set {
                Y = value;
            }
        }

        [XmlIgnore()]
        public double Left {
            get {
                return X;
            }
            set {
                X = value;
            }
        }

        [XmlIgnore()]
        public double Right {
            get {
                return X + Width;
            }
            set {
                X = value - Width;
            }
        }

        [XmlIgnore()]
        public PointE Location {
            get {
                return new PointE(X, Y);
            }
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        [XmlIgnore()]
        public SizeE Size {
            get {
                return new SizeE(Width, Height);
            }
            set {
                Width = value.Width;
                Height = value.Height;
            }
        }

        [XmlIgnore()]
        public PointE Center {
            get {
                return new PointE(X + Width / 2, Y + Height / 2);
            }
            set {
                X = value.X - Width / 2;
                Y = value.Y - Height / 2;
            }
        }

        [XmlIgnore()]
        public PointE TopLeft {
            get {
                return new PointE(X, Y + Height);
            }
            set {
                X = value.X;
                Y = value.Y - Height;
            }
        }

        [XmlIgnore()]
        public PointE TopRight {
            get {
                return new PointE(X + Width, Y + Height);
            }
            set {
                X = value.X - Width;
                Y = value.Y - Height;
            }
        }

        [XmlIgnore()]
        public PointE BottomLeft {
            get {
                return Location;
            }
            set {
                Location = value;
            }
        }

        [XmlIgnore()]
        public PointE BottomRight {
            get {
                return new PointE(X + Width, Y);
            }
            set {
                X = value.X - Width;
                Y = value.Y;
            }
        }

        [XmlIgnore()]
        public override RectangleE BoundingBox {
            get {
                return CloneRect();
            }
        }

        [XmlIgnore()]
        public IntervalE XInterval {
            get {
                return new IntervalE(X, Right);
            }
        }

        [XmlIgnore()]
        public IntervalE YInterval {
            get {
                return new IntervalE(Y, Top);
            }
        }

        [XmlIgnore()]
        public bool IsEmpty {
            get {
                return (m_Width == 0 || m_Height == 0);
            }
        }

        [XmlIgnore()]
        public double Area {
            get {
                return m_Width * m_Height;
            }
        }


        #endregion

        #region Operations

        public RectangleE Union(RectangleE r) {
            if (r.Height == 0 || r.Width == 0) {
                return CloneRect();
            }
            if (Height == 0 || Width == 0) {
                return r.CloneRect();
            }
            return new RectangleE(XInterval.Union(r.XInterval), YInterval.Union(r.YInterval));
        }

        public RectangleE Union(PointE pt) {
            if (Contains(pt)) {
                return CloneRect();
            }
            IntervalE xInt = XInterval.Union(pt.X);
            RectangleE ans = new RectangleE(xInt, YInterval.Union(pt.Y));
            return ans;
        }

        public override bool ContainsPoint(PointE point) {
            return Contains(point); //--simple wrapper for IShapeE compatibility
        }

        public bool Contains(PointE pt) {
            return XInterval.Contains(pt.X) && YInterval.Contains(pt.Y);
        }

        public override bool FullyContains(IShapeE shapeE) {
            if (shapeE == null) {
                return false;
            }
            return Contains(shapeE.BoundingBox);
        }

        public override bool MostlyContains(IShapeE shapeE, double tolerance) {
            if (shapeE == null) {
                return false;
            }
            RectangleE intersect = Intersect(shapeE.BoundingBox);
            return intersect.Area / Area > tolerance;
        }

        public bool Contains(RectangleE rect) {
            return XInterval.Contains(rect.XInterval) && YInterval.Contains(rect.YInterval);
        }

        public RectangleE Intersect(RectangleE r) {
            IntervalE xIntersect = XInterval.Intersect(r.XInterval);
            IntervalE yIntersect = YInterval.Intersect(r.YInterval);
            return new RectangleE(xIntersect, yIntersect);
        }

        public bool IntersectsWith(RectangleE r) {
            return XInterval.IntersectsWith(r.XInterval) && YInterval.IntersectsWith(r.YInterval);
        }

        public bool OverlapsWith(RectangleE r) {
            if (IntersectsWith(r)) return true;
            if (Contains(r)) return true;
            if (r.Contains(this)) return true;
            return false;
        }

        public void Inflate(SizeE size) {
            Inflate(size.Width, size.Height);
        }
        public void Inflate(double amount) {
            Inflate(amount, amount);
        }
        public void Inflate(double amountX, double amountY) {
            Inflate(amountX, amountX, amountY, amountY);
        }
        public void Inflate(double left, double right, double top, double bottom) {
            X -= left;
            Y -= top;
            Size = Size.Inflate(left + right, top + bottom);
        }

        public RectangleE ScaleCloneBy(double val) {
            RectangleE ans = new RectangleE(this);
            ans.ScaleSelfBy(val);
            return ans;
        }
        public RectangleE ScaleCloneBy(double val, PointE about) {
            RectangleE ans = new RectangleE(this);
            ans.ScaleSelfBy(val, about);
            return ans;
        }

        public void ScaleSelfBy(double val) {
            m_X = m_X * val;
            m_Y = m_Y * val;
            m_Width = m_Width * val;
            m_Height = m_Height * val;
        }

        public void ScaleSelfBy(double val, PointE about) {
            m_Width = m_Width * val;
            m_Height = m_Height * val;
            Location = Calc.PointBetween(about, Location, val);
        }

        public override string ToString() {
            return "RectangleE: X:" + X + ", Y:" + Y + ", W:" + Width + ", H:" + Height;
        }
        #endregion

        #region ICloneable Members

        public object Clone() {
            return CloneRect();
        }

        public RectangleE CloneRect() {
            return new RectangleE(X, Y, Width, Height);
        }

        public RectangleE ShiftRectangle(PointE point) {
            return new RectangleE(Location.ShiftPt(point), Size);
        }
        public override IShapeE Shift(PointE point) {
            return ShiftRectangle(point);
        }

        #endregion

        #region IComparable<RectangleE> Members

        public int CompareTo(RectangleE other) {
            return Area.CompareTo(other.Area);
        }

        #endregion

        public static explicit operator System.Drawing.Rectangle(RectangleE r) {
            return new System.Drawing.Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public static explicit operator System.Drawing.RectangleF(RectangleE r) {
            return new System.Drawing.RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
        }

        public static RectangleE FromMinMax(PointE minPt, PointE maxPt) {
            return new RectangleE(minPt, (SizeE)(maxPt - minPt));
        }
        public static RectangleE FromMinMax(double minX, double minY, double maxX, double maxY) {
            return new RectangleE(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
