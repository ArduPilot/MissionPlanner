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
	/// The OrdinalScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.Ordinal" />.
	/// </summary>
	/// <remarks>
	/// OrdinalScale is an ordinal axis with tic labels generated at integral values.  An ordinal axis means that
	/// all data points are evenly spaced at integral values, and the actual coordinate values
	/// for points corresponding to that axis are ignored.  That is, if the X axis is an
	/// ordinal type, then all X values associated with the curves are ignored.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.8 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	class OrdinalScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		/// <summary>
		/// Default constructor that defines the owner <see cref="Axis" />
		/// (containing object) for this new object.
		/// </summary>
		/// <param name="owner">The owner, or containing object, of this instance</param>
		public OrdinalScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="OrdinalScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="OrdinalScale" /></param>
		public OrdinalScale( Scale rhs, Axis owner )
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
			return new OrdinalScale( this, owner );
		}

	#endregion

	#region properties

		/// <summary>
		/// Return the <see cref="AxisType" /> for this <see cref="Scale" />, which is
		/// <see cref="AxisType.Ordinal" />.
		/// </summary>
		public override AxisType Type
		{
			get { return AxisType.Ordinal; }
		}

	#endregion

	#region methods

		/// <summary>
		/// Select a reasonable ordinal axis scale given a range of data values.
		/// </summary>
		/// <remarks>
		/// This method only applies to <see cref="AxisType.Ordinal"/> type axes, and it
		/// is called by the general <see cref="Scale.PickScale"/> method.  The scale range is chosen
		/// based on increments of 1, 2, or 5 (because they are even divisors of 10).
		/// Being an ordinal axis type, the <see cref="Scale.MajorStep" /> value will always be integral.  This
		/// method honors the <see cref="Scale.MinAuto" />, <see cref="Scale.MaxAuto" />,
		/// and <see cref="Scale.MajorStepAuto" /> autorange settings.
		/// In the event that any of the autorange settings are false, the
		/// corresponding <see cref="Scale.Min" />, <see cref="Scale.Max" />, or <see cref="Scale.MajorStep" />
		/// setting is explicitly honored, and the remaining autorange settings (if any) will
		/// be calculated to accomodate the non-autoranged values.  The basic defaults for
		/// scale selection are defined using <see cref="Scale.Default.ZeroLever" />,
		/// <see cref="Scale.Default.TargetXSteps" />, and <see cref="Scale.Default.TargetYSteps" />
		/// from the <see cref="Scale.Default" /> default class.
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
		/// <seealso cref="AxisType.Ordinal"/>
		override public void PickScale( GraphPane pane, Graphics g, float scaleFactor )
		{
			// call the base class first
			base.PickScale( pane, g, scaleFactor );

			PickScale( pane, g, scaleFactor, this );
		}

		internal static void PickScale( GraphPane pane, Graphics g, float scaleFactor, Scale scale )
		{
			// Test for trivial condition of range = 0 and pick a suitable default
			if ( scale._max - scale._min < 1.0 )
			{
				if ( scale._maxAuto )
					scale._max = scale._min + 0.5;
				else
					scale._min = scale._max - 0.5;
			}
			else
			{
				// Calculate the new step size
				if ( scale._majorStepAuto )
				{
					// Calculate the step size based on targetSteps
					scale._majorStep = Scale.CalcStepSize( scale._max - scale._min,
						( scale._ownerAxis is XAxis || scale._ownerAxis is X2Axis ) ?
								Default.TargetXSteps : Default.TargetYSteps );

					if ( scale.IsPreventLabelOverlap )
					{
						// Calculate the maximum number of labels
						double maxLabels = (double) scale.CalcMaxLabels( g, pane, scaleFactor );

						// Calculate a step size based on the width of the labels
						double tmpStep = Math.Ceiling( ( scale._max - scale._min ) / maxLabels );

						// Use the greater of the two step sizes
						if ( tmpStep > scale._majorStep )
							scale._majorStep = tmpStep;
					}

				}

				scale._majorStep = (int)scale._majorStep;
				if ( scale._majorStep < 1.0 )
					scale._majorStep = 1.0;

				// Calculate the new minor step size
				if ( scale._minorStepAuto )
					scale._minorStep = Scale.CalcStepSize( scale._majorStep,
						( scale._ownerAxis is XAxis || scale._ownerAxis is X2Axis ) ?
								Default.TargetMinorXSteps : Default.TargetMinorYSteps );

				if ( scale._minAuto )
					scale._min -= 0.5;
				if ( scale._maxAuto )
					scale._max += 0.5;
			}
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
		protected OrdinalScale( SerializationInfo info, StreamingContext context ) : base( info, context )
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
