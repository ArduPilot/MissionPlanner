using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using KMLib.Abstract;

namespace KMLib
{
    public class LineString : AGeometry
    {        
        private Coordinates m_coordinates;
        public Coordinates coordinates
        {
            get
            {
                return m_coordinates;
            }
            set
            {
                m_coordinates = value;
            }
        }
    }
}
