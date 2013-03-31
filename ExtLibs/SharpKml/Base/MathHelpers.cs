using System;

namespace SharpKml.Base
{
    /// <summary>
    /// Contains mathematical functions useful when working with KML or, more
    /// generally, geometry on a Great Circle.
    /// </summary>
    /// <remarks>
    /// Many of the formulae here were copied from the Aviation Formulary
    /// <see href="http://williams.best.vwh.net/avform.htm"/>.
    /// </remarks>
    public static class MathHelpers
    {
        // Equatorial = 6378137, polar = 6356752.
        private const int EarthRadius = 6366710; // The mean radius of the Earth in meters.

        /// <summary>
        /// Calculates the initial azimuth (the angle measured clockwise from
        /// true north) at a point from that point to a second point.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>
        /// The initial azimuth of the first point to the second point.
        /// </returns>
        /// <remarks>
        /// This is directly useful as the value of heading in
        /// <see cref="Dom.AbstractView"/>.
        /// </remarks>
        /// <example>
        /// The azimuth from 0,0 to 1,0 is 0 degrees. From 0,0 to 0,1 is 90
        /// degrees (due east). The range of the result is [-180, 180].
        /// </example>
        public static double Azimuth(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            latitude1 = DegreesToRadians(latitude1);
            longitude1 = DegreesToRadians(longitude1);
            latitude2 = DegreesToRadians(latitude2);
            longitude2 = DegreesToRadians(longitude2);

            double x = (Math.Cos(latitude1) * Math.Sin(latitude2)) - (Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitude2 - longitude1));
            double tan = Math.Atan2(Math.Sin(longitude2 - longitude1) * Math.Cos(latitude2), x);
            return RadiansToDegrees(tan % (2 * Math.PI));
        }

        /// <summary>Converts the specified value in degrees to radians.</summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The specified angle converted to radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points.
        /// </summary>
        /// <param name="point1">The location of the first point.</param>
        /// <param name="point2">The location of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        /// <exception cref="ArgumentNullException">point1 or point2 is null.</exception>
        public static double Distance(Vector point1, Vector point2)
        {
            if (point1 == null)
            {
                throw new ArgumentNullException("point1");
            }
            if (point2 == null)
            {
                throw new ArgumentNullException("point2");
            }

            if ((point1.Altitude == null) || (point2.Altitude == null))
            {
                return Distance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);
            }
            return Distance(point1.Latitude, point1.Longitude, point1.Altitude.Value, point2.Latitude, point2.Longitude, point2.Altitude.Value);
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points on
        /// the Earth's surface.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            latitude1 = DegreesToRadians(latitude1);
            longitude1 = DegreesToRadians(longitude1);
            latitude2 = DegreesToRadians(latitude2);
            longitude2 = DegreesToRadians(longitude2);

            double latitudeSqrd = Math.Pow(Math.Sin((latitude1 - latitude2) / 2), 2);
            double longitudeSqrd = Math.Pow(Math.Sin((longitude1 - longitude2) / 2), 2);
            double sqrt = Math.Sqrt(latitudeSqrd + (Math.Cos(latitude1) * Math.Cos(latitude2) * longitudeSqrd));
            return RadiansToMeters(2 * Math.Asin(sqrt));
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points in
        /// all three dimensions.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(double latitude1, double longitude1, double altitude1, double latitude2, double longitude2, double altitude2)
        {
            double surfaceDistance = Distance(latitude1, longitude1, latitude2, longitude2);
            return Math.Sqrt(Math.Pow(surfaceDistance, 2) + Math.Pow(altitude2 - altitude1, 2));
        }

        /// <summary>
        /// Calculates the angle from the horizontal plane between the two altitudes.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The angle from the horizontal plane in degrees.</returns>
        /// <remarks>
        /// This is a naive implementation accurate only over short distances
        /// and does not account for surface curvature. To use this as the value
        /// of KML's tilt, add 90 degrees (since in KML a tilt of 0 is vertical).
        /// </remarks>
        /// <example>
        /// The returned angle from (37.00, -121.98, 600) to a point about 1778
        /// meters west, 400 meters below at (37.00, -122.00, 200) is -12.7 degrees.
        /// </example>
        public static double Elevation(double latitude1, double longitude1, double altitude1, double latitude2, double longitude2, double altitude2)
        {
            double surfaceDistance = Distance(latitude1, longitude1, latitude2, longitude2);
            return RadiansToDegrees(Math.Atan2(altitude2 - altitude1, surfaceDistance));
        }

        /// <summary>
        /// Calculates the absolute distance between the ground point and the
        /// point directly under the end of the specified vector.
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute ground distance in meters.</returns>
        public static double GroundDistance(double range, double elevation)
        {
            return Math.Abs(Math.Cos(DegreesToRadians(elevation)) * range);
        }

        /// <summary>
        /// Calculates the absolute height between the ground point and the
        /// point directly under the end of the specified vector.
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute height in meters.</returns>
        public static double Height(double range, double elevation)
        {
            return Math.Abs(Math.Sin(DegreesToRadians(elevation)) * range);
        }

        /// <summary>Converts the specified distance in meters to radians.</summary>
        /// <param name="meters">The distance in meters.</param>
        /// <returns>The specified distance converted to radians.</returns>
        public static double MetersToRadians(double meters)
        {
            return meters / EarthRadius;
        }

        /// <summary>
        /// Calculates a point at the specified distance along a radial from a
        /// center point.
        /// </summary>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="radial">
        /// The radial in degrees, measures clockwise from north.
        /// </param>
        /// <returns>
        /// A <see cref="Vector"/> containing the Latitude and Longitude of the
        /// calculated point.
        /// </returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static Vector RadialPoint(double latitude, double longitude, double distance, double radial)
        {
            latitude = DegreesToRadians(latitude);
            longitude = DegreesToRadians(longitude);
            distance = MetersToRadians(distance);
            radial = DegreesToRadians(radial);

            double latitudeDistance = Math.Cos(latitude) * Math.Sin(distance);
            double radialLat = Math.Asin((Math.Sin(latitude) * Math.Cos(distance)) + (latitudeDistance * Math.Cos(radial)));

            double y = Math.Sin(radial) * latitudeDistance;
            double x = Math.Cos(distance) - (Math.Sin(latitude) * Math.Sin(radialLat));
            double deltaLon = Math.Atan2(y, x);

            double radialLon = ((longitude + deltaLon + Math.PI) % (2 * Math.PI)) - Math.PI;
            return new Vector(RadiansToDegrees(radialLat), RadiansToDegrees(radialLon));
        }

        /// <summary>Converts the specified value in radians to degrees.</summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The specified angle converted to degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>Converts the specified angle in radians to meters.</summary>
        /// <param name="radians">The angle in radians</param>
        /// <returns>The specified angle converted to meters.</returns>
        public static double RadiansToMeters(double radians)
        {
            return radians * EarthRadius;
        }
    }
}
