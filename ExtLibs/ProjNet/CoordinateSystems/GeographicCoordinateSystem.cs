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
	/// A coordinate system based on latitude and longitude. 
	/// </summary>
	/// <remarks>
	/// Some geographic coordinate systems are Lat/Lon, and some are Lon/Lat. 
	/// You can find out which this is by examining the axes. You should also 
	/// check the angular units, since not all geographic coordinate systems 
	/// use degrees.
    /// </remarks>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class GeographicCoordinateSystem : HorizontalCoordinateSystem, IGeographicCoordinateSystem
	{

		/// <summary>
		/// Creates an instance of a Geographic Coordinate System
		/// </summary>
		/// <param name="angularUnit">Angular units</param>
		/// <param name="horizontalDatum">Horizontal datum</param>
		/// <param name="primeMeridian">Prime meridian</param>
		/// <param name="axisInfo">Axis info</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="authorityCode">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal GeographicCoordinateSystem(IAngularUnit angularUnit, IHorizontalDatum horizontalDatum, IPrimeMeridian primeMeridian, List<AxisInfo> axisInfo, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
			:
			base(horizontalDatum, axisInfo, name, authority, authorityCode, alias, abbreviation, remarks)
		{
			_AngularUnit = angularUnit;
			_PrimeMeridian = primeMeridian;
		}

		#region Predefined geographic coordinate systems

		/// <summary>
		/// Creates a decimal degrees geographic coordinate system based on the WGS84 ellipsoid, suitable for GPS measurements
		/// </summary>
		public static IGeographicCoordinateSystem WGS84
		{
			get {
				var axes = new List<AxisInfo>(2);
				axes.Add(new AxisInfo("Lon", AxisOrientationEnum.East));
				axes.Add(new AxisInfo("Lat", AxisOrientationEnum.North));
				return new GeographicCoordinateSystem(CoordinateSystems.AngularUnit.Degrees,
					CoordinateSystems.HorizontalDatum.WGS84, CoordinateSystems.PrimeMeridian.Greenwich, axes,
					"WGS 84", "EPSG", 4326, String.Empty, string.Empty, string.Empty);
			}
		}
        
        #endregion

		#region IGeographicCoordinateSystem Members
		
		private IAngularUnit _AngularUnit;

		/// <summary>
		/// Gets or sets the angular units of the geographic coordinate system.
		/// </summary>
		public IAngularUnit AngularUnit
		{
			get { return _AngularUnit; }
			set { _AngularUnit = value; }
		}
		
		/// <summary>
		/// Gets units for dimension within coordinate system. Each dimension in 
		/// the coordinate system has corresponding units.
		/// </summary>
		/// <param name="dimension">Dimension</param>
		/// <returns>Unit</returns>
		public override IUnit GetUnits(int dimension)
		{
			return _AngularUnit;
		}
		private IPrimeMeridian _PrimeMeridian;

		/// <summary>
		/// Gets or sets the prime meridian of the geographic coordinate system.
		/// </summary>
		public IPrimeMeridian PrimeMeridian
		{
			get { return _PrimeMeridian; }
			set { _PrimeMeridian = value; }
		}
			
		/// <summary>
		/// Gets the number of available conversions to WGS84 coordinates.
		/// </summary>
		public int NumConversionToWGS84
		{
			get { return _WGS84ConversionInfo.Count; }
		}

		private List<Wgs84ConversionInfo> _WGS84ConversionInfo;
		
		internal List<Wgs84ConversionInfo> WGS84ConversionInfo
		{
			get { return _WGS84ConversionInfo; }
			set { _WGS84ConversionInfo = value; }
		}

		/// <summary>
		/// Gets details on a conversion to WGS84.
		/// </summary>
		public Wgs84ConversionInfo GetWgs84ConversionInfo(int index)
		{
			return _WGS84ConversionInfo[index];
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
				sb.AppendFormat("GEOGCS[\"{0}\", {1}, {2}, {3}",Name, HorizontalDatum.WKT, PrimeMeridian.WKT, AngularUnit.WKT);
				//Skip axis info if they contain default values
				if (AxisInfo.Count != 2 ||
					AxisInfo[0].Name != "Lon" || AxisInfo[0].Orientation != AxisOrientationEnum.East ||
					AxisInfo[1].Name != "Lat" || AxisInfo[1].Orientation != AxisOrientationEnum.North)
					for (int i = 0; i < AxisInfo.Count; i++)
						sb.AppendFormat(", {0}", GetAxis(i).WKT);
				if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
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
					"<CS_CoordinateSystem Dimension=\"{0}\"><CS_GeographicCoordinateSystem>{1}",
					this.Dimension, InfoXml);
				foreach(AxisInfo ai in this.AxisInfo)
					sb.Append(ai.XML);
				sb.AppendFormat("{0}{1}{2}</CS_GeographicCoordinateSystem></CS_CoordinateSystem>",
					HorizontalDatum.XML, AngularUnit.XML, PrimeMeridian.XML);
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
			if (!(obj is GeographicCoordinateSystem))
				return false;
			GeographicCoordinateSystem gcs = obj as GeographicCoordinateSystem;
			if (gcs.Dimension != this.Dimension) return false;
			if (this.WGS84ConversionInfo != null && gcs.WGS84ConversionInfo == null) return false;
			if (this.WGS84ConversionInfo == null && gcs.WGS84ConversionInfo != null) return false;
			if (this.WGS84ConversionInfo != null && gcs.WGS84ConversionInfo != null)
			{
				if (this.WGS84ConversionInfo.Count != gcs.WGS84ConversionInfo.Count) return false;
				for (int i = 0; i < this.WGS84ConversionInfo.Count; i++)
					if (!gcs.WGS84ConversionInfo[i].Equals(this.WGS84ConversionInfo[i]))
						return false;
			}
			if (this.AxisInfo.Count != gcs.AxisInfo.Count) return false;			
			for (int i = 0; i < gcs.AxisInfo.Count; i++)
				if (gcs.AxisInfo[i].Orientation != this.AxisInfo[i].Orientation)
					return false;
			return gcs.AngularUnit.EqualParams(this.AngularUnit) &&
					gcs.HorizontalDatum.EqualParams(this.HorizontalDatum) &&
					gcs.PrimeMeridian.EqualParams(this.PrimeMeridian);
		}
		#endregion
	}
}
