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
using System.Text;
using System.Drawing;

namespace ZedGraph
{
	/// <summary>
	/// A class designed to simplify the process of getting the actual value for
	/// the various stacked and regular curve types
	/// </summary>
	/// 
	/// <author> John Champion</author>
	/// <version> $Revision: 3.21 $ $Date: 2008-12-02 12:55:34 $ </version>
	public class ValueHandler
	{
		private GraphPane _pane;

		/// <summary>
		/// Basic constructor that saves a reference to the parent
		/// <see cref="GraphPane"/> object.
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane"/> object.</param>
		/// <param name="initialize">A <see cref="bool"/> flag to indicate whether or
		/// not the drawing variables should be initialized.  Initialization is not
		/// required if this is part of a ZedGraph internal draw operation (i.e., its in
		/// the middle of a call to <see cref="GraphPane.Draw"/>).  Otherwise, you should
		/// initialize to make sure the drawing variables are configured.  true to do
		/// an initialization, false otherwise.</param>
		public ValueHandler( GraphPane pane, bool initialize )
		{
			_pane = pane;
			if ( initialize )
			{
				// just create a dummy image, which results in a full draw operation
				using ( Image image = pane.GetImage() )
				{
				}
			}
		}

		/// <summary>
		/// Get the user scale values associate with a particular point of a
		/// particular curve.</summary>
		/// <remarks>The main purpose of this method is to handle
		/// stacked bars, in which case the stacked values are returned rather
		/// than the individual data values.
		/// </remarks>
		/// <param name="curve">A <see cref="CurveItem"/> object of interest.</param>
		/// <param name="iPt">The zero-based point index for the point of interest.</param>
		/// <param name="baseVal">A <see cref="Double"/> value representing the value
		/// for the independent axis.</param>
		/// <param name="lowVal">A <see cref="Double"/> value representing the lower
		/// value for the dependent axis.</param>
		/// <param name="hiVal">A <see cref="Double"/> value representing the upper
		/// value for the dependent axis.</param>
		/// <returns>true if the data point is value, false for
		/// <see cref="PointPairBase.Missing"/>, invalid, etc. data.</returns>
		public bool GetValues( CurveItem curve, int iPt, out double baseVal,
							out double lowVal, out double hiVal )
		{
			return GetValues( _pane, curve, iPt, out baseVal,
									out lowVal, out hiVal );
		}

