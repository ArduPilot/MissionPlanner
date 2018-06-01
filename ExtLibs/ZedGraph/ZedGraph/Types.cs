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
using System.Drawing;
using System.Drawing.Drawing2D;


namespace ZedGraph
{	
	/// <summary>
	/// Enumeration type for the various axis types that are available
	/// </summary>
	/// <seealso cref="ZedGraph.Axis.Type"/>
	public enum AxisType
	{
		/// <summary> An ordinary, cartesian axis </summary>
		Linear,
		/// <summary> A base 10 log axis </summary>
		Log,
		/// <summary> A cartesian axis with calendar dates or times </summary>
		Date,
		/// <summary> An ordinal axis with user-defined text labels.  An ordinal axis means that
		/// all data points are evenly spaced at integral values, and the actual coordinate values
		/// for points corresponding to that axis are ignored.  That is, if the X axis is an
		/// ordinal type, then all X values associated with the curves are ignored.</summary>
		/// <seealso cref="AxisType.Ordinal"/>
		/// <seealso cref="Scale.IsText"/>
		/// <seealso cref="ZedGraph.Scale.Default.MaxTextLabels"/>
		Text,
		/// <summary> An ordinal axis with regular numeric labels.  An ordinal axis means that
		/// all data points are evenly spaced at integral values, and the actual coordinate values
		/// for points corresponding to that axis are ignored.  That is, if the X axis is an
		/// ordinal type, then all X values associated with the curves are ignored. </summary>
		/// <seealso cref="AxisType.Text"/>
		/// <seealso cref="Scale.IsOrdinal"/>
		Ordinal,
		/// <summary> An ordinal axis that will have labels formatted with ordinal values corresponding
		/// to the number of values in each <see cref="CurveItem" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="CurveItem" /> data points will be evenly-spaced at ordinal locations, and the
		/// actual data values are ignored. </remarks>
		/// <seealso cref="AxisType.Text"/>
		/// <seealso cref="Scale.IsOrdinal"/>
		DateAsOrdinal,
		/// <summary> An ordinal axis that will have labels formatted with values from the actual data
		/// values of the first <see cref="CurveItem" /> in the <see cref="CurveList" />.
		/// </summary>
		/// <remarks>
		/// Although the tics are labeled with real data values, the actual points will be
		/// evenly-spaced in spite of the data values.  For example, if the X values of the first curve
		/// are 1, 5, and 100, then the tic labels will show 1, 5, and 100, but they will be equal
		/// distance from each other. </remarks>
		/// <seealso cref="AxisType.Text"/>
		/// <seealso cref="Scale.IsOrdinal"/>
		LinearAsOrdinal,
		/// <summary> An exponential axis </summary>
		Exponent
	}

