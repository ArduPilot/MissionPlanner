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
	/// Class for defining units
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class Unit : Info, IUnit
    {
		/// <summary>
		/// Initializes a new unit
		/// </summary>
		/// <param name="conversionFactor">Conversion factor to base unit</param>
		/// <param name="name">Name of unit</param>
		/// <param name="authority">Authority name</param>
		/// <param name="authorityCode">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal Unit(double conversionFactor, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
			:
			base(name, authority, authorityCode, alias, abbreviation, remarks)
		{
			_ConversionFactor = conversionFactor;
		}

		/// <summary>
		/// Initializes a new unit
		/// </summary>
		/// <param name="name">Name of unit</param>
		/// <param name="conversionFactor">Conversion factor to base unit</param>
		internal Unit(string name, double conversionFactor)
			: this(conversionFactor, name, String.Empty, -1, String.Empty, String.Empty, String.Empty)
		{
		}

		private double _ConversionFactor;

		/// <summary>
		/// Gets or sets the number of units per base-unit.
		/// </summary>
		public double ConversionFactor
		{
			get { return _ConversionFactor; }
			set { _ConversionFactor = value; }
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
				sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "UNIT[\"{0}\", {1}", Name, _ConversionFactor);
				if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
					sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
				sb.Append("]");
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets an XML representation of this object [NOT IMPLEMENTED].
		/// </summary>
		public override string XML
		{
			get
			{
				throw new NotImplementedException();
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
			if (!(obj is Unit))
				return false;
			return (obj as Unit).ConversionFactor == this.ConversionFactor;
		}
    }
}
