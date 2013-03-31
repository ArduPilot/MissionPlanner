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
	/// A class that represents a graphic arrow or line object on the graph.  A list of
	/// ArrowObj objects is maintained by the <see cref="GraphObjList"/> collection class.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.4 $ $Date: 2007-01-25 07:56:08 $ </version>
	[Serializable]
	public class ArrowObj : LineObj, ICloneable, ISerializable
	{
		#region Fields
		/// <summary>
		/// Private field that stores the arrowhead size, measured in points.
		/// Use the public property <see cref="Size"/> to access this value.
		/// </summary>
		private float _size;
		/// <summary>
		/// Private boolean field that stores the arrowhead state.
		/// Use the public property <see cref="IsArrowHead"/> to access this value.
		/// </summary>
		/// <value> true if an arrowhead is to be drawn, false otherwise </value>
		private bool _isArrowHead;

		#endregion

		#region Defaults
		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="ArrowObj"/> class.
		/// </summary>
		new public struct Default
		{
			/// <summary>
			/// The default size for the <see cref="ArrowObj"/> item arrowhead
			/// (<see cref="ArrowObj.Size"/> property).  Units are in points (1/72 inch).
			/// </summary>
			public static float Size = 12.0F;
			/// <summary>
			/// The default display mode for the <see cref="ArrowObj"/> item arrowhead
			/// (<see cref="ArrowObj.IsArrowHead"/> property).  true to show the
			/// arrowhead, false to hide it.
			/// </summary>
			public static bool IsArrowHead = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The size of the arrowhead.
		/// </summary>
		/// <remarks>The display of the arrowhead can be
		/// enabled or disabled with the <see cref="IsArrowHead"/> property.
		/// </remarks>
		/// <value> The size is defined in points (1/72 inch) </value>
		/// <seealso cref="Default.Size"/>
		public float Size
		{
			get { return _size; }
			set { _size = value; }
		}
		/// <summary>
		/// Determines whether or not to draw an arrowhead
		/// </summary>
		/// <value> true to show the arrowhead, false to show the line segment
		/// only</value>
		/// <seealso cref="Default.IsArrowHead"/>
		public bool IsArrowHead
		{
			get { return _isArrowHead; }
			set { _isArrowHead = value; }
		}
		#endregion

		#region Constructors
		/// <overloads>Constructors for the <see cref="ArrowObj"/> object</overloads>
		/// <summary>
		/// A constructor that allows the position, color, and size of the
		/// <see cref="ArrowObj"/> to be pre-specified.
		/// </summary>
		/// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification
		/// for the arrow</param>
		/// <param name="size">The size of the arrowhead, measured in points.</param>
		/// <param name="x1">The x position of the starting point that defines the
		/// arrow.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y1">The y position of the starting point that defines the
		/// arrow.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="x2">The x position of the ending point that defines the
		/// arrow.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y2">The y position of the ending point that defines the
		/// arrow.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		public ArrowObj( Color color, float size, double x1, double y1,
					double x2, double y2 )
			: base( color, x1, y1, x2, y2 )
		{
			_isArrowHead = Default.IsArrowHead;
			_size = size;
		}

		/// <summary>
		/// A constructor that allows only the position of the
		/// arrow to be pre-specified.  All other properties are set to
		/// default values
		/// </summary>
		/// <param name="x1">The x position of the starting point that defines the
		/// <see cref="ArrowObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y1">The y position of the starting point that defines the
		/// <see cref="ArrowObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="x2">The x position of the ending point that defines the
		/// <see cref="ArrowObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		/// <param name="y2">The y position of the ending point that defines the
		/// <see cref="ArrowObj"/>.  The units of this position are specified by the
		/// <see cref="Location.CoordinateFrame"/> property.</param>
		public ArrowObj( double x1, double y1, double x2, double y2 )
			: this( LineBase.Default.Color, Default.Size, x1, y1, x2, y2 )
		{
		}

		/// <summary>
		/// Default constructor -- places the <see cref="ArrowObj"/> at location
		/// (0,0) to (1,1).  All other values are defaulted.
		/// </summary>
		public ArrowObj()
			:
			this( LineBase.Default.Color, Default.Size, 0, 0, 1, 1 )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ArrowObj"/> object from which to copy</param>
		public ArrowObj( ArrowObj rhs )
			: base( rhs )
		{
			_size = rhs.Size;
			_isArrowHead = rhs.IsArrowHead;
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
		public new ArrowObj Clone()
		{
			return new ArrowObj( this );
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
		protected ArrowObj( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema3" );

			_size = info.GetSingle( "size" );
			_isArrowHead = info.GetBoolean( "isArrowHead" );
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
			info.AddValue( "size", _size );
			info.AddValue( "isArrowHead", _isArrowHead );
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
				// get a scaled size for the arrowhead
				float scaledSize = (float)( _size * scaleFactor );

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
					//new Pen( _color, pane.ScaledPenWidth( _penWidth, scaleFactor ) ) )
				{
					//pen.DashStyle = _style;

					// Only show the arrowhead if required
					if ( _isArrowHead )
					{
						// Draw the line segment for this arrow
						g.DrawLine( pen, 0, 0, length - scaledSize + 1, 0 );

						// Create a polygon representing the arrowhead based on the scaled
						// size
						PointF[] polyPt = new PointF[4];
						float hsize = scaledSize / 3.0F;
						polyPt[0].X = length;
						polyPt[0].Y = 0;
						polyPt[1].X = length - scaledSize;
						polyPt[1].Y = hsize;
						polyPt[2].X = length - scaledSize;
						polyPt[2].Y = -hsize;
						polyPt[3] = polyPt[0];

						using ( SolidBrush brush = new SolidBrush( _line._color ) )
							// render the arrowhead
							g.FillPolygon( brush, polyPt );
					}
					else
						g.DrawLine( pen, 0, 0, length, 0 );
				}

				// Restore the transform matrix back to its original state
				g.Transform = transform;
			}
		}

		#endregion

	}
}