	/// <summary>
	/// Enumeration type for the various types of fills that can be used with <see cref="Bar"/>
	/// charts.
	/// </summary>
	public enum FillType
	{
		/// <summary> No fill </summary>
		None,
		/// <summary> A solid fill using <see cref="System.Drawing.SolidBrush"/> </summary>
		Solid,
		/// <summary> A custom fill using either <see cref="LinearGradientBrush"/> or
		/// <see cref="TextureBrush"/></summary>
		Brush,
		/// <summary>
		/// Fill with a single solid color based on the X value of the data.</summary>
		/// <remarks>The X value is
		/// used to determine the color value based on a gradient brush, and using a data range
		/// of <see cref="Fill.RangeMin"/> and <see cref="Fill.RangeMax"/>.  You can create a multicolor
		/// range by initializing the <see cref="Fill"/> class with your own custom
		/// <see cref="Brush"/> object based on a <see cref="ColorBlend"/>.  In cases where a
		/// data value makes no sense (<see cref="PaneBase.Fill"/>, <see cref="Legend.Fill"/>,
		/// etc.), a default value of 50% of the range is assumed.  The default range is 0 to 1.
		/// </remarks>
		/// <seealso cref="Fill.RangeMin"/>
		/// <seealso cref="Fill.RangeMax"/>
		/// <seealso cref="Fill.RangeDefault"/>
		GradientByX,
		/// <summary>
		/// Fill with a single solid color based on the Z value of the data.</summary>
		/// <remarks>The Z value is
		/// used to determine the color value based on a gradient brush, and using a data range
		/// of <see cref="Fill.RangeMin"/> and <see cref="Fill.RangeMax"/>.  You can create a multicolor
		/// range by initializing the <see cref="Fill"/> class with your own custom
		/// <see cref="Brush"/> object based on a <see cref="ColorBlend"/>.  In cases where a
		/// data value makes no sense (<see cref="PaneBase.Fill"/>, <see cref="Legend.Fill"/>,
		/// etc.), a default value of 50% of the range is assumed.  The default range is 0 to 1.
		/// </remarks>
		/// <seealso cref="Fill.RangeMin"/>
		/// <seealso cref="Fill.RangeMax"/>
		/// <seealso cref="Fill.RangeDefault"/>
		GradientByY,
		/// <summary>
		/// Fill with a single solid color based on the Z value of the data.</summary>
		/// <remarks>The Z value is
		/// used to determine the color value based on a gradient brush, and using a data range
		/// of <see cref="Fill.RangeMin"/> and <see cref="Fill.RangeMax"/>.  You can create a multicolor
		/// range by initializing the <see cref="Fill"/> class with your own custom
		/// <see cref="Brush"/> object based on a <see cref="ColorBlend"/>.  In cases where a
		/// data value makes no sense (<see cref="PaneBase.Fill"/>, <see cref="Legend.Fill"/>,
		/// etc.), a default value of 50% of the range is assumed.  The default range is 0 to 1.
		/// </remarks>
		/// <seealso cref="Fill.RangeMin"/>
		/// <seealso cref="Fill.RangeMax"/>
		/// <seealso cref="Fill.RangeDefault"/>
		GradientByZ,
		/// <summary>
		/// Fill with a single solid color based on the "ColorValue" property of the data.</summary>
		/// <remarks>The "ColorValue" property is
		/// used to determine the color value based on a gradient brush, and using a data range
		/// of <see cref="Fill.RangeMin"/> and <see cref="Fill.RangeMax"/>.  You can create a multicolor
		/// range by initializing the <see cref="Fill"/> class with your own custom
		/// <see cref="Brush"/> object based on a <see cref="ColorBlend"/>.  In cases where a
		/// data value makes no sense (<see cref="PaneBase.Fill"/>, <see cref="Legend.Fill"/>,
		/// etc.), a default value of 50% of the range is assumed.  The default range is 0 to 1.
		/// </remarks>
		/// <seealso cref="Fill.RangeMin"/>
		/// <seealso cref="Fill.RangeMax"/>
		/// <seealso cref="Fill.RangeDefault"/>
		GradientByColorValue

	}

	/// <summary>
	/// Enumeration type for the various axis date and time unit types that are available
	/// </summary>
	public enum DateUnit
	{
		/// <summary> Yearly units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Year,
		/// <summary> Monthly units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Month,
		/// <summary> Daily units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Day,
		/// <summary> Hourly units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Hour,
		/// <summary> Minute units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Minute,
		/// <summary> Second units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Second,
		/// <summary> Millisecond units <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		Millisecond
	}

	/// <summary>
	/// Enumeration type for the various symbol shapes that are available
	/// </summary>
	/// <seealso cref="ZedGraph.Symbol.Fill"/>
	public enum SymbolType 
	{
		/// <summary> Square-shaped <see cref="ZedGraph.Symbol"/> </summary>
		Square,
		/// <summary> Rhombus-shaped <see cref="ZedGraph.Symbol"/> </summary>
		Diamond,
		/// <summary> Equilateral triangle <see cref="ZedGraph.Symbol"/> </summary>
		Triangle,
		/// <summary> Uniform circle <see cref="ZedGraph.Symbol"/> </summary>
		Circle,
		/// <summary> "X" shaped <see cref="ZedGraph.Symbol"/>.  This symbol cannot
		/// be filled since it has no outline. </summary>
		XCross,
		/// <summary> "+" shaped <see cref="ZedGraph.Symbol"/>.  This symbol cannot
		/// be filled since it has no outline. </summary>
		Plus,
		/// <summary> Asterisk-shaped <see cref="ZedGraph.Symbol"/>.  This symbol
		/// cannot be filled since it has no outline. </summary>
		Star,
		/// <summary> Unilateral triangle <see cref="ZedGraph.Symbol"/>, pointing
		/// down. </summary>
		TriangleDown,
		/// <summary>
		/// Horizontal dash <see cref="ZedGraph.Symbol"/>.  This symbol cannot be
		/// filled since it has no outline.
		/// </summary>
		HDash,
		/// <summary>
		/// Vertical dash <see cref="ZedGraph.Symbol"/>.  This symbol cannot be
		/// filled since it has no outline.
		/// </summary>
		VDash,
		/// <summary> A symbol defined by the <see cref="Symbol.UserSymbol"/> propery.
		/// If no symbol is defined, the <see cref="Symbol.Default.Type"/>. symbol will
		/// be used.
		/// </summary>
		UserDefined,
		/// <summary> A Default symbol type (the symbol type will be obtained
		/// from <see cref="Symbol.Default.Type"/>. </summary>
		Default,
		/// <summary> No symbol is shown (this is equivalent to using
		/// <see cref="Symbol.IsVisible"/> = false.</summary>
		None
	}

