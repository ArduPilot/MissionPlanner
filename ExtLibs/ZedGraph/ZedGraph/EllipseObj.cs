//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
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
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// A class that represents a bordered and/or filled ellipse object on
	/// the graph.  A list of EllipseObj objects is maintained by the
	/// <see cref="GraphObjList"/> collection class.  The ellipse is defined
	/// as the ellipse that would be contained by the rectangular box as
	/// defined by the <see cref="Location"/> property.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.3 $ $Date: 2007-01-25 07:56:08 $ </version>
	[Serializable]
	public class EllipseObj : BoxObj, ICloneable, ISerializable
	{
	#region Constructors
		/// <overloads>Constructors for the <see cref="EllipseObj"/> object</overloads>
		/// <summary>
		/// A constructor that allows the position and size
		/// of the <see cref="EllipseObj"/> to be pre-specified.  Other properties are defaulted.
		/// </summary>
		/// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		public EllipseObj( double x, double y, double width, double height )
			: base( x, y, width, height )
		{
		}
		
		/// <summary>
		/// A default constructor that places the <see cref="EllipseObj"/> at location (0,0),
		/// with width/height of (1,1).  Other properties are defaulted.
		/// </summary>
		public EllipseObj() : base()
		{
		}
		
		/// <summary>
		/// A constructor that allows the position, border color, and solid fill color
		/// of the <see cref="EllipseObj"/> to be pre-specified.
		/// </summary>
		/// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the ellipse border</param>
		/// <param name="fillColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the ellipse fill (will be a solid color fill)</param>
		/// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		public EllipseObj( double x, double y, double width, double height, Color borderColor, Color fillColor )
			: base( x, y, width, height, borderColor, fillColor )
		{
		}

		/// <summary>
		/// A constructor that allows the position, border color, and two-color
		/// gradient fill colors
		/// of the <see cref="EllipseObj"/> to be pre-specified.
		/// </summary>
		/// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the ellipse border</param>
		/// <param name="fillColor1">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the start of the ellipse gradient fill</param>
		/// <param name="fillColor2">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the end of the ellipse gradient fill</param>
		/// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		/// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
		public EllipseObj( double x, double y, double width, double height, Color borderColor,
							Color fillColor1, Color fillColor2 ) :
				base( x, y, width, height, borderColor, fillColor1, fillColor2 )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="EllipseObj"/> object from
		/// which to copy</param>
		public EllipseObj( BoxObj rhs ) : base( rhs )
		{
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
		public new EllipseObj Clone()
		{
			return new EllipseObj( this );
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
		protected EllipseObj( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema3" );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "schema3", schema3 );
		}
	#endregion
	
	#region Rendering Methods
		/// <summary>
		/// Render this object to the specified <see cref="Graphics"/> device.
		/// </summary>
		/// <remarks>
		/// This method is normally only called by the Draw method
		/// of the parent <see cref="GraphObjList"/> collection object.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="PaneBase"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		override public void Draw( Graphics g, PaneBase pane, float scaleFactor )
		{
			// Convert the arrow coordinates from the user coordinate system
			// to the screen coordinate system
			RectangleF pixRect = this.Location.TransformRect( pane );

			if (	Math.Abs( pixRect.Left ) < 100000 &&
					Math.Abs( pixRect.Top ) < 100000 &&
					Math.Abs( pixRect.Right ) < 100000 &&
					Math.Abs( pixRect.Bottom ) < 100000 )
			{
				if ( _fill.IsVisible )
					using ( Brush brush = _fill.MakeBrush( pixRect ) )
						g.FillEllipse( brush, pixRect );

				if ( _border.IsVisible )
					using ( Pen pen = _border.GetPen( pane, scaleFactor ) )
						g.DrawEllipse( pen, pixRect );
			}
		}
		
		/// <summary>
		/// Determine if the specified screen point lies inside the bounding box of this
		/// <see cref="BoxObj"/>.
		/// </summary>
		/// <param name="pt">The screen point, in pixels</param>
		/// <param name="pane">
		/// A reference to the <see cref="PaneBase"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>true if the point lies in the bounding box, false otherwise</returns>
		override public bool PointInBox( PointF pt, PaneBase pane, Graphics g, float scaleFactor )
		{
			if ( ! base.PointInBox(pt, pane, g, scaleFactor ) )
				return false;

			// transform the x,y location from the user-defined
			// coordinate frame to the screen pixel location
			RectangleF pixRect = _location.TransformRect( pane );

			using ( GraphicsPath path = new GraphicsPath() )
			{
				path.AddEllipse( pixRect );
				return path.IsVisible( pt );
			}
		}
		
	#endregion
	
	}
}
