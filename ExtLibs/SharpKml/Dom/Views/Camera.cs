using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the position and orientation of a virtual camera.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 14.2</para>
    /// <para>Can be used to specify views of the earth or of objects in space.</para>
    /// </remarks>
    [KmlElement("Camera")]
    public sealed class Camera : AbstractView
    {
        /// <summary>
        /// Gets or sets the distance of the camera from the Earth's surface,
        /// in meters, interpreted according to the <see cref="AltitudeMode"/>.
        /// </summary>
        [KmlElement("altitude", 3)]
        public double? Altitude { get; set; }

        /// <summary>Gets or sets how the altitude value should be interpreted.</summary>
        [KmlElement("altitudeMode", 7)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the direction (azimuth) of the camera, in decimal degrees.
        /// </summary>
        /// <remarks>Values range from 0 (North) to 360 degrees.</remarks>
        [KmlElement("heading", 6)]
        public double? Heading { get; set; }

        /// <summary>
        /// Gets or sets the geodetic latitude of the virtual camera.
        /// </summary>
        /// <remarks>
        /// Value is in decimal degrees north or south of the Equator (0 degrees).
        /// Values range from −90 degrees to 90 degrees.
        /// </remarks>
        [KmlElement("latitude", 2)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the geodetic longitude of the virtual camera (eye point).
        /// </summary>
        /// <remarks>
        /// The angular distance is in decimal degrees, relative to the Prime
        /// Meridian. Values west of the Meridian range from −180 to 0 degrees.
        /// Values east of the Meridian range from 0 to 180 degrees.
        /// </remarks>
        [KmlElement("longitude", 1)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the rotation, in decimal degrees, of the camera around
        /// the Z axis.
        /// </summary>
        /// <remarks>
        /// Values range from −180 to +180 degrees.
        /// </remarks>
        [KmlElement("roll", 4)]
        public double? Roll { get; set; }

        /// <summary>
        /// Gets or sets the rotation, in decimal degrees, of the camera around
        /// the X axis.
        /// </summary>
        /// <remarks>
        /// A value of 0 indicates that the view is aimed straight down toward
        /// the earth (the most common case). A value of 90 indicates that the
        /// view is aimed towards the horizon. Values greater than 90 indicate
        /// that the view is pointed up into the sky. Values are clamped at
        /// +180 degrees.
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
