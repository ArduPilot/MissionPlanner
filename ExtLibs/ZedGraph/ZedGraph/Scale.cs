//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ZedGraph
{
	/// <summary>
	/// The Scale class is an abstract base class that encompasses the properties
	/// and methods associated with a scale of data.
	/// </summary>
	/// <remarks>This class is inherited by the
	/// <see cref="LinearScale"/>, <see cref="LogScale"/>, <see cref="OrdinalScale"/>,
	/// <see cref="TextScale"/>, <see cref="DateScale"/>, <see cref="ExponentScale"/>,
	/// <see cref="DateAsOrdinalScale"/>, and <see cref="LinearAsOrdinalScale"/>
	/// classes to define specific characteristics for those types.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.33 $ $Date: 2007-09-19 06:41:56 $ </version>
	[Serializable]
	abstract public class Scale : ISerializable
	{
	#region Fields

		/// <summary> Private fields for the <see cref="Axis"/> scale definitions.
		/// Use the public properties <see cref="Min"/>, <see cref="Max"/>,
		/// <see cref="MajorStep"/>, <see cref="MinorStep"/>, and <see cref="Exponent" />
		/// for access to these values.
		/// </summary>
		internal double	_min,
								_max,
								_majorStep,
								_minorStep,
								_exponent,
								_baseTic;

		/// <summary> Private fields for the <see cref="Axis"/> automatic scaling modes.
		/// Use the public properties <see cref="MinAuto"/>, <see cref="MaxAuto"/>,
		/// <see cref="MajorStepAuto"/>, <see cref="MinorStepAuto"/>, 
		/// <see cref="MagAuto"/> and <see cref="FormatAuto"/>
		/// for access to these values.
		/// </summary>
		internal bool		_minAuto,
								_maxAuto,
								_majorStepAuto,
								_minorStepAuto,
								_magAuto,
								_formatAuto;

		/// <summary> Private fields for the <see cref="Axis"/> "grace" settings.
		/// These values determine how much extra space is left before the first data value
		/// and after the last data value.
		/// Use the public properties <see cref="MinGrace"/> and <see cref="MaxGrace"/>
		/// for access to these values.
		/// </summary>
		internal double	_minGrace,
								_maxGrace;


		/// <summary> Private field for the <see cref="Axis"/> scale value display.
		/// Use the public property <see cref="Mag"/> for access to this value.
		/// </summary>
		internal int		_mag;

		/// <summary> Private fields for the <see cref="Scale"/> attributes.
		/// Use the public properties <see cref="Scale.IsReverse"/> and <see cref="Scale.IsUseTenPower"/>
		/// for access to these values.
		/// </summary>
		internal bool		_isReverse,
								_isPreventLabelOverlap,
								_isUseTenPower,
								_isLabelsInside,
								_isSkipFirstLabel,
								_isSkipLastLabel,
								_isSkipCrossLabel,
								_isVisible;

		/// <summary> Private <see cref="System.Collections.ArrayList"/> field for the <see cref="Axis"/> array of text labels.
		/// This property is only used if <see cref="Type"/> is set to
		/// <see cref="AxisType.Text"/> </summary>
		internal string[] _textLabels = null;

		/// <summary> Private field for the format of the <see cref="Axis"/> tic labels.
		/// Use the public property <see cref="Format"/> for access to this value. </summary>
		/// <seealso cref="FormatAuto"/>
		internal string	_format;

		/// <summary>
		/// Private fields for Unit types to be used for the major and minor tics.
		/// See <see cref="MajorUnit"/> and <see cref="MinorUnit"/> for the corresponding
		/// public properties.
		/// These types only apply for date-time scales (<see cref="IsDate"/>).
		/// </summary>
		/// <value>The value of these types is of enumeration type <see cref="DateUnit"/>
		/// </value>
		internal DateUnit		_majorUnit,
									_minorUnit;

		/// <summary> Private field for the alignment of the <see cref="Axis"/> tic labels.
		/// This fields controls whether the inside, center, or outside edges of the text labels are aligned.
		/// Use the public property <see cref="Scale.Align"/>
		/// for access to this value. </summary>
		/// <seealso cref="FormatAuto"/>
		internal AlignP _align;

		/// <summary> Private field for the alignment of the <see cref="Axis"/> tic labels.
		/// This fields controls whether the left, center, or right edges of the text labels are aligned.
		/// Use the public property <see cref="Scale.AlignH"/>
		/// for access to this value. </summary>
		/// <seealso cref="FormatAuto"/>
		internal AlignH _alignH;


		/// <summary> Private fields for the <see cref="Axis"/> font specificatios.
		/// Use the public properties <see cref="FontSpec"/> and
		/// <see cref="Scale.FontSpec"/> for access to these values. </summary>
		internal FontSpec _fontSpec;

		/// <summary>
		/// Internal field that stores the amount of space between the scale labels and the
		/// major tics.  Use the public property <see cref="LabelGap" /> to access this
		/// value.
		/// </summary>
		internal float _labelGap;

		/// <summary>
		/// Data range temporary values, used by GetRange().
		/// </summary>
		internal double	_rangeMin,
								_rangeMax,
								_lBound,
								_uBound;

		/// <summary>
		/// Pixel positions at the minimum and maximum value for this scale.
		/// These are temporary values used/valid only during the Draw process.
		/// </summary>
		internal float _minPix,
							_maxPix;
	
		/// <summary>
		/// Scale values for calculating transforms.  These are temporary values
		/// used ONLY during the Draw process.
		/// </summary>
		/// <remarks>
		/// These values are just <see cref="Scale.Min" /> and <see cref="Scale.Max" />
		/// for normal linear scales, but for log or exponent scales they will be a
		/// linear representation.  For <see cref="LogScale" />, it is the
		/// <see cref="Math.Log(double)" /> of the value, and for <see cref="ExponentScale" />,
		/// it is the <see cref="Math.Exp(double)" />
		/// of the value.
		/// </remarks>
		internal double	_minLinTemp,
								_maxLinTemp;

		/// <summary>
		/// Gets or sets the linearized version of the <see cref="Min" /> scale range.
		/// </summary>
		/// <remarks>
		/// This value is valid at any time, whereas <see cref="_minLinTemp" /> is an optimization
		/// pre-set that is only valid during draw operations.
		/// </remarks>
		internal double _minLinearized
		{
			get { return Linearize( _min ); }
			set { _min = DeLinearize( value ); }
		}

		/// <summary>
		/// Gets or sets the linearized version of the <see cref="Max" /> scale range.
		/// </summary>
		/// <remarks>
		/// This value is valid at any time, whereas <see cref="_maxLinTemp" /> is an optimization
		/// pre-set that is only valid during draw operations.
		/// </remarks>
		internal double _maxLinearized
		{
			get { return Linearize( _max ); }
			set { _max = DeLinearize( value ); }
		}

		/// <summary>
		/// private field that stores the owner Axis that contains this Scale instance.
		/// </summary>
		internal Axis _ownerAxis;

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="Scale"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default "zero lever" for automatically selecting the axis
			/// scale range (see <see cref="PickScale"/>). This number is
			/// used to determine when an axis scale range should be extended to
			/// include the zero value.  This value is maintained only in the
			/// <see cref="Default"/> class, and cannot be changed after compilation.
			/// </summary>
			public static double ZeroLever = 0.25;
			/// <summary> The default "grace" value applied to the minimum data range.
			/// This value is
			/// expressed as a fraction of the total data range.  For example, assume the data
			/// range is from 4.0 to 16.0, leaving a range of 12.0.  If MinGrace is set to
			/// 0.1, then 10% of the range, or 1.2 will be subtracted from the minimum data value.
			/// The scale will then be ranged to cover at least 2.8 to 16.0.
			/// </summary>
			/// <seealso cref="MinGrace"/>
			public static double MinGrace = 0.1;
			/// <summary> The default "grace" value applied to the maximum data range.
			/// This value is
			/// expressed as a fraction of the total data range.  For example, assume the data
			/// range is from 4.0 to 16.0, leaving a range of 12.0.  If MaxGrace is set to
			/// 0.1, then 10% of the range, or 1.2 will be added to the maximum data value.
			/// The scale will then be ranged to cover at least 4.0 to 17.2.
			/// </summary>
			/// <seealso cref="MinGrace"/>
			/// <seealso cref="MaxGrace"/>
			public static double MaxGrace = 0.1;
			/// <summary>
			/// The maximum number of text labels (major tics) that will be allowed on the plot by
			/// the automatic scaling logic.  This value applies only to <see cref="AxisType.Text"/>
			/// axes.  If there are more than MaxTextLabels on the plot, then
			/// <see cref="MajorStep"/> will be increased to reduce the number of labels.  That is,
			/// the step size might be increased to 2.0 to show only every other label.
			/// </summary>
			public static double MaxTextLabels = 12.0;
			/// <summary>
			/// The default target number of steps for automatically selecting the X axis
			/// scale step size (see <see cref="PickScale"/>).
			/// This number is an initial target value for the number of major steps
			/// on an axis.  This value is maintained only in the
			/// <see cref="Default"/> class, and cannot be changed after compilation.
			/// </summary>
			public static double TargetXSteps = 7.0;
			/// <summary>
			/// The default target number of steps for automatically selecting the Y or Y2 axis
			/// scale step size (see <see cref="PickScale"/>).
			/// This number is an initial target value for the number of major steps
			/// on an axis.  This value is maintained only in the
			/// <see cref="Default"/> class, and cannot be changed after compilation.
			/// </summary>
			public static double TargetYSteps = 7.0;
			/// <summary>
			/// The default target number of minor steps for automatically selecting the X axis
			/// scale minor step size (see <see cref="PickScale"/>).
			/// This number is an initial target value for the number of minor steps
			/// on an axis.  This value is maintained only in the
			/// <see cref="Default"/> class, and cannot be changed after compilation.
			/// </summary>
			public static double TargetMinorXSteps = 5.0;
			/// <summary>
			/// The default target number of minor steps for automatically selecting the Y or Y2 axis
			/// scale minor step size (see <see cref="PickScale"/>).
			/// This number is an initial target value for the number of minor steps
			/// on an axis.  This value is maintained only in the
			/// <see cref="Default"/> class, and cannot be changed after compilation.
			/// </summary>
			public static double TargetMinorYSteps = 5.0;
			/// <summary>
			/// The default reverse mode for the <see cref="Axis"/> scale
			/// (<see cref="IsReverse"/> property). true for a reversed scale
			/// (X decreasing to the left, Y/Y2 decreasing upwards), false otherwise.
			/// </summary>
			public static bool IsReverse = false;
			/// <summary>
			/// The default setting for the <see cref="Axis"/> scale format string
			/// (<see cref="Format"/> property).  For numeric values, this value is
			/// setting according to the <see cref="String.Format(string,object)"/> format strings.  For date
			/// type values, this value is set as per the <see cref="XDate.ToString()"/> function.
			/// </summary>
			//public static string ScaleFormat = "&dd-&mmm-&yy &hh:&nn";
			public static string Format = "g";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Year"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Year"/>.
			/// This value normally defaults to 1825 days (5 years).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeYearYear = 1825;  // 5 years
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Year"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Month"/>.
			/// This value normally defaults to 730 days (2 years).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeYearMonth = 730;  // 2 years
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Month"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Month"/>.
			/// This value normally defaults to 300 days (10 months).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeMonthMonth = 300;  // 10 months
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Day"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Day"/>.
			/// This value normally defaults to 10 days.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeDayDay = 10;  // 10 days
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Day"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Hour"/>.
			/// This value normally defaults to 3 days.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeDayHour = 3;  // 3 days
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Hour"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Hour"/>.
			/// This value normally defaults to 0.4167 days (10 hours).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeHourHour = 0.4167;  // 10 hours
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Hour"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Minute"/>.
			/// This value normally defaults to 0.125 days (3 hours).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeHourMinute = 0.125;  // 3 hours
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Minute"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Minute"/>.
			/// This value normally defaults to 6.94e-3 days (10 minutes).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeMinuteMinute = 6.94e-3;  // 10 Minutes
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Minute"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Second"/>.
			/// This value normally defaults to 2.083e-3 days (3 minutes).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeMinuteSecond = 2.083e-3;  // 3 Minutes
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// If the total span of data exceeds this number (in days), then the auto-range
			/// code will select <see cref="MajorUnit"/> = <see cref="DateUnit.Second"/>
			/// and <see cref="MinorUnit"/> = <see cref="DateUnit.Second"/>.
			/// This value normally defaults to 3.472e-5 days (3 seconds).
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			public static double RangeSecondSecond = 3.472e-5;  // 3 Seconds

			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Year"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Year"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatYearYear = "yyyy";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Year"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Month"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatYearMonth = "MMM-yyyy";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Month"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Month"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatMonthMonth = "MMM-yyyy";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Day"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Day"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatDayDay = "d-MMM";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Day"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Hour"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatDayHour = "d-MMM HH:mm";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Hour"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Hour"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatHourHour = "HH:mm";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Hour"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Minute"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatHourMinute = "HH:mm";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Minute"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Minute"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatMinuteMinute = "HH:mm";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Minute"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Second"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatMinuteSecond = "mm:ss";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Second"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Second"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatSecondSecond = "mm:ss";
			/// <summary>
			/// A default setting for the <see cref="AxisType.Date"/> auto-ranging code.
			/// This values applies only to Date-Time type axes.
			/// This is the format used for the scale values when auto-ranging code
			/// selects a <see cref="Format"/> of <see cref="DateUnit.Millisecond"/>
			/// for <see cref="MajorUnit"/> and <see cref="DateUnit.Millisecond"/> for 
			/// for <see cref="MinorUnit"/>.
			/// This value is used by the <see cref="DateScale.CalcDateStepSize"/> method.
			/// </summary>
			/// <seealso cref="System.Globalization.DateTimeFormatInfo"/>
			public static string FormatMillisecond = "ss.fff";

			/*  Prior format assignments using original XDate.ToString()
					this.scaleFormat = "&yyyy";
					this.scaleFormat = "&mmm-&yy";
					this.scaleFormat = "&mmm-&yy";
					scaleFormat = "&d-&mmm";
					this.scaleFormat = "&d-&mmm &hh:&nn";
					scaleFormat = "&hh:&nn";
					scaleFormat = "&hh:&nn";
					scaleFormat = "&hh:&nn";
					scaleFormat = "&nn:&ss";
					scaleFormat = "&nn:&ss";
			*/
			/// <summary> The default alignment of the <see cref="Axis"/> tic labels.
			/// This value controls whether the inside, center, or outside edges of the text labels are aligned.
			/// </summary>
			/// <seealso cref="AlignP"/>
			public static AlignP Align = AlignP.Center;
			/// <summary> The default alignment of the <see cref="Axis"/> tic labels.
			/// This value controls whether the left, center, or right edges of the text labels are aligned.
			/// </summary>
			/// <seealso cref="AlignH"/>
			public static AlignH AlignH = AlignH.Center;
			/// <summary>
			/// The default font family for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.Family"/> property).
			/// </summary>
			public static string FontFamily = "Arial";
			/// <summary>
			/// The default font size for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.Size"/> property).  Units are
			/// in points (1/72 inch).
			/// </summary>
			public static float FontSize = 14;
			/// <summary>
			/// The default font color for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.FontColor"/> property).
			/// </summary>
			public static Color FontColor = Color.Black;
			/// <summary>
			/// The default font bold mode for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.IsBold"/> property). true
			/// for a bold typeface, false otherwise.
			/// </summary>
			public static bool FontBold = false;
			/// <summary>
			/// The default font italic mode for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.IsItalic"/> property). true
			/// for an italic typeface, false otherwise.
			/// </summary>
			public static bool FontItalic = false;
			/// <summary>
			/// The default font underline mode for the <see cref="Axis"/> scale values
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="ZedGraph.FontSpec.IsUnderline"/> property). true
			/// for an underlined typeface, false otherwise.
			/// </summary>
			public static bool FontUnderline = false;
			/// <summary>
			/// The default color for filling in the scale text background
			/// (see <see cref="ZedGraph.Fill.Color"/> property).
			/// </summary>
			public static Color FillColor = Color.White;
			/// <summary>
			/// The default custom brush for filling in the scale text background
			/// (see <see cref="ZedGraph.Fill.Brush"/> property).
			/// </summary>
			public static Brush FillBrush = null;
			/// <summary>
			/// The default fill mode for filling in the scale text background
			/// (see <see cref="ZedGraph.Fill.Type"/> property).
			/// </summary>
			public static FillType FillType = FillType.None;
			/// <summary>
			/// The default value for <see cref="IsVisible"/>, which determines
			/// whether or not the scale values are displayed.
			/// </summary>
			public static bool IsVisible = true;
			/// <summary>
			/// The default value for <see cref="IsLabelsInside"/>, which determines
			/// whether or not the scale labels and title for the <see cref="Axis"/> will appear
			/// on the opposite side of the <see cref="Axis"/> that it normally appears.
			/// </summary>
			public static bool IsLabelsInside = false;
			/// <summary>
			/// Determines the size of the band at the beginning and end of the axis that will have labels
			/// omitted if the axis is shifted due to a non-default location using the <see cref="Axis.Cross"/>
			/// property.
			/// </summary>
			/// <remarks>
			/// This parameter applies only when <see cref="Axis.CrossAuto"/> is false.  It is scaled according
			/// to the size of the graph based on <see cref="PaneBase.BaseDimension"/>.  When a non-default
			/// axis location is selected, the first and last labels on that axis will overlap the opposing
			/// axis frame.  This parameter allows those labels to be omitted to avoid the overlap.  Set this
			/// parameter to zero to turn off the effect.
			/// </remarks>
			public static float EdgeTolerance = 6;

			/// <summary>
			/// The default setting for the gap between the outside tics (or the axis edge
			/// if there are no outside tics) and the scale labels, expressed as a fraction of
			/// the major tic size.
			/// </summary>
			public static float LabelGap = 0.3f;
		}

	#endregion

	#region constructors

		/// <summary>
		/// Basic constructor -- requires that the <see cref="Scale" /> object be intialized with
		/// a pre-existing owner <see cref="Axis" />.
		/// </summary>
		/// <param name="ownerAxis">The <see cref="Axis" /> object that is the owner of this
		/// <see cref="Scale" /> instance.</param>
		public Scale( Axis ownerAxis )
		{
			_ownerAxis = ownerAxis;

			_min = 0.0;
			_max = 1.0;
			_majorStep = 0.1;
			_minorStep = 0.1;
			_exponent = 1.0;
			_mag = 0;
			_baseTic = PointPair.Missing;

			_minGrace = Default.MinGrace;
			_maxGrace = Default.MaxGrace;

			_minAuto = true;
			_maxAuto = true;
			_majorStepAuto = true;
			_minorStepAuto = true;
			_magAuto = true;
			_formatAuto = true;

			_isReverse = Default.IsReverse;
			_isUseTenPower = true;
			_isPreventLabelOverlap = true;
			_isVisible = true;
			_isSkipFirstLabel = false;
			_isSkipLastLabel = false;
			_isSkipCrossLabel = false;

			_majorUnit = DateUnit.Day;
			_minorUnit = DateUnit.Day;

			_format = null;
			_textLabels = null;

			_isLabelsInside = Default.IsLabelsInside;
			_align = Default.Align;
			_alignH = Default.AlignH;

			_fontSpec = new FontSpec(
				Default.FontFamily, Default.FontSize,
				Default.FontColor, Default.FontBold,
				Default.FontUnderline, Default.FontItalic,
				Default.FillColor, Default.FillBrush,
				Default.FillType );

			_fontSpec.Border.IsVisible = false;
			_labelGap = Default.LabelGap;
		}

		/// <summary>
		/// Copy Constructor.  Create a new <see cref="Scale" /> object based on the specified
		/// existing one.
		/// </summary>
		/// <param name="rhs">The <see cref="Scale" /> object to be copied.</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="Scale" /></param>
		public Scale( Scale rhs, Axis owner )
		{
			_ownerAxis = owner;

			_min = rhs._min;
			_max = rhs._max;
			_majorStep = rhs._majorStep;
			_minorStep = rhs._minorStep;
			_exponent = rhs._exponent;
			_baseTic = rhs._baseTic;

			_minAuto = rhs._minAuto;
			_maxAuto = rhs._maxAuto;
			_majorStepAuto = rhs._majorStepAuto;
			_minorStepAuto = rhs._minorStepAuto;
			_magAuto = rhs._magAuto;
			_formatAuto = rhs._formatAuto;

			_minGrace = rhs._minGrace;
			_maxGrace = rhs._maxGrace;

			_mag = rhs._mag;

			_isUseTenPower = rhs._isUseTenPower;
			_isReverse = rhs._isReverse;
			_isPreventLabelOverlap = rhs._isPreventLabelOverlap;
			_isVisible = rhs._isVisible;
			_isSkipFirstLabel = rhs._isSkipFirstLabel;
			_isSkipLastLabel = rhs._isSkipLastLabel;
			_isSkipCrossLabel = rhs._isSkipCrossLabel;

			_majorUnit = rhs._majorUnit;
			_minorUnit = rhs._minorUnit;

			_format = rhs._format;

			_isLabelsInside = rhs._isLabelsInside;
			_align = rhs._align;
			_alignH = rhs._alignH;

			_fontSpec = (FontSpec) rhs._fontSpec.Clone();

			_labelGap = rhs._labelGap;

			if ( rhs._textLabels != null )
				_textLabels = (string[])rhs._textLabels.Clone();
			else
				_textLabels = null;
		}

		/// <summary>
		/// Create a new clone of the current item, with a new owner assignment
		/// </summary>
		/// <param name="owner">The new <see cref="Axis" /> instance that will be
		/// the owner of the new Scale</param>
		/// <returns>A new <see cref="Scale" /> clone.</returns>
		abstract public Scale Clone( Axis owner );
