using System;
using System.Collections.Generic;
using System.Text;
using Core.Utils;

namespace Core.Geometry
{
    public class Point3D
    {
        public Point3D()
        { }

        public Point3D(double x, double y, double z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        public Point3D(float x, float y, float z)
        {
            m_X = (double)x;
            m_Y = (double)y;
            m_Z = (double)z;
        }

        public Point3D(Point3D xyz)
        {
            m_X = xyz.m_X;
            m_Y = xyz.m_Y;
            m_Z = xyz.m_Z;
        }

        public Point3D(double x, double y)
        {
            m_X = x;
            m_Y = y;
            m_Z = 0;
        }

        private double m_X;
        public double X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }

        private double m_Y;
        public double Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }

        private double m_Z;
        public double Z
        {
            get
            {
                return m_Z;
            }
            set
            {
                m_Z = value;
            }
        }

        public virtual string Serialize()
        {
            return m_X.ToString(new System.Globalization.CultureInfo("en-US")) + "," + m_Y.ToString(new System.Globalization.CultureInfo("en-US")) + "," + m_Z.ToString(new System.Globalization.CultureInfo("en-US"));
        }

        public virtual void Deserialize(string str)
        {
            List<string> bits = StringUtils.SplitToList(str, ",");
            m_X = Convert.ToDouble(bits[0]);
            m_Y = Convert.ToDouble(bits[1]);
            m_Z = Convert.ToDouble(bits[2]);
        }

        public static Point3D MakePointFromStr(string str)
        {
            Point3D ans = new Point3D();
            ans.Deserialize(str);
            return ans;
        }

        public bool SameCoordsAs(Point3D point3D)
        {
            return m_X == point3D.m_X && m_Y == point3D.m_Y && m_Z == point3D.m_Z;
        }

        public override string ToString()
        {
            return "[Point3D: " + Serialize() + "]";
        }
    }
}