	/// <summary>
	/// Enumeration type that defines the possible legend locations
	/// </summary>
	/// <seealso cref="Legend.Position"/>
	public enum LegendPos
	{
		/// <summary>
		/// Locate the <see cref="Legend"/> above the <see cref="Chart.Rect"/>
		/// </summary>
		Top,
		/// <summary>
		/// Locate the <see cref="Legend"/> on the left side of the <see cref="Chart.Rect"/>
		/// </summary>
		Left,
		/// <summary>
		/// Locate the <see cref="Legend"/> on the right side of the <see cref="Chart.Rect"/>
		/// </summary>
		Right,
		/// <summary>
		/// Locate the <see cref="Legend"/> below the <see cref="Chart.Rect"/>
		/// </summary>
		Bottom,
		/// <summary>
		/// Locate the <see cref="Legend"/> inside the <see cref="Chart.Rect"/> in the
		/// top-left corner.  
		/// </summary>
		InsideTopLeft,
		/// <summary>
		/// Locate the <see cref="Legend"/> inside the <see cref="Chart.Rect"/> in the
		/// top-right corner. 
		/// </summary>
		InsideTopRight,
		/// <summary>
		/// Locate the <see cref="Legend"/> inside the <see cref="Chart.Rect"/> in the
		/// bottom-left corner.
		/// </summary>
		InsideBotLeft,
		/// <summary>
		/// Locate the <see cref="Legend"/> inside the <see cref="Chart.Rect"/> in the
		/// bottom-right corner. 
		/// </summary>
		InsideBotRight,
		/// <summary>
		/// Locate the <see cref="Legend"/> as a floating object above the graph at the
		/// location specified by <see cref="Legend.Location"/>.
		/// </summary>
		Float,
		/// <summary>
		/// Locate the <see cref="Legend"/> centered above the <see cref="Chart.Rect"/>
		/// </summary>
		TopCenter,
		/// <summary>
		/// Locate the <see cref="Legend"/> centered below the <see cref="Chart.Rect"/>
		/// </summary>
		BottomCenter,
		/// <summary>
		/// Locate the <see cref="Legend"/> above the <see cref="Chart.Rect"/>, but flush
		/// against the left margin of the <see cref="PaneBase.Rect" />.
		/// </summary>
		TopFlushLeft,
		/// <summary>
		/// Locate the <see cref="Legend"/> below the <see cref="Chart.Rect"/>, but flush
		/// against the left margin of the <see cref="PaneBase.Rect" />.
		/// </summary>
		BottomFlushLeft

	}

	/// <summary>
	/// Enumeration type for the different horizontal text alignment options
	/// </summary>
	/// <seealso cref="FontSpec"/>
	public enum AlignH
	{
		/// <summary>
		/// Position the text so that its left edge is aligned with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Left,
		/// <summary>
		/// Position the text so that its center is aligned (horizontally) with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Center,
		/// <summary>
		/// Position the text so that its right edge is aligned with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Right
	}
	
