using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Core.Xml
{
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader r)
        {
            r.ReadStartElement();
            bool hasRead = false;
            r.ReadStartElement("dictionary");
            int begin_d = r.Depth;
            while (readStartElementOK("item", r)) {

                r.ReadStartElement("key");
                XmlSerializer keySer = XmlTypeAssociator<TKey>.GetSerializer(r);
                XMLSerializeManager.Report("Deserializing Key: " + r.Name);
                TKey key = (TKey)keySer.Deserialize(r);
                readUntilEndElement("key", r);

                r.ReadStartElement("value");
                XmlSerializer valueSer = XmlTypeAssociator<TValue>.GetSerializer(r);
                XMLSerializeManager.Report("Deserializing Value: " + r.Name);
                TValue value = (TValue)valueSer.Deserialize(r);
                readUntilEndElement("value", r);

                Add(key, value);

                r.ReadEndElement();//--item

                r.MoveToContent();
                hasRead = true;
            }
            r.ReadEndElement();//--dictionary
            if (hasRead) {
                r.ReadEndElement();
            }
        }

        private bool readStartElementOK(string nodeName, XmlReader r)
        {
            try {
                r.ReadStartElement(nodeName);
                return true;
            }
            catch {
                return false;
            }
        }

        private void readUntilEndElement(string p, XmlReader r)
        {
            //--sometimes a Value Type may not move the reader's cursor to after its last element, so we need to make sure to move it to the right place...
            while (r.Name != p) {
                r.ReadEndElement();
            }
            r.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter w)
        {
            w.WriteStartElement("dictionary");
            foreach (TKey key in Keys) {
                w.WriteStartElement("item");

                w.WriteStartElement("key");
                XmlSerializer keySer = XmlTypeAssociator<TKey>.GetSerializer(key.GetType());
                XMLSerializeManager.Report("Serializing key: " + key.GetType().Name);
                keySer.Serialize(w, key);
                w.WriteEndElement();

                w.WriteStartElement("value");
                TValue value = this[key];
                XmlSerializer valueSer = XmlTypeAssociator<TValue>.GetSerializer(value.GetType());
                XMLSerializeManager.Report("Serializing value: " + value.GetType().Name);
                valueSer.Serialize(w, value);
                w.WriteEndElement();

                w.WriteEndElement();
            }
            w.WriteEndElement();
        }

        #endregion
    }

}
