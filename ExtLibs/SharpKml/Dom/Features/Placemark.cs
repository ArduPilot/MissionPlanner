using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Represents a <see cref="Feature"/> that contains a <see cref="Geometry"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.11</para>
    /// <para>A Placemark with a <see cref="Point"/> geometry should be drawn
    /// with an icon to mark the Placemark in the geographic view. The point
    /// itself determines the position of the Placemark's name and display
    /// icon.</para>
    /// </remarks>
    [KmlElement("Placemark")]
    public sealed class Placemark : Feature
    {
        private Geometry _geometry;

        /// <summary>
        /// Gets or sets the associated <see cref="Geometry"/> of this instance.
        /// </summary>
        [KmlElement(null, 1)]
        public Geometry Geometry
        {
            get { return _geometry; }
            set { this.UpdatePropertyChild(value, ref _geometry); }
        }
    }
}
