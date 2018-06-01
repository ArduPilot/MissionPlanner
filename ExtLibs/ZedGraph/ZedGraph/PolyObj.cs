//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2005  John Champion
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
	/// A class that represents a bordered and/or filled polygon object on
	/// the graph.  A list of <see cref="PolyObj"/> objects is maintained by
	/// the <see cref="GraphObjList"/> collection class.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.4 $ $Date: 2007-01-25 07:56:09 $ </version>
	[Serializable]
	public class PolyObj : BoxObj, ICloneable, ISerializable
	{

	#region Fields

		private PointD[] _points;

		/// <summary>
		/// private value that determines if the polygon will be automatically closed.
		/// true to close the figure, false to leave it "open."  Use the public property
		/// <see cref="IsClosedFigure" /> to access this value.
		/// </summary>
		private bool _isClosedFigure = true;

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets the <see cref="PointD"/> array that defines
		/// the polygon.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame"/>.
		/// </summary>
		public PointD[] Points
		{
			get { return _points; }
			set { _points = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines if the polygon will be automatically closed.
		/// true to close the figure, false to leave it "open."
		/// </summary>
		/// <remarks>
		/// This boolean determines whether or not the CloseFigure() method will be called
		/// to fully close the path of the polygon.  This value defaults to true, and for any
		/// closed figure it should fine.  If you want to draw a line that does not close into
		/// a shape, then you should set this value to false.  For a figure that is naturally
		/// closed (e.g., the first point of the polygon is the same as the last point),
		/// leaving this value set to false may result in minor pixel artifacts due to
		/// rounding.
		/// </remarks>
		public bool IsClosedFigure
		{
			get { return _isClosedFigure; }
			set { _isClosedFigure = value; }
		}

	#endregion
	
	#region Constructors
		/// <overloads>Constructors for the <see cref="PolyObj"/> object</overloads>
		/// <summary>
		/// A constructor that allows the position, border color, and solid fill color
		/// of the <see cref="PolyObj"/> to be pre-specified.
		/// </summary>
		/// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the box border</param>
		/// <param name="fillColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the box fill (will be a solid color fill)</param>
		/// <param name="points">The <see cref="PointD"/> array that defines
		/// the polygon.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame"/>.
		/// </param>
		public PolyObj( PointD[] points, Color borderColor, Color fillColor ) :
				base( 0, 0, 1, 1, borderColor, fillColor )
		{
			_points = points;
		}

		/// <summary>
		/// A constructor that allows the position
		/// of the <see cref="PolyObj"/> to be pre-specified.  Other properties are defaulted.
		/// </summary>
		/// <param name="points">The <see cref="PointD"/> array that defines
		/// the polygon.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame"/>.
		/// </param>
		public PolyObj( PointD[] points ) : base( 0, 0, 1, 1 )
		{
			_points = points;
		}

		/// <summary>
		/// A default constructor that creates a <see cref="PolyObj"/> from an empty
		/// <see cref="PointD"/> array.  Other properties are defaulted.
		/// </summary>
		public PolyObj() : this( new PointD[0] )
		{
		}

		/// <summary>
		/// A constructor that allows the position, border color, and two-color
		/// gradient fill colors
		/// of the <see cref="PolyObj"/> to be pre-specified.
		/// </summary>
		/// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the box border</param>
		/// <param name="fillColor1">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the start of the box gradient fill</param>
		/// <param name="fillColor2">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the end of the box gradient fill</param>
		/// <param name="points">The <see cref="PointD"/> array that defines
		/// the polygon.  This will be in units determined by
		/// <see cref="ZedGraph.Location.CoordinateFrame"/>.
		/// </param>
		public PolyObj( PointD[] points, Color borderColor,
							Color fillColor1, Color fillColor2 ) :
				base( 0, 0, 1, 1, borderColor, fillColor1, fillColor2 )
		{
			_points = points;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="PolyObj"/> object from which to copy</param>
		public PolyObj( PolyObj rhs ) : base( rhs )
		{
			rhs._points = (PointD[]) _points.Clone();
			rhs._isClosedFigure = _isClosedFigure;
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
		public new PolyObj Clone()
		{
			return new PolyObj( this );
		}

	#endregion

	#region Serialization
		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema3 = 11;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected PolyObj( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema3" );

			_points = (PointD[]) info.GetValue( "points", typeof(PointD[]) );

			if ( schema3 >= 11 )
				_isClosedFigure = info.GetBoolean( "isClosedFigure" );

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

			info.AddValue( "points", _points );
			info.AddValue( "isClosedFigure", _isClosedFigure );
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
			if ( _points != null && _points.Length > 1 )
			{
				using ( GraphicsPath path = MakePath( pane ) )
				{
					// Fill or draw the symbol as required
					if ( _fill.IsVisible )
					{
						using ( Brush brush = this.Fill.MakeBrush( path.GetBounds() ) )
							g.FillPath( brush, path );
					}

					if ( _border.IsVisible )
					{
						using ( Pen pen = _border.GetPen( pane, scaleFactor ) )
							g.DrawPath( pen, path );
					}
				}
			}
		}
		
		internal GraphicsPath MakePath( PaneBase pane )
		{
			GraphicsPath path = new GraphicsPath();
			bool first = true;
			PointF lastPt = new PointF();

			foreach( PointD pt in _points )
			{
				// Convert the coordinates from the user coordinate system
				// to the screen coordinate system
				// Offset the points by the location value
				PointF pixPt = Location.Transform( pane, pt.X + _location.X, pt.Y + _location.Y,
						_location.CoordinateFrame );

				if (	Math.Abs( pixPt.X ) < 100000 &&
						Math.Abs( pixPt.Y ) < 100000 )
				{
					if ( first )
						first = false;
					else
						path.AddLine( lastPt, pixPt );

					lastPt = pixPt;
				}
			}

			if ( _isClosedFigure )
				path.CloseFigure();

			return path;
		}

		/// <summary>
		/// Determine if the specified screen point lies inside the bounding box of this
		/// <see cref="PolyObj"/>.
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
			if ( _points != null && _points.Length > 1 )
			{
				if ( ! base.PointInBox(pt, pane, g, scaleFactor ) )
					return false;

				using ( GraphicsPath path = MakePath( pane ) )
					return path.IsVisible( pt );
			}
			else
				return false;
		}
		
	#endregion
	
	}
}
