using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Core.Xml
{
    public class XmlItem<T> : IXmlSerializable
    {
        private T m_Value;

        public XmlItem() { }
        public XmlItem(T value)
        {
            m_Value = value;
        }

        public T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            if (r.IsEmptyElement) {
                return;
            }
            r.ReadStartElement();
            XmlSerializer ser = XmlTypeAssociator<T>.GetSerializer(r.Name);
            XMLSerializeManager.Report("Deserializing: " + r.Name);
            m_Value = (T)ser.Deserialize(r);
            r.MoveToContent();
            r.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter w)
        {
            if (m_Value == null) {
                return;
            }
            XmlSerializer ser = XmlTypeAssociator<T>.GetSerializer(m_Value.GetType());
            XMLSerializeManager.Report("Serializing value: " + m_Value.GetType().Name);
            ser.Serialize(w, m_Value);
        }
    }
}
