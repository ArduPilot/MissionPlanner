using System;

namespace SharpKml.Base
{
    /// <summary>
    /// Specifies a class member is serialized as an XML attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KmlAttributeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the KmlAttributeAttribute class.
        /// </summary>
        /// <param name="attributeName">The name of the XML attribute.</param>
        public KmlAttributeAttribute(string attributeName)
        {
            this.AttributeName = attributeName;
        }

        /// <summary>Gets the name of the XML attribute.</summary>
        public string AttributeName { get; private set; }
    }
}