	/// <summary>
	/// Enumeration type for the different proximal alignment options
	/// </summary>
	/// <seealso cref="FontSpec"/>
	/// <seealso cref="Scale.Align"/>
	public enum AlignP
	{
		/// <summary>
		/// Position the text so that its "inside" edge (the edge that is
		/// nearest to the alignment reference point or object) is aligned.
		/// Used by the <see cref="Scale.Align"/> method to align text
		/// to the axis.
		/// </summary>
		Inside,
		/// <summary>
		/// Position the text so that its center is aligned with the
		/// reference object or point.
		/// Used by the <see cref="Scale.Align"/> method to align text
		/// to the axis.
		/// </summary>
		Center,
		/// <summary>
		/// Position the text so that its right edge (the edge that is
		/// farthest from the alignment reference point or object) is aligned.
		/// Used by the <see cref="Scale.Align"/> method to align text
		/// to the axis.
		/// </summary>
		Outside
	}
	
	/// <summary>
	/// Enumeration type for the different vertical text alignment options
	/// </summary>
	/// specified X,Y location.  Used by the
	/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
	public enum AlignV
	{
		/// <summary>
		/// Position the text so that its top edge is aligned with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Top,
		/// <summary>
		/// Position the text so that its center is aligned (vertically) with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Center,
		/// <summary>
		/// Position the text so that its bottom edge is aligned with the
		/// specified X,Y location.  Used by the
		/// <see cref="FontSpec.Draw(Graphics,PaneBase,string,float,float,AlignH,AlignV,float)"/> method.
		/// </summary>
		Bottom
	}

	/// <summary>
	/// Enumeration type for the user-defined coordinate types available.
	/// These coordinate types are used the <see cref="ArrowObj"/> objects
	/// and <see cref="TextObj"/> objects only.
	/// </summary>
	/// <seealso cref="ZedGraph.Location.CoordinateFrame"/>
	public enum CoordType
	{
		/// <summary>
		/// Coordinates are specified as a fraction of the
		/// <see cref="Chart.Rect"/>.  That is, for the X coordinate, 0.0
		/// is at the left edge of the ChartRect and 1.0
		/// is at the right edge of the ChartRect. A value less
		/// than zero is left of the ChartRect and a value
		/// greater than 1.0 is right of the ChartRect.  For the Y coordinate, 0.0
		/// is the top and 1.0 is the bottom.
		/// </summary>
		ChartFraction,
		/// <summary>
		/// Coordinates are specified as a fraction of the
		/// <see cref="PaneBase.Rect"/>.  That is, for the X coordinate, 0.0
		/// is at the left edge of the Rect and 1.0
		/// is at the right edge of the Rect. A value less
		/// than zero is left of the Rect and a value
		/// greater than 1.0 is right of the Rect.  For the Y coordinate, 0.0
		/// is the top and 1.0 is the bottom.  Note that
		/// any value less than zero or greater than 1.0 will be outside
		/// the Rect, and therefore clipped.
		/// </summary>
		PaneFraction,
		/// <summary>
		/// Coordinates are specified according to the user axis scales
		/// for the <see cref="GraphPane.XAxis"/> and <see cref="GraphPane.YAxis"/>.
		/// </summary>
		AxisXYScale,
		/// <summary>
		/// Coordinates are specified according to the user axis scales
		/// for the <see cref="GraphPane.XAxis"/> and <see cref="GraphPane.Y2Axis"/>.
		/// </summary>
		AxisXY2Scale,
		/// <summary>
		/// The X coordinate is specified as a fraction of the <see cref="Chart.Rect"/>,
		/// and the Y coordinate is specified as a fraction of the <see cref="PaneBase.Rect" />.
		/// </summary>
		/// <remarks>
		/// For the X coordinate, 0.0
		/// is at the left edge of the ChartRect and 1.0
		/// is at the right edge of the ChartRect. A value less
		/// than zero is left of the ChartRect and a value
		/// greater than 1.0 is right of the ChartRect.  For the Y coordinate, a value of zero is at
		/// the left side of the pane, and a value of 1.0 is at the right side of the pane.
		/// </remarks>
		XChartFractionYPaneFraction,
		/// <summary>
		/// The X coordinate is specified as a fraction of the <see cref="PaneBase.Rect"/>,
		/// and the Y coordinate is specified as a fraction of the <see cref="Chart.Rect" />.
		/// </summary>
		/// <remarks>
		/// For the X coordinate, a value of zero is at
		/// the left side of the pane, and a value of 1.0 is at the right side of the pane.
		/// For the Y coordinate, 0.0
		/// is at the top edge of the ChartRect and 1.0
		/// is at the bottom edge of the ChartRect. A value less
		/// than zero is above the ChartRect and a value
		/// greater than 1.0 is below the ChartRect.
		/// </remarks>
		XPaneFractionYChartFraction,
		/// <summary>
		/// The X coordinate is specified as an X Scale value, and the Y coordinate
		/// is specified as a fraction of the <see cref="Chart.Rect"/>.
		/// </summary>
		/// <remarks>
		/// For the X coordinate, the value just corresponds to the values of the X scale.
		/// Values outside the scale range will be
		/// outside the <see cref="Chart.Rect" />.  For the Y coordinate, 0.0
		/// is at the top edge of the ChartRect and 1.0
		/// is at the bottom edge of the ChartRect. A value less
		/// than zero is above the ChartRect and a value
		/// greater than 1.0 is below the ChartRect.
		/// </remarks>
		XScaleYChartFraction,
		/// <summary>
		/// The X coordinate is specified as a fraction of the
		/// <see cref="Chart.Rect"/> and the Y coordinate is specified as
		/// a Y scale value.
		/// </summary>
		/// <remarks>
		/// For the X coordinate, 0.0
		/// is at the left edge of the ChartRect and 1.0
		/// is at the right edge of the ChartRect. A value less
		/// than zero is left of the ChartRect and a value
		/// greater than 1.0 is right of the ChartRect.  For the Y coordinate, the value just
		/// corresponds to the values of the Y scale.  Values outside the scale range will be
		/// outside the <see cref="Chart.Rect" />.
		/// </remarks>
		XChartFractionYScale,
		/// <summary>
		/// The X coordinate is specified as a fraction of the
		/// <see cref="Chart.Rect"/> and the Y coordinate is specified as
		/// a Y2 scale value.
		/// </summary>
		/// <remarks>
		/// For the X coordinate, 0.0
		/// is at the left edge of the ChartRect and 1.0
		/// is at the right edge of the ChartRect. A value less
		/// than zero is left of the ChartRect and a value
		/// greater than 1.0 is right of the ChartRect.  For the Y coordinate, the value just
		/// corresponds to the values of the Y2 scale.  Values outside the scale range will be
		/// outside the <see cref="Chart.Rect" />.
		/// </remarks>
		XChartFractionY2Scale

	}
	
