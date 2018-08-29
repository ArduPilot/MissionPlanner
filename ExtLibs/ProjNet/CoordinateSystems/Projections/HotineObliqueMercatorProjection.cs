using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    internal class ObliqueMercatorProjection : HotineObliqueMercatorProjection
    {
        public ObliqueMercatorProjection(IEnumerable<ProjectionParameter> parameters)
            : this(parameters, null)
        {
        }

        public ObliqueMercatorProjection(IEnumerable<ProjectionParameter> parameters, ObliqueMercatorProjection inverse)
            : base(parameters, inverse)
        {
            AuthorityCode = 9815;
            Name = "Oblique_Mercator";
        }

        public override IMathTransform Inverse()
        {
            if (_inverse == null)
                _inverse = new ObliqueMercatorProjection(_Parameters.ToProjectionParameter(), this);
            return _inverse;
        }
    }

#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    internal class HotineObliqueMercatorProjection : MapProjection
    {
        private readonly double _azimuth;
        private readonly double _sinP20, _cosP20;
        private readonly double _bl, _al;
        private readonly double _d, _el;
        private readonly double _singrid, _cosgrid;
        private readonly double _singam, _cosgam;
        private readonly double _sinaz, _cosaz;
        private readonly double _u;

        private bool NaturalOriginOffsets {
            get
            {
                if (AuthorityCode == 9812) return false;
                if (AuthorityCode == 9815) return true;
                throw new ArgumentException("AuthorityCode");
            }
        }

        public HotineObliqueMercatorProjection(IEnumerable<ProjectionParameter> parameters)
            : this(parameters, null)
        {
        }

        public HotineObliqueMercatorProjection(IEnumerable<ProjectionParameter> parameters, HotineObliqueMercatorProjection inverse)
            : base(parameters, inverse)
        {
            Authority = "EPSG";
            AuthorityCode = 9812;
            Name = "Hotine_Oblique_Mercator";

            _azimuth = Degrees2Radians(_Parameters.GetParameterValue("azimuth"));
            var rectifiedGridAngle = Degrees2Radians(_Parameters.GetParameterValue("rectified_grid_angle"));
            

            sincos(lat_origin, out _sinP20, out _cosP20);
            var con = 1.0 - _es * Math.Pow(_sinP20, 2);
            var com = Math.Sqrt(1.0 - _es);
            _bl = Math.Sqrt(1.0 + _es * Math.Pow(_cosP20, 4.0) / ( 1.0 - _es ));
            _al = _semiMajor * _bl * scale_factor * com / con;

            double f;
            if (Math.Abs(lat_origin) < EPSLN)
            {
                //ts = 1.0;
                _d = 1.0;
                _el = 1.0;
                f = 1.0;
            }
            else
            {
                var ts = tsfnz(_e, lat_origin, _sinP20);
                con = Math.Sqrt(con);
                _d = _bl * com / ( _cosP20 * con );
                if ( ( _d * _d - 1.0 ) > 0.0 )
                {
                    if ( lat_origin >= 0.0 )
                        f = _d + Math.Sqrt(_d * _d - 1.0);
                    else
                        f = _d - Math.Sqrt(_d * _d - 1.0);
                }
                else
                    f = _d;
                _el = f * Math.Pow(ts, _bl);
            }

            var g = .5 * ( f - 1.0 / f );
            var gama = asinz(Math.Sin(_azimuth) / _d);
            lon_origin = lon_origin - asinz(g * Math.Tan(gama)) / _bl;

            con = Math.Abs(lat_origin);
            if ( ( con > EPSLN ) && ( Math.Abs(con - HALF_PI) > EPSLN ) )
            {
                sincos(gama, out _singam, out _cosgam);
                sincos(_azimuth, out _sinaz, out _cosaz);
                if ( lat_origin >= 0 )
                    _u = ( _al / _bl ) * Math.Atan(Math.Sqrt(_d * _d - 1.0) / _cosaz);
                else
                    _u = -( _al / _bl ) * Math.Atan(Math.Sqrt(_d * _d - 1.0) / _cosaz);
            }
            else
            {
                throw new ArgumentException("Input data error");
            }

            sincos(rectifiedGridAngle, out _singrid, out _cosgrid);

        }

        public override IMathTransform Inverse()
        {
            if (_inverse == null)
            {
                _inverse = new HotineObliqueMercatorProjection(_Parameters.ToProjectionParameter(), this);
            }
            return _inverse;

        }

        protected override double[] RadiansToMeters(double[] lonlat)
        {
            var lon = lonlat[0];
            var lat = lonlat[1];

            Double us, ul;

            // Forward equations
            // -----------------
            var sin_phi = Math.Sin(lat);
            var dlon = adjust_lon(lon - lon_origin);
            var vl = Math.Sin(_bl * dlon);
            if (Math.Abs(Math.Abs(lat) - HALF_PI) > EPSLN)
            {
                var ts1 = tsfnz(_e, lat, sin_phi);
                var q = _el / (Math.Pow(ts1, _bl));
                var s = .5 * (q - 1.0 / q);
                var t = .5 * (q + 1.0 / q);
                ul = (s * _singam - vl * _cosgam) / t;
                var con = Math.Cos(_bl * dlon);
                if (Math.Abs(con) < .0000001)
                {
                    us = _al * _bl * dlon;
                }
                else
                {
                    us = _al * Math.Atan((s * _cosgam + vl * _singam) / con) / _bl;
                    if (con < 0)
                        us = us + PI * _al / _bl;
                }
            }
            else
            {
                if (lat >= 0)
                    ul = _singam;
                else
                    ul = -_singam;
                us = _al * lat / _bl;
            }
            if (Math.Abs(Math.Abs(ul) - 1.0) <= EPSLN)
            {
                throw new Exception("Point projects into infinity");
            }

            var vs = .5 * _al * Math.Log((1.0 - ul) / (1.0 + ul)) / _bl;
            if (!NaturalOriginOffsets) us = us - _u;
            var x = vs * _cosgrid + us * _singrid;
            var y = us * _cosgrid - vs * _singrid;

            return lonlat.Length == 2
                ? new [] {x, y} :
                  new [] {x, y, lonlat[2]};
        }

        protected override double[] MetersToRadians(double[] p)
        {
            // Inverse equations
            // -----------------
            var x = p[0];
            var y = p[1];
            var vs = x * _cosgrid - y * _singrid;
            var us = y * _cosgrid + x * _singrid;
            if (!NaturalOriginOffsets) us = us + _u;
            var q = Math.Exp(-_bl * vs / _al);
            var s = .5 * (q - 1.0 / q);
            var t = .5 * (q + 1.0 / q);
            var vl = Math.Sin(_bl * us / _al);
            var ul = (vl * _cosgam + s * _singam) / t;
            double lon, lat;
            if (Math.Abs(Math.Abs(ul) - 1.0) <= EPSLN)
            {
                lon = lon_origin;
                lat = sign(ul)*HALF_PI;
            }
            else
            {
                var con = 1.0/_bl;
                var ts1 = Math.Pow((_el/Math.Sqrt((1.0 + ul)/(1.0 - ul))), con);
                long flag;
                lat = phi2z(_e, ts1, out flag);
                con = Math.Cos(_bl*us/_al);
                var theta = lon_origin - Math.Atan2((s*_cosgam - vl*_singam), con)/_bl;
                lon = adjust_lon(theta);
            }

            return p.Length == 2
                ? new[] { lon, lat } : new[] { lon, lat, p[2] };
        }
    }
}