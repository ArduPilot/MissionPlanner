using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a time-based refresh mode.</summary>
    /// <remarks>OGC KML 2.2 Section 16.16</remarks>
    public enum RefreshMode
    {
        /// <summary>
        /// Refresh when the resource is first loaded and whenever the
        /// <see cref="Link"/> parameters change.
        /// </summary>
        [KmlElement("onChange")]
        OnChange = 0,

        /// <summary>
        /// Refresh the resource as specified in
        /// <see cref="LinkType.RefreshInterval"/>.
        /// </summary>
        [KmlElement("onInterval")]
        OnInterval,

        /// <summary>
        /// Refresh the resource when the expiration time is reached.
        /// </summary>
        [KmlElement("onExpire")]
        OnExpire
    }
}
