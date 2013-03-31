using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;
using System.Xml;
using System.Xml.Serialization;

namespace KMLib
{
    public class TimeSpan : ATimePrimitive
    {
        public TimeSpan() { }
        public TimeSpan(string beginTime, string endTime) {
            m_Begin = beginTime;
            m_End = endTime;
        }
        private string m_Begin;
        [XmlElement("begin")]
        public string Begin
        {
            get
            {
                return m_Begin;
            }
            set
            {
                m_Begin = value;
            }
        }

        private string m_End;
        [XmlElement("end")]
        public string End
        {
            get
            {
                return m_End;
            }
            set
            {
                m_End = value;
            }
        }
    }
}
