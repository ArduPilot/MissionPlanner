using System;
using System.Xml;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Represents data found when parsing which is not recognized.
    /// </summary>
    public sealed class UnknownElement : Element, ICustomElement
    {
        private XmlComponent _data;

        /// <summary>
        /// Initializes a new instance of the UnknownElement class.
        /// </summary>
        /// <param name="data">The unrecognized XML data to store.</param>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        public UnknownElement(XmlComponent data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            _data = data.Clone(); // Don't store the data from the user but store a copy instead.
        }

        /// <summary>Gets the unrecognized data found during parsing.</summary>
        public XmlComponent UnknownData
        {
            get { return _data.Clone(); } // Don't give them our data to play with
        }

        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ICustomElement.ProcessChildren
        {
            get { return true; }
        }

        /// <summary>Writes the start of an XML element.</summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void ICustomElement.CreateStartElement(XmlWriter writer)
        {
            writer.WriteStartElement(_data.Prefix, _data.Name, _data.NamespaceUri);
        }
    }
}
