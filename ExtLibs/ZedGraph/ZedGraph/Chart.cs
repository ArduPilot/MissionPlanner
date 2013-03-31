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
	/// Class that handles the properties of the charting area (where the curves are
	/// actually drawn), which is bounded by the <see cref="XAxis" />, <see cref="YAxis"/>,
	/// and <see cref="Y2Axis" />.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class Chart : ICloneable, ISerializable
	{
		/// <summary>
		/// The rectangle that contains the area bounded by the axes, in pixel units
		/// </summary>
		internal RectangleF _rect;

		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Chart"/>.  Use the public property <see cref="Fill"/> to
		/// access this value.
		/// </summary>
		internal Fill _fill;

		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.Border"/> data for this
		/// <see cref="Chart"/>.  Use the public property <see cref="Border"/> to
		/// access this value.
		/// </summary>
		internal Border _border;

		/// <summary>Private field that determines if the <see cref="Rect"/> will be
		/// sized automatically.  Use the public property <see cref="IsRectAuto"/> to access
		/// this value. </summary>
		internal bool _isRectAuto;

	#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Chart()
		{
			_isRectAuto = true;
			_border = new Border( Default.IsBorderVisible, Default.BorderColor, Default.BorderPenWidth );
			_fill = new Fill( Default.FillColor, Default.FillBrush, Default.FillType );
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">The source <see cref="Chart" /> to be copied.</param>
		public Chart( Chart rhs )
		{
			_border = rhs._border.Clone();
			_fill = rhs._fill.Clone();
			_rect = rhs._rect;
			_isRectAuto = rhs._isRectAuto;
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
		public Chart Clone()
		{
			return new Chart( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets the rectangle that contains the area bounded by the axes
		/// (<see cref="XAxis"/>, <see cref="YAxis"/>, and <see cref="Y2Axis"/>).
		/// If you set this value manually, then the <see cref="IsRectAuto"/>
		/// value will automatically be set to false.
		/// </summary>
		/// <value>The rectangle units are in screen pixels</value>
		public RectangleF Rect
		{
			get { return _rect; }
			set { _rect = value; _isRectAuto = false; }
		}

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Chart"/>.
		/// </summary>
		public Fill Fill
		{
			get { return _fill; }
			set { _fill = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.Border"/> class for drawing the border
		/// border around the <see cref="Chart"/>
		/// </summary>
		/// <seealso cref="Default.BorderColor"/>
		/// <seealso cref="Default.BorderPenWidth"/>
		public Border Border
		{
			get { return _border; }
			set { _border = value; }
		}

		/// <summary>
		/// Gets or sets a boolean value that determines whether or not the 
		/// <see cref="Rect"/> will be calculated automatically (almost always true).
		/// </summary>
		/// <remarks>
		/// If you have a need to set the ChartRect manually, such as you have multiple graphs
		/// on a page and you want to line up the edges perfectly, you can set this value
		/// to false.  If you set this value to false, you must also manually set
		/// the <see cref="Rect"/> property.
		/// You can easily determine the ChartRect that ZedGraph would have
		/// calculated by calling the <see cref="GraphPane.CalcChartRect(Graphics)"/> method, which returns
		/// a chart rect sized for the current data range, scale sizes, etc.
		/// </remarks>
		/// <value>true to have ZedGraph calculate the ChartRect, false to do it yourself</value>
		public bool IsRectAuto
		{
			get { return _isRectAuto; }
			set { _isRectAuto = value; }
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
		protected Chart( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_rect = (RectangleF)info.GetValue( "rect", typeof( RectangleF ) );
			_fill = (Fill)info.GetValue( "fill", typeof( Fill ) );
			_border = (Border)info.GetValue( "border", typeof( Border ) );
			_isRectAuto = info.GetBoolean( "isRectAuto" );
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
			info.AddValue( "rect", _rect );
			info.AddValue( "fill", _fill );
			info.AddValue( "border", _border );
			info.AddValue( "isRectAuto", _isRectAuto );
		}

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="Chart"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default color for the <see cref="Chart"/> border.
			/// (<see cref="Chart.Border"/> property). 
			/// </summary>
			public static Color BorderColor = Color.Black;
			/// <summary>
			/// The default color for the <see cref="Chart"/> background.
			/// (<see cref="Chart.Fill"/> property). 
			/// </summary>
			public static Color FillColor = Color.White;
			/// <summary>
			/// The default brush for the <see cref="GraphPane.Chart"/> background.
			/// (<see cref="ZedGraph.Fill.Brush"/> property of <see cref="Chart.Fill"/>). 
			/// </summary>
			public static Brush FillBrush = null;
			/// <summary>
			/// The default <see cref="FillType"/> for the <see cref="GraphPane.Chart"/> background.
			/// (<see cref="ZedGraph.Fill.Type"/> property of <see cref="Chart.Fill"/>). 
			/// </summary>
			public static FillType FillType = FillType.Brush;
			/// <summary>
			/// The default pen width for drawing the 
			/// <see cref="GraphPane.Chart"/> border
			/// (<see cref="Chart.Border"/> property).
			/// Units are in points (1/72 inch).
			/// </summary>
			public static float BorderPenWidth = 1F;
			/// <summary>
			/// The default display mode for the <see cref="Chart"/> border
			/// (<see cref="Chart.Border"/> property). true
			/// to show the border border, false to omit the border
			/// </summary>
			public static bool IsBorderVisible = true;

		}

	#endregion

	}
}