	/// <summary>
	/// Enumeration type that defines how a curve is drawn.  Curves can be drawn
	/// as ordinary lines by connecting the points directly, or in a stair-step
	/// fashion as a series of discrete, constant values.  In a stair step plot,
	/// all lines segments are either horizontal or vertical.  In a non-step (line)
	/// plot, the lines can be any angle.
	/// </summary>
	/// <seealso cref="Line.StepType"/>
	public enum StepType
	{
		/// <summary>
		/// Draw the <see cref="CurveItem"/> as a stair-step in which each
		/// point defines the
		/// beginning (left side) of a new stair.  This implies the points are
		/// defined at the beginning of an "event."
		/// </summary>
		ForwardStep,
		/// <summary>
		/// Draw the <see cref="CurveItem"/> as a stair-step in which each
		/// point defines the end (right side) of a new stair.  This implies
		/// the points are defined at the end of an "event."
		/// </summary>
		RearwardStep,
		/// <summary>
		/// Draw the <see cref="CurveItem"/> as an ordinary line, in which the
		/// points are connected directly by line segments.
		/// </summary>
		NonStep,
		/// <summary>
		/// Draw the <see cref="CurveItem"/> as a segment in which each point defines the
		/// beginning (left side) of a new "stair."  This implies the points are defined
		/// at the beginning of an "event."  Note that ForwardSegment is different
		/// from ForwardStep in that it does not draw the vertical portion of the step.
		/// </summary>
		ForwardSegment,
		/// <summary>
		/// Draw the <see cref="CurveItem"/> as a segment in which each point defines the
		/// end (right side) of a new "stair."  This implies the points are defined
		/// at the end of an "event."  Note that RearwardSegment is different
		/// from RearwardStep in that it does not draw the vertical portion of the step.
		/// </summary>
		RearwardSegment
	}
	
