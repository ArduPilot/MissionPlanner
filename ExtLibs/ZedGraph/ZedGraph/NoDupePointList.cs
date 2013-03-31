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
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ZedGraph
{
	/// <summary>
	/// A simple storage struct to maintain an individual sampling of data.  This only
	/// contains two data values in order to reduce to memory load for large datasets.
	/// (e.g., no Tag or Z property)
	/// </summary>
	public struct DataPoint
	{
		/// <summary>
		/// The X value for the point, stored as a double type.
		/// </summary>
		public double X;
		/// <summary>
		/// The Y value for the point, stored as a double type.
		/// </summary>
		public double Y;
	}

	/// <summary>
	/// A collection class to maintain a set of samples.
	/// </summary>
	/// <remarks>This type, intended for very
	/// large datasets, will reduce the number of points displayed by eliminating
	/// individual points that overlay (at the same pixel location) on the graph.
	/// Note that this type probably does not make sense for line plots, but is intended
	/// primarily for scatter plots.
	/// </remarks>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.5 $ $Date: 2007-06-02 06:56:03 $ </version>
	[Serializable]
	public class NoDupePointList : List<DataPoint>, IPointList, IPointListEdit
	{
		/// <summary>
		/// Protected field that stores a value indicating whether or not the data have been filtered.
		/// If the data have not been filtered, then <see cref="Count" /> will be equal to
		/// <see cref="TotalCount" />.  Use the public property <see cref="IsFiltered" /> to
		/// access this value.
		/// </summary>
		protected bool _isFiltered;
		/// <summary>
		/// Protected field that stores the number of data points after filtering (e.g.,
		/// <see cref="FilterData" /> has been called).  The <see cref="Count" /> property
		/// returns the total count for an unfiltered dataset, or <see cref="_filteredCount" />
		/// for a dataset that has been filtered.
		/// </summary>
		protected int _filteredCount;
		/// <summary>
		/// Protected array of indices for all the points that are currently visible.  This only
		/// applies if <see cref="IsFiltered" /> is true.
		/// </summary>
		protected int[] _visibleIndicies;


		/// <summary>
		/// Protected field that stores a value that determines how close a point must be to a prior
		/// neighbor in order to be filtered out.  Use the public property <see cref="FilterMode" />
		/// to access this value.
		/// </summary>
		protected int _filterMode;


		/// <summary>
		/// Gets or sets a value that determines how close a point must be to a prior
		/// neighbor in order to be filtered out.
		/// </summary>
		/// <remarks>
		/// A value of 0 indicates that subsequent
		/// points must coincide exactly at the same pixel location.  A value of 1 or more
		/// indicates that number of pixels distance from a prior point that will cause
		/// a new point to be filtered out.  For example, a value of 2 means that, once
		/// a particular pixel location is taken, any subsequent point that lies within 2
		/// pixels of that location will be filtered out.
		/// </remarks>
		public int FilterMode
		{
			get { return _filterMode; }
			set { _filterMode = value; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the data have been filtered.  If the data
		/// have not been filtered, then <see cref="Count" /> will be equal to
		/// <see cref="TotalCount" />.
		/// </summary>
		public bool IsFiltered
		{
			get { return _isFiltered; }
		}

		/// <summary>
		/// Indexer: get the DataPoint instance at the specified ordinal position in the list
		/// </summary>
		/// <remarks>
		/// This method will throw an exception if the index is out of range.  This can happen
		/// if the index is less than the number of filtered values, or if data points are
		/// removed from a filtered dataset with updating the filter (by calling
		/// <see cref="FilterData" />).
		/// </remarks>
		/// <param name="index">The ordinal position in the list of points</param>
		/// <returns>Returns a <see cref="PointPair" /> instance.  The <see cref="PointPair.Z" />
		/// and <see cref="PointPair.Tag" /> properties will be defaulted to
		/// <see cref="PointPairBase.Missing" /> and null, respectively.
		/// </returns>
		public new PointPair this[int index]
		{
			get
			{
				int j = index;
				if ( _isFiltered )
					j = _visibleIndicies[index];

				DataPoint dp = base[j];
				PointPair pt = new PointPair( dp.X, dp.Y );
				return pt;
			}
			set
			{
				int j = index;
				if ( _isFiltered )
					j = _visibleIndicies[index];

				DataPoint dp;
				dp.X = value.X;
				dp.Y = value.Y;
				base[j] = dp;
			}
		}

		/// <summary>
		/// Gets the number of active samples in the collection.  This is the number of
		/// samples that are non-duplicates.  See the <see cref="TotalCount" /> property
		/// to get the total number of samples in the list.
		/// </summary>
		public new int Count
		{
			get
			{
				if ( !_isFiltered )
					return base.Count;
				else
					return _filteredCount;
			}
		}

		/// <summary>
		/// Gets the total number of samples in the collection.  See the <see cref="Count" />
		/// property to get the number of active (non-duplicate) samples in the list.
		/// </summary>
		public int TotalCount
		{
			get { return base.Count; }
		}

		/// <summary>
		/// Append a data point to the collection
		/// </summary>
		/// <param name="pt">The <see cref="PointPair" /> value to append</param>
		public void Add( PointPair pt )
		{
			DataPoint dp = new DataPoint();
			dp.X = pt.X;
			dp.Y = pt.Y;
			Add( dp );
		}


		/// <summary>
		/// Append a point to the collection
		/// </summary>
		/// <param name="x">The x value of the point to append</param>
		/// <param name="y">The y value of the point to append</param>
		public void Add( double x, double y )
		{
			DataPoint dp = new DataPoint();
			dp.X = x;
			dp.Y = y;
			Add( dp );
		}


		// generic Clone: just call the typesafe version
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// typesafe clone method
		/// </summary>
		/// <returns>A new cloned NoDupePointList.  This returns a copy of the structure,
		/// but it does not duplicate the data (it just keeps a reference to the original)
		/// </returns>
		public NoDupePointList Clone()
		{
			return new NoDupePointList( this );
		}

		/// <summary>
		/// default constructor
		/// </summary>
		public NoDupePointList()
		{
			_isFiltered = false;
			_filteredCount = 0;
			_visibleIndicies = null;
			_filterMode = 0;
		}

		/// <summary>
		/// copy constructor -- this returns a copy of the structure,
		/// but it does not duplicate the data (it just keeps a reference to the original)
		/// </summary>
		/// <param name="rhs">The NoDupePointList to be copied</param>
		public NoDupePointList( NoDupePointList rhs )
		{
			int count = rhs.TotalCount;
			for ( int i = 0; i < count; i++ )
				Add( rhs.GetDataPointAt( i ) );

			_filteredCount = rhs._filteredCount;
			_isFiltered = rhs._isFiltered;
			_filterMode = rhs._filterMode;

			if ( rhs._visibleIndicies != null )
				_visibleIndicies = (int[]) rhs._visibleIndicies.Clone();
			else
				_visibleIndicies = null;
		}

		/// <summary>
		/// Protected method to access the internal DataPoint collection, without any
		/// translation to a PointPair.
		/// </summary>
		/// <param name="index">The ordinal position of the DataPoint of interest</param>
		protected DataPoint GetDataPointAt( int index )
		{
			return base[index];
		}

		/// <summary>
		/// Clears any filtering previously done by a call to <see cref="FilterData" />.
		/// After calling this method, all data points will be visible, and
		/// <see cref="Count" /> will be equal to <see cref="TotalCount" />.
		/// </summary>
		public void ClearFilter()
		{
			_isFiltered = false;
			_filteredCount = 0;
		}

		/// <summary>
		/// Go through the collection, and hide (filter out) any points that fall on the
		/// same pixel location as a previously included point.
		/// </summary>
		/// <remarks>
		/// This method does not delete any points, it just temporarily hides them until
		/// the next call to <see cref="FilterData" /> or <see cref="ClearFilter" />.
		/// You should call <see cref="FilterData" /> once your collection of points has
		/// been constructed.  You may need to call <see cref="FilterData" /> again if
		/// you add points, or if the chart rect changes size (by resizing, printing,
		/// image save, etc.), or if the scale range changes.
		/// You must call <see cref="GraphPane.AxisChange()" /> before calling
		/// this method so that the <see cref="Chart.Rect">GraphPane.Chart.Rect</see>
		/// and the scale ranges are valid.  This method is not valid for
		/// ordinal axes (but ordinal axes don't make sense for very large datasets
		/// anyway).
		/// </remarks>
		/// <param name="pane">The <see cref="GraphPane" /> into which the data
		/// will be plotted. </param>
		/// <param name="yAxis">The <see cref="Axis" /> class to be used in the Y direction
		/// for plotting these data.  This can be a <see cref="YAxis" /> or a 
		/// <see cref="Y2Axis" />, and can be a primary or secondary axis (if multiple Y or Y2
		/// axes are being used).
		/// </param>
		/// <param name="xAxis">The <see cref="Axis" /> class to be used in the X direction
		/// for plotting these data.  This can be an <see cref="XAxis" /> or a 
		/// <see cref="X2Axis" />.
		/// </param>
		public void FilterData( GraphPane pane, Axis xAxis, Axis yAxis )
		{
			if ( _visibleIndicies == null || _visibleIndicies.Length < base.Count )
				_visibleIndicies = new int[base.Count];

			_filteredCount = 0;
			_isFiltered = true;

			int width = (int)pane.Chart.Rect.Width;
			int height = (int)pane.Chart.Rect.Height;
			if ( width <= 0 || height <= 0 )
				throw new IndexOutOfRangeException( "Error in FilterData: Chart rect not valid" );

			bool[,] usedArray = new bool[width, height];
			for ( int i = 0; i < width; i++ )
				for ( int j = 0; j < height; j++ )
					usedArray[i, j] = false;

			xAxis.Scale.SetupScaleData( pane, xAxis );
			yAxis.Scale.SetupScaleData( pane, yAxis );

			int n = _filterMode < 0 ? 0 : _filterMode;
			int left = (int)pane.Chart.Rect.Left;
			int top = (int)pane.Chart.Rect.Top;

			for ( int i=0; i<base.Count; i++ )
			{
				DataPoint dp = base[i];
				int x = (int)( xAxis.Scale.Transform( dp.X ) + 0.5 ) - left;
				int y = (int)( yAxis.Scale.Transform( dp.Y ) + 0.5 ) - top;

				if ( x >= 0 && x < width && y >= 0 && y < height )
				{
					bool used = false;
					if ( n <= 0 )
						used = usedArray[x, y];
					else
					{
						for ( int ix = x - n; ix <= x + n; ix++ )
							for ( int iy = y - n; iy <= y + n; iy++ )
								used |= ( ix >= 0 && ix < width && iy >= 0 && iy < height && usedArray[ix, iy] );
					}

					if ( !used )
					{
						usedArray[x, y] = true;
						_visibleIndicies[_filteredCount] = i;
						_filteredCount++;
					}
				}
			}
		}
	}
}
