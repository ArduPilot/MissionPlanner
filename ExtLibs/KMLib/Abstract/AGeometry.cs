using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KMLib.Abstract
{
    public abstract class AGeometry
    {
        private IntBool m_Tessellate;
        [XmlElement("tessellate")]
        public IntBool Tessellate {
            get {
                return m_Tessellate;
            }
            set {
                m_Tessellate = value;
            }
        }

        private IntBool m_Extrude;
        [XmlElement("extrude")]
        public IntBool Extrude {
            get {
                return m_Extrude;
            }
            set {
                m_Extrude = value;
            }
        }

        private AltitudeMode m_altitudeMode = AltitudeMode.relativeToGround;
        [XmlElement("altitudeMode")]
        public AltitudeMode AltitudeMode {
            get {
                return m_altitudeMode;
            }
            set {
                m_altitudeMode = value;
                AltitudeModeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool AltitudeModeSpecified = false;
    }
}
