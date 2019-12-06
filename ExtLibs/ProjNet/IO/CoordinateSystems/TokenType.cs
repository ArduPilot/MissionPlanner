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

#region Using



#endregion

namespace ProjNet.Converters.WellKnownText
{
	/// <summary>
	/// Represents the type of token created by the StreamTokenizer class.
	/// </summary>
	internal enum TokenType
	{
		/// <summary>
		/// Indicates that the token is a word.
		/// </summary>
		Word,
		/// <summary>
		/// Indicates that the token is a number. 
		/// </summary>
		Number,
		/// <summary>
		/// Indicates that the end of line has been read. The field can only have this value if the eolIsSignificant method has been called with the argument true. 
		/// </summary>
		Eol,
		/// <summary>
		/// Indicates that the end of the input stream has been reached.
		/// </summary>
		Eof,
		/// <summary>
		/// Indictaes that the token is white space (space, tab, newline).
		/// </summary>
		Whitespace,
		/// <summary>
		/// Characters that are not whitespace, numbers, etc...
		/// </summary>
		Symbol
	}
}
