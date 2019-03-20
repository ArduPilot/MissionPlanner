/*
 * http://svn.osgeo.org/geotools/tags/2.6.2/modules/library/referencing/src/main/java/org/geotools/referencing/operation/projection/Polyconic.java
 * http://svn.osgeo.org/geotools/tags/2.6.2/modules/library/referencing/src/main/java/org/geotools/referencing/operation/projection/MapProjection.java
 */
using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// 
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    internal class PolyconicProjection : MapProjection
    {
        /// <summary>
        /// Maximum difference allowed when comparing real numbers.
        /// </summary>
        private const double Epsilon = 1E-10;

        /// <summary>
        /// Maximum number of iterations for iterative computations.
        /// </summary>
        private const int MaximumIterations = 20;

        /// <summary>
        /// Difference allowed in iterative computations.
        /// </summary>
        private const double IterationTolerance = 1E-12;

        ///<summary>
        /// Meridian distance at the latitude of origin.
        /// Used for calculations for the ellipsoid.
        /// </summary>
        private readonly double _ml0;

        ///<summary>
        /// Constructs a new map projection from the supplied parameters.
        ///</summary>
        /// <param name="parameters">The parameter values in standard units</param>
        public PolyconicProjection(IEnumerable<ProjectionParameter> parameters)
            : this(parameters, null)
        { }

        /// <summary>
        /// Constructs a new map projection from the supplied parameters.
        /// </summary>
        /// <param name="parameters">The parameter values in standard units</param>
        /// <param name="inverse">Defines if Projection is inverse</param>
        protected PolyconicProjection(IEnumerable<ProjectionParameter> parameters, PolyconicProjection inverse)
            : base(parameters, inverse)
        {
            _ml0 = mlfn(lat_origin, Math.Sin(lat_origin), Math.Cos(lat_origin));
        }

        protected override double[] RadiansToMeters(double[] lonlat)
        {

            var lam = lonlat[0];
            var phi = lonlat[1];

            var delta_lam = adjust_lon(lam - central_meridian);

            double x, y;

            if (Math.Abs(phi) <= Epsilon)
            {
                x = delta_lam; //lam;
                y = -_ml0;
            }
            else
            {
                var sp = Math.Sin(phi);
                double cp;
                var ms = Math.Abs(cp = Math.Cos(phi)) > Epsilon ? msfn(sp, cp) / sp : 0.0;
                /*lam =*/ delta_lam *= sp;
                x = ms * Math.Sin(/*lam*/delta_lam);
                y = (mlfn(phi, sp, cp) - _ml0) + ms * (1.0 - Math.Cos(/*lam*/delta_lam));
            }

            x = scale_factor*_semiMajor*x; // + false_easting;
            y = scale_factor*_semiMajor*y;// +false_northing;

            return new[] { x, y };
        }

        protected override double[] MetersToRadians(double[] p)
        {
            
            var x = (p[0]) / (_semiMajor * scale_factor);
            var y = (p[1]) / (_semiMajor * scale_factor);

            double lam, phi;

            y += _ml0;
            if (Math.Abs(y) <= Epsilon)
            {
                lam = x;
                phi = 0.0;
            }
            else
            {
                var r = y * y + x * x;
                phi = y;
                var i = 0;
                for (; i <= MaximumIterations; i++)
                {
                    var sp = Math.Sin(phi);
                    var cp = Math.Cos(phi);
                    if (Math.Abs(cp) < IterationTolerance)
                        throw new Exception("No Convergence");

                    var s2ph = sp * cp;
                    var mlp = Math.Sqrt(1.0 - _es * sp * sp);
                    var c = sp * mlp / cp;
                    var ml = mlfn(phi, sp, cp);
                    var mlb = ml * ml + r;
                    mlp = (1.0 - _es) / (mlp * mlp * mlp);
                    var dPhi = (ml + ml + c * mlb - 2.0 * y * (c * ml + 1.0)) / (
                                _es * s2ph * (mlb - 2.0 * y * ml) / c +
                                2.0 * (y - ml) * (c * mlp - 1.0 / s2ph) - mlp - mlp);
                    if (Math.Abs(dPhi) <= IterationTolerance)
                        break;

                    phi += dPhi;
                }
                if (i > MaximumIterations)
                    throw new Exception("No Convergence");
                var c2 = Math.Sin(phi);
                lam = Math.Asin(x * Math.Tan(phi) * Math.Sqrt(1.0 - _es * c2 * c2)) / Math.Sin(phi);
            }

            return new[] { adjust_lon(lam+central_meridian), phi };
        }
        /// <summary>
        /// Returns the inverse of this projection.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current projection.</returns>
        public override IMathTransform Inverse()
        {
            if (_inverse == null)
                _inverse = new PolyconicProjection(_Parameters.ToProjectionParameter(), this);
            return _inverse;
        }

        #region Private helpers
        ///<summary>
         /// Computes function <code>f(s,c,e²) = c/sqrt(1 - s²*e²)</code> needed for the true scale
         /// latitude (Snyder 14-15), where <var>s</var> and <var>c</var> are the sine and cosine of
         /// the true scale latitude, and <var>e²</var> is the eccentricity squared.
        ///</summary>
        double msfn(double s, double c)
        {
            return c / Math.Sqrt(1.0 - (s * s) * _es);
        }

        #endregion

    }
}