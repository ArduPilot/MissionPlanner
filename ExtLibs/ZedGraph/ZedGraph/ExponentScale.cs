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
	/// The ExponentScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.Exponent" />.
	/// </summary>
	/// <remarks>
	/// ExponentScale is a non-linear axis in which the values are scaled using an exponential function
	/// with the <see cref="Scale.Exponent" /> property.
	/// </remarks>
	/// 
	/// <author> John Champion with contributions by jackply </author>
	/// <version> $Revision: 1.8 $ $Date: 2007-04-16 00:03:01 $ </version>
	[Serializable]
	class ExponentScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		/// <summary>
		/// Default constructor that defines the owner <see cref="Axis" />
		/// (containing object) for this new object.
		/// </summary>
		/// <param name="owner">The owner, or containing object, of this instance</param>
		public ExponentScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ExponentScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="ExponentScale" /></param>
		public ExponentScale( Scale rhs, Axis owner )
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
			return new ExponentScale( this, owner );
		}

	#endregion

	#region properties

		public override AxisType Type
		{
			get { return AxisType.Exponent; }
		}

	#endregion

	#region methods

		/// <summary>
		/// Setup some temporary transform values in preparation for rendering the <see cref="Axis"/>.
		/// </summary>
		/// <remarks>
		/// This method is typically called by the parent <see cref="GraphPane"/>
		/// object as part of the <see cref="GraphPane.Draw"/> method.  It is also
		/// called by <see cref="GraphPane.GeneralTransform(double,double,CoordType)"/> and
		/// <see cref="GraphPane.ReverseTransform( PointF, out double, out double )"/>
		/// methods to setup for coordinate transformations.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="axis">
		/// The parent <see cref="Axis" /> for this <see cref="Scale" />
		/// </param>
		override public void SetupScaleData( GraphPane pane, Axis axis )
		{
			base.SetupScaleData( pane, axis );

			if (  _exponent > 0 )
			{
				_minLinTemp = Linearize( _min );
				_maxLinTemp = Linearize( _max );
			}
			else if ( _exponent < 0 )
			{
				_minLinTemp = Linearize( _max );
				_maxLinTemp = Linearize( _min );
			}
		}

		/// <summary>
		/// Convert a value to its linear equivalent for this type of scale.
		/// </summary>
		/// <remarks>
		/// The default behavior is to just return the value unchanged.  However,
		/// for <see cref="AxisType.Log" /> and <see cref="AxisType.Exponent" />,
		/// it returns the log or power equivalent.
		/// </remarks>
		/// <param name="val">The value to be converted</param>
		override public double Linearize( double val )
		{
			return SafeExp( val, _exponent );
		}

		/// <summary>
		/// Convert a value from its linear equivalent to its actual scale value
		/// for this type of scale.
		/// </summary>
		/// <remarks>
		/// The default behavior is to just return the value unchanged.  However,
		/// for <see cref="AxisType.Log" /> and <see cref="AxisType.Exponent" />,
		/// it returns the anti-log or inverse-power equivalent.
		/// </remarks>
		/// <param name="val">The value to be converted</param>
		override public double DeLinearize( double val )
		{
			return Math.Pow( val, 1 / _exponent );
		}

		/// <summary>
		/// Determine the value for any major tic.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="Scale.IsLog"/>, <see cref="Scale.IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <param name="baseVal">
		/// The value of the first major tic (floating point double)
		/// </param>
		/// <param name="tic">
		/// The major tic number (0 = first major tic).  For log scales, this is the actual power of 10.
		/// </param>
		/// <returns>
		/// The specified major tic value (floating point double).
		/// </returns>
		override internal double CalcMajorTicValue( double baseVal, double tic )
		{
			if ( _exponent > 0.0 )
			{
				//return baseVal + Math.Pow ( (double) this.majorStep * tic, exp );
				//baseVal is got from CalBase..., and it is exp..
				return Math.Pow( Math.Pow( baseVal, 1 / _exponent ) + _majorStep * tic, _exponent );
			}
			else if ( _exponent < 0.0 )
			{
				//baseVal is got from CalBase..., and it is exp..
				return Math.Pow( Math.Pow( baseVal, 1 / _exponent ) + _majorStep * tic, _exponent );
			}

			return 1.0;
		}

		/// <summary>
		/// Determine the value for any minor tic.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="Scale.IsLog"/>, <see cref="Scale.IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <param name="baseVal">
		/// The value of the first major tic (floating point double).  This tic value is the base
		/// reference for all tics (including minor ones).
		/// </param>
		/// <param name="iTic">
		/// The major tic number (0 = first major tic).  For log scales, this is the actual power of 10.
		/// </param>
		/// <returns>
		/// The specified minor tic value (floating point double).
		/// </returns>
		override internal double CalcMinorTicValue( double baseVal, int iTic )
		{
			return baseVal + Math.Pow( (double) _majorStep * (double) iTic, _exponent );
		}

		/// <summary>
		/// Internal routine to determine the ordinals of the first minor tic mark
		/// </summary>
		/// <param name="baseVal">
		/// The value of the first major tic for the axis.
		/// </param>
		/// <returns>
		/// The ordinal position of the first minor tic, relative to the first major tic.
		/// This value can be negative (e.g., -3 means the first minor tic is 3 minor step
		/// increments before the first major tic.
		/// </returns>
		override internal int CalcMinorStart( double baseVal )
		{
			return (int) ( ( Math.Pow( _min, _exponent ) - baseVal ) / Math.Pow( _minorStep, _exponent ) );
		}

		/// <summary>
		/// Select a reasonable exponential axis scale given a range of data values.
		/// </summary>
		/// <remarks>
		/// This method only applies to <see cref="AxisType.Exponent"/> type axes, and it
		/// is called by the general <see cref="Scale.PickScale"/> method.  The exponential scale
		/// relies on the <see cref="Scale.Exponent" /> property to set the scaling exponent.  This
		/// method honors the <see cref="Scale.MinAuto"/>, <see cref="Scale.MaxAuto"/>,
		/// and <see cref="Scale.MajorStepAuto"/> autorange settings.
		/// In the event that any of the autorange settings are false, the
		/// corresponding <see cref="Scale.Min"/>, <see cref="Scale.Max"/>, or <see cref="Scale.MajorStep"/>
		/// setting is explicitly honored, and the remaining autorange settings (if any) will
		/// be calculated to accomodate the non-autoranged values.  For log axes, the MinorStep
		/// value is not used.
		/// <para>On Exit:</para>
		/// <para><see cref="Scale.Min"/> is set to scale minimum (if <see cref="Scale.MinAuto"/> = true)</para>
		/// <para><see cref="Scale.Max"/> is set to scale maximum (if <see cref="Scale.MaxAuto"/> = true)</para>
		/// <para><see cref="Scale.MajorStep"/> is set to scale step size (if <see cref="Scale.MajorStepAuto"/> = true)</para>
		/// <para><see cref="Scale.Mag"/> is set to a magnitude multiplier according to the data</para>
		/// <para><see cref="Scale.Format"/> is set to the display format for the values (this controls the
		/// number of decimal places, whether there are thousands separators, currency types, etc.)</para>
		/// </remarks>
		/// <seealso cref="Scale.PickScale"/>
		/// <seealso cref="AxisType.Exponent"/>
		override public void PickScale( GraphPane pane, Graphics g, float scaleFactor )
		{
			// call the base class first
			base.PickScale( pane, g, scaleFactor );

			// Test for trivial condition of range = 0 and pick a suitable default
			if ( _max - _min < 1.0e-20 )
			{
				if ( _maxAuto )
					_max = _max + 0.2 * ( _max == 0 ? 1.0 : Math.Abs( _max ) );
				if ( _minAuto )
					_min = _min - 0.2 * ( _min == 0 ? 1.0 : Math.Abs( _min ) );
			}

			// This is the zero-lever test.  If minVal is within the zero lever fraction
			// of the data range, then use zero.

			if ( _minAuto && _min > 0 &&
				_min / ( _max - _min ) < Default.ZeroLever )
				_min = 0;

			// Repeat the zero-lever test for cases where the maxVal is less than zero
			if ( _maxAuto && _max < 0 &&
				Math.Abs( _max / ( _max - _min ) ) <
				Default.ZeroLever )
				_max = 0;

			// Calculate the new step size
			if ( _majorStepAuto )
			{
				double targetSteps = ( _ownerAxis is XAxis || _ownerAxis is X2Axis ) ?
						Default.TargetXSteps : Default.TargetYSteps;

				// Calculate the step size based on target steps
				_majorStep = CalcStepSize( _max - _min, targetSteps );

				if ( _isPreventLabelOverlap )
				{
					// Calculate the maximum number of labels
					double maxLabels = (double) this.CalcMaxLabels( g, pane, scaleFactor );

					if ( maxLabels < ( _max - _min ) / _majorStep )
						_majorStep = CalcBoundedStepSize( _max - _min, maxLabels );
				}
			}

			// Calculate the new step size
			if ( _minorStepAuto )
				_minorStep = CalcStepSize( _majorStep,
					( _ownerAxis is XAxis || _ownerAxis is X2Axis ) ?
							Default.TargetMinorXSteps : Default.TargetMinorYSteps );

			// Calculate the scale minimum
			if ( _minAuto )
				_min = _min - MyMod( _min, _majorStep );

			// Calculate the scale maximum
			if ( _maxAuto )
				_max = MyMod( _max, _majorStep ) == 0.0 ? _max :
					_max + _majorStep - MyMod( _max, _majorStep );

			// set the scale magnitude if required
			if ( _magAuto )
			{
				// Find the optimal scale display multiple
				double mag = 0;
				double mag2 = 0;

				if ( Math.Abs( _min ) > 1.0e-10 )
					mag = Math.Floor( Math.Log10( Math.Abs( _min ) ) );
				if ( Math.Abs( _max ) > 1.0e-10 )
					mag2 = Math.Floor( Math.Log10( Math.Abs( _max ) ) );
				if ( Math.Abs( mag2 ) > Math.Abs( mag ) )
					mag = mag2;

				// Do not use scale multiples for magnitudes below 4
				if ( Math.Abs( mag ) <= 3 )
					mag = 0;

				// Use a power of 10 that is a multiple of 3 (engineering scale)
				_mag = (int) ( Math.Floor( mag / 3.0 ) * 3.0 );
			}

			// Calculate the appropriate number of dec places to display if required
			if ( _formatAuto )
			{
				int numDec = 0 - (int) ( Math.Floor( Math.Log10( _majorStep ) ) - _mag );
				if ( numDec < 0 )
					numDec = 0;
				_format = "f" + numDec.ToString();
			}
		}

		/// <summary>
		/// Make a value label for an <see cref="AxisType.Exponent" /> <see cref="Axis" />.
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

			double scaleMult = Math.Pow( (double) 10.0, _mag );
			double val = Math.Pow( dVal, 1 / _exponent ) / scaleMult;
			return val.ToString( _format );
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
		protected ExponentScale( SerializationInfo info, StreamingContext context ) : base( info, context )
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
