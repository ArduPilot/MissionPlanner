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
	/// Class that handles the data associated with the major grid lines on the chart.
	/// Inherits from <see cref="MinorGrid" />.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class MajorGrid : MinorGrid, ICloneable, ISerializable
	{
		internal bool _isZeroLine;

	#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public MajorGrid()
		{
			_dashOn = Default.DashOn;
			_dashOff = Default.DashOff;
			_penWidth = Default.PenWidth;
			_isVisible = Default.IsVisible;
			_color = Default.Color;
            _isZeroLine = Default.IsZeroLine;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">The source <see cref="MajorGrid" /> to be copied.</param>
		public MajorGrid( MajorGrid rhs ) : base( rhs )
		{
			_isZeroLine = rhs._isZeroLine;
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
		public new MajorGrid Clone()
		{
			return new MajorGrid( this );
		}

		#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a boolean value that determines if a line will be drawn at the
		/// zero value for the axis.
		/// </summary>
		/// <remarks>
		/// The zero line is a line that divides the negative values from the positive values.
		/// The default is set according to
		/// <see cref="XAxis.Default.IsEmptyLine"/>, <see cref="YAxis.Default.IsEmptyLine"/>,
		/// <see cref="Y2Axis.Default.IsEmptyLine"/>,
		/// </remarks>
		/// <value>true to show the zero line, false otherwise</value>
		public bool IsZeroLine
		{
			get { return _isZeroLine; }
			set { _isZeroLine = value; }
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema2 = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected MajorGrid( SerializationInfo info, StreamingContext context ) :
			base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			_isZeroLine = info.GetBoolean( "isZeroLine" );
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

			info.AddValue( "schema2", schema2 );

			info.AddValue( "isZeroLine", _isZeroLine );
		}

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="MajorGrid"/> class.
		/// </summary>
		public new struct Default
		{
			/// <summary>
			/// The default "dash on" size for drawing the <see cref="Axis"/> grid
			/// (<see cref="MinorGrid.DashOn"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOn = 1.0F;
			/// <summary>
			/// The default "dash off" size for drawing the <see cref="Axis"/> grid
			/// (<see cref="MinorGrid.DashOff"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOff = 5.0F;
			/// <summary>
			/// The default pen width for drawing the <see cref="Axis"/> grid
			/// (<see cref="MinorGrid.PenWidth"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float PenWidth = 1.0F;
			/// <summary>
			/// The default color for the <see cref="Axis"/> grid lines
			/// (<see cref="MinorGrid.Color"/> property).  This color only affects the
			/// grid lines.
			/// </summary>
			public static Color Color = Color.Black;

			/// <summary>
			/// The default display mode for the <see cref="Axis"/> grid lines
			/// (<see cref="MinorGrid.IsVisible"/> property). true
			/// to show the grid lines, false to hide them.
			/// </summary>
			public static bool IsVisible = false;

			/// <summary>
			/// The default boolean value that determines if a line will be drawn at the
			/// zero value for the axis.
			/// </summary>
			/// <remarks>
			/// The zero line is a line that divides the negative values from the positive values.
			/// The default is set according to
			/// <see cref="XAxis.Default.IsEmptyLine"/>, <see cref="YAxis.Default.IsEmptyLine"/>,
			/// <see cref="Y2Axis.Default.IsEmptyLine"/>,
			/// </remarks>
			/// <value>true to show the zero line, false otherwise</value>
			public static bool IsZeroLine = false;
		}

	#endregion

	}
}
