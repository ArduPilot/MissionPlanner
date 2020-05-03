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
using System.Text;
using GeoAPI.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// The Info object defines the standard information
	/// stored with spatial reference objects
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public abstract class Info : IInfo
	{
		/// <summary>
		/// A base interface for metadata applicable to coordinate system objects.
		/// </summary>
		/// <remarks>
		/// <para>The metadata items ‘Abbreviation’, ‘Alias’, ‘Authority’, ‘AuthorityCode’, ‘Name’ and ‘Remarks’ 
		/// were specified in the Simple Features interfaces, so they have been kept here.</para>
		/// <para>This specification does not dictate what the contents of these items 
		/// should be. However, the following guidelines are suggested:</para>
		/// <para>When <see cref="ICoordinateSystemAuthorityFactory"/> is used to create an object, the ‘Authority’
		/// and 'AuthorityCode' values should be set to the authority name of the factory object, and the authority 
		/// code supplied by the client, respectively. The other values may or may not be set. (If the authority is 
		/// EPSG, the implementer may consider using the corresponding metadata values in the EPSG tables.)</para>
		/// <para>When <see cref="CoordinateSystemFactory"/> creates an object, the 'Name' should be set to the value
		/// supplied by the client. All of the other metadata items should be left empty</para>
		/// </remarks>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
		internal Info(
						string name, 
						string authority, 
						long code, 
						string alias, 
						string abbreviation, 
						string remarks)
		{
			_Name = name;
			_Authority = authority;
			_Code = code;
			_Alias = alias;
			_Abbreviation = abbreviation;
			_Remarks = remarks;
		}

		#region ISpatialReferenceInfo Members

		private string _Name;

		/// <summary>
		/// Gets or sets the name of the object.
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private string _Authority;

		/// <summary>
		/// Gets or sets the authority name for this object, e.g., "EPSG",
		/// is this is a standard object with an authority specific
		/// identity code. Returns "CUSTOM" if this is a custom object.
		/// </summary>
		public string Authority
		{
			get { return _Authority; }
			set { _Authority = value; }
		}

		private long _Code;

		/// <summary>
		/// Gets or sets the authority specific identification code of the object
		/// </summary>
		public long AuthorityCode
		{
			get { return _Code; }
			set { _Code = value; }
		}

		private string _Alias;

		/// <summary>
		/// Gets or sets the alias of the object.
		/// </summary>
		public string Alias
		{
			get { return _Alias; }
			set { _Alias = value; }
		}

		private string _Abbreviation;

		/// <summary>
		/// Gets or sets the abbreviation of the object.
		/// </summary>
		public string Abbreviation
		{
			get { return _Abbreviation; }
			set { _Abbreviation = value; }
		}

		private string _Remarks;

		/// <summary>
		/// Gets or sets the provider-supplied remarks for the object.
		/// </summary>
		public string Remarks
		{
			get { return _Remarks; }
			set { _Remarks = value; }
		}

		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification.
		/// </summary>
		public override string ToString()
		{
			return WKT;
		}
		
		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification.
		/// </summary>
		public abstract string WKT {get ;}
		
		/// <summary>
		/// Gets an XML representation of this object.
		/// </summary>
		public abstract string XML { get; }

		/// <summary>
		/// Returns an XML string of the info object
		/// </summary>
		internal string InfoXml
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("<CS_Info");
				if(AuthorityCode>0) sb.AppendFormat(" AuthorityCode=\"{0}\"",AuthorityCode);
				if (!String.IsNullOrEmpty(Abbreviation)) sb.AppendFormat(" Abbreviation=\"{0}\"", Abbreviation);
				if (!String.IsNullOrEmpty(Authority)) sb.AppendFormat(" Authority=\"{0}\"", Authority);
				if (!String.IsNullOrEmpty(Name)) sb.AppendFormat(" Name=\"{0}\"", Name);
				sb.Append("/>");
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
		public abstract bool EqualParams(object obj);

		#endregion
	}
}