	/// <summary>
	/// Enumeration type that defines the base axis from which <see cref="Bar"/> graphs
	/// are displayed. The bars can be drawn on any of the four axes (<see cref="XAxis"/>,
	/// <see cref="X2Axis"/>, <see cref="YAxis"/>, and <see cref="Y2Axis"/>).
	/// </summary>
	/// <seealso cref="BarSettings.Base"/>
	public enum BarBase
	{
		/// <summary>
		/// Draw the <see cref="Bar"/> chart based from the <see cref="XAxis"/>.
		/// </summary>
		X,
		/// <summary>
		/// Draw the <see cref="Bar"/> chart based from the <see cref="X2Axis"/>.
		/// </summary>
		X2,
		/// <summary>
		/// Draw the <see cref="Bar"/> chart based from the <see cref="YAxis"/>.
		/// </summary>
		Y,
		/// <summary>
		/// Draw the <see cref="Bar"/> chart based from the <see cref="Y2Axis"/>.
		/// </summary>
		Y2
	}
	
	/// <summary>
	/// Enumeration type that defines the available types of <see cref="LineItem"/> graphs.
	/// </summary>
	/// <seealso cref="GraphPane.LineType"/>
	public enum LineType
	{
		/// <summary>
		/// Draw the lines as normal.  Any fill area goes from each line down to the X Axis.
		/// </summary>
		Normal,
		/// <summary>
		/// Draw the lines stacked on top of each other, accumulating values to a total value.
		/// </summary>
		Stack
	}
	
	/// <summary>
	/// Enumeration type that defines the available types of <see cref="BarItem"/> graphs.
	/// </summary>
	/// <seealso cref="BarSettings.Type"/>
	public enum BarType
	{
		/// <summary>
		/// Draw each <see cref="BarItem"/> side by side in clusters.
		/// </summary>
		Cluster,
		/// <summary>
		/// Draw the <see cref="BarItem"/> bars one on top of the other.  The bars will
		/// be drawn such that the last bar in the <see cref="CurveList"/> will be behind
		/// all other bars.  Note that the bar values are not summed up for the overlay
		/// mode.  The data values must be summed before being passed
		/// to <see cref="GraphPane.AddBar(string,IPointList,Color)"/>.
		/// For example, if the first bar of
		/// the first <see cref="BarItem"/> has a value of 100, and the first bar of
		/// the second <see cref="BarItem"/> has a value of 120, then that bar will
		/// appear to be 20 units on top of the first bar.
		/// </summary>
		Overlay,
		/// <summary>
		/// Draw the <see cref="BarItem"/> bars one on top of the other.  The bars will
		/// be drawn such that the bars are sorted according to the maximum value, with
		/// the tallest bar at each point at the back and the shortest bar at the front.
		/// This is similar to the <see cref="Overlay"/> mode, but the bars are sorted at
		/// each base value.
		/// The data values must be summed before being passed
		/// to <see cref="GraphPane.AddBar(string,IPointList,Color)"/>.  For example, if the first bar of
		/// the first <see cref="BarItem"/> has a value of 100, and the first bar of
		/// the second <see cref="BarItem"/> has a value of 120, then that bar will
		/// appear to be 20 units on top of the first bar.
		/// </summary>
		SortedOverlay,
		/// <summary>
		/// Draw the <see cref="BarItem"/> bars in an additive format so that they stack on
		/// top of one another.  The value of the last bar drawn will be the sum of the values
		/// of all prior bars.
		/// </summary>
		Stack,
		/// <summary>
		/// Draw the <see cref="BarItem"/> bars in a format whereby the height of each
		/// represents the percentage of the total each one represents.  Negative values
		///are displayed below the zero line as percentages of the absolute total of all values. 
		/// </summary>
		PercentStack 
	}
	
	/// <summary>
	/// Enumeration type that defines which set of data points - X or Y - is used  
	/// <seealso cref="System.Collections.ArrayList.Sort()"/> to perform the sort.
	/// </summary>
	public enum SortType
	{
	   /// <summary>
	   /// Use the Y values to sort the list.
	   /// </summary>
	   YValues,
	   /// <summary>
	   /// Use the X values to sort the list.
	   /// </summary>
	   XValues
	};

