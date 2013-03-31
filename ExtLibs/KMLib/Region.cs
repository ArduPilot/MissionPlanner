using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using System.Xml;
using System.Xml.Serialization;

namespace KMLib
{
    public class Region : AObject
    {
        private Lod m_Lod;
        public Lod Lod {
            get {
                if (m_Lod == null) {
                    m_Lod = new Lod();
                }
                return m_Lod;
            }
            set {
                m_Lod = value;
            }
        }

        private LatLonAltBox m_LatLonAltBox;
        public LatLonAltBox LatLonAltBox {
            get {
                return m_LatLonAltBox;
            }
            set {
                m_LatLonAltBox = value;
            }
        }
    }


    /*
      <Region>
        <Lod>
          <minLodPixels>128</minLodPixels><maxLodPixels>-1</maxLodPixels>
        </Lod>
        <LatLonAltBox>
          <north>37.430419921875</north><south>37.4249267578125</south>
          <east>-122.0965576171875</east><west>-122.10205078125</west>
        </LatLonAltBox>
      </Region>
     */
}
