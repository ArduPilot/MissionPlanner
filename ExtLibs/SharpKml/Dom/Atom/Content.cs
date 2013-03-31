using System;
using SharpKml.Base;

namespace SharpKml.Dom.Atom
{
    /// <summary>
    /// Represents information about or links to the content of an entry.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.1.3 (see http://atompub.org/rfc4287.html)
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("content", KmlNamespaces.AtomNamespace)]
    public sealed class Content : Element
    {
        /// <summary>Gets or sets the MIME media type of the content.</summary>
        /// <remarks>
        /// The value of may be one of "text", "html", or "xhtml". Failing that,
        /// it must conform to the syntax of a MIME media type, but must not be
        /// a composite type [RFC 4288].
        /// </remarks>
        [KmlAttribute("type")]
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets an URI reference to retrieve the content from.
        /// </summary>
        /// <remarks>
        /// Remote content is allowed to be ignored or presented in a different
        /// manner than local content.
        /// </remarks>
        [KmlAttribute("src")]
        public Uri Source { get; set; }
    }
}
