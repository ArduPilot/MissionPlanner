using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Core.Xml
{
    public abstract class AXmlHandler
    {
        protected XmlDocument doc;
        protected string m_FilePath;
        public AXmlHandler() { }
        public AXmlHandler(string filepath) {
            LoadFile(filepath);
        }

        public string FilePath {
            get {
                return m_FilePath;
            }
            set {
                m_FilePath = value;
            }
        }

        protected void SetupXmlDeclaration() {
            doc = new XmlDocument();
            XmlNode xmldecl = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(xmldecl);
        }

        protected void LoadFile(string filepath) {
            XmlTextReader reader = new XmlTextReader(filepath);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            doc = new XmlDocument();
            doc.Load(reader);
            m_FilePath = filepath;
            reader.Close();
        }

        public virtual void SaveAs(string path) {
            doc.Save(path);
        }

        public void Save() {
            doc.Save(m_FilePath);
        }
    }
}
