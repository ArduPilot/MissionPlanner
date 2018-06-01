//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// Class that handles the data associated with text title and its associated font
	/// properties
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.2 $ $Date: 2007-03-11 02:08:16 $ </version>
	[Serializable]
	public class Label : ICloneable, ISerializable
	{
		/// <summary>
		/// private field that stores the <see cref="string" /> text for this label
		/// </summary>
		internal string _text;
		/// <summary>
		/// private field that stores the <see cref="FontSpec" /> font properties for this label
		/// </summary>
		internal FontSpec _fontSpec;
		/// <summary>
		/// private field that determines if this label will be displayed.
		/// </summary>
		internal bool _isVisible;

	#region Constructors

		/// <summary>
		/// Constructor to build an <see cref="AxisLabel" /> from the text and the
		/// associated font properties.
		/// </summary>
		/// <param name="text">The <see cref="string" /> representing the text to be
		/// displayed</param>
		/// <param name="fontFamily">The <see cref="String" /> font family name</param>
		/// <param name="fontSize">The size of the font in points and scaled according
		/// to the <see cref="PaneBase.CalcScaleFactor" /> logic.</param>
		/// <param name="color">The <see cref="Color" /> instance representing the color
		/// of the font</param>
		/// <param name="isBold">true for a bold font face</param>
		/// <param name="isItalic">true for an italic font face</param>
		/// <param name="isUnderline">true for an underline font face</param>
		public Label( string text, string fontFamily, float fontSize, Color color, bool isBold,
			bool isItalic, bool isUnderline )
		{
			_text = ( text == null ) ? string.Empty : text;

			_fontSpec = new FontSpec( fontFamily, fontSize, color, isBold, isItalic, isUnderline );
			_isVisible = true;
		}

		/// <summary>
		/// Constructor that builds a <see cref="Label" /> from a text <see cref="string" />
		/// and a <see cref="FontSpec" /> instance.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="fontSpec"></param>
		public Label( string text, FontSpec fontSpec )
		{
			_text = (text == null) ? string.Empty : text;

			_fontSpec = fontSpec;
			_isVisible = true;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">the <see cref="Label" /> instance to be copied.</param>
		public Label( Label rhs )
		{
			if (rhs._text != null)
				_text = (string)rhs._text.Clone();
			else
				_text = string.Empty;

			_isVisible = rhs._isVisible;
			if ( rhs._fontSpec != null )
				_fontSpec = rhs._fontSpec.Clone();
			else
				_fontSpec = null;
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" />
		/// </summary>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Typesafe, deep-copy clone method.
		/// </summary>
		/// <returns>A new, independent copy of this class</returns>
		public Label Clone()
		{
			return new Label( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// The <see cref="String" /> text to be displayed
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		/// A <see cref="ZedGraph.FontSpec" /> instance representing the font properties
		/// for the displayed text.
		/// </summary>
		public FontSpec FontSpec
		{
			get { return _fontSpec; }
			set { _fontSpec = value; }
		}

		/// <summary>
		/// Gets or sets a boolean value that determines whether or not this label will be displayed.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected Label( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_text = info.GetString( "text" );
			_isVisible = info.GetBoolean( "isVisible" );
			_fontSpec = (FontSpec) info.GetValue( "fontSpec", typeof( FontSpec ) );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );
			info.AddValue( "text", _text );
			info.AddValue( "isVisible", _isVisible );
			info.AddValue( "fontSpec", _fontSpec );
		}
	#endregion


	}
}
