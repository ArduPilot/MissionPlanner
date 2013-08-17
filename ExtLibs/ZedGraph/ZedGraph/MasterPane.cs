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

#region Using directives

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace ZedGraph
{
	/// <summary>
	/// A collection class containing a list of <see cref="GraphPane"/> objects
	/// organized together in some form.
	/// </summary>
	/// 
	/// <author>John Champion</author>
	/// <version> $Revision: 3.26 $ $Date: 2007-11-05 18:28:56 $ </version>
	[Serializable]
	public class MasterPane : PaneBase, ICloneable, ISerializable, IDeserializationCallback
	{

	#region Fields

		/// <summary>
		/// Private field that holds a collection of <see cref="GraphPane"/> objects for inclusion
		/// in this <see cref="MasterPane"/>.  Use the public property <see cref="PaneList"/>
		/// to access this collection.
		/// </summary>
		internal PaneList _paneList;
		/// <summary>
		/// Private field that sets the amount of space between the GraphPanes.  Use the public property
		/// <see cref="InnerPaneGap"/> to access this value;
		/// </summary>
		internal float _innerPaneGap;
		/// <summary>
		///Private field that stores a boolean value which signifies whether all 
		///<see cref="ZedGraph.GraphPane"/>s in the chart use the same entries in their 
		///<see cref="Legend"/>  If set to true, only one set of entries will be displayed in 
		///this <see cref="Legend"/> instance.  If set to false, this instance will display all 
		///entries from all <see cref="ZedGraph.GraphPane"/>s.
		/// </summary>
		private bool _isUniformLegendEntries;
		/// <summary>
		/// private field that determines if the
		/// <see cref="DoLayout(Graphics)" />
		/// function will automatically set
		/// the <see cref="PaneBase.BaseDimension" /> of each <see cref="GraphPane" /> in the
		/// <see cref="PaneList" /> such that the scale factors have the same value.
		/// </summary>
		private bool _isCommonScaleFactor;

		/// <summary>
		/// private field that saves the paneLayout format specified when
		/// <see cref="SetLayout(Graphics,PaneLayout)"/> was called. This value will
		/// default to <see cref="MasterPane.Default.PaneLayout"/> if
		/// <see cref="SetLayout(Graphics,PaneLayout)"/> (or an overload) was never called.
		/// </summary>
		internal PaneLayout _paneLayout;

		/// <summary>
		/// Private field that stores the boolean value that determines whether
		/// <see cref="_countList"/> is specifying rows or columns.
		/// </summary>
		internal bool _isColumnSpecified;
		/// <summary>
		/// private field that stores the row/column item count that was specified to the
		/// <see cref="SetLayout(Graphics,bool,int[],float[])"/> method.  This values will be
		/// null if <see cref="SetLayout(Graphics,bool,int[],float[])"/> was never called.
		/// </summary>
		internal int[] _countList;

		/// <summary>
		/// private field that stores the row/column size proportional values as specified
		/// to the <see cref="SetLayout(Graphics,bool,int[],float[])"/> method.  This
		/// value will be null if <see cref="SetLayout(Graphics,bool,int[],float[])"/>
		/// was never called.  
		/// </summary>
		internal float[] _prop;

/*		/// <summary>
		/// private field to store the <see cref="PaneLayoutMgr" /> instance, which
		/// manages the persistence and handling of pane layout information.
		/// </summary>
		private PaneLayoutMgr _paneLayoutMgr;
*/
		/// <summary>
		/// private field that determines if anti-aliased drawing will be forced on.  Use the
		/// public property <see cref="IsAntiAlias"/> to access this value.
		/// </summary>
		private bool _isAntiAlias = false;

	#endregion

	#region Defaults
		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="MasterPane"/> class.
		/// </summary>
		public new struct Default
		{
			/// <summary>
			/// The default pane layout for
			/// <see cref="DoLayout(Graphics)"/>
			/// method calls.
			/// </summary>
			/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
			/// <seealso cref="SetLayout(Graphics,int,int)" />
			/// <seealso cref="SetLayout(Graphics,bool,int[])" />
			/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
			/// <seealso cref="ReSize(Graphics,RectangleF)" />
			public static PaneLayout PaneLayout = PaneLayout.SquareColPreferred;

			/// <summary>
			/// The default value for the <see cref="Default.InnerPaneGap"/> property.
			/// This is the size of the margin between adjacent <see cref="GraphPane"/>
			/// objects, in units of points (1/72 inch).
			/// </summary>
			/// <seealso cref="MasterPane.InnerPaneGap"/>
			public static float InnerPaneGap = 10;
			
			/// <summary>
			/// The default value for the <see cref="Legend.IsVisible"/> property for
			/// the <see cref="MasterPane"/> class.
			/// </summary>
			public static bool IsShowLegend = false;
			/// <summary>
			/// The default value for the <see cref="IsUniformLegendEntries"/> property.
			/// </summary>
			public static bool IsUniformLegendEntries = false;
			/// <summary>
			/// The default value for the <see cref="IsCommonScaleFactor"/> property.
			/// </summary>
			public static bool IsCommonScaleFactor = false;
		}
	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets the <see cref="PaneList"/> collection instance that holds the list of
		/// <see cref="GraphPane"/> objects that are included in this <see cref="MasterPane"/>.
		/// </summary>
		/// <seealso cref="Add"/>
		/// <seealso cref="MasterPane.this[int]"/>
		public PaneList PaneList
		{
			get { return _paneList; }
			set { _paneList = value; }
		}
/*
		/// <summary>
		/// Gets the <see cref="PaneLayoutMgr" /> instance, which manages the pane layout
		/// settings, and handles the layout functions.
		/// </summary>
		/// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(PaneLayout)" />
		/// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(int,int)" />
		/// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(bool,int[])" />
		/// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(bool,int[],float[])" />
		/// <seealso cref="ReSize" />
		public PaneLayoutMgr PaneLayoutMgr
		{
			get { return _paneLayoutMgr; }
		}
*/
		/// <summary>
		/// Gets or sets the size of the margin between adjacent <see cref="GraphPane"/>
		/// objects.
		/// </summary>
		/// <remarks>This property is scaled according to <see cref="PaneBase.CalcScaleFactor"/>,
		/// based on <see cref="PaneBase.BaseDimension"/>.  The default value comes from
		/// <see cref="Default.InnerPaneGap"/>.
		/// </remarks>
		/// <value>The value is in points (1/72nd inch).</value>
		public float InnerPaneGap
		{
			get { return _innerPaneGap; }
			set { _innerPaneGap = value; }
		}
		/// <summary>
		/// Gets or set the value of the	 <see cref="IsUniformLegendEntries"/>
		/// </summary>
		public bool IsUniformLegendEntries
		{
			get { return (_isUniformLegendEntries); }
			set { _isUniformLegendEntries = value; } 
		}

		/// <summary>
		/// Gets or sets a value that determines if the
		/// <see cref="DoLayout(Graphics)" /> method will automatically set the
		/// <see cref="PaneBase.BaseDimension" />
		/// of each <see cref="GraphPane" /> in the <see cref="PaneList" /> such that the
		/// scale factors have the same value.
		/// </summary>
		/// <remarks>
		/// The scale factors, calculated by <see cref="PaneBase.CalcScaleFactor" />, determine
		/// scaled font sizes, tic lengths, etc.  This function will insure that for
		/// multiple graphpanes, a certain specified font size will be the same for
		/// all the panes.
		/// </remarks>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		/// <seealso cref="ReSize(Graphics,RectangleF)" />
		public bool IsCommonScaleFactor
		{
			get { return _isCommonScaleFactor; }
			set { _isCommonScaleFactor = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines if all drawing operations for this
		/// <see cref="MasterPane" /> will be forced to operate in Anti-alias mode.
		/// Note that if this value is set to "true", it overrides the setting for sub-objects.
		/// Otherwise, the sub-object settings (such as <see cref="FontSpec.IsAntiAlias"/>)
		/// will be honored.
		/// </summary>
		public bool IsAntiAlias
		{
			get { return _isAntiAlias; }
			set { _isAntiAlias = value; }
		}

	#endregion
	
	#region Constructors

		/// <summary>
		/// Default constructor for the class.  Sets the <see cref="PaneBase.Rect"/> to (0, 0, 500, 375).
		/// </summary>
		public MasterPane() : this( "", new RectangleF( 0, 0, 500, 375 ) )
		{
		}
		
		/// <summary>
		/// Default constructor for the class.  Specifies the <see cref="PaneBase.Title"/> of
		/// the <see cref="MasterPane"/>, and the size of the <see cref="PaneBase.Rect"/>.
		/// </summary>
		public MasterPane( string title, RectangleF paneRect ) : base( title, paneRect )
		{
			_innerPaneGap = Default.InnerPaneGap;

			//_paneLayoutMgr = new PaneLayoutMgr();

			_isUniformLegendEntries = Default.IsUniformLegendEntries ;
			_isCommonScaleFactor = Default.IsCommonScaleFactor;

			_paneList = new PaneList();

			_legend.IsVisible = Default.IsShowLegend;

			_isAntiAlias = false;

			InitLayout();
		}

		private void InitLayout()
		{
			_paneLayout = Default.PaneLayout;
			_countList = null;
			_isColumnSpecified = false;
			_prop = null;
		}

		/// <summary>
		/// The Copy Constructor - Make a deep-copy clone of this class instance.
		/// </summary>
		/// <param name="rhs">The <see cref="MasterPane"/> object from which to copy</param>
		public MasterPane( MasterPane rhs ) : base( rhs )
		{
			// copy all the value types
			//_paneLayoutMgr = rhs._paneLayoutMgr.Clone();
			_innerPaneGap = rhs._innerPaneGap;
			_isUniformLegendEntries = rhs._isUniformLegendEntries;
			_isCommonScaleFactor = rhs._isCommonScaleFactor;

			// Then, fill in all the reference types with deep copies
			_paneList = rhs._paneList.Clone();

			_paneLayout = rhs._paneLayout;
			_countList = rhs._countList;
			_isColumnSpecified = rhs._isColumnSpecified;
			_prop = rhs._prop;
			_isAntiAlias = rhs._isAntiAlias;
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" /> to make a deep copy.
		/// </summary>
		/// <returns>A deep copy of this object</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Typesafe, deep-copy clone method.
		/// </summary>
		/// <returns>A new, independent copy of this class</returns>
		public MasterPane Clone()
		{
			return new MasterPane( this );
		}

	#endregion

	#region Serialization
		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		// schema changed to 2 with addition of 'prop'
		// schema changed to 11 with addition of 'isAntiAlias'
		public const int schema2 = 11;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected MasterPane( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			_paneList = (PaneList) info.GetValue( "paneList", typeof(PaneList) );
			//_paneLayoutMgr = (PaneLayoutMgr) info.GetValue( "paneLayoutMgr", typeof(PaneLayoutMgr) );
			_innerPaneGap = info.GetSingle( "innerPaneGap" );

			_isUniformLegendEntries = info.GetBoolean( "isUniformLegendEntries" );
			_isCommonScaleFactor = info.GetBoolean( "isCommonScaleFactor" );

			_paneLayout = (PaneLayout)info.GetValue( "paneLayout", typeof( PaneLayout ) );
			_countList = (int[])info.GetValue( "countList", typeof( int[] ) );

			_isColumnSpecified = info.GetBoolean( "isColumnSpecified" );
			_prop = (float[])info.GetValue( "prop", typeof( float[] ) );

			if ( sch >= 11 )
				_isAntiAlias = info.GetBoolean( "isAntiAlias" );
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

			info.AddValue( "paneList", _paneList );
			//info.AddValue( "paneLayoutMgr", _paneLayoutMgr );
			info.AddValue( "innerPaneGap", _innerPaneGap );

			info.AddValue( "isUniformLegendEntries", _isUniformLegendEntries );
			info.AddValue( "isCommonScaleFactor", _isCommonScaleFactor );

			info.AddValue( "paneLayout", _paneLayout );
			info.AddValue( "countList", _countList );
			info.AddValue( "isColumnSpecified", _isColumnSpecified );
			info.AddValue( "prop", _prop );

			info.AddValue( "isAntiAlias", _isAntiAlias );
		}

		/// <summary>
		/// Respond to the callback when the MasterPane objects are fully initialized.
		/// </summary>
		/// <param name="sender"></param>
		public void OnDeserialization(object sender)
		{
			Bitmap bitmap = new Bitmap( 10, 10 );
			Graphics g = Graphics.FromImage( bitmap );
			ReSize( g, _rect );
		}
	#endregion

	#region List Methods
		/// <summary>
		/// Indexer to access the specified <see cref="GraphPane"/> object from <see cref="PaneList"/>
		/// by its ordinal position in the list.
		/// </summary>
		/// <param name="index">The ordinal position (zero-based) of the
		/// <see cref="GraphPane"/> object to be accessed.</param>
		/// <value>A <see cref="GraphPane"/> object reference.</value>
		public GraphPane this[ int index ]  
		{
			get { return( (GraphPane) _paneList[index] ); }
			set { _paneList[index] = value; }
		}

		/// <summary>
		/// Indexer to access the specified <see cref="GraphPane"/> object from <see cref="PaneList"/>
		/// by its <see cref="PaneBase.Title"/> string.
		/// </summary>
		/// <param name="title">The string title of the
		/// <see cref="GraphPane"/> object to be accessed.</param>
		/// <value>A <see cref="GraphPane"/> object reference.</value>
		public GraphPane this[ string title ]  
		{
			get { return _paneList[title]; }
		}

		/// <summary>
		/// Add a <see cref="GraphPane"/> object to the <see cref="PaneList"/> collection at the end of the list.
		/// </summary>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object to
		/// be added</param>
		/// <seealso cref="IList.Add"/>
		public void Add( GraphPane pane )
		{
			_paneList.Add( pane );
		}

		/// <summary>
		/// Call <see cref="GraphPane.AxisChange()"/> for all <see cref="GraphPane"/> objects in the
		/// <see cref="PaneList"/> list.
		/// </summary>
		/// <remarks>
		/// This overload of AxisChange just uses the default Graphics instance for the screen.
		/// If you have a Graphics instance available from your Windows Form, you should use
		/// the <see cref="AxisChange(Graphics)" /> overload instead.
		/// </remarks>
		public void AxisChange()
		{
			using ( Graphics g = Graphics.FromHwnd( IntPtr.Zero ) )
				AxisChange( g );
		}

		/// <summary>
		/// Call <see cref="GraphPane.AxisChange()"/> for all <see cref="GraphPane"/> objects in the
		/// <see cref="PaneList"/> list.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		public void AxisChange( Graphics g )
		{
			foreach ( GraphPane pane in _paneList )
				pane.AxisChange( g );
		}

		/// <summary>
		/// Redo the layout using the current size of the <see cref="PaneBase.Rect"/>,
		/// and also handle resizing the
		/// contents by calling <see cref="DoLayout(Graphics)"/>.
		/// </summary>
		/// <remarks>This method will use the pane layout that was specified by a call to
		/// <see cref="SetLayout(Graphics,PaneLayout)"/>.  If
		/// <see cref="SetLayout(Graphics,PaneLayout)"/> has not previously been called,
		/// it will default to <see cref="Default.PaneLayout"/>.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public void ReSize( Graphics g )
		{
			ReSize( g, _rect );
		}

		/// <summary>
		/// Change the size of the <see cref="PaneBase.Rect"/>, and also handle resizing the
		/// contents by calling <see cref="DoLayout(Graphics)"/>.
		/// </summary>
		/// <remarks>This method will use the pane layout that was specified by a call to
		/// <see cref="SetLayout(Graphics,PaneLayout)"/>.  If
		/// <see cref="SetLayout(Graphics,PaneLayout)"/> has not previously been called,
		/// it will default to <see cref="Default.PaneLayout"/>.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="rect"></param>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public override void ReSize( Graphics g, RectangleF rect )
		{
			_rect = rect;
			DoLayout( g );
			CommonScaleFactor();
		}

		/// <summary>
		/// Method that forces the scale factor calculations
		/// (via <see cref="PaneBase.CalcScaleFactor" />),
		/// to give a common scale factor for all <see cref="GraphPane" /> objects in the
		/// <see cref="PaneList" />.
		/// </summary>
		/// <remarks>
		/// This will make it such that a given font size will result in the same output font
		/// size for all <see cref="GraphPane" />'s.  Note that this does not make the scale
		/// factor for the <see cref="GraphPane" />'s the same as that of the
		/// <see cref="MasterPane" />.
		/// </remarks>
		/// <seealso cref="IsCommonScaleFactor" />
		public void CommonScaleFactor()
		{
			if ( _isCommonScaleFactor )
			{
				// Find the maximum scaleFactor of all the GraphPanes
				float maxFactor = 0;
				foreach ( GraphPane pane in PaneList )
				{
					pane.BaseDimension = PaneBase.Default.BaseDimension;
					float scaleFactor = pane.CalcScaleFactor();
					maxFactor = scaleFactor > maxFactor ? scaleFactor : maxFactor;
				}

				// Now, calculate the base dimension
				foreach ( GraphPane pane in PaneList )
				{
					float scaleFactor = pane.CalcScaleFactor();
					pane.BaseDimension *= scaleFactor / maxFactor;
				}
			}
		}

		/// <summary>
		/// Render all the <see cref="GraphPane"/> objects in the <see cref="PaneList"/> to the
		/// specified graphics device.
		/// </summary>
		/// <remarks>This method should be part of the Paint() update process.  Calling this routine
		/// will redraw all
		/// features of all the <see cref="GraphPane"/> items.  No preparation is required other than
		/// instantiated <see cref="GraphPane"/> objects that have been added to the list with the
		/// <see cref="Add"/> method.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		public override void Draw( Graphics g )
		{
			// Save current AntiAlias mode
			SmoothingMode sModeSave = g.SmoothingMode;
			TextRenderingHint sHintSave = g.TextRenderingHint;
			CompositingQuality sCompQual = g.CompositingQuality;
			InterpolationMode sInterpMode = g.InterpolationMode;

			SetAntiAliasMode( g, _isAntiAlias );

			// Draw the pane border & background fill, the title, and the GraphObj objects that lie at
			// ZOrder.GBehindAll
			base.Draw( g );

			if ( _rect.Width <= 1 || _rect.Height <= 1 )
				return;

			float scaleFactor = CalcScaleFactor();

			// Clip everything to the rect
			g.SetClip( _rect );

			// For the MasterPane, All GraphItems go behind the GraphPanes, except those that
			// are explicity declared as ZOrder.AInFront
			_graphObjList.Draw( g, this, scaleFactor, ZOrder.G_BehindChartFill );
			_graphObjList.Draw( g, this, scaleFactor, ZOrder.E_BehindCurves );
			_graphObjList.Draw( g, this, scaleFactor, ZOrder.D_BehindAxis );
			_graphObjList.Draw( g, this, scaleFactor, ZOrder.C_BehindChartBorder );

			// Reset the clipping
			g.ResetClip();

			foreach ( GraphPane pane in _paneList )
				pane.Draw( g );

			// Clip everything to the rect
			g.SetClip( _rect );

			_graphObjList.Draw( g, this, scaleFactor, ZOrder.B_BehindLegend );

			// Recalculate the legend rect, just in case it has not yet been done
			// innerRect is the area for the GraphPane's
			RectangleF innerRect = CalcClientRect( g, scaleFactor );
			_legend.CalcRect( g, this, scaleFactor, ref innerRect );
			//this.legend.SetLocation( this, 
			
			_legend.Draw( g, this, scaleFactor );

			_graphObjList.Draw( g, this, scaleFactor, ZOrder.A_InFront );
			
			// Reset the clipping
			g.ResetClip();

			// Restore original anti-alias mode
			g.SmoothingMode = sModeSave;
			g.TextRenderingHint = sHintSave;
			g.CompositingQuality = sCompQual;
			g.InterpolationMode = sInterpMode;

		}

		/// <summary>
		/// Find the pane and the object within that pane that lies closest to the specified
		/// mouse (screen) point.
		/// </summary>
		/// <remarks>
		/// This method first finds the <see cref="GraphPane"/> within the list that contains
		/// the specified mouse point.  It then calls the <see cref="GraphPane.FindNearestObject"/>
		/// method to determine which object, if any, was clicked.  With the exception of the
		/// <see paramref="pane"/>, all the parameters in this method are identical to those
		/// in the <see cref="GraphPane.FindNearestObject"/> method.
		/// If the mouse point lies within the <see cref="PaneBase.Rect"/> of any 
		/// <see cref="GraphPane"/> item, then that pane will be returned (otherwise it will be
		/// null).  Further, within the selected pane, if the mouse point is within the
		/// bounding box of any of the items (or in the case
		/// of <see cref="ArrowObj"/> and <see cref="CurveItem"/>, within
		/// <see cref="GraphPane.Default.NearestTol"/> pixels), then the object will be returned.
		/// You must check the type of the object to determine what object was
		/// selected (for example, "if ( object is Legend ) ...").  The
		/// <see paramref="index"/> parameter returns the index number of the item
		/// within the selected object (such as the point number within a
		/// <see cref="CurveItem"/> object.
		/// </remarks>
		/// <param name="mousePt">The screen point, in pixel coordinates.</param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object that was clicked.</param>
		/// <param name="nearestObj">A reference to the nearest object to the
		/// specified screen point.  This can be any of <see cref="Axis"/>,
		/// <see cref="Legend"/>, <see cref="PaneBase.Title"/>,
		/// <see cref="TextObj"/>, <see cref="ArrowObj"/>, or <see cref="CurveItem"/>.
		/// Note: If the pane title is selected, then the <see cref="GraphPane"/> object
		/// will be returned.
		/// </param>
		/// <param name="index">The index number of the item within the selected object
		/// (where applicable).  For example, for a <see cref="CurveItem"/> object,
		/// <see paramref="index"/> will be the index number of the nearest data point,
		/// accessible via <see cref="CurveItem.Points">CurveItem.Points[index]</see>.
		/// index will be -1 if no data points are available.</param>
		/// <returns>true if a <see cref="GraphPane"/> was found, false otherwise.</returns>
		/// <seealso cref="GraphPane.FindNearestObject"/>
		public bool FindNearestPaneObject( PointF mousePt, Graphics g, out GraphPane pane,
			out object nearestObj, out int index )
		{
			pane = null;
			nearestObj = null;
			index = -1;

			GraphObj	saveGraphItem = null;
			int			saveIndex = -1;
			float		scaleFactor = CalcScaleFactor();

			// See if the point is in a GraphObj
			// If so, just save the object and index so we can see if other overlying objects were
			// intersected as well.
			if ( this.GraphObjList.FindPoint( mousePt, this, g, scaleFactor, out index ) )
			{
				saveGraphItem = this.GraphObjList[index];
				saveIndex = index;

				// If it's an "In-Front" item, then just return it
				if ( saveGraphItem.ZOrder == ZOrder.A_InFront )
				{
					nearestObj = saveGraphItem;
					index = saveIndex;
					return true;
				}
			}

			foreach ( GraphPane tPane in _paneList )
			{
				if ( tPane.Rect.Contains( mousePt ) )
				{
					pane = tPane;
					return tPane.FindNearestObject( mousePt, g, out nearestObj, out index );
				}
			}

			// If no items were found in the GraphPanes, then return the item found on the MasterPane (if any)
			if ( saveGraphItem != null )
			{
				nearestObj = saveGraphItem;
				index = saveIndex;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Find the <see cref="GraphPane"/> within the <see cref="PaneList"/> that contains the
		/// <see paramref="mousePt"/> within its <see cref="PaneBase.Rect"/>.
		/// </summary>
		/// <param name="mousePt">The mouse point location where you want to search</param>
		/// <returns>A <see cref="GraphPane"/> object that contains the mouse point, or
		/// null if no <see cref="GraphPane"/> was found.</returns>
		public GraphPane FindPane( PointF mousePt )
		{
			foreach ( GraphPane pane in _paneList )
			{
				if ( pane.Rect.Contains( mousePt ) )
					return pane;
			}
			
			return null;
		}

		/// <summary>
		/// Find the <see cref="GraphPane"/> within the <see cref="PaneList"/> that contains the
		/// <see paramref="mousePt"/> within its <see cref="Chart.Rect"/>.
		/// </summary>
		/// <param name="mousePt">The mouse point location where you want to search</param>
		/// <returns>A <see cref="GraphPane"/> object that contains the mouse point, or
		/// null if no <see cref="GraphPane"/> was found.</returns>
		public GraphPane FindChartRect( PointF mousePt )
		{
			foreach ( GraphPane pane in _paneList )
			{
				if ( pane.Chart._rect.Contains( mousePt ) )
					return pane;
			}
			
			return null;
		}

	#endregion

	#region Layout Methods

		/// <overloads>The SetLayout() methods setup the desired layout of the
		/// <see cref="GraphPane" /> objects within a <see cref="MasterPane" />.  These functions
		/// do not make any changes, they merely set the parameters so that future calls
		/// to <see cref="PaneBase.ReSize" /> or <see cref="DoLayout(Graphics)" />
		/// will use the desired layout.<br /><br />
		/// The layout options include a set of "canned" layouts provided by the
		/// <see cref="ZedGraph.PaneLayout" /> enumeration, options to just set a specific
		/// number of rows and columns of panes (and all pane sizes are the same), and more
		/// customized options of specifying the number or rows in each column or the number of
		/// columns in each row, along with proportional values that determine the size of each
		/// individual column or row.
		/// </overloads>
		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to a pre-defined layout configuration from a <see cref="PaneLayout" />
		/// enumeration.
		/// </summary>
		/// <remarks>This method uses a <see cref="PaneLayout"/> enumeration to describe the type of layout
		/// to be used.  Overloads are available that provide other layout options</remarks>
		/// <param name="paneLayout">A <see cref="PaneLayout"/> enumeration that describes how
		/// the panes should be laid out within the <see cref="PaneBase.Rect"/>.</param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally created with a call to
		/// the CreateGraphics() method of the Control or Form.
		/// </param>
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public void SetLayout( Graphics g, PaneLayout paneLayout )
		{
			InitLayout();

			_paneLayout = paneLayout;

			DoLayout( g );
		}

		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to a reasonable configuration.
		/// </summary>
		/// <remarks>This method explicitly specifies the number of rows and columns to use
		/// in the layout, and all <see cref="GraphPane" /> objects will have the same size.
		/// Overloads are available that provide other layout options</remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally created with a call to
		/// the CreateGraphics() method of the Control or Form.
		/// </param>
		/// <param name="rows">The number of rows of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		/// <param name="columns">The number of columns of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public void SetLayout( Graphics g, int rows, int columns )
		{
			InitLayout();

			if ( rows < 1 )
				rows = 1;
			if ( columns < 1 )
				columns = 1;

			int[] countList = new int[rows];

			for ( int i = 0; i < rows; i++ )
				countList[i] = columns;

			SetLayout( g, true, countList, null );
		}

		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to the specified configuration.
		/// </summary>
		/// <remarks>This method specifies the number of rows in each column, or the number of
		/// columns in each row, allowing for irregular layouts.  Overloads are available that
		/// provide other layout options.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally created with a call to
		/// the CreateGraphics() method of the Control or Form.
		/// </param>
		/// <param name="isColumnSpecified">Specifies whether the number of columns in each row, or
		/// the number of rows in each column will be specified.  A value of true indicates the
		/// number of columns in each row are specified in <see paramref="countList"/>.</param>
		/// <param name="countList">An integer array specifying either the number of columns in
		/// each row or the number of rows in each column, depending on the value of
		/// <see paramref="isColumnSpecified"/>.</param>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public void SetLayout( Graphics g, bool isColumnSpecified, int[] countList )
		{
			SetLayout( g, isColumnSpecified, countList, null );
		}

		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to the specified configuration.
		/// </summary>
		/// <remarks>This method specifies the number of panes in each row or column, allowing for
		/// irregular layouts.</remarks>
		/// <remarks>This method specifies the number of rows in each column, or the number of
		/// columns in each row, allowing for irregular layouts.  Additionally, a
		/// <see paramref="proportion" /> parameter is provided that allows varying column or
		/// row sizes.  Overloads for SetLayout() are available that provide other layout options.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally created with a call to
		/// the CreateGraphics() method of the Control or Form.
		/// </param>
		/// <param name="isColumnSpecified">Specifies whether the number of columns in each row, or
		/// the number of rows in each column will be specified.  A value of true indicates the
		/// number of columns in each row are specified in <see paramref="_countList"/>.</param>
		/// <param name="countList">An integer array specifying either the number of columns in
		/// each row or the number of rows in each column, depending on the value of
		/// <see paramref="isColumnSpecified"/>.</param>
		/// <param name="proportion">An array of float values specifying proportional sizes for each
		/// row or column.  Note that these proportions apply to the non-specified dimension -- that is,
		/// if <see paramref="isColumnSpecified"/> is true, then these proportions apply to the row
		/// heights, and if <see paramref="isColumnSpecified"/> is false, then these proportions apply
		/// to the column widths.  The values in this array are arbitrary floats -- the dimension of
		/// any given row or column is that particular proportional value divided by the sum of all
		/// the values.  For example, let <see paramref="isColumnSpecified"/> be true, and
		/// <see paramref="proportion"/> is an array with values of { 1.0, 2.0, 3.0 }.  The sum of
		/// those values is 6.0.  Therefore, the first row is 1/6th of the available height, the
		/// second row is 2/6th's of the available height, and the third row is 3/6th's of the
		/// available height.
		/// </param>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		public void SetLayout( Graphics g, bool isColumnSpecified, int[] countList, float[] proportion )
		{
			InitLayout();

			// use defaults if the parameters are invalid
			if ( countList != null && countList.Length > 0 )
			{
				_prop = new float[countList.Length];

				// Sum up the total proportional factors
				float sumProp = 0.0f;
				for ( int i = 0; i < countList.Length; i++ )
				{
					_prop[i] = ( proportion == null || proportion.Length <= i || proportion[i] < 1e-10 ) ?
												1.0f : proportion[i];
					sumProp += _prop[i];
				}

				// Make prop sum to 1.0
				for ( int i = 0; i < countList.Length; i++ )
					_prop[i] /= sumProp;

				_isColumnSpecified = isColumnSpecified;
				_countList = countList;

				DoLayout( g );
			}
		}

		/// <summary>
		/// Modify the <see cref="GraphPane" /> <see cref="PaneBase.Rect" /> sizes of each
		/// <see cref="GraphPane" /> such that they fit within the <see cref="MasterPane" />
		/// in a pre-configured layout.
		/// </summary>
		/// <remarks>The <see cref="SetLayout(Graphics,PaneLayout)" /> method (and overloads) is
		/// used for setting the layout configuration.</remarks>
		/// <seealso cref="SetLayout(Graphics,PaneLayout)" />
		/// <seealso cref="SetLayout(Graphics,int,int)" />
		/// <seealso cref="SetLayout(Graphics,bool,int[])" />
		/// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
		public void DoLayout( Graphics g )
		{
			if ( _countList != null )
				DoLayout( g, _isColumnSpecified, _countList, _prop );
			else
			{
				int count = _paneList.Count;
				if ( count == 0 )
					return;

				int rows,
						cols,
						root = (int)( Math.Sqrt( (double)count ) + 0.9999999 );

				//float[] widthList = new float[5];

				switch ( _paneLayout )
				{
					case PaneLayout.ForceSquare:
						rows = root;
						cols = root;
						DoLayout( g, rows, cols );
						break;
					case PaneLayout.SingleColumn:
						rows = count;
						cols = 1;
						DoLayout( g, rows, cols );
						break;
					case PaneLayout.SingleRow:
						rows = 1;
						cols = count;
						DoLayout( g, rows, cols );
						break;
					default:
					case PaneLayout.SquareColPreferred:
						rows = root;
						cols = root;
						if ( count <= root * ( root - 1 ) )
							rows--;
						DoLayout( g, rows, cols );
						break;
					case PaneLayout.SquareRowPreferred:
						rows = root;
						cols = root;
						if ( count <= root * ( root - 1 ) )
							cols--;
						DoLayout( g, rows, cols );
						break;
					case PaneLayout.ExplicitCol12:
						DoLayout( g, true, new int[2] { 1, 2 }, null );
						break;
					case PaneLayout.ExplicitCol21:
						DoLayout( g, true, new int[2] { 2, 1 }, null );
						break;
					case PaneLayout.ExplicitCol23:
						DoLayout( g, true, new int[2] { 2, 3 }, null );
						break;
					case PaneLayout.ExplicitCol32:
						DoLayout( g, true, new int[2] { 3, 2 }, null );
						break;
					case PaneLayout.ExplicitRow12:
						DoLayout( g, false, new int[2] { 1, 2 }, null );
						break;
					case PaneLayout.ExplicitRow21:
						DoLayout( g, false, new int[2] { 2, 1 }, null );
						break;
					case PaneLayout.ExplicitRow23:
						DoLayout( g, false, new int[2] { 2, 3 }, null );
						break;
					case PaneLayout.ExplicitRow32:
						DoLayout( g, false, new int[2] { 3, 2 }, null );
						break;
				}
			}
		}

		/// <summary>
		/// Internal method that applies a previously set layout with a specific
		/// row and column count.  This method is only called by
		/// <see cref="DoLayout(Graphics)" />.
		/// </summary>
		internal void DoLayout( Graphics g, int rows, int columns )
		{
			if ( rows < 1 )
				rows = 1;
			if ( columns < 1 )
				columns = 1;

			int[] countList = new int[rows];

			for ( int i = 0; i < rows; i++ )
				countList[i] = columns;

			DoLayout( g, true, countList, null );
		}

		/// <summary>
		/// Internal method that applies a previously set layout with a rows per column or
		/// columns per row configuration.  This method is only called by
		/// <see cref="DoLayout(Graphics)" />.
		/// </summary>
		internal void DoLayout( Graphics g, bool isColumnSpecified, int[] countList,
					float[] proportion )
		{

			// calculate scaleFactor on "normal" pane size (BaseDimension)
			float scaleFactor = CalcScaleFactor();

			// innerRect is the area for the GraphPane's
			RectangleF innerRect = CalcClientRect( g, scaleFactor );
			_legend.CalcRect( g, this, scaleFactor, ref innerRect );

			// scaled InnerGap is the area between the GraphPane.Rect's
			float scaledInnerGap = (float)( _innerPaneGap * scaleFactor );

			int iPane = 0;

			if ( isColumnSpecified )
			{
				int rows = countList.Length;

				float y = 0.0f;

				for ( int rowNum = 0; rowNum < rows; rowNum++ )
				{
					float propFactor = _prop == null ? 1.0f / rows : _prop[rowNum];

					float height = ( innerRect.Height - (float)( rows - 1 ) * scaledInnerGap ) *
									propFactor;

					int columns = countList[rowNum];
					if ( columns <= 0 )
						columns = 1;
					float width = ( innerRect.Width - (float)( columns - 1 ) * scaledInnerGap ) /
									(float)columns;

					for ( int colNum = 0; colNum < columns; colNum++ )
					{
						if ( iPane >= _paneList.Count )
							return;

						this[iPane].Rect = new RectangleF(
											innerRect.X + colNum * ( width + scaledInnerGap ),
											innerRect.Y + y,
											width,
											height );
						iPane++;
					}

					y += height + scaledInnerGap;
				}
			}
			else
			{
				int columns = countList.Length;

				float x = 0.0f;

				for ( int colNum = 0; colNum < columns; colNum++ )
				{
					float propFactor = _prop == null ? 1.0f / columns : _prop[colNum];

					float width = ( innerRect.Width - (float)( columns - 1 ) * scaledInnerGap ) *
									propFactor;

					int rows = countList[colNum];
					if ( rows <= 0 )
						rows = 1;
					float height = ( innerRect.Height - (float)( rows - 1 ) * scaledInnerGap ) / (float)rows;

					for ( int rowNum = 0; rowNum < rows; rowNum++ )
					{
						if ( iPane >= _paneList.Count )
							return;

						this[iPane].Rect = new RectangleF(
											innerRect.X + x,
											innerRect.Y + rowNum * ( height + scaledInnerGap ),
											width,
											height );
						iPane++;
					}

					x += width + scaledInnerGap;
				}
			}
		}

		/*
		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to a reasonable configuration.
		/// </summary>
		/// <remarks>This method explicitly specifies the number of rows and columns to use in the layout.
		/// A more automatic overload, using a <see cref="PaneLayout"/> enumeration, is available.</remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="rows">The number of rows of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		/// <param name="columns">The number of columns of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		public void DoPaneLayout( Graphics g, int rows, int columns )
		{
			// save the layout settings for future reference
			_countList = null;
			_rows = rows;
			_columns = columns;

			// calculate scaleFactor on "normal" pane size (BaseDimension)
			float scaleFactor = this.CalcScaleFactor();

			// innerRect is the area for the GraphPane's
			RectangleF innerRect = CalcClientRect( g, scaleFactor );
			_legend.CalcRect( g, this, scaleFactor, ref innerRect );

			// scaled InnerGap is the area between the GraphPane.Rect's
			float scaledInnerGap = (float)( _innerPaneGap * scaleFactor );

			float width = ( innerRect.Width - (float)( columns - 1 ) * scaledInnerGap ) / (float)columns;
			float height = ( innerRect.Height - (float)( rows - 1 ) * scaledInnerGap ) / (float)rows;

			int i = 0;
			foreach ( GraphPane pane in _paneList )
			{
				float rowNum = (float)( i / columns );
				float colNum = (float)( i % columns );

				pane.Rect = new RectangleF(
									innerRect.X + colNum * ( width + scaledInnerGap ),
									innerRect.Y + rowNum * ( height + scaledInnerGap ),
									width,
									height );

				i++;
			}
		}
		*/
	#endregion

	}
}
