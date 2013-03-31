using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how to display an image draped over the terrain.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.2</remarks>
    [KmlElement("GroundOverlay")]
    public sealed class GroundOverlay : Overlay
    {
        private LatLonBox _box;

        /// <summary>
        /// Gets or sets the distance above the terrain in meters.
        /// </summary>
        /// <remarks>
        /// The value shall be interpreted according to <see cref="AltitudeMode"/>.
        /// Only <see cref="Dom.AltitudeMode.ClampToGround"/> or
        /// <see cref="Dom.AltitudeMode.Absolute"/> values are valid.
        /// </remarks>
        [KmlElement("altitude", 1)]
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets how <see cref="Altitude"/> should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 2)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>Gets or sets a bounding box for the overlay.</summary>
        [KmlElement(null, 3)]
        public LatLonBox Bounds
        {
            get { return _box; }
            set { this.UpdatePropertyChild(value, ref _box); }
        }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 4)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }
    }
}
