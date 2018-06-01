using System;
using System.Collections.Generic;
using System.Text;

namespace KMLib
{
    public class BoundaryIs
    {
        private LinearRing m_LinearRing = new LinearRing();
        public LinearRing LinearRing
        {
            get
            {
                return m_LinearRing;
            }
            set
            {
                m_LinearRing = value;
            }
        }
    }
}