/*
		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of Clone />
		/// </summary>
		/// <remarks>
		/// Note that this method must be called with an explicit cast to ICloneable, and
		/// that it is inherently virtual.  For example:
		/// <code>
		/// ParentClass foo = new ChildClass();
		/// ChildClass bar = (ChildClass) ((ICloneable)foo).Clone();
		/// </code>
		/// Assume that ChildClass is inherited from ParentClass.  Even though foo is declared with
		/// ParentClass, it is actually an instance of ChildClass.  Calling the ICloneable implementation
		/// of Clone() on foo actually calls ChildClass.Clone() as if it were a virtual function.
		/// </remarks>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			throw new NotImplementedException( "Can't clone an abstract base type -- child types must implement ICloneable" );
			//return new PaneBase( this );
		}
*/

		/// <summary>
		/// A construction method that creates a new <see cref="Scale"/> object using the
		/// properties of an existing <see cref="Scale"/> object, but specifying a new
		/// <see cref="AxisType"/>.
		/// </summary>
		/// <remarks>
		/// This constructor is used to change the type of an existing <see cref="Axis" />.
		/// By specifying the old <see cref="Scale"/> object, you are giving a set of properties
		/// (which encompasses all fields associated with the scale, since the derived types
		/// have no fields) to be used in creating a new <see cref="Scale"/> object, only this
		/// time having the newly specified object type.</remarks>
		/// <param name="oldScale">The existing <see cref="Scale" /> object from which to
		/// copy the field data.</param>
		/// <param name="type">An <see cref="AxisType"/> representing the type of derived type
		/// of new <see cref="Scale" /> object to create.</param>
		/// <returns>The new <see cref="Scale"/> object.</returns>
		public Scale MakeNewScale( Scale oldScale, AxisType type )
		{
			switch ( type )
			{
				case AxisType.Linear:
					return new LinearScale( oldScale, _ownerAxis );
				case AxisType.Date:
					return new DateScale( oldScale, _ownerAxis );
				case AxisType.Log:
					return new LogScale( oldScale, _ownerAxis );
				case AxisType.Exponent:
					return new ExponentScale( oldScale, _ownerAxis );
				case AxisType.Ordinal:
					return new OrdinalScale( oldScale, _ownerAxis );
				case AxisType.Text:
					return new TextScale( oldScale, _ownerAxis );
				case AxisType.DateAsOrdinal:
					return new DateAsOrdinalScale( oldScale, _ownerAxis );
				case AxisType.LinearAsOrdinal:
					return new LinearAsOrdinalScale( oldScale, _ownerAxis );
				default:
					throw new Exception( "Implementation Error: Invalid AxisType" );
			}
		}
	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		// schema changed to 2 with isScaleVisible
		public const int schema = 11;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected Scale( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_min = info.GetDouble( "min" );
			_max = info.GetDouble( "max" );
			_majorStep = info.GetDouble( "majorStep" );
			_minorStep = info.GetDouble( "minorStep" );
			_exponent = info.GetDouble( "exponent" );
			_baseTic = info.GetDouble( "baseTic" );


			_minAuto = info.GetBoolean( "minAuto" );
			_maxAuto = info.GetBoolean( "maxAuto" );
			_majorStepAuto = info.GetBoolean( "majorStepAuto" );
			_minorStepAuto = info.GetBoolean( "minorStepAuto" );
			_magAuto = info.GetBoolean( "magAuto" );
			_formatAuto = info.GetBoolean( "formatAuto" );
			
			_minGrace = info.GetDouble( "minGrace" );
			_maxGrace = info.GetDouble( "maxGrace" );

			_mag = info.GetInt32( "mag" );

			_isReverse = info.GetBoolean( "isReverse" );
			_isPreventLabelOverlap = info.GetBoolean( "isPreventLabelOverlap" );
			_isUseTenPower = info.GetBoolean( "isUseTenPower" );

			_isVisible = true;
			_isVisible = info.GetBoolean( "isVisible" );

			_isSkipFirstLabel = info.GetBoolean( "isSkipFirstLabel" );
			_isSkipLastLabel = info.GetBoolean( "isSkipLastLabel" );
			_isSkipCrossLabel = info.GetBoolean( "isSkipCrossLabel" );

			_textLabels = (string[]) info.GetValue( "textLabels", typeof(string[]) );
			_format = info.GetString( "format" );

			_majorUnit = (DateUnit) info.GetValue( "majorUnit", typeof(DateUnit) );
			_minorUnit = (DateUnit) info.GetValue( "minorUnit", typeof(DateUnit) );

			_isLabelsInside = info.GetBoolean( "isLabelsInside" );
			_align = (AlignP)info.GetValue( "align", typeof( AlignP ) );
			if ( schema >= 11 )
				_alignH = (AlignH)info.GetValue( "alignH", typeof( AlignH ) );

			_fontSpec = (FontSpec)info.GetValue( "fontSpec", typeof( FontSpec ) );
			_labelGap = info.GetSingle( "labelGap" );

		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to
		/// serialize the target object
		/// </summary>
		/// <remarks>
		/// You MUST set the _ownerAxis property after deserializing a BarSettings object.
		/// </remarks>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );
			info.AddValue( "min", _min );
			info.AddValue( "max", _max );
			info.AddValue( "majorStep", _majorStep );
			info.AddValue( "minorStep", _minorStep );
			info.AddValue( "exponent", _exponent );
			info.AddValue( "baseTic", _baseTic );

			info.AddValue( "minAuto", _minAuto );
			info.AddValue( "maxAuto", _maxAuto );
			info.AddValue( "majorStepAuto", _majorStepAuto );
			info.AddValue( "minorStepAuto", _minorStepAuto );
			info.AddValue( "magAuto", _magAuto );
			info.AddValue( "formatAuto", _formatAuto );

			info.AddValue( "minGrace", _minGrace );
			info.AddValue( "maxGrace", _maxGrace );

			info.AddValue( "mag", _mag );
			info.AddValue( "isReverse", _isReverse );
			info.AddValue( "isPreventLabelOverlap", _isPreventLabelOverlap );
			info.AddValue( "isUseTenPower", _isUseTenPower );
			info.AddValue( "isVisible", _isVisible );
			info.AddValue( "isSkipFirstLabel", _isSkipFirstLabel );
			info.AddValue( "isSkipLastLabel", _isSkipLastLabel );
			info.AddValue( "isSkipCrossLabel", _isSkipCrossLabel );


			info.AddValue( "textLabels", _textLabels );
			info.AddValue( "format", _format );

			info.AddValue( "majorUnit", _majorUnit );
			info.AddValue( "minorUnit", _minorUnit );

			info.AddValue( "isLabelsInside", _isLabelsInside );
			info.AddValue( "align", _align );
			info.AddValue( "alignH", _alignH );
			info.AddValue( "fontSpec", _fontSpec );
			info.AddValue( "labelGap", _labelGap );
		}
	#endregion

	#region properties

		/// <summary>
		/// Get an <see cref="AxisType" /> enumeration that indicates the type of this scale.
		/// </summary>
		abstract public AxisType Type { get; }

		/// <summary>
		/// True if this scale is <see cref="AxisType.Log" />, false otherwise.
		/// </summary>
		public bool IsLog { get { return this is LogScale; } }
		/// <summary>
		/// True if this scale is <see cref="AxisType.Exponent" />, false otherwise.
		/// </summary>
		public bool IsExponent { get { return this is ExponentScale; } }
		/// <summary>
		/// True if this scale is <see cref="AxisType.Date" />, false otherwise.
		/// </summary>
		public bool IsDate { get { return this is DateScale; } }
		/// <summary>
		/// True if this scale is <see cref="AxisType.Text" />, false otherwise.
		/// </summary>
		public bool IsText { get { return this is TextScale; } }
		/// <summary>
		/// True if this scale is <see cref="AxisType.Ordinal" />, false otherwise.
		/// </summary>
		/// <remarks>
		/// Note that this is only true for an actual <see cref="OrdinalScale" /> class.
		/// This property will be false for other ordinal types such as
		/// <see cref="AxisType.Text" />, <see cref="AxisType.LinearAsOrdinal" />,
		/// or <see cref="AxisType.DateAsOrdinal" />.  Use the <see cref="IsAnyOrdinal" />
		/// as a "catchall" for all ordinal type axes.
		/// </remarks>
		public bool IsOrdinal { get { return this is OrdinalScale; } }

		/// <summary>
		/// Gets a value that indicates if this <see cref="Scale" /> is of any of the
		/// ordinal types in the <see cref="AxisType" /> enumeration.
		/// </summary>
		/// <seealso cref="Type" />
		public bool IsAnyOrdinal
		{
			get
			{
				AxisType type = this.Type;

				return	type == AxisType.Ordinal ||
							type == AxisType.Text ||
							type == AxisType.LinearAsOrdinal ||
							type == AxisType.DateAsOrdinal;
			}
		}
