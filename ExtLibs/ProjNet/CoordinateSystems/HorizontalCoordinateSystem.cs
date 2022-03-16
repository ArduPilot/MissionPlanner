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
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// A 2D coordinate system suitable for positions on the Earth's surface.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public abstract class HorizontalCoordinateSystem : CoordinateSystem, IHorizontalCoordinateSystem
	{
		/// <summary>
		/// Creates an instance of HorizontalCoordinateSystem
		/// </summary>
		/// <param name="datum">Horizontal datum</param>
		/// <param name="axisInfo">Axis information</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal HorizontalCoordinateSystem(IHorizontalDatum datum, List<AxisInfo> axisInfo, 
			string name, string authority, long code, string alias,
			string remarks, string abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_HorizontalDatum = datum;
			if (axisInfo.Count != 2)
				throw new ArgumentException("Axis info should contain two axes for horizontal coordinate systems");
			base.AxisInfo = axisInfo;
		}

		#region IHorizontalCoordinateSystem Members

		private IHorizontalDatum _HorizontalDatum;

		/// <summary>
		/// Gets or sets the HorizontalDatum.
		/// </summary>
		public IHorizontalDatum HorizontalDatum
		{
			get { return _HorizontalDatum; }
			set { _HorizontalDatum = value; }
		}
		
		#endregion
	}
}
