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
	/// Horizontal datum defining the standard datum information.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class HorizontalDatum : Datum, IHorizontalDatum
	{
		/// <summary>
		/// Initializes a new instance of a horizontal datum
		/// </summary>
		/// <param name="ellipsoid">Ellipsoid</param>
		/// <param name="toWgs84">Parameters for a Bursa Wolf transformation into WGS84</param>
		/// <param name="type">Datum type</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal HorizontalDatum(
			IEllipsoid ellipsoid, Wgs84ConversionInfo toWgs84, DatumType type,
			string name, string authority, long code, string alias, string remarks, string abbreviation)
			: base(type, name, authority, code, alias, remarks, abbreviation)
		{
			_Ellipsoid = ellipsoid;
			_Wgs84ConversionInfo = toWgs84;
		}

		#region Predefined datums
		/// <summary>
		/// EPSG's WGS 84 datum has been the then current realisation. No distinction is made between the original WGS 84 
		/// frame, WGS 84 (G730), WGS 84 (G873) and WGS 84 (G1150). Since 1997, WGS 84 has been maintained within 10cm of 
		/// the then current ITRF.
		/// </summary>
		/// <remarks>
		/// <para>Area of use: World</para>
		/// <para>Origin description: Defined through a consistent set of station coordinates. These have changed with time: by 0.7m 
		/// on 29/6/1994 [WGS 84 (G730)], a further 0.2m on 29/1/1997 [WGS 84 (G873)] and a further 0.06m on 
		/// 20/1/2002 [WGS 84 (G1150)].</para>
		/// </remarks>
		public static HorizontalDatum WGS84
		{
			get
			{
                return new HorizontalDatum(CoordinateSystems.Ellipsoid.WGS84,
                    null, DatumType.HD_Geocentric, "World Geodetic System 1984", "EPSG", 6326, String.Empty,
                    "EPSG's WGS 84 datum has been the then current realisation. No distinction is made between the original WGS 84 frame, WGS 84 (G730), WGS 84 (G873) and WGS 84 (G1150). Since 1997, WGS 84 has been maintained within 10cm of the then current ITRF.", String.Empty);
            }
		}

		/// <summary>
		/// World Geodetic System 1972
		/// </summary>
		/// <remarks>
		/// <para>Used by GPS before 1987. For Transit satellite positioning see also WGS 72BE. Datum code 6323 reserved for southern hemisphere ProjCS's.</para>
		/// <para>Area of use: World</para>
		/// <para>Origin description: Developed from a worldwide distribution of terrestrial and
		/// geodetic satellite observations and defined through a set of station coordinates.</para>
		/// </remarks>
		public static HorizontalDatum WGS72
		{
			get
			{
				HorizontalDatum datum =
					new HorizontalDatum(CoordinateSystems.Ellipsoid.WGS72,
					null, DatumType.HD_Geocentric, "World Geodetic System 1972", "EPSG", 6322, String.Empty,
					"Used by GPS before 1987. For Transit satellite positioning see also WGS 72BE. Datum code 6323 reserved for southern hemisphere ProjCS's.", String.Empty);
				datum.Wgs84Parameters = new Wgs84ConversionInfo(0, 0, 4.5, 0, 0, 0.554, 0.219);
				return datum;
			}
		}


		/// <summary>
		/// European Terrestrial Reference System 1989
		/// </summary>
		/// <remarks>
		/// <para>Area of use: 
		/// Europe: Albania; Andorra; Austria; Belgium; Bosnia and Herzegovina; Bulgaria; Croatia; 
		/// Cyprus; Czech Republic; Denmark; Estonia; Finland; Faroe Islands; France; Germany; Greece; 
		/// Hungary; Ireland; Italy; Latvia; Liechtenstein; Lithuania; Luxembourg; Malta; Netherlands; 
		/// Norway; Poland; Portugal; Romania; San Marino; Serbia and Montenegro; Slovakia; Slovenia; 
		/// Spain; Svalbard; Sweden; Switzerland; United Kingdom (UK) including Channel Islands and 
		/// Isle of Man; Vatican City State.</para>
		/// <para>Origin description: Fixed to the stable part of the Eurasian continental 
		/// plate and consistent with ITRS at the epoch 1989.0.</para>
		/// </remarks>
		public static HorizontalDatum ETRF89
		{
			get
			{
				HorizontalDatum datum = new HorizontalDatum(CoordinateSystems.Ellipsoid.GRS80, null, DatumType.HD_Geocentric,
					"European Terrestrial Reference System 1989", "EPSG", 6258, "ETRF89", "The distinction in usage between ETRF89 and ETRS89 is confused: although in principle conceptually different in practice both are used for the realisation.", String.Empty);
				datum.Wgs84Parameters = new Wgs84ConversionInfo();
				return datum;
			}
		}

		/// <summary>
		/// European Datum 1950
		/// </summary>
		/// <remarks>
		/// <para>Area of use:
		/// Europe - west - Denmark; Faroe Islands; France offshore; Israel offshore; Italy including San 
		/// Marino and Vatican City State; Ireland offshore; Netherlands offshore; Germany; Greece (offshore);
		/// North Sea; Norway; Spain; Svalbard; Turkey; United Kingdom UKCS offshore. Egypt - Western Desert.
		/// </para>
		/// <para>Origin description: Fundamental point: Potsdam (Helmert Tower). 
		/// Latitude: 52 deg 22 min 51.4456 sec N; Longitude: 13 deg  3 min 58.9283 sec E (of Greenwich).</para>
		/// </remarks>
		public static HorizontalDatum ED50
		{
			get
			{
				return new HorizontalDatum(CoordinateSystems.Ellipsoid.International1924, new Wgs84ConversionInfo(-87, -98, -121, 0, 0, 0, 0), DatumType.HD_Geocentric,
				"European Datum 1950", "EPSG", 6230, "ED50", String.Empty, String.Empty);
			}
		}
		#endregion

		#region IHorizontalDatum Members

		IEllipsoid _Ellipsoid;

		/// <summary>
		/// Gets or sets the ellipsoid of the datum
		/// </summary>
		public IEllipsoid Ellipsoid
		{
			get { return _Ellipsoid; }
			set { _Ellipsoid = value; }
		}

		Wgs84ConversionInfo _Wgs84ConversionInfo;
		/// <summary>
		/// Gets preferred parameters for a Bursa Wolf transformation into WGS84
		/// </summary>
		public Wgs84ConversionInfo Wgs84Parameters
		{
			get { return _Wgs84ConversionInfo; }
			set { _Wgs84ConversionInfo = value; }
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
				sb.AppendFormat("DATUM[\"{0}\", {1}", Name, _Ellipsoid.WKT);
				if (_Wgs84ConversionInfo != null)
					sb.AppendFormat(", {0}", _Wgs84ConversionInfo.WKT);
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
				return String.Format(CultureInfo.InvariantCulture.NumberFormat,
					"<CS_HorizontalDatum DatumType=\"{0}\">{1}{2}{3}</CS_HorizontalDatum>",
					(int)DatumType, InfoXml, Ellipsoid.XML, (Wgs84Parameters == null ? String.Empty : Wgs84Parameters.XML));
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
			if (!(obj is HorizontalDatum))
				return false;
			HorizontalDatum datum = obj as HorizontalDatum;
			if (datum.Wgs84Parameters == null && this.Wgs84Parameters != null) return false;
			if (datum.Wgs84Parameters != null && !datum.Wgs84Parameters.Equals(this.Wgs84Parameters))
				return false;
			return (datum != null && this.Ellipsoid != null &&
				datum.Ellipsoid.EqualParams(this.Ellipsoid) || datum == null && this.Ellipsoid == null) && this.DatumType == datum.DatumType;
		}
	}
}
