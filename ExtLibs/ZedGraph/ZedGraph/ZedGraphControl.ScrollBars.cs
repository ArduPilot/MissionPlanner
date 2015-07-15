//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2007  John Champion
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZedGraph
{
	partial class ZedGraphControl
	{

	#region ScrollBars

		private void vScrollBar1_Scroll( object sender, ScrollEventArgs e )
		{
			if ( this.GraphPane != null )
			{
				if ( ( e.Type != ScrollEventType.ThumbPosition &&
						 e.Type != ScrollEventType.ThumbTrack ) ||
					  ( e.Type == ScrollEventType.ThumbTrack &&
						 _zoomState == null ) )
				{
					ZoomStateSave( this.GraphPane, ZoomState.StateType.Scroll );
				}
				for ( int i = 0; i < this.GraphPane.YAxisList.Count; i++ )
				{
					ScrollRange scroll = _yScrollRangeList[i];
					if ( scroll.IsScrollable )
					{
						Axis axis = this.GraphPane.YAxisList[i];
						HandleScroll( axis, e.NewValue, scroll.Min, scroll.Max, vScrollBar1.LargeChange,
										!axis.Scale.IsReverse );
					}
				}

				for ( int i = 0; i < this.GraphPane.Y2AxisList.Count; i++ )
				{
					ScrollRange scroll = _y2ScrollRangeList[i];
					if ( scroll.IsScrollable )
					{
						Axis axis = this.GraphPane.Y2AxisList[i];
						HandleScroll( axis, e.NewValue, scroll.Min, scroll.Max, vScrollBar1.LargeChange,
										!axis.Scale.IsReverse );
					}
				}

				ApplyToAllPanes( this.GraphPane );

				ProcessEventStuff( vScrollBar1, e );
			}
		}

		private void ApplyToAllPanes( GraphPane primaryPane )
		{
			foreach ( GraphPane pane in _masterPane._paneList )
			{
				if ( pane != primaryPane )
				{
					if ( _isSynchronizeXAxes )
						Synchronize( primaryPane.XAxis, pane.XAxis );
					if ( _isSynchronizeYAxes )
						Synchronize( primaryPane.YAxis, pane.YAxis );
				}
			}
		}

		private void Synchronize( Axis source, Axis dest )
		{
			dest._scale._min = source._scale._min;
			dest._scale._max = source._scale._max;
			dest._scale._majorStep = source._scale._majorStep;
			dest._scale._minorStep = source._scale._minorStep;
			dest._scale._minAuto = source._scale._minAuto;
			dest._scale._maxAuto = source._scale._maxAuto;
			dest._scale._majorStepAuto = source._scale._majorStepAuto;
			dest._scale._minorStepAuto = source._scale._minorStepAuto;
		}

		private void hScrollBar1_Scroll( object sender, ScrollEventArgs e )
		{
			if ( this.GraphPane != null )
			{
				if ( ( e.Type != ScrollEventType.ThumbPosition &&
						 e.Type != ScrollEventType.ThumbTrack ) ||
					  ( e.Type == ScrollEventType.ThumbTrack &&
						 _zoomState == null ) )
					ZoomStateSave( this.GraphPane, ZoomState.StateType.Scroll );

				HandleScroll( this.GraphPane.XAxis, e.NewValue, _xScrollRange.Min, _xScrollRange.Max,
								hScrollBar1.LargeChange, this.GraphPane.XAxis.Scale.IsReverse );

				ApplyToAllPanes( this.GraphPane );

				ProcessEventStuff( hScrollBar1, e );
			}
		}

		private void ProcessEventStuff( ScrollBar scrollBar, ScrollEventArgs e )
		{
			if ( e.Type == ScrollEventType.ThumbTrack )
			{
				if ( this.ScrollProgressEvent != null )
					this.ScrollProgressEvent( this, hScrollBar1, _zoomState,
								new ZoomState( this.GraphPane, ZoomState.StateType.Scroll ) );
			}
			else // if ( e.Type == ScrollEventType.ThumbPosition )
			{
				if ( _zoomState != null && _zoomState.IsChanged( this.GraphPane ) )
				{
					//this.GraphPane.ZoomStack.Push( _zoomState );
					ZoomStatePush( this.GraphPane );

					// Provide Callback to notify the user of pan events
					if ( this.ScrollDoneEvent != null )
						this.ScrollDoneEvent( this, hScrollBar1, _zoomState,
									new ZoomState( this.GraphPane, ZoomState.StateType.Scroll ) );

					_zoomState = null;
				}
			}

			if ( this.ScrollEvent != null )
				this.ScrollEvent( scrollBar, e );
		}

/*
		/// <summary>
		/// Use the MouseCaptureChanged as an indicator for the start and end of a scrolling operation
		/// </summary>
		private void ScrollBarMouseCaptureChanged( object sender, EventArgs e )
		{
			return;

			ScrollBar scrollBar = sender as ScrollBar;
			if ( scrollBar != null )
			{
				// If this is the start of a new scroll, then Capture will be true
				if ( scrollBar.Capture )
				{
					// save the original zoomstate
					//_zoomState = new ZoomState( this.GraphPane, ZoomState.StateType.Scroll );
					ZoomStateSave( this.GraphPane, ZoomState.StateType.Scroll );
				}
				else
				{
					// push the prior saved zoomstate, since the scale ranges have already been changed on
					// the fly during the scrolling operation
					if ( _zoomState != null && _zoomState.IsChanged( this.GraphPane ) )
					{
						//this.GraphPane.ZoomStack.Push( _zoomState );
						ZoomStatePush( this.GraphPane );

						// Provide Callback to notify the user of pan events
						if ( this.ScrollDoneEvent != null )
							this.ScrollDoneEvent( this, scrollBar, _zoomState,
										new ZoomState( this.GraphPane, ZoomState.StateType.Scroll ) );

						_zoomState = null;
					}
				}
			}
		}
*/

		private void HandleScroll( Axis axis, int newValue, double scrollMin, double scrollMax,
									int largeChange, bool reverse )
		{
			if ( axis != null )
			{
				if ( scrollMin > axis._scale._min )
					scrollMin = axis._scale._min;
				if ( scrollMax < axis._scale._max )
					scrollMax = axis._scale._max;

				int span = _ScrollControlSpan - largeChange;
				if ( span <= 0 )
					return;

				if ( reverse )
					newValue = span - newValue;

				Scale scale = axis._scale;

				double delta = scale._maxLinearized - scale._minLinearized;
				double scrollMin2 = scale.Linearize( scrollMax ) - delta;
				scrollMin = scale.Linearize( scrollMin );
				//scrollMax = scale.Linearize( scrollMax );
				double val = scrollMin + (double)newValue / (double)span *
						( scrollMin2 - scrollMin );
				scale._minLinearized = val;
				scale._maxLinearized = val + delta;
				/*
								if ( axis.Scale.IsLog )
								{
									double ratio = axis._scale._max / axis._scale._min;
									double scrollMin2 = scrollMax / ratio;

									double val = scrollMin * Math.Exp( (double)newValue / (double)span *
												( Math.Log( scrollMin2 ) - Math.Log( scrollMin ) ) );
									axis._scale._min = val;
									axis._scale._max = val * ratio;
								}
								else
								{
									double delta = axis._scale._max - axis._scale._min;
									double scrollMin2 = scrollMax - delta;

									double val = scrollMin + (double)newValue / (double)span *
												( scrollMin2 - scrollMin );
									axis._scale._min = val;
									axis._scale._max = val + delta;
								}
				*/
				this.Invalidate();
			}
		}

		/// <summary>
		/// Sets the value of the scroll range properties (see <see cref="ScrollMinX" />,
		/// <see cref="ScrollMaxX" />, <see cref="YScrollRangeList" />, and 
		/// <see cref="Y2ScrollRangeList" /> based on the actual range of the data for
		/// each corresponding <see cref="Axis" />.
		/// </summary>
		/// <remarks>
		/// This method is called automatically by <see cref="AxisChange" /> if
		/// <see cref="IsAutoScrollRange" />
		/// is true.  Note that this will not be called if you call AxisChange directly from the
		/// <see cref="GraphPane" />.  For example, zedGraphControl1.AxisChange() works properly, but
		/// zedGraphControl1.GraphPane.AxisChange() does not.</remarks>
		public void SetScrollRangeFromData()
		{
			if ( this.GraphPane != null )
			{
				double grace = CalcScrollGrace( this.GraphPane.XAxis.Scale._rangeMin,
							this.GraphPane.XAxis.Scale._rangeMax );

				_xScrollRange.Min = this.GraphPane.XAxis.Scale._rangeMin - grace;
				_xScrollRange.Max = this.GraphPane.XAxis.Scale._rangeMax + grace;
				_xScrollRange.IsScrollable = true;

				for ( int i = 0; i < this.GraphPane.YAxisList.Count; i++ )
				{
					Axis axis = this.GraphPane.YAxisList[i];
					grace = CalcScrollGrace( axis.Scale._rangeMin, axis.Scale._rangeMax );
					ScrollRange range = new ScrollRange( axis.Scale._rangeMin - grace,
						axis.Scale._rangeMax + grace, _yScrollRangeList[i].IsScrollable );

					if ( i >= _yScrollRangeList.Count )
						_yScrollRangeList.Add( range );
					else
						_yScrollRangeList[i] = range;
				}

				for ( int i = 0; i < this.GraphPane.Y2AxisList.Count; i++ )
				{
					Axis axis = this.GraphPane.Y2AxisList[i];
					grace = CalcScrollGrace( axis.Scale._rangeMin, axis.Scale._rangeMax );
					ScrollRange range = new ScrollRange( axis.Scale._rangeMin - grace,
							axis.Scale._rangeMax + grace, _y2ScrollRangeList[i].IsScrollable );

					if ( i >= _y2ScrollRangeList.Count )
						_y2ScrollRangeList.Add( range );
					else
						_y2ScrollRangeList[i] = range;
				}

				//this.GraphPane.CurveList.GetRange( out scrollMinX, out scrollMaxX,
				//		out scrollMinY, out scrollMaxY, out scrollMinY2, out scrollMaxY2, false, false,
				//		this.GraphPane );
			}
		}

		private double CalcScrollGrace( double min, double max )
		{
			if ( Math.Abs( max - min ) < 1e-30 )
			{
				if ( Math.Abs( max ) < 1e-30 )
					return _scrollGrace;
				else
					return max * _scrollGrace;
			}
			else
				return ( max - min ) * _scrollGrace;
		}

		private void SetScroll( ScrollBar scrollBar, Axis axis, double scrollMin, double scrollMax )
		{
			if ( scrollBar != null && axis != null )
			{
				scrollBar.Minimum = 0;
				scrollBar.Maximum = _ScrollControlSpan - 1;

				if ( scrollMin > axis._scale._min )
					scrollMin = axis._scale._min;
				if ( scrollMax < axis._scale._max )
					scrollMax = axis._scale._max;

				int val = 0;

				Scale scale = axis._scale;
				double minLinearized = scale._minLinearized;
				double maxLinearized = scale._maxLinearized;
				scrollMin = scale.Linearize( scrollMin );
				scrollMax = scale.Linearize( scrollMax );

				double scrollMin2 = scrollMax - ( maxLinearized - minLinearized );
				/*
				if ( axis.Scale.IsLog )
					scrollMin2 = scrollMax / ( axis._scale._max / axis._scale._min );
				else
					scrollMin2 = scrollMax - ( axis._scale._max - axis._scale._min );
				*/
				if ( scrollMin >= scrollMin2 )
				{
					//scrollBar.Visible = false;
					scrollBar.Enabled = false;
					scrollBar.Value = 0;
				}
				else
				{
					double ratio = ( maxLinearized - minLinearized ) / ( scrollMax - scrollMin );

					/*
					if ( axis.Scale.IsLog )
						ratio = ( Math.Log( axis._scale._max ) - Math.Log( axis._scale._min ) ) /
									( Math.Log( scrollMax ) - Math.Log( scrollMin ) );
					else
						ratio = ( axis._scale._max - axis._scale._min ) / ( scrollMax - scrollMin );
					*/

					int largeChange = (int)( ratio * _ScrollControlSpan + 0.5 );
					if ( largeChange < 1 )
						largeChange = 1;
					scrollBar.LargeChange = largeChange;

					int smallChange = largeChange / _ScrollSmallRatio;
					if ( smallChange < 1 )
						smallChange = 1;
					scrollBar.SmallChange = smallChange;

					int span = _ScrollControlSpan - largeChange;

					val = (int)( ( minLinearized - scrollMin ) / ( scrollMin2 - scrollMin ) *
									span + 0.5 );
					/*
					if ( axis.Scale.IsLog )
						val = (int)( ( Math.Log( axis._scale._min ) - Math.Log( scrollMin ) ) /
								( Math.Log( scrollMin2 ) - Math.Log( scrollMin ) ) * span + 0.5 );
					else
						val = (int)( ( axis._scale._min - scrollMin ) / ( scrollMin2 - scrollMin ) *
								span + 0.5 );
					*/
					if ( val < 0 )
						val = 0;
					else if ( val > span )
						val = span;

					//if ( ( axis is XAxis && axis.IsReverse ) || ( ( ! axis is XAxis ) && ! axis.IsReverse ) )
					if ( ( axis is XAxis ) == axis.Scale.IsReverse )
						val = span - val;

					if ( val < scrollBar.Minimum )
						val = scrollBar.Minimum;
					if ( val > scrollBar.Maximum )
						val = scrollBar.Maximum;

					scrollBar.Value = val;
					scrollBar.Enabled = true;
					//scrollBar.Visible = true;
				}
			}
		}

	#endregion

	}
}
