// Copyright 2015
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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Implemetns the Oblique Stereographic Projection.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable]
#endif
    internal class ObliqueStereographicProjection : MapProjection
    {
        private double globalScale;

        private static double ITERATION_TOLERANCE = 1E-14;
        private static int MAXIMUM_ITERATIONS = 15;
        private static double EPSILON = 1E-6;
        private double C, K, ratexp;
        private double phic0, cosc0, sinc0, R2;


        /// <summary>
        /// Initializes the ObliqueStereographicProjection object with the specified parameters.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <remarks>
        /// <para>The parameters this projection expects are listed below.</para>
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>central_meridian</term><description>The longitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the longitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).</description></item>
        /// <item><term>latitude_of_origin</term><description>The latitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the latitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).</description></item>
        /// <item><term>scale_factor</term><description>The factor by which the map grid is reduced or enlarged during the projection process, defined by its value at the natural origin.</description></item>
        /// <item><term>false_easting</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Easting, FE, is the easting value assigned to the abscissa (east).</description></item>
        /// <item><term>false_northing</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Northing, FN, is the northing value assigned to the ordinate.</description></item>
        /// </list>
        /// </remarks>
        public ObliqueStereographicProjection(IEnumerable<ProjectionParameter> parameters)
            : this(parameters, null)
        {
        }

        /// <summary>
        /// Initializes the ObliqueStereographicProjection object with the specified parameters.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <param name="inverse">Inverse projection</param>
        /// <remarks>
        /// <para>The parameters this projection expects are listed below.</para>
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>central_meridian</term><description>The longitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the longitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).</description></item>
        /// <item><term>latitude_of_origin</term><description>The latitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the latitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).</description></item>
        /// <item><term>scale_factor</term><description>The factor by which the map grid is reduced or enlarged during the projection process, defined by its value at the natural origin.</description></item>
        /// <item><term>false_easting</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Easting, FE, is the easting value assigned to the abscissa (east).</description></item>
        /// <item><term>false_northing</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Northing, FN, is the northing value assigned to the ordinate.</description></item>
        /// </list>
        /// </remarks>
        public ObliqueStereographicProjection(IEnumerable<ProjectionParameter> parameters, ObliqueStereographicProjection inverse)
            : base(parameters, inverse)
        {
            globalScale = scale_factor * this._semiMajor;

            double sphi = Math.Sin(lat_origin);
            double cphi = Math.Cos(lat_origin);
            cphi *= cphi;
            R2 = 2.0 * Math.Sqrt(1 - _es) / (1 - _es * sphi * sphi);
            C = Math.Sqrt(1.0 + _es * cphi * cphi / (1.0 - _es));
            phic0 = Math.Asin(sphi / C);
            sinc0 = Math.Sin(phic0);
            cosc0 = Math.Cos(phic0);
            ratexp = 0.5 * C * _e;
            K = Math.Tan(0.5 * phic0 + Math.PI / 4) / (Math.Pow(Math.Tan(0.5 * lat_origin + Math.PI / 4), C) * srat(_e * sphi, ratexp));
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        protected override double[] MetersToRadians(double[] p)
        {
            double x = p[0] / this.globalScale;
            double y = p[1] / this.globalScale;

            double rho = Math.Sqrt((x * x) + (y * y));
            if (Math.Abs(rho) < EPSILON)
            {
                x = 0.0;
                y = phic0;
            }
            else
            {
                double ce = 2.0 * Math.Atan2(rho, R2);
                double sinc = Math.Sin(ce);
                double cosc = Math.Cos(ce);
                x = Math.Atan2(x * sinc, rho * cosc0 * cosc - y * sinc0
                       * sinc);
                y = (cosc * sinc0) + (y * sinc * cosc0 / rho);

                if (Math.Abs(y) >= 1.0)
                {
                    y = (y < 0.0) ? -Math.PI / 2.0 : Math.PI / 2.0;
                }
                else
                {
                    y = Math.Asin(y);
                }
            }

            x /= C;
            double num = Math.Pow(Math.Tan(0.5 * y + Math.PI / 4.0) / K, 1.0 / C);
            for (int i = MAXIMUM_ITERATIONS; ; )
            {
                double phi = 2.0 * Math.Atan(num * srat(_e * Math.Sin(y), -0.5 * _e)) - Math.PI / 2.0;
                if (Math.Abs(phi - y) < ITERATION_TOLERANCE)
                {
                    break;
                }
                y = phi;
                if (--i < 0)
                {
                    throw new Exception("Oblique Stereographics doesn't converge");
                }
            }

            x += central_meridian;

            if (p.Length == 2)
                return new double[] { x, y };
            else
                return new double[] { x, y, p[2] };
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        protected override double[] RadiansToMeters(double[] lonlat)
        {
            double x = lonlat[0] - this.central_meridian;
            double y = lonlat[1];


            y = 2.0 * Math.Atan(K * Math.Pow(Math.Tan(0.5 * y + Math.PI / 4), C)
                          * srat(_e * Math.Sin(y), ratexp))
                   - Math.PI / 2;
            x *= C;
            double sinc = Math.Sin(y);
            double cosc = Math.Cos(y);
            double cosl = Math.Cos(x);
            double k = R2 / (1.0 + sinc0 * sinc + cosc0 * cosc * cosl);
            x = k * cosc * Math.Sin(x);
            y = k * (cosc0 * sinc - sinc0 * cosc * cosl);


            if (lonlat.Length == 2)

                return new double[] { x * this.globalScale, y * this.globalScale };
            else
                return new double[] { x * this.globalScale, y * this.globalScale, lonlat[2] };
        }

        /// <summary>
        /// Returns the inverse of this projection.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current projection.</returns>
        public override IMathTransform Inverse()
        {
            if (_inverse == null)
            {
                _inverse = new ObliqueStereographicProjection(_Parameters.ToProjectionParameter(), this);
            }

            return _inverse;
        }


        private double srat(double esinp, double exp)
        {
            return Math.Pow((1.0 - esinp) / (1.0 + esinp), exp);
        }
    }
}
