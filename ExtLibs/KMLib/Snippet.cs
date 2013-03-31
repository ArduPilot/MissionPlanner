using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace KMLib
{
    public class Snippet
    {
        public Snippet() { }
        public Snippet(string text) {
            m_Text = text;
        }

        private int m_maxLines = 2;
        [XmlAttribute()]
        public int maxLines {
            get {
                return m_maxLines;
            }
            set {
                m_maxLines = value;
            }
        }

        private string m_Text;
        [XmlText()]
        public string Text {
            get {
                return m_Text;
            }
            set {
                m_Text = value;
            }
        }

        public static implicit operator string(Snippet comp) {
            return comp.Text;
        }

        public static implicit operator Snippet(string comp) {
            return new Snippet(comp);
        }
    }

    /*public class Snippet : AXmlSerializable
    {
        private int m_maxLines = 2;
        public int maxLines {
            get {
                return m_maxLines;
            }
            set {
                m_maxLines = value;
            }
        }

        private string m_Text;
        public string Text {
            get {
                return m_Text;
            }
            set {
                m_Text = value;
            }
        }

        public override void ReadXml(System.Xml.XmlReader r) {
            r.ReadStartElement("Snippet");
            if (r.HasAttributes) {
                r.MoveToAttribute("maxLines");
                m_maxLines = r.ReadContentAsInt();
            }
            r.MoveToContent();
            m_Text = r.ReadString();
            r.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter w) {
            w.WriteStartElement("Snippet");
            w.WriteAttributeString("maxLines", m_maxLines.ToString());
            w.WriteString(m_Text);
            w.WriteEndElement();
        }
    }*/
}
