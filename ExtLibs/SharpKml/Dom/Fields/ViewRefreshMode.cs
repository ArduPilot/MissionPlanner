using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how the link is refreshed when the geographic view changes.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.22</remarks>
    public enum ViewRefreshMode
    {
        /// <summary>Ignore changes in the geographic view.</summary>
        /// <remarks>
        /// Also ignore <see cref="LinkType.ViewFormat"/> values, if any.
        /// </remarks>
        [KmlElement("never")]
        Never = 0,

        /// <summary>
        /// Refresh the resource only when the user explicitly requests it.
        /// </summary>
        [KmlElement("onRequest")]
        OnRequest,

        /// <summary>
        /// Refresh the resource <see cref="LinkType.ViewRefreshTime"/> seconds
        /// after movement stops.
        /// </summary>
        [KmlElement("onStop")]
        OnStop,

        /// <summary>
        /// Refresh the resource if a <see cref="Region"/> becomes active.
        /// </summary>
        [KmlElement("onRegion")]
        OnRegion
    }
}
