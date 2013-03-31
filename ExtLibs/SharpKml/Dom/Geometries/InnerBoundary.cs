using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies an inner boundary of a <see cref="Polygon"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.8.3.5</remarks>
    [KmlElement("innerBoundaryIs")]
    public sealed class InnerBoundary : Element
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
