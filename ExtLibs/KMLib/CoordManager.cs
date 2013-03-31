using System;
using System.Collections.Generic;
using System.Text;
using Core.Geometry;

namespace KMLib
{
    public class CoordManager
    {
        //--longitude is X, latitude is Y

        double degreeInKm = 111;
        double kmInFt = 3280.8399;

        public Point3D ConvertToLatLong(double x, double y, double z)
        {
            Point3D ans = new Point3D();
            ans.X = m_LatLongOrigin.X + ConvertFtToLong(x);
            ans.Y = m_LatLongOrigin.Y + ConvertFtToLat(y);
            ans.Z = z / (kmInFt/1000);
            return ans;
        }

        private double ConvertFtToLong(double ft)
        {
            return ConvertFtToLat(ft) / Math.Cos(m_LatLongOrigin.Y*(Math.PI/180));
        }

        private double ConvertFtToLat(double ft)
        {            
            return (ft / kmInFt) / degreeInKm;
        }

        private PointE m_LatLongOrigin = new PointE(-77.05, 38.97);
        public PointE LatLongOrigin
        {
            get
            {
                return m_LatLongOrigin;
            }
            set
            {
                m_LatLongOrigin = value;
            }
        } 
    }
}