	/// <summary>
	/// Enumeration that specifies a Z-Order position for <see cref="GraphObj"/>
	/// objects.
	/// </summary>
	/// <remarks>This enumeration allows you to set the layering of various graph
	/// features.  Except for the <see cref="GraphObj"/> objects, other feature types
	/// all have a fixed depth as follows (front to back):
	/// <list>
	/// <see cref="Legend"/> objects
	/// The border around <see cref="Chart.Rect"/>
	/// <see cref="CurveItem"/> objects
	/// The <see cref="Axis"/> features
	/// The background fill of the <see cref="Chart.Rect"/>
	/// The pane <see cref="PaneBase.Title"/>
	/// The background fill of the <see cref="PaneBase.Rect"/>
	/// </list>
	/// You cannot place anything behind the <see cref="PaneBase.Rect"/>
	/// background fill, but <see cref="GraphObj.ZOrder"/> allows you to
	/// explicitly control the depth of <see cref="GraphObj"/> objects
	/// between all other object types.  For items of equal <see cref="ZOrder"/>,
	/// such as multiple <see cref="CurveItem"/>'s or <see cref="GraphObj"/>'s
	/// having the same <see cref="ZOrder"/> value, the relative depth is
	/// controlled by the ordinal position in the list (either
	/// <see cref="CurveList"/> or <see cref="GraphObjList"/>).
	/// <see cref="GraphObj"/> objects
	/// can be placed in the <see cref="GraphObjList"/> of either a
	/// <see cref="GraphPane"/> or a <see cref="MasterPane"/>.  For a
	/// <see cref="GraphPane"/>-based <see cref="GraphObj"/>, all <see cref="ZOrder"/>
	/// values are applicable.  For a <see cref="MasterPane"/>-based
	/// <see cref="GraphObj"/>, any <see cref="ZOrder"/> value can be used, but there
	/// are really only three depths:
	/// <list><see cref="ZOrder.H_BehindAll"/> will place the item behind the pane title,
	/// <see cref="ZOrder.A_InFront"/> will place on top of all other graph features,
	/// any other value places the object above the pane title, but behind the <see cref="GraphPane"/>'s.
	/// </list>
	/// </remarks>
	public enum ZOrder
	{
	   /// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be behind all other
	   /// objects (including the <see cref="PaneBase"/> <see cref="PaneBase.Title"/>).
	   /// </summary>
	   H_BehindAll,
	   /// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be behind the
	   /// <see cref="Chart.Rect"/> background <see cref="Fill"/>
	   /// (see <see cref="Chart.Fill"/>).
	   /// </summary>
	   G_BehindChartFill,
		/// <summary>
		/// Specifies that the <see cref="GraphObj"/> will be behind the grid lines.
		/// </summary>
		F_BehindGrid,
		/// <summary>
		/// Specifies that the <see cref="GraphObj"/> will be behind the
		/// <see cref="CurveItem"/> objects.
		/// </summary>
		E_BehindCurves,
		/// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be behind the
	   /// <see cref="Axis"/> objects.
	   /// </summary>
	   D_BehindAxis,
	   /// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be behind the
	   /// <see cref="Chart"/> border.
	   /// </summary>
	   C_BehindChartBorder,
	   /// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be behind the
	   /// <see cref="Legend"/> object.
	   /// </summary>
	   B_BehindLegend,
	   /// <summary>
	   /// Specifies that the <see cref="GraphObj"/> will be in front of
	   /// all other objects, except for the other <see cref="GraphObj"/>
	   /// objects that have the same <see cref="ZOrder"/> and are before
	   /// this object in the <see cref="GraphObjList"/>.
	   /// </summary>
	   A_InFront
	}

	/// <summary>
	/// Enumeration that determines the type of label that is displayed for each pie slice
	/// (see <see cref="PieItem.LabelType"/>).
	/// </summary>
	public enum PieLabelType
	{
		/// <summary>
		/// Displays <see cref="CurveItem.Label"/> and <see cref="PieItem.Value"/> for
		/// a slice in a Pie Chart.
		/// </summary>
		Name_Value,

		/// <summary>
		/// Displays <see cref="CurveItem.Label"/> and <see cref="PieItem.Value"/> (as % of total) for
		/// a slice in a Pie Chart.
		/// </summary>
		Name_Percent,		

