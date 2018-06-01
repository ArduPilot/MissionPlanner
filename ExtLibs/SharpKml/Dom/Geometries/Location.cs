using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the coordinates of a location.</summary>
    /// <remarks>OGC KML 2.2 Section 10.10</remarks>
    [KmlElement("Location")]
    public sealed class Location : KmlObject
    {
        /// <summary>
        /// Gets or sets the altitude of origin measured in meters.
        /// </summary>
        [KmlElement("altitude", 3)]
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the geodetic latitude of origin in decimal degrees.
        /// </summary>
        [KmlElement("latitude", 2)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the geodetic longitude of origin in decimal degrees.
        /// </summary>
        [KmlElement("longitude", 1)]
        public double? Longitude { get; set; }
    }
}
