//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2007  John Champion
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// A class that handles the basic attributes of a line segment.
	/// </summary>
	/// <remarks>
	/// This is the base class for <see cref="Line" /> and <see cref="Border" /> classes.
	/// </remarks>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.2 $ $Date: 2007-03-17 18:43:44 $ </version>
	[Serializable]
	public class LineBase : ICloneable, ISerializable
	{

	#region Fields

		/// <summary>
		/// Private field that stores the pen width for this line.
		/// Use the public property <see cref="Width"/> to access this value.
		/// </summary>
		internal float _width;
		/// <summary>
		/// Private field that stores the <see cref="DashStyle"/> for this
		/// line.  Use the public
		/// property <see cref="Style"/> to access this value.
		/// </summary>
		internal DashStyle _style;
		/// <summary>
		/// private field that stores the "Dash On" length for drawing the line.  Use the
		/// public property <see cref="DashOn" /> to access this value.
		/// </summary>
		internal float _dashOn;
		/// <summary>
		/// private field that stores the "Dash Off" length for drawing the line.  Use the
		/// public property <see cref="DashOff" /> to access this value.
		/// </summary>
		internal float _dashOff;

		/// <summary>
		/// Private field that stores the visibility of this line.  Use the public
		/// property <see cref="IsVisible"/> to access this value.
		/// </summary>
		internal bool _isVisible;

		/// <summary>
		/// private field that determines if the line is drawn using
		/// Anti-Aliasing capabilities from the <see cref="Graphics" /> class.
		/// Use the public property <see cref="IsAntiAlias" /> to access
		/// this value.
		/// </summary>
		internal bool _isAntiAlias;
		/// <summary>
		/// Private field that stores the color of this line.  Use the public
		/// property <see cref="Color"/> to access this value.  If this value is
		/// false, the line will not be shown (but the <see cref="Symbol"/> may
		/// still be shown).
		/// </summary>
		internal Color _color;

		/// <summary>
		/// Internal field that stores a custom <see cref="Fill" /> class.  This
		/// fill is used strictly for <see cref="FillType.GradientByX" />,
		/// <see cref="FillType.GradientByY" />, <see cref="FillType.GradientByZ" />,
		/// and <see cref="FillType.GradientByColorValue" /> calculations to determine
		/// the color of the line.
		/// </summary>
		internal Fill _gradientFill;


	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="LineBase"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default mode for displaying line segments (<see cref="LineBase.IsVisible"/>
			/// property).  True to show the line segments, false to hide them.
			/// </summary>
			public static bool IsVisible = true;
			/// <summary>
			/// The default width for line segments (<see cref="LineBase.Width"/> property).
			/// Units are points (1/72 inch).
			/// </summary>
			public static float Width = 1;
			/// <summary>
			/// The default value for the <see cref="LineBase.IsAntiAlias"/>
			/// property.
			/// </summary>
			public static bool IsAntiAlias = false;

			/// <summary>
			/// The default drawing style for line segments (<see cref="LineBase.Style"/> property).
			/// This is defined with the <see cref="DashStyle"/> enumeration.
			/// </summary>
			public static DashStyle Style = DashStyle.Solid;
			/// <summary>
			/// The default "dash on" size for drawing the line
			/// (<see cref="LineBase.DashOn"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOn = 1.0F;
			/// <summary>
			/// The default "dash off" size for drawing the the line
			/// (<see cref="LineBase.DashOff"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float DashOff = 1.0F;

			/// <summary>
			/// The default color for the line.
			/// This is the default value for the <see cref="LineBase.Color"/> property.
			/// </summary>
			public static Color Color = Color.Black;
		}

	#endregion

	#region Properties

		/// <summary>
		/// The color of the <see cref="Line"/>.  Note that this color value can be
		/// overridden if the <see cref="Fill.Type">GradientFill.Type</see> is one of the
		/// <see cref="FillType.GradientByX" />,
		/// <see cref="FillType.GradientByY" />, <see cref="FillType.GradientByZ" />,
		/// and <see cref="FillType.GradientByColorValue" /> types.
		/// </summary>
		/// <seealso cref="GradientFill"/>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}
		/// <summary>
		/// The style of the <see cref="Line"/>, defined as a <see cref="DashStyle"/> enum.
		/// This allows the line to be solid, dashed, or dotted.
		/// </summary>
		/// <seealso cref="Default.Style"/>
		/// <seealso cref="DashOn" />
		/// <seealso cref="DashOff" />
		public DashStyle Style
		{
			get { return _style; }
			set { _style = value; }
		}

		/// <summary>
		/// The "Dash On" mode for drawing the line.
		/// </summary>
		/// <remarks>
		/// This is the distance, in points (1/72 inch), of the dash segments that make up
		/// the dashed grid lines.  This setting is only valid if 
		/// <see cref="Style" /> is set to <see cref="DashStyle.Custom" />.
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
		/// The "Dash Off" mode for drawing the line.
		/// </summary>
		/// <remarks>
		/// This is the distance, in points (1/72 inch), of the spaces between the dash
		/// segments that make up the dashed grid lines.  This setting is only valid if 
		/// <see cref="Style" /> is set to <see cref="DashStyle.Custom" />.
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
		/// The pen width used to draw the <see cref="Line"/>, in points (1/72 inch)
		/// </summary>
		/// <seealso cref="Default.Width"/>
		public float Width
		{
			get { return _width; }
			set { _width = value; }
		}
		/// <summary>
		/// Gets or sets a property that shows or hides the <see cref="Line"/>.
		/// </summary>
		/// <value>true to show the line, false to hide it</value>
		/// <seealso cref="Default.IsVisible"/>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines if the lines are drawn using
		/// Anti-Aliasing capabilities from the <see cref="Graphics" /> class.
		/// </summary>
		/// <remarks>
		/// If this value is set to true, then the <see cref="Graphics.SmoothingMode" />
		/// property will be set to <see cref="SmoothingMode.HighQuality" /> only while
		/// this <see cref="Line" /> is drawn.  A value of false will leave the value of
		/// <see cref="Graphics.SmoothingMode" /> unchanged.
		/// </remarks>
		public bool IsAntiAlias
		{
			get { return _isAntiAlias; }
			set { _isAntiAlias = value; }
		}

		/// <summary>
		/// Gets or sets a custom <see cref="Fill" /> class.
		/// </summary>
		/// <remarks>This fill is used strictly for <see cref="FillType.GradientByX" />,
		/// <see cref="FillType.GradientByY" />, <see cref="FillType.GradientByZ" />,
		/// and <see cref="FillType.GradientByColorValue" /> calculations to determine
		/// the color of the line.  It overrides the <see cref="Color" /> property if
		/// one of the above <see cref="FillType" /> values are selected.
		/// </remarks>
		/// <seealso cref="Color"/>
		public Fill GradientFill
		{
			get { return _gradientFill; }
			set { _gradientFill = value; }
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Default constructor that sets all <see cref="LineBase"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		public LineBase()
			: this( Color.Empty )
		{
		}

		/// <summary>
		/// Constructor that sets the color property to the specified value, and sets
		/// the remaining <see cref="LineBase"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <param name="color">The color to assign to this new Line object</param>
		public LineBase( Color color )
		{
			_width = Default.Width;
			_style = Default.Style;
			_dashOn = Default.DashOn;
			_dashOff = Default.DashOff;
			_isVisible = Default.IsVisible;
			_color = color.IsEmpty ? Default.Color : color;
			_isAntiAlias = Default.IsAntiAlias;
			_gradientFill = new Fill( Color.Red, Color.White );
			_gradientFill.Type = FillType.None;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The LineBase object from which to copy</param>
		public LineBase( LineBase rhs )
		{
			_width = rhs._width;
			_style = rhs._style;
			_dashOn = rhs._dashOn;
			_dashOff = rhs._dashOff;

			_isVisible = rhs._isVisible;
			_color = rhs._color;

			_isAntiAlias = rhs._isAntiAlias;
			_gradientFill = new Fill( rhs._gradientFill );
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of Clone.
		/// </summary>
		/// <remarks>
		/// Note that this method must be called with an explicit cast to ICloneable, and
		/// that it is inherently virtual.  For example:
		/// <code>
		/// ParentClass foo = new ChildClass();
		/// ChildClass bar = (ChildClass) ((ICloneable)foo).Clone();
		/// </code>
		/// Assume that ChildClass is inherited from ParentClass.  Even though foo is declared with
		/// ParentClass, it is actually an instance of ChildClass.  Calling the ICloneable implementation
		/// of Clone() on foo actually calls ChildClass.Clone() as if it were a virtual function.
		/// </remarks>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			throw new NotImplementedException( "Can't clone an abstract base type -- child types must implement ICloneable" );
			//return new PaneBase( this );
		}

		// /// <summary>
		// /// Typesafe, deep-copy clone method.
		// /// </summary>
		// /// <returns>A new, independent copy of this class</returns>
		//public LineBase Clone()
		//{
		//	return new LineBase( this );
		//}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema0 = 12;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the
		/// serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains
		/// the serialized data
		/// </param>
		protected LineBase( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema0" );

			_width = info.GetSingle( "width" );
			_style = (DashStyle)info.GetValue( "style", typeof( DashStyle ) );
			_dashOn = info.GetSingle( "dashOn" );
			_dashOff = info.GetSingle( "dashOff" );
			_isVisible = info.GetBoolean( "isVisible" );
			_isAntiAlias = info.GetBoolean( "isAntiAlias" );
			_color = (Color)info.GetValue( "color", typeof( Color ) );
			_gradientFill = (Fill)info.GetValue( "gradientFill", typeof( Fill ) );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize
		/// the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the
		/// serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the
		/// serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema0", schema0 );

			info.AddValue( "width", _width );
			info.AddValue( "style", _style );
			info.AddValue( "dashOn", _dashOn );
			info.AddValue( "dashOff", _dashOff );
			info.AddValue( "isVisible", _isVisible );
			info.AddValue( "isAntiAlias", _isAntiAlias );
			info.AddValue( "color", _color );
			info.AddValue( "gradientFill", _gradientFill );
		}

	#endregion

	#region Methods

		/// <summary>
		/// Create a <see cref="Pen" /> object based on the properties of this
		/// <see cref="LineBase" />.
		/// </summary>
		/// <param name="pane">The owner <see cref="GraphPane" /> of this
		/// <see cref="LineBase" />.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>A <see cref="Pen" /> object with the properties of this <see cref="LineBase" />
		/// </returns>
		public Pen GetPen( PaneBase pane, float scaleFactor )
		{
			return GetPen( pane, scaleFactor, null );
		}

		/// <summary>
		/// Create a <see cref="Pen" /> object based on the properties of this
		/// <see cref="LineBase" />.
		/// </summary>
		/// <param name="pane">The owner <see cref="GraphPane" /> of this
		/// <see cref="LineBase" />.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="dataValue">The data value to be used for a value-based
		/// color gradient.  This is only applicable if <see cref="Fill.Type">GradientFill.Type</see>
		/// is one of <see cref="FillType.GradientByX"/>,
		/// <see cref="FillType.GradientByY"/>, <see cref="FillType.GradientByZ"/>,
		/// or <see cref="FillType.GradientByColorValue" />.
		/// </param>
		/// <returns>A <see cref="Pen" /> object with the properties of this <see cref="LineBase" />
		/// </returns>
		public Pen GetPen( PaneBase pane, float scaleFactor, PointPair dataValue )
		{
			Color color = _color;
			if ( _gradientFill.IsGradientValueType )
				color = _gradientFill.GetGradientColor( dataValue );

			Pen pen = new Pen( color,
						pane.ScaledPenWidth( _width, scaleFactor ) );

			pen.DashStyle = _style;

			if ( _style == DashStyle.Custom )
			{
				if ( _dashOff > 1e-10 && _dashOn > 1e-10 )
				{
					pen.DashStyle = DashStyle.Custom;
					float[] pattern = new float[2];
					pattern[0] = _dashOn;
					pattern[1] = _dashOff;
					pen.DashPattern = pattern;
				}
				else
					pen.DashStyle = DashStyle.Solid;
			}

			return pen;
		}

	#endregion


	}
}
