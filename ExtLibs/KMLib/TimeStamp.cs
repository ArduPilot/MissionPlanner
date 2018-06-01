using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using System.Xml;
using System.Xml.Serialization;

namespace KMLib
{
    public class TimeStamp : ATimePrimitive
    {
        public TimeStamp() { }
        public TimeStamp(string time)
        {
            m_Time = time;
        }
        private string m_Time;
        [XmlElement("when")]
        public string Time
        {
            get
            {
                return m_Time;
            }
            set
            {
                m_Time = value;
            }
        }
    }
}
