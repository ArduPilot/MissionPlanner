using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Allows the tour to be paused until a user takes action to continue the tour.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("TourControl", KmlNamespaces.GX22Namespace)]
    public sealed class TourControl : TourPrimitive
    {
        /// <summary>Gets or sets whether to pause the tour.</summary>
        [KmlElement("playMode", KmlNamespaces.GX22Namespace)]
        public PlayMode? Mode { get; set; }
    }
}
