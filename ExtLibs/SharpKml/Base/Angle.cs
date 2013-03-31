using System;

namespace SharpKml.Base
{
    /// <summary>
    /// Represents the heading, tilt, and roll angles to the icon or model in
    /// a gx:Track.
    /// </summary>
    public sealed class Angle : IEquatable<Angle>
    {
        /// <summary>Initializes a new instance of the Angle class.</summary>
        public Angle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Angle class with the specified values.
        /// </summary>
        /// <param name="pitch">The latitude for this instance.</param>
        /// <param name="heading">The longitude for this instance.</param>
        /// <param name="roll">The altitude for this instance.</param>
        public Angle(double pitch, double heading, double roll)
        {
            this.Heading = heading;
            this.Pitch = pitch;
            this.Roll = roll;
        }

        /// <summary>Gets or sets the heading.</summary>
        public double Heading { get; set; }

        /// <summary>Gets or sets the pitch.</summary>
        public double Pitch { get; set; }

        /// <summary>Gets or sets the roll.</summary>
        public double Roll { get; set; }

        /// <summary>
        /// Determines whether this instance and the specified object have the
        /// same value.
        /// </summary>
        /// <param name="obj">
        /// An object, which must be an Angle, to compare to this instance.
        /// </param>
        /// <returns>
        /// true if the object is an Angle and the value of the object is the
        /// same as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Vector);
        }

        /// <summary>
        /// Determines whether this instance and the specified Angle have the
        /// same value.
        /// </summary>
        /// <param name="other">The Angle to compare to this instance.</param>
        /// <returns>
        /// true if the angles of the value parameter are the same as this
        /// instance; otherwise, false.
        /// </returns>
        public bool Equals(Angle other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Roll == other.Roll) &&
                   (this.Pitch == other.Pitch) &&
                   (this.Heading == other.Heading);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (this.Pitch.GetHashCode() * 17) +
                   (this.Heading.GetHashCode() * 23) +
                    (this.Roll.GetHashCode() * 23);
        }
    }
}
