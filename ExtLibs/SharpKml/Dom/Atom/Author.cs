using System;
using SharpKml.Base;

namespace SharpKml.Dom.Atom
{
    /// <summary>
    /// Represents a Person construct that indicates the author of the entry or feed.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.2.1 (see http://atompub.org/rfc4287.html)
    /// </remarks>
    [KmlElement("author", KmlNamespaces.AtomNamespace)]
    public sealed class Author : Element
    {
        /// <summary>
        /// Gets or sets the e-mail address associated with the person.
        /// </summary>
        /// <remarks>
        /// The content must conform to the "addr-spec" production in [RFC 2822].
        /// </remarks>
        [KmlElement("email", KmlNamespaces.AtomNamespace)]
        public string Email { get; set; }

        /// <summary>Gets or sets the human-readable name for the person.</summary>
        /// <remarks>The content of is Language-Sensitive.</remarks>
        [KmlElement("name", KmlNamespaces.AtomNamespace)]
        public string Name { get; set; }

        /// <summary>Gets or sets the URI associated with the person.</summary>
        [KmlElement("uri", KmlNamespaces.AtomNamespace)]
        public Uri Uri { get; set; }
    }
}
