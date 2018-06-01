using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KMLib.Feature;

namespace KMLib.Abstract
{
    public abstract class AContainer : AFeature
    {
        public void Add(AFeature feature) {
            if (m_List == null) {
                m_List = new List<AFeature>();
            }
            m_List.Add(feature);
        }

        private List<AFeature> m_List;
        [XmlElement(ElementName = "Folder", Type = typeof(Folder))]
        [XmlElement(ElementName = "Placemark", Type = typeof(Placemark))]
        [XmlElement(ElementName = "NetworkLink", Type = typeof(NetworkLink))]
        [XmlElement(ElementName = "GroundOverlay", Type = typeof(GroundOverlay))]
        [XmlElement(ElementName = "ScreenOverlay", Type = typeof(ScreenOverlay))]
        public List<AFeature> List {
            get {
                return m_List;
            }
            set {
                m_List = value;
            }
        }
    }
}
