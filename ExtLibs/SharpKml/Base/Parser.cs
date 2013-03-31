using System;
using System.IO;
using System.Reflection;
using System.Xml;
using SharpKml.Dom;

namespace SharpKml.Base
{
    /// <summary>
    /// Creates a structure of <see cref="Element"/>s from XML data.
    /// </summary>
    public sealed class Parser
    {
        // The maximum nesting depth we permit. Depths beyond this are treated as errors.
        private const int MaxNestingDepth = 100;

        private XmlReader _reader;
        private string _defaultNamespace;

        /// <summary>
        /// Raised when a new <see cref="Element"/> is parsed and added to the
        /// hierarchy.
        /// </summary>
        public event EventHandler<ElementEventArgs> ElementAdded;

        /// <summary>
        /// Gets the root <see cref="Element"/> found during parsing.
        /// </summary>
        public Element Root { get; private set; }

        /// <summary>
        /// Parses the specified stream for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="input">The stream containing the XML data.</param>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have sufficient permissions to access the
        /// location of the XML data.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void Parse(Stream input)
        {
            _defaultNamespace = null; // This method is strict about namespaces
            XmlReader reader = XmlReader.Create(input);
            this.Parse(reader);
        }

        /// <summary>
        /// Parses the specified stream for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="input">
        /// The <see cref="TextReader"/> containing the XML data.
        /// </param>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void Parse(TextReader input)
        {
            _defaultNamespace = null; // This method is strict about namespaces
            XmlReader reader = XmlReader.Create(input);
            this.Parse(reader);
        }

        /// <summary>
        /// Parses the specified string for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="xml">The XML data to parse.</param>
        /// <param name="namespaces">
        /// true to allow namespace support; false to ignore namespaces.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void ParseString(string xml, bool namespaces)
        {
            using (StringReader stream = new StringReader(xml))
            {
                if (namespaces)
                {
                    this.Parse(stream);
                }
                else
                {
                    _defaultNamespace = KmlNamespaces.Kml22Namespace;
                    var textReader = new XmlTextReader(stream);
                    textReader.Namespaces = false;
                    this.Parse(textReader);
                }
            }
        }

        private static void AssignValue(object instance, PropertyInfo property, string text)
        {
            if (!property.CanWrite)
            {
                return; // Can't do anything
            }

            // Get the type, checking if it's nullable, as TryParse doesn't exist on "int?"
            Type type = property.PropertyType;
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }

            object value;
            if (ValueConverter.TryGetValue(type, text, out value))
            {
                if (value != null)
                {
                    property.SetValue(instance, value, null);
                }
                else if (type.IsValueType || (nullableType != null)) // Make sure we can set it to null
                {
                    property.SetValue(instance, null, null);
                }
            }
        }

        private void AddChild(Element parent)
        {
            TypeBrowser browser = TypeBrowser.Create(parent.GetType());
            Element child = this.GetElement();
            if (child is UnknownElement)
            {
                PropertyInfo property = browser.FindElement(this.GetXmlComponent());
                if (property != null)
                {
                    AssignValue(parent, property, child.InnerText);

                    // We're not going to add it to the parent, which has the potential to
                    // lose any attributes/child elements assigned to the unknown, but this
                    // is the behaviour of the C++ version.
                    return;
                }
            }
            else if (parent.AddChild(child))
            {
                this.OnElementAdded(child); // Call it here after it's Parent has been set
                return; // Not an orphan
            }
            else // Lets try an Element as a proprty?
            {
                this.OnElementAdded(child); // Will be either added as a Property or Orphan

                // Search for a property that we can assign to
                Type type = child.GetType();
                foreach (var property in browser.Elements)
                {
                    if (property.Item1.PropertyType.IsAssignableFrom(type))
                    {
                        property.Item1.SetValue(parent, child, null);
                        return;
                    }
                }
            }

            parent.AddOrphan(child); // Save for later serialization
        }

        private Element GetElement()
        {
            if (_reader.Depth > MaxNestingDepth)
            {
                throw new InvalidOperationException("Maximum nesting depth has been reached.");
            }

            if (_reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            // Need to check this here before we move to the attributes,
            // as reader.NodeType will never be EndElement for empty elements
            // and when we move to an attribute, IsEmptyElement doesn't work
            bool isEmpty = _reader.IsEmptyElement;
            Element parent = KmlFactory.CreateElement(this.GetXmlComponent());
            if (parent == null)
            {
                parent = new UnknownElement(new XmlComponent(_reader));
            }
            else if (parent is IHtmlContent)
            {
                this.ProcessAttributes(parent);

                // No need to process all the children
                string text = string.Empty;
                if (!isEmpty) // Is there something to parse?
                {
                    text = XmlExtractor.FlattenXml(_reader);
                }

                ((IHtmlContent)parent).Text = text;
                return parent;
            }

            this.ProcessAttributes(parent); // Empties can have attributes though

            if (!isEmpty) // Is there any text/children to process?
            {
                while (_reader.Read())
                {
                    if (_reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }

                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            this.AddChild(parent);
                            break;
                        case XmlNodeType.CDATA: // Treat like normal text
                        case XmlNodeType.Text:
                            parent.AddInnerText(_reader.Value);
                            break;
                    }
                }
            }
            return parent;
        }

        private XmlComponent GetXmlComponent()
        {
            if (_defaultNamespace == null)
            {
                return new XmlComponent(_reader);
            }
            return new XmlComponent(null, _reader.LocalName, _defaultNamespace);
        }

        private void OnElementAdded(Element element)
        {
            var callback = this.ElementAdded;
            if (callback != null)
            {
                callback(this, new ElementEventArgs(element));
            }
        }

        private void Parse(XmlReader reader)
        {
            try
            {
                this.Root = null; // If anything bad happens, makes sure Root is empty.
                _reader = reader;
                if (_reader.MoveToContent() == XmlNodeType.Element) // Try to find the first element
                {
                    Element element = this.GetElement();
                    if (!(element is UnknownElement)) // Do not allow unknown root elements
                    {
                        if (element != null)
                        {
                            this.OnElementAdded(element);
                        }
                        this.Root = element;
                    }
                }
            }
            finally
            {
                _reader = null;
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
        }

        private void ProcessAttributes(Element element)
        {
            TypeBrowser browser = TypeBrowser.Create(element.GetType());
            while (_reader.MoveToNextAttribute())
            {
                // Check for namespaces first
                if (string.Equals("xmlns", _reader.Name, StringComparison.Ordinal))
                {
                    // Set default namespace only on unknown elements
                    if (element is UnknownElement)
                    {
                        element.Namespaces.AddNamespace(string.Empty, _reader.Value);
                    }
                }
                else if (string.Equals("xmlns", _reader.Prefix, StringComparison.Ordinal))
                {
                    element.Namespaces.AddNamespace(_reader.LocalName, _reader.Value);
                }
                else // just a normal attribute
                {
                    PropertyInfo property = browser.FindAttribute(new XmlComponent(null, _reader.LocalName, null)); // Attributes never have namespace info.
                    if (property != null)
                    {
                        AssignValue(element, property, _reader.Value);
                    }
                    else
                    {
                        // Unknown, save for later serialization
                        element.AddAttribute(new XmlComponent(_reader));
                    }
                }
            }
        }
    }
}
