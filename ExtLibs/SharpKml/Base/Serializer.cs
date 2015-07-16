using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SharpKml.Dom;

namespace SharpKml.Base
{
    /// <summary>
    /// Serializes a derived class of <see cref="Element"/> into XML data.
    /// </summary>
    public class Serializer
    {
        private StringBuilder _data = new StringBuilder();
        private XmlWriter _writer;

        /// <summary>
        /// Gets the XML content after the most recent call to
        /// <see cref="Serialize(Element)"/>.
        /// </summary>
        public string Xml
        {
            get
            {
                string ans = _data.ToString();

                return ans;
            }
        }

        /// <summary>
        /// Serializes the specified <see cref="Element"/> to XML.
        /// </summary>
        /// <param name="root">
        /// The <c>Element</c> to serialize, including all its children.
        /// </param>
        /// <remarks>
        /// The generated XML will be indented and have a full XML
        /// declaration header.
        /// </remarks>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        public void Serialize(Element root)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            //settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            this.Serialize(root, settings);
        }

        /// <summary>
        /// Serializes the specified <see cref="Element"/> to XML without formatting.
        /// </summary>
        /// <param name="root">
        /// The <c>Element</c> to serialize, including all its children.
        /// </param>
        /// <remarks>
        /// The generated XML will not contain white space between elements and
        /// the XML declaration will also be omitted.
        /// </remarks>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        public void SerializeRaw(Element root)
        {
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            this.Serialize(root, settings);
        }

        private static string GetString(object value)
        {
            Type type = value.GetType();
            if (type.IsEnum)
            {
                KmlElementAttribute att = TypeBrowser.GetEnum((Enum)value);
                if (att != null)
                {
                    return att.ElementName;
                }
            }
            return string.Format(KmlFormatter.Instance, "{0}", value);
        }

        private void Serialize(Element root, XmlWriterSettings settings)
        {
            // We check here so the two public functions don't need to
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            XmlWriter writer = null;
            try
            {
                _data = new StringBuilder();
                writer = XmlWriter.Create(_data, settings);
                _writer = writer;
                this.SerializeElement(root);
            }
            finally
            {
                _writer = null;
                if (writer != null)
                {
                    ((IDisposable)writer).Dispose();
                }
            }
        }

        private void SerializeElement(Element element)
        {
            // Write start tag
            XmlComponent component = KmlFactory.FindType(element.GetType());
            ICustomElement customElement = element as ICustomElement;
            if (customElement != null) // Takes priority over component
            {
                customElement.CreateStartElement(_writer);
                if (!customElement.ProcessChildren)
                {
                    return; // Don't need to to any more work.
                }
            }
            else if (component != null)
            {
                _writer.WriteStartElement(component.Name, component.NamespaceUri);
            }
            else // We can't handle it so ignore it
            {
                System.Diagnostics.Debug.WriteLine("Unknown Element type - please register first." + element.GetType());
                return; // Skip
            }

            // Write the attributes - unknown, serialized then namespaces.
            foreach (var att in element.Attributes)
            {
                _writer.WriteAttributeString(att.Prefix, att.Name, att.NamespaceUri, att.Value);
            }

            this.WriteAttributes(element);

            foreach (var ns in element.Namespaces.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml))
            {
                _writer.WriteAttributeString("xmlns", ns.Key, string.Empty, ns.Value);
            }

            // Now the text part
            this.WriteData(element.InnerText);

            // Now write the elements - serialized, children then unknown children.
            this.WriteElements(element);
            this.SerializeElements(element.OrderedChildren);
            this.SerializeElements(element.Orphans);

            // Finished...
            _writer.WriteEndElement();
        }

        private void SerializeElements(IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                this.SerializeElement(element);
            }
        }

        private void WriteAttributes(Element element)
        {
            TypeBrowser browser = TypeBrowser.Create(element.GetType());
            foreach (var property in browser.Attributes)
            {
                object value = property.Item1.GetValue(element, null);
                if (value != null) // Make sure it needs saving
                {
                    _writer.WriteAttributeString(property.Item2.AttributeName, GetString(value));
                }
            }
        }

        private void WriteData(string data)
        {
            // The XmlWriter will escape any illegal XML characters, but the original
            // C++ code would CDATA it instead, making sure it's not already a CDATA.

            // First make sure there is some data, as if we write string.Empty
            // then WriteEndElement will always write a full end element.
            if (!string.IsNullOrEmpty(data))
            {
                if (!data.Contains("<![CDATA[") &&
                    (data.IndexOfAny(new char[] { '&', '\'', '<', '>', '\"' }) != -1))
                {
                    // Illegal character found and the string isn't CDATA
                    _writer.WriteCData(data);
                }
                else
                {
                    // Just write normal - XmlWriter takes care of illegal characters
                    _writer.WriteString(data);
                }
            }
        }

        private void WriteElements(Element element)
        {
            TypeBrowser browser = TypeBrowser.Create(element.GetType());

            foreach (var property in browser.Elements)
            {
                object value = property.Item1.GetValue(element, null);
                if (value != null) // Make sure it needs saving
                {
                    if (property.Item2.ElementName == null) // This is an element
                    {
                        this.SerializeElement((Element)value);
                    }
                    else
                    {
                        _writer.WriteStartElement(property.Item2.ElementName, property.Item2.Namespace);
                        this.WriteData(GetString(value));
                        _writer.WriteEndElement();
                    }
                }
            }
        }
    }
}
