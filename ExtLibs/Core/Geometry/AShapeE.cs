using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Geometry
{
    public interface IShapeE
    {
        event EventHandler GeometryChanged;       
        RectangleE BoundingBox { get; }
        bool ContainsPoint(PointE point);
        bool MostlyContains(IShapeE iShapeE);
        bool FullyContains(IShapeE iShapeE);
        IShapeE Shift(PointE pointE);
        object Tag { get; set; }
    }

    public interface IShapeEUpdateable : IShapeE
    {
        event ShapeUpdateEventHandler ShapeUpdated;
        void UpdateShape(IShapeE newShape);
    }

    public delegate void ShapeUpdateEventHandler(object sender, ShapeUpdateEventArgs e);
    public class ShapeUpdateEventArgs : EventArgs
    {
        public IShapeE OldShape;
        public IShapeE NewShape;
        public ShapeUpdateEventArgs(IShapeE oldShape, IShapeE newShape)
        {
            OldShape = oldShape;
            NewShape = newShape;
        }
    }

    public abstract class AShapeE : IShapeEUpdateable
    {
        #region IShape Members
        public event EventHandler GeometryChanged;
        public event ShapeUpdateEventHandler ShapeUpdated;
        public abstract RectangleE BoundingBox { get; }
        public abstract IShapeE Shift(PointE point);

        public virtual bool ContainsPoint(PointE point)
        {
            return false;
        }

        public bool MostlyContains(IShapeE shapeE)
        {
            return MostlyContains(shapeE, 0.95);
        }
        public virtual bool MostlyContains(IShapeE shapeE, double tolerance)
        {
            return false;
        }

        public virtual bool FullyContains(IShapeE shapeE)
        {
            return false;
        }
        #endregion

        protected void CallGeometryChanged()
        {
            if (GeometryChanged != null)
            {
                GeometryChanged(this, EventArgs.Empty);
            }
        }

        public void UpdateShape(IShapeE newShape)
        {
            if (ShapeUpdated != null) {
                ShapeUpdated(this, new ShapeUpdateEventArgs(this, newShape));
            }
        }

        public static int Compare(IShapeE a, IShapeE b)
        {
            if (a.Equals(b)) {
                return 0;
            }
            if (a == null || a.BoundingBox == null) {
                return -1;
            }
            if (b == null || b.BoundingBox == null) {
                return 1;
            }

            return b.BoundingBox.CompareTo(a.BoundingBox); 
        }

        private object m_Tag;
        public object Tag {
            get {
                return m_Tag;
            }
            set {
                m_Tag = value;
            }
        }
    }
}
