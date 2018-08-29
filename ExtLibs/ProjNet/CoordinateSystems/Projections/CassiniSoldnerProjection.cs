using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
    internal class CassiniSoldnerProjection : MapProjection
    {
// ReSharper disable InconsistentNaming
        private const Double One6th = 0.16666666666666666666d;      //C1
        private const Double One120th = 0.00833333333333333333d;    //C2
        private const Double One24th = 0.04166666666666666666d;     //C3
        private const Double One3rd = 0.33333333333333333333d;      //C4
        private const Double One15th = 0.06666666666666666666d;     //C5
// ReSharper restore InconsistentNaming

        private readonly double _cFactor;
        private readonly double _m0;
        private readonly double _reciprocalSemiMajor;

        public CassiniSoldnerProjection(IEnumerable<ProjectionParameter> parameters) : this(parameters, null)
        {
        }

        public CassiniSoldnerProjection(IEnumerable<ProjectionParameter> parameters, CassiniSoldnerProjection inverse)
            : base(parameters, inverse)
        {
            Authority = "EPSG";
            AuthorityCode = 9806;
            Name = "Cassini_Soldner";

            _cFactor = _es/(1 - _es);
            _m0 = mlfn(lat_origin, Math.Sin(lat_origin), Math.Cos(lat_origin));
            _reciprocalSemiMajor = 1d/_semiMajor;
        }

        public override IMathTransform Inverse()
        {
            if (_inverse == null)
                _inverse = new CassiniSoldnerProjection(_Parameters.ToProjectionParameter(), this);
            return _inverse;
        }

        protected override double[] RadiansToMeters(double[] lonlat)
        {
            var lambda = lonlat[0] - central_meridian;
            var phi = lonlat[1];

            double sinPhi, cosPhi; // sin and cos value
            sincos(phi, out sinPhi, out cosPhi);

            var y = mlfn(phi, sinPhi, cosPhi);
            var n = 1.0d / Math.Sqrt(1 - _es * sinPhi * sinPhi);
            var tn = Math.Tan(phi);
            var t = tn * tn;
            var a1 = lambda * cosPhi;
            var a2 = a1 * a1;
            var c = _cFactor * Math.Pow(cosPhi, 2.0d);

            var x = n * a1 * (1.0d - a2 * t * (One6th - (8.0d - t + 8.0d * c) * a2 * One120th));
            y -= _m0 - n * tn * a2 * (0.5d + (5.0d - t + 6.0d * c) * a2 * One24th);

            return lonlat.Length == 2
                       ? new[] {_semiMajor*x, _semiMajor*y}
                       : new[] {_semiMajor*x, _semiMajor*y, lonlat[2]};
        }

        protected override double[] MetersToRadians(double[] p)
        {
            
            var x = p[0] * _reciprocalSemiMajor;
            var y = p[1] * _reciprocalSemiMajor;
            var phi1 = Phi1(_m0 + y);

            var tn = Math.Tan(phi1);
            var t = tn * tn;
            var n = Math.Sin(phi1);
            var r = 1.0d / (1.0d - _es * n * n);
            n = Math.Sqrt(r);
            r *= (1.0d - _es) * n;
            var dd = x / n;
            var d2 = dd * dd;

            var phi = phi1 - (n * tn / r) * d2 * (.5 - (1.0 + 3.0 * t) * d2 * One24th);
            var lambda = dd * (1.0 + t * d2 * (-One3rd + (1.0 + 3.0 * t) * d2 * One15th)) / Math.Cos(phi1);
            lambda = adjust_lon(lambda + central_meridian);

            return p.Length == 2
                       ? new[] {lambda, phi}
                       : new[] {lambda, phi, p[2]};
        }

        private double Phi1(Double arg)
        {
            const int maxIter = 10;
            const double eps = 1e-11;

            var k = 1.0d / (1.0d - _es);

            var phi = arg;
            for (var i = maxIter; i > 0; --i)
            { // rarely goes over 2 iterations 
                var sinPhi = Math.Sin(phi);
                var t = 1.0d - _es * sinPhi * sinPhi;
                t = (mlfn(phi, sinPhi, Math.Cos(phi)) - arg) * (t * Math.Sqrt(t)) * k;
                phi -= t;
                if (Math.Abs(t) < eps) return phi;
            }
            throw new ArgumentException("Convergence error.");
        }

    }
}