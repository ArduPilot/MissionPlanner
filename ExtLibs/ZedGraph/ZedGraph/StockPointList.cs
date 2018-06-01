//============================================================================
//PointPairList Class
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
using System.Collections.Generic;

namespace ZedGraph
{
	/// <summary>
	/// A collection class containing a list of <see cref="StockPt"/> objects
	/// that define the set of points to be displayed on the curve.
	/// </summary>
	/// 
	/// <author> John Champion based on code by Jerry Vos</author>
	/// <version> $Revision: 3.4 $ $Date: 2007-02-18 05:51:54 $ </version>
	[Serializable]
	public class StockPointList : List<StockPt>, IPointList, IPointListEdit
	{

	#region Properties

		/// <summary>
		/// Indexer to access the specified <see cref="StockPt"/> object by
		/// its ordinal position in the list.
		/// </summary>
		/// <param name="index">The ordinal position (zero-based) of the
		/// <see cref="StockPt"/> object to be accessed.</param>
		/// <value>A <see cref="StockPt"/> object reference.</value>
		public new PointPair this[int index]
		{
			get { return base[index]; }
			set { base[index] = new StockPt( value ); }
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Default constructor for the collection class
		/// </summary>
		public StockPointList()
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The StockPointList from which to copy</param>
		public StockPointList( StockPointList rhs )
		{
			for ( int i = 0; i < rhs.Count; i++ )
			{
				StockPt pt = new StockPt( rhs[i] );
				this.Add( pt );
			}
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
		public StockPointList Clone()
		{
			return new StockPointList( this );
		}

	#endregion

	#region Methods

		/// <summary>
		/// Add a <see cref="StockPt"/> object to the collection at the end of the list.
		/// </summary>
		/// <param name="point">The <see cref="StockPt"/> object to
		/// be added</param>
		new public void Add( StockPt point )
		{
			base.Add( new StockPt( point ) );
		}

		/// <summary>
		/// Add a <see cref="PointPair"/> object to the collection at the end of the list.
		/// </summary>
		/// <param name="point">The <see cref="PointPair"/> object to be added</param>
		public void Add( PointPair point )
		{
//			throw new ArgumentException( "Error: Only the StockPt type can be added to StockPointList" +
//				".  An ordinary PointPair is not allowed" );
			base.Add( new StockPt( point ) );
		}

		/// <summary>
		/// Add a <see cref="StockPt"/> object to the collection at the end of the list using
		/// the specified values.  The unspecified values (low, open, close) are all set to
		/// <see cref="PointPairBase.Missing" />.
		/// </summary>
		/// <param name="date">An <see cref="XDate" /> value</param>
		/// <param name="high">The high value for the day</param>
		/// <returns>The zero-based ordinal index where the point was added in the list.</returns>
		public void Add( double date, double high )
		{
			Add( new StockPt( date, high, PointPair.Missing, PointPair.Missing,
				PointPair.Missing, PointPair.Missing ) );
		}

		/// <summary>
		/// Add a single point to the <see cref="PointPairList"/> from values of type double.
		/// </summary>
		/// <param name="date">An <see cref="XDate" /> value</param>
		/// <param name="high">The high value for the day</param>
		/// <param name="low">The low value for the day</param>
		/// <param name="open">The opening value for the day</param>
		/// <param name="close">The closing value for the day</param>
		/// <param name="vol">The trading volume for the day</param>
		/// <returns>The zero-based ordinal index where the point was added in the list.</returns>
		public void Add( double date, double high, double low, double open, double close, double vol )
		{
			StockPt point = new StockPt( date, high, low, open, close, vol );
			Add( point );
		}

		/// <summary>
		/// Access the <see cref="StockPt" /> at the specified ordinal index.
		/// </summary>
		/// <remarks>
		/// To be compatible with the <see cref="IPointList" /> interface, the
		/// <see cref="StockPointList" /> must implement an index that returns a
		/// <see cref="PointPair" /> rather than a <see cref="StockPt" />.  This method
		/// will return the actual <see cref="StockPt" /> at the specified position.
		/// </remarks>
		/// <param name="index">The ordinal position (zero-based) in the list</param>
		/// <returns>The specified <see cref="StockPt" />.
		/// </returns>
		public StockPt GetAt( int index )
		{
			return base[index];
		}

	#endregion
	}
}


