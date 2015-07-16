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

	#region Events

		/// <summary>
		/// A delegate that allows subscribing methods to append or modify the context menu.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="menuStrip">A reference to the <see cref="ContextMenuStrip"/> object
		/// that contains the context menu.
		/// </param>
		/// <param name="mousePt">The point at which the mouse was clicked</param>
		/// <param name="objState">The current context menu state</param>
		/// <seealso cref="ContextMenuBuilder" />
		public delegate void ContextMenuBuilderEventHandler( ZedGraphControl sender,
			ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState );
		/// <summary>
		/// Subscribe to this event to be able to modify the ZedGraph context menu.
		/// </summary>
		/// <remarks>
		/// The context menu is built on the fly after a right mouse click.  You can add menu items
		/// to this menu by simply modifying the <see paramref="menu"/> parameter.
		/// </remarks>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to this event to be able to modify the ZedGraph context menu" )]
		public event ContextMenuBuilderEventHandler ContextMenuBuilder;

		/// <summary>
		/// A delegate that allows notification of zoom and pan events.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> before the zoom or pan event.</param>
		/// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> after the zoom or pan event</param>
		/// <seealso cref="ZoomEvent" />
		public delegate void ZoomEventHandler( ZedGraphControl sender, ZoomState oldState,
			ZoomState newState );

		/// <summary>
		/// Subscribe to this event to be notified when the <see cref="GraphPane"/> is zoomed or panned by the user,
		/// either via a mouse drag operation or by the context menu commands.
		/// </summary>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to this event to be notified when the graph is zoomed or panned" )]
		public event ZoomEventHandler ZoomEvent;

		/// <summary>
		/// A delegate that allows notification of scroll events.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="scrollBar">The source <see cref="ScrollBar"/> object</param>
		/// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> before the scroll event.</param>
		/// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> after the scroll event</param>
		/// <seealso cref="ZoomEvent" />
		public delegate void ScrollDoneHandler( ZedGraphControl sender, ScrollBar scrollBar,
			ZoomState oldState, ZoomState newState );

		/// <summary>
		/// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
		/// using the scrollbars.
		/// </summary>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe this event to be notified when a scroll operation using the scrollbars is completed" )]
		public event ScrollDoneHandler ScrollDoneEvent;

		/// <summary>
		/// A delegate that allows notification of scroll events.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="scrollBar">The source <see cref="ScrollBar"/> object</param>
		/// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> before the scroll event.</param>
		/// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
		/// <see cref="GraphPane"/> after the scroll event</param>
		/// <seealso cref="ZoomEvent" />
		public delegate void ScrollProgressHandler( ZedGraphControl sender, ScrollBar scrollBar,
			ZoomState oldState, ZoomState newState );

		/// <summary>
		/// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
		/// using the scrollbars.
		/// </summary>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe this event to be notified continuously as a scroll operation is taking place" )]
		public event ScrollProgressHandler ScrollProgressEvent;

		/// <summary>
		/// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
		/// using the scrollbars.
		/// </summary>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe this event to be notified of general scroll events" )]
		public event ScrollEventHandler ScrollEvent;

		/// <summary>
		/// A delegate that receives notification after a point-edit operation is completed.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="pane">The <see cref="GraphPane"/> object that contains the
		/// point that has been edited</param>
		/// <param name="curve">The <see cref="CurveItem"/> object that contains the point
		/// that has been edited</param>
		/// <param name="iPt">The integer index of the edited <see cref="PointPair"/> within the
		/// <see cref="IPointList"/> of the selected <see cref="CurveItem"/>
		/// </param>
		/// <seealso cref="PointValueEvent" />
		public delegate string PointEditHandler( ZedGraphControl sender, GraphPane pane,
			CurveItem curve, int iPt );

		/// <summary>
		/// Subscribe to this event to receive notifcation and/or respond after a data
		/// point has been edited via <see cref="IsEnableHEdit" /> and <see cref="IsEnableVEdit" />.
		/// </summary>
		/// <example>
		/// <para>To subscribe to this event, use the following in your Form_Load method:</para>
		/// <code>zedGraphControl1.PointEditEvent +=
		/// new ZedGraphControl.PointEditHandler( MyPointEditHandler );</code>
		/// <para>Add this method to your Form1.cs:</para>
		/// <code>
		///    private string MyPointEditHandler( object sender, GraphPane pane, CurveItem curve, int iPt )
		///    {
		///        PointPair pt = curve[iPt];
		///        return "This value is " + pt.Y.ToString("f2") + " gallons";
		///    }</code>
		/// </example>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to this event to respond to data point edit actions" )]
		public event PointEditHandler PointEditEvent;

		/// <summary>
		/// A delegate that allows custom formatting of the point value tooltips
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="pane">The <see cref="GraphPane"/> object that contains the point value of interest</param>
		/// <param name="curve">The <see cref="CurveItem"/> object that contains the point value of interest</param>
		/// <param name="iPt">The integer index of the selected <see cref="PointPair"/> within the
		/// <see cref="IPointList"/> of the selected <see cref="CurveItem"/></param>
		/// <seealso cref="PointValueEvent" />
		public delegate string PointValueHandler( ZedGraphControl sender, GraphPane pane,
			CurveItem curve, int iPt );

		/// <summary>
		/// Subscribe to this event to provide custom formatting for the tooltips
		/// </summary>
		/// <example>
		/// <para>To subscribe to this event, use the following in your FormLoad method:</para>
		/// <code>zedGraphControl1.PointValueEvent +=
		/// new ZedGraphControl.PointValueHandler( MyPointValueHandler );</code>
		/// <para>Add this method to your Form1.cs:</para>
		/// <code>
		///    private string MyPointValueHandler( object sender, GraphPane pane, CurveItem curve, int iPt )
		///    {
		///    #region
		///        PointPair pt = curve[iPt];
		///        return "This value is " + pt.Y.ToString("f2") + " gallons";
		///    #endregion
		///    }</code>
		/// </example>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to this event to provide custom-formatting for data point tooltips" )]
		public event PointValueHandler PointValueEvent;

		/// <summary>
		/// A delegate that allows custom formatting of the cursor value tooltips
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="pane">The <see cref="GraphPane"/> object that contains the cursor of interest</param>
        /// <param name="mousePt">The <see cref="Point"/> object that represents the cursor value location</param>
		/// <seealso cref="CursorValueEvent" />
		public delegate string CursorValueHandler( ZedGraphControl sender, GraphPane pane,
			Point mousePt );

		/// <summary>
		/// Subscribe to this event to provide custom formatting for the cursor value tooltips
		/// </summary>
		/// <example>
		/// <para>To subscribe to this event, use the following in your FormLoad method:</para>
		/// <code>zedGraphControl1.CursorValueEvent +=
		/// new ZedGraphControl.CursorValueHandler( MyCursorValueHandler );</code>
		/// <para>Add this method to your Form1.cs:</para>
		/// <code>
		///    private string MyCursorValueHandler( object sender, GraphPane pane, Point mousePt )
		///    {
		///    #region
		///		double x, y;
		///		pane.ReverseTransform( mousePt, out x, out y );
		///		return "( " + x.ToString( "f2" ) + ", " + y.ToString( "f2" ) + " )";
		///    #endregion
		///    }</code>
		/// </example>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to this event to provide custom-formatting for cursor value tooltips" )]
		public event CursorValueHandler CursorValueEvent;

		/// <summary>
		/// A delegate that allows notification of mouse events on Graph objects.
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="e">A <see cref="MouseEventArgs" /> corresponding to this event</param>
		/// <seealso cref="MouseDownEvent" />
		/// <returns>
		/// Return true if you have handled the mouse event entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
		/// a zoom operation).  Return false if ZedGraph should go ahead and process the
		/// mouse event.
		/// </returns>
		public delegate bool ZedMouseEventHandler( ZedGraphControl sender, MouseEventArgs e );

		/// <summary>
		/// Subscribe to this event to provide notification of MouseDown clicks on graph
		/// objects
		/// </summary>
		/// <remarks>
		/// This event provides for a notification when the mouse is clicked on an object
		/// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
		/// with this <see cref="ZedGraphControl" />.  This event will use the
		/// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
		/// was clicked.  The boolean value that you return from this handler determines whether
		/// or not the <see cref="ZedGraphControl"/> will do any further handling of the
		/// MouseDown event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
		/// handled the MouseDown event entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
		/// a zoom operation).  Return false if ZedGraph should go ahead and process the
		/// MouseDown event.
		/// </remarks>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to be notified when the left mouse button is clicked down" )]
		public event ZedMouseEventHandler MouseDownEvent;

		/// <summary>
		/// Hide the standard control MouseDown event so that the ZedGraphControl.MouseDownEvent
		/// can be used.  This is so that the user must return true/false in order to indicate
		/// whether or not we should respond to the event.
		/// </summary>
		[Bindable( false ), Browsable( false )]
		public new event MouseEventHandler MouseDown;
		/// <summary>
		/// Hide the standard control MouseUp event so that the ZedGraphControl.MouseUpEvent
		/// can be used.  This is so that the user must return true/false in order to indicate
		/// whether or not we should respond to the event.
		/// </summary>
		[Bindable( false ), Browsable( false )]
		public new event MouseEventHandler MouseUp;
		/// <summary>
		/// Hide the standard control MouseMove event so that the ZedGraphControl.MouseMoveEvent
		/// can be used.  This is so that the user must return true/false in order to indicate
		/// whether or not we should respond to the event.
		/// </summary>
		[Bindable( false ), Browsable( false )]
		private new event MouseEventHandler MouseMove;
		/// <summary>
		/// Subscribe to this event to provide notification of MouseUp clicks on graph
		/// objects
		/// </summary>
		/// <remarks>
		/// This event provides for a notification when the mouse is clicked on an object
		/// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
		/// with this <see cref="ZedGraphControl" />.  This event will use the
		/// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
		/// was clicked.  The boolean value that you return from this handler determines whether
		/// or not the <see cref="ZedGraphControl"/> will do any further handling of the
		/// MouseUp event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
		/// handled the MouseUp event entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
		/// a zoom operation).  Return false if ZedGraph should go ahead and process the
		/// MouseUp event.
		/// </remarks>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to be notified when the left mouse button is released" )]
		public event ZedMouseEventHandler MouseUpEvent;
		/// <summary>
		/// Subscribe to this event to provide notification of MouseMove events over graph
		/// objects
		/// </summary>
		/// <remarks>
		/// This event provides for a notification when the mouse is moving over on the control.
		/// The boolean value that you return from this handler determines whether
		/// or not the <see cref="ZedGraphControl"/> will do any further handling of the
		/// MouseMove event (see <see cref="ZedMouseEventHandler" />).  Return true if you
		/// have handled the MouseMove event entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action.
		/// Return false if ZedGraph should go ahead and process the MouseMove event.
		/// </remarks>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to be notified when the mouse is moved inside the control" )]
		public event ZedMouseEventHandler MouseMoveEvent;

		/// <summary>
		/// Subscribe to this event to provide notification of Double Clicks on graph
		/// objects
		/// </summary>
		/// <remarks>
		/// This event provides for a notification when the mouse is double-clicked on an object
		/// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
		/// with this <see cref="ZedGraphControl" />.  This event will use the
		/// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
		/// was clicked.  The boolean value that you return from this handler determines whether
		/// or not the <see cref="ZedGraphControl"/> will do any further handling of the
		/// DoubleClick event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
		/// handled the DoubleClick event entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action. 
		/// Return false if ZedGraph should go ahead and process the
		/// DoubleClick event.
		/// </remarks>
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to be notified when the left mouse button is double-clicked" )]
		public event ZedMouseEventHandler DoubleClickEvent;

		/// <summary>
		/// A delegate that allows notification of clicks on ZedGraph objects that have
		/// active links enabled
		/// </summary>
		/// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
		/// <param name="pane">The source <see cref="GraphPane" /> in which the click
		/// occurred.
		/// </param>
		/// <param name="source">The source object which was clicked.  This is typically
		/// a type of <see cref="CurveItem" /> if a curve point was clicked, or
		/// a type of <see cref="GraphObj" /> if a graph object was clicked.
		/// </param>
		/// <param name="link">The <see cref="Link" /> object, belonging to
		/// <paramref name="source" />, that contains the link information
		/// </param>
		/// <param name="index">An index value, typically used if a <see cref="CurveItem" />
		/// was clicked, indicating the ordinal value of the actual point that was clicked.
		/// </param>
		/// <returns>
		/// Return true if you have handled the LinkEvent entirely, and you do not
		/// want the <see cref="ZedGraphControl"/> to do any further action.
		/// Return false if ZedGraph should go ahead and process the LinkEvent.
		/// </returns>
		public delegate bool LinkEventHandler( ZedGraphControl sender, GraphPane pane,
			object source, Link link, int index );

		/// <summary>
		/// Subscribe to this event to be able to respond to mouse clicks within linked
		/// objects.
		/// </summary>
		/// <remarks>
		/// Linked objects are typically either <see cref="GraphObj" /> type objects or
		/// <see cref="CurveItem" /> type objects.  These object types can include
		/// hyperlink information allowing for "drill-down" type operation.  
		/// </remarks>
		/// <seealso cref="LinkEventHandler"/>
		/// <seealso cref="Link" />
		/// <seealso cref="CurveItem.Link">CurveItem.Link</seealso>
		/// <seealso cref="GraphObj.Link">GraphObj.Link</seealso>
		// /// <seealso cref="ZedGraph.Web.IsImageMap" />
		[Bindable( true ), Category( "Events" ),
		 Description( "Subscribe to be notified when a link-enabled item is clicked" )]
		public event LinkEventHandler LinkEvent;

	#endregion

	#region Mouse Events

		/// <summary>
		/// Handle a MouseDown event in the <see cref="ZedGraphControl" />
		/// </summary>
		/// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
		/// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
		protected void ZedGraphControl_MouseDown( object sender, MouseEventArgs e )
		{
			_isPanning = false;
			_isZooming = false;
			_isEditing = false;
			_isSelecting = false;
			_dragPane = null;

			Point mousePt = new Point( e.X, e.Y );

			// Callback for doubleclick events
			if ( _masterPane != null && e.Clicks > 1 && this.DoubleClickEvent != null )
			{
				if ( this.DoubleClickEvent( this, e ) )
					return;
			}

			// Provide Callback for MouseDown events
			if ( _masterPane != null && this.MouseDownEvent != null )
			{
				if ( this.MouseDownEvent( this, e ) )
					return;
			}

			if ( e.Clicks > 1 || _masterPane == null )
				return;

			// First, see if the click is within a Linkable object within any GraphPane
			GraphPane pane = this.MasterPane.FindPane( mousePt );
			if ( pane != null &&
					e.Button == _linkButtons && Control.ModifierKeys == _linkModifierKeys )
			{
				object source;
				Link link;
				int index;
				using ( Graphics g = this.CreateGraphics() )
				{
					float scaleFactor = pane.CalcScaleFactor();
					if ( pane.FindLinkableObject( mousePt, g, scaleFactor, out source, out link, out index ) )
					{
						if ( LinkEvent != null && LinkEvent( this, pane, source, link, index ) )
							return;

						string url;
						CurveItem curve = source as CurveItem;

						if ( curve != null )
							url = link.MakeCurveItemUrl( pane, curve, index );
						else
							url = link._url;

						if ( url != string.Empty )
						{
							System.Diagnostics.Process.Start( url );
							// linkable objects override any other actions with mouse
							return;
						}
					}
					//g.Dispose();
				}
			}

			// Second, Check to see if it's within a Chart Rect
			pane = this.MasterPane.FindChartRect( mousePt );
			//Rectangle rect = new Rectangle( mousePt, new Size( 1, 1 ) );

			if ( pane != null &&
				( _isEnableHPan || _isEnableVPan ) &&
				( ( e.Button == _panButtons && Control.ModifierKeys == _panModifierKeys ) ||
				( e.Button == _panButtons2 && Control.ModifierKeys == _panModifierKeys2 ) ) )
			{
				_isPanning = true;
				_dragStartPt = mousePt;
				_dragPane = pane;
				//_zoomState = new ZoomState( _dragPane, ZoomState.StateType.Pan );
				ZoomStateSave( _dragPane, ZoomState.StateType.Pan );
			}
			else if ( pane != null && ( _isEnableHZoom || _isEnableVZoom ) &&
				( ( e.Button == _zoomButtons && Control.ModifierKeys == _zoomModifierKeys ) ||
				( e.Button == _zoomButtons2 && Control.ModifierKeys == _zoomModifierKeys2 ) ) )
			{
				_isZooming = true;
				_dragStartPt = mousePt;
				_dragEndPt = mousePt;
				_dragEndPt.Offset( 1, 1 );
				_dragPane = pane;
				ZoomStateSave( _dragPane, ZoomState.StateType.Zoom );
			}
			//Revision: JCarpenter 10/06
			else if ( pane != null && _isEnableSelection && e.Button == _selectButtons &&
				( Control.ModifierKeys == _selectModifierKeys ||
					Control.ModifierKeys == _selectAppendModifierKeys ) )
			{
				_isSelecting = true;
				_dragStartPt = mousePt;
				_dragEndPt = mousePt;
				_dragEndPt.Offset( 1, 1 );
				_dragPane = pane;
			}
			else if ( pane != null && ( _isEnableHEdit || _isEnableVEdit ) &&
				 ( e.Button == EditButtons && Control.ModifierKeys == EditModifierKeys ) )
			{

				// find the point that was clicked, and make sure the point list is editable
				// and that it's a primary Y axis (the first Y or Y2 axis)
				if ( pane.FindNearestPoint( mousePt, out _dragCurve, out _dragIndex ) &&
							_dragCurve.Points is IPointListEdit )
				{
					_isEditing = true;
					_dragPane = pane;
					_dragStartPt = mousePt;
					_dragStartPair = _dragCurve[_dragIndex];
				}
			}
		}

		/// <summary>
		/// Set the cursor according to the current mouse location.
		/// </summary>
		protected void SetCursor()
		{
			SetCursor( this.PointToClient( Control.MousePosition ) );
		}

		/// <summary>
		/// Set the cursor according to the current mouse location.
		/// </summary>
		protected void SetCursor( Point mousePt )
		{
			if ( _masterPane != null )
			{
				GraphPane pane = _masterPane.FindChartRect( mousePt );
				if ( ( _isEnableHPan || _isEnableVPan ) && ( Control.ModifierKeys == Keys.Shift || _isPanning ) &&
					( pane != null || _isPanning ) )
					this.Cursor = Cursors.Hand;
				else if ( ( _isEnableVZoom || _isEnableHZoom ) && ( pane != null || _isZooming ) )
					this.Cursor = Cursors.Cross;
				else if ( _isEnableSelection && ( pane != null || _isSelecting ) )
					this.Cursor = Cursors.Cross;
				else
					this.Cursor = Cursors.Default;

				//			else if ( isZoomMode || isPanMode )
				//				this.Cursor = Cursors.No;
			}
		}

		/// <summary>
		/// Handle a KeyUp event
		/// </summary>
		/// <param name="sender">The <see cref="ZedGraphControl" /> in which the KeyUp occurred.</param>
		/// <param name="e">A <see cref="KeyEventArgs" /> instance.</param>
		protected void ZedGraphControl_KeyUp( object sender, KeyEventArgs e )
		{
			SetCursor();
		}

		/// <summary>
		/// Handle the Key Events so ZedGraph can Escape out of a panning or zooming operation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ZedGraphControl_KeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
		{
			SetCursor();

			if ( e.KeyCode == Keys.Escape )
			{
				if ( _isPanning )
					HandlePanCancel();
				if ( _isZooming )
					HandleZoomCancel();
				if ( _isEditing )
					HandleEditCancel();
				//if ( _isSelecting )
				// Esc always cancels the selection
				HandleSelectionCancel();

				_isZooming = false;
				_isPanning = false;
				_isEditing = false;
				_isSelecting = false;

				Refresh();
			}
		}

		/// <summary>
		/// Handle a MouseUp event in the <see cref="ZedGraphControl" />
		/// </summary>
		/// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
		/// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
		protected void ZedGraphControl_MouseUp( object sender, MouseEventArgs e )
		{
			// Provide Callback for MouseUp events
			if ( _masterPane != null && this.MouseUpEvent != null )
			{
				if ( this.MouseUpEvent( this, e ) )
					return;
			}

			if ( _masterPane != null && _dragPane != null )
			{
				// If the MouseUp event occurs, the user is done dragging.
				if ( _isZooming )
					HandleZoomFinish( sender, e );
				else if ( _isPanning )
					HandlePanFinish();
				else if ( _isEditing )
					HandleEditFinish();
				//Revision: JCarpenter 10/06
				else if ( _isSelecting )
					HandleSelectionFinish( sender, e );
			}

			// Reset the rectangle.
			//dragStartPt = new Rectangle( 0, 0, 0, 0 );
			_dragPane = null;
			_isZooming = false;
			_isPanning = false;
			_isEditing = false;
			_isSelecting = false;

			Cursor.Current = Cursors.Default;
		}

		/// <summary>
		/// Make a string label that corresponds to a user scale value.
		/// </summary>
		/// <param name="axis">The axis from which to obtain the scale value.  This determines
		/// if it's a date value, linear, log, etc.</param>
		/// <param name="val">The value to be made into a label</param>
		/// <param name="iPt">The ordinal position of the value</param>
		/// <param name="isOverrideOrdinal">true to override the ordinal settings of the axis,
		/// and prefer the actual value instead.</param>
		/// <returns>The string label.</returns>
		protected string MakeValueLabel( Axis axis, double val, int iPt, bool isOverrideOrdinal )
		{
			if ( axis != null )
			{
				if ( axis.Scale.IsDate || axis.Scale.Type == AxisType.DateAsOrdinal )
				{
					return XDate.ToString( val, _pointDateFormat );
				}
				else if ( axis._scale.IsText && axis._scale._textLabels != null )
				{
					int i = iPt;
					if ( isOverrideOrdinal )
						i = (int)( val - 0.5 );

					if ( i >= 0 && i < axis._scale._textLabels.Length )
						return axis._scale._textLabels[i];
					else
						return ( i + 1 ).ToString();
				}
				else if ( axis.Scale.IsAnyOrdinal && axis.Scale.Type != AxisType.LinearAsOrdinal
								&& !isOverrideOrdinal )
				{
					return iPt.ToString( _pointValueFormat );
				}
				else
					return val.ToString( _pointValueFormat );
			}
			else
				return "";
		}

		/// <summary>
		/// protected method for handling MouseMove events to display tooltips over
		/// individual datapoints.
		/// </summary>
		/// <param name="sender">
		/// A reference to the control that has the MouseMove event.
		/// </param>
		/// <param name="e">
		/// A MouseEventArgs object.
		/// </param>
		protected void ZedGraphControl_MouseMove( object sender, MouseEventArgs e )
		{
			if ( _masterPane != null )
			{
				Point mousePt = new Point( e.X, e.Y );

				// Provide Callback for MouseMove events
				if ( this.MouseMoveEvent != null && this.MouseMoveEvent( this, e ) )
					return;

				//Point tempPt = this.PointToClient( Control.MousePosition );

				SetCursor( mousePt );

                try
                {
                    // If the mouse is being dragged,
                    // undraw and redraw the rectangle as the mouse moves.
                    if (_isZooming)
                        HandleZoomDrag(mousePt);
                    else if (_isPanning)
                        HandlePanDrag(mousePt);
                    else if (_isEditing)
                        HandleEditDrag(mousePt);
                    else if (_isShowCursorValues)
                        HandleCursorValues(mousePt);
                    else if (_isShowPointValues)
                        HandlePointValues(mousePt);
                    //Revision: JCarpenter 10/06
                    else if (_isSelecting)
                        HandleZoomDrag(mousePt);
                }
                catch { }
			}
		}

		private Point HandlePointValues( Point mousePt )
		{
			int iPt;
			GraphPane pane;
			object nearestObj;

			using ( Graphics g = this.CreateGraphics() )
			{

				if ( _masterPane.FindNearestPaneObject( mousePt,
					g, out pane, out nearestObj, out iPt ) )
				{
					if ( nearestObj is CurveItem && iPt >= 0 )
					{
						CurveItem curve = (CurveItem)nearestObj;
						// Provide Callback for User to customize the tooltips
						if ( this.PointValueEvent != null )
						{
							string label = this.PointValueEvent( this, pane, curve, iPt );
							if ( label != null && label.Length > 0 )
							{
								this.pointToolTip.SetToolTip( this, label );
								this.pointToolTip.Active = true;
							}
							else
								this.pointToolTip.Active = false;
						}
						else
						{

							if ( curve is PieItem )
							{
								this.pointToolTip.SetToolTip( this,
									( (PieItem)curve ).Value.ToString( _pointValueFormat ) );
							}
							//							else if ( curve is OHLCBarItem || curve is JapaneseCandleStickItem )
							//							{
							//								StockPt spt = (StockPt)curve.Points[iPt];
							//								this.pointToolTip.SetToolTip( this, ( (XDate) spt.Date ).ToString( "MM/dd/yyyy" ) + "\nOpen: $" +
							//								spt.Open.ToString( "N2" ) +
							//								"\nHigh: $" +
							//								spt.High.ToString( "N2" ) + "\nLow: $" +
							//								spt.Low.ToString( "N2" ) + "\nClose: $" +
							//								spt.Close.ToString
							//								( "N2" ) );
							//							}
							else
							{
								PointPair pt = curve.Points[iPt];

								if ( pt.Tag is string )
									this.pointToolTip.SetToolTip( this, (string)pt.Tag );
								else
								{
									double xVal, yVal, lowVal;
									ValueHandler valueHandler = new ValueHandler( pane, false );
									if ( ( curve is BarItem || curve is ErrorBarItem || curve is HiLowBarItem )
											&& pane.BarSettings.Base != BarBase.X )
										valueHandler.GetValues( curve, iPt, out yVal, out lowVal, out xVal );
									else
										valueHandler.GetValues( curve, iPt, out xVal, out lowVal, out yVal );

									string xStr = MakeValueLabel( curve.GetXAxis( pane ), xVal, iPt,
										curve.IsOverrideOrdinal );
									string yStr = MakeValueLabel( curve.GetYAxis( pane ), yVal, iPt,
										curve.IsOverrideOrdinal );

									this.pointToolTip.SetToolTip( this, "( " + xStr + ", " + yStr + " )" );

									//this.pointToolTip.SetToolTip( this,
									//	curve.Points[iPt].ToString( this.pointValueFormat ) );
								}
							}

							this.pointToolTip.Active = true;
						}
					}
					else
						this.pointToolTip.Active = false;
				}
				else
					this.pointToolTip.Active = false;

				//g.Dispose();
			}
			return mousePt;
		}

		private Point HandleCursorValues( Point mousePt )
		{
			GraphPane pane = _masterPane.FindPane( mousePt );
			if ( pane != null && pane.Chart._rect.Contains( mousePt ) )
			{
				// Provide Callback for User to customize the tooltips
				if ( this.CursorValueEvent != null )
				{
					string label = this.CursorValueEvent( this, pane, mousePt );
					if ( label != null && label.Length > 0 )
					{
						this.pointToolTip.SetToolTip( this, label );
						this.pointToolTip.Active = true;
					}
					else
						this.pointToolTip.Active = false;
				}
				else
				{
					double x, x2, y, y2;
					pane.ReverseTransform( mousePt, out x, out x2, out y, out y2 );
					string xStr = MakeValueLabel( pane.XAxis, x, -1, true );
					string yStr = MakeValueLabel( pane.YAxis, y, -1, true );
					string y2Str = MakeValueLabel( pane.Y2Axis, y2, -1, true );

					this.pointToolTip.SetToolTip( this, "( " + xStr + ", " + yStr + ", " + y2Str + " )" );
					this.pointToolTip.Active = true;
				}
			}
			else
				this.pointToolTip.Active = false;

			return mousePt;
		}


	#endregion

	#region Mouse Wheel Zoom Events

		/// <summary>
		/// Handle a MouseWheel event in the <see cref="ZedGraphControl" />
		/// </summary>
		/// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
		/// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
		protected void ZedGraphControl_MouseWheel( object sender, MouseEventArgs e )
		{
			if ( ( _isEnableVZoom || _isEnableHZoom ) && _isEnableWheelZoom && _masterPane != null )
			{
				GraphPane pane = this.MasterPane.FindChartRect( new PointF( e.X, e.Y ) );
				if ( pane != null && e.Delta != 0 )
				{
					ZoomState oldState = ZoomStateSave( pane, ZoomState.StateType.WheelZoom );
					//ZoomState oldState = pane.ZoomStack.Push( pane, ZoomState.StateType.Zoom );

					PointF centerPoint = new PointF( e.X, e.Y );
					double zoomFraction = ( 1 + ( e.Delta < 0 ? 1.0 : -1.0 ) * ZoomStepFraction );

					ZoomPane( pane, zoomFraction, centerPoint, _isZoomOnMouseCenter, false );

					ApplyToAllPanes( pane );

					using ( Graphics g = this.CreateGraphics() )
					{
						// always AxisChange() the dragPane
						pane.AxisChange( g );

						foreach ( GraphPane tempPane in _masterPane._paneList )
						{
							if ( tempPane != pane && ( _isSynchronizeXAxes || _isSynchronizeYAxes ) )
								tempPane.AxisChange( g );
						}
					}

					ZoomStatePush( pane );

					// Provide Callback to notify the user of zoom events
					if ( this.ZoomEvent != null )
						this.ZoomEvent( this, oldState, new ZoomState( pane, ZoomState.StateType.WheelZoom ) );

					this.Refresh();

				}
			}
		}

		/// <summary>
		/// Zoom a specified pane in or out according to the specified zoom fraction.
		/// </summary>
		/// <remarks>
		/// The zoom will occur on the <see cref="XAxis" />, <see cref="YAxis" />, and
		/// <see cref="Y2Axis" /> only if the corresponding flag, <see cref="IsEnableHZoom" /> or
		/// <see cref="IsEnableVZoom" />, is true.  Note that if there are multiple Y or Y2 axes, all of
		/// them will be zoomed.
		/// </remarks>
		/// <param name="pane">The <see cref="GraphPane" /> instance to be zoomed.</param>
		/// <param name="zoomFraction">The fraction by which to zoom, less than 1 to zoom in, greater than
		/// 1 to zoom out.  For example, 0.9 will zoom in such that the scale is 90% of what it was
		/// originally.</param>
		/// <param name="centerPt">The screen position about which the zoom will be centered.  This
		/// value is only used if <see paramref="isZoomOnCenter" /> is true.
		/// </param>
		/// <param name="isZoomOnCenter">true to cause the zoom to be centered on the point
		/// <see paramref="centerPt" />, false to center on the <see cref="Chart.Rect" />.
		/// </param>
		/// <param name="isRefresh">true to force a refresh of the control, false to leave it unrefreshed</param>
		protected void ZoomPane( GraphPane pane, double zoomFraction, PointF centerPt,
					bool isZoomOnCenter, bool isRefresh )
		{
			double x;
			double x2;
			double[] y;
			double[] y2;

			pane.ReverseTransform( centerPt, out x, out x2, out y, out y2 );

			if ( _isEnableHZoom )
			{
				ZoomScale( pane.XAxis, zoomFraction, x, isZoomOnCenter );
				ZoomScale( pane.X2Axis, zoomFraction, x2, isZoomOnCenter );
			}
			if ( _isEnableVZoom )
			{
				for ( int i = 0; i < pane.YAxisList.Count; i++ )
					ZoomScale( pane.YAxisList[i], zoomFraction, y[i], isZoomOnCenter );
				for ( int i = 0; i < pane.Y2AxisList.Count; i++ )
					ZoomScale( pane.Y2AxisList[i], zoomFraction, y2[i], isZoomOnCenter );
			}

			using ( Graphics g = this.CreateGraphics() )
			{
				pane.AxisChange( g );
				//g.Dispose();
			}

			this.SetScroll( this.hScrollBar1, pane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
			this.SetScroll( this.vScrollBar1, pane.YAxis, _yScrollRangeList[0].Min,
				_yScrollRangeList[0].Max );

			if ( isRefresh )
				Refresh();
		}

		/// <summary>
		/// Zoom a specified pane in or out according to the specified zoom fraction.
		/// </summary>
		/// <remarks>
		/// The zoom will occur on the <see cref="XAxis" />, <see cref="YAxis" />, and
		/// <see cref="Y2Axis" /> only if the corresponding flag, <see cref="IsEnableHZoom" /> or
		/// <see cref="IsEnableVZoom" />, is true.  Note that if there are multiple Y or Y2 axes, all of
		/// them will be zoomed.
		/// </remarks>
		/// <param name="pane">The <see cref="GraphPane" /> instance to be zoomed.</param>
		/// <param name="zoomFraction">The fraction by which to zoom, less than 1 to zoom in, greater than
		/// 1 to zoom out.  For example, 0.9 will zoom in such that the scale is 90% of what it was
		/// originally.</param>
		/// <param name="centerPt">The screen position about which the zoom will be centered.  This
		/// value is only used if <see paramref="isZoomOnCenter" /> is true.
		/// </param>
		/// <param name="isZoomOnCenter">true to cause the zoom to be centered on the point
		/// <see paramref="centerPt" />, false to center on the <see cref="Chart.Rect" />.
		/// </param>
		public void ZoomPane( GraphPane pane, double zoomFraction, PointF centerPt, bool isZoomOnCenter )
		{
			ZoomPane( pane, zoomFraction, centerPt, isZoomOnCenter, true );
		}


		/// <summary>
		/// Zoom the specified axis by the specified amount, with the center of the zoom at the
		/// (optionally) specified point.
		/// </summary>
		/// <remarks>
		/// This method is used for MouseWheel zoom operations</remarks>
		/// <param name="axis">The <see cref="Axis" /> to be zoomed.</param>
		/// <param name="zoomFraction">The zoom fraction, less than 1.0 to zoom in, greater than 1.0 to
		/// zoom out.  That is, a value of 0.9 will zoom in such that the scale length is 90% of what
		/// it previously was.</param>
		/// <param name="centerVal">The location for the center of the zoom.  This is only used if
		/// <see paramref="IsZoomOnMouseCenter" /> is true.</param>
		/// <param name="isZoomOnCenter">true if the zoom is to be centered at the
		/// <see paramref="centerVal" /> screen position, false for the zoom to be centered within
		/// the <see cref="Chart.Rect" />.
		/// </param>
		protected void ZoomScale( Axis axis, double zoomFraction, double centerVal, bool isZoomOnCenter )
		{
			if ( axis != null && zoomFraction > 0.0001 && zoomFraction < 1000.0 )
			{
				Scale scale = axis._scale;
				/*
								if ( axis.Scale.IsLog )
								{
									double ratio = Math.Sqrt( axis._scale._max / axis._scale._min * zoomFraction );

									if ( !isZoomOnCenter )
										centerVal = Math.Sqrt( axis._scale._max * axis._scale._min );

									axis._scale._min = centerVal / ratio;
									axis._scale._max = centerVal * ratio;
								}
								else
								{
				*/
				double minLin = axis._scale._minLinearized;
				double maxLin = axis._scale._maxLinearized;
				double range = ( maxLin - minLin ) * zoomFraction / 2.0;

				if ( !isZoomOnCenter )
					centerVal = ( maxLin + minLin ) / 2.0;

				axis._scale._minLinearized = centerVal - range;
				axis._scale._maxLinearized = centerVal + range;
				//				}

				axis._scale._minAuto = false;
				axis._scale._maxAuto = false;
			}
		}

	#endregion

	#region Pan Events

		private Point HandlePanDrag( Point mousePt )
		{
			double x1, x2, xx1, xx2;
			double[] y1, y2, yy1, yy2;
			//PointF endPoint = mousePt;
			//PointF startPoint = ( (Control)sender ).PointToClient( this.dragRect.Location );

			_dragPane.ReverseTransform( _dragStartPt, out x1, out xx1, out y1, out yy1 );
			_dragPane.ReverseTransform( mousePt, out x2, out xx2, out y2, out yy2 );

			if ( _isEnableHPan )
			{
				PanScale( _dragPane.XAxis, x1, x2 );
				PanScale( _dragPane.X2Axis, xx1, xx2 );
				this.SetScroll( this.hScrollBar1, _dragPane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
			}
			if ( _isEnableVPan )
			{
				for ( int i = 0; i < y1.Length; i++ )
					PanScale( _dragPane.YAxisList[i], y1[i], y2[i] );
				for ( int i = 0; i < yy1.Length; i++ )
					PanScale( _dragPane.Y2AxisList[i], yy1[i], yy2[i] );
				this.SetScroll( this.vScrollBar1, _dragPane.YAxis, _yScrollRangeList[0].Min,
					_yScrollRangeList[0].Max );
			}

			ApplyToAllPanes( _dragPane );

			Refresh();

			_dragStartPt = mousePt;

			return mousePt;
		}

		private void HandlePanFinish()
		{
			// push the prior saved zoomstate, since the scale ranges have already been changed on
			// the fly during the panning operation
			if ( _zoomState != null && _zoomState.IsChanged( _dragPane ) )
			{
				//_dragPane.ZoomStack.Push( _zoomState );
				ZoomStatePush( _dragPane );

				// Provide Callback to notify the user of pan events
				if ( this.ZoomEvent != null )
					this.ZoomEvent( this, _zoomState,
						new ZoomState( _dragPane, ZoomState.StateType.Pan ) );

				_zoomState = null;
			}
		}

		private void HandlePanCancel()
		{
			if ( _isPanning )
			{
				if ( _zoomState != null && _zoomState.IsChanged( _dragPane ) )
				{
					ZoomStateRestore( _dragPane );
					//_zoomState.ApplyState( _dragPane );
					//_zoomState = null;
				}
				_isPanning = false;
				Refresh();

				ZoomStateClear();
			}
		}

		/// <summary>
		/// Handle a panning operation for the specified <see cref="Axis" />.
		/// </summary>
		/// <param name="axis">The <see cref="Axis" /> to be panned</param>
		/// <param name="startVal">The value where the pan started.  The scale range
		/// will be shifted by the difference between <see paramref="startVal" /> and
		/// <see paramref="endVal" />.
		/// </param>
		/// <param name="endVal">The value where the pan ended.  The scale range
		/// will be shifted by the difference between <see paramref="startVal" /> and
		/// <see paramref="endVal" />.
		/// </param>
		protected void PanScale( Axis axis, double startVal, double endVal )
		{
			if ( axis != null )
			{
				Scale scale = axis._scale;
				double delta = scale.Linearize( startVal ) - scale.Linearize( endVal );

				scale._minLinearized += delta;
				scale._maxLinearized += delta;

				scale._minAuto = false;
				scale._maxAuto = false;

				/*
								if ( axis.Type == AxisType.Log )
								{
									axis._scale._min *= startVal / endVal;
									axis._scale._max *= startVal / endVal;
								}
								else
								{
									axis._scale._min += startVal - endVal;
									axis._scale._max += startVal - endVal;
								}
				*/
			}
		}

	#endregion

	#region Edit Point Events

		private void HandleEditDrag( Point mousePt )
		{
			// get the scale values that correspond to the current point
			double curX, curY;
			_dragPane.ReverseTransform( mousePt, _dragCurve.IsX2Axis, _dragCurve.IsY2Axis,
					_dragCurve.YAxisIndex, out curX, out curY );
			double startX, startY;
			_dragPane.ReverseTransform( _dragStartPt, _dragCurve.IsX2Axis, _dragCurve.IsY2Axis,
					_dragCurve.YAxisIndex, out startX, out startY );

			// calculate the new scale values for the point
			PointPair newPt = new PointPair( _dragStartPair );

			Scale xScale = _dragCurve.GetXAxis( _dragPane )._scale;
			if ( _isEnableHEdit )
				newPt.X = xScale.DeLinearize( xScale.Linearize( newPt.X ) +
							xScale.Linearize( curX ) - xScale.Linearize( startX ) );

			Scale yScale = _dragCurve.GetYAxis( _dragPane )._scale;
			if ( _isEnableVEdit )
				newPt.Y = yScale.DeLinearize( yScale.Linearize( newPt.Y ) +
							yScale.Linearize( curY ) - yScale.Linearize( startY ) );

			// save the data back to the point list
			IPointListEdit list = _dragCurve.Points as IPointListEdit;
			if ( list != null )
				list[_dragIndex] = newPt;

			// force a redraw
			Refresh();
		}

		private void HandleEditFinish()
		{
			if ( this.PointEditEvent != null )
				this.PointEditEvent( this, _dragPane, _dragCurve, _dragIndex );
		}

		private void HandleEditCancel()
		{
			if ( _isEditing )
			{
				IPointListEdit list = _dragCurve.Points as IPointListEdit;
				if ( list != null )
					list[_dragIndex] = _dragStartPair;
				_isEditing = false;
				Refresh();
			}
		}

	#endregion

	#region Zoom Events

        private void HandleZoomDrag(Point mousePt)
        {
            // Hide the previous rectangle by calling the
            // DrawReversibleFrame method with the same parameters.
            Rectangle rect = CalcScreenRect(_dragStartPt, _dragEndPt);
            ControlPaint.DrawReversibleFrame(rect, this.BackColor, FrameStyle.Dashed);

            // Bound the zoom to the ChartRect
            _dragEndPt = Point.Round(BoundPointToRect(mousePt, _dragPane.Chart._rect));
            rect = CalcScreenRect(_dragStartPt, _dragEndPt);
            // Draw the new rectangle by calling DrawReversibleFrame again.
            ControlPaint.DrawReversibleFrame(rect, this.BackColor, FrameStyle.Dashed);
        }
        /*
        private void HandleZoomDrag(Point mousePt)
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                // Hide the previous rectangle by calling the
                // DrawReversibleFrame method with the same parameters.
                Rectangle rect = this.CalcScreenRect(this._dragStartPt, this._dragEndPt);
                ReversibleFrame.Draw(g, this.BackColor, rect);

                // Bound the zoom to the ChartRect
                _dragEndPt = Point.Round(this.BoundPointToRect(mousePt, this._dragPane.Chart._rect));
                rect = this.CalcScreenRect(this._dragStartPt, this._dragEndPt);

                // Draw the new rectangle by calling DrawReversibleFrame again.
                ReversibleFrame.Draw(g, this.BackColor, rect);
            }
        }
        */
		private const double ZoomResolution = 1e-300;

		private void HandleZoomFinish( object sender, MouseEventArgs e )
		{
			PointF mousePtF = BoundPointToRect( new Point( e.X, e.Y ), _dragPane.Chart._rect );

			// Only accept a drag if it covers at least 5 pixels in each direction
			//Point curPt = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );
			if ( ( Math.Abs( mousePtF.X - _dragStartPt.X ) > 4 || !_isEnableHZoom ) &&
					( Math.Abs( mousePtF.Y - _dragStartPt.Y ) > 4 || !_isEnableVZoom ) )
			{
				// Draw the rectangle to be evaluated. Set a dashed frame style
				// using the FrameStyle enumeration.
				//ControlPaint.DrawReversibleFrame( this.dragRect,
				//	this.BackColor, FrameStyle.Dashed );

				double x1, x2, xx1, xx2;
				double[] y1, y2, yy1, yy2;
				//PointF startPoint = ( (Control)sender ).PointToClient( this.dragRect.Location );

				_dragPane.ReverseTransform( _dragStartPt, out x1, out xx1, out y1, out yy1 );
				_dragPane.ReverseTransform( mousePtF, out x2, out xx2, out y2, out yy2 );

				bool zoomLimitExceeded = false;

				if ( _isEnableHZoom )
				{
					double min1 = Math.Min( x1, x2 );
					double max1 = Math.Max( x1, x2 );
					double min2 = Math.Min( xx1, xx2 );
					double max2 = Math.Max( xx1, xx2 );

					if ( Math.Abs( x1 - x2 ) < ZoomResolution || Math.Abs( xx1 - xx2 ) < ZoomResolution )
						zoomLimitExceeded = true;
				}

				if ( _isEnableVZoom && !zoomLimitExceeded )
				{
					for ( int i = 0; i < y1.Length; i++ )
					{
						if ( Math.Abs( y1[i] - y2[i] ) < ZoomResolution )
						{
							zoomLimitExceeded = true;
							break;
						}
					}
					for ( int i = 0; i < yy1.Length; i++ )
					{
						if ( Math.Abs( yy1[i] - yy2[i] ) < ZoomResolution )
						{
							zoomLimitExceeded = true;
							break;
						}
					}
				}

				if ( !zoomLimitExceeded )
				{

					ZoomStatePush( _dragPane );
					//ZoomState oldState = _dragPane.ZoomStack.Push( _dragPane,
					//			ZoomState.StateType.Zoom );


					if ( _isEnableHZoom )
					{
						_dragPane.XAxis._scale._min = Math.Min( x1, x2 );
						_dragPane.XAxis._scale._minAuto = false;
						_dragPane.XAxis._scale._max = Math.Max( x1, x2 );
						_dragPane.XAxis._scale._maxAuto = false;

						_dragPane.X2Axis._scale._min = Math.Min( xx1, xx2 );
						_dragPane.X2Axis._scale._minAuto = false;
						_dragPane.X2Axis._scale._max = Math.Max( xx1, xx2 );
						_dragPane.X2Axis._scale._maxAuto = false;
					}

					if ( _isEnableVZoom )
					{
						for ( int i = 0; i < y1.Length; i++ )
						{
							_dragPane.YAxisList[i]._scale._min = Math.Min( y1[i], y2[i] );
							_dragPane.YAxisList[i]._scale._max = Math.Max( y1[i], y2[i] );
							_dragPane.YAxisList[i]._scale._minAuto = false;
							_dragPane.YAxisList[i]._scale._maxAuto = false;
						}
						for ( int i = 0; i < yy1.Length; i++ )
						{
							_dragPane.Y2AxisList[i]._scale._min = Math.Min( yy1[i], yy2[i] );
							_dragPane.Y2AxisList[i]._scale._max = Math.Max( yy1[i], yy2[i] );
							_dragPane.Y2AxisList[i]._scale._minAuto = false;
							_dragPane.Y2AxisList[i]._scale._maxAuto = false;
						}
					}

					this.SetScroll( this.hScrollBar1, _dragPane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
					this.SetScroll( this.vScrollBar1, _dragPane.YAxis, _yScrollRangeList[0].Min,
						_yScrollRangeList[0].Max );

					ApplyToAllPanes( _dragPane );

					// Provide Callback to notify the user of zoom events
					if ( this.ZoomEvent != null )
						this.ZoomEvent( this, _zoomState, //oldState,
							new ZoomState( _dragPane, ZoomState.StateType.Zoom ) );

					using ( Graphics g = this.CreateGraphics() )
					{
						// always AxisChange() the dragPane
						_dragPane.AxisChange( g );

						foreach ( GraphPane pane in _masterPane._paneList )
						{
							if ( pane != _dragPane && ( _isSynchronizeXAxes || _isSynchronizeYAxes ) )
								pane.AxisChange( g );
						}
					}
				}

				Refresh();
			}
		}

		private void HandleZoomCancel()
		{
			if ( _isZooming )
			{
				_isZooming = false;
				Refresh();

				ZoomStateClear();
			}
		}

		private PointF BoundPointToRect( Point mousePt, RectangleF rect )
		{
			PointF newPt = new PointF( mousePt.X, mousePt.Y );

			if ( mousePt.X < rect.X ) newPt.X = rect.X;
			if ( mousePt.X > rect.Right ) newPt.X = rect.Right;
			if ( mousePt.Y < rect.Y ) newPt.Y = rect.Y;
			if ( mousePt.Y > rect.Bottom ) newPt.Y = rect.Bottom;

			return newPt;
		}

		private Rectangle CalcScreenRect( Point mousePt1, Point mousePt2 )
		{
			Point screenPt = PointToScreen( mousePt1 );
			Size size = new Size( mousePt2.X - mousePt1.X, mousePt2.Y - mousePt1.Y );
			Rectangle rect = new Rectangle( screenPt, size );

			if ( _isZooming )
			{
				Rectangle chartRect = Rectangle.Round( _dragPane.Chart._rect );

				Point chartPt = PointToScreen( chartRect.Location );

				if ( !_isEnableVZoom )
				{
					rect.Y = chartPt.Y;
					rect.Height = chartRect.Height + 1;
				}
				else if ( !_isEnableHZoom )
				{
					rect.X = chartPt.X;
					rect.Width = chartRect.Width + 1;
				}
			}

			return rect;
		}

	#endregion

	#region Selection Events

		// Revision: JCarpenter 10/06
		/// <summary>
		/// Perform selection on curves within the drag pane, or under the mouse click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleSelectionFinish( object sender, MouseEventArgs e )
		{
			if ( e.Button != _selectButtons )
			{
				Refresh();
				return;
			}

			PointF mousePtF = BoundPointToRect( new Point( e.X, e.Y ), _dragPane.Chart._rect );

			PointF mousePt = BoundPointToRect( new Point( e.X, e.Y ), _dragPane.Rect );

			Point curPt = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );

			// Only accept a drag if it covers at least 5 pixels in each direction
			//Point curPt = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );
			if ( ( Math.Abs( mousePtF.X - _dragStartPt.X ) > 4 ) &&
					  ( Math.Abs( mousePtF.Y - _dragStartPt.Y ) > 4 ) )
			{

				#region New Code to Select on Rubber Band

				double x1, x2, xx1, xx2;
				double[] y1, y2, yy1, yy2;
				PointF startPoint = ( (Control)sender ).PointToClient( new Point( Convert.ToInt32( this._dragPane.Rect.X ), Convert.ToInt32( this._dragPane.Rect.Y ) ) );

				_dragPane.ReverseTransform( _dragStartPt, out x1, out xx1, out y1, out yy1 );
				_dragPane.ReverseTransform( mousePtF, out x2, out xx2, out y2, out yy2 );

				CurveList objects = new CurveList();

				double left = Math.Min( x1, x2 );
				double right = Math.Max( x1, x2 );

				double top = 0;
				double bottom = 0;

				for ( int i = 0; i < y1.Length; i++ )
				{
					bottom = Math.Min( y1[i], y2[i] );
					top = Math.Max( y1[i], y2[i] );
				}

				for ( int i = 0; i < yy1.Length; i++ )
				{
					bottom = Math.Min( bottom, yy2[i] );
					bottom = Math.Min( yy1[i], bottom );
					top = Math.Max( top, yy2[i] );
					top = Math.Max( yy1[i], top );
				}

				double w = right - left;
				double h = bottom - top;

				RectangleF rF = new RectangleF( (float)left, (float)top, (float)w, (float)h );

                using (Graphics gg = this.CreateGraphics())
                {
                    _dragPane.FindContainedObjects(rF, gg, out objects);
                }

				if ( Control.ModifierKeys == _selectAppendModifierKeys )
					_selection.AddToSelection( _masterPane, objects );
				else
					_selection.Select( _masterPane, objects );
				//				this.Select( objects );

				//Graphics g = this.CreateGraphics();
				//this._dragPane.AxisChange( g );
				//g.Dispose();

				#endregion
			}
			else   // It's a single-select
			{
				#region New Code to Single Select

				//Point mousePt = new Point( e.X, e.Y );

				int iPt;
				GraphPane pane;
				object nearestObj;

				using ( Graphics g = this.CreateGraphics() )
				{
					if ( this.MasterPane.FindNearestPaneObject( mousePt, g, out pane,
								out nearestObj, out iPt ) )
					{
						if ( nearestObj is CurveItem && iPt >= 0 )
						{
							if ( Control.ModifierKeys == _selectAppendModifierKeys )
								_selection.AddToSelection( _masterPane, nearestObj as CurveItem );
							else
								_selection.Select( _masterPane, nearestObj as CurveItem );
						}
						else
							_selection.ClearSelection( _masterPane );

						Refresh();
					}
					else
					{
						_selection.ClearSelection( _masterPane );
					}
				}
				#endregion New Code to Single Select
			}

			using ( Graphics g = this.CreateGraphics() )
			{
				// always AxisChange() the dragPane
				_dragPane.AxisChange( g );

				foreach ( GraphPane pane in _masterPane._paneList )
				{
					if ( pane != _dragPane && ( _isSynchronizeXAxes || _isSynchronizeYAxes ) )
						pane.AxisChange( g );
				}
			}

			Refresh();
		}

		private void HandleSelectionCancel()
		{
			_isSelecting = false;

			_selection.ClearSelection( _masterPane );

			Refresh();
		}

	#endregion

	}
}
