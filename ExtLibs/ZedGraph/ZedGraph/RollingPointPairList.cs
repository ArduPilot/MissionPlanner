//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
//RollingPointPairList class Copyright © 2006 by Colin Green
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
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// A class that provides a rolling list of <see cref="PointPair" /> objects.
	/// This is essentially a 
	/// first-in-first-out (FIFO) queue with a fixed capacity which allows 'rolling' 
	/// (or oscilloscope like) graphs to be be animated without having the overhead of an
	/// ever-growing ArrayList.
	/// 
	/// The queue is constructed with a fixed capacity and new points can be enqueued. When the 
	/// capacity is reached the oldest (first in) PointPair is overwritten. However, when 
	/// accessing via <see cref="IPointList" />, the <see cref="PointPair" /> objects are
	/// seen in the order in which they were enqeued.
	///
	/// RollingPointPairList supports data editing through the <see cref="IPointListEdit" />
	/// interface.
	/// 
	/// <author>Colin Green with mods by John Champion</author>
	/// <version> $Date: 2007-11-05 04:33:26 $ </version>
	/// </summary>
	[Serializable]
	public class RollingPointPairList : IPointList, ISerializable, IPointListEdit
	{

	#region Fields

		/// <summary>
		/// An array of PointPair objects that acts as the underlying buffer.
		/// </summary>
		protected PointPair[] _mBuffer;

		/// <summary>
		/// The index of the previously enqueued item. -1 if buffer is empty.
		/// </summary>
		protected int _headIdx;

		/// <summary>
		/// The index of the next item to be dequeued. -1 if buffer is empty.
		/// </summary>
		protected int _tailIdx;

	#endregion

	#region Constructors

		/// <summary>
		/// Constructs an empty buffer with the specified capacity.
		/// </summary>
		/// <param name="capacity">Number of elements in the rolling list.  This number
		/// cannot be changed once the RollingPointPairList is constructed.</param>
		public RollingPointPairList( int capacity )
			: this( capacity, false )
		{
			_mBuffer = new PointPair[capacity];
			_headIdx = _tailIdx = -1;
		}

		/// <summary>
		/// Constructs an empty buffer with the specified capacity.  Pre-allocates space
		/// for all PointPair's in the list if <paramref name="preLoad"/> is true.
		/// </summary>
		/// <param name="capacity">Number of elements in the rolling list.  This number
		/// cannot be changed once the RollingPointPairList is constructed.</param>
		/// <param name="preLoad">true to pre-allocate all PointPair instances in
		/// the list, false otherwise.  Note that in order to be memory efficient,
		/// the <see cref="Add(double,double,double)"/> method should be used to add
		/// data.  Avoid the <see cref="Add(PointPair)"/> method.
		/// </param>
		/// <seealso cref="Add(double,double,double)"/>
		public RollingPointPairList( int capacity, bool preLoad )
		{
			_mBuffer = new PointPair[capacity];
			_headIdx = _tailIdx = -1;

			if ( preLoad )
				for ( int i = 0; i < capacity; i++ )
					_mBuffer[i] = new PointPair();
		}

		/// <summary>
		/// Constructs a buffer with a copy of the items within the provided
		/// <see cref="IPointList" />.
		/// The <see cref="Capacity" /> is set to the length of the provided list.
		/// </summary>
		/// <param name="rhs">The <see cref="IPointList" /> to be copied.</param>
		public RollingPointPairList( IPointList rhs )
		{
			_mBuffer = new PointPair[rhs.Count];

			for ( int i = 0; i < rhs.Count; i++ )
			{
				_mBuffer[i] = new PointPair( rhs[i] );
			}

			_headIdx = rhs.Count - 1;
			_tailIdx = 0;
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets the capacity of the rolling buffer.
		/// </summary>
		public int Capacity
		{
			get { return _mBuffer.Length; }
		}

		/// <summary>
		/// Gets the count of items within the rolling buffer. Note that this may be less than
		/// the capacity.
		/// </summary>
		public int Count
		{
			get
			{
				if ( _headIdx == -1 )
					return 0;

				if ( _headIdx > _tailIdx )
					return ( _headIdx - _tailIdx ) + 1;

				if ( _tailIdx > _headIdx )
					return ( _mBuffer.Length - _tailIdx ) + _headIdx + 1;

				return 1;
			}
		}

		/// <summary>
		/// Gets a bolean that indicates if the buffer is empty.
		/// Alternatively you can test Count==0.
		/// </summary>
		public bool IsEmpty
		{
			get { return _headIdx == -1; }
		}

		/// <summary>
		/// Gets or sets the <see cref="PointPair" /> at the specified index in the buffer.
		/// </summary>
		/// <remarks>
		/// Index must be within the current size of the buffer, e.g., the set
		/// method will not expand the buffer even if <see cref="Capacity" /> is available
		/// </remarks>
		public PointPair this[int index]
		{
			get
			{
				if ( index >= Count || index < 0 )
					throw new ArgumentOutOfRangeException();

				index += _tailIdx;
				if ( index >= _mBuffer.Length )
					index -= _mBuffer.Length;

				return _mBuffer[index];
			}
			set
			{
				if ( index >= Count || index < 0 )
					throw new ArgumentOutOfRangeException();

				index += _tailIdx;
				if ( index >= _mBuffer.Length )
					index -= _mBuffer.Length;

				_mBuffer[index] = value;
			}

		}

	#endregion

	#region Public Methods

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
		public RollingPointPairList Clone()
		{
			return new RollingPointPairList( this );
		}

		/// <summary>
		/// Clear the buffer of all <see cref="PointPair"/> objects.
		/// Note that the <see cref="Capacity" /> remains unchanged.
		/// </summary>
		public void Clear()
		{
			_headIdx = _tailIdx = -1;
		}

		/// <summary>
		/// Calculate that the next index in the buffer that should receive a new data point.
		/// Note that this method actually advances the buffer, so a datapoint should be
		/// added at _mBuffer[_headIdx].
		/// </summary>
		/// <returns>The index position of the new head element</returns>
		private int GetNextIndex()
		{
			if ( _headIdx == -1 )
			{	// buffer is currently empty.
				_headIdx = _tailIdx = 0;
			}
			else
			{
				// Determine the index to write to.
				if ( ++_headIdx == _mBuffer.Length )
				{	// Wrap around.
					_headIdx = 0;
				}

				if ( _headIdx == _tailIdx )
				{	// Buffer overflow. Increment tailIdx.
					if ( ++_tailIdx == _mBuffer.Length )
					{	// Wrap around.
						_tailIdx = 0;
					}
				}
			}

			return _headIdx;
		}

		/// <summary>
		/// Add a <see cref="PointPair"/> onto the head of the queue,
		/// overwriting old values if the buffer is full.
		/// </summary>
		/// <param name="item">The <see cref="PointPair" /> to be added.</param>
		public void Add( PointPair item )
		{
			_mBuffer[ GetNextIndex() ] = item;
		}

		/// <summary>
		/// Add an <see cref="IPointList"/> object to the head of the queue.
		/// </summary>
		/// <param name="pointList">A reference to the <see cref="IPointList"/> object to
		/// be added</param>
		public void Add( IPointList pointList )
		{   // A slightly more efficient approach would be to determine where the new points should placed within
			// the buffer and to then copy them in directly - updating the head and tail indexes appropriately.
			for ( int i = 0; i < pointList.Count; i++ )
				Add( pointList[i] );
		}

		/// <summary>
		/// Remove an old item from the tail of the queue.
		/// </summary>
		/// <returns>The removed item. Throws an <see cref="InvalidOperationException" />
		/// if the buffer was empty. 
		/// Check the buffer's length (<see cref="Count" />) or the <see cref="IsEmpty" />
		/// property to avoid exceptions.</returns>
		public PointPair Remove()
		{
			if ( _tailIdx == -1 )
			{	// buffer is currently empty.
				throw new InvalidOperationException( "buffer is empty." );
			}

			PointPair o = _mBuffer[_tailIdx];

			if ( _tailIdx == _headIdx )
			{	// The buffer is now empty.
				_headIdx = _tailIdx = -1;
				return o;
			}

			if ( ++_tailIdx == _mBuffer.Length )
			{	// Wrap around.
				_tailIdx = 0;
			}

			return o;
		}

		/// <summary>
		/// Remove the <see cref="PointPair" /> at the specified index
		/// </summary>
		/// <remarks>
		/// All items in the queue that lie after <paramref name="index"/> will
		/// be shifted back by one, and the queue will be one item shorter.
		/// </remarks>
		/// <param name="index">The ordinal position of the item to be removed.
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if index is less than
		/// zero or greater than or equal to <see cref="Count" />
		/// </param>
		public void RemoveAt( int index )
		{
			int count = this.Count;

			if ( index >= count || index < 0 )
				throw new ArgumentOutOfRangeException();

			// shift all the items that lie after index back by 1
			for ( int i = index + _tailIdx; i < _tailIdx + count - 1; i++ )
			{
				i = ( i >= _mBuffer.Length ) ? 0 : i;
				int j = i + 1;
				j = ( j >= _mBuffer.Length ) ? 0 : j;
				_mBuffer[i] = _mBuffer[j];
			}

			// Remove the item from the head (it's been duplicated already)
			Pop();
		}

		/// <summary>
		/// Remove a range of <see cref="PointPair" /> objects starting at the specified index
		/// </summary>
		/// <remarks>
		/// All items in the queue that lie after <paramref name="index"/> will
		/// be shifted back, and the queue will be <paramref name="count" /> items shorter.
		/// </remarks>
		/// <param name="index">The ordinal position of the item to be removed.
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if index is less than
		/// zero or greater than or equal to <see cref="Count" />
		/// </param>
		/// <param name="count">The number of items to be removed.  Throws an
		/// <see cref="ArgumentOutOfRangeException" /> if <paramref name="count" /> is less than zero
		/// or greater than the total available items in the queue</param>
		public void RemoveRange( int index, int count )
		{
			int totalCount = this.Count;

			if ( index >= totalCount || index < 0 || count < 0 || count > totalCount )
				throw new ArgumentOutOfRangeException();

			for ( int i = 0; i < count; i++ )
				this.RemoveAt( index );
		}

		/// <summary>
		/// Pop an item off the head of the queue.
		/// </summary>
		/// <returns>The popped item. Throws an exception if the buffer was empty.</returns>
		public PointPair Pop()
		{
			if ( _tailIdx == -1 )
			{	// buffer is currently empty.
				throw new InvalidOperationException( "buffer is empty." );
			}

			PointPair o = _mBuffer[_headIdx];

			if ( _tailIdx == _headIdx )
			{	// The buffer is now empty.
				_headIdx = _tailIdx = -1;
				return o;
			}

			if ( --_headIdx == -1 )
			{	// Wrap around.
				_headIdx = _mBuffer.Length - 1;
			}

			return o;
		}

		/// <summary>
		/// Peek at the <see cref="PointPair" /> item at the head of the queue.
		/// </summary>
		/// <returns>The <see cref="PointPair" /> item at the head of the queue.
		/// Throws an <see cref="InvalidOperationException" /> if the buffer was empty.
		/// </returns>
		public PointPair Peek()
		{
			if ( _headIdx == -1 )
			{	// buffer is currently empty.
				throw new InvalidOperationException( "buffer is empty." );
			}

			return _mBuffer[_headIdx];
		}

	#endregion

	#region Auxilliary Methods

		/// <summary>
		/// Add a set of values onto the head of the queue,
		/// overwriting old values if the buffer is full.
		/// </summary>
		/// <remarks>
		/// This method is much more efficient that the <see cref="Add(PointPair)">Add(PointPair)</see>
		/// method, since it does not require that a new PointPair instance be provided.
		/// If the buffer already contains a <see cref="PointPair"/> at the head position,
		/// then the x, y, z, and tag values will be copied into the existing PointPair.
		/// Otherwise, a new PointPair instance must be created.
		/// In this way, each PointPair position in the rolling list will only be allocated one time.
		/// To truly be memory efficient, the <see cref="Remove" />, <see cref="RemoveAt" />,
		/// and <see cref="Pop" /> methods should be avoided.  Also, the <paramref name="tag"/> property
		/// for this method should be null, since it is a reference type.
		/// </remarks>
		/// <param name="x">The X value</param>
		/// <param name="y">The Y value</param>
		/// <param name="z">The Z value</param>
		/// <param name="tag">The Tag value for the PointPair</param>
		public void Add( double x, double y, double z, object tag )
		{
			// advance the rolling list
			GetNextIndex();

			if ( _mBuffer[_headIdx] == null )
				_mBuffer[_headIdx] = new PointPair( x, y, z, tag );
			else
			{
				_mBuffer[_headIdx].X = x;
				_mBuffer[_headIdx].Y = y;
				_mBuffer[_headIdx].Z = z;
				_mBuffer[_headIdx].Tag = tag;
			}
		}

		/// <summary>
		/// Add a set of values onto the head of the queue,
		/// overwriting old values if the buffer is full.
		/// </summary>
		/// <remarks>
		/// This method is much more efficient that the <see cref="Add(PointPair)">Add(PointPair)</see>
		/// method, since it does not require that a new PointPair instance be provided.
		/// If the buffer already contains a <see cref="PointPair"/> at the head position,
		/// then the x, y, z, and tag values will be copied into the existing PointPair.
		/// Otherwise, a new PointPair instance must be created.
		/// In this way, each PointPair position in the rolling list will only be allocated one time.
		/// To truly be memory efficient, the <see cref="Remove" />, <see cref="RemoveAt" />,
		/// and <see cref="Pop" /> methods should be avoided.
		/// </remarks>
		/// <param name="x">The X value</param>
		/// <param name="y">The Y value</param>
		public void Add( double x, double y )
		{
			Add( x, y, PointPair.Missing, null );
		}

		/// <summary>
		/// Add a set of values onto the head of the queue,
		/// overwriting old values if the buffer is full.
		/// </summary>
		/// <remarks>
		/// This method is much more efficient that the <see cref="Add(PointPair)">Add(PointPair)</see>
		/// method, since it does not require that a new PointPair instance be provided.
		/// If the buffer already contains a <see cref="PointPair"/> at the head position,
		/// then the x, y, z, and tag values will be copied into the existing PointPair.
		/// Otherwise, a new PointPair instance must be created.
		/// In this way, each PointPair position in the rolling list will only be allocated one time.
		/// To truly be memory efficient, the <see cref="Remove" />, <see cref="RemoveAt" />,
		/// and <see cref="Pop" /> methods should be avoided.  Also, the <paramref name="tag"/> property
		/// for this method should be null, since it is a reference type.
		/// </remarks>
		/// <param name="x">The X value</param>
		/// <param name="y">The Y value</param>
		/// <param name="tag">The Tag value for the PointPair</param>
		public void Add( double x, double y, object tag )
		{
			Add( x, y, PointPair.Missing, tag );
		}

		/// <summary>
		/// Add a set of values onto the head of the queue,
		/// overwriting old values if the buffer is full.
		/// </summary>
		/// <remarks>
		/// This method is much more efficient that the <see cref="Add(PointPair)">Add(PointPair)</see>
		/// method, since it does not require that a new PointPair instance be provided.
		/// If the buffer already contains a <see cref="PointPair"/> at the head position,
		/// then the x, y, z, and tag values will be copied into the existing PointPair.
		/// Otherwise, a new PointPair instance must be created.
		/// In this way, each PointPair position in the rolling list will only be allocated one time.
		/// To truly be memory efficient, the <see cref="Remove" />, <see cref="RemoveAt" />,
		/// and <see cref="Pop" /> methods should be avoided.
		/// </remarks>
		/// <param name="x">The X value</param>
		/// <param name="y">The Y value</param>
		/// <param name="z">The Z value</param>
		public void Add( double x, double y, double z )
		{
			Add( x, y, z, null );
		}

		/// <summary>
		/// Add a set of points to the <see cref="RollingPointPairList"/>
		/// from two arrays of type double.
		/// If either array is null, then a set of ordinal values is automatically
		/// generated in its place (see <see cref="AxisType.Ordinal"/>).
		/// If the arrays are of different size, then the larger array prevails and the
		/// smaller array is padded with <see cref="PointPairBase.Missing"/> values.
		/// </summary>
		/// <param name="x">A double[] array of X values</param>
		/// <param name="y">A double[] array of Y values</param>
		public void Add( double[] x, double[] y )
		{
			int len = 0;

			if ( x != null )
				len = x.Length;
			if ( y != null && y.Length > len )
				len = y.Length;

			for ( int i = 0; i < len; i++ )
			{
				PointPair point = new PointPair( 0, 0, 0 );
				if ( x == null )
					point.X = (double)i + 1.0;
				else if ( i < x.Length )
					point.X = x[i];
				else
					point.X = PointPair.Missing;

				if ( y == null )
					point.Y = (double)i + 1.0;
				else if ( i < y.Length )
					point.Y = y[i];
				else
					point.Y = PointPair.Missing;

				Add( point );
			}
		}

		/// <summary>
		/// Add a set of points to the <see cref="RollingPointPairList"/> from
		/// three arrays of type double.
		/// If the X or Y array is null, then a set of ordinal values is automatically
		/// generated in its place (see <see cref="AxisType.Ordinal"/>.
		/// If the <see paramref="z"/> value
		/// is null, then it is set to zero.
		/// If the arrays are of different size, then the larger array prevails and the
		/// smaller array is padded with <see cref="PointPairBase.Missing"/> values.
		/// </summary>
		/// <param name="x">A double[] array of X values</param>
		/// <param name="y">A double[] array of Y values</param>
		/// <param name="z">A double[] array of Z values</param>
		public void Add( double[] x, double[] y, double[] z )
		{
			int len = 0;

			if ( x != null )
				len = x.Length;
			if ( y != null && y.Length > len )
				len = y.Length;
			if ( z != null && z.Length > len )
				len = z.Length;

			for ( int i = 0; i < len; i++ )
			{
				PointPair point = new PointPair();

				if ( x == null )
					point.X = (double)i + 1.0;
				else if ( i < x.Length )
					point.X = x[i];
				else
					point.X = PointPair.Missing;

				if ( y == null )
					point.Y = (double)i + 1.0;
				else if ( i < y.Length )
					point.Y = y[i];
				else
					point.Y = PointPair.Missing;

				if ( z == null )
					point.Z = (double)i + 1.0;
				else if ( i < z.Length )
					point.Z = z[i];
				else
					point.Z = PointPair.Missing;

				Add( point );
			}
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
		protected RollingPointPairList( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_headIdx = info.GetInt32( "headIdx" );
			_tailIdx = info.GetInt32( "tailIdx" );
			_mBuffer = (PointPair[])info.GetValue( "mBuffer", typeof( PointPair[] ) );
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
			info.AddValue( "headIdx", _headIdx );
			info.AddValue( "tailIdx", _tailIdx );
			info.AddValue( "mBuffer", _mBuffer );
		}

	#endregion

	}
}
