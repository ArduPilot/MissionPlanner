using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the exterior boundary of a <see cref="Polygon"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.8.3.4</remarks>
    [KmlElement("outerBoundaryIs")]
    public sealed class OuterBoundary : Element
    {
        private LinearRing _ring;

        /// <summary>
        /// Gets or sets the <see cref="LinearRing"/> acting as the boundary.
        /// </summary>
        [KmlElement(null, 1)]
        public LinearRing LinearRing
        {
            get { return _ring; }
            set { this.UpdatePropertyChild(value, ref _ring); }
        }
    }
}