		/// <summary>
		/// Displays a <see cref="CurveItem.Label"/> containing the <see cref="PieItem.Value"/> both  
		/// as an absolute number and as percentage of the total.
		/// </summary>
		Name_Value_Percent,		

		/// <summary>
		/// Displays <see cref="PieItem.Value"/> for
		/// a slice in a Pie Chart.
		/// </summary>
		Value,

		/// <summary>
		/// Displays <see cref="PieItem.Value"/> (as % of total) for
		/// a slice in a Pie Chart.
		/// </summary>
		Percent,

		/// <summary>
		/// Displays <see cref="CurveItem.Label"/> for a slice in a Pie Chart.
		/// </summary>
		Name,

		/// <summary>
		///No label displayed. 
		/// </summary>
		None
	}
	
	/// <summary>
	/// Define the auto layout options for the
	/// <see cref="MasterPane.SetLayout(Graphics,PaneLayout)"/> method.
	/// </summary>
	public enum PaneLayout
	{
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s so they are in a square grid (always 2x2, 3x3, 4x4),
		/// leaving blank spaces as required.
		/// </summary>
		/// <remarks>For example, a single pane would generate a 1x1 grid, between 2 and 4 panes would generate
		/// a 2x2 grid, 5 to 9 panes would generate a 3x3 grid.</remarks>
		ForceSquare,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s so they are in a general square (2x2, 3x3, etc.), but use extra
		/// columns when necessary (row x column = 1x2, 2x3, 3x4, etc.) depending on the total number
		/// of panes required.
		/// </summary>
		/// <remarks>For example, a 2x2 grid has four panes and a 3x3 grid has 9 panes.  If there are
		/// 6 panes required, then this option will eliminate a row (column preferred) to make a
		/// 2 row x 3 column grid.  With 7 panes, it will make a 3x3 grid with 2 empty spaces.</remarks>
		SquareColPreferred,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s so they are in a general square (2x2, 3x3, etc.), but use extra
		/// rows when necessary (2x1, 3x2, 4x3, etc.) depending on the total number of panes required.
		/// </summary>
		/// <remarks>For example, a 2x2 grid has four panes and a 3x3 grid has 9 panes.  If there are
		/// 6 panes required, then this option will eliminate a column (row preferred) to make a
		/// 3 row x 2 column grid.  With 7 panes, it will make a 3x3 grid with 2 empty spaces.</remarks>
		SquareRowPreferred,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s in a single row
		/// </summary>
		SingleRow,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s in a single column
		/// </summary>
		SingleColumn,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of columns: The first row has
		/// 1 column and the second row has 2 columns for a total of 3 panes.
		/// </summary>
		ExplicitCol12,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of columns: The first row has
		/// 2 columns and the second row has 1 column for a total of 3 panes.
		/// </summary>
		ExplicitCol21,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of columns: The first row has
		/// 2 columns and the second row has 3 columns for a total of 5 panes.
		/// </summary>
		ExplicitCol23,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of columns: The first row has
		/// 3 columns and the second row has 2 columns for a total of 5 panes.
		/// </summary>
		ExplicitCol32,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of rows: The first column has
		/// 1 row and the second column has 2 rows for a total of 3 panes.
		/// </summary>
		ExplicitRow12,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of rows: The first column has
		/// 2 rows and the second column has 1 row for a total of 3 panes.
		/// </summary>
		ExplicitRow21,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of rows: The first column has
		/// 2 rows and the second column has 3 rows for a total of 5 panes.
		/// </summary>
		ExplicitRow23,
		/// <summary>
		/// Layout the <see cref="GraphPane"/>'s with an explicit number of rows: The first column has
		/// 3 rows and the second column has 2 rows for a total of 5 panes.
		/// </summary>
		ExplicitRow32
	}

	/// <summary>
	/// Enum for specifying the type of data to be returned by the ZedGraphWeb Render() method.
	/// </summary>
	public enum RenderModeType
	{
		/// <summary>
		/// Renders as an IMG tag referencing a local generated image. ContentType stays text.
		/// </summary>
		ImageTag,
		/// <summary>
		/// Renders the binary image. ContentType is changed accordingly.
		/// </summary>
		RawImage
	}


}