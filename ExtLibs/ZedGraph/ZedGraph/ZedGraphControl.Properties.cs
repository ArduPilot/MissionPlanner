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

	#region Button and Key Properties

		/// <summary>
		/// Gets or sets a value that determines which mouse button will be used as a primary option
		/// to trigger a zoom event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="ZoomModifierKeys"/> to determine the actual zoom combination.
		/// A secondary zoom button/key combination option is available via <see cref="ZoomButtons2"/> and
		/// <see cref="ZoomModifierKeys2"/>.  To not use this button/key combination, set the value
		/// of <see cref="ZoomButtons"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Left ),
		 Description( "Determines which mouse button is used as the primary for zooming" )]
		public MouseButtons ZoomButtons
		{
			get { return _zoomButtons; }
			set { _zoomButtons = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which mouse button will be used as the secondary option
		/// to trigger a zoom event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="ZoomModifierKeys2"/> to determine the actual zoom combination.
		/// The primary zoom button/key combination option is available via <see cref="ZoomButtons"/> and
		/// <see cref="ZoomModifierKeys"/>.  To not use this button/key combination, set the value
		/// of <see cref="ZoomButtons2"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.None ),
		 Description( "Determines which mouse button is used as the secondary for zooming" )]
		public MouseButtons ZoomButtons2
		{
			get { return _zoomButtons2; }
			set { _zoomButtons2 = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a primary option
		/// to trigger a zoom event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="ZoomButtons"/> to determine the actual zoom combination.
		/// A secondary zoom button/key combination option is available via <see cref="ZoomButtons2"/> and
		/// <see cref="ZoomModifierKeys2"/>.  To not use this button/key combination, set the value
		/// of <see cref="ZoomButtons"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.None ),
		 Description( "Determines which modifier key used as the primary for zooming" )]
		public Keys ZoomModifierKeys
		{
			get { return _zoomModifierKeys; }
			set { _zoomModifierKeys = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a secondary option
		/// to trigger a zoom event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="ZoomButtons2"/> to determine the actual zoom combination.
		/// A primary zoom button/key combination option is available via <see cref="ZoomButtons"/> and
		/// <see cref="ZoomModifierKeys"/>.  To not use this button/key combination, set the value
		/// of <see cref="ZoomButtons2"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.None ),
		 Description( "Determines which modifier key used as the secondary for zooming" )]
		public Keys ZoomModifierKeys2
		{
			get { return _zoomModifierKeys2; }
			set { _zoomModifierKeys2 = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines which mouse button will be used as a primary option
		/// to trigger a pan event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="PanModifierKeys"/> to determine the actual pan combination.
		/// A secondary pan button/key combination option is available via <see cref="PanButtons2"/> and
		/// <see cref="PanModifierKeys2"/>.  To not use this button/key combination, set the value
		/// of <see cref="PanButtons"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Left ),
		 Description( "Determines which mouse button is used as the primary for panning" )]
		public MouseButtons PanButtons
		{
			get { return _panButtons; }
			set { _panButtons = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which mouse button will be used as the secondary option
		/// to trigger a pan event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="PanModifierKeys2"/> to determine the actual pan combination.
		/// The primary pan button/key combination option is available via <see cref="PanButtons"/> and
		/// <see cref="PanModifierKeys"/>.  To not use this button/key combination, set the value
		/// of <see cref="PanButtons2"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Middle ),
		 Description( "Determines which mouse button is used as the secondary for panning" )]
		public MouseButtons PanButtons2
		{
			get { return _panButtons2; }
			set { _panButtons2 = value; }
		}

		// NOTE: The default value of PanModifierKeys is Keys.Shift. Because of an apparent bug in
		// VS 2003, the initial value set in InitializeComponent by the code wizard is "Keys.Shift+None"
		// which will not compile.  As a temporary workaround, I've hidden the value so that it won't
		// have compile errors.  This problem does not exist in VS 2005.

		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a primary option
		/// to trigger a pan event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="PanButtons"/> to determine the actual pan combination.
		/// A secondary pan button/key combination option is available via <see cref="PanButtons2"/> and
		/// <see cref="PanModifierKeys2"/>.  To not use this button/key combination, set the value
		/// of <see cref="PanButtons"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.Control ),
		 Description( "Determines which modifier key is used as the primary for panning" )]
		public Keys PanModifierKeys
		{
			get { return _panModifierKeys; }
			set { _panModifierKeys = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used as a secondary option
		/// to trigger a pan event.
		/// </summary>
		/// <remarks>
		/// This value is combined with <see cref="PanButtons2"/> to determine the actual pan combination.
		/// A primary pan button/key combination option is available via <see cref="PanButtons"/> and
		/// <see cref="PanModifierKeys"/>.  To not use this button/key combination, set the value
		/// of <see cref="PanButtons2"/> to <see cref="MouseButtons.None"/>.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.None ),
		 Description( "Determines which modifier key is used as the secondary for panning" )]
		public Keys PanModifierKeys2
		{
			get { return _panModifierKeys2; }
			set { _panModifierKeys2 = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to edit point
		/// data values
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHEdit" /> and/or
		/// <see cref="IsEnableVEdit" /> are true.
		/// </remarks>
		/// <seealso cref="EditModifierKeys" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Right ),
		 Description( "Specify mouse button for point editing" )]
		public MouseButtons EditButtons
		{
			get { return _editButtons; }
			set { _editButtons = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to edit point
		/// data values
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableHEdit" /> and/or
		/// <see cref="IsEnableVEdit" /> are true.
		/// </remarks>
		/// <seealso cref="EditButtons" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.Alt ),
		 Description( "Specify modifier key for point editing" )]
		public Keys EditModifierKeys
		{
			get { return _editModifierKeys; }
			set { _editModifierKeys = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to 
		/// select <see cref="CurveItem" />'s.
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableSelection" /> is true.
		/// </remarks>
		/// <seealso cref="SelectModifierKeys" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Left ),
		 Description( "Specify mouse button for curve selection" )]
		public MouseButtons SelectButtons
		{
			get { return _selectButtons; }
			set { _selectButtons = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which Modifier keys will be used to 
		/// select <see cref="CurveItem" />'s.
		/// </summary>
		/// <remarks>
		/// This setting only applies if <see cref="IsEnableSelection" /> is true.
		/// </remarks>
		/// <seealso cref="SelectButtons" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.Shift ),
		 Description( "Specify modifier key for curve selection" )]
		public Keys SelectModifierKeys
		{
			get { return _selectModifierKeys; }
			set { _selectModifierKeys = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines which Modifier keys will be used to 
		/// append a <see cref="CurveItem" /> to the selection list.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.Shift | Keys.Alt ),
		 Description( "Specify modifier key for append curve selection" )]
		public Keys SelectAppendModifierKeys
		{
			get { return _selectAppendModifierKeys; }
		}

		/// <summary>
		/// Gets or sets a value that determines which Mouse button will be used to click
		/// on linkable objects
		/// </summary>
		/// <seealso cref="LinkModifierKeys" />
		/// <seealso cref="LinkEvent"/>
		// /// <seealso cref="ZedGraph.Web.IsImageMap"/>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( MouseButtons.Left ),
		 Description( "Specify mouse button for clicking on linkable objects" )]
		public MouseButtons LinkButtons
		{
			get { return _linkButtons; }
			set { _linkButtons = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines which modifier keys will be used to click
		/// on linkable objects
		/// </summary>
		/// <seealso cref="LinkButtons" />
		/// <seealso cref="LinkEvent"/>
		// /// <seealso cref="ZedGraph.Web.IsImageMap"/>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( Keys.Alt ),
		 Description( "Specify modifier key for clicking on linkable objects" )]
		public Keys LinkModifierKeys
		{
			get { return _linkModifierKeys; }
			set { _linkModifierKeys = value; }
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.MasterPane"/> property for the control
		/// </summary>
		[Bindable( false ), Browsable( false ),
		 DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public MasterPane MasterPane
		{
			get { lock ( this ) return _masterPane; }
			set { lock ( this ) _masterPane = value; }
		}

		// Testing for Designer attribute
		/*
		Class1 _class1 = null;
		[ Bindable( true ), Browsable( true ), Category( "Data" ), NotifyParentProperty( true ),
			DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
			Description( "My Class1 Test" )]
		public Class1 Class1
		{
			get { if ( _class1 == null ) _class1 = new Class1(); return _class1; }
			set { _class1 = value; }
		}
		*/

		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.GraphPane"/> property for the control
		/// </summary>
		/// <remarks>
		/// <see cref="ZedGraphControl"/> actually uses a <see cref="MasterPane"/> object
		/// to hold a list of <see cref="GraphPane"/> objects.  This property really only
		/// accesses the first <see cref="GraphPane"/> in the list.  If there is more
		/// than one <see cref="GraphPane"/>, use the <see cref="MasterPane"/>
		/// indexer property to access any of the <see cref="GraphPane"/> objects.</remarks>
		[
			Bindable( false ), Browsable( false ),
			DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		//[
		//	Bindable( true ), Browsable( true ), Category( "Data" ), NotifyParentProperty( true ),
		//	AttributeProvider( typeof( GraphPane ) ),
		//	Description("Access to the primary GraphPane object associated with this control")
		//]
		public GraphPane GraphPane
		{
			get
			{
				// Just return the first GraphPane in the list
				lock ( this )
				{
					if ( _masterPane != null && _masterPane.PaneList.Count > 0 )
						return _masterPane[0];
					else
						return null;
				}
			}

			set
			{
				lock ( this )
				{
					//Clear the list, and replace it with the specified Graphpane
					if ( _masterPane != null )
					{
						_masterPane.PaneList.Clear();
						_masterPane.Add( value );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that determines if all drawing operations for this control
		/// will be forced to operate in Anti-alias mode.  Note that if this value is set to
		/// "true", it overrides the setting for sub-objects.  Otherwise, the sub-object settings
		/// (such as <see cref="FontSpec.IsAntiAlias"/>)
		/// will be honored.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to force all objects to be draw in anti-alias mode" )]
		public bool IsAntiAlias
		{
			get { return _masterPane.IsAntiAlias; }
			set { _masterPane.IsAntiAlias = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not tooltips will be displayed
		/// when the mouse hovers over data values.
		/// </summary>
		/// <remarks>The displayed values are taken from <see cref="PointPair.Tag"/>
		/// if it is a <see cref="System.String"/> type, or <see cref="PointPairBase.ToString()"/>
		/// otherwise (using the <see cref="PointValueFormat" /> as a format string).
		/// Additionally, the user can custom format the values using the
		/// <see cref="PointValueEvent" /> event.  Note that <see cref="IsShowPointValues" />
		/// may be overridden by <see cref="IsShowCursorValues" />.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to display tooltips when the mouse hovers over data points" )]
		public bool IsShowPointValues
		{
			get { return _isShowPointValues; }
			set { _isShowPointValues = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not tooltips will be displayed
		/// showing the current scale values when the mouse is within the
		/// <see cref="Chart.Rect" />.
		/// </summary>
		/// <remarks>The displayed values are taken from the current mouse position, and formatted
		/// according to <see cref="PointValueFormat" /> and/or <see cref="PointDateFormat" />.  If this
		/// value is set to true, it overrides the <see cref="IsShowPointValues" /> setting.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to display tooltips showing the current mouse position within the Chart area" )]
		public bool IsShowCursorValues
		{
			get { return _isShowCursorValues; }
			set { _isShowCursorValues = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not editing of point data is allowed in
		/// the horizontal direction.
		/// </summary>
		/// <remarks>
		/// Editing is done by holding down the Alt key, and left-clicking on an individual point of
		/// a given <see cref="CurveItem" /> to drag it to a new location.  The Mouse and Key
		/// combination for this mode are modifiable using <see cref="EditButtons" /> and
		/// <see cref="EditModifierKeys" />.
		/// </remarks>
		/// <seealso cref="EditButtons" />
		/// <seealso cref="EditModifierKeys" />
		/// <seealso cref="IsEnableVEdit" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to allow horizontal editing by alt-left-click-drag" )]
		public bool IsEnableHEdit
		{
			get { return _isEnableHEdit; }
			set { _isEnableHEdit = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not editing of point data is allowed in
		/// the vertical direction.
		/// </summary>
		/// <remarks>
		/// Editing is done by holding down the Alt key, and left-clicking on an individual point of
		/// a given <see cref="CurveItem" /> to drag it to a new location.  The Mouse and Key
		/// combination for this mode are modifiable using <see cref="EditButtons" /> and
		/// <see cref="EditModifierKeys" />.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to allow vertical editing by alt-left-click-drag" )]
		public bool IsEnableVEdit
		{
			get { return _isEnableVEdit; }
			set { _isEnableVEdit = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not zooming is allowed for the control.
		/// </summary>
		/// <remarks>
		/// Zooming is done by left-clicking inside the <see cref="Chart.Rect"/> to drag
		/// out a rectangle, indicating the new scale ranges that will be part of the graph.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow horizontal and vertical zooming by left-click-drag" )]
		public bool IsEnableZoom
		{
			set { _isEnableHZoom = value; _isEnableVZoom = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not zooming is allowed for the control in
		/// the horizontal direction.
		/// </summary>
		/// <remarks>
		/// Zooming is done by left-clicking inside the <see cref="Chart.Rect"/> to drag
		/// out a rectangle, indicating the new scale ranges that will be part of the graph.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow horizontal zooming by left-click-drag" )]
		public bool IsEnableHZoom
		{
			get { return _isEnableHZoom; }
			set { _isEnableHZoom = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not zooming is allowed for the control in
		/// the vertical direction.
		/// </summary>
		/// <remarks>
		/// Zooming is done by left-clicking inside the <see cref="Chart.Rect"/> to drag
		/// out a rectangle, indicating the new scale ranges that will be part of the graph.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow vertical zooming by left-click-drag" )]
		public bool IsEnableVZoom
		{
			get { return _isEnableVZoom; }
			set { _isEnableVZoom = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not zooming is allowed via the mouse wheel.
		/// </summary>
		/// <remarks>
		/// Wheel zooming is done by rotating the mouse wheel.
		/// Note that this property is used in combination with the <see cref="IsEnableHZoom"/> and
		/// <see cref="IsEnableVZoom" /> properties to control zoom options.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow zooming with the mouse wheel" )]
		public bool IsEnableWheelZoom
		{
			get { return _isEnableWheelZoom; }
			set { _isEnableWheelZoom = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not panning is allowed for the control in
		/// the horizontal direction.
		/// </summary>
		/// <remarks>
		/// Panning is done by clicking the middle mouse button (or holding down the shift key
		/// while clicking the left mouse button) inside the <see cref="Chart.Rect"/> and
		/// dragging the mouse around to shift the scale ranges as desired.
		/// </remarks>
		/// <seealso cref="IsEnableVPan"/>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow horizontal panning by middle-mouse-drag or shift-left-drag" )]
		public bool IsEnableHPan
		{
			get { return _isEnableHPan; }
			set { _isEnableHPan = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not panning is allowed for the control in
		/// the vertical direction.
		/// </summary>
		/// <remarks>
		/// Panning is done by clicking the middle mouse button (or holding down the shift key
		/// while clicking the left mouse button) inside the <see cref="Chart.Rect"/> and
		/// dragging the mouse around to shift the scale ranges as desired.
		/// </remarks>
		/// <seealso cref="IsEnableHPan"/>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to allow vertical panning by middle-mouse-drag or shift-left-drag" )]
		public bool IsEnableVPan
		{
			get { return _isEnableVPan; }
			set { _isEnableVPan = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not the context menu will be available.
		/// </summary>
		/// <remarks>The context menu is a menu that appears when you right-click on the
		/// <see cref="ZedGraphControl"/>.  It provides options for Zoom, Pan, AutoScale, Clipboard
		/// Copy, and toggle <see cref="IsShowPointValues"/>.
		/// </remarks>
		/// <value>true to allow the context menu, false to disable it</value>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to enable the right mouse button context menu" )]
		public bool IsShowContextMenu
		{
			get { return _isShowContextMenu; }
			set { _isShowContextMenu = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not a message box will be shown
		/// in response to a context menu "Copy" command.
		/// </summary>
		/// <remarks>
		/// Note that, if this property is set to false, the user will receive no
		/// indicative feedback in response to a Copy action.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to show a message box after a 'Copy' context menu action completes" )]
		public bool IsShowCopyMessage
		{
			get { return _isShowCopyMessage; }
			set { _isShowCopyMessage = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SaveFileDialog" /> instance that will be used
		/// by the "Save As..." context menu item.
		/// </summary>
		/// <remarks>
		/// This provides the opportunity to modify the dialog, such as setting the
		/// <see cref="FileDialog.InitialDirectory" /> property.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "Provides access to the SaveFileDialog for the 'Save As' menu item" )]
		public SaveFileDialog SaveFileDialog
		{
			get { return _saveFileDialog; }
			set { _saveFileDialog = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not the visible aspect ratio of the
		/// <see cref="MasterPane" /> <see cref="PaneBase.Rect" /> will be preserved
		/// when printing this <see cref="ZedGraphControl" />.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to preserve the displayed aspect ratio when printing" )]
		public bool IsPrintKeepAspectRatio
		{
			get { return _isPrintKeepAspectRatio; }
			set { _isPrintKeepAspectRatio = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not the <see cref="MasterPane" />
		/// <see cref="PaneBase.Rect" /> dimensions will be expanded to fill the
		/// available space when printing this <see cref="ZedGraphControl" />.
		/// </summary>
		/// <remarks>
		/// If <see cref="IsPrintKeepAspectRatio" /> is also true, then the <see cref="MasterPane" />
		/// <see cref="PaneBase.Rect" /> dimensions will be expanded to fit as large
		/// a space as possible while still honoring the visible aspect ratio.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to resize to fill the page when printing" )]
		public bool IsPrintFillPage
		{
			get { return _isPrintFillPage; }
			set { _isPrintFillPage = value; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether the settings of
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
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( true ),
		 Description( "true to force font and pen width scaling when printing" )]
		public bool IsPrintScaleAll
		{
			get { return _isPrintScaleAll; }
			set { _isPrintScaleAll = value; }
		}

		/// <summary>
		/// Gets or sets a value that controls whether or not the axis value range for the scroll
		/// bars will be set automatically.
		/// </summary>
		/// <remarks>
		/// If this value is set to true, then the range of the scroll bars will be set automatically
		/// to the actual range of the data as returned by <see cref="CurveList.GetRange" /> at the
		/// time that <see cref="AxisChange" /> was last called.  Note that a value of true
		/// can override any setting of <see cref="ScrollMinX" />, <see cref="ScrollMaxX" />,
		/// <see cref="ScrollMinY" />, <see cref="ScrollMaxY" />, 
		/// <see cref="ScrollMinY2" />, and <see cref="ScrollMaxY2" />.  Note also that you must
		/// call <see cref="AxisChange" /> from the <see cref="ZedGraphControl" /> for this to
		/// work properly (e.g., don't call it directly from the <see cref="GraphPane" />.
		/// Alternatively, you can call <see cref="SetScrollRangeFromData" /> at anytime to set
		/// the scroll bar range.<br />
		/// <b>In most cases, you will probably want to disable
		/// <see cref="ZedGraph.GraphPane.IsBoundedRanges" /> before activating this option.</b>
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to automatically set the scroll bar range to the actual data range" )]
		public bool IsAutoScrollRange
		{
			get { return _isAutoScrollRange; }
			set { _isAutoScrollRange = value; }
		}

		/// <summary>
		/// Set a "grace" value that leaves a buffer area around the data when
		/// <see cref="IsAutoScrollRange" /> is true.
		/// </summary>
		/// <remarks>
		/// This value represents a fraction of the total range around each axis.  For example, if the
		/// axis ranges from 0 to 100, then a 0.05 value for ScrollGrace would set the scroll range
		/// to -5 to 105.
		/// </remarks>
		public double ScrollGrace
		{
			get { return _scrollGrace; }
			set { _scrollGrace = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines if the horizontal scroll bar will be visible.
		/// </summary>
		/// <remarks>This scroll bar allows the display to be scrolled in the horizontal direction.
		/// Another option is display panning, in which the user can move the display around by
		/// clicking directly on it and dragging (see <see cref="IsEnableHPan"/> and <see cref="IsEnableVPan"/>).
		/// You can control the available range of scrolling with the <see cref="ScrollMinX"/> and
		/// <see cref="ScrollMaxX"/> properties.  Note that the scroll range can be set automatically by
		/// <see cref="IsAutoScrollRange" />.<br />
		/// <b>In most cases, you will probably want to disable
		/// <see cref="ZedGraph.GraphPane.IsBoundedRanges" /> before activating this option.</b>
		/// </remarks>
		/// <value>A boolean value.  true to display a horizontal scrollbar, false otherwise.</value>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to display the horizontal scroll bar" )]
		public bool IsShowHScrollBar
		{
			get { return _isShowHScrollBar; }
			set { _isShowHScrollBar = value; ZedGraphControl_ReSize( this, new EventArgs() ); }
		}
		/// <summary>
		/// Gets or sets a value that determines if the vertical scroll bar will be visible.
		/// </summary>
		/// <remarks>This scroll bar allows the display to be scrolled in the vertical direction.
		/// Another option is display panning, in which the user can move the display around by
		/// clicking directly on it and dragging (see <see cref="IsEnableHPan"/> and <see cref="IsEnableVPan"/>).
		/// You can control the available range of scrolling with the <see cref="ScrollMinY"/> and
		/// <see cref="ScrollMaxY"/> properties.
		/// Note that the vertical scroll bar only affects the <see cref="YAxis"/>; it has no impact on
		/// the <see cref="Y2Axis"/>.  The panning options affect both the <see cref="YAxis"/> and
		/// <see cref="Y2Axis"/>.  Note also that the scroll range can be set automatically by
		/// <see cref="IsAutoScrollRange" />.<br />
		/// <b>In most cases, you will probably want to disable
		/// <see cref="ZedGraph.GraphPane.IsBoundedRanges" /> before activating this option.</b>
		/// </remarks>
		/// <value>A boolean value.  true to display a vertical scrollbar, false otherwise.</value>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to display the vertical scroll bar" )]
		public bool IsShowVScrollBar
		{
			get { return _isShowVScrollBar; }
			set { _isShowVScrollBar = value; ZedGraphControl_ReSize( this, new EventArgs() ); }
		}

		/// <summary>
		/// Gets or sets a value that determines if the <see cref="XAxis" /> <see cref="Scale" />
		/// ranges for all <see cref="GraphPane" /> objects in the <see cref="MasterPane" /> will
		/// be forced to match.
		/// </summary>
		/// <remarks>
		/// If set to true (default is false), then all of the <see cref="GraphPane" /> objects
		/// in the <see cref="MasterPane" /> associated with this <see cref="ZedGraphControl" />
		/// will be forced to have matching scale ranges for the x axis.  That is, zoom, pan,
		/// and scroll operations will result in zoom/pan/scroll for all graphpanes simultaneously.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to force the X axis ranges for all GraphPanes to match" )]
		public bool IsSynchronizeXAxes
		{
			get { return _isSynchronizeXAxes; }
			set
			{
				if ( _isSynchronizeXAxes != value )
					ZoomStatePurge();
				_isSynchronizeXAxes = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that determines if the <see cref="YAxis" /> <see cref="Scale" />
		/// ranges for all <see cref="GraphPane" /> objects in the <see cref="MasterPane" /> will
		/// be forced to match.
		/// </summary>
		/// <remarks>
		/// If set to true (default is false), then all of the <see cref="GraphPane" /> objects
		/// in the <see cref="MasterPane" /> associated with this <see cref="ZedGraphControl" />
		/// will be forced to have matching scale ranges for the y axis.  That is, zoom, pan,
		/// and scroll operations will result in zoom/pan/scroll for all graphpanes simultaneously.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to force the Y axis ranges for all GraphPanes to match" )]
		public bool IsSynchronizeYAxes
		{
			get { return _isSynchronizeYAxes; }
			set
			{
				if ( _isSynchronizeYAxes != value )
					ZoomStatePurge();
				_isSynchronizeYAxes = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that determines if the vertical scroll bar will affect the Y2 axis.
		/// </summary>
		/// <remarks>
		/// The vertical scroll bar is automatically associated with the Y axis.  With this value, you
		/// can choose to include or exclude the Y2 axis with the scrolling.  Note that the Y2 axis
		/// scrolling is handled as a secondary.  The vertical scroll bar position always reflects
		/// the status of the Y axis.  This can cause the Y2 axis to "jump" when first scrolled if
		/// the <see cref="ScrollMinY2" /> and <see cref="ScrollMaxY2" /> values are not set to the
		/// same proportions as <see cref="ScrollMinY" /> and <see cref="ScrollMaxY" /> with respect
		/// to the actual <see cref="Scale.Min"/> and <see cref="Scale.Max" />.  Also note that
		/// this property is actually just an alias to the <see cref="ScrollRange.IsScrollable" />
		/// property of the first element of <see cref="YScrollRangeList" />.
		/// </remarks>
		/// <seealso cref="IsShowVScrollBar"/>
		/// <seealso cref="ScrollMinY2"/>
		/// <seealso cref="ScrollMaxY2"/>
		/// <seealso cref="YScrollRangeList" />
		/// <seealso cref="Y2ScrollRangeList" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to scroll the Y2 axis along with the Y axis" )]
		public bool IsScrollY2
		{
			get
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
					return _y2ScrollRangeList[0].IsScrollable;
				else
					return false;
			}
			set
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
				{
					ScrollRange tmp = _y2ScrollRangeList[0];
					tmp.IsScrollable = value;
					_y2ScrollRangeList[0] = tmp;
				}
			}
		}

		/// <summary>
		/// Access the <see cref="ScrollRangeList" /> for the Y axes.
		/// </summary>
		/// <remarks>
		/// This list maintains the user scale ranges for the scroll bars for each axis
		/// in the <see cref="ZedGraph.GraphPane.YAxisList" />.  Each ordinal location in
		/// <see cref="YScrollRangeList" /> corresponds to an equivalent ordinal location
		/// in <see cref="ZedGraph.GraphPane.YAxisList" />.
		/// </remarks>
		/// <seealso cref="ScrollMinY" />
		/// <seealso cref="ScrollMaxY" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true )]
		[Description( "Sets the manual scroll bar ranges for the collection of Y axes" )]
		public ScrollRangeList YScrollRangeList
		{
			get { return _yScrollRangeList; }
		}

		/// <summary>
		/// Access the <see cref="ScrollRangeList" /> for the Y2 axes.
		/// </summary>
		/// <remarks>
		/// This list maintains the user scale ranges for the scroll bars for each axis
		/// in the <see cref="ZedGraph.GraphPane.Y2AxisList" />.  Each ordinal location in
		/// <see cref="Y2ScrollRangeList" /> corresponds to an equivalent ordinal location
		/// in <see cref="ZedGraph.GraphPane.Y2AxisList" />.
		/// </remarks>
		/// <seealso cref="ScrollMinY2" />
		/// <seealso cref="ScrollMaxY2" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true )]
		[Description( "Sets the manual scroll bar ranges for the collection of Y2 axes" )]
		public ScrollRangeList Y2ScrollRangeList
		{
			get { return _y2ScrollRangeList; }
		}

		/// <summary>
		/// The minimum value for the X axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the minimum endpoint of the scroll range will cause the
		/// <see cref="Scale.Min"/> value to be set to <see cref="ScrollMinX"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableHPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.
		/// </remarks>
		/// <value>A double value indicating the minimum axis value</value>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll minimum value for the X axis" )]
		public double ScrollMinX
		{
			get { return _xScrollRange.Min; }
			set { _xScrollRange.Min = value; }
		}
		/// <summary>
		/// The maximum value for the X axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the maximum endpoint of the scroll range will cause the
		/// <see cref="Scale.Max"/> value to be set to <see cref="ScrollMaxX"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableHPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.
		/// </remarks>
		/// <value>A double value indicating the maximum axis value</value>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll maximum value for the X axis" )]
		public double ScrollMaxX
		{
			get { return _xScrollRange.Max; }
			set { _xScrollRange.Max = value; }
		}
		/// <summary>
		/// The minimum value for the Y axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the minimum endpoint of the scroll range will cause the
		/// <see cref="Scale.Min"/> value to be set to <see cref="ScrollMinY"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableVPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.  Also note that
		/// this property is actually just an alias to the <see cref="ScrollRange.Min" />
		/// property of the first element of <see cref="YScrollRangeList" />.
		/// </remarks>
		/// <value>A double value indicating the minimum axis value</value>
		/// <seealso cref="YScrollRangeList" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll minimum value for the Y axis" )]
		public double ScrollMinY
		{
			get
			{
				if ( _yScrollRangeList != null && _yScrollRangeList.Count > 0 )
					return _yScrollRangeList[0].Min;
				else
					return double.NaN;
			}
			set
			{
				if ( _yScrollRangeList != null && _yScrollRangeList.Count > 0 )
				{
					ScrollRange tmp = _yScrollRangeList[0];
					tmp.Min = value;
					_yScrollRangeList[0] = tmp;
				}
			}
		}
		/// <summary>
		/// The maximum value for the Y axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the maximum endpoint of the scroll range will cause the
		/// <see cref="Scale.Max"/> value to be set to <see cref="ScrollMaxY"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableVPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.  Also note that
		/// this property is actually just an alias to the <see cref="ScrollRange.Max" />
		/// property of the first element of <see cref="YScrollRangeList" />.
		/// </remarks>
		/// <value>A double value indicating the maximum axis value</value>
		/// <seealso cref="YScrollRangeList" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll maximum value for the Y axis" )]
		public double ScrollMaxY
		{
			get
			{
				if ( _yScrollRangeList != null && _yScrollRangeList.Count > 0 )
					return _yScrollRangeList[0].Max;
				else
					return double.NaN;
			}
			set
			{
				if ( _yScrollRangeList != null && _yScrollRangeList.Count > 0 )
				{
					ScrollRange tmp = _yScrollRangeList[0];
					tmp.Max = value;
					_yScrollRangeList[0] = tmp;
				}
			}
		}
		/// <summary>
		/// The minimum value for the Y2 axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the minimum endpoint of the scroll range will cause the
		/// <see cref="Scale.Min"/> value to be set to <see cref="ScrollMinY2"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableVPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.  Also note that
		/// this property is actually just an alias to the <see cref="ScrollRange.Min" />
		/// property of the first element of <see cref="Y2ScrollRangeList" />.
		/// </remarks>
		/// <value>A double value indicating the minimum axis value</value>
		/// <seealso cref="Y2ScrollRangeList" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll minimum value for the Y2 axis" )]
		public double ScrollMinY2
		{
			get
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
					return _y2ScrollRangeList[0].Min;
				else
					return double.NaN;
			}
			set
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
				{
					ScrollRange tmp = _y2ScrollRangeList[0];
					tmp.Min = value;
					_y2ScrollRangeList[0] = tmp;
				}
			}
		}
		/// <summary>
		/// The maximum value for the Y2 axis scroll range.
		/// </summary>
		/// <remarks>
		/// Effectively, the maximum endpoint of the scroll range will cause the
		/// <see cref="Scale.Max"/> value to be set to <see cref="ScrollMaxY2"/>.  Note that this
		/// value applies only to the scroll bar settings.  Axis panning (see <see cref="IsEnableVPan"/>)
		/// is not affected by this value.  Note that this value can be overridden by
		/// <see cref="IsAutoScrollRange" /> and <see cref="SetScrollRangeFromData" />.  Also note that
		/// this property is actually just an alias to the <see cref="ScrollRange.Max" />
		/// property of the first element of <see cref="Y2ScrollRangeList" />.
		/// </remarks>
		/// <value>A double value indicating the maximum axis value</value>
		/// <seealso cref="Y2ScrollRangeList" />
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0 ),
		 Description( "Sets the manual scroll maximum value for the Y2 axis" )]
		public double ScrollMaxY2
		{
			get
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
					return _y2ScrollRangeList[0].Max;
				else
					return double.NaN;
			}
			set
			{
				if ( _y2ScrollRangeList != null && _y2ScrollRangeList.Count > 0 )
				{
					ScrollRange tmp = _y2ScrollRangeList[0];
					tmp.Max = value;
					_y2ScrollRangeList[0] = tmp;
				}
			}
		}

		/// <summary>
		/// Returns true if the user is currently scrolling via the scrollbar, or
		/// false if no scrolling is taking place.
		/// </summary>
		/// <remarks>
		/// This method just tests ScrollBar.Capture to see if the
		/// mouse has been captured by the scroll bar.  If so, scrolling is active.
		/// </remarks>
		public Boolean IsScrolling
		{
			get
			{
				if ( hScrollBar1 != null && vScrollBar1 != null )
					return hScrollBar1.Capture || vScrollBar1.Capture;
				else
					return false;
			}
		}


		/// <summary>
		/// Gets or sets the format for displaying tooltip values.
		/// This format is passed to <see cref="PointPairBase.ToString(string)"/>.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="System.Globalization.NumberFormatInfo" /> type
		/// to determine the format strings.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( PointPair.DefaultFormat ),
		 Description( "Sets the numeric display format string for the point value tooltips" )]
		public string PointValueFormat
		{
			get { return _pointValueFormat; }
			set { _pointValueFormat = value; }
		}

		/// <summary>
		/// Gets or sets the format for displaying tooltip values.
		/// This format is passed to <see cref="XDate.ToString(string)"/>.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="System.Globalization.DateTimeFormatInfo" /> type
		/// to determine the format strings.
		/// </remarks>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( XDate.DefaultFormatStr ),
		 Description( "Sets the date display format for the point value tooltips" )]
		public string PointDateFormat
		{
			get { return _pointDateFormat; }
			set { _pointDateFormat = value; }
		}

		/// <summary>
		/// Gets or sets the step size fraction for zooming with the mouse wheel.
		/// A value of 0.1 will result in a 10% zoom step for each mouse wheel movement.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( 0.1 ),
		 Description( "Sets the step size fraction for zooming with the mouse wheel" )]
		public double ZoomStepFraction
		{
			get { return _zoomStepFraction; }
			set { _zoomStepFraction = value; }
		}

		/// <summary>
		/// Gets or sets a boolean value that determines if zooming with the wheel mouse
		/// is centered on the mouse location, or centered on the existing graph.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
		 DefaultValue( false ),
		 Description( "true to center the mouse wheel zoom at the current mouse location" )]
		public bool IsZoomOnMouseCenter
		{
			get { return _isZoomOnMouseCenter; }
			set { _isZoomOnMouseCenter = value; }
		}

		/// <summary>
		/// Gets the graph pane's current image.
		/// <seealso cref="Bitmap"/>
		/// </summary>
		/// <exception cref="ZedGraphException">
		/// When the control has been disposed before this call.
		/// </exception>
		[Bindable( false ), Browsable( false ),
		 DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Image GetImage()
		{
			lock ( this )
			{
				if ( BeenDisposed || _masterPane == null || _masterPane[0] == null )
					throw new ZedGraphException( "The control has been disposed" );

				return _masterPane.GetImage();
			}
		}

		/// <summary>
		/// This checks if the control has been disposed.  This is synonymous with
		/// the graph pane having been nulled or disposed.  Therefore this is the
		/// same as <c>ZedGraphControl.GraphPane == null</c>.
		/// </summary>
		[Bindable( false ), Browsable( false ),
		 DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool BeenDisposed
		{
			get
			{
				lock ( this ) return _masterPane == null;
			}
		}

		// Revision: JCarpenter 10/06
		/// <summary>
		/// Readonly property that gets the list of selected CurveItems
		/// </summary>
		public Selection Selection
		{
			get { return _selection; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not selection is allowed for the control.
		/// </summary>
		[Bindable( true ), Category( "Display" ), NotifyParentProperty( true ),
			DefaultValue( false ),
			Description( "true to allow selecting Curves" )]
		public bool IsEnableSelection
		{
			get { return _isEnableSelection; }
			set
			{
				_isEnableSelection = value;

				/*
				if ( value )
				{
					this.Cursor = Cursors.Default;
					this.IsEnableZoom = false;
				}
				else
				{
					this.Cursor = Cursors.Cross;
					this.IsEnableZoom = true;
				}
				*/
			}
		}

	#endregion

	}
}