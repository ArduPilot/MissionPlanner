using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Core.Xml;
using KMLib.Abstract;
using System.IO;
using KMLib.Feature;

namespace KMLib
{
    //[XmlRoot(ElementName = "kml", Namespace = "http://earth.google.com/kml/2.1")]
    //--if you add the namespace, then all children get a blank namespace attrib...
    [XmlRoot(ElementName = "kml")]
    public class KMLRoot
    {
        public static bool deserializing;
        public static KMLRoot Load(string path) {
            deserializing = true;
            KMLRoot ans = XmlSerializer<KMLRoot>.DeserializeObjectFromFile(path);
            deserializing = false;
            return ans;
        }

        private bool serializing;
        public void Save(string path) {
            serializing = true;
            XmlSerializer<KMLRoot>.SerializeObjectToFile(this, path);
            serializing = false;
        }

        private Document m_Document;
        public KMLRoot() {
        }

        [XmlIgnore()]
        public bool UsesDocument {
            get {
                return m_Feature == null;
            }
        }

        public Document Document {
            get {
                if (!serializing && m_Document == null) {
                    if (m_Feature != null) {
                        throw new Exception("Cannot use a Document and other features at the root level. If you have more than 1 feature, add all features to the Document (not the root).");
                    }
                    m_Document = new Document();                    
                }
                return m_Document;
            }
            set {
                m_Document = value;                
            }
        }

        private AFeature m_Feature;
        [XmlElement(ElementName = "Folder", Type = typeof(Folder))]
        [XmlElement(ElementName = "Placemark", Type = typeof(Placemark))]
        [XmlElement(ElementName = "NetworkLink", Type = typeof(NetworkLink))]
        [XmlElement(ElementName = "GroundOverlay", Type = typeof(GroundOverlay))]
        [XmlElement(ElementName = "ScreenOverlay", Type = typeof(ScreenOverlay))]
        public AFeature Feature {
            get {
                return m_Feature;
            }
            set {
                if (m_Document != null) {
                    throw new Exception("Cannot set features at the root level if a Document node is already defined.");
                }
                m_Feature = value;
            }
        }        
    }
}
