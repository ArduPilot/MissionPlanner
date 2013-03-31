using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Core.ExtendedObjects;

namespace Core.Xml
{
    /// <summary>
    /// Although MS Lists are XML serializable, there is no way to serialize Lists of interface types. XmlLists allow you to associate derived types using "AddType". This is an alternative to using the XmlArrayItem attribute that has to be applied to a particular property or field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlList<T> : EventList<T>, IXmlSerializable
    {
        public XmlList() : base() { }
        public XmlList(IEnumerable<T> collection) : base(collection) { }
        public XmlList(int capacity) : base(capacity) { }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            int depth = r.Depth;           
            r.ReadStartElement();
            bool hasRead = false;
            while (r.Depth > depth) {
                if (r.IsStartElement()) {
                    XmlSerializer ser = XmlTypeAssociator<T>.GetSerializer(r);
                    XMLSerializeManager.Report("Deserializing: " + r.Name);
                    Add((T)ser.Deserialize(r));
                    r.MoveToContent();
                } else {
                    r.Read();
                }
                hasRead = true;
            }
            if (hasRead) {
                r.ReadEndElement();
            }
        }

        public void WriteXml(System.Xml.XmlWriter w)
        {
            foreach (T item in this) {
                XmlSerializer ser = XmlTypeAssociator<T>.GetSerializer(item.GetType());
                XMLSerializeManager.Report("Serializing item: " + item.GetType().Name);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                ser.Serialize(w, item, ns);
            }
        }
    }
}
