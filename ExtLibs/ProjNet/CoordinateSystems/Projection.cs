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
	/// The Projection class defines the standard information stored with a projection
	/// objects. A projection object implements a coordinate transformation from a geographic
	/// coordinate system to a projected coordinate system, given the ellipsoid for the
	/// geographic coordinate system. It is expected that each coordinate transformation of
	/// interest, e.g., Transverse Mercator, Lambert, will be implemented as a class of
	/// type Projection, supporting the IProjection interface.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class Projection : Info, IProjection
	{
		internal Projection(string className, List<ProjectionParameter> parameters,
			string name, string authority, long code, string alias, 
			string remarks, string abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_parameters = parameters;
			_ClassName = className;
		}

		#region Predefined projections
		#endregion

		#region IProjection Members

		/// <summary>
		/// Gets the number of parameters of the projection.
		/// </summary>
		public int NumParameters
		{
			get { return _parameters.Count; }
		}

		private List<ProjectionParameter> _parameters;

		/// <summary>
		/// Gets or sets the parameters of the projection
		/// </summary>
		internal List<ProjectionParameter> Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		/// <summary>
		/// Gets an indexed parameter of the projection.
		/// </summary>
		/// <param name="index">Index of parameter</param>
		/// <returns>n'th parameter</returns>
		public ProjectionParameter GetParameter(int index)
		{
			return _parameters[index];
		}

		/// <summary>
		/// Gets an named parameter of the projection.
		/// </summary>
		/// <remarks>The parameter name is case insensitive</remarks>
		/// <param name="name">Name of parameter</param>
		/// <returns>parameter or null if not found</returns>
		public ProjectionParameter GetParameter(string name)
		{
			foreach (ProjectionParameter par in _parameters)
				if (par.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					return par;
			return null;
		}
				
		private string _ClassName;

		/// <summary>
		/// Gets the projection classification name (e.g. "Transverse_Mercator").
		/// </summary>
		public string ClassName
		{
			get { return _ClassName; }
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
				sb.AppendFormat("PROJECTION[\"{0}\"", Name);
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
				sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "<CS_Projection Classname=\"{0}\">{1}", ClassName, InfoXml);
				foreach (ProjectionParameter param in Parameters)
					sb.Append(param.XML);
				sb.Append("</CS_Projection>");
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
			if (!(obj is Projection))
				return false;
			Projection proj = obj as Projection;
			if (proj.NumParameters != this.NumParameters)
				return false;
			for (int i = 0; i < _parameters.Count; i++)
			{
				ProjectionParameter param = GetParameter(proj.GetParameter(i).Name);
				if (param == null)
					return false;
				if (param.Value != proj.GetParameter(i).Value)
					return false;
			}
			return true;
		}

		#endregion
	}
}
