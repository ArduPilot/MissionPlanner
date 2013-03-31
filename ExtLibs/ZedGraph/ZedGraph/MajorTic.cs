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
	/// Class that holds the specific properties for the major tics.  Inherits from
	/// <see cref="MinorTic" />.
	/// </summary>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class MajorTic : MinorTic, ICloneable, ISerializable
	{
		internal bool	_isBetweenLabels;

	#region Constructors

		/// <summary>
		/// Default constructor for <see cref="MajorTic" />.
		/// </summary>
		public MajorTic()
		{
			_size = Default.Size;
			_color = Default.Color;
			_penWidth = Default.PenWidth;

			this.IsOutside = Default.IsOutside;
			this.IsInside = Default.IsInside;
			this.IsOpposite = Default.IsOpposite;
			_isCrossOutside = Default.IsCrossOutside;
			_isCrossInside = Default.IsCrossInside;

			_isBetweenLabels = false;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="rhs">The <see cref="MajorTic" /> that is to be copied.</param>
		public MajorTic( MajorTic rhs )
			: base( rhs )
		{
			_isBetweenLabels = rhs._isBetweenLabels;
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
		public new MajorTic Clone()
		{
			return new MajorTic( this );
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a property that determines whether or not the major tics will be drawn
		/// inbetween the labels, rather than right at the labels.
		/// </summary>
		/// <remarks>
		/// Note that this setting is only
		/// applicable if <see cref="Axis.Type"/> = <see cref="AxisType.Text"/>.
		/// </remarks>
		/// <value>true to place the text between the labels for text axes, false otherwise</value>
		/// <seealso cref="MinorTic.IsOutside"/>
		/// <seealso cref="MinorTic.IsInside"/>
		/// <seealso cref="MinorTic.IsOpposite"/>
		/// <seealso cref="MinorTic.IsCrossOutside"/>
		/// <seealso cref="MinorTic.IsCrossInside"/>
		public bool IsBetweenLabels
		{
			get { return _isBetweenLabels; }
			set { _isBetweenLabels = value; }
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema2 = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected MajorTic( SerializationInfo info, StreamingContext context ) :
			base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch2 = info.GetInt32( "schema2" );

			_isBetweenLabels = info.GetBoolean( "isBetweenLabels" );
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

			info.AddValue( "isBetweenLabels", _isBetweenLabels );
		}

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="MinorTic"/> class.
		/// </summary>
		public new struct Default
		{
			// Default Axis Properties
			/// <summary>
			/// The default size for the <see cref="Axis"/> tic marks.
			/// (<see cref="MinorTic.Size"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float Size = 5;
			/// <summary>
			/// The default pen width for drawing the <see cref="Axis"/> tic marks.
			/// (<see cref="MinorTic.PenWidth"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float PenWidth = 1.0F;
			/// <summary>
			/// The display mode for the <see cref="Axis"/> major outside tic marks
			/// (<see cref="MinorTic.IsOutside"/> property).
			/// The major tic spacing is controlled by <see cref="Scale.MajorStep"/>.
			/// </summary>
			/// <value>true to show the major tic marks (outside the axis),
			/// false otherwise</value>
			public static bool IsOutside = true;
			/// <summary>
			/// The display mode for the <see cref="Axis"/> major inside tic marks
			/// (<see cref="MinorTic.IsInside"/> property).
			/// The major tic spacing is controlled by <see cref="Scale.MajorStep"/>.
			/// </summary>
			/// <value>true to show the major tic marks (inside the axis),
			/// false otherwise</value>
			public static bool IsInside = true;
			/// <summary>
			/// The display mode for the <see cref="Axis"/> major opposite tic marks
			/// (<see cref="MinorTic.IsOpposite"/> property).
			/// The major tic spacing is controlled by <see cref="Scale.MajorStep"/>.
			/// </summary>
			/// <value>true to show the major tic marks
			/// (inside the axis on the opposite side),
			/// false otherwise</value>
			public static bool IsOpposite = true;

			/// <summary>
			/// The default display mode for the <see cref="Axis"/> major outside 
			/// "cross" tic marks (<see cref="MinorTic.IsCrossOutside"/> property).
			/// </summary>
			/// <remarks>
			/// The "cross" tics are a special, additional set of tic marks that
			/// always appear on the actual axis, even if it has been shifted due
			/// to the <see cref="Axis.Cross" /> setting.  The other tic marks are always
			/// fixed to the edges of the <see cref="Chart.Rect"/>.  The cross tics
			/// are normally not displayed, since, if <see cref="Axis.CrossAuto" /> is true,
			/// they will exactly overlay the "normal" and "inside" tics.  If
			/// <see cref="Axis.CrossAuto"/> is false, then you will most likely want to
			/// enable the cross tics.
			/// The major tic spacing is controlled by <see cref="Scale.MajorStep"/>.
			/// </remarks>
			/// <value>true to show the major cross tic marks, false otherwise</value>
			public static bool IsCrossOutside = false;
			/// <summary>
			/// The default display mode for the <see cref="Axis"/> major inside 
			/// "cross" tic marks (<see cref="MinorTic.IsCrossInside"/> property).
			/// </summary>
			/// <remarks>
			/// The "cross" tics are a special, additional set of tic marks that
			/// always appear on the actual axis, even if it has been shifted due
			/// to the <see cref="Axis.Cross" /> setting.  The other tic marks are always
			/// fixed to the edges of the <see cref="Chart.Rect"/>.  The cross tics
			/// are normally not displayed, since, if <see cref="Axis.CrossAuto" /> is true,
			/// they will exactly overlay the "normal" and "inside" tics.  If
			/// <see cref="Axis.CrossAuto"/> is false, then you will most likely want to
			/// enable the cross tics.
			/// The major tic spacing is controlled by <see cref="Scale.MajorStep"/>.
			/// </remarks>
			/// <value>true to show the major cross tic marks, false otherwise</value>
			public static bool IsCrossInside = false;

			/// <summary>
			/// The default color for major tics (<see cref="MinorTic.Color"/> property).
			/// </summary>
			public static Color Color = Color.Black;
		}

	#endregion

	}
}
