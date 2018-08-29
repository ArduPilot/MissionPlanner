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

using System;
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// Definition of angular units.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class AngularUnit : Info, IAngularUnit
	{
	    /// <summary>
	    /// Equality tolerance value. Values with a difference less than this are considered equal.
	    /// </summary>
        private const double EqualityTolerance = 2.0e-17;
        
        /// <summary>
		/// Initializes a new instance of a angular unit
		/// </summary>
		/// <param name="radiansPerUnit">Radians per unit</param>
		public AngularUnit(double radiansPerUnit)
			: this(
			radiansPerUnit,String.Empty,String.Empty,-1,String.Empty,String.Empty,String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of a angular unit
		/// </summary>
		/// <param name="radiansPerUnit">Radians per unit</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="authorityCode">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal AngularUnit(double radiansPerUnit, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
			:
			base(name, authority, authorityCode, alias, abbreviation, remarks)
		{
			_RadiansPerUnit = radiansPerUnit;
		}

		#region Predifined units

		/// <summary>
		/// The angular degrees are PI/180 = 0.017453292519943295769236907684886 radians
		/// </summary>
		public static AngularUnit Degrees
		{
			get { return new AngularUnit(0.017453292519943295769236907684886, "degree", "EPSG", 9102, "deg", String.Empty, "=pi/180 radians"); }
		}

		/// <summary>
		/// SI standard unit
		/// </summary>
		public static AngularUnit Radian
		{
			get { return new AngularUnit(1, "radian", "EPSG", 9101, "rad", String.Empty, "SI standard unit."); }
		}

		/// <summary>
		/// Pi / 200 = 0.015707963267948966192313216916398 radians
		/// </summary>
		public static AngularUnit Grad
		{
			get { return new AngularUnit(0.015707963267948966192313216916398, "grad", "EPSG", 9105, "gr", String.Empty, "=pi/200 radians."); }
		}

		/// <summary>
		/// Pi / 200 = 0.015707963267948966192313216916398 radians
		/// </summary>		
		public static AngularUnit Gon
		{
			get { return new AngularUnit(0.015707963267948966192313216916398, "gon", "EPSG", 9106, "g", String.Empty, "=pi/200 radians."); }
		}
		#endregion

		#region IAngularUnit Members

		private double _RadiansPerUnit;

		/// <summary>
		/// Gets or sets the number of radians per <see cref="AngularUnit"/>.
		/// </summary>
		public double RadiansPerUnit
		{
			get { return _RadiansPerUnit; }
			set { _RadiansPerUnit = value; }
		}

		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification.
		/// </summary>
		public override string WKT
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat,"UNIT[\"{0}\", {1}", Name, RadiansPerUnit);
				if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
					sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
				sb.Append("]");
				return sb.ToString();				
			}
		}

		/// <summary>
		/// Gets an XML representation of this object.
		/// </summary>
		public override string XML
		{
			get
			{
                return String.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_AngularUnit RadiansPerUnit=\"{0}\">{1}</CS_AngularUnit>", RadiansPerUnit, InfoXml);
			}
		}

		#endregion

		/// <summary>
		/// Checks whether the values of this instance is equal to the values of another instance.
		/// Only parameters used for coordinate system are used for comparison.
		/// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if equal</returns>
		public override bool EqualParams(object obj)
		{
			if (!(obj is AngularUnit))
				return false;
            return Math.Abs(((AngularUnit)obj).RadiansPerUnit - this.RadiansPerUnit) < EqualityTolerance;
		}
	}
}
