//============================================================================
//RadarPointList Class
//Copyright © 2006  John Champion, Jerry Vos
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
using System.Collections.Generic;

namespace ZedGraph
{
	/// <summary>
	/// A class containing a set of data values to be plotted as a RadarPlot.
	/// This class will effectively convert the data into <see cref="PointPair" /> objects
	/// by converting the polar coordinates to rectangular coordinates
	/// </summary>
	/// <seealso cref="BasicArrayPointList" />
	/// <seealso cref="IPointList" />
	/// <seealso cref="IPointListEdit" />
	/// 
	/// <author>Jerry Vos and John Champion</author>
	/// <version> $Revision: 3.5 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	public class RadarPointList : List<PointPair>, IPointList, IPointListEdit
	{

	#region Fields
		/// <summary>
		/// Default to clockwise rotation as this is the standard for radar charts
		/// </summary>
		private bool _clockwise = true;

		/// <summary>
		/// Default to 90 degree rotation so main axis is in the 12 o'clock position,
		/// which is the standard for radar charts.
		/// </summary>
		private double _rotation = 90;
	#endregion

	#region Properties

		/// <summary>
		/// Indexer to access the specified <see cref="PointPair"/> object by
		/// its ordinal position in the list.  This method does the calculations
		/// to convert the data from polar to rectangular coordinates.
		/// </summary>
		/// <param name="index">The ordinal position (zero-based) of the
		/// <see cref="PointPair"/> object to be accessed.</param>
		/// <value>A <see cref="PointPair"/> object reference.</value>
		public new PointPair this[int index]
		{
			get
			{
				int count = this.Count;
				// The last point is a repeat of the first point
				if ( index == count - 1 )
					index = 0;

				if ( index < 0 || index >= count )
					return null;

				PointPair pt = (PointPair)base[index];
//				double theta = (double) index / (double) count * 2.0 * Math.PI;
				double rotationRadians = _rotation * Math.PI / 180;
				double theta = rotationRadians + ( _clockwise ? -1.0d : 1.0d ) *
						( (double) index / (double) (count-1) * 2.0 * Math.PI);
				double x = pt.Y * Math.Cos( theta );
				double y = pt.Y * Math.Sin( theta );
				return new PointPair( x, y, pt.Z, (string) pt.Tag );
			}
			set
			{
				int count = this.Count;
				// The last point is a repeat of the first point
				if ( index == count - 1 )
					index = 0;

				if ( index < 0 || index >= count )
					return;

				PointPair pt = (PointPair)base[index];
				pt.Y = Math.Sqrt( value.X * value.X + value.Y * value.Y );
			}
		}

		/// <summary>
		/// Indicates if points should be added in clockwise or counter-clockwise order
		/// </summary>
		public bool Clockwise
		{
			get { return _clockwise; }
			set { _clockwise = value; }
		}

		/// <summary>
		/// Sets the angular rotation (starting angle) for the initial axis
		/// </summary>
		public double Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}

		/// <summary>
		/// Get the raw data
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private PointPair GetAt( int index )
		{
			return base[index];
		}

		/// <summary>
		/// gets the number of points available in the list
		/// </summary>
		public new int Count
		{
			get { return base.Count + 1; }
		}

	#endregion

	#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		public RadarPointList() : base()
		{
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		public RadarPointList( RadarPointList rhs )
		{
			for ( int i = 0; i < rhs.Count; i++ )
				this.Add( rhs.GetAt(i) );
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
		public RadarPointList Clone()
		{
			return new RadarPointList( this );
		}


	#endregion

	#region Methods
/*
 * /// <summary>
		/// Add the specified PointPair to the collection.
		/// </summary>
		/// <param name="pt">The PointPair to be added</param>
		/// <returns>The ordinal position in the list where the point was added</returns>
		public int Add( PointPair pt )
		{
			return List.Add( pt );
		}

		/// <summary>
		/// Add a single point to the <see cref="RadarPointList"/> from a value of type double.
		/// </summary>
		/// <param name="r">The radial coordinate value</param>
		/// <returns>The zero-based ordinal index where the point was added in the list.</returns>
		/// <seealso cref="IList.Add"/>
		public int Add( double r )
		{
			return List.Add( new PointPair( PointPair.Missing, r ) );
		}
*/
		/// <summary>
		/// Add a single point to the <see cref="RadarPointList"/> from two values of type double.
		/// </summary>
		/// <param name="r">The radial coordinate value</param>
		/// <param name="z">The 'Z' coordinate value, which is not normally used for plotting,
		/// but can be used for <see cref="FillType.GradientByZ" /> type fills</param>
		/// <returns>The zero-based ordinal index where the point was added in the list.</returns>
		public void Add( double r, double z )
		{
			Add( new PointPair( PointPair.Missing, r, z ) );
		}

	#endregion
	}
}


