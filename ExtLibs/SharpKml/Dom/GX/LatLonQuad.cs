using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Allows nonrectangular quadrilateral ground overlays.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("LatLonQuad", KmlNamespaces.GX22Namespace)]
    public sealed class LatLonQuad : KmlObject
    {
        private CoordinateCollection _coords;

        /// <summary>
        /// Gets or sets the four corner points of a quadrilateral defining the
        /// overlay area.
        /// </summary>
        /// <remarks>
        /// <para>Exactly four coordinate tuples have to be provided, specified
        /// in counter-clockwise order with the first coordinate corresponding
        /// to the lower-left corner of the overlayed image. The shape described
        /// by these corners must be convex.</para>
        /// <para>All altitude values are ignored.</para>
        /// </remarks>
        [KmlElement(null)]
        public CoordinateCollection Coordinates
        {
            get { return _coords; }
            set { this.UpdatePropertyChild(value, ref _coords); }
        }
    }
}
