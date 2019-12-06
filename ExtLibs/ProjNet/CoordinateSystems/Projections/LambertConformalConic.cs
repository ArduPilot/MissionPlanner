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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{

	/// <summary>
	/// Implemetns the Lambert Conformal Conic 2SP Projection.
	/// </summary>
	/// <remarks>
	/// <para>The Lambert Conformal Conic projection is a standard projection for presenting maps
	/// of land areas whose East-West extent is large compared with their North-South extent.
	/// This projection is "conformal" in the sense that lines of latitude and longitude, 
	/// which are perpendicular to one another on the earth's surface, are also perpendicular
	/// to one another in the projected domain.</para>
    /// </remarks>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    internal class LambertConformalConic2SP : MapProjection
	{
	
		//double _falseEasting;
		//double _falseNorthing;
	
		//private double es=0;              /* eccentricity squared         */
		//private double e=0;               /* eccentricity                 */
		//private double center_lon=0;      /* center longituted            */
		//private double center_lat=0;      /* cetner latitude              */
		private readonly double ns=0;              /* ratio of angle between meridian*/
		private readonly double f0=0;              /* flattening of ellipsoid      */
		private readonly double rh=0;              /* height above ellipsoid       */

		#region Constructors

		/// <summary>
		/// Creates an instance of an LambertConformalConic2SPProjection projection object.
		/// </summary>
		/// <remarks>
		/// <para>The parameters this projection expects are listed below.</para>
		/// <list type="table">
		/// <listheader><term>Items</term><description>Descriptions</description></listheader>
		/// <item><term>latitude_of_false_origin</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>longitude_of_false_origin</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>latitude_of_1st_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>latitude_of_2nd_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>easting_at_false_origin</term><description>The easting value assigned to the false origin.</description></item>
		/// <item><term>northing_at_false_origin</term><description>The northing value assigned to the false origin.</description></item>
		/// </list>
		/// </remarks>
		/// <param name="parameters">List of parameters to initialize the projection.</param>
		public LambertConformalConic2SP(IEnumerable<ProjectionParameter> parameters) 
            : this(parameters,null)
		{
		}
	
		/// <summary>
		/// Creates an instance of an Albers projection object.
		/// </summary>
		/// <remarks>
		/// <para>The parameters this projection expects are listed below.</para>
		/// <list type="table">
		/// <listheader><term>Parameter</term><description>Description</description></listheader>
		/// <item><term>latitude_of_origin</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>central_meridian</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
		/// <item><term>standard_parallel_1</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>standard_parallel_2</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
		/// <item><term>false_easting</term><description>The easting value assigned to the false origin.</description></item>
		/// <item><term>false_northing</term><description>The northing value assigned to the false origin.</description></item>
		/// </list>
		/// </remarks>
		/// <param name="parameters">List of parameters to initialize the projection.</param>
		/// <param name="inverse">Indicates whether the projection forward (meters to degrees or degrees to meters).</param>
		protected LambertConformalConic2SP(IEnumerable<ProjectionParameter> parameters, LambertConformalConic2SP inverse)
			: base(parameters, inverse)
		{
			Name = "Lambert_Conformal_Conic_2SP";
			Authority = "EPSG";
			AuthorityCode = 9802;
			
            //Check for missing parameters
			var lat1 = Degrees2Radians(_Parameters.GetParameterValue("standard_parallel_1"));
            var lat2 = Degrees2Radians(_Parameters.GetParameterValue("standard_parallel_2"));

			double sin_po;                  /* sin value                            */
			double cos_po;                  /* cos value                            */
			double con;                     /* temporary variable                   */
			double ms1;                     /* small m 1                            */
			double ms2;                     /* small m 2                            */
			double ts0;                     /* small t 0                            */
			double ts1;                     /* small t 1                            */
			double ts2;                     /* small t 2                            */



			/* Standard Parallels cannot be equal and on opposite sides of the equator
			------------------------------------------------------------------------*/
			if (Math.Abs(lat1+lat2) < EPSLN)
			{
				//Debug.Assert(true,"LambertConformalConic:LambertConformalConic() - Equal Latitiudes for St. Parallels on opposite sides of equator");
				throw new ArgumentException("Equal latitudes for St. Parallels on opposite sides of equator.");
			}

			sincos(lat1,out sin_po,out cos_po);
			con = sin_po;
			ms1 = msfnz(_e,sin_po,cos_po);
			ts1 = tsfnz(_e,lat1,sin_po);
			sincos(lat2,out sin_po,out cos_po);
			ms2 = msfnz(_e,sin_po,cos_po);
			ts2 = tsfnz(_e,lat2,sin_po);
			sin_po = Math.Sin(lat_origin);
			ts0 = tsfnz(_e,lat_origin,sin_po);

			if (Math.Abs(lat1 - lat2) > EPSLN)
				ns = Math.Log(ms1/ms2)/ Math.Log (ts1/ts2);
			else
				ns = con;
			f0 = ms1 / (ns * Math.Pow(ts1,ns));
			rh = _semiMajor * f0 * Math.Pow(ts0,ns);
		}
		#endregion

		/// <summary>
		/// Converts coordinates in decimal degrees to projected meters.
		/// </summary>
		/// <param name="lonlat">The point in decimal degrees.</param>
		/// <returns>Point in projected meters</returns>
        protected override double[] RadiansToMeters(double[] lonlat)
		{
			double dLongitude = lonlat[0];
			double dLatitude = lonlat[1];

			double con;                     /* temporary angle variable             */
			double rh1;                     /* height above ellipsoid               */
			double sinphi;                  /* sin value                            */
			double theta;                   /* angle                                */
			double ts;                      /* small value t                        */


			con  = Math.Abs( Math.Abs(dLatitude) - HALF_PI);
			if (con > EPSLN)
			{
				sinphi = Math.Sin(dLatitude);
				ts = tsfnz(_e,dLatitude,sinphi);
				rh1 = _semiMajor * f0 * Math.Pow(ts,ns);
			}
			else
			{
				con = dLatitude * ns;
				if (con <= 0)
					throw new ArgumentException();
				rh1 = 0;
			}
			theta = ns * adjust_lon(dLongitude - central_meridian);
			dLongitude = rh1 * Math.Sin(theta);
			dLatitude = rh - rh1 * Math.Cos(theta);
			
            return lonlat.Length == 2 
                ? new [] { dLongitude, dLatitude } 
                : new [] { dLongitude, dLatitude , lonlat[2] };
		}

		/// <summary>
		/// Converts coordinates in projected meters to decimal degrees.
		/// </summary>
		/// <param name="p">Point in meters</param>
		/// <returns>Transformed point in decimal degrees</returns>
        protected override double[] MetersToRadians(double[] p)
		{
			double dLongitude = Double.NaN;
			double dLatitude = Double.NaN;

			double rh1;			/* height above ellipsoid	*/
			double con;			/* sign variable		*/
			double ts;			/* small t			*/
			double theta;			/* angle			*/
			long   flag;			/* error flag			*/

			flag = 0;
			double dX = p[0];
			double dY = rh - p[1];
			if (ns > 0)
			{
				rh1 = Math.Sqrt(dX * dX + dY * dY);
				con = 1.0;
			}
			else
			{
				rh1 = -Math.Sqrt(dX * dX + dY * dY);
				con = -1.0;
			}
			theta = 0.0;
			if (rh1 != 0)
				theta = Math.Atan2((con * dX),(con * dY));
			if ((rh1 != 0) || (ns > 0.0))
			{
				con = 1.0/ns;
				ts = Math.Pow((rh1/(_semiMajor * f0)),con);
				dLatitude = phi2z(_e,ts,out flag);
				if (flag != 0)
					throw new ArgumentException();				
			}
			else dLatitude = -HALF_PI;
			
			dLongitude = adjust_lon(theta/ns + central_meridian);
			return p.Length==2 
                ? new [] { dLongitude, dLatitude } 
                : new [] { dLongitude, dLatitude, p[2]};
		}

		/// <summary>
		/// Returns the inverse of this projection.
		/// </summary>
		/// <returns>IMathTransform that is the reverse of the current projection.</returns>
		public override IMathTransform Inverse()
		{
			if (_inverse == null)
			{
			    _inverse = new LambertConformalConic2SP(_Parameters.ToProjectionParameter(), this);
			}
			return _inverse;
		}
	}
}
