using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the extent of a 3D bounding box.</summary>
    /// <remarks>OGC KML 2.2 Section 9.15</remarks>
    [KmlElement("LatLonAltBox")]
    public sealed class LatLonAltBox : AbstractLatLonBox
    {
        /// <summary>
        /// Gets or sets how <see cref="MinimumAltitude"/> and
        /// <see cref="MaximumAltitude"/> should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 3)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the maximum altitude in meters above the vertical datum.
        /// </summary>
        /// <remarks>
        /// This property is affected by <see cref="AltitudeMode"/> and must
        /// be greater than or equal to <see cref="MinimumAltitude"/>. If both
        /// <c>MinimumAltitude</c> and <c>MaximumAltitude</c> are specified,
        /// <c>AltitudeMode</c> shall not have a value of
        /// <see cref="Dom.AltitudeMode.ClampToGround"/>.
        /// </remarks>
        [KmlElement("maxAltitude", 2)]
        public double? MaximumAltitude { get; set; }

        /// <summary>
        /// Gets or sets the minimum altitude in meters above the vertical datum.
        /// </summary>
        /// <remarks>
        /// This property is affected by <see cref="AltitudeMode"/> and must
        /// be less than or equal to <see cref="MaximumAltitude"/>. If both
        /// <c>MinimumAltitude</c> and <c>MaximumAltitude</c> are specified,
        /// <c>AltitudeMode</c> shall not have a value of
        /// <see cref="Dom.AltitudeMode.ClampToGround"/>.
        /// </remarks>
        [KmlElement("minAltitude", 1)]
        public double? MinimumAltitude { get; set; }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 4)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }
    }
}
