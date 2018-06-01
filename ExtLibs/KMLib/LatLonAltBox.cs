using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace KMLib
{
    public abstract class ALatLonBox
    {
        public ALatLonBox() { }
        public ALatLonBox(double n, double s, double e, double w) {
            m_north = n;
            m_south = s;
            m_east = e;
            m_west = w;
        }

        public ALatLonBox(ALatLonBox box) {
            m_north = box.north;
            m_south = box.south;
            m_east = box.east;
            m_west = box.west;
        }        

        private double m_north;
        public double north {
            get {
                return m_north;
            }
            set {
                m_north = value;
            }
        }

        private double m_south;
        public double south {
            get {
                return m_south;
            }
            set {
                m_south = value;
            }
        }

        private double m_east;
        public double east {
            get {
                return m_east;
            }
            set {
                m_east = value;
            }
        }

        private double m_west;
        public double west {
            get {
                return m_west;
            }
            set {
                m_west = value;
            }
        }
    }

    public class LatLonBox : ALatLonBox
    {
        public LatLonBox() { }
        public LatLonBox(double n, double s, double e, double w)
            : base(n, s, e, w) { }
        public LatLonBox(ALatLonBox box)
            : base(box) { }

        private double m_rotation;
        public double rotation {
            get {
                return m_rotation;
            }
            set {
                m_rotation = value;
                rotationSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool rotationSpecified = false;
    }

    public class LatLonAltBox : ALatLonBox
    {
        public LatLonAltBox() { }
        public LatLonAltBox(double n, double s, double e, double w)
            : base(n, s, e, w) { }

        public LatLonAltBox(ALatLonBox box)
            : base(box) { }

        private double m_minAltitude;
        public double minAltitude {
            get {
                return m_minAltitude;
            }
            set {
                m_minAltitude = value;
                minAltitudeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool minAltitudeSpecified = false;

        private double m_maxAltitude;
        public double maxAltitude {
            get {
                return m_maxAltitude;
            }
            set {
                m_maxAltitude = value;
                maxAltitudeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool maxAltitudeSpecified = false;
    }

    /*

    <LatLonAltBox>
      <north>43.374</north>
      <south>42.983</south>
      <east>-0.335</east>
      <west>-1.423</west>
      <minAltitude>0</minAltitude>
      <maxAltitude>0</maxAltitude>
    </LatLonAltBox>
 
     <LatLonBox>
    <north>...</north>                      <! kml:angle90 -->
    <south>...</south>                      <! kml:angle90 -->
    <east>...</east>                        <! kml:angle180 -->
    <west>...</west>                        <! kml:angle180 -->
    <rotation>0</rotation>                  <! kml:angle180 -->
  </LatLonBox>
     */
}
