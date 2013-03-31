using System;
using System.Collections.Generic;
using System.Text;
using Core.Xml;
using System.Xml.Serialization;

namespace KMLib.Abstract
{
    public abstract class AOverlay : AFeature
    {
        private ColorKML m_color;
        public ColorKML color {
            get {
                return m_color;
            }
            set {
                m_color = value;
            }
        }

        private int m_drawOrder;
        public int drawOrder {
            get {
                return m_drawOrder;
            }
            set {
                m_drawOrder = value;
                drawOrderSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool drawOrderSpecified = false;

        private Icon m_Icon;
        public Icon Icon {
            get {
                if (!XMLSerializeManager.Serializing && m_Icon == null) {
                    m_Icon = new Icon();
                }
                return m_Icon;
            }
            set {
                m_Icon = value;
            }
        }
    }

    /*
  <!-- specific to Overlay -->
  <color>ffffffff</color>                   <!-- kml:color -->
  <drawOrder>0</drawOrder>                  <!-- int -->
  <Icon>
    <href>...</href>
  </Icon>
<!-- /Overlay -->*/
}
