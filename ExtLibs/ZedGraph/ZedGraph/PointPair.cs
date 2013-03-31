//============================================================================
//PointPair Class
//Copyright © 2004  Jerry Vos
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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using IComparer	= System.Collections.IComparer;

#if ( !DOTNET1 )	// Is this a .Net 2 compilation?
using System.Collections.Generic;
#endif


namespace ZedGraph
{
	/// <summary>
	/// A simple point represented by an (X,Y,Z) group of double values.
	/// </summary>
	/// 
	/// <author> Jerry Vos modified by John Champion </author>
	/// <version> $Revision: 3.26 $ $Date: 2007-11-28 02:38:22 $ </version>
	[Serializable]
	public class PointPair : PointPairBase, ISerializable, ICloneable
	{
	#region Member variables
		
		/// <summary>
		/// This PointPair's Z coordinate.  Also used for the lower value (dependent axis)
		/// for <see cref="HiLowBarItem"/> and <see cref="ErrorBarItem" /> charts.
		/// </summary>
		public double Z;

		/// <summary>
		/// A tag object for use by the user.  This can be used to store additional
		/// information associated with the <see cref="PointPair"/>.  ZedGraph never
		/// modifies this value, but if it is a <see cref="String"/> type, it
		/// may be displayed in a <see cref="System.Windows.Forms.ToolTip"/>
		/// within the <see cref="ZedGraphControl"/> object.
		/// </summary>
		/// <remarks>
		/// Note that, if you are going to Serialize ZedGraph data, then any type
		/// that you store in <see cref="Tag"/> must be a serializable type (or
		/// it will cause an exception).
		/// </remarks>
		public object Tag;

	#endregion

	#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public PointPair() : this( 0, 0, 0, null )
		{
		}

		/// <summary>
		/// Creates a point pair with the specified X and Y.
		/// </summary>
		/// <param name="x">This pair's x coordinate.</param>
		/// <param name="y">This pair's y coordinate.</param>
		public PointPair( double x, double y )
			: this ( x, y, 0, null )
		{
		}

		/// <summary>
		/// Creates a point pair with the specified X, Y, and
		/// label (<see cref="Tag"/>).
		/// </summary>
		/// <param name="x">This pair's x coordinate.</param>
		/// <param name="y">This pair's y coordinate.</param>
		/// <param name="label">This pair's string label (<see cref="Tag"/>)</param>
		public PointPair( double x, double y, string label )
			: this( x, y, 0, label as object )
		{
		}

		/// <summary>
		/// Creates a point pair with the specified X, Y, and base value.
		/// </summary>
		/// <param name="x">This pair's x coordinate.</param>
		/// <param name="y">This pair's y coordinate.</param>
		/// <param name="z">This pair's z or lower dependent coordinate.</param>
		public PointPair( double x, double y, double z )
			: this( x, y, z, null )
		{
		}

		/// <summary>
		/// Creates a point pair with the specified X, Y, base value, and
		/// string label (<see cref="Tag"/>).
		/// </summary>
		/// <param name="x">This pair's x coordinate.</param>
		/// <param name="y">This pair's y coordinate.</param>
		/// <param name="z">This pair's z or lower dependent coordinate.</param>
		/// <param name="label">This pair's string label (<see cref="Tag"/>)</param>
		public PointPair( double x, double y, double z, string label )
			: this( x, y, z, label as object )
		{
		}

		/// <summary>
		/// Creates a point pair with the specified X, Y, base value, and
		/// (<see cref="Tag"/>).
		/// </summary>
		/// <param name="x">This pair's x coordinate.</param>
		/// <param name="y">This pair's y coordinate.</param>
		/// <param name="z">This pair's z or lower dependent coordinate.</param>
		/// <param name="tag">This pair's <see cref="Tag"/> property</param>
		public PointPair( double x, double y, double z, object tag )
			: base( x, y )
		{
			this.Z = z;
			this.Tag = tag;
		}

