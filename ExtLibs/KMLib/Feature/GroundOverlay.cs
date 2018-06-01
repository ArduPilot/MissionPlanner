using System;
using System.Collections.Generic;
using System.Text;
using Core.Xml;
using KMLib.Abstract;
using System.Xml.Serialization;

namespace KMLib.Feature
{
    public class GroundOverlay : AOverlay
    {
        private double m_altitude = 0;
        public double altitude {
            get {
                return m_altitude;
            }
            set {
                m_altitude = value;
                altitudeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool altitudeSpecified = false;

        private AltitudeMode m_altitudeMode = AltitudeMode.clampedToGround;
        public AltitudeMode altitudeMode {
            get {
                return m_altitudeMode;
            }
            set {
                m_altitudeMode = value;
                altitudeModeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool altitudeModeSpecified = false;

        private LatLonBox m_LatLonBox;
        public LatLonBox LatLonBox {
            get {
                if (!XMLSerializeManager.Serializing && m_LatLonBox == null) {
                    m_LatLonBox = new LatLonBox();
                }
                return m_LatLonBox;
            }
            set {
                m_LatLonBox = value;
            }
        }
    }
}
/*
  <!-- specific to GroundOverlay -->
  <altitude>0</altitude>                    <!-- double -->
  <altitudeMode>clampToGround</altitudeMode>
     <!-- kml:altitudeModeEnum: clampToGround or absolute --> 
  <LatLonBox>
    <north>...</north>                      <! kml:angle90 -->
    <south>...</south>                      <! kml:angle90 -->
    <east>...</east>                        <! kml:angle180 -->
    <west>...</west>                        <! kml:angle180 -->
    <rotation>0</rotation>                  <! kml:angle180 -->
  </LatLonBox>
 */
