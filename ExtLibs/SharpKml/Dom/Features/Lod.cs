using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the Level Of Detail to use when displaying a <see cref="Region"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.18</para>
    /// <para>An earth browser should calculate the size of the <see cref="Region"/>
    /// when projected onto screen space then compute the square root of the
    /// <c>Region</c>'s area. For example, if an untiled <c>Region</c> is square
    /// and the viewpoint is directly above it, this measurement is equal to the
    /// width of the projected <c>Region</c>. If this measurement falls within the
    /// limits defined by <see cref="MinimumPixels"/> and
    /// <see cref="MaximumPixels"/>, and if the <see cref="LatLonAltBox"/> is in
    /// view, then the <c>Region</c> should be activated. If this limit is not
    /// reached, the associated geometry should not be drawn since it would be
    /// too far from the user's viewpoint to be visible.</para>
    /// <para>See the standard for further details of how to handle
    /// <see cref="MinimumFadeExtent"/> and <see cref="MaximumFadeExtent"/>.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assume P is the calculated projected pixel size:
    /// if (P &lt; MinimumPixels)
    ///     opacity = 0; // Not visible
    /// else if (P &lt; (MinimumPixels + MinimumFadeExtent))
    ///     opacity = (P - MinimumPixels) / MinimumFadeExtent; // Partially visible
    /// else if (P &lt; (MaximumPixels - MaximumFadeExtent))
    ///     opacity = 1; // Fully visible
    /// else if (P &lt; MaximumPixels)
    ///     opacity = (MaximumPixels - P) / MaximumFadeExtent // Partially visible
    /// else // P &gt; MaximumPixels
    ///     opacity = 0 // Not visible
    /// </code></example>
    [KmlElement("Lod")]
    public sealed class Lod : KmlObject
    {
        /// <summary>The default value that should be used for <see cref="MaximumPixels"/>.</summary>
        public const double DefaultMaximumPixels = -1.0;

        /// <summary>
        /// Gets or sets the distance over which the geometry fades, from fully
        /// transparent to fully opaque.
        /// </summary>
        /// <remarks>
        /// This ramp value, expressed in pixels, is applied at the maximum end
        /// of the visibility limits.
        /// </remarks>
        [KmlElement("maxFadeExtent", 4)]
        public double? MaximumFadeExtent { get; set; }

        /// <summary>
        /// Gets or sets a measurement (in pixels) that represents the maximum
        /// limit of the visibility range.
        /// </summary>
        /// <remarks>A value of -1.0 indicates "active to infinite size."</remarks>
        [KmlElement("maxLodPixels", 2)]
        public double? MaximumPixels { get; set; }

        /// <summary>
        /// Gets or sets the distance over which the geometry fades, from fully
        /// opaque to fully transparent.
        /// </summary>
        /// <remarks>
        /// This ramp value, expressed in pixels, is applied at the minimum end
        /// of the visibility limits.
        /// </remarks>
        [KmlElement("minFadeExtent", 3)]
        public double? MinimumFadeExtent { get; set; }

        /// <summary>
        /// Gets or sets a measurement (in pixels) that represents the minimum
        /// limit of the visibility range.
        /// </summary>
        [KmlElement("minLodPixels", 1)]
        public double? MinimumPixels { get; set; }
    }
}