		/// <summary>
		/// Get the user scale values associate with a particular point of a
		/// particular curve.</summary>
		/// <remarks>The main purpose of this method is to handle
		/// stacked bars and lines, in which case the stacked values are returned rather
		/// than the individual data values.  However, this method works generically for any
		/// curve type.
		/// </remarks>
		/// <param name="pane">The parent <see cref="GraphPane"/> object.</param>
		/// <param name="curve">A <see cref="CurveItem"/> object of interest.</param>
		/// <param name="iPt">The zero-based point index for the point of interest.</param>
		/// <param name="baseVal">A <see cref="Double"/> value representing the value
		/// for the independent axis.</param>
		/// <param name="lowVal">A <see cref="Double"/> value representing the lower
		/// value for the dependent axis.</param>
		/// <param name="hiVal">A <see cref="Double"/> value representing the upper
		/// value for the dependent axis.</param>
		/// <returns>true if the data point is value, false for
		/// <see cref="PointPairBase.Missing"/>, invalid, etc. data.</returns>
		public static bool GetValues( GraphPane pane, CurveItem curve, int iPt,
							out double baseVal, out double lowVal, out double hiVal )
		{
			hiVal = PointPair.Missing;
			lowVal = PointPair.Missing;
			baseVal = PointPair.Missing;

			if ( curve == null || curve.Points.Count <= iPt || !curve.IsVisible )
				return false;

			Axis baseAxis = curve.BaseAxis( pane );
			Axis valueAxis = curve.ValueAxis( pane );

			if ( baseAxis is XAxis || baseAxis is X2Axis )
				baseVal = curve.Points[iPt].X;
			else
				baseVal = curve.Points[iPt].Y;

			// is it a stacked bar type?
			if ( curve is BarItem && ( pane._barSettings.Type == BarType.Stack ||
						pane._barSettings.Type == BarType.PercentStack ) )
			{
				double positiveStack = 0;
				double negativeStack = 0;
				double curVal;

				// loop through all the curves, summing up the values to get a total (only
				// for the current ordinal position iPt)
				foreach ( CurveItem tmpCurve in pane.CurveList )
				{
					// Sum the value for the current curve only if it is a bar
					if ( tmpCurve.IsBar && tmpCurve.IsVisible )
					{
						curVal = PointPair.Missing;
						// For non-ordinal curves, find a matching base value (must match exactly)
						if ( curve.IsOverrideOrdinal || !baseAxis._scale.IsAnyOrdinal )
						{
							IPointList points = tmpCurve.Points;

							for ( int i=0; i<points.Count; i++ )
							{
								if ( ( baseAxis is XAxis || baseAxis is X2Axis ) && points[i].X == baseVal )
								{
									curVal = points[i].Y;
									break;
								}
								else if ( !(baseAxis is XAxis || baseAxis is X2Axis) && points[i].Y == baseVal )
								{
									curVal = points[i].X;
									break;
								}
							}
						}
						// otherwise, it's an ordinal type so use the value at the same ordinal position
						else if ( iPt < tmpCurve.Points.Count )
						{
							// Get the value for the appropriate value axis
							if ( baseAxis is XAxis || baseAxis is X2Axis )
								curVal = tmpCurve.Points[iPt].Y;
							else
								curVal = tmpCurve.Points[iPt].X;
						}

						// If it's a missing value, skip it
						if ( curVal == PointPair.Missing )
						{
							positiveStack = PointPair.Missing;
							negativeStack = PointPair.Missing;
						}

						// the current curve is the target curve, save the summed values for later
						if ( tmpCurve == curve )
						{
							// if the value is positive, use the positive stack
							if ( curVal >= 0 )
							{
								lowVal = positiveStack;
								hiVal = ( curVal == PointPair.Missing || positiveStack == PointPair.Missing ) ?
										PointPair.Missing : positiveStack + curVal;
							}
							// otherwise, use the negative stack
							else
							{
								hiVal = negativeStack;
								lowVal = ( curVal == PointPair.Missing || negativeStack == PointPair.Missing ) ?
										PointPair.Missing : negativeStack + curVal;
							}
						}

						// Add all positive values to the positive stack, and negative values to the
						// negative stack
						if ( curVal >= 0 )
							positiveStack = ( curVal == PointPair.Missing || positiveStack == PointPair.Missing ) ?
										PointPair.Missing : positiveStack + curVal;
						else
							negativeStack = ( curVal == PointPair.Missing || negativeStack == PointPair.Missing ) ?
										PointPair.Missing : negativeStack + curVal;
					}
				}

				// if the curve is a PercentStack type, then calculate the percent for this bar
				// based on the total height of the stack
				if ( pane._barSettings.Type == BarType.PercentStack &&
							hiVal != PointPair.Missing && lowVal != PointPair.Missing )
				{
					// Use the total magnitude of the positive plus negative bar stacks to determine
					// the percentage value
					positiveStack += Math.Abs( negativeStack );

					// just to avoid dividing by zero...
					if ( positiveStack != 0 )
					{
						// calculate the percentage values
						lowVal = lowVal / positiveStack * 100.0;
						hiVal = hiVal / positiveStack * 100.0;
					}
					else
					{
						lowVal = 0;
						hiVal = 0;
					}
				}

				if ( baseVal == PointPair.Missing || lowVal == PointPair.Missing ||
						hiVal == PointPair.Missing )
					return false;
				else
					return true;
			}
			// If the curve is a stacked line type, then sum up the values similar to the stacked bar type
			else if ( curve is LineItem && pane.LineType == LineType.Stack )
			{
				double stack = 0;
				double curVal;

				// loop through all the curves, summing up the values to get a total (only
				// for the current ordinal position iPt)
				foreach ( CurveItem tmpCurve in pane.CurveList )
				{
					// make sure the curve is a Line type
					if ( tmpCurve is LineItem && tmpCurve.IsVisible )
					{
						curVal = PointPair.Missing;
						// For non-ordinal curves, find a matching base value (must match exactly)
						if ( curve.IsOverrideOrdinal || !baseAxis._scale.IsAnyOrdinal )
						{
							IPointList points = tmpCurve.Points;

							for ( int i = 0; i < points.Count; i++ )
							{
								if ( points[i].X == baseVal )
								{
									curVal = points[i].Y;
									break;
								}
							}
						}
						// otherwise, it's an ordinal type so use the value at the same ordinal position
						else if ( iPt < tmpCurve.Points.Count )
						{
							// For line types, the Y axis is always the value axis
							curVal = tmpCurve.Points[iPt].Y;
						}

						// if the current value is missing, then the rest of the stack is missing
						if ( curVal == PointPair.Missing )
							stack = PointPair.Missing;

						// if the current curve is the target curve, save the values
						if ( tmpCurve == curve )
						{
							lowVal = stack;
//							if ( curVal < 0 && stack == 0 )
//							{
//								stack = curVal;
//								lowVal = curVal;
//								hiVal = curVal;
//							}
//							else
								hiVal = ( curVal == PointPair.Missing || stack == PointPair.Missing ) ?
									PointPair.Missing : stack + curVal;
						}

						// sum all the curves to a single total.  This includes both positive and
						// negative values (unlike the bar stack type).
						stack = ( curVal == PointPair.Missing || stack == PointPair.Missing ) ?
								PointPair.Missing : stack + curVal;
					}
				}
				
				if ( baseVal == PointPair.Missing || lowVal == PointPair.Missing ||
					hiVal == PointPair.Missing )
					return false;
				else
					return true;
			}
			// otherwise, the curve is not a stacked type (not a stacked bar or stacked line)
			else
			{
                if ((!(curve is HiLowBarItem)) && (!(curve is ErrorBarItem)))
					lowVal = 0;
				else
					lowVal = curve.Points[iPt].LowValue;

				if ( baseAxis is XAxis || baseAxis is X2Axis )
					hiVal = curve.Points[iPt].Y;
				else
					hiVal = curve.Points[iPt].X;
			}

			// Special Exception: Bars on log scales should always plot from the Min value upwards,
			// since they can never be zero
			if ( curve is BarItem && valueAxis._scale.IsLog && lowVal == 0 )
				lowVal = valueAxis._scale._min;

			if ( baseVal == PointPair.Missing || hiVal == PointPair.Missing ||
					( lowVal == PointPair.Missing && ( curve is ErrorBarItem ||
						curve is HiLowBarItem ) ) )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Calculate the user scale position of the center of the specified bar, using the
		/// <see cref="Axis"/> as specified by <see cref="BarSettings.Base"/>.  This method is
		/// used primarily by the
		/// <see cref="GraphPane.FindNearestPoint(PointF,out CurveItem,out int)"/> method in order to
		/// determine the bar "location," which is defined as the center of the top of the individual bar.
		/// </summary>
		/// <param name="curve">The <see cref="CurveItem"/> representing the
		/// bar of interest.</param>
		/// <param name="barWidth">The width of each individual bar. This can be calculated using
		/// the <see cref="CurveItem.GetBarWidth"/> method.</param>
		/// <param name="iCluster">The cluster number for the bar of interest.  This is the ordinal
		/// position of the current point.  That is, if a particular <see cref="CurveItem"/> has
		/// 10 points, then a value of 3 would indicate the 4th point in the data array.</param>
		/// <param name="val">The actual independent axis value for the bar of interest.</param>
		/// <param name="iOrdinal">The ordinal position of the <see cref="CurveItem"/> of interest.
		/// That is, the first bar series is 0, the second is 1, etc.  Note that this applies only
		/// to the bars.  If a graph includes both bars and lines, then count only the bars.</param>
		/// <returns>A user scale value position of the center of the bar of interest.</returns>
		public double BarCenterValue( CurveItem curve, float barWidth, int iCluster,
										  double val, int iOrdinal )
		{
			Axis baseAxis = curve.BaseAxis( _pane );
			if ( curve is ErrorBarItem || curve is HiLowBarItem ||
					curve is OHLCBarItem || curve is JapaneseCandleStickItem )
			{
				if ( baseAxis._scale.IsAnyOrdinal && iCluster >= 0 && !curve.IsOverrideOrdinal )
					return (double) iCluster + 1.0;
				else
					return val;
			}
			else
			{
				float clusterWidth = _pane._barSettings.GetClusterWidth();
				float clusterGap = _pane._barSettings.MinClusterGap * barWidth;
				float barGap = barWidth * _pane._barSettings.MinBarGap;

				if ( curve.IsBar && _pane._barSettings.Type != BarType.Cluster )
					iOrdinal = 0;

				float centerPix = baseAxis.Scale.Transform( curve.IsOverrideOrdinal, iCluster, val )
					- clusterWidth / 2.0F + clusterGap / 2.0F +
					iOrdinal * ( barWidth + barGap ) + 0.5F * barWidth;
				return baseAxis.Scale.ReverseTransform( centerPix );
			}
		}
	}
}
