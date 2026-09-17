using System;
using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Endpoint behaviour for a spline segment.
    /// Stop    : vehicle conceptually stops at this end (tangent small, toward the segment).
    /// Straight: straight-through; tangent aligned with incoming/outgoing straight leg.
    /// Spline  : spline-through; tangent aligned using both neighbours.
    /// </summary>
    public enum SplineEndpointType
    {
        Stop,
        Straight,
        Spline
    }

    /// <summary>
    /// Spline3: build a cubic Hermite spline segment between two points (origin -> dest),
    /// using up to four waypoints (prev, origin, dest, next) supplied as PointLatLngAlt.
    ///
    /// Geometry is computed in a local frame centered at 'origin' and returned as
    /// a discretised list of PointLatLngAlt along the path between origin and dest.
    ///
    /// If 'prev' or 'next' are null, the corresponding endpoint velocity falls back
    /// to a Stop-like behaviour.
    /// </summary>
    public class Spline3
    {
        const double Deg2Rad = Math.PI / 180.0;
        const double LATLON_TO_M = 0.01113195;

        readonly Vector3 c0;
        readonly Vector3 c1;
        readonly Vector3 c2;
        readonly Vector3 c3;
        readonly PointLatLngAlt origin;

        public Spline3(
            PointLatLngAlt wpBefore,
            PointLatLngAlt wpStart,
            PointLatLngAlt wpEnd,
            PointLatLngAlt wpAfter,
            SplineEndpointType startType,
            SplineEndpointType endType)
        {
            if (wpStart == null || wpEnd == null)
            {
                throw new ArgumentNullException("Origin and destination points must be non-null.");
            }

            // The math works in cartesian coordinates, so we turn these all into
            // position vectors relative to wpStart.
            origin = wpStart;
            Vector3 startLocal = Vector3.Zero;
            Vector3 endLocal = ToLocalVector(wpEnd);
            // If either neighbour is missing, the corresponding endpoint should be a Stop.
            // The caller should be doing that, but we'll enforce it here.
            Vector3 beforeLocal = startLocal;
            if (wpBefore != null)
            {
                beforeLocal = ToLocalVector(wpBefore);
            }
            else
            {
                startType = SplineEndpointType.Stop;
            }
            Vector3 afterLocal = endLocal;
            if (wpAfter != null)
            {
                afterLocal = ToLocalVector(wpAfter);
            }
            else
            {
                endType = SplineEndpointType.Stop;
            }

            Vector3 startTangent = new Vector3();
            Vector3 endTangent = new Vector3();

            switch (startType)
            {
                case SplineEndpointType.Stop:
                    startTangent = (endLocal - startLocal) * 0.1;
                    break;

                case SplineEndpointType.Straight:
                    startTangent = (startLocal - beforeLocal);
                    break;

                case SplineEndpointType.Spline:
                    startTangent = (endLocal - beforeLocal);
                    break;
            }

            switch (endType)
            {
                case SplineEndpointType.Stop:
                    endTangent = (endLocal - startLocal) * 0.1;
                    break;

                case SplineEndpointType.Straight:
                    endTangent = (afterLocal - endLocal);
                    break;

                case SplineEndpointType.Spline:
                    endTangent = (afterLocal - startLocal);
                    break;
            }

            // Clamp excessive tangents similar to Spline2
            double tangentMagnitude = (startTangent + endTangent).length();
            double spanMagnitude = (endLocal - startLocal).length();
            if (tangentMagnitude > 1e-6)
            {
                double scaleFactor = 4 * spanMagnitude / tangentMagnitude;
                scaleFactor = Math.Min(scaleFactor, 1.0); // only scale down
                startTangent *= scaleFactor;
                endTangent *= scaleFactor;
            }

            // Hermite coefficients
            c0 = startLocal;
            c1 = startTangent;
            c2 = (-3 * startLocal) - (2 * startTangent) + (3 * endLocal) - endTangent;
            c3 = (2 * startLocal) + startTangent - (2 * endLocal) + endTangent;
        }
        
        /// <summary>
        /// Discretises the spline into <paramref name="samples"/> + 1 points
        /// evenly spaced in parameter space from t = 0 (origin) to t = 1 (dest).
        /// </summary>
        public List<PointLatLngAlt> BuildPath(int samples = 40)
        {

            var result = new List<PointLatLngAlt>(samples + 1);

            for (int i = 0; i <= samples; i++)
            {
                result.Add(GetPointAt((double)i / samples));
            }

            return result;
        }

        /// <summary>
        /// Evaluates the spline at parameter <paramref name="t"/> (0 = origin, 1 = dest).
        /// </summary>
        public PointLatLngAlt GetPointAt(double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            Vector3 local =
                c0 +
                c1 * t +
                c2 * t2 +
                c3 * t3;

            return FromLocalVector(local);
        }

        /// <summary>
        /// Convert a PointLatLngAlt into a local Vector3 (m, m, m) in a frame
        /// centered at origin.
        /// </summary>
        Vector3 ToLocalVector(PointLatLngAlt pt)
        {
            if (pt == null || origin == null)
                return new Vector3();

            double lat0 = origin.Lat;
            double lon0 = origin.Lng;
            double alt0 = origin.Alt;

            double scaleLongDown = Math.Cos(lat0 * Deg2Rad);

            double dx = (pt.Lat - lat0) * 1.0e7 * LATLON_TO_M;
            double dy = (pt.Lng - lon0) * 1.0e7 * LATLON_TO_M * scaleLongDown;
            double dz = (pt.Alt - alt0);

            return new Vector3(dx, dy, dz);
        }

        /// <summary>
        /// Convert a local Vector3 (m, m, m) back into PointLatLngAlt using
        /// origin as the reference.
        /// </summary>
        PointLatLngAlt FromLocalVector(Vector3 local)
        {
            double lat0 = origin.Lat;
            double lon0 = origin.Lng;
            double alt0 = origin.Alt;

            double scaleLongDown = Math.Cos(lat0 * Deg2Rad);

            double lat = lat0 + local.x / (1.0e7 * LATLON_TO_M);
            double lon = lon0 + local.y / (1.0e7 * LATLON_TO_M * scaleLongDown);
            double alt = alt0 + local.z;

            return new PointLatLngAlt(lat, lon, alt, "");
        }
    }
}
