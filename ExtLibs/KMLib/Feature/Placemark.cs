using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml;
using KMLib.Abstract;
using KMLib.Geometry;

namespace KMLib.Feature
{
    public enum GeometryType
    {
        Empty, Point, LinearRing, LineString, Model, MultiGeometry, Polygon
    }

    public class Placemark : AFeature
    {
        private AGeometry m_Geometry;
        [XmlIgnore()]
        public AGeometry Geometry {
            get {
                return m_Geometry;
            }
            set {
                m_Geometry = value;
            }
        }

        public KmlPoint Point {
            get {
                return m_Geometry as KmlPoint;
            }
            set {
                m_Geometry = value;
            }
        }

        public LinearRing LinearRing {
            get {
                return m_Geometry as LinearRing;
            }
            set {
                m_Geometry = value;
            }
        }

        public LineString LineString {
            get {
                return m_Geometry as LineString;
            }
            set {
                m_Geometry = value;
            }
        }

        public Model Model {
            get {
                return m_Geometry as Model;
            }
            set {
                m_Geometry = value;
            }
        }

        public MultiGeometry MultiGeometry {
            get {
                return m_Geometry as MultiGeometry;
            }
            set {
                m_Geometry = value;
            }
        }

        public Polygon Polygon {
            get {
                return m_Geometry as Polygon;
            }
            set {
                m_Geometry = value;
            }
        }

        [XmlIgnore()]
        public GeometryType GeometryType {
            get {
                if (m_Geometry == null) {
                    return GeometryType.Empty;
                } else if (m_Geometry is KmlPoint) {
                    return GeometryType.Point;
                } else if (m_Geometry is LinearRing) {
                    return GeometryType.LinearRing;
                } else if (m_Geometry is LineString) {
                    return GeometryType.LineString;
                } else if (m_Geometry is Model) {
                    return GeometryType.Model;
                } else if (m_Geometry is MultiGeometry) {
                    return GeometryType.MultiGeometry;
                } else if (m_Geometry is Polygon) {
                    return GeometryType.Polygon;
                }
                return GeometryType.Empty;
            }
        }
    }


    /*public class Placemark : ADoc
    {
        private LineString m_LineString;
        public LineString LineString
        {
            get
            {
                return m_LineString;
            }
            set
            {
                m_LineString = value;
            }
        }

        private IntBool m_Open;
        [XmlElement("open")]
        public IntBool Open
        {
            get
            {
                return m_Open;
            }
            set
            {
                m_Open = value;
            }
        }

        private Polygon m_Polygon;
        public Polygon Polygon
        {
            get
            {
                return m_Polygon;
            }
            set
            {
                m_Polygon = value;
            }
        }
    }*/

}
