using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents a KML AbstractLatLonAltBox.</summary>
    /// <remarks>OGC KML 2.2 Section 9.14</remarks>
    public abstract class AbstractLatLonBox : KmlObject
    {
        /// <summary>The default value that should be used for <see cref="East"/>.</summary>
        public const double DefaultEast = 180.0;

        /// <summary>The default value that should be used for <see cref="North"/>.</summary>
        public const double DefaultNorth = 180.0;

        /// <summary>The default value that should be used for <see cref="South"/>.</summary>
        public const double DefaultSouth = -180.0;

        /// <summary>The default value that should be used for <see cref="West"/>.</summary>
        public const double DefaultWest = -180.0;

        /// <summary>
        /// Gets or sets the longitude of the east edge of the bounding box,
        /// in decimal degrees from 0 to ±180.
        /// </summary>
        /// <remarks>
        /// For overlays that overlap the meridian of 180° longitude,
        /// values can extend beyond that range.
        /// </remarks>
        [KmlElement("east", 3)]
        public double? East { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the north edge of the bounding box,
        /// in decimal degrees from 0 to ±90.
        /// </summary>
        [KmlElement("north", 1)]
        public double? North { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the south edge of the bounding box,
        /// in decimal degrees from 0 to ±90.
        /// </summary>
        [KmlElement("south", 2)]
        public double? South { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the west edge of the bounding box,
        /// in decimal degrees from 0 to ±180.
        /// </summary>
        /// <remarks>
        /// For overlays that overlap the meridian of 180° longitude,
        /// values can extend beyond that range.
        /// </remarks>
        [KmlElement("west", 4)]
        public double? West { get; set; }
    }
}
