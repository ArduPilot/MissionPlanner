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
	/// The DateScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.Date" />.
	/// </summary>
	/// <remarks>
	/// DateScale is a cartesian axis with calendar dates or times.  The actual data values should
	/// be created with the <see cref="XDate" /> type, which is directly translatable to a
	/// <see cref="System.Double" /> type for storage in the point value arrays.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.15 $ $Date: 2007-09-19 06:41:56 $ </version>
	[Serializable]
	class DateScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		/// <summary>
		/// Default constructor that defines the owner <see cref="Axis" />
		/// (containing object) for this new object.
		/// </summary>
		/// <param name="owner">The owner, or containing object, of this instance</param>
		public DateScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="DateScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="DateScale" /></param>
		public DateScale( Scale rhs, Axis owner )
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
			return new DateScale( this, owner );
		}

	#endregion

	#region properties

		/// <summary>
		/// Return the <see cref="AxisType" /> for this <see cref="Scale" />, which is
		/// <see cref="AxisType.Date" />.
		/// </summary>
		public override AxisType Type
		{
			get { return AxisType.Date; }
		}

		/// <summary>
		/// Gets or sets the minimum value for this scale.
		/// </summary>
		/// <remarks>
		/// The set property is specifically adapted for <see cref="AxisType.Date" /> scales,
		/// in that it automatically limits the value to the range of valid dates for the
		/// <see cref="XDate" /> struct.
		/// </remarks>
		public override double Min
		{
			get { return _min; }
			set { _min = XDate.MakeValidDate( value ); _minAuto = false; }
		}

		/// <summary>
		/// Gets or sets the maximum value for this scale.
		/// </summary>
		/// <remarks>
		/// The set property is specifically adapted for <see cref="AxisType.Date" /> scales,
		/// in that it automatically limits the value to the range of valid dates for the
		/// <see cref="XDate" /> struct.
		/// </remarks>
		public override double Max
		{
			get { return _max; }
			set { _max = XDate.MakeValidDate( value ); _maxAuto = false; }
		}
	#endregion

	#region methods

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
			XDate xDate = new XDate( baseVal );

			switch ( _majorUnit )
			{
				case DateUnit.Year:
				default:
					xDate.AddYears( tic * _majorStep );
					break;
				case DateUnit.Month:
					xDate.AddMonths( tic * _majorStep );
					break;
				case DateUnit.Day:
					xDate.AddDays( tic * _majorStep );
					break;
				case DateUnit.Hour:
					xDate.AddHours( tic * _majorStep );
					break;
				case DateUnit.Minute:
					xDate.AddMinutes( tic * _majorStep );
					break;
				case DateUnit.Second:
					xDate.AddSeconds( tic * _majorStep );
					break;
				case DateUnit.Millisecond:
					xDate.AddMilliseconds( tic * _majorStep );
					break;
			}

			return xDate.XLDate;
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
			XDate xDate = new XDate( baseVal );

			switch ( _minorUnit )
			{
				case DateUnit.Year:
				default:
					xDate.AddYears( (double) iTic * _minorStep );
					break;
				case DateUnit.Month:
					xDate.AddMonths( (double) iTic * _minorStep );
					break;
				case DateUnit.Day:
					xDate.AddDays( (double) iTic * _minorStep );
					break;
				case DateUnit.Hour:
					xDate.AddHours( (double) iTic * _minorStep );
					break;
				case DateUnit.Minute:
					xDate.AddMinutes( (double) iTic * _minorStep );
					break;
				case DateUnit.Second:
					xDate.AddSeconds( (double) iTic * _minorStep );
					break;
			}

			return xDate.XLDate;
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
			switch ( _minorUnit )
			{
				case DateUnit.Year:
				default:
					return (int) ( ( _min - baseVal ) / ( 365.0 * _minorStep ) );
				case DateUnit.Month:
					return (int) ( ( _min - baseVal ) / ( 28.0 * _minorStep ) );
				case DateUnit.Day:
					return (int) ( ( _min - baseVal ) / _minorStep );
				case DateUnit.Hour:
					return (int) ( ( _min - baseVal ) * XDate.HoursPerDay / _minorStep );
				case DateUnit.Minute:
					return (int) ( ( _min - baseVal ) * XDate.MinutesPerDay / _minorStep );
				case DateUnit.Second:
					return (int) ( ( _min - baseVal ) * XDate.SecondsPerDay / _minorStep );
			}
		}

		/// <summary>
		/// Determine the value for the first major tic.
		/// </summary>
		/// <remarks>
		/// This is done by finding the first possible value that is an integral multiple of
		/// the step size, taking into account the date/time units if appropriate.
		/// This method properly accounts for <see cref="Scale.IsLog"/>, <see cref="Scale.IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <returns>
		/// First major tic value (floating point double).
		/// </returns>
		override internal double CalcBaseTic()
		{
			if ( _baseTic != PointPair.Missing )
				return _baseTic;
			else
			{
				int year, month, day, hour, minute, second, millisecond;
				XDate.XLDateToCalendarDate( _min, out year, out month, out day, out hour, out minute,
											out second, out millisecond );
				switch ( _majorUnit )
				{
					case DateUnit.Year:
					default:
						month = 1; day = 1; hour = 0; minute = 0; second = 0; millisecond = 0;
						break;
					case DateUnit.Month:
						day = 1; hour = 0; minute = 0; second = 0; millisecond = 0;
						break;
					case DateUnit.Day:
						hour = 0; minute = 0; second = 0; millisecond = 0;
						break;
					case DateUnit.Hour:
						minute = 0; second = 0; millisecond = 0;
						break;
					case DateUnit.Minute:
						second = 0; millisecond = 0;
						break;
					case DateUnit.Second:
						millisecond = 0;
						break;
					case DateUnit.Millisecond:
						break;

				}

				double xlDate = XDate.CalendarDateToXLDate( year, month, day, hour, minute, second, millisecond );
				if ( xlDate < _min )
				{
					switch ( _majorUnit )
					{
						case DateUnit.Year:
						default:
							year++;
							break;
						case DateUnit.Month:
							month++;
							break;
						case DateUnit.Day:
							day++;
							break;
						case DateUnit.Hour:
							hour++;
							break;
						case DateUnit.Minute:
							minute++;
							break;
						case DateUnit.Second:
							second++;
							break;
						case DateUnit.Millisecond:
							millisecond++;
							break;

					}

					xlDate = XDate.CalendarDateToXLDate( year, month, day, hour, minute, second, millisecond );
				}

				return xlDate;
			}
		}
		
		/// <summary>
		/// Internal routine to determine the ordinals of the first and last major axis label.
		/// </summary>
		/// <returns>
		/// This is the total number of major tics for this axis.
		/// </returns>
		override internal int CalcNumTics()
		{
			int nTics = 1;

			int year1, year2, month1, month2, day1, day2, hour1, hour2, minute1, minute2;
			int second1, second2, millisecond1, millisecond2;

			XDate.XLDateToCalendarDate( _min, out year1, out month1, out day1,
										out hour1, out minute1, out second1, out millisecond1 );
			XDate.XLDateToCalendarDate( _max, out year2, out month2, out day2,
										out hour2, out minute2, out second2, out millisecond2 );

			switch ( _majorUnit )
			{
				case DateUnit.Year:
				default:
					nTics = (int) ( ( year2 - year1 ) / _majorStep + 1.001 );
					break;
				case DateUnit.Month:
					nTics = (int) ( ( month2 - month1 + 12.0 * ( year2 - year1 ) ) / _majorStep + 1.001 );
					break;
				case DateUnit.Day:
					nTics = (int) ( ( _max - _min ) / _majorStep + 1.001 );
					break;
				case DateUnit.Hour:
					nTics = (int) ( ( _max - _min ) / ( _majorStep / XDate.HoursPerDay ) + 1.001 );
					break;
				case DateUnit.Minute:
					nTics = (int) ( ( _max - _min ) / ( _majorStep / XDate.MinutesPerDay ) + 1.001 );
					break;
				case DateUnit.Second:
					nTics = (int)( ( _max - _min ) / ( _majorStep / XDate.SecondsPerDay ) + 1.001 );
					break;
				case DateUnit.Millisecond:
					nTics = (int)( ( _max - _min ) / ( _majorStep / XDate.MillisecondsPerDay ) + 1.001 );
					break;
			}

			if ( nTics < 1 )
				nTics = 1;
			else if ( nTics > 1000 )
				nTics = 1000;

			return nTics;
		}

		/// <summary>
		/// Select a reasonable date-time axis scale given a range of data values.
		/// </summary>
		/// <remarks>
		/// This method only applies to <see cref="AxisType.Date"/> type axes, and it
		/// is called by the general <see cref="PickScale"/> method.  The scale range is chosen
		/// based on increments of 1, 2, or 5 (because they are even divisors of 10).
		/// Note that the <see cref="Scale.MajorStep"/> property setting can have multiple unit
		/// types (<see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit" />),
		/// but the <see cref="Scale.Min"/> and
		/// <see cref="Scale.Max"/> units are always days (<see cref="XDate"/>).  This
		/// method honors the <see cref="Scale.MinAuto"/>, <see cref="Scale.MaxAuto"/>,
		/// and <see cref="Scale.MajorStepAuto"/> autorange settings.
		/// In the event that any of the autorange settings are false, the
		/// corresponding <see cref="Scale.Min"/>, <see cref="Scale.Max"/>, or <see cref="Scale.MajorStep"/>
		/// setting is explicitly honored, and the remaining autorange settings (if any) will
		/// be calculated to accomodate the non-autoranged values.  The basic default for
		/// scale selection is defined with
		/// <see cref="Scale.Default.TargetXSteps"/> and <see cref="Scale.Default.TargetYSteps"/>
		/// from the <see cref="Scale.Default"/> default class.
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
		/// <seealso cref="Scale.PickScale"/>
		/// <seealso cref="AxisType.Date"/>
		/// <seealso cref="Scale.MajorUnit"/>
		/// <seealso cref="Scale.MinorUnit"/>
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

			double targetSteps = ( _ownerAxis is XAxis || _ownerAxis is X2Axis ) ?
						Default.TargetXSteps : Default.TargetYSteps;

			// Calculate the step size based on target steps
			double tempStep = CalcDateStepSize( _max - _min, targetSteps );

			// Calculate the new step size
			if ( _majorStepAuto )
			{
				_majorStep = tempStep;

				if ( _isPreventLabelOverlap )
				{
					// Calculate the maximum number of labels
					double maxLabels = (double) this.CalcMaxLabels( g, pane, scaleFactor );

					if ( maxLabels < this.CalcNumTics() )
						_majorStep = CalcDateStepSize( _max - _min, maxLabels );
				}
			}

			// Calculate the scale minimum
			if ( _minAuto )
				_min = CalcEvenStepDate( _min, -1 );

			// Calculate the scale maximum
			if ( _maxAuto )
				_max = CalcEvenStepDate( _max, 1 );

			_mag = 0;		// Never use a magnitude shift for date scales
			//this.numDec = 0;		// The number of decimal places to display is not used

		}

		/// <summary>
		/// Calculate a step size for a <see cref="AxisType.Date"/> scale.
		/// This method is used by <see cref="PickScale"/>.
		/// </summary>
		/// <param name="range">The range of data in units of days</param>
		/// <param name="targetSteps">The desired "typical" number of steps
		/// to divide the range into</param>
		/// <returns>The calculated step size for the specified data range.  Also
		/// calculates and sets the values for <see cref="Scale.MajorUnit"/>,
		/// <see cref="Scale.MinorUnit"/>, <see cref="Scale.MinorStep"/>, and
		/// <see cref="Scale.Format"/></returns>
		protected double CalcDateStepSize( double range, double targetSteps )
		{
			return CalcDateStepSize( range, targetSteps, this );
		}

		/// <summary>
		/// Calculate a step size for a <see cref="AxisType.Date"/> scale.
		/// This method is used by <see cref="PickScale"/>.
		/// </summary>
		/// <param name="range">The range of data in units of days</param>
		/// <param name="targetSteps">The desired "typical" number of steps
		/// to divide the range into</param>
		/// <param name="scale">
		/// The <see cref="Scale" /> object on which to calculate the Date step size.</param>
		/// <returns>The calculated step size for the specified data range.  Also
		/// calculates and sets the values for <see cref="Scale.MajorUnit"/>,
		/// <see cref="Scale.MinorUnit"/>, <see cref="Scale.MinorStep"/>, and
		/// <see cref="Scale.Format"/></returns>
		internal static double CalcDateStepSize( double range, double targetSteps, Scale scale )
		{
			// Calculate an initial guess at step size
			double tempStep = range / targetSteps;

			if ( range > Default.RangeYearYear )
			{
				scale._majorUnit = DateUnit.Year;
				if ( scale._formatAuto )
					scale._format = Default.FormatYearYear;

				tempStep = Math.Ceiling( tempStep / 365.0 );

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Year;
					if ( tempStep == 1.0 )
						scale._minorStep = 0.25;
					else
						scale._minorStep = Scale.CalcStepSize( tempStep, targetSteps );
				}
			}
			else if ( range > Default.RangeYearMonth )
			{
				scale._majorUnit = DateUnit.Year;
				if ( scale._formatAuto )
					scale._format = Default.FormatYearMonth;
				tempStep = Math.Ceiling( tempStep / 365.0 );

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Month;
					// Calculate the minor steps to give an estimated 4 steps
					// per major step.
					scale._minorStep = Math.Ceiling( range / ( targetSteps * 3 ) / 30.0 );
					// make sure the minorStep is 1, 2, 3, 6, or 12 months
					if ( scale._minorStep > 6 )
						scale._minorStep = 12;
					else if ( scale._minorStep > 3 )
						scale._minorStep = 6;
				}
			}
			else if ( range > Default.RangeMonthMonth )
			{
				scale._majorUnit = DateUnit.Month;
				if ( scale._formatAuto )
					scale._format = Default.FormatMonthMonth;
				tempStep = Math.Ceiling( tempStep / 30.0 );

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Month;
					scale._minorStep = tempStep * 0.25;
				}
			}
			else if ( range > Default.RangeDayDay )
			{
				scale._majorUnit = DateUnit.Day;
				if ( scale._formatAuto )
					scale._format = Default.FormatDayDay;
				tempStep = Math.Ceiling( tempStep );

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Day;
					scale._minorStep = tempStep * 0.25;
					// make sure the minorStep is 1, 2, 3, 6, or 12 hours
				}
			}
			else if ( range > Default.RangeDayHour )
			{
				scale._majorUnit = DateUnit.Day;
				if ( scale._formatAuto )
					scale._format = Default.FormatDayHour;
				tempStep = Math.Ceiling( tempStep );

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Hour;
					// Calculate the minor steps to give an estimated 4 steps
					// per major step.
					scale._minorStep = Math.Ceiling( range / ( targetSteps * 3 ) * XDate.HoursPerDay );
					// make sure the minorStep is 1, 2, 3, 6, or 12 hours
					if ( scale._minorStep > 6 )
						scale._minorStep = 12;
					else if ( scale._minorStep > 3 )
						scale._minorStep = 6;
					else
						scale._minorStep = 1;
				}
			}
			else if ( range > Default.RangeHourHour )
			{
				scale._majorUnit = DateUnit.Hour;
				tempStep = Math.Ceiling( tempStep * XDate.HoursPerDay );
				if ( scale._formatAuto )
					scale._format = Default.FormatHourHour;

				if ( tempStep > 12.0 )
					tempStep = 24.0;
				else if ( tempStep > 6.0 )
					tempStep = 12.0;
				else if ( tempStep > 2.0 )
					tempStep = 6.0;
				else if ( tempStep > 1.0 )
					tempStep = 2.0;
				else
					tempStep = 1.0;

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Hour;
					if ( tempStep <= 1.0 )
						scale._minorStep = 0.25;
					else if ( tempStep <= 6.0 )
						scale._minorStep = 1.0;
					else if ( tempStep <= 12.0 )
						scale._minorStep = 2.0;
					else
						scale._minorStep = 4.0;
				}
			}
			else if ( range > Default.RangeHourMinute )
			{
				scale._majorUnit = DateUnit.Hour;
				tempStep = Math.Ceiling( tempStep * XDate.HoursPerDay );

				if ( scale._formatAuto )
					scale._format = Default.FormatHourMinute;

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Minute;
					// Calculate the minor steps to give an estimated 4 steps
					// per major step.
					scale._minorStep = Math.Ceiling( range / ( targetSteps * 3 ) * XDate.MinutesPerDay );
					// make sure the minorStep is 1, 5, 15, or 30 minutes
					if ( scale._minorStep > 15.0 )
						scale._minorStep = 30.0;
					else if ( scale._minorStep > 5.0 )
						scale._minorStep = 15.0;
					else if ( scale._minorStep > 1.0 )
						scale._minorStep = 5.0;
					else
						scale._minorStep = 1.0;
				}
			}
			else if ( range > Default.RangeMinuteMinute )
			{
				scale._majorUnit = DateUnit.Minute;
				if ( scale._formatAuto )
					scale._format = Default.FormatMinuteMinute;

				tempStep = Math.Ceiling( tempStep * XDate.MinutesPerDay );
				// make sure the minute step size is 1, 5, 15, or 30 minutes
				if ( tempStep > 15.0 )
					tempStep = 30.0;
				else if ( tempStep > 5.0 )
					tempStep = 15.0;
				else if ( tempStep > 1.0 )
					tempStep = 5.0;
				else
					tempStep = 1.0;

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Minute;
					if ( tempStep <= 1.0 )
						scale._minorStep = 0.25;
					else if ( tempStep <= 5.0 )
						scale._minorStep = 1.0;
					else
						scale._minorStep = 5.0;
				}
			}
			else if ( range > Default.RangeMinuteSecond )
			{
				scale._majorUnit = DateUnit.Minute;
				tempStep = Math.Ceiling( tempStep * XDate.MinutesPerDay );

				if ( scale._formatAuto )
					scale._format = Default.FormatMinuteSecond;

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Second;
					// Calculate the minor steps to give an estimated 4 steps
					// per major step.
					scale._minorStep = Math.Ceiling( range / ( targetSteps * 3 ) * XDate.SecondsPerDay );
					// make sure the minorStep is 1, 5, 15, or 30 seconds
					if ( scale._minorStep > 15.0 )
						scale._minorStep = 30.0;
					else if ( scale._minorStep > 5.0 )
						scale._minorStep = 15.0;
					else if ( scale._minorStep > 1.0 )
						scale._minorStep = 5.0;
					else
						scale._minorStep = 1.0;
				}
			}
			else  if ( range > Default.RangeSecondSecond ) // SecondSecond
			{
				scale._majorUnit = DateUnit.Second;
				if ( scale._formatAuto )
					scale._format = Default.FormatSecondSecond;

				tempStep = Math.Ceiling( tempStep * XDate.SecondsPerDay );
				// make sure the second step size is 1, 5, 15, or 30 seconds
				if ( tempStep > 15.0 )
					tempStep = 30.0;
				else if ( tempStep > 5.0 )
					tempStep = 15.0;
				else if ( tempStep > 1.0 )
					tempStep = 5.0;
				else
					tempStep = 1.0;

				if ( scale._minorStepAuto )
				{
					scale._minorUnit = DateUnit.Second;
					if ( tempStep <= 1.0 )
						scale._minorStep = 0.25;
					else if ( tempStep <= 5.0 )
						scale._minorStep = 1.0;
					else
						scale._minorStep = 5.0;
				}
			}
			else // MilliSecond
			{
				scale._majorUnit = DateUnit.Millisecond;
				if ( scale._formatAuto )
					scale._format = Default.FormatMillisecond;

				tempStep = CalcStepSize( range * XDate.MillisecondsPerDay, Default.TargetXSteps );

				if ( scale._minorStepAuto )
				{
					scale._minorStep = CalcStepSize( tempStep,
							( scale._ownerAxis is XAxis || scale._ownerAxis is X2Axis ) ?
							Default.TargetMinorXSteps : Default.TargetMinorYSteps );
					scale._minorUnit = DateUnit.Millisecond;
				}
			}

			return tempStep;
		}

		/// <summary>
		/// Calculate a date that is close to the specified date and an
		/// even multiple of the selected
		/// <see cref="Scale.MajorUnit"/> for a <see cref="AxisType.Date"/> scale.
		/// This method is used by <see cref="PickScale"/>.
		/// </summary>
		/// <param name="date">The date which the calculation should be close to</param>
		/// <param name="direction">The desired direction for the date to take.
		/// 1 indicates the result date should be greater than the specified
		/// date parameter.  -1 indicates the other direction.</param>
		/// <returns>The calculated date</returns>
		protected double CalcEvenStepDate( double date, int direction )
		{
			int year, month, day, hour, minute, second, millisecond;

			XDate.XLDateToCalendarDate( date, out year, out month, out day,
										out hour, out minute, out second, out millisecond );

			// If the direction is -1, then it is sufficient to go to the beginning of
			// the current time period, .e.g., for 15-May-95, and monthly steps, we
			// can just back up to 1-May-95
			if ( direction < 0 )
				direction = 0;

			switch ( _majorUnit )
			{
				case DateUnit.Year:
				default:
					// If the date is already an exact year, then don't step to the next year
					if ( direction == 1 && month == 1 && day == 1 && hour == 0
						&& minute == 0 && second == 0 )
						return date;
					else
						return XDate.CalendarDateToXLDate( year + direction, 1, 1,
														0, 0, 0 );
				case DateUnit.Month:
					// If the date is already an exact month, then don't step to the next month
					if ( direction == 1 && day == 1 && hour == 0
						&& minute == 0 && second == 0 )
						return date;
					else
						return XDate.CalendarDateToXLDate( year, month + direction, 1,
												0, 0, 0 );
				case DateUnit.Day:
					// If the date is already an exact Day, then don't step to the next day
					if ( direction == 1 && hour == 0 && minute == 0 && second == 0 )
						return date;
					else
						return XDate.CalendarDateToXLDate( year, month,
											day + direction, 0, 0, 0 );
				case DateUnit.Hour:
					// If the date is already an exact hour, then don't step to the next hour
					if ( direction == 1 && minute == 0 && second == 0 )
						return date;
					else
						return XDate.CalendarDateToXLDate( year, month, day,
													hour + direction, 0, 0 );
				case DateUnit.Minute:
					// If the date is already an exact minute, then don't step to the next minute
					if ( direction == 1 && second == 0 )
						return date;
					else
						return XDate.CalendarDateToXLDate( year, month, day, hour,
													minute + direction, 0 );
				case DateUnit.Second:
					return XDate.CalendarDateToXLDate( year, month, day, hour,
													minute, second + direction );

				case DateUnit.Millisecond:
					return XDate.CalendarDateToXLDate( year, month, day, hour,
													minute, second, millisecond + direction );

			}
		}

		/// <summary>
		/// Make a value label for an <see cref="AxisType.Date" /> <see cref="Axis" />.
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

			return XDate.ToString( dVal, _format );
		}

		/// <summary>
		/// Gets the major unit multiplier for this scale type, if any.
		/// </summary>
		/// <remarks>The major unit multiplier will correct the units of
		/// <see cref="Scale.MajorStep" /> to match the units of <see cref="Scale.Min" />
		/// and <see cref="Scale.Max" />.  This reflects the setting of
		/// <see cref="Scale.MajorUnit" />.
		/// </remarks>
		override internal double MajorUnitMultiplier
		{
			get { return GetUnitMultiple( _majorUnit ); }
		}

		/// <summary>
		/// Gets the minor unit multiplier for this scale type, if any.
		/// </summary>
		/// <remarks>The minor unit multiplier will correct the units of
		/// <see cref="Scale.MinorStep" /> to match the units of <see cref="Scale.Min" />
		/// and <see cref="Scale.Max" />.  This reflects the setting of
		/// <see cref="Scale.MinorUnit" />.
		/// </remarks>
		override internal double MinorUnitMultiplier
		{
			get { return GetUnitMultiple( _minorUnit ); }
		}

		/// <summary>
		/// Internal routine to calculate a multiplier to the selected unit back to days.
		/// </summary>
		/// <param name="unit">The unit type for which the multiplier is to be
		/// calculated</param>
		/// <returns>
		/// This is ratio of days/selected unit
		/// </returns>
		private double GetUnitMultiple( DateUnit unit )
		{
			switch ( unit )
			{
				case DateUnit.Year:
				default:
					return 365.0;
				case DateUnit.Month:
					return 30.0;
				case DateUnit.Day:
					return 1.0;
				case DateUnit.Hour:
					return 1.0 / XDate.HoursPerDay;
				case DateUnit.Minute:
					return 1.0 / XDate.MinutesPerDay;
				case DateUnit.Second:
					return 1.0 / XDate.SecondsPerDay;
				case DateUnit.Millisecond:
					return 1.0 / XDate.MillisecondsPerDay;
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
		protected DateScale( SerializationInfo info, StreamingContext context ) : base( info, context )
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
