using System;
using SharpKml.Base;

namespace SharpKml.Dom.Atom
{
    /// <summary>
    /// Defines a reference from an entry or feed to a Web resource.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.2.7 (see http://atompub.org/rfc4287.html)
    /// </remarks>
    [KmlElement("link", KmlNamespaces.AtomNamespace)]
    public sealed class Link : Element
    {
        /// <summary>Gets or sets the Link's URI.</summary>
        [KmlAttribute("href")]
        public Uri Href { get; set; }

        /// <summary>
        /// Gets or sets the language of the resource pointed to by <see cref="Href"/>.
        /// </summary>
        /// <remarks>
        /// Value must be a language tag [RFC 3066]. When used together with the
        /// <c>Relation="alternate"</c>, it implies a translated version of the entry.
        /// </remarks>
        [KmlAttribute("hreflang")]
        public string HrefLang { get; set; }

        /// <summary>
        /// Gets or sets an advisory length of the linked content in octets.
        /// </summary>
        /// <remarks>
        /// This is a hint about the content length of the representation returned
        /// when the value of <see cref="Href"/> is dereferenced. Note that Length
        /// does not override the actual content length of the representation as
        /// reported by the underlying protocol.
        /// </remarks>
        [KmlAttribute("length")]
        public int? Length { get; set; }

        /// <summary>Gets or sets an advisory media type.</summary>
        /// <remarks>
        /// The value must conform to the syntax of a MIME media type [RFC 4288].
        /// This is a hint about the type of the representation that is expected to
        /// be returned when the value of Href is dereferenced. Note that MediaType
        /// does not override the actual media type returned with the representation.
        /// </remarks>
        [KmlAttribute("type")]
        public string MediaType { get; set; }

        /// <summary>Gets or sets the Link's relation type.</summary>
        /// <remarks>
        /// If the value is not present, the Link must be interpreted as if
        /// Relation is "alternate". There are five initial values for the
        /// Registry of Link Relations:
        /// <list type="number">
        /// <item>
        /// <term>alternate</term>
        /// <description>
        /// Signifies that the value in Href identifies an alternate version of
        /// the resource described by the containing element.
        /// </description>
        /// </item><item>
        /// <term>related</term>
        /// <description>
        /// Signifies that the value in Href identifies a resource related to the
        /// resource described by the containing element.
        /// </description>
        /// </item><item>
        /// <term>self</term>
        /// <description>
        /// Signifies that value in Href identifies a resource equivalent to the
        /// containing element.
        /// </description>
        /// </item><item>
        /// <term>enclosure</term>
        /// <description>
        /// Signifies that the value in Href identifies a related resource that
        /// is potentially large in size and might require special handling.
        /// </description>
        /// </item><item>
        /// <term>via</term>
        /// <description>
        /// Signifies that the value in Href identifies a resource that is the
        /// source of the information provided in the containing element.
        /// </description>
        /// </item></list>
        /// </remarks>
        /// <example>
        /// The feed for a site that discusses the performance of the search
        /// engine at "http://search.example.com" might contain, as a child
        /// of atom:feed:
        /// <code>&lt;link rel="related" href="http://search.example.com/"/&gt;</code>
        /// An identical link might appear as a child of any atom:entry whose
        /// content contains a discussion of that same search engine.
        /// </example>
        [KmlAttribute("rel")]
        public string Relation { get; set; }

        /// <summary>Gets or sets human-readable information about the Link.</summary>
        /// <remarks>The content is Language-Sensitive.</remarks>
        [KmlAttribute("title")]
        public string Title { get; set; }
    }
}
