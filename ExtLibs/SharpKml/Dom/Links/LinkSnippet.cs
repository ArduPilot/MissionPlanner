using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a short description for a <see cref="NetworkLinkControl"/>.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Sections 13.2.3.7 and 16.19</para>
    /// <para>LinkSnippet content shall take precedence over the
    /// <see cref="Feature.Snippet"/> value.</para>
    /// </remarks>
    [KmlElement("linkSnippet")]
    public sealed class LinkSnippet : Element, IHtmlContent
    {
        /// <summary>The default value that should be used for <see cref="MaximumLines"/>.</summary>
        public const int DefaultMaximumLines = 2;

        /// <summary>Gets or sets the maximum number of lines to display.</summary>
        [KmlAttribute("maxLines")]
        public int MaximumLines { get; set; }

        /// <summary>Gets or sets the content of this instance.</summary>
        /// <remarks>The value may contain well formed HTML.</remarks>
        public string Text
        {
            get
            {
                return this.InnerText;
            }
            set
            {
                this.ClearInnerText();
                this.AddInnerText(value);
            }
        }
    }
}
