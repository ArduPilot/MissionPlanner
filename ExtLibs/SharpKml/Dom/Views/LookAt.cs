using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the geographic view in terms of a point of interest viewed
    /// from a virtual camera.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 14.3</para>
    /// <para>The LookAt class is more limited in scope than <see cref="Camera"/>
    /// and should establish a view direction that intersects the Earth's surface.
    /// </para>
    /// </remarks>
    [KmlElement("LookAt")]
    public sealed class LookAt : AbstractView
    {
        /// <summary>
        /// Gets or sets the altitude in meters, interpreted according to
        /// <see cref="AltitudeMode"/>.
        /// </summary>
        [KmlElement("altitude", 3)]
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets how <see cref="Altitude"/> should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 7)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the direction (North, South, East, West), in decimal degrees.
        /// </summary>
        /// <remarks>Values range from 0 (North) to 360 degrees.</remarks>
        [KmlElement("heading", 4)]
        public double? Heading { get; set; }

        /// <summary>
        /// Gets or sets the geodetic latitude of the point the camera is looking at.
        /// </summary>
        /// <remarks>
        /// Value is in decimal degrees north or south of the Equator (0 degrees).
        /// Values range from −90 degrees to 90 degrees.
        /// </remarks>
        [KmlElement("latitude", 2)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the geodetic longitude of the point the camera is looking at.
        /// </summary>
        /// <remarks>
        /// The angular distance is in decimal degrees, relative to the Prime
        /// Meridian. Values west of the Meridian range from −180 to 0 degrees.
        /// Values east of the Meridian range from 0 to 180 degrees.
        /// </remarks>
        [KmlElement("longitude", 1)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the distance in meters from the point specified by
        /// <see cref="Longitude"/>, <see cref="Latitude"/> and <see cref="Altitude"/>
        /// to the LookAt position.
        /// </summary>
        [KmlElement("range", 6)]
        public double? Range { get; set; }

        /// <summary>
        /// Gets or sets the angle, in decimal degrees, between the direction of
        /// the LookAt position and the normal to the surface of the Earth.
        /// </summary>
        /// <remarks>
        /// Values range from 0 to 90 degrees and cannot be negative. A value
        /// of 0 degrees indicates viewing from directly above; a value of 90
        /// degrees indicates viewing along the horizon.
        /// </remarks>
        [KmlElement("tilt", 5)]
        public double? Tilt { get; set; }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 8)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }
    }
}
