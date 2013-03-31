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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ZedGraph
{
/*
	/// <summary>
	/// 
	/// </summary>
	public struct DrawingThreadData
	{
		/// <summary>
		/// 
		/// </summary>
		public Graphics _g;
		/// <summary>
		/// 
		/// </summary>
		public MasterPane _masterPane;

//		public DrawingThread( Graphics g, MasterPane masterPane )
//		{
//			_g = g;
//			_masterPane = masterPane;
//		}
	}
*/

	/// <summary>
	/// The ZedGraphControl class provides a UserControl interface to the
	/// <see cref="ZedGraph"/> class library.  This allows ZedGraph to be installed
	/// as a control in the Visual Studio toolbox.  You can use the control by simply
	/// dragging it onto a form in the Visual Studio form editor.  All graph
	/// attributes are accessible via the <see cref="ZedGraphControl.GraphPane"/>
	/// property.
	/// </summary>
	/// <author> John Champion revised by Jerry Vos </author>
	/// <version> $Revision: 3.86 $ $Date: 2007-11-03 04:41:29 $ </version>
	public partial class ZedGraphControl : UserControl
	{

	#region Private Fields

		/// <summary>
		/// This private field contains the instance for the MasterPane object of this control.
		/// You can access the MasterPane object through the public property
		/// <see cref="ZedGraphControl.MasterPane"/>. This is nulled when this Control is
		/// disposed.
		/// </summary>
		private MasterPane _masterPane;

		/// <summary>
		/// private field that determines whether or not tooltips will be displayed
		/// when the mouse hovers over data values.  Use the public property
		/// <see cref="IsShowPointValues"/> to access this value.
		/// </summary>
		private bool _isShowPointValues = false;
		/// <summary>
		/// private field that determines whether or not tooltips will be displayed
		/// showing the scale values while the mouse is located within the ChartRect.
		/// Use the public property <see cref="IsShowCursorValues"/> to access this value.
		/// </summary>
		private bool _isShowCursorValues = false;
		/// <summary>
		/// private field that determines the format for displaying tooltip values.
		/// This format is passed to <see cref="PointPairBase.ToString(string)"/>.
		/// Use the public property <see cref="PointValueFormat"/> to access this
		/// value.
		/// </summary>
		private string _pointValueFormat = PointPair.DefaultFormat;

		/// <summary>
		/// private field that determines whether or not the context menu will be available.  Use the
		/// public property <see cref="IsShowContextMenu"/> to access this value.
		/// </summary>
		private bool _isShowContextMenu = true;

		/// <summary>
		/// private field that determines whether or not a message box will be shown in response to
		/// a context menu "Copy" command.  Use the
		/// public property <see cref="IsShowCopyMessage"/> to access this value.
		/// </summary>
		/// <remarks>
		/// Note that, if this value is set to false, the user will receive no indicative feedback
		/// in response to a Copy action.
		/// </remarks>
		private bool _isShowCopyMessage = true;

		private SaveFileDialog _saveFileDialog = new SaveFileDialog();

		/// <summary>
		/// private field that determines whether the settings of
		/// <see cref="ZedGraph.PaneBase.IsFontsScaled" /> and <see cref="PaneBase.IsPenWidthScaled" />
		/// will be overridden to true during printing operations.
		/// </summary>
		/// <remarks>
		/// Printing involves pixel maps that are typically of a dramatically different dimension
		/// than on-screen pixel maps.  Therefore, it becomes more important to scale the fonts and
		/// lines to give a printed image that looks like what is shown on-screen.  The default
		/// setting for <see cref="ZedGraph.PaneBase.IsFontsScaled" /> is true, but the default
		/// setting for <see cref="PaneBase.IsPenWidthScaled" /> is false.
		/// </remarks>
		/// <value>
		/// A value of true will cause both <see cref="ZedGraph.PaneBase.IsFontsScaled" /> and
		/// <see cref="PaneBase.IsPenWidthScaled" /> to be temporarily set to true during
		/// printing operations.
		/// </value>
		private bool _isPrintScaleAll = true;
		/// <summary>
		/// private field that determines whether or not the visible aspect ratio of the
		/// <see cref="MasterPane" /> <see cref="PaneBase.Rect" /> will be preserved
		/// when printing this <see cref="ZedGraphControl" />.
		/// </summary>
		private bool _isPrintKeepAspectRatio = true;
		/// <summary>
		/// private field that determines whether or not the <see cref="MasterPane" />
		/// <see cref="PaneBase.Rect" /> dimensions will be expanded to fill the
		/// available space when printing this <see cref="ZedGraphControl" />.
		/// </summary>
		/// <remarks>
		/// If <see cref="IsPrintKeepAspectRatio" /> is also true, then the <see cref="MasterPane" />
		/// <see cref="PaneBase.Rect" /> dimensions will be expanded to fit as large
		/// a space as possible while still honoring the visible aspect ratio.
		/// </remarks>
		private bool _isPrintFillPage = true;

		/// <summary>
		/// private field that determines the format for displaying tooltip date values.
		/// This format is passed to <see cref="XDate.ToString(string)"/>.
		/// Use the public property <see cref="PointDateFormat"/> to access this
		/// value.
		/// </summary>
		private string _pointDateFormat = XDate.DefaultFormatStr;

		/// <summary>
		/// private value that determines whether or not zooming is enabled for the control in the
		/// vertical direction.  Use the public property <see cref="IsEnableVZoom"/> to access this
		/// value.
		/// </summary>
		private bool _isEnableVZoom = true;
		/// <summary>
		/// private value that determines whether or not zooming is enabled for the control in the
		/// horizontal direction.  Use the public property <see cref="IsEnableHZoom"/> to access this
		/// value.
		/// </summary>
		private bool _isEnableHZoom = true;

		/// <summary>
		/// private value that determines whether or not zooming is enabled with the mousewheel.
		/// Note that this property is used in combination with the <see cref="IsEnableHZoom"/> and
		/// <see cref="IsEnableVZoom" /> properties to control zoom options.
		/// </summary>
		private bool _isEnableWheelZoom = true;

		/// <summary>
		/// private value that determines whether or not point editing is enabled in the
		/// vertical direction.  Use the public property <see cref="IsEnableVEdit"/> to access this
		/// value.
		/// </summary>
		private bool _isEnableVEdit = false;
		/// <summary>
		/// private value that determines whether or not point editing is enabled in the
		/// horizontal direction.  Use the public property <see cref="IsEnableHEdit"/> to access this
		/// value.
		/// </summary>
		private bool _isEnableHEdit = false;

		/// <summary>
		/// private value that determines whether or not panning is allowed for the control in the
		/// horizontal direction.  Use the
		/// public property <see cref="IsEnableHPan"/> to access this value.
		/// </summary>
		private bool _isEnableHPan = true;
		/// <summary>
		/// private value that determines whether or not panning is allowed for the control in the
		/// vertical direction.  Use the
		/// public property <see cref="IsEnableVPan"/> to access this value.
		/// </summary>
		private bool _isEnableVPan = true;

		// Revision: JCarpenter 10/06
		/// <summary>
		/// Internal variable that indicates if the control can manage selections. 
		/// </summary>
		private bool _isEnableSelection = false;

		private double _zoomStepFraction = 0.1;

		private ScrollRange _xScrollRange;

		private ScrollRangeList _yScrollRangeList;
		private ScrollRangeList _y2ScrollRangeList;

		private bool _isShowHScrollBar = false;
		private bool _isShowVScrollBar = false;
		//private bool		isScrollY2 = false;
		private bool _isAutoScrollRange = false;

		private double _scrollGrace = 0.00; //0.05;

		private bool _isSynchronizeXAxes = false;
		private bool _isSynchronizeYAxes = false;

		//private System.Windows.Forms.HScrollBar hScrollBar1;
		//private System.Windows.Forms.VScrollBar vScrollBar1;

		// The range of values to use the scroll control bars
		private const int _ScrollControlSpan = int.MaxValue;
		// The ratio of the largeChange to the smallChange for the scroll bars
		private const int _ScrollSmallRatio = 10;

		private bool _isZoomOnMouseCenter = false;

		private ResourceManager _resourceManager;

		/// <summary>
		/// private field that stores a <see cref="PrintDocument" /> instance, which maintains
		/// a persistent selection of printer options.
		/// </summary>
		/// <remarks>
		/// This is needed so that a "Print" action utilizes the settings from a prior
		/// "Page Setup" action.</remarks>
		private PrintDocument _pdSave = null;
		//private PrinterSettings printSave = null;
		//private PageSettings pageSave = null;

		/// <summary>
		/// This private field contains a list of selected CurveItems.
		/// </summary>
		//private List<CurveItem> _selection = new List<CurveItem>();
		private Selection _selection = new Selection();

	#endregion

	#region Fields: Buttons & Keys Properties

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to click on
		/// linkable objects
		/// </summary>
		/// <seealso cref="LinkModifierKeys" />
		private MouseButtons _linkButtons = MouseButtons.Left;
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to click
		/// on linkable objects
		/// </summary>
		/// <seealso cref="LinkButtons" />
		private Keys _linkModifierKeys = Keys.Alt;

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to edit point
		/// data values
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHEdit" /> and/or
		/// <see cref="IsEnableVEdit" /> are true.
		/// </remarks>
		/// <seealso cref="EditModifierKeys" />
		private MouseButtons _editButtons = MouseButtons.Right;
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to edit point
		/// data values
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHEdit" /> and/or
		/// <see cref="IsEnableVEdit" /> are true.
		/// </remarks>
		/// <seealso cref="EditButtons" />
		private Keys _editModifierKeys = Keys.Alt;

		/// <summary>
		/// Gets or sets a value that determines which mouse button will be used to select
		/// <see cref="CurveItem" />'s.
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableSelection" /> is true.
		/// </remarks>
		/// <seealso cref="SelectModifierKeys" />
		private MouseButtons _selectButtons = MouseButtons.Left;
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to select
		/// <see cref="CurveItem" />'s.
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableSelection" /> is true.
		/// </remarks>
		/// <seealso cref="SelectButtons" />
		private Keys _selectModifierKeys = Keys.Shift;

		private Keys _selectAppendModifierKeys = Keys.Shift | Keys.Control;

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to perform
		/// zoom operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHZoom" /> and/or
		/// <see cref="IsEnableVZoom" /> are true.
		/// </remarks>
		/// <seealso cref="ZoomModifierKeys" />
		/// <seealso cref="ZoomButtons2" />
		/// <seealso cref="ZoomModifierKeys2" />
		private MouseButtons _zoomButtons = MouseButtons.Left;
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to perform
		/// zoom operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHZoom" /> and/or
		/// <see cref="IsEnableVZoom" /> are true.
		/// </remarks>
		/// <seealso cref="ZoomButtons" />
		/// <seealso cref="ZoomButtons2" />
		/// <seealso cref="ZoomModifierKeys2" />
		private Keys _zoomModifierKeys = Keys.None;

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used as a
		/// secondary option to perform zoom operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHZoom" /> and/or
		/// <see cref="IsEnableVZoom" /> are true.
		/// </remarks>
		/// <seealso cref="ZoomModifierKeys2" />
		/// <seealso cref="ZoomButtons" />
		/// <seealso cref="ZoomModifierKeys" />
		private MouseButtons _zoomButtons2 = MouseButtons.None;
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a
		/// secondary option to perform zoom operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHZoom" /> and/or
		/// <see cref="IsEnableVZoom" /> are true.
		/// </remarks>
		/// <seealso cref="ZoomButtons" />
		/// <seealso cref="ZoomButtons2" />
		/// <seealso cref="ZoomModifierKeys2" />
		private Keys _zoomModifierKeys2 = Keys.None;

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to perform
		/// panning operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHPan" /> and/or
		/// <see cref="IsEnableVPan" /> are true.  A Pan operation (dragging the graph with
		/// the mouse) should not be confused with a scroll operation (using a scroll bar to
		/// move the graph).
		/// </remarks>
		/// <seealso cref="PanModifierKeys" />
		/// <seealso cref="PanButtons2" />
		/// <seealso cref="PanModifierKeys2" />
		private MouseButtons _panButtons = MouseButtons.Left;

		// Setting this field to Keys.Shift here
		// causes an apparent bug to crop up in VS 2003, by which it will have the value:
		// "System.Windows.Forms.Keys.Shift+None", which won't compile
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to perform
		/// panning operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHPan" /> and/or
		/// <see cref="IsEnableVPan" /> are true.  A Pan operation (dragging the graph with
		/// the mouse) should not be confused with a scroll operation (using a scroll bar to
		/// move the graph).
		/// </remarks>
		/// <seealso cref="PanButtons" />
		/// <seealso cref="PanButtons2" />
		/// <seealso cref="PanModifierKeys2" />
		private Keys _panModifierKeys = Keys.Control;

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used as a
		/// secondary option to perform panning operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHPan" /> and/or
		/// <see cref="IsEnableVPan" /> are true.  A Pan operation (dragging the graph with
		/// the mouse) should not be confused with a scroll operation (using a scroll bar to
		/// move the graph).
		/// </remarks>
		/// <seealso cref="PanModifierKeys2" />
		/// <seealso cref="PanButtons" />
		/// <seealso cref="PanModifierKeys" />
		private MouseButtons _panButtons2 = MouseButtons.Middle;

		// Setting this field to Keys.Shift here
		// causes an apparent bug to crop up in VS 2003, by which it will have the value:
		// "System.Windows.Forms.Keys.Shift+None", which won't compile
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a
		/// secondary option to perform panning operations
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHPan" /> and/or
		/// <see cref="IsEnableVPan" /> are true.  A Pan operation (dragging the graph with
		/// the mouse) should not be confused with a scroll operation (using a scroll bar to
		/// move the graph).
		/// </remarks>
		/// <seealso cref="PanButtons2" />
		/// <seealso cref="PanButtons" />
		/// <seealso cref="PanModifierKeys" />
		private Keys _panModifierKeys2 = Keys.None;

	#endregion

	#region Fields: Temporary state variables

		/// <summary>
		/// Internal variable that indicates the control is currently being zoomed. 
		/// </summary>
		private bool _isZooming = false;
		/// <summary>
		/// Internal variable that indicates the control is currently being panned.
		/// </summary>
		private bool _isPanning = false;
		/// <summary>
		/// Internal variable that indicates a point value is currently being edited.
		/// </summary>
		private bool _isEditing = false;

		// Revision: JCarpenter 10/06
		/// <summary>
		/// Internal variable that indicates the control is currently using selection. 
		/// </summary>
		private bool _isSelecting = false;

		/// <summary>
		/// Internal variable that stores the <see cref="GraphPane"/> reference for the Pane that is
		/// currently being zoomed or panned.
		/// </summary>
		private GraphPane _dragPane = null;
		/// <summary>
		/// Internal variable that stores a rectangle which is either the zoom rectangle, or the incremental
		/// pan amount since the last mousemove event.
		/// </summary>
		private Point _dragStartPt;
		private Point _dragEndPt;

		private int _dragIndex;
		private CurveItem _dragCurve;
		private PointPair _dragStartPair;
		/// <summary>
		/// private field that stores the state of the scale ranges prior to starting a panning action.
		/// </summary>
		private ZoomState _zoomState;
		private ZoomStateStack _zoomStateStack;

		//temporarily save the location of a context menu click so we can use it for reference
		// Note that Control.MousePosition ends up returning the position after the mouse has
		// moved to the menu item within the context menu.  Therefore, this point is saved so
		// that we have the point at which the context menu was first right-clicked
		internal Point _menuClickPt;

	#endregion

	#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ZedGraphControl()
		{
			InitializeComponent();

			// These commands do nothing, but they get rid of the compiler warnings for
			// unused events
			bool b = MouseDown == null || MouseUp == null || MouseMove == null;

			// Link in these events from the base class, since we disable them from this class.
			base.MouseDown += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseDown );
			base.MouseUp += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseUp );
			base.MouseMove += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseMove );

			//this.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseWheel );

			// Use double-buffering for flicker-free updating:
			SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
				| ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true );
			//isTransparentBackground = false;
			//SetStyle( ControlStyles.Opaque, false );
			SetStyle( ControlStyles.SupportsTransparentBackColor, true );
			//this.BackColor = Color.Transparent;

			_resourceManager = new ResourceManager( "ZedGraph.ZedGraph.ZedGraphLocale",
				Assembly.GetExecutingAssembly() );

			Rectangle rect = new Rectangle( 0, 0, this.Size.Width, this.Size.Height );
			_masterPane = new MasterPane( "", rect );
			_masterPane.Margin.All = 0;
			_masterPane.Title.IsVisible = false;

			string titleStr = _resourceManager.GetString( "title_def" );
			string xStr = _resourceManager.GetString( "x_title_def" );
			string yStr = _resourceManager.GetString( "y_title_def" );

			//GraphPane graphPane = new GraphPane( rect, "Title", "X Axis", "Y Axis" );
			GraphPane graphPane = new GraphPane( rect, titleStr, xStr, yStr );
			using ( Graphics g = this.CreateGraphics() )
			{
				graphPane.AxisChange( g );
				//g.Dispose();
			}
			_masterPane.Add( graphPane );

			this.hScrollBar1.Minimum = 0;
			this.hScrollBar1.Maximum = 100;
			this.hScrollBar1.Value = 0;

			this.vScrollBar1.Minimum = 0;
			this.vScrollBar1.Maximum = 100;
			this.vScrollBar1.Value = 0;

			_xScrollRange = new ScrollRange( true );
			_yScrollRangeList = new ScrollRangeList();
			_y2ScrollRangeList = new ScrollRangeList();

			_yScrollRangeList.Add( new ScrollRange( true ) );
			_y2ScrollRangeList.Add( new ScrollRange( false ) );

			_zoomState = null;
			_zoomStateStack = new ZoomStateStack();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if the components should be
		/// disposed, false otherwise</param>
		protected override void Dispose( bool disposing )
		{
			lock ( this )
			{
				if ( disposing )
				{
					if ( components != null )
						components.Dispose();
				}
				base.Dispose( disposing );

				_masterPane = null;
			}
		}
	
	#endregion

	#region Methods

		/// <summary>
		/// Called by the system to update the control on-screen
		/// </summary>
		/// <param name="e">
		/// A PaintEventArgs object containing the Graphics specifications
		/// for this Paint event.
		/// </param>
		protected override void OnPaint( PaintEventArgs e )
		{
			lock ( this )
			{
				if ( BeenDisposed || _masterPane == null || this.GraphPane == null )
					return;

				if ( hScrollBar1 != null && this.GraphPane != null &&
					vScrollBar1 != null && _yScrollRangeList != null )
				{
					SetScroll( hScrollBar1, this.GraphPane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
					SetScroll( vScrollBar1, this.GraphPane.YAxis, _yScrollRangeList[0].Min,
						_yScrollRangeList[0].Max );
				}

				base.OnPaint( e );

				// Add a try/catch pair since the users of the control can't catch this one
				try { _masterPane.Draw( e.Graphics ); }
				catch { }
			}

/*
			// first, see if an old thread is still running
			if ( t != null && t.IsAlive )
			{
				t.Abort();
			}

			//dt = new DrawingThread( e.Graphics, _masterPane );
			//g = e.Graphics;

			// Fire off the new thread
			t = new Thread( new ParameterizedThreadStart( DoDrawingThread ) );
			//ct.ApartmentState = ApartmentState.STA;
			//ct.SetApartmentState( ApartmentState.STA );
			DrawingThreadData dtd;
			dtd._g = e.Graphics;
			dtd._masterPane = _masterPane;

			t.Start( dtd );
			//ct.Join();
*/
		}

//		Thread t = null;
		//DrawingThread dt = null;

/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtdobj"></param>
		public void DoDrawingThread( object dtdobj )
		{
			try
			{
				DrawingThreadData dtd = (DrawingThreadData) dtdobj;

				if ( dtd._g != null && dtd._masterPane != null )
					dtd._masterPane.Draw( dtd._g );

				//				else
				//				{
				//					using ( Graphics g2 = CreateGraphics() )
				//						_masterPane.Draw( g2 );
				//				}
			}
			catch
			{

			}
		}
*/

		/// <summary>
		/// Called when the control has been resized.
		/// </summary>
		/// <param name="sender">
		/// A reference to the control that has been resized.
		/// </param>
		/// <param name="e">
		/// An EventArgs object.
		/// </param>
		protected void ZedGraphControl_ReSize( object sender, System.EventArgs e )
		{
			lock ( this )
			{
				if ( BeenDisposed || _masterPane == null )
					return;

				Size newSize = this.Size;

				if ( _isShowHScrollBar )
				{
					hScrollBar1.Visible = true;
					newSize.Height -= this.hScrollBar1.Size.Height;
					hScrollBar1.Location = new Point( 0, newSize.Height );
					hScrollBar1.Size = new Size( newSize.Width, hScrollBar1.Height );
				}
				else
					hScrollBar1.Visible = false;

				if ( _isShowVScrollBar )
				{
					vScrollBar1.Visible = true;
					newSize.Width -= this.vScrollBar1.Size.Width;
					vScrollBar1.Location = new Point( newSize.Width, 0 );
					vScrollBar1.Size = new Size( vScrollBar1.Width, newSize.Height );
				}
				else
					vScrollBar1.Visible = false;

				using ( Graphics g = this.CreateGraphics() )
				{
					_masterPane.ReSize( g, new RectangleF( 0, 0, newSize.Width, newSize.Height ) );
					//g.Dispose();
				}
				this.Invalidate();
			}
		}
		/// <summary>This performs an axis change command on the graphPane.
		/// </summary>
		/// <remarks>
		/// This is the same as
		/// <c>ZedGraphControl.GraphPane.AxisChange( ZedGraphControl.CreateGraphics() )</c>, however,
		/// this method also calls <see cref="SetScrollRangeFromData" /> if <see cref="IsAutoScrollRange" />
		/// is true.
		/// </remarks>
		public virtual void AxisChange()
		{
			lock ( this )
			{
				if ( BeenDisposed || _masterPane == null )
					return;

				using ( Graphics g = this.CreateGraphics() )
				{
					_masterPane.AxisChange( g );
					//g.Dispose();
				}

				if ( _isAutoScrollRange )
					SetScrollRangeFromData();
			}
		}
	#endregion

	#region Zoom States

		/// <summary>
		/// Save the current states of the GraphPanes to a separate collection.  Save a single
		/// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
		/// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
		/// or save a list of states for all GraphPanes if the panes are synchronized.
		/// </summary>
		/// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
		/// are taking place</param>
		/// <param name="type">The <see cref="ZoomState.StateType" /> that describes the
		/// current operation</param>
		/// <returns>The <see cref="ZoomState" /> that corresponds to the
		/// <see paramref="primaryPane" />.
		/// </returns>
		private ZoomState ZoomStateSave( GraphPane primaryPane, ZoomState.StateType type )
		{
			ZoomStateClear();

			if ( _isSynchronizeXAxes || _isSynchronizeYAxes )
			{
				foreach ( GraphPane pane in _masterPane._paneList )
				{
					ZoomState state = new ZoomState( pane, type );
					if ( pane == primaryPane )
						_zoomState = state;
					_zoomStateStack.Add( state );
				}
			}
			else
				_zoomState = new ZoomState( primaryPane, type );

			return _zoomState;
		}

		/// <summary>
		/// Restore the states of the GraphPanes to a previously saved condition (via
		/// <see cref="ZoomStateSave" />.  This is essentially an "undo" for live
		/// pan and scroll actions.  Restores a single
		/// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
		/// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
		/// or save a list of states for all GraphPanes if the panes are synchronized.
		/// </summary>
		/// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
		/// are taking place</param>
		private void ZoomStateRestore( GraphPane primaryPane )
		{
			if ( _isSynchronizeXAxes || _isSynchronizeYAxes )
			{
				for ( int i = 0; i < _masterPane._paneList.Count; i++ )
				{
					if ( i < _zoomStateStack.Count )
						_zoomStateStack[i].ApplyState( _masterPane._paneList[i] );
				}
			}
			else if ( _zoomState != null )
				_zoomState.ApplyState( primaryPane );

			ZoomStateClear();
		}

		/// <summary>
		/// Place the previously saved states of the GraphPanes on the individual GraphPane
		/// <see cref="ZedGraph.GraphPane.ZoomStack" /> collections.  This provides for an
		/// option to undo the state change at a later time.  Save a single
		/// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
		/// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
		/// or save a list of states for all GraphPanes if the panes are synchronized.
		/// </summary>
		/// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
		/// are taking place</param>
		/// <returns>The <see cref="ZoomState" /> that corresponds to the
		/// <see paramref="primaryPane" />.
		/// </returns>
		private void ZoomStatePush( GraphPane primaryPane )
		{
			if ( _isSynchronizeXAxes || _isSynchronizeYAxes )
			{
				for ( int i = 0; i < _masterPane._paneList.Count; i++ )
				{
					if ( i < _zoomStateStack.Count )
						_masterPane._paneList[i].ZoomStack.Add( _zoomStateStack[i] );
				}
			}
			else if ( _zoomState != null )
				primaryPane.ZoomStack.Add( _zoomState );

			ZoomStateClear();
		}

		/// <summary>
		/// Clear the collection of saved states.
		/// </summary>
		private void ZoomStateClear()
		{
			_zoomStateStack.Clear();
			_zoomState = null;
		}

		/// <summary>
		/// Clear all states from the undo stack for each GraphPane.
		/// </summary>
		private void ZoomStatePurge()
		{
			foreach ( GraphPane pane in _masterPane._paneList )
				pane.ZoomStack.Clear();
		}

	#endregion

	}
}
