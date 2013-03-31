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
using System.Collections;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// The LinearAsOrdinalScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.LinearAsOrdinal" />.
	/// </summary>
	/// <remarks>
	/// LinearAsOrdinal is an ordinal axis that will have labels formatted with values from the actual data
	/// values of the first <see cref="CurveItem" /> in the <see cref="CurveList" />.
	/// Although the tics are labeled with real data values, the actual points will be
	/// evenly-spaced in spite of the data values.  For example, if the X values of the first curve
	/// are 1, 5, and 100, then the tic labels will show 1, 5, and 100, but they will be equal
	/// distance from each other.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.10 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	class LinearAsOrdinalScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		/// <summary>
		/// Default constructor that defines the owner <see cref="Axis" />
		/// (containing object) for this new object.
		/// </summary>
		/// <param name="owner">The owner, or containing object, of this instance</param>
		public LinearAsOrdinalScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="LinearAsOrdinalScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="LinearAsOrdinalScale" /></param>
		public LinearAsOrdinalScale( Scale rhs, Axis owner )
			: base( rhs, owner )
		{
		}


		/// <summary>
		/// Create a new clone of the current item, with a new owner assignment
		/// </summary>
		/// <param name="owner">The new <see cref="Axis" /> instance that will be
		/// the owner of the new Scale</param>
		/// <returns>A new <see cref="Scale" /> clone.</returns>
		public override Scale Clone( Axis owner )
		{
			return new LinearAsOrdinalScale( this, owner );
		}
	#endregion

	#region properties

		/// <summary>
		/// Return the <see cref="AxisType" /> for this <see cref="Scale" />, which is
		/// <see cref="AxisType.LinearAsOrdinal" />.
		/// </summary>
		public override AxisType Type
		{
			get { return AxisType.LinearAsOrdinal; }
		}

	#endregion

	#region methods

		/// <summary>
		/// Select a reasonable ordinal axis scale given a range of data values, with the expectation that
		/// linear values will be displayed.
		/// </summary>
		/// <remarks>
		/// This method only applies to <see cref="AxisType.DateAsOrdinal"/> type axes, and it
		/// is called by the general <see cref="Scale.PickScale"/> method.  For this type,
		/// the first curve is the "master", which contains the dates to be applied.
		/// <para>On Exit:</para>
		/// <para><see cref="Scale.Min"/> is set to scale minimum (if <see cref="Scale.MinAuto"/> = true)</para>
		/// <para><see cref="Scale.Max"/> is set to scale maximum (if <see cref="Scale.MaxAuto"/> = true)</para>
		/// <para><see cref="Scale.MajorStep"/> is set to scale step size (if <see cref="Scale.MajorStepAuto"/> = true)</para>
		/// <para><see cref="Scale.MinorStep"/> is set to scale minor step size (if <see cref="Scale.MinorStepAuto"/> = true)</para>
		/// <para><see cref="Scale.Mag"/> is set to a magnitude multiplier according to the data</para>
		/// <para><see cref="Scale.Format"/> is set to the display format for the values (this controls the
		/// number of decimal places, whether there are thousands separators, currency types, etc.)</para>
		/// </remarks>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object
		/// associated with this <see cref="Axis"/></param>
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
		/// <seealso cref="PickScale"/>
		/// <seealso cref="AxisType.Ordinal"/>
		override public void PickScale( GraphPane pane, Graphics g, float scaleFactor )
		{
			// call the base class first
			base.PickScale( pane, g, scaleFactor );

			// First, get the date ranges from the first curve in the list
			double xMin; // = Double.MaxValue;
			double xMax; // = Double.MinValue;
			double yMin; // = Double.MaxValue;
			double yMax; // = Double.MinValue;
			double tMin = 0;
			double tMax = 1;

			foreach ( CurveItem curve in pane.CurveList )
			{
				if ( ( _ownerAxis is Y2Axis && curve.IsY2Axis ) ||
						( _ownerAxis is YAxis && !curve.IsY2Axis ) ||
						( _ownerAxis is X2Axis && curve.IsX2Axis ) ||
						( _ownerAxis is XAxis && !curve.IsX2Axis ) )
				{
					curve.GetRange( out xMin, out xMax, out yMin, out yMax, false, false, pane );
					if ( _ownerAxis is XAxis || _ownerAxis is X2Axis )
					{
						tMin = xMin;
						tMax = xMax;
					}
					else
					{
						tMin = yMin;
						tMax = yMax;
					}
				}
			}

			double range = Math.Abs( tMax - tMin );

			// Now, set the axis range based on a ordinal scale
			base.PickScale( pane, g, scaleFactor );
			OrdinalScale.PickScale( pane, g, scaleFactor, this );

			SetScaleMag( tMin, tMax, range / Default.TargetXSteps );
		}

		/// <summary>
		/// Make a value label for an <see cref="AxisType.LinearAsOrdinal" /> <see cref="Axis" />.
		/// </summary>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="index">
		/// The zero-based, ordinal index of the label to be generated.  For example, a value of 2 would
		/// cause the third value label on the axis to be generated.
		/// </param>
		/// <param name="dVal">
		/// The numeric value associated with the label.  This value is ignored for log (<see cref="Scale.IsLog"/>)
		/// and text (<see cref="Scale.IsText"/>) type axes.
		/// </param>
		/// <returns>The resulting value label as a <see cref="string" /></returns>
		override internal string MakeLabel( GraphPane pane, int index, double dVal )
		{
			if ( _format == null )
				_format = Scale.Default.Format;

			double val;

			int tmpIndex = (int) dVal - 1;

			if ( pane.CurveList.Count > 0 && pane.CurveList[0].Points.Count > tmpIndex )
			{
				val = pane.CurveList[0].Points[tmpIndex].X;
				double scaleMult = Math.Pow( (double) 10.0, _mag );
				return ( val / scaleMult ).ToString( _format );
			}
			else
				return string.Empty;
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
		protected LinearAsOrdinalScale( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

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
		}
	#endregion

	}
}
