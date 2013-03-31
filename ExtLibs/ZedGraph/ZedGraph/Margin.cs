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
	/// Class that handles that stores the margin properties for the GraphPane
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class Margin : ICloneable, ISerializable
	{
		/// <summary>
		/// Private fields that store the size of the margin around the edge of the pane which will be
		/// kept blank.  Use the public properties <see cref="Margin.Left"/>, <see cref="Margin.Right"/>,
		/// <see cref="Margin.Top"/>, <see cref="Margin.Bottom"/> to access these values.
		/// </summary>
		/// <value>Units are points (1/72 inch)</value>
		protected float	_left,
								_right,
								_top,
								_bottom;

	#region Constructors

		/// <summary>
		/// Constructor to build a <see cref="Margin" /> from the default values.
		/// </summary>
		public Margin()
		{
			_left = Default.Left;
			_right = Default.Right;
			_top = Default.Top;
			_bottom = Default.Bottom;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="rhs">the <see cref="Margin" /> instance to be copied.</param>
		public Margin( Margin rhs )
		{
			_left = rhs._left;
			_right = rhs._right;
			_top = rhs._top;
			_bottom = rhs._bottom;
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
		public Margin Clone()
		{
			return new Margin( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a float value that determines the margin area between the left edge of the
		/// <see cref="PaneBase.Rect"/> rectangle and the features of the graph.
		/// </summary>
		/// <value>This value is in units of points (1/72 inch), and is scaled
		/// linearly with the graph size.</value>
		/// <seealso cref="Default.Left"/>
		/// <seealso cref="PaneBase.IsFontsScaled"/>
		/// <seealso cref="Right"/>
		/// <seealso cref="Top"/>
		/// <seealso cref="Bottom"/>
		public float Left
		{
			get { return _left; }
			set { _left = value; }
		}
		/// <summary>
		/// Gets or sets a float value that determines the margin area between the right edge of the
		/// <see cref="PaneBase.Rect"/> rectangle and the features of the graph.
		/// </summary>
		/// <value>This value is in units of points (1/72 inch), and is scaled
		/// linearly with the graph size.</value>
		/// <seealso cref="Default.Right"/>
		/// <seealso cref="PaneBase.IsFontsScaled"/>
		/// <seealso cref="Left"/>
		/// <seealso cref="Top"/>
		/// <seealso cref="Bottom"/>
		public float Right
		{
			get { return _right; }
			set { _right = value; }
		}
		/// <summary>
		/// Gets or sets a float value that determines the margin area between the top edge of the
		/// <see cref="PaneBase.Rect"/> rectangle and the features of the graph.
		/// </summary>
		/// <value>This value is in units of points (1/72 inch), and is scaled
		/// linearly with the graph size.</value>
		/// <seealso cref="Default.Top"/>
		/// <seealso cref="PaneBase.IsFontsScaled"/>
		/// <seealso cref="Left"/>
		/// <seealso cref="Right"/>
		/// <seealso cref="Bottom"/>
		public float Top
		{
			get { return _top; }
			set { _top = value; }
		}
		/// <summary>
		/// Gets or sets a float value that determines the margin area between the bottom edge of the
		/// <see cref="PaneBase.Rect"/> rectangle and the features of the graph.
		/// </summary>
		/// <value>This value is in units of points (1/72 inch), and is scaled
		/// linearly with the graph size.</value>
		/// <seealso cref="Default.Bottom"/>
		/// <seealso cref="PaneBase.IsFontsScaled"/>
		/// <seealso cref="Left"/>
		/// <seealso cref="Right"/>
		/// <seealso cref="Top"/>
		public float Bottom
		{
			get { return _bottom; }
			set { _bottom = value; }
		}

		/// <summary>
		/// Concurrently sets all outer margin values to a single value.
		/// </summary>
		/// <value>This value is in units of points (1/72 inch), and is scaled
		/// linearly with the graph size.</value>
		/// <seealso cref="PaneBase.IsFontsScaled"/>
		/// <seealso cref="Bottom"/>
		/// <seealso cref="Left"/>
		/// <seealso cref="Right"/>
		/// <seealso cref="Top"/>
		public float All
		{
			set
			{
				_bottom = value;
				_top = value;
				_left = value;
				_right = value;
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
		protected Margin( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_left = info.GetSingle( "left" );
			_right = info.GetSingle( "right" );
			_top = info.GetSingle( "top" );
			_bottom = info.GetSingle( "bottom" );
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

			info.AddValue( "left", _left );
			info.AddValue( "right", _right );
			info.AddValue( "top", _top );
			info.AddValue( "bottom", _bottom );
		}

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the default property values for the <see cref="Margin"/> class.
		/// </summary>
		public class Default
		{
			/// <summary>
			/// The default value for the <see cref="Margin.Left"/> property, which is
			/// the size of the space on the left side of the <see cref="PaneBase.Rect"/>.
			/// </summary>
			/// <value>Units are points (1/72 inch)</value>
			public static float Left = 10.0F;
			/// <summary>
			/// The default value for the <see cref="Margin.Right"/> property, which is
			/// the size of the space on the right side of the <see cref="PaneBase.Rect"/>.
			/// </summary>
			/// <value>Units are points (1/72 inch)</value>
			public static float Right = 10.0F;
			/// <summary>
			/// The default value for the <see cref="Margin.Top"/> property, which is
			/// the size of the space on the top side of the <see cref="PaneBase.Rect"/>.
			/// </summary>
			/// <value>Units are points (1/72 inch)</value>
			public static float Top = 10.0F;
			/// <summary>
			/// The default value for the <see cref="Margin.Bottom"/> property, which is
			/// the size of the space on the bottom side of the <see cref="PaneBase.Rect"/>.
			/// </summary>
			/// <value>Units are points (1/72 inch)</value>
			public static float Bottom = 10.0F;

		}

	#endregion

	}
}
