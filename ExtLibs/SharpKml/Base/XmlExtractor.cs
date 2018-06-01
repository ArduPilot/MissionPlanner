using System.Text;
using System.Xml;

namespace SharpKml.Base
{
    /// <summary>
    /// Used to get the inner text of a XML element.
    /// </summary>
    /// <remarks>
    /// XmlReader.ReadInnerXml() is namespace aware, meaning it will add namespace
    /// declarations to any child element which inherits a namespace (e.g. if
    /// ReadInnerXml() is called on the following &lt;root&gt; node:
    /// <![CDATA[<root xmlns="http://example.com"><child>Some text</child></root>]]>
    /// the returned value would be
    /// <![CDATA[<child xmlns="http://example.com">Some text</child>]]>)
    /// The output produced by this class matches the C++ version.
    /// </remarks>
    internal class XmlExtractor
    {
        private XmlReader _reader;
        private StringBuilder _xml = new StringBuilder();

        private XmlExtractor(XmlReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Extracts the inner XML of the XmlReader, without adding additional
        /// namespaces.
        /// </summary>
        /// <param name="reader">The XmlReader to extract data from.</param>
        /// <returns>
        /// A string representing the inner XML of the current XML node.
        /// </returns>
        public static string FlattenXml(XmlReader reader)
        {
            XmlExtractor instance = new XmlExtractor(reader);
            instance.ProcessChild();
            return instance._xml.ToString();
        }

        private string GetAttributes()
        {
            StringBuilder sb = new StringBuilder();
            while (_reader.MoveToNextAttribute())
            {
                sb.AppendFormat(" {0}=\"{1}\"", _reader.Name, _reader.Value);
            }
            return sb.ToString();
        }

        private void ProcessChild()
        {
            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (_reader.IsEmptyElement) // Check here before we access attributes.
                        {
                            _xml.AppendFormat("<{0}{1} />", _reader.Name, this.GetAttributes());
                        }
                        else
                        {
                            _xml.AppendFormat("<{0}{1}>", _reader.Name, this.GetAttributes());
                            this.ProcessChild();
                            _xml.AppendFormat("</{0}>", _reader.Name);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        return;
                    case XmlNodeType.CDATA: // Fall through
                    case XmlNodeType.Text:
                        _xml.Append(_reader.Value);
                        break;
                }
            }
        }
    }
}
