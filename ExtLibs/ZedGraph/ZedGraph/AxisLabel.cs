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
	/// properties.  Inherits from <see cref="Label" />, and adds <see cref="IsOmitMag" />
	/// and <see cref="IsTitleAtCross" /> properties, which are specifically associated with
	/// the <see cref="Axis" /> <see cref="Axis.Title" />.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class AxisLabel : GapLabel, ICloneable, ISerializable
	{
		internal bool	_isOmitMag,
							_isTitleAtCross;

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
		public AxisLabel( string text, string fontFamily, float fontSize, Color color, bool isBold,
								bool isItalic, bool isUnderline ) :
			base( text, fontFamily, fontSize, color, isBold, isItalic, isUnderline )
		{
			_isOmitMag = false;
			_isTitleAtCross = true;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">the <see cref="AxisLabel" /> instance to be copied.</param>
		public AxisLabel( AxisLabel rhs )
			: base( rhs )
		{
			_isOmitMag = rhs._isOmitMag;
			_isTitleAtCross = rhs._isTitleAtCross;
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
		public new AxisLabel Clone()
		{
			return new AxisLabel( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets the property that controls whether or not the magnitude factor (power of 10) for
		/// this scale will be included in the label.
		/// </summary>
		/// <remarks>
		/// For large scale values, a "magnitude" value (power of 10) is automatically
		/// used for scaling the graph.  This magnitude value is automatically appended
		/// to the end of the Axis <see cref="Axis.Title"/> (e.g., "(10^4)") to indicate
		/// that a magnitude is in use.  This property controls whether or not the
		/// magnitude is included in the title.  Note that it only affects the axis
		/// title; a magnitude value may still be used even if it is not shown in the title.
		/// </remarks>
		/// <value>true to show the magnitude value, false to hide it</value>
		/// <seealso cref="Axis.Title"/>
		/// <seealso cref="Scale.Mag"/>
		/// <seealso cref="Scale.Format"/>
		public bool IsOmitMag
		{
			get { return _isOmitMag; }
			set { _isOmitMag = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether the Axis title is located at the
		/// <see cref="Axis.Cross" />
		/// value or at the normal position (outside the <see cref="Chart.Rect" />).
		/// </summary>
		/// <remarks>
		/// This value only applies if <see cref="Axis.CrossAuto" /> is false.
		/// </remarks>
		public bool IsTitleAtCross
		{
			get { return _isTitleAtCross; }
			set { _isTitleAtCross = value; }
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema3 = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected AxisLabel( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch2 = info.GetInt32( "schema3" );

			_isOmitMag = info.GetBoolean( "isOmitMag" );
			_isTitleAtCross = info.GetBoolean( "isTitleAtCross" );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );

			info.AddValue( "schema3", schema2 );
			info.AddValue( "isOmitMag", _isVisible );
			info.AddValue( "isTitleAtCross", _isTitleAtCross );
		}
		#endregion


	}
}
