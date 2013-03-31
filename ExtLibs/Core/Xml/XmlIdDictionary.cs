using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using Core.CommonObjects;

namespace Core.Xml
{
    public class XmlIdDictionary<T> : Dictionary<string, T>, IXmlSerializable, IIdItem where T : IIdItem
    {
        public void Add(T obj)
        {
            this[obj.Id] = obj;
        }

        public new void Add(string id, T obj)
        {
            obj.Id = id;
            Add(obj);
        }

        /*public T GetIdObjectHard(string id)
        {
            if (this.ContainsKey(id)) {
                return this[id];
            } else {
                T tnew = new T();
                tnew.Id = id;
                this[id] = tnew;
                return tnew;
            }
        }*/

        public T GetIdObject(string id)
        {
            if (this.ContainsKey(id)) {
                return this[id];
            } else {
                return default(T);
            }
        }

        #region IXmlIdObject Members

        private string m_Id;
        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }

        public void Init(string id)
        {
            m_Id = id;
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader r)
        {            
            r.ReadStartElement();
            Id = r.GetAttribute("id");
            r.ReadStartElement("items");
            if (r.NodeType == System.Xml.XmlNodeType.EndElement)
            {
                r.ReadEndElement();//--items
                return;
            }
            while (r.NodeType != System.Xml.XmlNodeType.EndElement) {
                XMLSerializeManager.Report("Deserializing: " + r.Name);
                XmlSerializer valueSer = XmlTypeAssociator<T>.GetSerializer(r);
                object obj = valueSer.Deserialize(r);
                T value = (T)obj;
                if (value.Id != null) {
                    this[value.Id] = value;
                }
            }
            r.ReadEndElement();//--items
            r.ReadEndElement();
        }

        public void WriteXml(XmlWriter w)
        {
            if (Id != null) {
                w.WriteAttributeString("id", Id);
            }            
            w.WriteStartElement("items");
            foreach (IIdItem value in Values) {
                XMLSerializeManager.Report("Serializing: " + value.GetType().Name);
                XmlSerializer valueSer = XmlTypeAssociator<T>.GetSerializer(value.GetType());
                valueSer.Serialize(w, value, new XmlSerializerNamespaces());
            }
            w.WriteEndElement();
        }

        #endregion
    }

}
