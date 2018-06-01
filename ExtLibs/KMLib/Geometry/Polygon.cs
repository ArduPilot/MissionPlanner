using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KMLib.Abstract;

namespace KMLib
{
    public class Polygon : AGeometry
    {
        private BoundaryIs m_OuterBoundaryIs = new BoundaryIs();//--required for all polygons
        private BoundaryIs m_InnerBoundaryIs;//--optional

        [XmlElement("outerBoundaryIs")]
        public BoundaryIs OuterBoundaryIs
        {
            get
            {
                return m_OuterBoundaryIs;
            }
            set
            {
                m_OuterBoundaryIs = value;
            }
        }

        [XmlElement("innerBoundaryIs")]
        public BoundaryIs InnerBoundaryIs
        {
            get
            {
                return m_InnerBoundaryIs;
            }
            set
            {
                m_InnerBoundaryIs = value;
            }
        }
    }
}
