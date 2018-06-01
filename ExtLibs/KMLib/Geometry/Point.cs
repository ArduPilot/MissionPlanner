using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using System.Xml;
using System.Xml.Serialization;
using Core.Utils;

namespace KMLib.Geometry
{
    public class KmlPoint : AGeometry
    {
        public KmlPoint() { }

        /// <summary>
        /// A geographic location defined by longitude, latitude, and (optional) altitude. When a Point is contained by a Placemark, the point itself determines the position of the Placemark's name and icon. When a Point is extruded, it is connected to the ground with a line. This "tether" uses the current LineStyle.
        /// </summary>
        /// <param name="longitude">between -180 and 180</param>
        /// <param name="latitude">between -90 and 90</param>
        public KmlPoint(float longitude, float latitude) {
            Longitude = longitude;
            Latitude = latitude;
        }

        /// <summary>
        /// A geographic location defined by longitude, latitude, and (optional) altitude. When a Point is contained by a Placemark, the point itself determines the position of the Placemark's name and icon. When a Point is extruded, it is connected to the ground with a line. This "tether" uses the current LineStyle.
        /// </summary>
        /// <param name="longitude">between -180 and 180</param>
        /// <param name="latitude">between -90 and 90</param>
        /// <param name="altitude">altitude values (optional) are in meters above sea level</param>
        public KmlPoint(float longitude, float latitude, float altitude) {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
        }

        private float m_Longitude;
        /// <summary>
        /// longitude between -180 and 180
        /// </summary>
        [XmlIgnore()]
        public float Longitude {
            get {
                return m_Longitude;
            }
            set {
                if (value < -180 || value > 180) {
                    throw new NotSupportedException("Longitude must be between -180 and 180");
                }
                m_Longitude = value;
            }
        }

        private float m_Latitude;
        /// <summary>
        /// latitude between -90 and 90
        /// </summary>
        [XmlIgnore()]
        public float Latitude {
            get {
                return m_Latitude;
            }
            set {
                if (value < -90 || value > 90) {
                    throw new NotSupportedException("Latitude must be between -90 and 90");
                }
                m_Latitude = value;
            }
        }

        private float m_Altitude;
        /// <summary>
        /// altitude values (optional) are in meters above sea level
        /// </summary>
        [XmlIgnore()]
        public float Altitude {
            get {
                return m_Altitude;
            }
            set {
                m_Altitude = value;
            }
        }

        public string coordinates {
            get {
                if (m_Altitude == 0) {
                    return m_Longitude + "," + m_Latitude;
                } else {
                    return m_Longitude + "," + m_Latitude + "," + m_Altitude;
                }
            }
            set {
                string[] bits = StringUtils.Split(value, ",");
                if (bits.Length < 2) Fail(value);
                if (!float.TryParse(bits[0], out m_Longitude)) Fail(value);
                if (!float.TryParse(bits[1], out m_Latitude)) Fail(value);
                if (bits.Length == 3) {
                    if (!float.TryParse(bits[2], out m_Altitude)) Fail(value);
                }
            }
        }

        private void Fail(string value) {
            throw new Exception("coordinates string not valid: " + value);
        }
    }
}
