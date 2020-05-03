// Copyright 2005 - 2009 - Morten Nielsen (www.sharpgis.net)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Projections inherit from this abstract class to get access to useful mathematical functions.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public abstract class MapProjection : MathTransform, IProjection
    {
// ReSharper disable InconsistentNaming
        protected readonly double _e;
        protected readonly double _es;
        protected readonly double _semiMajor;
        protected readonly double _semiMinor;
        protected readonly double _metersPerUnit;

        protected readonly double scale_factor; /* scale factor				*/
        protected double central_meridian; /* Center longitude (projection center) */
        protected double lon_origin { get { return central_meridian; } set { central_meridian = value; } }
        protected readonly double lat_origin; /* center latitude			*/
        protected readonly double false_northing; /* y offset in meters			*/
        protected readonly double false_easting; /* x offset in meters			*/

        protected readonly double en0, en1, en2, en3, en4;

        protected ProjectionParameterSet _Parameters;

        protected MathTransform _inverse;
        private bool _isInverse;

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="parameters">An enumeration of projection parameters</param>
        /// <param name="inverse">Indicator if this projection is inverse</param>
        protected MapProjection(IEnumerable<ProjectionParameter> parameters, MapProjection inverse)
            : this(parameters)
        {
            _inverse = inverse;
            if (_inverse != null)
            {
                inverse._inverse = this;
                _isInverse = !inverse._isInverse;
            }
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="parameters">An enumeration of projection parameters</param>
        protected MapProjection(IEnumerable<ProjectionParameter> parameters)
        {
            _Parameters = new ProjectionParameterSet(parameters);

            _semiMajor = _Parameters.GetParameterValue("semi_major");
            _semiMinor = _Parameters.GetParameterValue("semi_minor");

            //_es = 1.0 - (_semiMinor * _semiMinor) / (_semiMajor * _semiMajor);
            _es = EccentricySquared(_semiMajor, _semiMinor);
            _e = Math.Sqrt(_es);

            scale_factor = _Parameters.GetOptionalParameterValue("scale_factor", 1);

            central_meridian = Degrees2Radians(_Parameters.GetParameterValue("central_meridian", "longitude_of_center"));
            lat_origin = Degrees2Radians(_Parameters.GetParameterValue("latitude_of_origin", "latitude_of_center"));

            _metersPerUnit = _Parameters.GetParameterValue("unit");

            false_easting = _Parameters.GetOptionalParameterValue("false_easting", 0)*_metersPerUnit;
            false_northing = _Parameters.GetOptionalParameterValue("false_northing", 0)*_metersPerUnit;

            // TODO: Should really convert to the correct linear units??

            //  Compute constants for the mlfn
            double t;
            en0 = C00 - _es*(C02 + _es*
                             (C04 + _es*(C06 + _es*C08)));
            en1 = _es*(C22 - _es*
                       (C04 + _es*(C06 + _es*C08)));
            en2 = (t = _es*_es)*
                  (C44 - _es*(C46 + _es*C48));
            en3 = (t *= _es)*(C66 - _es*C68);
            en4 = t*_es*C88;

        }

        /// <summary>
        /// Returns a list of projection "cloned" projection parameters
        /// </summary>
        /// <returns></returns>
        protected internal static List<ProjectionParameter> CloneParametersList(
            IEnumerable<ProjectionParameter> projectionParameters)
        {
            var res = new List<ProjectionParameter>();
            foreach (var pp in projectionParameters)
                res.Add(new ProjectionParameter(pp.Name, pp.Value));
            return res;
        }


        #region Implementation of IProjection

        /// <summary>
        /// Gets the projection classification name (e.g. 'Transverse_Mercator').
        /// </summary>
        public string ClassName
        {
            get { return Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ProjectionParameter GetParameter(int index)
        {
            return _Parameters.GetAtIndex(index);
        }

        /// <summary>
        /// Gets an named parameter of the projection.
        /// </summary>
        /// <remarks>The parameter name is case insensitive</remarks>
        /// <param name="name">Name of parameter</param>
        /// <returns>parameter or null if not found</returns>
        public ProjectionParameter GetParameter(string name)
        {
            return _Parameters.Find(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public int NumParameters
        {
            get { return _Parameters.Count; }
        }

        /// <summary>
        /// Gets or sets the abbreviation of the object.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets the alias of the object.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the authority name for this object, e.g., "EPSG",
        /// is this is a standard object with an authority specific
        /// identity code. Returns "CUSTOM" if this is a custom object.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the authority specific identification code of the object
        /// </summary>
        public long AuthorityCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider-supplied remarks for the object.
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// Returns the Well-known text for this object
        /// as defined in the simple features specification.
        /// </summary>
        public override string WKT
        {
            get
            {
                var sb = new StringBuilder();
                if (_isInverse)
                    sb.Append("INVERSE_MT[");
                sb.AppendFormat("PARAM_MT[\"{0}\"", Name);
                for (int i = 0; i < NumParameters; i++)
                    sb.AppendFormat(", {0}", GetParameter(i).WKT);
                //if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
                //	sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
                sb.Append("]");
                if (_isInverse)
                    sb.Append("]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an XML representation of this object
        /// </summary>
        public override string XML
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("<CT_MathTransform>");
                sb.AppendFormat(
                    _isInverse
                        ? "<CT_InverseTransform Name=\"{0}\">"
                        : "<CT_ParameterizedMathTransform Name=\"{0}\">", ClassName);
                for (int i = 0; i < NumParameters; i++)
                    sb.AppendFormat(GetParameter(i).XML);
                sb.Append(_isInverse ? "</CT_InverseTransform>" : "</CT_ParameterizedMathTransform>");
                sb.Append("</CT_MathTransform>");
                return sb.ToString();
            }
        }

        #endregion

        #region IMathTransform

        public override int DimSource
        {
            get { return 2; }
        }

        public override int DimTarget
        {
            get { return 2; }
        }

        /// <summary>
        /// Function to transform from meters to degrees
        /// </summary>
        /// <param name="p">The ordinates of the point</param>
        /// <returns>The transformed ordinates</returns>
        public double[] MetersToDegrees(double[] p)
        {
            var tmp = p.Length == 2
                          ? new[] {p[0]*_metersPerUnit - false_easting, p[1]*_metersPerUnit - false_northing}
                          : new[]
                              {
                                  p[0]*_metersPerUnit - false_easting, p[1]*_metersPerUnit - false_northing,
                                  p[2]*_metersPerUnit
                              };

            var res = MetersToRadians(tmp);

            res[0] = Radians2Degrees(res[0]);
            res[1] = Radians2Degrees(res[1]);
            if (DimTarget == 3) res[2] = Radians2Degrees(res[2]);

            return res;
        }

        /// <summary>
        /// Function to transform from degrees to meters
        /// </summary>
        /// <param name="lonlat">The ordinates of the point</param>
        /// <returns>The transformed ordinates</returns>
        public double[] DegreesToMeters(double[] lonlat)
        {
            // Convert to radians
            var tmp = lonlat.Length == 2
                          ? new[] {Degrees2Radians(lonlat[0]), Degrees2Radians(lonlat[1])}
                          : new[] {Degrees2Radians(lonlat[0]), Degrees2Radians(lonlat[1]), Degrees2Radians(lonlat[2])};

            // Convert to meters
            var res = RadiansToMeters(tmp);

            // Add false easting and northing, convert to unit
            res[0] = (res[0] + false_easting)/_metersPerUnit;
            res[1] = (res[1] + false_northing)/_metersPerUnit;
            if (res.Length == 3) res[2] = res[2]/_metersPerUnit;

            return res;
        }

        protected abstract double[] RadiansToMeters(double[] lonlat);
        protected abstract double[] MetersToRadians(double[] p);

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert()
        {
            _isInverse = !_isInverse;
            if (_inverse != null) ((MapProjection) _inverse).Invert(false);
        }

        protected void Invert(bool invertInverse)
        {
            _isInverse = !_isInverse;
            if (invertInverse && _inverse != null) ((MapProjection) _inverse).Invert(false);
        }

        /// <summary>
        /// Returns true if this projection is inverted.
        /// Most map projections define forward projection as "from geographic to projection", and backwards
        /// as "from projection to geographic". If this projection is inverted, this will be the other way around.
        /// </summary>
        protected internal bool IsInverse
        {
            get { return _isInverse; }
        }

        /// <summary>
        /// Transforms the specified cp.
        /// </summary>
        /// <param name="cp">The cp.</param>
        /// <returns></returns>
        public override double[] Transform(double[] cp)
        {
            //var projectedPoint = new double[] { 0, 0, 0, };
            return !_isInverse
                       ? DegreesToMeters(cp)
                       : MetersToDegrees(cp);
        }

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// Only parameters used for coordinate system are used for comparison.
        /// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal</returns>
        public bool EqualParams(object obj)
        {
            if (!(obj is MapProjection))
                return false;
            var proj = obj as MapProjection;

            if (!_Parameters.Equals(proj._Parameters))
                return false;
            /*
            if (proj.NumParameters != NumParameters)
				return false;
			
            for (var i = 0; i < _Parameters.Count; i++)
			{
				var param = _Parameters.Find(par => par.Name.Equals(proj.GetParameter(i).Name, StringComparison.OrdinalIgnoreCase));
				if (param == null)
					return false;
				if (param.Value != proj.GetParameter(i).Value)
					return false;
			}
             */
            return IsInverse == proj.IsInverse;
        }

        #endregion

        #region Helper mathmatical functions

        // defines some usefull constants that are used in the projection routines
        /// <summary>
        /// PI
        /// </summary>
        protected const double PI = Math.PI;

        /// <summary>
        /// Half of PI
        /// </summary>
        protected const double HALF_PI = (PI*0.5);

        /// <summary>
        /// PI * 2
        /// </summary>
        protected const double TWO_PI = (PI*2.0);

        /// <summary>
        /// EPSLN
        /// </summary>
        protected const double EPSLN = 1.0e-10;

        /// <summary>
        /// S2R
        /// </summary>
        protected const double S2R = 4.848136811095359e-6;

        /// <summary>
        /// MAX_VAL
        /// </summary>
        protected const double MAX_VAL = 4;

        /// <summary>
        /// prjMAXLONG
        /// </summary>
        protected const double prjMAXLONG = 2147483647;

        /// <summary>
        /// DBLLONG
        /// </summary>
        protected const double DBLLONG = 4.61168601e18;

        /// <summary>
        /// Returns the cube of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static double CUBE(double x)
        {
            return Math.Pow(x, 3); /* x^3 */
        }

        /// <summary>
        /// Returns the quad of a number.
        /// </summary>
        /// <param name="x"> </param>
        protected static double QUAD(double x)
        {
            return Math.Pow(x, 4); /* x^4 */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double GMAX(ref double A, ref double B)
        {
            return Math.Max(A, B); /* assign maximum of a and b */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double GMIN(ref double A, ref double B)
        {
            return ((A) < (B) ? (A) : (B)); /* assign minimum of a and b */
        }

        /// <summary>
        /// IMOD
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected static double IMOD(double A, double B)
        {
            return (A) - (((A)/(B))*(B)); /* Integer mod function */

        }

        ///<summary>
        ///Function to return the sign of an argument
        ///</summary>
        protected static double sign(double x)
        {
            if (x < 0.0)
                return (-1);
            else return (1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double adjust_lon(double x)
        {
            long count = 0;
            for (;;)
            {
                if (Math.Abs(x) <= PI)
                    break;
                else if (((long) Math.Abs(x/Math.PI)) < 2)
                    x = x - (sign(x)*TWO_PI);
                else if (((long) Math.Abs(x/TWO_PI)) < prjMAXLONG)
                {
                    x = x - (((long) (x/TWO_PI))*TWO_PI);
                }
                else if (((long) Math.Abs(x/(prjMAXLONG*TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((long) (x/(prjMAXLONG*TWO_PI)))*(TWO_PI*prjMAXLONG));
                }
                else if (((long) Math.Abs(x/(DBLLONG*TWO_PI))) < prjMAXLONG)
                {
                    x = x - (((long) (x/(DBLLONG*TWO_PI)))*(TWO_PI*DBLLONG));
                }
                else
                    x = x - (sign(x)*TWO_PI);
                count++;
                if (count > MAX_VAL)
                    break;
            }
            return (x);
        }

        /// <summary>
        /// Function to compute the constant small m which is the radius of
        /// a parallel of latitude, phi, divided by the semimajor axis.
        /// </summary>
        protected static double msfnz(double eccent, double sinphi, double cosphi)
        {
            double con;

            con = eccent*sinphi;
            return ((cosphi/(Math.Sqrt(1.0 - con*con))));
        }

        /// <summary>
        /// Function to compute constant small q which is the radius of a 
        /// parallel of latitude, phi, divided by the semimajor axis. 
        /// </summary>
        protected static double qsfnz(double eccent, double sinphi)
        {
            double con;

            if (eccent > 1.0e-7)
            {
                con = eccent*sinphi;
                return ((1.0 - eccent*eccent)*(sinphi/(1.0 - con*con) - (.5/eccent)*
                                               Math.Log((1.0 - con)/(1.0 + con))));
            }
            else
                return 2.0*sinphi;
        }

        /// <summary>
        /// Function to calculate the sine and cosine in one call.  Some computer
        /// systems have implemented this function, resulting in a faster implementation
        /// than calling each function separately.  It is provided here for those
        /// computer systems which don`t implement this function
        /// </summary>
        protected static void sincos(double val, out double sin_val, out double cos_val)

        {
            sin_val = Math.Sin(val);
            cos_val = Math.Cos(val);
        }

        /// <summary>
        /// Function to compute the constant small t for use in the forward
        /// computations in the Lambert Conformal Conic and the Polar
        /// Stereographic projections.
        /// </summary>
        protected static double tsfnz(double eccent, double phi, double sinphi)
        {
            double con;
            double com;
            con = eccent*sinphi;
            com = .5*eccent;
            con = Math.Pow(((1.0 - con)/(1.0 + con)), com);
            return (Math.Tan(.5*(HALF_PI - phi))/con);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="eccent"></param>
        /// <param name="qs"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected static double phi1z(double eccent, double qs, out long flag)
        {
            double eccnts;
            double dphi;
            double con;
            double com;
            double sinpi;
            double cospi;
            double phi;
            flag = 0;
            //double asinz();
            long i;

            phi = asinz(.5*qs);
            if (eccent < EPSLN)
                return (phi);
            eccnts = eccent*eccent;
            for (i = 1; i <= 25; i++)
            {
                sincos(phi, out sinpi, out cospi);
                con = eccent*sinpi;
                com = 1.0 - con*con;
                dphi = .5*com*com/cospi*(qs/(1.0 - eccnts) - sinpi/com +
                                         .5/eccent*Math.Log((1.0 - con)/(1.0 + con)));
                phi = phi + dphi;
                if (Math.Abs(dphi) <= 1e-7)
                    return (phi);
            }
            //p_error ("Convergence error","phi1z-conv");
            //ASSERT(FALSE);
            throw new ArgumentException("Convergence error.");
        }

        ///<summary>
        ///Function to eliminate roundoff errors in asin
        ///</summary>
        protected static double asinz(double con)
        {
            if (Math.Abs(con) > 1.0)
            {
                if (con > 1.0)
                    con = 1.0;
                else
                    con = -1.0;
            }
            return (Math.Asin(con));
        }

        /// <summary>
        /// Function to compute the latitude angle, phi2, for the inverse of the
        /// Lambert Conformal Conic and Polar Stereographic projections.
        /// </summary>
        /// <param name="eccent">Spheroid eccentricity</param>
        /// <param name="ts">Constant value t</param>
        /// <param name="flag">Error flag number</param>
        protected static double phi2z(double eccent, double ts, out long flag)
        {
            double con;
            double dphi;
            double sinpi;
            long i;

            flag = 0;
            double eccnth = .5*eccent;
            double chi = HALF_PI - 2*Math.Atan(ts);
            for (i = 0; i <= 15; i++)
            {
                sinpi = Math.Sin(chi);
                con = eccent*sinpi;
                dphi = HALF_PI - 2*Math.Atan(ts*(Math.Pow(((1.0 - con)/(1.0 + con)), eccnth))) - chi;
                chi += dphi;
                if (Math.Abs(dphi) <= .0000000001)
                    return (chi);
            }
            throw new ArgumentException("Convergence error - phi2z-conv");
        }

        private const double C00 = 1.0,
                             C02 = 0.25,
                             C04 = 0.046875,
                             C06 = 0.01953125,
                             C08 = 0.01068115234375,
                             C22 = 0.75,
                             C44 = 0.46875,
                             C46 = 0.01302083333333333333,
                             C48 = 0.00712076822916666666,
                             C66 = 0.36458333333333333333,
                             C68 = 0.00569661458333333333,
                             C88 = 0.3076171875;

        ///<summary>
        ///Functions to compute the constants e0, e1, e2, and e3 which are used
        ///in a series for calculating the distance along a meridian.  The
        ///input x represents the eccentricity squared.
        ///</summary>
        protected static double e0fn(double x)
        {
            return (1.0 - 0.25*x*(1.0 + x/16.0*(3.0 + 1.25*x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e1fn(double x)
        {
            return (0.375*x*(1.0 + 0.25*x*(1.0 + 0.46875*x)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e2fn(double x)
        {
            return (0.05859375*x*x*(1.0 + 0.75*x));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected static double e3fn(double x)
        {
            return (x*x*x*(35.0/3072.0));
        }

        /// <summary>
        /// Function to compute the constant e4 from the input of the eccentricity
        /// of the spheroid, x.  This constant is used in the Polar Stereographic
        /// projection.
        /// </summary>
        protected static double e4fn(double x)
        {
            double con;
            double com;
            con = 1.0 + x;
            com = 1.0 - x;
            return (Math.Sqrt((Math.Pow(con, con))*(Math.Pow(com, com))));
        }

        /// <summary>
        /// Function computes the value of M which is the distance along a meridian
        /// from the Equator to latitude phi.
        /// </summary>
        protected static double mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return (e0*phi - e1*Math.Sin(2.0*phi) + e2*Math.Sin(4.0*phi) - e3*Math.Sin(6.0*phi));
        }

        /// <summary>
        /// Calculates the meridian distance. This is the distance along the central 
        /// meridian from the equator to <paramref name="phi"/>. Accurate to &lt; 1e-5 meters 
        /// when used in conjuction with typical major axis values.
        /// </summary>
        /// <param name="phi"></param>
        /// <param name="sphi"></param>
        /// <param name="cphi"></param>
        /// <returns></returns>
        protected double mlfn(double phi, double sphi, double cphi)
        {
            cphi *= sphi;
            sphi *= sphi;
            return en0*phi - cphi*(en1 + sphi*(en2 + sphi*(en3 + sphi*en4)));
        }

        /// <summary>
        /// Calculates the latitude (phi) from a meridian distance.
        /// Determines phi to TOL (1e-11) radians, about 1e-6 seconds.
        /// </summary>
        /// <param name="arg">The meridonial distance</param>
        /// <returns>The latitude of the meridian distance.</returns>
        protected double inv_mlfn(double arg)
        {
            const double MLFN_TOL = 1E-11;
            const int MAXIMUM_ITERATIONS = 20;
            double s, t, phi, k = 1.0/(1.0 - _es);
            int i;
            phi = arg;
            for (i = MAXIMUM_ITERATIONS; true;)
            {
                // rarely goes over 5 iterations
                if (--i < 0)
                {
                    throw new InvalidOperationException("No convergence");
                }
                s = Math.Sin(phi);
                t = 1.0 - _es*s*s;
                t = (mlfn(phi, s, Math.Cos(phi)) - arg)*(t*Math.Sqrt(t))*k;
                phi -= t;
                if (Math.Abs(t) < MLFN_TOL)
                {
                    return phi;
                }
            }
        }

        /// <summary>
        /// Calculates the flattening factor, (<paramref name="equatorialRadius"/> - <paramref name="polarRadius"/>) / <paramref name="equatorialRadius"/>.
        /// </summary>
        /// <param name="equatorialRadius">The radius of the equator</param>
        /// <param name="polarRadius">The radius of a circle touching the poles</param>
        /// <returns>The flattening factor</returns>
        private static double FlatteningFactor(double equatorialRadius, double polarRadius)
        {
            return (equatorialRadius - polarRadius)/equatorialRadius;
        }

        /// <summary>
        /// Calculates the square of eccentricity according to es = (2f - f^2) where f is the <see cref="FlatteningFactor">flattening factor</see>.
        /// </summary>
        /// <param name="equatorialRadius">The radius of the equator</param>
        /// <param name="polarRadius">The radius of a circle touching the poles</param>
        /// <returns>The square of eccentricity</returns>
        private static double EccentricySquared(double equatorialRadius, double polarRadius)
        {
            var f = FlatteningFactor(equatorialRadius, polarRadius);
            return 2*f - f*f;
        }


        /// <summary>
        /// Function to calculate UTM zone number
        /// </summary>
        /// <param name="lon">The longitudinal value (in Degrees!)</param>
        /// <returns>The UTM zone number</returns>
        protected static long CalcUtmZone(double lon)
        {
            return ((long) (((lon + 180.0)/6.0) + 1.0));
        }

        #endregion

        #region Static Methods;

        /// <summary>
        /// Converts a longitude value in degrees to radians.
        /// </summary>
        /// <param name="x">The value in degrees to convert to radians.</param>
        /// <param name="edge">If true, -180 and +180 are valid, otherwise they are considered out of range.</param>
        /// <returns></returns>
        protected static double LongitudeToRadians(double x, bool edge)
        {
            if (edge ? (x >= -180 && x <= 180) : (x > -180 && x < 180))
                return Degrees2Radians(x);
            throw new ArgumentOutOfRangeException("x",
                                                  x.ToString(CultureInfo.InvariantCulture) +
                                                  " not a valid longitude in degrees.");
        }

        /// <summary>
        /// Converts a latitude value in degrees to radians.
        /// </summary>
        /// <param name="y">The value in degrees to to radians.</param>
        /// <param name="edge">If true, -90 and +90 are valid, otherwise they are considered out of range.</param>
        /// <returns></returns>
        protected static double LatitudeToRadians(double y, bool edge)
        {
            if (edge ? (y >= -90 && y <= 90) : (y > -90 && y < 90))
                return Degrees2Radians(y);
            throw new ArgumentOutOfRangeException("y",
                                                  y.ToString(CultureInfo.InvariantCulture) +
                                                  " not a valid latitude in degrees.");
        }

        #endregion
    }
}