		/// <summary>
		/// Creates a point pair from the specified <see cref="PointF"/> struct.
		/// </summary>
		/// <param name="pt">The <see cref="PointF"/> struct from which to get the
		/// new <see cref="PointPair"/> values.</param>
		public PointPair( PointF pt ) : this( pt.X, pt.Y, 0, null )
		{
		}

		/// <summary>
		/// The PointPair copy constructor.
		/// </summary>
		/// <param name="rhs">The basis for the copy.</param>
		public PointPair( PointPair rhs ) : base( rhs )
		{
			this.Z = rhs.Z;

			if ( rhs.Tag is ICloneable )
				this.Tag = ((ICloneable) rhs.Tag).Clone();
			else
				this.Tag = rhs.Tag;
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
		public PointPair Clone()
		{
			return new PointPair( this );
		}

	#endregion

	#region Serialization
		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema2 = 11;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected PointPair( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			Z = info.GetDouble( "Z" );
			Tag = info.GetValue( "Tag", typeof(object) );
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
			info.AddValue( "schema2", schema2 );
			info.AddValue( "Z", Z );
			info.AddValue( "Tag", Tag );
		}
	#endregion

	#region Properties

		/// <summary>
		/// Readonly value that determines if either the X, Y, or Z
		/// coordinate in this PointPair is an invalid (not plotable) value.
		/// It is considered invalid if it is missing (equal to System.Double.Max),
		/// Infinity, or NaN.
		/// </summary>
		/// <returns>true if any value is invalid</returns>
		public bool IsInvalid3D
		{
			get { return	this.X == PointPair.Missing ||
							this.Y == PointPair.Missing ||
							this.Z == PointPair.Missing ||
							Double.IsInfinity( this.X ) ||
							Double.IsInfinity( this.Y ) ||
							Double.IsInfinity( this.Z ) ||
							Double.IsNaN( this.X ) ||
							Double.IsNaN( this.Y ) ||
							Double.IsNaN( this.Z );
				}
		}

		/// <summary>
		/// The "low" value for this point (lower dependent-axis value).
		/// This is really just an alias for <see cref="PointPair.Z"/>.
		/// </summary>
		/// <value>The lower dependent value for this <see cref="PointPair"/>.</value>
		public double LowValue
		{
			get { return this.Z; }
			set { this.Z = value; }
		}

		/// <summary>
		/// The ColorValue property is just an alias for the <see cref="PointPair.Z" />
		/// property.
		/// </summary>
		/// <remarks>
		/// For other types, such as the <see cref="StockPt"/>, the <see cref="StockPt" />
		/// can be mapped to a unique value.  This is used with the
		/// <see cref="FillType.GradientByColorValue" /> option.
		/// </remarks>
		virtual public double ColorValue
		{
			get { return Z; }
			set { Z = value; }
		}

	#endregion

	#region Inner classes

	#if ( DOTNET1 )	// Is this a .Net 1.1 compilation?

		/// <summary>
		/// Compares points based on their y values.  Is setup to be used in an
		/// ascending order sort.
		/// <seealso cref="System.Collections.ArrayList.Sort()"/>
		/// </summary>
		public class PointPairComparerY : IComparer
		{
		
			/// <summary>
			/// Compares two <see cref="PointPair"/>s.
			/// </summary>
			/// <param name="l">Point to the left.</param>
			/// <param name="r">Point to the right.</param>
			/// <returns>-1, 0, or 1 depending on l.Y's relation to r.Y</returns>
			public int Compare( object l, object r ) 
			{
				PointPair pl = (PointPair) l;
				PointPair pr = (PointPair) r;

				if (pl == null && pr == null) 
				{
					return 0;
				} 
				else if (pl == null && pr != null) 
				{
					return -1;
				} 
				else if (pl != null && pr == null) 
				{
					return 1;
				} 

				double lY = pl.Y;
				double rY = pr.Y;

				if (System.Math.Abs(lY - rY) < .000000001)
					return 0;
				
				return lY < rY ? -1 : 1;
			}
		}
	
		/// <summary>
		/// Compares points based on their x values.  Is setup to be used in an
		/// ascending order sort.
		/// <seealso cref="System.Collections.ArrayList.Sort()"/>
		/// </summary>
		public class PointPairComparer : IComparer
		{
			private SortType sortType;
			
			/// <summary>
			/// Constructor for PointPairComparer.
			/// </summary>
			/// <param name="type">The axis type on which to sort.</param>
			public PointPairComparer( SortType type )
			{
				this.sortType = type;
			}
			
			/// <summary>
			/// Compares two <see cref="PointPair"/>s.
			/// </summary>
			/// <param name="l">Point to the left.</param>
			/// <param name="r">Point to the right.</param>
			/// <returns>-1, 0, or 1 depending on l.X's relation to r.X</returns>
			public int Compare( object l, object r ) 
			{				 
				PointPair pl = (PointPair) l;
				PointPair pr = (PointPair) r;

				if ( pl == null && pr == null ) 
					return 0;
				else if ( pl == null && pr != null ) 
					return -1;
				else if ( pl != null && pr == null ) 
					return 1;

				double lVal, rVal;
			
				if ( sortType == SortType.XValues )
				{
					lVal = pl.X;
					rVal = pr.X;
				}
				else
				{
					lVal = pl.Y;
					rVal = pr.Y;
				}
				
				if ( lVal == PointPair.Missing || Double.IsInfinity( lVal ) || Double.IsNaN( lVal ) )
					pl = null;
				if ( rVal == PointPair.Missing || Double.IsInfinity( rVal ) || Double.IsNaN( rVal ) )
					pr = null;

				if ( ( pl == null && pr == null ) || ( System.Math.Abs( lVal - rVal ) < 1e-10 ) )
					return 0;
				else if ( pl == null && pr != null ) 
					return -1;
				else if ( pl != null && pr == null ) 
					return 1;
				else
					return lVal < rVal ? -1 : 1;
			}
		}
	
	#else		// Otherwise, it's .Net 2.0, so use generics

		/// <summary>
		/// Compares points based on their y values.  Is setup to be used in an
		/// ascending order sort.
		/// <seealso cref="System.Collections.ArrayList.Sort()"/>
		/// </summary>
		public class PointPairComparerY : IComparer<PointPair>
		{

			/// <summary>
			/// Compares two <see cref="PointPair"/>s.
			/// </summary>
			/// <param name="l">Point to the left.</param>
			/// <param name="r">Point to the right.</param>
			/// <returns>-1, 0, or 1 depending on l.Y's relation to r.Y</returns>
			public int Compare( PointPair l, PointPair r )
			{
				if ( l == null && r == null )
				{
					return 0;
				}
				else if ( l == null && r != null )
				{
					return -1;
				}
				else if ( l != null && r == null )
				{
					return 1;
				}

				double lY = l.Y;
				double rY = r.Y;

				if ( System.Math.Abs( lY - rY ) < .000000001 )
					return 0;

				return lY < rY ? -1 : 1;
			}
		}

		/// <summary>
		/// Compares points based on their x values.  Is setup to be used in an
		/// ascending order sort.
		/// <seealso cref="System.Collections.ArrayList.Sort()"/>
		/// </summary>
		public class PointPairComparer : IComparer<PointPair>
		{
			private SortType sortType;

			/// <summary>
			/// Constructor for PointPairComparer.
			/// </summary>
			/// <param name="type">The axis type on which to sort.</param>
			public PointPairComparer( SortType type )
			{
				this.sortType = type;
			}

			/// <summary>
			/// Compares two <see cref="PointPair"/>s.
			/// </summary>
			/// <param name="l">Point to the left.</param>
			/// <param name="r">Point to the right.</param>
			/// <returns>-1, 0, or 1 depending on l.X's relation to r.X</returns>
			public int Compare( PointPair l, PointPair r )
			{
				if ( l == null && r == null )
					return 0;
				else if ( l == null && r != null )
					return -1;
				else if ( l != null && r == null )
					return 1;

				double lVal, rVal;

				if ( sortType == SortType.XValues )
				{
					lVal = l.X;
					rVal = r.X;
				}
				else
				{
					lVal = l.Y;
					rVal = r.Y;
				}

				if ( lVal == PointPair.Missing || Double.IsInfinity( lVal ) || Double.IsNaN( lVal ) )
					l = null;
				if ( rVal == PointPair.Missing || Double.IsInfinity( rVal ) || Double.IsNaN( rVal ) )
					r = null;

				if ( ( l == null && r == null ) || ( System.Math.Abs( lVal - rVal ) < 1e-100 ) )
					return 0;
				else if ( l == null && r != null )
					return -1;
				else if ( l != null && r == null )
					return 1;
				else
					return lVal < rVal ? -1 : 1;
			}
		}

	#endif

	#endregion

	#region Methods

		/// <summary>
		/// Compare two <see cref="PointPair"/> objects for equality.  To be equal, X, Y, and Z
		/// must be exactly the same between the two objects.
		/// </summary>
		/// <param name="obj">The <see cref="PointPair"/> object to be compared with.</param>
		/// <returns>true if the <see cref="PointPair"/> objects are equal, false otherwise</returns>
		public override bool Equals( object obj )
		{
			PointPair rhs = obj as PointPair;
			return this.X == rhs.X && this.Y == rhs.Y && this.Z == rhs.Z;
		}

		/// <summary>
		/// Return the HashCode from the base class.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Format this PointPair value using the default format.  Example:  "( 12.345, -16.876 )".
		/// The two double values are formatted with the "g" format type.
		/// </summary>
		/// <param name="isShowZ">true to show the third "Z" or low dependent value coordinate</param>
		/// <returns>A string representation of the PointPair</returns>
		virtual public string ToString( bool isShowZ )
		{
			return this.ToString( PointPair.DefaultFormat, isShowZ );
		}

		/// <summary>
		/// Format this PointPair value using a general format string.
		/// Example:  a format string of "e2" would give "( 1.23e+001, -1.69e+001 )".
		/// If <see paramref="isShowZ"/>
		/// is true, then the third "Z" coordinate is also shown.
		/// </summary>
		/// <param name="format">A format string that will be used to format each of
		/// the two double type values (see <see cref="System.Double.ToString()"/>).</param>
		/// <returns>A string representation of the PointPair</returns>
		/// <param name="isShowZ">true to show the third "Z" or low dependent value coordinate</param>
		virtual public string ToString( string format, bool isShowZ )
		{
			return "( " + this.X.ToString( format ) +
					", " + this.Y.ToString( format ) + 
					( isShowZ ? ( ", " + this.Z.ToString( format ) ) : "" )
					+ " )";
		}

		/// <summary>
		/// Format this PointPair value using different general format strings for the X, Y, and Z values.
		/// Example:  a format string of "e2" would give "( 1.23e+001, -1.69e+001 )".
		/// </summary>
		/// <param name="formatX">A format string that will be used to format the X
		/// double type value (see <see cref="System.Double.ToString()"/>).</param>
		/// <param name="formatY">A format string that will be used to format the Y
		/// double type value (see <see cref="System.Double.ToString()"/>).</param>
		/// <param name="formatZ">A format string that will be used to format the Z
		/// double type value (see <see cref="System.Double.ToString()"/>).</param>
		/// <returns>A string representation of the PointPair</returns>
		public string ToString( string formatX, string formatY, string formatZ )
		{
			return "( " + this.X.ToString( formatX ) +
					", " + this.Y.ToString( formatY ) + 
					", " + this.Z.ToString( formatZ ) + 
					" )";
		}

	#endregion

	}
}
