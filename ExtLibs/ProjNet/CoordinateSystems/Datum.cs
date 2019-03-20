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
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// A set of quantities from which other quantities are calculated.
	/// </summary>
	/// <remarks>
	/// For the OGC abstract model, it can be defined as a set of real points on the earth 
	/// that have coordinates. EG. A datum can be thought of as a set of parameters 
	/// defining completely the origin and orientation of a coordinate system with respect 
	/// to the earth. A textual description and/or a set of parameters describing the 
	/// relationship of a coordinate system to some predefined physical locations (such 
	/// as center of mass) and physical directions (such as axis of spin). The definition 
	/// of the datum may also include the temporal behavior (such as the rate of change of
	/// the orientation of the coordinate axes).
    /// </remarks>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public abstract class Datum : Info, IDatum
	{
		/// <summary>
		/// Initializes a new instance of a Datum object
		/// </summary>
		/// <param name="type">Datum type</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal Datum(DatumType type,
			string name, string authority, long code, string alias,
			string remarks, string abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_DatumType = type;
		}
		#region IDatum Members

		private DatumType _DatumType;

		/// <summary>
		/// Gets or sets the type of the datum as an enumerated code.
		/// </summary>
		public DatumType DatumType
		{
			get { return _DatumType; }
			set { _DatumType = value; }
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
			if (!(obj is Ellipsoid))
				return false;
			return (obj as Datum).DatumType == this.DatumType;
		}
	}
}
