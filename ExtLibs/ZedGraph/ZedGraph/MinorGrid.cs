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
using System.Drawing.Drawing2D;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// Class that holds the specific properties for the minor grid.
	/// </summary>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class MinorGrid : ICloneable, ISerializable
	{
		internal bool	_isVisible;

		internal float _dashOn,
							_dashOff,
							_penWidth;

		internal Color _color;


	#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public MinorGrid()
		{
			_dashOn = Default.DashOn;
			_dashOff = Default.DashOff;
			_penWidth = Default.PenWidth;
			_isVisible = Default.IsVisible;
			_color = Default.Color;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">The source <see cref="MinorGrid" /> to be copied.</param>
		public MinorGrid( MinorGrid rhs )
		{
			_dashOn = rhs._dashOn;
			_dashOff = rhs._dashOff;
			_penWidth = rhs._penWidth;

			_isVisible = rhs._isVisible;

			_color = rhs._color;
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
		public MinorGrid Clone()
		{
			return new MinorGrid( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a value that determines if the major <see cref="Axis"/> gridlines
		/// (at each labeled value) will be visible
		/// </summary>
		/// <value>true to show the gridlines, false otherwise</value>
		/// <seealso cref="Default.IsVisible">Default.IsShowGrid</seealso>.
		/// <seealso cref="Color"/>
		/// <seealso cref="PenWidth"/>
		/// <seealso cref="DashOn"/>
		/// <seealso cref="DashOff"/>
		/// <seealso cref="IsVisible"/>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		/// <summary>
		/// The "Dash On" mode for drawing the grid.
		/// </summary>
		/// <remarks>
		/// This is the distance,
		/// in points (1/72 inch), of the dash segments that make up the dashed grid lines.
		/// </remarks>
		/// <value>The dash on length is defined in points (1/72 inch)</value>
		/// <seealso cref="DashOff"/>
		/// <seealso cref="IsVisible"/>
		/// <seealso cref="Default.DashOn"/>.
		public float DashOn
		{
			get { return _dashOn; }
			set { _dashOn = value; }
		}
		/// <summary>
		/// The "Dash Off" mode for drawing the grid.
		/// </summary>
		/// <remarks>
		/// This is the distance,
		/// in points (1/72 inch), of the spaces between the dash segments that make up
		/// the dashed grid lines.
		/// </remarks>
		/// <value>The dash off length is defined in points (1/72 inch)</value>
		/// <seealso cref="DashOn"/>
		/// <seealso cref="IsVisible"/>
		/// <seealso cref="Default.DashOff"/>.
		public float DashOff
		{
			get { return _dashOff; }
			set { _dashOff = value; }
		}
		/// <summary>
		/// The pen width used for drawing the grid lines.
		/// </summary>
		/// <value>The grid pen width is defined in points (1/72 inch)</value>
		/// <seealso cref="IsVisible"/>
		/// <seealso cref="Default.PenWidth"/>.
		/// <seealso cref="Color"/>
		public float PenWidth
		{
			get { return _penWidth; }
			set { _penWidth = value; }
		}
		/// <summary>
		/// The color to use for drawing this <see cref="Axis"/> grid.
		/// </summary>
		/// <value> The color is defined using the
		/// <see cref="System.Drawing.Color"/> class</value>
		/// <seealso cref="Default.Color"/>.
		/// <seealso cref="PenWidth"/>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
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
		protected MinorGrid( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_isVisible = info.GetBoolean( "isVisible" );

			_dashOn = info.GetSingle( "dashOn" );
			_dashOff = info.GetSingle( "dashOff" );
			_penWidth = info.GetSingle( "penWidth" );

			_color = (Color)info.GetValue( "color", typeof( Color ) );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );

			info.AddValue( "isVisible", _isVisible );

			info.AddValue( "dashOn", _dashOn );
			info.AddValue( "dashOff", _dashOff );
			info.AddValue( "penWidth", _penWidth );

			info.AddValue( "color", _color );
		}

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="MinorGrid"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default "dash on" size for drawing the <see cref="Axis"/> minor grid
			/// (<see cref="MinorGrid.DashOn"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOn = 1.0F;
			/// <summary>
			/// The default "dash off" size for drawing the <see cref="Axis"/> minor grid
			/// (<see cref="MinorGrid.DashOff"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOff = 10.0F;
			/// <summary>
			/// The default pen width for drawing the <see cref="Axis"/> minor grid
			/// (<see cref="MinorGrid.PenWidth"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float PenWidth = 1.0F;
			/// <summary>
			/// The default color for the <see cref="Axis"/> minor grid lines
			/// (<see cref="MinorGrid.Color"/> property).  This color only affects the
			/// minor grid lines.
			/// </summary>
			public static Color Color = Color.Gray;

			/// <summary>
			/// The default display mode for the <see cref="Axis"/> minor grid lines
			/// (<see cref="MinorGrid.IsVisible"/> property). true
			/// to show the minor grid lines, false to hide them.
			/// </summary>
			public static bool IsVisible = false;

		}

	#endregion

	#region Methods

		internal void Draw( Graphics g, Pen pen, float pixVal, float topPix )
		{
			// draw the minor grid
			if ( _isVisible )
				g.DrawLine( pen, pixVal, 0.0F, pixVal, topPix );
		}

		internal Pen GetPen( GraphPane pane, float scaleFactor )
		{
			Pen pen = new Pen( _color,
						pane.ScaledPenWidth( _penWidth, scaleFactor ) );

			if ( _dashOff > 1e-10 && _dashOn > 1e-10 )
			{
				pen.DashStyle = DashStyle.Custom;
				float[] pattern = new float[2];
				pattern[0] = _dashOn;
				pattern[1] = _dashOff;
				pen.DashPattern = pattern;
			}

			return pen;
		}

	#endregion

	}
}
