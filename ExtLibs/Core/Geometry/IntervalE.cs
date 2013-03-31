using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Geometry
{
    public class IntervalE
    {
        #region Private Fields
        private double m_Maximum;
        private double m_Minimum;
        #endregion

        #region Constructors
        public IntervalE(double min, double max)
        {
            m_Minimum = min;
            m_Maximum = max;
        }
        public IntervalE()
        { }
        #endregion

        #region GetSet
        [XmlIgnore()]
        public double Dimension
        {
            get
            {
                return m_Maximum - m_Minimum;
            }
            set
            {
                m_Maximum = m_Minimum + value;
            }
        }

        [XmlAttribute()]
        public double Minimum
        {
            get
            {
                return m_Minimum;
            }
            set
            {
                m_Minimum = value;
            }
        }

        [XmlAttribute()]
        public double Maximum
        {
            get
            {
                return  m_Maximum;
            }
            set
            {
                m_Maximum = value;
            }
        }

        [XmlIgnore()]
        public bool IsEmpty
        {
            get
            {
                return (Dimension == 0);
            }
        }
        #endregion

        public bool IntersectsWith(IntervalE b)
        {
            if (b.Minimum <= Minimum)
            {
                return b.Contains(Minimum);
            }
            else
            {
                return b.Contains(Maximum);
            }
        }

        public bool Contains(double testPos)
        {
            return testPos >= Minimum && testPos <= Maximum;
        }

        public bool Contains(IntervalE testInterval)
        {
            return testInterval.Minimum >= Minimum && testInterval.Maximum <= Maximum;
        }

        public IntervalE Union(double uPos)
        {
            return new IntervalE(Math.Min(Minimum, uPos), Math.Max(Maximum, uPos));
        }

        public IntervalE Union(IntervalE b)
        {
            return new IntervalE(Math.Min(Minimum, b.Minimum), Math.Max(Maximum, b.Maximum));
        }

        public IntervalE Intersect(IntervalE b)
        {
            if (b.Minimum < Minimum)
            {
                return new IntervalE(Minimum, b.Maximum);
            }
            else
            {
                return new IntervalE(b.Minimum, Maximum);
            }
        }

        public override string ToString()
        {
            return "IntervalE (" + Minimum + ", " + Maximum + ")";
        }
    }
}
