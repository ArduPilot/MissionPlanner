using System;
using System.Xml;

namespace SharpKml.Base
{
    /// <summary>
    /// Represents general information about XML attributes and elements.
    /// </summary>
    public class XmlComponent : IEquatable<XmlComponent>
    {
        /// <summary>Initializes a new instance of the XmlComponent class.</summary>
        /// <param name="reader">
        /// A <see cref="XmlReader"/> object to extract information from. The
        /// reader will be left in the same state and will not be modified.
        /// </param>
        public XmlComponent(XmlReader reader)
        {
            // Important to set Prefix first in case SetName finds one.
            this.Prefix = reader.Prefix;
            this.SetName(reader.LocalName, reader.NamespaceURI);
            this.Value = reader.Value;
        }

        /// <summary>Initializes a new instance of the XmlComponent class.</summary>
        /// <param name="prefix">The XML prefix.</param>
        /// <param name="local">The XML local name.</param>
        /// <param name="uri">The XML namespace.</param>
        public XmlComponent(string prefix, string local, string uri)
        {
            this.Prefix = prefix ?? string.Empty;
            this.SetName(local, uri);
            this.Value = string.Empty;
        }

        // Used by the Clone method
        private XmlComponent()
        {
        }

        /// <summary>Gets the local XML name of the component.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the XML namespace URI of the component.</summary>
        public string NamespaceUri { get; private set; }

        /// <summary>Gets the XML prefix of the component.</summary>
        public string Prefix { get; private set; }

        /// <summary>Gets or sets the value of the component.</summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new instance of the XmlComponent class with the same
        /// information as this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public XmlComponent Clone()
        {
            XmlComponent output = new XmlComponent();
            output.Name = this.Name;
            output.NamespaceUri = this.NamespaceUri;
            output.Prefix = this.Prefix;
            output.Value = this.Value;
            return output;
        }

        /// <summary>
        /// Indicates whether this instance and another specified XmlComponent
        /// have the same local name and namespace URI.
        /// </summary>
        /// <param name="other">
        /// The XmlComponent to compare with this object.
        /// </param>
        /// <returns>
        /// true if the value of the value parameter is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(XmlComponent other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Name, other.Name, StringComparison.Ordinal) &&
                   string.Equals(this.NamespaceUri, other.NamespaceUri, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must
        /// also be an XmlComponent object, have the same value.
        /// </summary>
        /// <param name="obj">The XmlComponent to compare with this object.</param>
        /// <returns>
        /// true if the value parameter is an XmlComponent and its value is the
        /// same as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as XmlComponent);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^
                   this.NamespaceUri.GetHashCode();
        }

        private void SetName(string local, string uri)
        {
            if (uri == null)
            {
                uri = string.Empty;
            }

            string[] names = (local ?? string.Empty).Split(':');
            if (names.Length == 1) // No colon found
            {
                this.Name = names[0];
                this.NamespaceUri = uri;
            }
            else
            {
                this.Name = names[1];
                this.NamespaceUri = KmlNamespaces.FindNamespace(names[0]) ?? uri;
                this.Prefix = names[0];
            }
        }
    }
}
