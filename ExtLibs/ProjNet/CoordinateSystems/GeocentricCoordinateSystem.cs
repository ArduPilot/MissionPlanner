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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// A 3D coordinate system, with its origin at the center of the Earth.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class GeocentricCoordinateSystem : CoordinateSystem, IGeocentricCoordinateSystem
	{
		internal GeocentricCoordinateSystem(IHorizontalDatum datum, ILinearUnit linearUnit, IPrimeMeridian primeMeridian, List<AxisInfo> axisinfo,
			string name, string authority, long code, string alias, 
			string remarks, string abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_HorizontalDatum = datum;
			_LinearUnit = linearUnit;
			_Primemeridan = primeMeridian;
			if (axisinfo.Count != 3)
				throw new ArgumentException("Axis info should contain three axes for geocentric coordinate systems");
			base.AxisInfo = axisinfo;
		}

		#region Predefined geographic coordinate systems

		/// <summary>
		/// Creates a geocentric coordinate system based on the WGS84 ellipsoid, suitable for GPS measurements
		/// </summary>
		public static IGeocentricCoordinateSystem WGS84
		{
			get
			{
				return new CoordinateSystemFactory().CreateGeocentricCoordinateSystem("WGS84 Geocentric",
					CoordinateSystems.HorizontalDatum.WGS84, CoordinateSystems.LinearUnit.Metre, 
					CoordinateSystems.PrimeMeridian.Greenwich);
			}
		}

		#endregion

		#region IGeocentricCoordinateSystem Members

		private IHorizontalDatum _HorizontalDatum;

		/// <summary>
		/// Returns the HorizontalDatum. The horizontal datum is used to determine where
		/// the centre of the Earth is considered to be. All coordinate points will be 
		/// measured from the centre of the Earth, and not the surface.
		/// </summary>
		public IHorizontalDatum HorizontalDatum
		{
			get { return _HorizontalDatum; }
			set { _HorizontalDatum = value; }
		}

		private ILinearUnit _LinearUnit;

		/// <summary>
		/// Gets the units used along all the axes.
		/// </summary>
		public ILinearUnit LinearUnit
		{
			get { return _LinearUnit; }
			set { _LinearUnit = value; }
		}

		/// <summary>
		/// Gets units for dimension within coordinate system. Each dimension in 
		/// the coordinate system has corresponding units.
		/// </summary>
		/// <param name="dimension">Dimension</param>
		/// <returns>Unit</returns>
		public override IUnit GetUnits(int dimension)
		{
			return _LinearUnit;
		}

		private IPrimeMeridian _Primemeridan;

		/// <summary>
		/// Returns the PrimeMeridian.
		/// </summary>
		public IPrimeMeridian PrimeMeridian
		{
			get { return _Primemeridan; }
			set { _Primemeridan = value; }
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
				sb.AppendFormat("GEOCCS[\"{0}\", {1}, {2}, {3}", Name, HorizontalDatum.WKT, PrimeMeridian.WKT, LinearUnit.WKT);
				//Skip axis info if they contain default values				
				if (AxisInfo.Count != 3 ||
					AxisInfo[0].Name != "X" || AxisInfo[0].Orientation != AxisOrientationEnum.Other ||
					AxisInfo[1].Name != "Y" || AxisInfo[1].Orientation != AxisOrientationEnum.East ||
					AxisInfo[2].Name != "Z" || AxisInfo[2].Orientation != AxisOrientationEnum.North)
					for (int i = 0; i < AxisInfo.Count; i++)
						sb.AppendFormat(", {0}", GetAxis(i).WKT);
				if (!String.IsNullOrEmpty(Authority) && AuthorityCode>0)
					sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
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
				StringBuilder sb = new StringBuilder();
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat,
					"<CS_CoordinateSystem Dimension=\"{0}\"><CS_GeocentricCoordinateSystem>{1}",
					this.Dimension, InfoXml);				
				foreach (AxisInfo ai in this.AxisInfo)
					sb.Append(ai.XML);
				sb.AppendFormat("{0}{1}{2}</CS_GeocentricCoordinateSystem></CS_CoordinateSystem>",
					HorizontalDatum.XML, LinearUnit.XML, PrimeMeridian.XML);
				return sb.ToString();
			}
		}

		/// <summary>
		/// Checks whether the values of this instance is equal to the values of another instance.
		/// Only parameters used for coordinate system are used for comparison.
		/// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if equal</returns>
		public override bool EqualParams(object obj)
		{
			if (!(obj is GeocentricCoordinateSystem))
				return false;
			GeocentricCoordinateSystem gcc = obj as GeocentricCoordinateSystem;
			return gcc.HorizontalDatum.EqualParams(this.HorizontalDatum) &&
				gcc.LinearUnit.EqualParams(this.LinearUnit) &&
				gcc.PrimeMeridian.EqualParams(this.PrimeMeridian);
		}

		#endregion
	}
}
