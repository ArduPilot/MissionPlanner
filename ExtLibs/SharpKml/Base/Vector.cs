using System;

namespace SharpKml.Base
{
    /// <summary>
    /// Represents a point in either 2D or, if <see cref="Altitude"/> is set, 3D space.
    /// </summary>
    public sealed class Vector : IEquatable<Vector>
    {
        /// <summary>Initializes a new instance of the Vector class.</summary>
        public Vector()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Vector class with the specified values.
        /// </summary>
        /// <param name="latitude">The latitude for this instance.</param>
        /// <param name="longitude">The longitude for this instance.</param>
        public Vector(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the Vector class with the specified values.
        /// </summary>
        /// <param name="latitude">The latitude for this instance.</param>
        /// <param name="longitude">The longitude for this instance.</param>
        /// <param name="altitude">The altitude for this instance.</param>
        public Vector(double latitude, double longitude, double altitude)
        {
            this.Altitude = altitude;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets or sets the Altitude. Altitude can be null to indicate there
        /// is no altitude.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>Gets or sets the Latitude.</summary>
        public double Latitude { get; set; }

        /// <summary>Gets or sets the Longitude.</summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Determines whether this instance and the specified object have the
        /// same value.
        /// </summary>
        /// <param name="obj">
        /// An object, which must be a Vector, to compare to this instance.
        /// </param>
        /// <returns>
        /// true if the object is a Vector and the value of the object is the
        /// same as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Vector);
        }

        /// <summary>
        /// Determines whether this instance and the specified Vector have the
        /// same value.
        /// </summary>
        /// <param name="other">The Vector to compare to this instance.</param>
        /// <returns>
        /// true if the location of the value parameter is the same as this
        /// instance; otherwise, false.
        /// </returns>
        public bool Equals(Vector other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Altitude == other.Altitude) &&
                   (this.Latitude == other.Latitude) &&
                   (this.Longitude == other.Longitude);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int value = this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
            if (this.Altitude.HasValue)
            {
                value ^= this.Altitude.GetHashCode();
            }
            return value;
        }
    }
}
