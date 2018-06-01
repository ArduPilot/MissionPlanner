using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using Core.Xml;

namespace KMLib.Feature
{
    public class NetworkLink : AFeature
   {
        private Link m_Link;
        public Link Link {
            get {
                if (!XMLSerializeManager.Serializing && m_Link == null) {
                    m_Link = new Link();
                }
                return m_Link;
            }
            set {
                m_Link = value;
            }
        }
    }

    /*
     <NetworkLink>
      <name>001120</name>
      <Region>
        <Lod>
          <minLodPixels>128</minLodPixels><maxLodPixels>-1</maxLodPixels>
        </Lod>
        <LatLonAltBox>
          <north>37.430419921875</north><south>37.4249267578125</south>
          <east>-122.0965576171875</east><west>-122.10205078125</west>
        </LatLonAltBox>
      </Region>
      <Link>
        <href>180.kml</href>
        <viewRefreshMode>onRegion</viewRefreshMode>
      </Link>
    </NetworkLink>
     */
}