/*
		/// <summary>
		/// The pixel position at the minimum value for this axis.  This read-only
		/// value is used/valid only during the Draw process.
		/// </summary>
		public float MinPix
		{
			get { return _minPix; }
		}
		/// <summary>
		/// The pixel position at the maximum value for this axis.  This read-only
		/// value is used/valid only during the Draw process.
		/// </summary>
		public float MaxPix
		{
			get { return _maxPix; }
		}
*/
		/// <summary>
		/// Gets or sets the minimum scale value for this <see cref="Scale" />.
		/// </summary>
		/// <remarks>This value can be set
		/// automatically based on the state of <see cref="MinAuto"/>.  If
		/// this value is set manually, then <see cref="MinAuto"/> will
		/// also be set to false.
		/// </remarks>
		/// <value> The value is defined in user scale units for <see cref="AxisType.Log"/>
		/// and <see cref="AxisType.Linear"/> axes. For <see cref="AxisType.Text"/>
		/// and <see cref="AxisType.Ordinal"/> axes,
		/// this value is an ordinal starting with 1.0.  For <see cref="AxisType.Date"/>
		/// axes, this value is in XL Date format (see <see cref="XDate"/>, which is the
		/// number of days since the reference date of January 1, 1900.</value>
		/// <seealso cref="Max"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MinAuto"/>
		public virtual double Min
		{
			get { return _min; }
			set { _min = value; _minAuto = false; }
		}
		/// <summary>
		/// Gets or sets the maximum scale value for this <see cref="Scale" />.
		/// </summary>
		/// <remarks>
		/// This value can be set
		/// automatically based on the state of <see cref="MaxAuto"/>.  If
		/// this value is set manually, then <see cref="MaxAuto"/> will
		/// also be set to false.
		/// </remarks>
		/// <value> The value is defined in user scale units for <see cref="AxisType.Log"/>
		/// and <see cref="AxisType.Linear"/> axes. For <see cref="AxisType.Text"/>
		/// and <see cref="AxisType.Ordinal"/> axes,
		/// this value is an ordinal starting with 1.0.  For <see cref="AxisType.Date"/>
		/// axes, this value is in XL Date format (see <see cref="XDate"/>, which is the
		/// number of days since the reference date of January 1, 1900.</value>
		/// <seealso cref="Min"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MaxAuto"/>
		public virtual double Max
		{
			get { return _max; }
			set { _max = value; _maxAuto = false; }
		}
		/// <summary>
		/// Gets or sets the scale step size for this <see cref="Scale" /> (the increment between
		/// labeled axis values).
		/// </summary>
		/// <remarks>
		/// This value can be set
		/// automatically based on the state of <see cref="MajorStepAuto"/>.  If
		/// this value is set manually, then <see cref="MajorStepAuto"/> will
		/// also be set to false.  This value is ignored for <see cref="AxisType.Log"/>
		/// axes.  For <see cref="AxisType.Date"/> axes, this
		/// value is defined in units of <see cref="MajorUnit"/>.
		/// </remarks>
		/// <value> The value is defined in user scale units </value>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MajorStepAuto"/>
		/// <seealso cref="ZedGraph.Scale.Default.TargetXSteps"/>
		/// <seealso cref="ZedGraph.Scale.Default.TargetYSteps"/>
		/// <seealso cref="ZedGraph.Scale.Default.ZeroLever"/>
		/// <seealso cref="ZedGraph.Scale.Default.MaxTextLabels"/>
		public double MajorStep
		{
			get { return _majorStep; }
			set
			{
				if ( value < 1e-300 )
				{
					_majorStepAuto = true;
				}
				else
				{
					_majorStep = value;
					_majorStepAuto = false;
				}
			}
		}
		/// <summary>
		/// Gets or sets the scale minor step size for this <see cref="Scale" /> (the spacing between
		/// minor tics).
		/// </summary>
		/// <remarks>This value can be set
		/// automatically based on the state of <see cref="MinorStepAuto"/>.  If
		/// this value is set manually, then <see cref="MinorStepAuto"/> will
		/// also be set to false.  This value is ignored for <see cref="AxisType.Log"/> and
		/// <see cref="AxisType.Text"/> axes.  For <see cref="AxisType.Date"/> axes, this
		/// value is defined in units of <see cref="MinorUnit"/>.
		/// </remarks>
		/// <value> The value is defined in user scale units </value>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStepAuto"/>
		public double MinorStep
		{
			get { return _minorStep; }
			set
			{
				if ( value < 1e-300 )
				{
					_minorStepAuto = true;
				}
				else
				{
					_minorStep = value;
					_minorStepAuto = false;
				}
			}
		}
		/// <summary>
		/// Gets or sets the scale exponent value.  This only applies to <see cref="AxisType.Exponent" />. 
		/// </summary>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MajorStepAuto"/>
		/// <seealso cref="ZedGraph.Scale.Default.TargetXSteps"/>
		/// <seealso cref="ZedGraph.Scale.Default.TargetYSteps"/>
		/// <seealso cref="ZedGraph.Scale.Default.ZeroLever"/>
		/// <seealso cref="ZedGraph.Scale.Default.MaxTextLabels"/>
		public double Exponent
		{
			get { return _exponent; }
			set { _exponent = value; }
		}

		/// <summary>
		/// Gets or sets the scale value at which the first major tic label will appear.
		/// </summary>
		/// <remarks>This property allows the scale labels to start at an irregular value.
		/// For example, on a scale range with <see cref="Min"/> = 0, <see cref="Max"/> = 1000,
		/// and <see cref="MajorStep"/> = 200, a <see cref="BaseTic"/> value of 50 would cause
		/// the scale labels to appear at values 50, 250, 450, 650, and 850.  Note that the
		/// default value for this property is <see cref="PointPairBase.Missing"/>, which means the
		/// value is not used.  Setting this property to any value other than
		/// <see cref="PointPairBase.Missing"/> will activate the effect.  The value specified must
		/// coincide with the first major tic.  That is, if <see cref="BaseTic"/> were set to
		/// 650 in the example above, then the major tics would only occur at 650 and 850.  This
		/// setting may affect the minor tics, since the minor tics are always referenced to the
		/// <see cref="BaseTic"/>.  That is, in the example above, if the <see cref="MinorStep"/>
		/// were set to 30 (making it a non-multiple of the major step), then the minor tics would
		/// occur at 20, 50 (so it lines up with the BaseTic), 80, 110, 140, etc.
		/// </remarks>
		/// <value> The value is defined in user scale units </value>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="Axis.Cross"/>
		public double BaseTic
		{
			get { return _baseTic; }
			set { _baseTic = value; }
		}

		/// <summary>
		/// Gets or sets the type of units used for the major step size (<see cref="MajorStep"/>).
		/// </summary>
		/// <remarks>
		/// This unit type only applies to Date-Time axes (<see cref="AxisType.Date"/> = true).
		/// The axis is set to date type with the <see cref="Type"/> property.
		/// The unit types are defined as <see cref="DateUnit"/>.
		/// </remarks>
		/// <value> The value is a <see cref="DateUnit"/> enum type </value>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MajorStepAuto"/>
		public DateUnit MajorUnit
		{
			get { return _majorUnit; }
			set { _majorUnit = value; }
		}
		/// <summary>
		/// Gets or sets the type of units used for the minor step size (<see cref="MinorStep"/>).
		/// </summary>
		/// <remarks>
		/// This unit type only applies to Date-Time axes (<see cref="AxisType.Date"/> = true).
		/// The axis is set to date type with the <see cref="Type"/> property.
		/// The unit types are defined as <see cref="DateUnit"/>.
		/// </remarks>
		/// <value> The value is a <see cref="DateUnit"/> enum type </value>
		/// <seealso cref="Min"/>
		/// <seealso cref="Max"/>
		/// <seealso cref="MajorStep"/>
		/// <seealso cref="MinorStep"/>
		/// <seealso cref="MinorStepAuto"/>
		public DateUnit MinorUnit
		{
			get { return _minorUnit; }
			set { _minorUnit = value; }
		}

		/// <summary>
		/// Gets the major unit multiplier for this scale type, if any.
		/// </summary>
		/// <remarks>The major unit multiplier will correct the units of
		/// <see cref="MajorStep" /> to match the units of <see cref="Min" />
		/// and <see cref="Max" />.  This reflects the setting of
		/// <see cref="MajorUnit" />.
		/// </remarks>
		virtual internal double MajorUnitMultiplier
		{
			get { return 1.0; }
		}

		/// <summary>
		/// Gets the minor unit multiplier for this scale type, if any.
		/// </summary>
		/// <remarks>The minor unit multiplier will correct the units of
		/// <see cref="MinorStep" /> to match the units of <see cref="Min" />
		/// and <see cref="Max" />.  This reflects the setting of
		/// <see cref="MinorUnit" />.
		/// </remarks>
		virtual internal double MinorUnitMultiplier
		{
			get { return 1.0; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not the minimum scale value <see cref="Min"/>
		/// is set automatically.
		/// </summary>
		/// <remarks>
		/// This value will be set to false if
		/// <see cref="Min"/> is manually changed.
		/// </remarks>
		/// <value>true for automatic mode, false for manual mode</value>
		/// <seealso cref="Min"/>
		public bool MinAuto
		{
			get { return _minAuto; }
			set { _minAuto = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not the maximum scale value <see cref="Max"/>
		/// is set automatically.
		/// </summary>
		/// <remarks>
		/// This value will be set to false if
		/// <see cref="Max"/> is manually changed.
		/// </remarks>
		/// <value>true for automatic mode, false for manual mode</value>
		/// <seealso cref="Max"/>
		public bool MaxAuto
		{
			get { return _maxAuto; }
			set { _maxAuto = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not the scale step size <see cref="MajorStep"/>
		/// is set automatically.
		/// </summary>
		/// <remarks>
		/// This value will be set to false if
		/// <see cref="MajorStep"/> is manually changed.
		/// </remarks>
		/// <value>true for automatic mode, false for manual mode</value>
		/// <seealso cref="MajorStep"/>
		public bool MajorStepAuto
		{
			get { return _majorStepAuto; }
			set { _majorStepAuto = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not the minor scale step size <see cref="MinorStep"/>
		/// is set automatically.
		/// </summary>
		/// <remarks>
		/// This value will be set to false if
		/// <see cref="MinorStep"/> is manually changed.
		/// </remarks>
		/// <value>true for automatic mode, false for manual mode</value>
		/// <seealso cref="MinorStep"/>
		public bool MinorStepAuto
		{
			get { return _minorStepAuto; }
			set { _minorStepAuto = value; }
		}

		/// <summary>
		/// Determines whether or not the scale label format <see cref="Format"/>
		/// is determined automatically based on the range of data values.
		/// </summary>
		/// <remarks>
		/// This value will be set to false if
		/// <see cref="Format"/> is manually changed.
		/// </remarks>
		/// <value>true if <see cref="Format"/> will be set automatically, false
		/// if it is to be set manually by the user</value>
		/// <seealso cref="Mag"/>
		/// <seealso cref="Format"/>
		/// <seealso cref="FontSpec"/>
		public bool FormatAuto
		{
			get { return _formatAuto; }
			set { _formatAuto = value; }
		}

		/// <summary>
		/// The format of the <see cref="Axis"/> tic labels.
		/// </summary>
		/// <remarks>
		/// This property may be a date format or a numeric format, depending on the setting of
		/// <see cref="Type">Scale.Type</see>.
		/// This property may be set automatically by ZedGraph, depending on the state of
		/// <see cref="FormatAuto"/>.
		/// </remarks>
		/// <value>The format string conforms to the
		/// <see cref="System.Globalization.DateTimeFormatInfo" /> for date formats, and
		/// <see cref="System.Globalization.NumberFormatInfo" /> for numeric formats.
		/// </value>
		/// <seealso cref="Mag"/>
		/// <seealso cref="FormatAuto"/>
		/// <seealso cref="FontSpec"/>
		// /// <seealso cref="NumDec"/>
		public string Format
		{
			get { return _format; }
			set { _format = value; _formatAuto = false; }
		}

		/// <summary>
		/// The magnitude multiplier for scale values.
		/// </summary>
		/// <remarks>
		/// This is used to limit
		/// the size of the displayed value labels.  For example, if the value
		/// is really 2000000, then the graph will display 2000 with a 10^3
		/// magnitude multiplier.  This value can be determined automatically
		/// depending on the state of <see cref="MagAuto"/>.
		/// If this value is set manually by the user,
		/// then <see cref="MagAuto"/> will also be set to false.
		/// </remarks>
		/// <value>The magnitude multiplier (power of 10) for the scale
		/// value labels</value>
		/// <seealso cref="AxisLabel.IsOmitMag"/>
		/// <seealso cref="Axis.Title"/>
		/// <seealso cref="Format"/>
		/// <seealso cref="FontSpec"/>
		// /// <seealso cref="NumDec"/>
		public int Mag
		{
			get { return _mag; }
			set { _mag = value; _magAuto = false; }
		}
		/// <summary>
		/// Determines whether the <see cref="Mag"/> value will be set
		/// automatically based on the data, or manually by the user.
		/// </summary>
		/// <remarks>
		/// If the user manually sets the <see cref="Mag"/> value, then this
		/// flag will be set to false.
		/// </remarks>
		/// <value>true to have <see cref="Mag"/> set automatically,
		/// false otherwise</value>
		/// <seealso cref="AxisLabel.IsOmitMag"/>
		/// <seealso cref="Axis.Title"/>
		/// <seealso cref="Mag"/>
		public bool MagAuto
		{
			get { return _magAuto; }
			set { _magAuto = value; }
		}

		/// <summary> Gets or sets the "grace" value applied to the minimum data range.
		/// </summary>
		/// <remarks>
		/// This value is
		/// expressed as a fraction of the total data range.  For example, assume the data
		/// range is from 4.0 to 16.0, leaving a range of 12.0.  If MinGrace is set to
		/// 0.1, then 10% of the range, or 1.2 will be subtracted from the minimum data value.
		/// The scale will then be ranged to cover at least 2.8 to 16.0.
		/// </remarks>
		/// <seealso cref="Min"/>
		/// <seealso cref="ZedGraph.Scale.Default.MinGrace"/>
		/// <seealso cref="MaxGrace"/>
		public double MinGrace
		{
			get { return _minGrace; }
			set { _minGrace = value; }
		}
		/// <summary> Gets or sets the "grace" value applied to the maximum data range.
		/// </summary>
		/// <remarks>
		/// This values determines how much extra space is left after the last data value.
		/// This value is
		/// expressed as a fraction of the total data range.  For example, assume the data
		/// range is from 4.0 to 16.0, leaving a range of 12.0.  If MaxGrace is set to
		/// 0.1, then 10% of the range, or 1.2 will be added to the maximum data value.
		/// The scale will then be ranged to cover at least 4.0 to 17.2.
		/// </remarks>
		/// <seealso cref="Max"/>
		/// <seealso cref="ZedGraph.Scale.Default.MaxGrace"/>
		/// <seealso cref="MinGrace"/>
		public double MaxGrace
		{
			get { return _maxGrace; }
			set { _maxGrace = value; }
		}

		/// <summary> Controls the alignment of the <see cref="Axis"/> tic labels.
		/// </summary>
		/// <remarks>
		/// This property controls whether the inside, center, or outside edges of the
		/// text labels are aligned.
		/// </remarks>
		public AlignP Align
		{
			get { return _align; }
			set { _align = value; }
		}

		/// <summary> Controls the alignment of the <see cref="Axis"/> tic labels.
		/// </summary>
		/// <remarks>
		/// This property controls whether the left, center, or right edges of the
		/// text labels are aligned.
		/// </remarks>
		public AlignH AlignH
		{
			get { return _alignH; }
			set { _alignH = value; }
		}

		/// <summary>
		/// Gets a reference to the <see cref="ZedGraph.FontSpec"/> class used to render
		/// the scale values
		/// </summary>
		/// <seealso cref="Default.FontFamily"/>
		/// <seealso cref="Default.FontSize"/>
		/// <seealso cref="Default.FontColor"/>
		/// <seealso cref="Default.FontBold"/>
		/// <seealso cref="Default.FontUnderline"/>
		/// <seealso cref="Default.FontItalic"/>
		public FontSpec FontSpec
		{
			get { return _fontSpec; }
			set
			{
				if ( value == null )
					throw new ArgumentNullException( "Uninitialized FontSpec in Scale" );
				_fontSpec = value;
			}
		}

		/// <summary>
		/// The gap between the scale labels and the tics.
		/// </summary>
		public float LabelGap
		{
			get { return _labelGap; }
			set { _labelGap = value; }
		}

		/// <summary>
		/// Gets or sets a value that causes the axis scale labels and title to appear on the
		/// opposite side of the axis.
		/// </summary>
		/// <remarks>
		/// For example, setting this flag to true for the <see cref="YAxis"/> will shift the
		/// axis labels and title to the right side of the <see cref="YAxis"/> instead of the
		/// normal left-side location.  Set this property to true for the <see cref="XAxis" />,
		/// and set the <see cref="Axis.Cross"/> property for the <see cref="XAxis"/> to an arbitrarily
		/// large value (assuming <see cref="IsReverse"/> is false for the <see cref="YAxis" />) in
		/// order to have the <see cref="XAxis"/> appear at the top of the <see cref="Chart.Rect" />.
		/// </remarks>
		/// <seealso cref="IsReverse"/>
		/// <seealso cref="Axis.Cross"/>
		public bool IsLabelsInside
		{
			get { return _isLabelsInside; }
			set { _isLabelsInside = value; }
		}

		/// <summary>
		/// Gets or sets a value that causes the first scale label for this <see cref="Axis"/> to be
		/// hidden.
		/// </summary>
		/// <remarks>
		/// Often, for axis that have an active <see cref="Axis.Cross"/> setting (e.g., <see cref="Axis.CrossAuto"/>
		/// is false), the first and/or last scale label are overlapped by opposing axes.  Use this
		/// property to hide the first scale label to avoid the overlap.  Note that setting this value
		/// to true will hide any scale label that appears within <see cref="Scale.Default.EdgeTolerance"/> of the
		/// beginning of the <see cref="Axis"/>.
		/// </remarks>
		public bool IsSkipFirstLabel
		{
			get { return _isSkipFirstLabel; }
			set { _isSkipFirstLabel = value; }
		}

		/// <summary>
		/// Gets or sets a value that causes the last scale label for this <see cref="Axis"/> to be
		/// hidden.
		/// </summary>
		/// <remarks>
		/// Often, for axis that have an active <see cref="Axis.Cross"/> setting (e.g., <see cref="Axis.CrossAuto"/>
		/// is false), the first and/or last scale label are overlapped by opposing axes.  Use this
		/// property to hide the last scale label to avoid the overlap.  Note that setting this value
		/// to true will hide any scale label that appears within <see cref="Scale.Default.EdgeTolerance"/> of the
		/// end of the <see cref="Axis"/>.
		/// </remarks>
		public bool IsSkipLastLabel
		{
			get { return _isSkipLastLabel; }
			set { _isSkipLastLabel = value; }
		}

		/// <summary>
		/// Gets or sets a value that causes the scale label that is located at the <see cref="Axis.Cross" />
		/// value for this <see cref="Axis"/> to be hidden.
		/// </summary>
		/// <remarks>
		/// For axes that have an active <see cref="Axis.Cross"/> setting (e.g., <see cref="Axis.CrossAuto"/>
		/// is false), the scale label at the <see cref="Axis.Cross" /> value is overlapped by opposing axes.
		/// Use this property to hide the scale label to avoid the overlap.
		/// </remarks>
		public bool IsSkipCrossLabel
		{
			get { return _isSkipCrossLabel; }
			set { _isSkipCrossLabel = value; }
		}

		/// <summary>
		/// Determines if the scale values are reversed for this <see cref="Axis"/>
		/// </summary>
		/// <value>true for the X values to decrease to the right or the Y values to
		/// decrease upwards, false otherwise</value>
		/// <seealso cref="ZedGraph.Scale.Default.IsReverse"/>.
		public bool IsReverse
		{
			get { return _isReverse; }
			set { _isReverse = value; }
		}
		/// <summary>
		/// Determines if powers-of-ten notation will be used for the numeric value labels.
		/// </summary>
		/// <remarks>
		/// The powers-of-ten notation is just the text "10" followed by a superscripted value
		/// indicating the magnitude.  This mode is only valid for log scales (see
		/// <see cref="IsLog"/> and <see cref="Type"/>).
		/// </remarks>
		/// <value> boolean value; true to show the title as a power of ten, false to
		/// show a regular numeric value (e.g., "0.01", "10", "1000")</value>
		public bool IsUseTenPower
		{
			get { return _isUseTenPower; }
			set { _isUseTenPower = value; }
		}

		/// <summary>
		/// Gets or sets a <see cref="bool"/> value that determines if ZedGraph will check to
		/// see if the <see cref="Axis"/> scale labels are close enough to overlap.  If so,
		/// ZedGraph will adjust the step size to prevent overlap.
		/// </summary>
		/// <remarks>
		/// The process of checking for overlap is done during the <see cref="GraphPane.AxisChange()"/>
		/// method call, and affects the selection of the major step size (<see cref="MajorStep"/>).
		/// </remarks>
		/// <value> boolean value; true to check for overlap, false otherwise</value>
		public bool IsPreventLabelOverlap
		{
			get { return _isPreventLabelOverlap; }
			set { _isPreventLabelOverlap = value; }
		}

		/// <summary>
		/// Gets or sets a property that determines whether or not the scale values will be shown.
		/// </summary>
		/// <value>true to show the scale values, false otherwise</value>
		/// <seealso cref="Axis.IsVisible"/>.
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		/// <summary>
		/// The text labels for this <see cref="Axis"/>.
		/// </summary>
		/// <remarks>
		/// This property is only
		/// applicable if <see cref="Type"/> is set to <see cref="AxisType.Text"/>.
		/// </remarks>
		public string[] TextLabels
		{
			get { return _textLabels; }
			set { _textLabels = value; }
		}

	#endregion
/*
	#region events

		/// <summary>
		/// A delegate that allows full custom formatting of the Axis labels
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane" /> for which the label is to be
		/// formatted</param>
		/// <param name="axis">The <see cref="Axis" /> for which the label is to be formatted</param>
		/// <param name="val">The value to be formatted</param>
		/// <param name="index">The zero-based index of the label to be formatted</param>
		/// <returns>
		/// A string value representing the label, or null if the ZedGraph should go ahead
		/// and generate the label according to the current settings</returns>
		/// <seealso cref="ScaleFormatEvent" />
		public delegate string ScaleFormatHandler( GraphPane pane, Axis axis, double val, int index );

		/// <summary>
		/// Subscribe to this event to handle custom formatting of the scale labels.
		/// </summary>
		public event ScaleFormatHandler ScaleFormatEvent;

	#endregion
*/
	#region Methods

		/// <summary>
		/// Setup some temporary transform values in preparation for rendering the
		/// <see cref="Axis"/>.
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
		virtual public void SetupScaleData( GraphPane pane, Axis axis )
		{
			// save the ChartRect data for transforming scale values to pixels
			if ( axis is XAxis || axis is X2Axis )
			{
				_minPix = pane.Chart._rect.Left;
				_maxPix = pane.Chart._rect.Right;
			}
			else
			{
				_minPix = pane.Chart._rect.Top;
				_maxPix = pane.Chart._rect.Bottom;
			}

			_minLinTemp = Linearize( _min );
			_maxLinTemp = Linearize( _max );

		}

/*		internal void ResetScaleData()
		{
			_minPix = float.NaN;
			_maxPix = float.NaN;
			_minLinTemp = double.NaN;
			_maxLinTemp = double.NaN;
		}
*/
		/// <summary>
		/// Convert a value to its linear equivalent for this type of scale.
		/// </summary>
		/// <remarks>
		/// The default behavior is to just return the value unchanged.  However,
		/// for <see cref="AxisType.Log" /> and <see cref="AxisType.Exponent" />,
		/// it returns the log or power equivalent.
		/// </remarks>
		/// <param name="val">The value to be converted</param>
		virtual public double Linearize( double val )
		{
			return val;
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
		virtual public double DeLinearize( double val )
		{
			return val;
		}
/*
		/// <summary>
		/// Make a value label for the axis at the specified ordinal position.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="IsLog"/>, <see cref="IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="index">
		/// The zero-based, ordinal index of the label to be generated.  For example, a value of 2 would
		/// cause the third value label on the axis to be generated.
		/// </param>
		/// <param name="dVal">
		/// The numeric value associated with the label.  This value is ignored for log (<see cref="IsLog"/>)
		/// and text (<see cref="IsText"/>) type axes.
		/// </param>
		/// <returns>The resulting value label as a <see cref="string" /></returns>
		virtual internal string MakeLabel( GraphPane pane, int index, double dVal )
		{
			if ( this.ScaleFormatEvent != null )
			{
				string label;

				label = this.ScaleFormatEvent( pane, _ownerAxis, dVal, index );
				if ( label != null )
					return label;
			}

			if ( _format == null )
				_format = Scale.Default.Format;

			// linear or ordinal is the default behavior
			// this method is overridden for other Scale types

			double scaleMult = Math.Pow( (double) 10.0, _mag );

			return ( dVal / scaleMult ).ToString( _format );
		}
*/

		/// <summary>
		/// Make a value label for the axis at the specified ordinal position.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="IsLog"/>, <see cref="IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="index">
		/// The zero-based, ordinal index of the label to be generated.  For example, a value of 2 would
		/// cause the third value label on the axis to be generated.
		/// </param>
		/// <param name="dVal">
		/// The numeric value associated with the label.  This value is ignored for log (<see cref="IsLog"/>)
		/// and text (<see cref="IsText"/>) type axes.
		/// </param>
		/// <returns>The resulting value label as a <see cref="string" /></returns>
		virtual internal string MakeLabel( GraphPane pane, int index, double dVal )
		{
			if ( _format == null )
				_format = Scale.Default.Format;

			// linear or ordinal is the default behavior
			// this method is overridden for other Scale types

			double scaleMult = Math.Pow( (double)10.0, _mag );

			return ( dVal / scaleMult ).ToString( _format );
		}

		/// <summary>
		/// Get the maximum width of the scale value text that is required to label this
		/// <see cref="Axis"/>.
		/// The results of this method are used to determine how much space is required for
		/// the axis labels.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="applyAngle">
		/// true to get the bounding box of the text using the <see cref="ZedGraph.FontSpec.Angle" />,
		/// false to just get the bounding box without rotation
		/// </param>
		/// <returns>the maximum width of the text in pixel units</returns>
		internal SizeF GetScaleMaxSpace( Graphics g, GraphPane pane, float scaleFactor,
							bool applyAngle )
		{
			if ( _isVisible )
			{
				double dVal,
					scaleMult = Math.Pow( (double)10.0, _mag );
				int i;

				float saveAngle = _fontSpec.Angle;
				if ( !applyAngle )
					_fontSpec.Angle = 0;

				int nTics = CalcNumTics();

				double startVal = CalcBaseTic();

				SizeF maxSpace = new SizeF( 0, 0 );

				// Repeat for each tic
				for ( i = 0; i < nTics; i++ )
				{
					dVal = CalcMajorTicValue( startVal, i );

					// draw the label
					//string tmpStr = MakeLabel( pane, i, dVal );
					string tmpStr = _ownerAxis.MakeLabelEventWorks( pane, i, dVal );

					SizeF sizeF;
					if ( this.IsLog && _isUseTenPower )
						sizeF = _fontSpec.BoundingBoxTenPower( g, tmpStr,
							scaleFactor );
					else
						sizeF = _fontSpec.BoundingBox( g, tmpStr,
							scaleFactor );

					if ( sizeF.Height > maxSpace.Height )
						maxSpace.Height = sizeF.Height;
					if ( sizeF.Width > maxSpace.Width )
						maxSpace.Width = sizeF.Width;
				}

				_fontSpec.Angle = saveAngle;

				return maxSpace;
			}
			else
				return new SizeF(0,0);
		}

		/// <summary>
		/// Determine the value for any major tic.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="IsLog"/>, <see cref="IsText"/>,
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
		virtual internal double CalcMajorTicValue( double baseVal, double tic )
		{
			// Default behavior is a normal linear scale (also works for ordinal types)
			return baseVal + (double) _majorStep * tic;
		}

		/// <summary>
		/// Determine the value for any minor tic.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="IsLog"/>, <see cref="IsText"/>,
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
		virtual internal double CalcMinorTicValue( double baseVal, int iTic )
		{
			// default behavior is a linear axis (works for ordinal types too
			return baseVal + (double) _minorStep * (double) iTic;
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
		virtual internal int CalcMinorStart( double baseVal )
		{
			// Default behavior is for a linear scale (works for ordinal as well
			return (int) ( ( _min - baseVal ) / _minorStep );
		}

		/// <summary>
		/// Determine the value for the first major tic.
		/// </summary>
		/// <remarks>
		/// This is done by finding the first possible value that is an integral multiple of
		/// the step size, taking into account the date/time units if appropriate.
		/// This method properly accounts for <see cref="IsLog"/>, <see cref="IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <returns>
		/// First major tic value (floating point double).
		/// </returns>
		virtual internal double CalcBaseTic()
		{
			if ( _baseTic != PointPair.Missing )
				return _baseTic;
			else if ( IsAnyOrdinal )
			{
				// basetic is always 1 for ordinal types
				return 1;
			}
			else
			{
				// default behavior is linear or ordinal type
				// go to the nearest even multiple of the step size
				return Math.Ceiling( (double)_min / (double)_majorStep - 0.00000001 )
														* (double)_majorStep;
			}
		}

		/// <summary>
		/// Draw the value labels, tic marks, and grid lines as
		/// required for this <see cref="Axis"/>.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="baseVal">
		/// The first major tic value for the axis
		/// </param>
		/// <param name="nTics">
		/// The total number of major tics for the axis
		/// </param>
		/// <param name="topPix">
		/// The pixel location of the far side of the ChartRect from this axis.
		/// This value is the ChartRect.Height for the XAxis, or the ChartRect.Width
		/// for the YAxis and Y2Axis.
		/// </param>
		/// <param name="shift">The number of pixels to shift this axis, based on the
		/// value of <see cref="Axis.Cross"/>.  A positive value is into the ChartRect relative to
		/// the default axis position.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		internal void DrawLabels( Graphics g, GraphPane pane, double baseVal, int nTics,
						float topPix, float shift, float scaleFactor )
		{
			MajorTic tic = _ownerAxis._majorTic;
//			MajorGrid grid = _ownerAxis._majorGrid;

			double dVal, dVal2;
			float pixVal, pixVal2;
			float scaledTic = tic.ScaledTic( scaleFactor );

			double scaleMult = Math.Pow( (double)10.0, _mag );

			using ( Pen ticPen = tic.GetPen( pane, scaleFactor ) )
//			using ( Pen gridPen = grid.GetPen( pane, scaleFactor ) )
			{
				// get the Y position of the center of the axis labels
				// (the axis itself is referenced at zero)
				SizeF maxLabelSize = GetScaleMaxSpace( g, pane, scaleFactor, true );
				float charHeight = _fontSpec.GetHeight( scaleFactor );
				float maxSpace = maxLabelSize.Height;

				float edgeTolerance = Default.EdgeTolerance * scaleFactor;
				double rangeTol = ( _maxLinTemp - _minLinTemp ) * 0.001;

				int firstTic = (int)( ( _minLinTemp - baseVal ) / _majorStep + 0.99 );
				if ( firstTic < 0 )
					firstTic = 0;

				// save the position of the previous tic
				float lastPixVal = -10000;

				// loop for each major tic
				for ( int i = firstTic; i < nTics + firstTic; i++ )
				{
					dVal = CalcMajorTicValue( baseVal, i );

					// If we're before the start of the scale, just go to the next tic
					if ( dVal < _minLinTemp )
						continue;
					// if we've already past the end of the scale, then we're done
					if ( dVal > _maxLinTemp + rangeTol )
						break;

					// convert the value to a pixel position
					pixVal = LocalTransform( dVal );

					// see if the tic marks will be drawn between the labels instead of at the labels
					// (this applies only to AxisType.Text
					if ( tic._isBetweenLabels && IsText )
					{
						// We need one extra tic in order to draw the tics between labels
						// so provide an exception here
						if ( i == 0 )
						{
							dVal2 = CalcMajorTicValue( baseVal, -0.5 );
							if ( dVal2 >= _minLinTemp )
							{
								pixVal2 = LocalTransform( dVal2 );
								tic.Draw( g, pane, ticPen, pixVal2, topPix, shift, scaledTic );

//								grid.Draw( g, gridPen, pixVal2, topPix );
							}
						}

						dVal2 = CalcMajorTicValue( baseVal, (double)i + 0.5 );
						if ( dVal2 > _maxLinTemp )
							break;
						pixVal2 = LocalTransform( dVal2 );
					}
					else
						pixVal2 = pixVal;

					tic.Draw( g, pane, ticPen, pixVal2, topPix, shift, scaledTic );
					// draw the grid
//					grid.Draw( g, gridPen, pixVal2, topPix );

					bool isMaxValueAtMaxPix = ( ( _ownerAxis is XAxis || _ownerAxis is Y2Axis ) &&
															!IsReverse ) ||
												( _ownerAxis is Y2Axis && IsReverse );

					bool isSkipZone = ( ( ( _isSkipFirstLabel && isMaxValueAtMaxPix ) ||
											( _isSkipLastLabel && !isMaxValueAtMaxPix ) ) &&
												pixVal < edgeTolerance ) ||
										( ( ( _isSkipLastLabel && isMaxValueAtMaxPix ) ||
											( _isSkipFirstLabel && !isMaxValueAtMaxPix ) ) &&
												pixVal > _maxPix - _minPix - edgeTolerance );

					bool isSkipCross = _isSkipCrossLabel && !_ownerAxis._crossAuto &&
									Math.Abs( _ownerAxis._cross - dVal ) < rangeTol * 10.0;

					isSkipZone = isSkipZone || isSkipCross;

					if ( _isVisible && !isSkipZone )
					{
						// For exponential scales, just skip any label that would overlap with the previous one
						// This is because exponential scales have varying label spacing
						if ( IsPreventLabelOverlap &&
								Math.Abs( pixVal - lastPixVal ) < maxLabelSize.Width )
							continue;

						DrawLabel( g, pane, i, dVal, pixVal, shift, maxSpace, scaledTic, charHeight, scaleFactor );
						lastPixVal = pixVal;
					}
				}
			}
		}

		internal void DrawGrid( Graphics g, GraphPane pane, double baseVal, float topPix, float scaleFactor )
		{
			MajorTic tic = _ownerAxis._majorTic;
			MajorGrid grid = _ownerAxis._majorGrid;

			int nTics = CalcNumTics();

			double dVal, dVal2;
			float pixVal, pixVal2;

			using ( Pen gridPen = grid.GetPen( pane, scaleFactor ) )
			{
				// get the Y position of the center of the axis labels
				// (the axis itself is referenced at zero)
//				SizeF maxLabelSize = GetScaleMaxSpace( g, pane, scaleFactor, true );
//				float charHeight = _fontSpec.GetHeight( scaleFactor );
//				float maxSpace = maxLabelSize.Height;

//				float edgeTolerance = Default.EdgeTolerance * scaleFactor;
				double rangeTol = ( _maxLinTemp - _minLinTemp ) * 0.001;

				int firstTic = (int)( ( _minLinTemp - baseVal ) / _majorStep + 0.99 );
				if ( firstTic < 0 )
					firstTic = 0;

				// save the position of the previous tic
//				float lastPixVal = -10000;

				// loop for each major tic
				for ( int i = firstTic; i < nTics + firstTic; i++ )
				{
					dVal = CalcMajorTicValue( baseVal, i );

					// If we're before the start of the scale, just go to the next tic
					if ( dVal < _minLinTemp )
						continue;
					// if we've already past the end of the scale, then we're done
					if ( dVal > _maxLinTemp + rangeTol )
						break;

					// convert the value to a pixel position
					pixVal = LocalTransform( dVal );

					// see if the tic marks will be drawn between the labels instead of at the labels
					// (this applies only to AxisType.Text
					if ( tic._isBetweenLabels && IsText )
					{
						// We need one extra tic in order to draw the tics between labels
						// so provide an exception here
						if ( i == 0 )
						{
							dVal2 = CalcMajorTicValue( baseVal, -0.5 );
							if ( dVal2 >= _minLinTemp )
							{
								pixVal2 = LocalTransform( dVal2 );
								grid.Draw( g, gridPen, pixVal2, topPix );
							}
						}

						dVal2 = CalcMajorTicValue( baseVal, (double)i + 0.5 );
						if ( dVal2 > _maxLinTemp )
							break;
						pixVal2 = LocalTransform( dVal2 );
					}
					else
						pixVal2 = pixVal;

					// draw the grid
					grid.Draw( g, gridPen, pixVal2, topPix );
				}
			}
		}

		internal void DrawLabel( Graphics g, GraphPane pane, int i, double dVal, float pixVal,
						float shift, float maxSpace, float scaledTic, float charHeight, float scaleFactor )
		{
			float textTop, textCenter;
			if ( _ownerAxis.MajorTic.IsOutside )
				textTop = scaledTic + charHeight * _labelGap;
			else
				textTop = charHeight * _labelGap;

			// draw the label
			//string tmpStr = MakeLabel( pane, i, dVal );
			string tmpStr = _ownerAxis.MakeLabelEventWorks( pane, i, dVal );

			float height;
			if ( this.IsLog && _isUseTenPower )
				height = _fontSpec.BoundingBoxTenPower( g, tmpStr, scaleFactor ).Height;
			else
				height = _fontSpec.BoundingBox( g, tmpStr, scaleFactor ).Height;

			if ( _align == AlignP.Center )
				textCenter = textTop + maxSpace / 2.0F;
			else if ( _align == AlignP.Outside )
				textCenter = textTop + maxSpace - height / 2.0F;
			else	// inside
				textCenter = textTop + height / 2.0F;

			if ( _isLabelsInside )
				textCenter = shift - textCenter;
			else
				textCenter = shift + textCenter;

			AlignV av = AlignV.Center;
			AlignH ah = AlignH.Center;

			if ( _ownerAxis is XAxis || _ownerAxis is X2Axis )
				ah = _alignH;
			else
				av = _alignH == AlignH.Left ? AlignV.Top : ( _alignH == AlignH.Right ? AlignV.Bottom : AlignV.Center );

			if ( this.IsLog && _isUseTenPower )
				_fontSpec.DrawTenPower( g, pane, tmpStr,
					pixVal, textCenter,
					ah, av,
					scaleFactor );
			else
				_fontSpec.Draw( g, pane, tmpStr,
					pixVal, textCenter,
					ah, av,
					scaleFactor );
		}

		/// <summary>
		/// Draw the scale, including the tic marks, value labels, and grid lines as
		/// required for this <see cref="Axis"/>.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="shiftPos">
		/// The number of pixels to shift to account for non-primary axis position (e.g.,
		/// the second, third, fourth, etc. <see cref="YAxis" /> or <see cref="Y2Axis" />.
		/// </param>
		internal void Draw( Graphics g, GraphPane pane, float scaleFactor, float shiftPos )
		{
			MajorGrid majorGrid = _ownerAxis._majorGrid;
			MajorTic majorTic = _ownerAxis._majorTic;
			MinorTic minorTic = _ownerAxis._minorTic;

			float rightPix,
					topPix;

			GetTopRightPix( pane, out topPix, out rightPix );

			// calculate the total number of major tics required
			int nTics = CalcNumTics();

			// get the first major tic value
			double baseVal = CalcBaseTic();

			using ( Pen pen = new Pen( _ownerAxis.Color,
						pane.ScaledPenWidth( majorTic._penWidth, scaleFactor ) ) )
			{
				// redraw the axis border
				if ( _ownerAxis.IsAxisSegmentVisible )
					g.DrawLine( pen, 0.0F, shiftPos, rightPix, shiftPos );

				// Draw a zero-value line if needed
				if ( majorGrid._isZeroLine && _min < 0.0 && _max > 0.0 )
				{
					float zeroPix = LocalTransform( 0.0 );
					g.DrawLine( pen, zeroPix, 0.0F, zeroPix, topPix );

				}
			}

			// draw the major tics and labels
			DrawLabels( g, pane, baseVal, nTics, topPix, shiftPos, scaleFactor );

            //			_ownerAxis.DrawMinorTics( g, pane, baseVal, shiftPos, scaleFactor, topPix );

			_ownerAxis.DrawTitle( g, pane, shiftPos, scaleFactor );
		}

		internal void GetTopRightPix( GraphPane pane, out float topPix, out float rightPix )
		{
			if ( _ownerAxis is XAxis || _ownerAxis is X2Axis )
			{
				rightPix = pane.Chart._rect.Width;
				topPix = -pane.Chart._rect.Height;
			}
			else
			{
				rightPix = pane.Chart._rect.Height;
				topPix = -pane.Chart._rect.Width;
			}

			// sanity check
			if ( _min >= _max )
				return;

			// if the step size is outrageous, then quit
			// (step size not used for log scales)
			if ( !IsLog )
			{
				if ( _majorStep <= 0 || _minorStep <= 0 )
					return;

				double tMajor = ( _max - _min ) / ( _majorStep * MajorUnitMultiplier );
				double tMinor = ( _max - _min ) / ( _minorStep * MinorUnitMultiplier );

				MinorTic minorTic = _ownerAxis._minorTic;

				if ( tMajor > 1000 ||
					( ( minorTic.IsOutside || minorTic.IsInside || minorTic.IsOpposite )
					&& tMinor > 5000 ) )
					return;
			}
		}

		/// <summary>
		/// Determine the width, in pixel units, of each bar cluster including
		/// the cluster gaps and bar gaps.
		/// </summary>
		/// <remarks>
		/// This method uses the <see cref="BarSettings.ClusterScaleWidth" /> for
		/// non-ordinal axes, or a cluster width of 1.0 for ordinal axes.
		/// </remarks>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object
		/// associated with this <see cref="Axis"/></param>
		/// <returns>The width of each bar cluster, in pixel units</returns>
		public float GetClusterWidth( GraphPane pane )
		{
			double basisVal = _min;
			return Math.Abs( Transform( basisVal +
					( IsAnyOrdinal ? 1.0 : pane._barSettings._clusterScaleWidth ) ) -
					Transform( basisVal ) );
		}

		/// <summary>
		/// Calculates the cluster width, in pixels, by transforming the specified
		/// clusterScaleWidth.
		/// </summary>
		/// <param name="clusterScaleWidth">The width in user scale units of each
		/// bar cluster</param>
		/// <returns>The equivalent pixel size of the bar cluster</returns>
		public float GetClusterWidth( double clusterScaleWidth )
		{
			double basisVal = _min;
			return Math.Abs( Transform( basisVal + clusterScaleWidth ) -
					Transform( basisVal ) );
		}

	#endregion

	#region Scale Picker Methods

		/// <summary>
		/// Select a reasonable scale given a range of data values.
		/// </summary>
		/// <remarks>
		/// The scale range is chosen
		/// based on increments of 1, 2, or 5 (because they are even divisors of 10).  This
		/// routine honors the <see cref="MinAuto"/>, <see cref="MaxAuto"/>,
		/// and <see cref="MajorStepAuto"/> autorange settings as well as the <see cref="IsLog"/>
		/// setting.  In the event that any of the autorange settings are false, the
		/// corresponding <see cref="Min"/>, <see cref="Max"/>, or <see cref="MajorStep"/>
		/// setting is explicitly honored, and the remaining autorange settings (if any) will
		/// be calculated to accomodate the non-autoranged values.  The basic defaults for
		/// scale selection are defined using <see cref="Default.ZeroLever"/>,
		/// <see cref="Default.TargetXSteps"/>, and <see cref="Default.TargetYSteps"/>
		/// from the <see cref="Default"/> default class.
		/// <para>On Exit:</para>
		/// <para><see cref="Min"/> is set to scale minimum (if <see cref="MinAuto"/> = true)</para>
		/// <para><see cref="Max"/> is set to scale maximum (if <see cref="MaxAuto"/> = true)</para>
		/// <para><see cref="MajorStep"/> is set to scale step size (if <see cref="MajorStepAuto"/> = true)</para>
		/// <para><see cref="MinorStep"/> is set to scale minor step size (if <see cref="MinorStepAuto"/> = true)</para>
		/// <para><see cref="Mag"/> is set to a magnitude multiplier according to the data</para>
		/// <para><see cref="Format"/> is set to the display format for the values (this controls the
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
		virtual public void PickScale( GraphPane pane, Graphics g, float scaleFactor )
		{
			double minVal = _rangeMin;
			double maxVal = _rangeMax;

			// Make sure that minVal and maxVal are legitimate values
			if ( Double.IsInfinity( minVal ) || Double.IsNaN( minVal ) || minVal == Double.MaxValue )
				minVal = 0.0;
			if ( Double.IsInfinity( maxVal ) || Double.IsNaN( maxVal ) || maxVal == Double.MaxValue )
				maxVal = 0.0;

			// if the scales are autoranged, use the actual data values for the range
			double range = maxVal - minVal;

			// "Grace" is applied to the numeric axis types only
			bool numType = !this.IsAnyOrdinal;

			// For autoranged values, assign the value.  If appropriate, adjust the value by the
			// "Grace" value.
			if ( _minAuto )
			{
				_min = minVal;
				// Do not let the grace value extend the axis below zero when all the values were positive
				if ( numType && ( _min < 0 || minVal - _minGrace * range >= 0.0 ) )
					_min = minVal - _minGrace * range;
			}
			if ( _maxAuto )
			{
				_max = maxVal;
				// Do not let the grace value extend the axis above zero when all the values were negative
				if ( numType && ( _max > 0 || maxVal + _maxGrace * range <= 0.0 ) )
					_max = maxVal + _maxGrace * range;
			}

			if ( _max == _min && _maxAuto && _minAuto )
			{
				if ( Math.Abs( _max ) > 1e-100 )
				{
					_max *= ( _min < 0 ? 0.95 : 1.05 );
					_min *= ( _min < 0 ? 1.05 : 0.95 );
				}
				else
				{
					_max = 1.0;
					_min = -1.0;
				}
			}

			if ( _max <= _min )
			{
				if ( _maxAuto )
					_max = _min + 1.0;
				else if ( _minAuto )
					_min = _max - 1.0;
			}

		}

		/// <summary>
		/// Calculate the maximum number of labels that will fit on this axis.
		/// </summary>
		/// <remarks>
		/// This method works for
		/// both X and Y direction axes, and it works for angled text (assuming that a bounding box
		/// is an appropriate measure).  Technically, labels at 45 degree angles could fit better than
		/// the return value of this method since the bounding boxes can overlap without the labels actually
		/// overlapping.
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
		public int CalcMaxLabels( Graphics g, GraphPane pane, float scaleFactor )
		{
			SizeF size = this.GetScaleMaxSpace( g, pane, scaleFactor, false );

			// The font angles are already set such that the Width is parallel to the appropriate (X or Y)
			// axis.  Therefore, we always use size.Width.
			// use the minimum of 1/4 the max Width or 1 character space
			//			double allowance = this.Scale.FontSpec.GetWidth( g, scaleFactor );
			//			if ( allowance > size.Width / 4 )
			//				allowance = size.Width / 4;


			float maxWidth = 1000;
			float temp = 1000;
			float costh = (float) Math.Abs( Math.Cos( _fontSpec.Angle * Math.PI / 180.0 ) );
			float sinth = (float) Math.Abs( Math.Sin( _fontSpec.Angle * Math.PI / 180.0 ) );

			if ( costh > 0.001 )
				maxWidth = size.Width / costh;
			if ( sinth > 0.001 )
				temp = size.Height / sinth;
			if ( temp < maxWidth )
				maxWidth = temp;


			//maxWidth = size.Width;
			/*
						if ( this is XAxis )
							// Add an extra character width to leave a minimum of 1 character space between labels
							maxWidth = size.Width + this.Scale.FontSpec.GetWidth( g, scaleFactor );
						else
							// For vertical spacing, we only need 1/2 character
							maxWidth = size.Width + this.Scale.FontSpec.GetWidth( g, scaleFactor ) / 2.0;
			*/
			if ( maxWidth <= 0 )
				maxWidth = 1;


			// Calculate the maximum number of labels
			double width;
			RectangleF chartRect = pane.Chart._rect;
			if ( _ownerAxis is XAxis || _ownerAxis is X2Axis )
				width = ( chartRect.Width == 0 ) ? pane.Rect.Width * 0.75 : chartRect.Width;
			else
				width = ( chartRect.Height == 0 ) ? pane.Rect.Height * 0.75 : chartRect.Height;

			int maxLabels = (int) ( width / maxWidth );
			if ( maxLabels <= 0 )
				maxLabels = 1;

			return maxLabels;
		}

		internal void SetScaleMag( double min, double max, double step )
		{
			// set the scale magnitude if required
			if ( _magAuto )
			{
				// Find the optimal scale display multiple
				double mag = -100;
				double mag2 = -100;

				if ( Math.Abs( _min ) > 1.0e-30 )
					mag = Math.Floor( Math.Log10( Math.Abs( _min ) ) );
				if ( Math.Abs( _max ) > 1.0e-30 )
					mag2 = Math.Floor( Math.Log10( Math.Abs( _max ) ) );

				mag = Math.Max( mag2, mag );

				// Do not use scale multiples for magnitudes below 4
				if ( mag == -100 || Math.Abs( mag ) <= 3 )
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
		/// Calculate a step size based on a data range.
		/// </summary>
		/// <remarks>
		/// This utility method
		/// will try to honor the <see cref="Default.TargetXSteps"/> and
		/// <see cref="Default.TargetYSteps"/> number of
		/// steps while using a rational increment (1, 2, or 5 -- which are
		/// even divisors of 10).  This method is used by <see cref="PickScale"/>.
		/// </remarks>
		/// <param name="range">The range of data in user scale units.  This can
		/// be a full range of the data for the major step size, or just the
		/// value of the major step size to calculate the minor step size</param>
		/// <param name="targetSteps">The desired "typical" number of steps
		/// to divide the range into</param>
		/// <returns>The calculated step size for the specified data range.</returns>
		protected static double CalcStepSize( double range, double targetSteps )
		{
			// Calculate an initial guess at step size
			double tempStep = range / targetSteps;

			// Get the magnitude of the step size
			double mag = Math.Floor( Math.Log10( tempStep ) );
			double magPow = Math.Pow( (double) 10.0, mag );

			// Calculate most significant digit of the new step size
			double magMsd = ( (int) ( tempStep / magPow + .5 ) );

			// promote the MSD to either 1, 2, or 5
			if ( magMsd > 5.0 )
				magMsd = 10.0;
			else if ( magMsd > 2.0 )
				magMsd = 5.0;
			else if ( magMsd > 1.0 )
				magMsd = 2.0;

			return magMsd * magPow;
		}

		/// <summary>
		/// Calculate a step size based on a data range, limited to a maximum number of steps.
		/// </summary>
		/// <remarks>
		/// This utility method
		/// will calculate a step size, of no more than maxSteps,
		/// using a rational increment (1, 2, or 5 -- which are
		/// even divisors of 10).  This method is used by <see cref="PickScale"/>.
		/// </remarks>
		/// <param name="range">The range of data in user scale units.  This can
		/// be a full range of the data for the major step size, or just the
		/// value of the major step size to calculate the minor step size</param>
		/// <param name="maxSteps">The maximum allowable number of steps
		/// to divide the range into</param>
		/// <returns>The calculated step size for the specified data range.</returns>
		protected double CalcBoundedStepSize( double range, double maxSteps )
		{
			// Calculate an initial guess at step size
			double tempStep = range / maxSteps;

			// Get the magnitude of the step size
			double mag = Math.Floor( Math.Log10( tempStep ) );
			double magPow = Math.Pow( (double) 10.0, mag );

			// Calculate most significant digit of the new step size
			double magMsd = Math.Ceiling( tempStep / magPow );

			// promote the MSD to either 1, 2, or 5
			if ( magMsd > 5.0 )
				magMsd = 10.0;
			else if ( magMsd > 2.0 )
				magMsd = 5.0;
			else if ( magMsd > 1.0 )
				magMsd = 2.0;

			return magMsd * magPow;
		}

		/// <summary>
		/// Internal routine to determine the ordinals of the first and last major axis label.
		/// </summary>
		/// <returns>
		/// This is the total number of major tics for this axis.
		/// </returns>
		virtual internal int CalcNumTics()
		{
			int nTics = 1;

			// default behavior is for a linear or ordinal scale
			nTics = (int) ( ( _max - _min ) / _majorStep + 0.01 ) + 1;

			if ( nTics < 1 )
				nTics = 1;
			else if ( nTics > 1000 )
				nTics = 1000;

			return nTics;
		}

		/// <summary>
		/// Calculate the modulus (remainder) in a safe manner so that divide
		/// by zero errors are avoided
		/// </summary>
		/// <param name="x">The divisor</param>
		/// <param name="y">The dividend</param>
		/// <returns>the value of the modulus, or zero for the divide-by-zero
		/// case</returns>
		protected double MyMod( double x, double y )
		{
			double temp;

			if ( y == 0 )
				return 0;

			temp = x / y;
			return y * ( temp - Math.Floor( temp ) );
		}

		/// <summary>
		/// Define suitable default ranges for an axis in the event that
		/// no data were available
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane"/> of interest</param>
		/// <param name="axis">The <see cref="Axis"/> for which to set the range</param>
		internal void SetRange( GraphPane pane, Axis axis )
		{
			if ( _rangeMin >= Double.MaxValue || _rangeMax <= Double.MinValue )
			{
				// If this is a Y axis, and the main Y axis is valid, use it for defaults
				if ( axis != pane.XAxis && axis != pane.X2Axis &&
					pane.YAxis.Scale._rangeMin < double.MaxValue && pane.YAxis.Scale._rangeMax > double.MinValue )
				{
					_rangeMin = pane.YAxis.Scale._rangeMin;
					_rangeMax = pane.YAxis.Scale._rangeMax;
				}
				// Otherwise, if this is a Y axis, and the main Y2 axis is valid, use it for defaults
				else if ( axis != pane.XAxis && axis != pane.X2Axis &&
					pane.Y2Axis.Scale._rangeMin < double.MaxValue && pane.Y2Axis.Scale._rangeMax > double.MinValue )
				{
					_rangeMin = pane.Y2Axis.Scale._rangeMin;
					_rangeMax = pane.Y2Axis.Scale._rangeMax;
				}
				// Otherwise, just use 0 and 1
				else
				{
					_rangeMin = 0;
					_rangeMax = 1;
				}

			}

			/*
				if ( yMinVal >= Double.MaxValue || yMaxVal <= Double.MinValue )
				{
					if ( y2MinVal < Double.MaxValue && y2MaxVal > Double.MinValue )
					{
						yMinVal = y2MinVal;
						yMaxVal = y2MaxVal;
					}
					else
					{
						yMinVal = 0;
						yMaxVal = 0.01;
					}
				}
			
				if ( y2MinVal >= Double.MaxValue || y2MaxVal <= Double.MinValue )
				{
					if ( yMinVal < Double.MaxValue && yMaxVal > Double.MinValue )
					{
						y2MinVal = yMinVal;
						y2MaxVal = yMaxVal;
					}
					else
					{
						y2MinVal = 0;
						y2MaxVal = 1;
					}
				}
				*/
		}

	#endregion

	#region Coordinate Transform Methods

		/// <summary>
		/// Transform the coordinate value from user coordinates (scale value)
		/// to graphics device coordinates (pixels).
		/// </summary>
		/// <remarks>This method takes into
		/// account the scale range (<see cref="Min"/> and <see cref="Max"/>),
		/// logarithmic state (<see cref="IsLog"/>), scale reverse state
		/// (<see cref="IsReverse"/>) and axis type (<see cref="XAxis"/>,
		/// <see cref="YAxis"/>, or <see cref="Y2Axis"/>).
		/// Note that the <see cref="Chart.Rect"/> must be valid, and
		/// <see cref="SetupScaleData"/> must be called for the
		/// current configuration before using this method (this is called everytime
		/// the graph is drawn (i.e., <see cref="GraphPane.Draw"/> is called).
		/// </remarks>
		/// <param name="x">The coordinate value, in user scale units, to
		/// be transformed</param>
		/// <returns>the coordinate value transformed to screen coordinates
		/// for use in calling the <see cref="Graphics"/> draw routines</returns>
		public float Transform( double x )
		{
			// Must take into account Log, and Reverse Axes
			double denom = ( _maxLinTemp - _minLinTemp );
			double ratio;
			if ( denom > 1e-100 )
				ratio = ( Linearize( x ) - _minLinTemp ) / denom;
			else
				ratio = 0;

			// _isReverse   axisType    Eqn
			//     T          XAxis     _maxPix - ...
			//     F          YAxis     _maxPix - ...
			//     F          Y2Axis    _maxPix - ...

			//     T          YAxis     _minPix + ...
			//     T          Y2Axis    _minPix + ...
			//     F          XAxis     _minPix + ...

			if ( _isReverse == ( _ownerAxis is XAxis || _ownerAxis is X2Axis ) )
				return (float) ( _maxPix - ( _maxPix - _minPix ) * ratio );
			else
				return (float) ( _minPix + ( _maxPix - _minPix ) * ratio );
		}

		/// <summary>
		/// Transform the coordinate value from user coordinates (scale value)
		/// to graphics device coordinates (pixels).
		/// </summary>
		/// <remarks>
		/// This method takes into
		/// account the scale range (<see cref="Min"/> and <see cref="Max"/>),
		/// logarithmic state (<see cref="IsLog"/>), scale reverse state
		/// (<see cref="IsReverse"/>) and axis type (<see cref="XAxis"/>,
		/// <see cref="YAxis"/>, or <see cref="Y2Axis"/>).
		/// Note that the <see cref="Chart.Rect"/> must be valid, and
		/// <see cref="SetupScaleData"/> must be called for the
		/// current configuration before using this method (this is called everytime
		/// the graph is drawn (i.e., <see cref="GraphPane.Draw"/> is called).
		/// </remarks>
		/// <param name="isOverrideOrdinal">true to force the axis to honor the data
		/// value, rather than replacing it with the ordinal value</param>
		/// <param name="i">The ordinal value of this point, just in case
		/// this is an <see cref="AxisType.Ordinal"/> axis</param>
		/// <param name="x">The coordinate value, in user scale units, to
		/// be transformed</param>
		/// <returns>the coordinate value transformed to screen coordinates
		/// for use in calling the <see cref="Graphics"/> draw routines</returns>
		public float Transform( bool isOverrideOrdinal, int i, double x )
		{
			// ordinal types ignore the X value, and just use the ordinal position
			if ( this.IsAnyOrdinal && i >= 0 && !isOverrideOrdinal )
				x = (double) i + 1.0;
			return Transform( x );

		}

		/// <summary>
		/// Reverse transform the user coordinates (scale value)
		/// given a graphics device coordinate (pixels).
		/// </summary>
		/// <remarks>
		/// This method takes into
		/// account the scale range (<see cref="Min"/> and <see cref="Max"/>),
		/// logarithmic state (<see cref="IsLog"/>), scale reverse state
		/// (<see cref="IsReverse"/>) and axis type (<see cref="XAxis"/>,
		/// <see cref="YAxis"/>, or <see cref="Y2Axis"/>).
		/// Note that the <see cref="Chart.Rect"/> must be valid, and
		/// <see cref="SetupScaleData"/> must be called for the
		/// current configuration before using this method (this is called everytime
		/// the graph is drawn (i.e., <see cref="GraphPane.Draw"/> is called).
		/// </remarks>
		/// <param name="pixVal">The screen pixel value, in graphics device coordinates to
		/// be transformed</param>
		/// <returns>The user scale value that corresponds to the screen pixel location</returns>
		public double ReverseTransform( float pixVal )
		{
			double val;

			// see if the sign of the equation needs to be reversed
			if ( ( _isReverse ) == ( _ownerAxis is XAxis || _ownerAxis is X2Axis ) )
				val = (double) ( pixVal - _maxPix )
						/ (double) ( _minPix - _maxPix )
						* ( _maxLinTemp - _minLinTemp ) + _minLinTemp;
			else
				val = (double) ( pixVal - _minPix )
						/ (double) ( _maxPix - _minPix )
						* ( _maxLinTemp - _minLinTemp ) + _minLinTemp;

			return DeLinearize( val );
		}


		/// <summary>
		/// Transform the coordinate value from user coordinates (scale value)
		/// to graphics device coordinates (pixels).
		/// </summary>
		/// <remarks>Assumes that the origin
		/// has been set to the "left" of this axis, facing from the label side.
		/// Note that the left side corresponds to the scale minimum for the X and
		/// Y2 axes, but it is the scale maximum for the Y axis.
		/// This method takes into
		/// account the scale range (<see cref="Min"/> and <see cref="Max"/>),
		/// logarithmic state (<see cref="IsLog"/>), scale reverse state
		/// (<see cref="IsReverse"/>) and axis type (<see cref="XAxis"/>,
		/// <see cref="YAxis"/>, or <see cref="Y2Axis"/>).  Note that
		/// the <see cref="Chart.Rect"/> must be valid, and
		/// <see cref="SetupScaleData"/> must be called for the
		/// current configuration before using this method.
		/// </remarks>
		/// <param name="x">The coordinate value, in linearized user scale units, to
		/// be transformed</param>
		/// <returns>the coordinate value transformed to screen coordinates
		/// for use in calling the <see cref="Draw"/> method</returns>
		public float LocalTransform( double x )
		{
			// Must take into account Log, and Reverse Axes
			double ratio;
			float rv;

			// Coordinate values for log scales are already in exponent form, so no need
			// to take the log here
			ratio = ( x - _minLinTemp ) /
						( _maxLinTemp - _minLinTemp );

			if ( _isReverse == ( _ownerAxis is YAxis || _ownerAxis is X2Axis ) )
				rv = (float) ( ( _maxPix - _minPix ) * ratio );
			else
				rv = (float)( ( _maxPix - _minPix ) * ( 1.0F - ratio ) );

			return rv;
		}

		/// <summary>
		/// Calculate a base 10 logarithm in a safe manner to avoid math exceptions
		/// </summary>
		/// <param name="x">The value for which the logarithm is to be calculated</param>
		/// <returns>The value of the logarithm, or 0 if the <paramref name="x"/>
		/// argument was negative or zero</returns>
		public static double SafeLog( double x )
		{
			if ( x > 1.0e-20 )
				return Math.Log10( x );
			else
				return 0.0;
		}

		///<summary>
		///Calculate an exponential in a safe manner to avoid math exceptions
		///</summary> 
		/// <param name="x">The value for which the exponential is to be calculated</param>
		/// <param name="exponent">The exponent value to use for calculating the exponential.</param>
		public static double SafeExp( double x, double exponent )
		{
			if ( x > 1.0e-20 )
				return Math.Pow( x, exponent );
			else
				return 0.0;
		}

	#endregion


	}
}
