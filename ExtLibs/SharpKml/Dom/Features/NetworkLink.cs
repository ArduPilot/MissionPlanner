using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>References a KML resource on a local or remote network.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.12</para>
    /// <para>May be used in combination with <see cref="Region"/> elements to
    /// efficiently load and display large datasets.</para>
    /// </remarks>
    [KmlElement("NetworkLink")]
    public sealed class NetworkLink : Feature
    {
        private Link _link;

        /// <summary>
        /// Gets or sets whether to adjust the geographic view upon activation.
        /// </summary>
        /// <remarks>
        /// <para>A value of false indicates that the geographic view shall
        /// remain unchanged.</para>
        /// <para>A value of true indicates that the geographic view shall be
        /// displayed according to the <see cref="AbstractView"/> specified by
        /// either:
        /// <list type="bullet"><item>
        /// <description>a <see cref="NetworkLinkControl"/></description>
        /// </item><item>
        /// <description>a child <see cref="Feature"/> of <see cref="Kml"/></description>
        /// </item></list>
        /// if they exist in the referenced KML resource. The <c>AbstractView</c>
        /// of the <c>NetworkLinkControl</c> shall take precedence over the
        /// <c>AbstractView</c> of the <c>Feature</c> if they both exist. If
        /// neither exists then the view shall remain unchanged.</para>
        /// </remarks>
        [KmlElement("flyToView", 2)]
        public bool? FlyToView { get; set; }

        /// <summary>
        /// Gets or sets the location of the KML resource to be fetched.
        /// </summary>
        [KmlElement(null, 3)]
        public Link Link
        {
            get { return _link; }
            set { this.UpdatePropertyChild(value, ref _link); }
        }

        /// <summary>
        /// Gets or sets the visibility of any <see cref="Feature"/>s within
        /// the referenced KML resource.
        /// </summary>
        /// <remarks>
        /// <para>A value of false shall leave the visibility of any referenced
        /// <c>Features</c> in the geographic view within the control of the
        /// earth browser user.</para>
        /// <para>A value of true shall require any referenced <c>Features</c>
        /// to be visible within the geographic view whenever they are
        /// refreshed.</para>
        /// </remarks>
        [KmlElement("refreshVisibility", 1)]
        public bool? RefreshVisibility { get; set; }

        /// <summary>
        /// Converts depreciated <see cref="Url"/>s to <see cref="Link"/>s.
        /// </summary>
        /// <param name="orphan">
        /// The <see cref="Element"/> to store for serialization.
        /// </param>
        protected internal override void AddOrphan(Element orphan)
        {
#pragma warning disable 0618 // Url is obsolete, but it's ok because we're updating it
            Url url = orphan as Url;
#pragma warning restore 0618

            if (url != null)
            {
                this.Link = (Link)url; // Explicit conversion
            }
            else
            {
                base.AddOrphan(orphan);
            }
        }
    }
}
