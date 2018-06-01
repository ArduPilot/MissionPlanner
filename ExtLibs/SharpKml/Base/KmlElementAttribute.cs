using System;

namespace SharpKml.Base
{
    /// <summary>
    /// Specifies a class or class member should be serialized as an XML element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class KmlElementAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the KmlElementAttribute class.
        /// </summary>
        /// <param name="elementName">
        /// The name of the generated XML element.
        /// </param>
        /// <param name="order">
        /// The explicit order in which the element is serialized.
        /// </param>
        public KmlElementAttribute(string elementName, int order = 0)
            : this(elementName, KmlNamespaces.Kml22Namespace, order) // Default namespace is KML
        {
        }

        /// <summary>
        /// Initializes a new instance of the KmlElementAttribute class.
        /// </summary>
        /// <param name="elementName">
        /// The name of the generated XML element.
        /// </param>
        /// <param name="namespace">
        /// The namespace assigned to the XML element.
        /// </param>
        /// <param name="order">
        /// The explicit order in which the element is serialized.
        /// </param>
        public KmlElementAttribute(string elementName, string @namespace, int order = 0)
        {
            this.ElementName = elementName;
            this.Namespace = @namespace;
            this.Order = order;
        }

        /// <summary>Gets the name of the generated XML element.</summary>
        public string ElementName { get; private set; }

        /// <summary>
        /// Gets the namespace assigned to the XML element that results when the
        /// class is serialized.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the explicit order in which the elements are serialized.
        /// </summary>
        public int Order { get; private set; }
    }
}
