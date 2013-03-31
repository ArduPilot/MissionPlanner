using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using KMLib.Geometry;

namespace KMLib.Geometry
{
    public class Model : AGeometry
    {
        public struct ALocation
        {
            public double latitude;
            public double longitude;
            public double altitude;
        }

        public struct AOrientation
        {
            public double heading;
            public double tilt;
            public double roll;
        }

        public struct AScale
        {
            public float x;
            public float y;
            public float z;
        }

        public ALocation Location;
        public AOrientation Orientation;
        public AScale Scale;
        Link m_Link;

        /*
        public Location Location
        {
            get
            {
                return m_Location as Location;
            }
            set
            {
                m_Location = value;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return m_Orientation as Orientation;
            }
            set
            {
                m_Orientation = value;
            }
        }

        public Scale Scale
        {
            get
            {
                return m_Scale as Scale;
            }
            set
            {
                m_Scale = value;
            }
        }
        */
        public Link Link
        {
            get
            {
                return m_Link as Link;
            }
            set
            {
                m_Link = value;
            }
        }
    }
}
