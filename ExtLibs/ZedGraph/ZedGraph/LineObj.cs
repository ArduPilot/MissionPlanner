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
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// A class that represents a line segment object on the graph.  A list of
	/// GraphObj objects is maintained by the <see cref="GraphObjList"/> collection class.
	/// </summary>
	/// <remarks>
	/// This should not be confused with the <see cref="LineItem" /> class, which represents
	/// a set of points plotted together as a "curve".  The <see cref="LineObj" /> class is
	/// a single line segment, drawn as a "decoration" on the chart.</remarks>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.4 $ $Date: 2007-01-25 07:56:09 $ </version>
	[Serializable]
	public class LineObj : GraphObj, ICloneable, ISerializable
	{
	#region Fields

		/// <summary>
		/// protected field that maintains the attributes of the line using an
		/// instance of the <see cref="LineBase" /> class.
		/// </summary>
		protected LineBase _line;

	#endregion

	#region Properties

		/// <summary>
		/// A <see cref="LineBase" /> class that contains the attributes for drawing this
		/// <see cref="LineObj" />.
		/// </summary>
		public LineBase Line
		{
			get { return _line; }
			set { _line = value; }
		}

	#endregion


	#region Constructors

		/// <overloads>Constructors for the <see cref="LineObj"/> object</overloads>
		/// <summary>
		/// A constructor that allows the position, color, and size of the
		/// <see cref="LineObj"/> to be pre-specified.
		/// </summary>
		/// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the arrow</param>
		/// <param name="x1">The x position of the starting point that defines the
		/// line.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y1">The y position of the starting point that defines the
		/// line.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="x2">The x position of the ending point that defines the
		/// line.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y2">The y position of the ending point that defines the
		/// line.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		public LineObj( Color color, double x1, double y1, double x2, double y2 )
			: base( x1, y1, x2 - x1, y2 - y1 )
		{
			_line = new LineBase( color );
			this.Location.AlignH = AlignH.Left;
			this.Location.AlignV = AlignV.Top;
		}

		/// <summary>
		/// A constructor that allows only the position of the
		/// line to be pre-specified.  All other properties are set to
		/// default values
		/// </summary>
		/// <param name="x1">The x position of the starting point that defines the
		/// <see cref="LineObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y1">The y position of the starting point that defines the
		/// <see cref="LineObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="x2">The x position of the ending point that defines the
		/// <see cref="LineObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y2">The y position of the ending point that defines the
		/// <see cref="LineObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		public LineObj( double x1, double y1, double x2, double y2 )
			: this( LineBase.Default.Color, x1, y1, x2, y2 )
		{
		}

		/// <summary>
		/// Default constructor -- places the <see cref="LineObj"/> at location
		/// (0,0) to (1,1).  All other values are defaulted.
		/// </summary>
		public LineObj() : this( LineBase.Default.Color, 0, 0, 1, 1 )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="LineObj"/> object from which to copy</param>
		public LineObj( LineObj rhs ) : base( rhs )
		{
			_line = new LineBase( rhs._line );
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
		public LineObj Clone()
		{
			return new LineObj( this );
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		// changed to 2 with addition of Style property
		public const int schema2 = 11;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected LineObj( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			_line = (LineBase)info.GetValue( "line", typeof( LineBase ) );
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

			info.AddValue( "line", _line );
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
			PointF pix1 = this.Location.TransformTopLeft( pane );
			PointF pix2 = this.Location.TransformBottomRight( pane );

			if ( pix1.X > -10000 && pix1.X < 100000 && pix1.Y > -100000 && pix1.Y < 100000 &&
				pix2.X > -10000 && pix2.X < 100000 && pix2.Y > -100000 && pix2.Y < 100000 )
			{
				// calculate the length and the angle of the arrow "vector"
				double dy = pix2.Y - pix1.Y;
				double dx = pix2.X - pix1.X;
				float angle = (float)Math.Atan2( dy, dx ) * 180.0F / (float)Math.PI;
				float length = (float)Math.Sqrt( dx * dx + dy * dy );

				// Save the old transform matrix
				Matrix transform = g.Transform;
				// Move the coordinate system so it is located at the starting point
				// of this arrow
				g.TranslateTransform( pix1.X, pix1.Y );
				// Rotate the coordinate system according to the angle of this arrow
				// about the starting point
				g.RotateTransform( angle );

				// get a pen according to this arrow properties
				using ( Pen pen = _line.GetPen( pane, scaleFactor ) )
				//new Pen( _line._color, pane.ScaledPenWidth( _line._width, scaleFactor ) ) )
				{
					//pen.DashStyle = _style;

					g.DrawLine( pen, 0, 0, length, 0 );
				}

				// Restore the transform matrix back to its original state
				g.Transform = transform;
			}
		}

		/// <summary>
		/// Determine if the specified screen point lies inside the bounding box of this
		/// <see cref="LineObj"/>.
		/// </summary>
		/// <remarks>The bounding box is calculated assuming a distance
		/// of <see cref="GraphPane.Default.NearestTol"/> pixels around the arrow segment.
		/// </remarks>
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
			PointF pix = _location.TransformTopLeft( pane );
			PointF pix2 = _location.TransformBottomRight( pane );

			using ( Pen pen = new Pen( Color.Black, (float)GraphPane.Default.NearestTol * 2.0F ) )
			{
				using ( GraphicsPath path = new GraphicsPath() )
				{
					path.AddLine( pix, pix2 );
					return path.IsOutlineVisible( pt, pen );
				}
			}
		}

		/// <summary>
		/// Determines the shape type and Coords values for this GraphObj
		/// </summary>
		override public void GetCoords( PaneBase pane, Graphics g, float scaleFactor,
				out string shape, out string coords )
		{
			// transform the x,y location from the user-defined
			// coordinate frame to the screen pixel location
			RectangleF pixRect = _location.TransformRect( pane );

			Matrix matrix = new Matrix();
			if ( pixRect.Right == 0 )
				pixRect.Width = 1;
			float angle = (float) Math.Atan( ( pixRect.Top - pixRect.Bottom ) /
					( pixRect.Left - pixRect.Right ) );
			matrix.Rotate( angle, MatrixOrder.Prepend );

			// Move the coordinate system to local coordinates
			// of this text object (that is, at the specified
			// x,y location)
			matrix.Translate( -pixRect.Left, -pixRect.Top, MatrixOrder.Prepend );

			PointF[] pts = new PointF[4];
			pts[0] = new PointF( 0, 3 );
			pts[1] = new PointF( pixRect.Width, 3 );
			pts[2] = new PointF( pixRect.Width, -3 );
			pts[3] = new PointF( 0, -3 );
			matrix.TransformPoints( pts );

			shape = "poly";
			coords = String.Format( "{0:f0},{1:f0},{2:f0},{3:f0},{4:f0},{5:f0},{6:f0},{7:f0},",
						pts[0].X, pts[0].Y, pts[1].X, pts[1].Y,
						pts[2].X, pts[2].Y, pts[3].X, pts[3].Y );
		}

	#endregion

	}
}
