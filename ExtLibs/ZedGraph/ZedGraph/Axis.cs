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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// The Axis class is an abstract base class that encompasses all properties
	/// and methods required to define a graph Axis.
	/// </summary>
	/// <remarks>This class is inherited by the
	/// <see cref="XAxis"/>, <see cref="YAxis"/>, and <see cref="Y2Axis"/> classes
	/// to define specific characteristics for those types.
	/// </remarks>
	/// 
	/// <author> John Champion modified by Jerry Vos </author>
	/// <version> $Revision: 3.76 $ $Date: 2008-02-16 23:21:48 $ </version>
	[Serializable]
	abstract public class Axis : ISerializable, ICloneable
	{

	#region Class Fields

		/// <summary>
		/// private field that stores the <see cref="ZedGraph.Scale" /> class, which implements all the
		/// calculations and methods associated with the numeric scale for this
		/// <see cref="Axis" />.  See the public property <see cref="Scale" /> to access this class.
		/// </summary>
		internal Scale _scale;

		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.MinorTic" /> class, which handles all
		/// the minor tic information.  See the public property <see cref="MinorTic" /> to access this class.
		/// </summary>
		internal MinorTic _minorTic;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.MajorTic" /> class, which handles all
		/// the major tic information.  See the public property <see cref="MajorTic" /> to access this class.
		/// </summary>
		internal MajorTic _majorTic;

		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.MajorGrid" /> class, which handles all
		/// the major grid information.  See the public property <see cref="MajorGrid" /> to access this class.
		/// </summary>
		internal MajorGrid	_majorGrid;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.MinorGrid" /> class, which handles all
		/// the minor grid information.  See the public property <see cref="MinorGrid" /> to access this class.
		/// </summary>
		internal MinorGrid _minorGrid;

		/// <summary> Private fields for the <see cref="Axis"/> scale rendering properties.
		/// Use the public properties <see cref="Cross"/> and <see cref="ZedGraph.Scale.BaseTic"/>
		/// for access to these values.
		/// </summary>
		internal double _cross;

		/// <summary> Private field for the <see cref="Axis"/> automatic cross position mode.
		/// Use the public property <see cref="CrossAuto"/> for access to this value.
		/// </summary>
		internal bool _crossAuto;

		/// <summary> Private fields for the <see cref="Axis"/> attributes.
		/// Use the public properties <see cref="IsVisible"/>, <see cref="IsAxisSegmentVisible"/>
		/// for access to these values.
		/// </summary>
		protected bool _isVisible,
							_isAxisSegmentVisible;

		/// <summary> Private field for the <see cref="Axis"/> title string.
		/// Use the public property <see cref="Title"/> for access to this value.
		/// </summary>
		protected AxisLabel _title;

		/// <summary>
		/// A tag object for use by the user.  This can be used to store additional
		/// information associated with the <see cref="Axis"/>.  ZedGraph does
		/// not use this value for any purpose.
		/// </summary>
		/// <remarks>
		/// Note that, if you are going to Serialize ZedGraph data, then any type
		/// that you store in <see cref="Tag"/> must be a serializable type (or
		/// it will cause an exception).
		/// </remarks>
		public object Tag;

		/// <summary> Private field for the <see cref="Axis"/> drawing dimensions.
		/// Use the public property <see cref="AxisGap"/>
		/// for access to these values. </summary>
		private float	_axisGap;

		/// <summary>
		/// Private field for the <see cref="Axis"/> minimum allowable space allocation.
		/// Use the public property <see cref="MinSpace"/> to access this value.
		/// </summary>
		/// <seealso cref="Default.MinSpace"/>
		private float _minSpace;

		/// <summary> Private fields for the <see cref="Axis"/> colors.
		/// Use the public property <see cref="Color"/> for access to this values.
		/// </summary>
		private Color _color;

		/// <summary>
		/// Temporary values for axis space calculations (see <see cref="CalcSpace" />).
		/// </summary>
		internal float _tmpSpace;

	#endregion

	#region Events

		/// <summary>
		/// A delegate that allows full custom formatting of the Axis labels
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane" /> for which the label is to be
		/// formatted</param>
		/// <param name="axis">The <see cref="Scale" /> of interest.</param>
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

		// Revision: JCarpenter 10/06
		/// <summary>
		/// Allow customization of title based on user preferences.
		/// </summary>
		/// <param name="axis">The <see cref="Axis" /> of interest.</param>
		/// <returns>
		/// A string value representing the label, or null if the ZedGraph should go ahead
		/// and generate the label according to the current settings.  To make the title
		/// blank, return "".</returns>
		/// <seealso cref="ScaleFormatEvent" />
		public delegate string ScaleTitleEventHandler( Axis axis );

		//Revision: JCarpenter 10/06
		/// <summary>
		/// Allow customization of the title when the scale is very large
		/// Subscribe to this event to handle custom formatting of the scale axis label.
		/// </summary>
		public event ScaleTitleEventHandler ScaleTitleEvent;

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="Axis"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default size for the gap between multiple axes
			/// (<see cref="Axis.AxisGap"/> property). Units are in points (1/72 inch).
			/// </summary>
			public static float AxisGap = 5;

			/// <summary>
			/// The default setting for the gap between the scale labels and the axis title.
			/// </summary>
			public static float TitleGap = 0.0f;

			/// <summary>
			/// The default font family for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.Family"/> property).
			/// </summary>
			public static string TitleFontFamily = "Arial";
			/// <summary>
			/// The default font size for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.Size"/> property).  Units are
			/// in points (1/72 inch).
			/// </summary>
			public static float TitleFontSize = 14;
			/// <summary>
			/// The default font color for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.FontColor"/> property).
			/// </summary>
			public static Color TitleFontColor = Color.Black;
			/// <summary>
			/// The default font bold mode for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.IsBold"/> property). true
			/// for a bold typeface, false otherwise.
			/// </summary>
			public static bool TitleFontBold = true;
			/// <summary>
			/// The default font italic mode for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.IsItalic"/> property). true
			/// for an italic typeface, false otherwise.
			/// </summary>
			public static bool TitleFontItalic = false;
			/// <summary>
			/// The default font underline mode for the <see cref="Axis"/> <see cref="Title" /> text
			/// font specification <see cref="FontSpec"/>
			/// (<see cref="FontSpec.IsUnderline"/> property). true
			/// for an underlined typeface, false otherwise.
			/// </summary>
			public static bool TitleFontUnderline = false;
			/// <summary>
			/// The default color for filling in the <see cref="Title" /> text background
			/// (see <see cref="ZedGraph.Fill.Color"/> property).
			/// </summary>
			public static Color TitleFillColor = Color.White;
			/// <summary>
			/// The default custom brush for filling in the <see cref="Title" /> text background
			/// (see <see cref="ZedGraph.Fill.Brush"/> property).
			/// </summary>
			public static Brush TitleFillBrush = null;
			/// <summary>
			/// The default fill mode for filling in the <see cref="Title" /> text background
			/// (see <see cref="ZedGraph.Fill.Type"/> property).
			/// </summary>
			public static FillType TitleFillType = FillType.None;

			/// <summary>
			/// The default color for the <see cref="Axis"/> itself
			/// (<see cref="Axis.Color"/> property).  This color only affects the
			/// the axis border.
			/// </summary>
			public static Color BorderColor = Color.Black;
			/// <summary>
			/// The default value for <see cref="Axis.IsAxisSegmentVisible"/>, which determines
			/// whether or not the scale segment itself is visible
			/// </summary>
			public static bool IsAxisSegmentVisible = true;

			/// <summary>
			/// The default setting for the <see cref="Axis"/> scale axis type
			/// (<see cref="Axis.Type"/> property).  This value is set as per
			/// the <see cref="AxisType"/> enumeration
			/// </summary>
			public static AxisType Type = AxisType.Linear;

			/// <summary>
			/// The default color for the axis segment.
			/// </summary>
			public static Color Color = Color.Black;

			/// <summary>
			/// The default setting for the axis space allocation.  This term, expressed in
			/// points (1/72 inch) and scaled according to <see cref="PaneBase.CalcScaleFactor"/> for the
			/// <see cref="GraphPane"/>, determines the minimum amount of space an axis must
			/// have between the <see cref="Chart.Rect"/> and the
			/// <see cref="PaneBase.Rect"/>.  This minimum space
			/// applies whether <see cref="Axis.IsVisible"/> is true or false.
			/// </summary>
			public static float MinSpace = 0f;
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Default constructor for <see cref="Axis"/> that sets all axis properties
		/// to default values as defined in the <see cref="Default"/> class.
		/// </summary>
		public Axis()
		{
			_scale = new LinearScale( this );

			_cross = 0.0;

			_crossAuto = true;

			_majorTic = new MajorTic();
			_minorTic = new MinorTic();

			_majorGrid = new MajorGrid();
			_minorGrid = new MinorGrid();

			_axisGap = Default.AxisGap;

			_minSpace = Default.MinSpace;
			_isVisible = true;

			_isAxisSegmentVisible = Default.IsAxisSegmentVisible;

			_title = new AxisLabel( "", Default.TitleFontFamily, Default.TitleFontSize,
					Default.TitleFontColor, Default.TitleFontBold,
					Default.TitleFontUnderline, Default.TitleFontItalic );
			_title.FontSpec.Fill = new Fill( Default.TitleFillColor, Default.TitleFillBrush,
					Default.TitleFillType );

			_title.FontSpec.Border.IsVisible = false;


			_color = Default.Color;

		}

		/// <summary>
		/// Constructor for <see cref="Axis"/> that sets all axis properties
		/// to default values as defined in the <see cref="Default"/> class,
		/// except for the <see cref="Title"/>.
		/// </summary>
		/// <param name="title">A string containing the axis title</param>
		public Axis( string title )
			: this()
		{
			_title._text = title;
		}

		/// <summary>
		/// The Copy Constructor.
		/// </summary>
		/// <param name="rhs">The Axis object from which to copy</param>
		public Axis( Axis rhs )
		{
			_scale = rhs._scale.Clone( this );

			_cross = rhs._cross;

			_crossAuto = rhs._crossAuto;

			_majorTic = rhs.MajorTic.Clone();
			_minorTic = rhs.MinorTic.Clone();

			_majorGrid = rhs._majorGrid.Clone();
			_minorGrid = rhs._minorGrid.Clone();

			_isVisible = rhs.IsVisible;

			_isAxisSegmentVisible = rhs._isAxisSegmentVisible;

			_title = (AxisLabel) rhs.Title.Clone();

			_axisGap = rhs._axisGap;

			_minSpace = rhs.MinSpace;

			_color = rhs.Color;
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of Clone.
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

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected Axis( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_cross = info.GetDouble( "cross" );
			_crossAuto = info.GetBoolean( "crossAuto" );

			_majorTic = (MajorTic)info.GetValue( "MajorTic", typeof( MajorTic ) );
			_minorTic = (MinorTic)info.GetValue( "MinorTic", typeof( MinorTic ) );
			_majorGrid = (MajorGrid)info.GetValue( "majorGrid", typeof( MajorGrid ) );
			_minorGrid = (MinorGrid)info.GetValue( "minorGrid", typeof( MinorGrid ) );

			_isVisible = info.GetBoolean( "isVisible" );

			_title = (AxisLabel) info.GetValue( "title", typeof( AxisLabel ) );

			_minSpace = info.GetSingle( "minSpace" );

			_color = (Color)info.GetValue( "color", typeof( Color ) );

			_isAxisSegmentVisible = info.GetBoolean( "isAxisSegmentVisible" );


			_axisGap = info.GetSingle( "axisGap" );

			_scale = (Scale)info.GetValue( "scale", typeof( Scale ) );
			_scale._ownerAxis = this;

		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );

			info.AddValue( "cross", _cross );
			info.AddValue( "crossAuto", _crossAuto );

			info.AddValue( "MajorTic", MajorTic );
			info.AddValue( "MinorTic", MinorTic );
			info.AddValue( "majorGrid", _majorGrid );
			info.AddValue( "minorGrid", _minorGrid );

			info.AddValue( "isVisible", _isVisible );

			info.AddValue( "title", _title );

			info.AddValue( "minSpace", _minSpace );

			info.AddValue( "color", _color );

			info.AddValue( "isAxisSegmentVisible", _isAxisSegmentVisible );

			info.AddValue( "axisGap", _axisGap );

			info.AddValue( "scale", _scale );

		}

	#endregion

	#region Scale Properties

		/// <summary>
		/// Gets the <see cref="Scale" /> instance associated with this <see cref="Axis" />.
		/// </summary>
		public Scale Scale
		{
			get { return _scale; }
		}
		/// <summary>
		/// Gets or sets the scale value at which this axis should cross the "other" axis.
		/// </summary>
		/// <remarks>This property allows the axis to be shifted away from its default location.
		/// For example, for a graph with an X range from -100 to +100, the Y Axis can be located
		/// at the X=0 value rather than the left edge of the ChartRect.  This value can be set
		/// automatically based on the state of <see cref="CrossAuto"/>.  If
		/// this value is set manually, then <see cref="CrossAuto"/> will
		/// also be set to false.  The "other" axis is the axis the handles the second dimension
		/// for the graph.  For the XAxis, the "other" axis is the YAxis.  For the YAxis or
		/// Y2Axis, the "other" axis is the XAxis.
		/// </remarks>
		/// <value> The value is defined in user scale units </value>
		/// <seealso cref="ZedGraph.Scale.Min"/>
		/// <seealso cref="ZedGraph.Scale.Max"/>
		/// <seealso cref="ZedGraph.Scale.MajorStep"/>
		/// <seealso cref="CrossAuto"/>
		public double Cross
		{
			get { return _cross; }
			set { _cross = value; _crossAuto = false; }
		}
		/// <summary>
		/// Gets or sets a value that determines whether or not the <see cref="Cross"/> value
		/// is set automatically.
		/// </summary>
		/// <value>Set to true to have ZedGraph put the axis in the default location, or false
		/// to specify the axis location manually with a <see cref="Cross"/> value.</value>
		/// <seealso cref="ZedGraph.Scale.Min"/>
		/// <seealso cref="ZedGraph.Scale.Max"/>
		/// <seealso cref="ZedGraph.Scale.MajorStep"/>
		/// <seealso cref="Cross"/>
		public bool CrossAuto
		{
			get { return _crossAuto; }
			set { _crossAuto = value; }
		}

		/// <summary>
		/// Gets or sets the minimum axis space allocation.
		/// </summary>
		/// <remarks>
		/// This term, expressed in
		/// points (1/72 inch) and scaled according to <see cref="PaneBase.CalcScaleFactor"/>
		/// for the <see cref="GraphPane"/>, determines the minimum amount of space
		/// an axis must have between the <see cref="Chart.Rect">Chart.Rect</see> and the
		/// <see cref="PaneBase.Rect">GraphPane.Rect</see>.  This minimum space
		/// applies whether <see cref="IsVisible"/> is true or false.
		/// </remarks>
		public float MinSpace
		{
			get { return _minSpace; }
			set { _minSpace = value; }
		}

	#endregion

	#region Tic Properties

		/// <summary>
		/// The color to use for drawing this <see cref="Axis"/>.
		/// </summary>
		/// <remarks>
		/// This affects only the axis segment (see <see cref="IsAxisSegmentVisible" />),
		/// since the <see cref="Title"/>,
		/// <see cref="Scale"/>, <see cref="MajorTic" />, <see cref="MinorTic" />,
		/// <see cref="MajorGrid" />, and <see cref="MinorGrid" />
		/// all have their own color specification.
		/// </remarks>
		/// <value> The color is defined using the
		/// <see cref="System.Drawing.Color"/> class</value>
		/// <seealso cref="Default.Color"/>.
		/// <seealso cref="IsVisible"/>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		/// <summary>
		/// Gets a reference to the <see cref="ZedGraph.MajorTic" /> class instance
		/// for this <see cref="Axis" />.  This class stores all the major tic settings.
		/// </summary>
		public MajorTic MajorTic
		{
			get { return _majorTic; }
		}
		/// <summary>
		/// Gets a reference to the <see cref="ZedGraph.MinorTic" /> class instance
		/// for this <see cref="Axis" />.  This class stores all the minor tic settings.
		/// </summary>
		public MinorTic MinorTic
		{
			get { return _minorTic; }
		}

		#endregion

	#region Grid Properties

		/// <summary>
		/// Gets a reference to the <see cref="MajorGrid" /> class that contains the properties
		/// of the major grid.
		/// </summary>
		public MajorGrid MajorGrid
		{
			get { return _majorGrid; }
		}

		/// <summary>
		/// Gets a reference to the <see cref="MinorGrid" /> class that contains the properties
		/// of the minor grid.
		/// </summary>
		public MinorGrid MinorGrid
		{
			get { return _minorGrid; }
		}


	#endregion

	#region Type Properties

		/// <summary>
		/// This property determines whether or not the <see cref="Axis"/> is shown.
		/// </summary>
		/// <remarks>
		/// Note that even if
		/// the axis is not visible, it can still be actively used to draw curves on a
		/// graph, it will just be invisible to the user
		/// </remarks>
		/// <value>true to show the axis, false to disable all drawing of this axis</value>
		/// <seealso cref="ZedGraph.Scale.IsVisible"/>.
		/// <seealso cref="XAxis.Default.IsVisible"/>.
		/// <seealso cref="YAxis.Default.IsVisible"/>.
		/// <seealso cref="Y2Axis.Default.IsVisible"/>.
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		/// <summary>
		/// Gets or sets a property that determines whether or not the axis segment (the line that
		/// represents the axis itself) is drawn.
		/// </summary>
		/// <remarks>
		/// Under normal circumstances, this value won't affect the appearance of the display because
		/// the Axis segment is overlain by the Axis border (see <see cref="Chart.Border"/>).
		/// However, when the border is not visible, or when <see cref="Axis.CrossAuto"/> is set to
		/// false, this value will make a difference.
		/// </remarks>
		public bool IsAxisSegmentVisible
		{
			get { return _isAxisSegmentVisible; }
			set { _isAxisSegmentVisible = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="AxisType"/> for this <see cref="Axis"/>.
		/// </summary>
		/// <remarks>
		/// The type can be either <see cref="AxisType.Linear"/>,
		/// <see cref="AxisType.Log"/>, <see cref="AxisType.Date"/>,
		/// or <see cref="AxisType.Text"/>.
		/// </remarks>
		/// <seealso cref="ZedGraph.Scale.IsLog"/>
		/// <seealso cref="ZedGraph.Scale.IsText"/>
		/// <seealso cref="ZedGraph.Scale.IsOrdinal"/>
		/// <seealso cref="ZedGraph.Scale.IsDate"/>
		/// <seealso cref="ZedGraph.Scale.IsReverse"/>
		public AxisType Type
		{
			get { return _scale.Type; }
			set { _scale = Scale.MakeNewScale( _scale, value ); }
		}

	#endregion

	#region Label Properties

		/// <summary>
		/// Gets or sets the <see cref="Label" /> class that contains the title of this
		/// <see cref="Axis"/>.
		/// </summary>
		/// <remarks>The title normally shows the basis and dimensions of
		/// the scale range, such as "Time (Years)".  The title is only shown if the
		/// <see cref="Label.IsVisible"/> property is set to true.  If the Title text is empty,
		/// then no title is shown, and no space is "reserved" for the title on the graph.
		/// </remarks>
		/// <value>the title is a string value</value>
		/// <seealso cref="AxisLabel.IsOmitMag"/>
		public AxisLabel Title
		{
			get { return _title; }
			set { _title = value; }
		}

		/// <summary>
		/// The size of the gap between multiple axes (see <see cref="GraphPane.YAxisList" /> and
		/// <see cref="GraphPane.Y2AxisList" />).
		/// </summary>
		/// <remarks>
		/// This size will be scaled
		/// according to the <see cref="PaneBase.CalcScaleFactor"/> for the
		/// <see cref="GraphPane"/>
		/// </remarks>
		/// <value>The axis gap is measured in points (1/72 inch)</value>
		/// <seealso cref="Default.AxisGap"/>.
		public float AxisGap
		{
			get { return _axisGap; }
			set { _axisGap = value; }
		}

	#endregion

	#region Rendering Methods

		/// <summary>
		/// Restore the scale ranging to automatic mode, and recalculate the
		/// <see cref="Axis"/> scale ranges
		/// </summary>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <seealso cref="ZedGraph.Scale.MinAuto"/>
		/// <seealso cref="ZedGraph.Scale.MaxAuto"/>
		/// <seealso cref="ZedGraph.Scale.MajorStepAuto"/>
		/// <seealso cref="ZedGraph.Scale.MagAuto"/>
		/// <seealso cref="ZedGraph.Scale.FormatAuto"/>
		public void ResetAutoScale( GraphPane pane, Graphics g )
		{
			_scale._minAuto = true;
			_scale._maxAuto = true;
			_scale._majorStepAuto = true;
			_scale._minorStepAuto = true;
			_crossAuto = true;
			_scale._magAuto = true;
			//this.numDecAuto = true;
			_scale._formatAuto = true;
			pane.AxisChange( g );
		}

		/// <summary>
		/// Do all rendering associated with this <see cref="Axis"/> to the specified
		/// <see cref="Graphics"/> device.
		/// </summary>
		/// <remarks>
		/// This method is normally only
		/// called by the Draw method of the parent <see cref="GraphPane"/> object.
		/// </remarks>
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
		public void Draw( Graphics g, GraphPane pane, float scaleFactor, float shiftPos )
		{
			Matrix saveMatrix = g.Transform;

			_scale.SetupScaleData( pane, this );

			if ( _isVisible )
			{
				SetTransformMatrix( g, pane, scaleFactor );

				shiftPos = CalcTotalShift( pane, scaleFactor, shiftPos );

				_scale.Draw( g, pane, scaleFactor, shiftPos );
				//DrawTitle( g, pane, scaleFactor );

				g.Transform = saveMatrix;
			}
		}

		internal void DrawGrid( Graphics g, GraphPane pane, float scaleFactor, float shiftPos )
		{
			if ( _isVisible )
			{
				Matrix saveMatrix = g.Transform;
				SetTransformMatrix( g, pane, scaleFactor );

				double baseVal = _scale.CalcBaseTic();
				float topPix, rightPix;
				_scale.GetTopRightPix( pane, out topPix, out rightPix );

				shiftPos = CalcTotalShift( pane, scaleFactor, shiftPos );

				_scale.DrawGrid( g, pane, baseVal, topPix, scaleFactor );

				DrawMinorTics( g, pane, baseVal, shiftPos, scaleFactor, topPix );

				g.Transform = saveMatrix;
			}
		}

		/// <summary>
		/// This method will set the <see cref="MinSpace"/> property for this <see cref="Axis"/>
		/// using the currently required space multiplied by a fraction (<paramref>bufferFraction</paramref>).
		/// </summary>
		/// <remarks>
		/// The currently required space is calculated using <see cref="CalcSpace"/>, and is
		/// based on current data ranges, font sizes, etc.  The "space" is actually the amount of space
		/// required to fit the tic marks, scale labels, and axis title.
		/// </remarks>
		/// <param name="g">A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.</param>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.</param>
		/// <param name="bufferFraction">The amount of space to allocate for the axis, expressed
		/// as a fraction of the currently required space.  For example, a value of 1.2 would
		/// allow for 20% extra above the currently required space.</param>
		/// <param name="isGrowOnly">If true, then this method will only modify the <see cref="MinSpace"/>
		/// property if the calculated result is more than the current value.</param>
		public void SetMinSpaceBuffer( Graphics g, GraphPane pane, float bufferFraction,
										bool isGrowOnly )
		{
			// save the original value of minSpace
			float oldSpace = this.MinSpace;
			// set minspace to zero, since we don't want it to affect the CalcSpace() result
			this.MinSpace = 0;
			// Calculate the space required for the current graph assuming scalefactor = 1.0
			// and apply the bufferFraction
			float fixedSpace;
			float space = this.CalcSpace( g, pane, 1.0F, out fixedSpace ) * bufferFraction;
			// isGrowOnly indicates the minSpace can grow but not shrink
			if ( isGrowOnly )
				space = Math.Max( oldSpace, space );
			// Set the minSpace
			this.MinSpace = space;
		}

		/// <summary>
		/// Setup the Transform Matrix to handle drawing of this <see cref="Axis"/>
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
		abstract public void SetTransformMatrix( Graphics g, GraphPane pane, float scaleFactor );


		/// <summary>
		/// Calculate the "shift" size, in pixels, in order to shift the axis from its default
		/// location to the value specified by <see cref="Cross"/>.
		/// </summary>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <returns>The shift amount measured in pixels</returns>
		abstract internal float CalcCrossShift( GraphPane pane );

		/// <summary>
		/// Gets the "Cross" axis that corresponds to this axis.
		/// </summary>
		/// <remarks>
		/// The cross axis is the axis which determines the of this Axis when the
		/// <see cref="Axis.Cross" >Axis.Cross</see> property is used.  The
		/// cross axis for any <see cref="XAxis" /> or <see cref="X2Axis" />
		/// is always the primary <see cref="YAxis" />, and
		/// the cross axis for any <see cref="YAxis" /> or <see cref="Y2Axis" /> is
		/// always the primary <see cref="XAxis" />.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		abstract public Axis GetCrossAxis( GraphPane pane );

//		abstract internal float GetMinPix( GraphPane pane );

		//abstract internal float CalcCrossFraction( GraphPane pane );

		/// <summary>
		/// Returns the linearized actual cross position for this axis, reflecting the settings of
		/// <see cref="Cross" />, <see cref="CrossAuto" />, and <see cref="ZedGraph.Scale.IsReverse" />.
		/// </summary>
		/// <remarks>
		/// If the value of <see cref="Cross" /> lies outside the axis range, it is
		/// limited to the axis range.
		/// </remarks>
		internal double EffectiveCrossValue( GraphPane pane )
		{
			Axis crossAxis = GetCrossAxis( pane );

			// Use Linearize here instead of _minLinTemp because this method is called
			// as part of CalcRect() before scale is fully setup
			double min = crossAxis._scale.Linearize( crossAxis._scale._min );
			double max = crossAxis._scale.Linearize( crossAxis._scale._max );

			if ( _crossAuto )
			{
				if ( crossAxis._scale.IsReverse == ( this is Y2Axis || this is X2Axis ) )
					return max;
				else
					return min;
			}
			else if ( _cross < min )
				return min;
			else if ( _cross > max )
				return max;
			else
				return _scale.Linearize( _cross );
		}

		/// <summary>
		/// Returns true if the axis is shifted at all due to the setting of
		/// <see cref="Cross" />.  This function will always return false if
		/// <see cref="CrossAuto" /> is true.
		/// </summary>
		internal bool IsCrossShifted( GraphPane pane )
		{
			if ( _crossAuto )
				return false;
			else
			{
				Axis crossAxis = GetCrossAxis( pane );
				if ( ( ( this is XAxis || this is YAxis ) && !crossAxis._scale.IsReverse ) ||
					( ( this is X2Axis || this is Y2Axis ) && crossAxis._scale.IsReverse ) )
				{
					if ( _cross <= crossAxis._scale._min )
						return false;
				}
				else
				{
					if ( _cross >= crossAxis._scale._max )
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Calculates the proportional fraction of the total cross axis width at which
		/// this axis is located.
		/// </summary>
		/// <param name="pane"></param>
		/// <returns></returns>
		internal float CalcCrossFraction( GraphPane pane )
		{
			// if this axis is not shifted due to the Cross value
			if ( !this.IsCrossShifted( pane ) )
			{
				// if it's the primary axis and the scale labels are on the inside, then we
				// don't need to save any room for the axis labels (they will be inside the chart rect)
				if ( IsPrimary( pane ) && _scale._isLabelsInside )
					return 1.0f;
				// otherwise, it's a secondary (outboard) axis and we always save room for the axis and labels.
				else
					return 0.0f;
			}

			double effCross = EffectiveCrossValue( pane );
			Axis crossAxis = GetCrossAxis( pane );

			// Use Linearize here instead of _minLinTemp because this method is called
			// as part of CalcRect() before scale is fully setup
			//			double max = crossAxis._scale._maxLinTemp;
			//			double min = crossAxis._scale._minLinTemp;
			double max = crossAxis._scale.Linearize( crossAxis._scale._min );
			double min = crossAxis._scale.Linearize( crossAxis._scale._max );
			float frac;

			if ( ( ( this is XAxis || this is YAxis ) && _scale._isLabelsInside == crossAxis._scale.IsReverse ) ||
				 ( ( this is X2Axis || this is Y2Axis ) && _scale._isLabelsInside != crossAxis._scale.IsReverse ) )
				frac = (float)( ( effCross - min ) / ( max - min ) );
			else
				frac = (float)( ( max - effCross ) / ( max - min ) );

			if ( frac < 0.0f )
				frac = 0.0f;
			if ( frac > 1.0f )
				frac = 1.0f;

			return frac;
		}

		private float CalcTotalShift( GraphPane pane, float scaleFactor, float shiftPos )
		{
			if ( !IsPrimary( pane ) )
			{
				// if ( CalcCrossFraction( pane ) != 0.0 )
				if ( IsCrossShifted( pane ) )
				{
					shiftPos = 0;
				}
				else
				{
					// Scaled size (pixels) of a tic
					float ticSize = _majorTic.ScaledTic( scaleFactor );

					// if the scalelabels are on the inside, shift everything so the axis is drawn,
					// for example, to the left side of the available space for a YAxis type
					if ( _scale._isLabelsInside )
					{
						shiftPos += _tmpSpace;

						// shift the axis to leave room for the outside tics
						if ( _majorTic.IsOutside || _majorTic._isCrossOutside ||
										_minorTic.IsOutside || _minorTic._isCrossOutside )
							shiftPos -= ticSize;
					}
					else
					{
						// if it's not the primary axis, add a tic space for the spacing between axes
						shiftPos += _axisGap * scaleFactor;

						// if it has inside tics, leave another tic space
						if ( _majorTic.IsInside || _majorTic._isCrossInside ||
								_minorTic.IsInside || _minorTic._isCrossInside )
							shiftPos += ticSize;
					}
				}
			}

			// shift is the position of the actual axis line itself
			// everything else is based on that position.
			float crossShift = CalcCrossShift( pane );
			shiftPos += crossShift;

			return shiftPos;
		}

		/// <summary>
		/// Calculate the space required (pixels) for this <see cref="Axis"/> object.
		/// </summary>
		/// <remarks>
		/// This is the total space (vertical space for the X axis, horizontal space for
		/// the Y axes) required to contain the axis.  If <see cref="Cross" /> is zero, then
		/// this space will be the space required between the <see cref="Chart.Rect" /> and
		/// the <see cref="PaneBase.Rect" />.  This method sets the internal values of
		/// <see cref="_tmpSpace" /> for use by the <see cref="GraphPane.CalcChartRect(Graphics)" />
		/// method.
		/// </remarks>
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
		/// <param name="fixedSpace">The amount of space (pixels) at the edge of the ChartRect
		/// that is always required for this axis, even if the axis is shifted by the
		/// <see cref="Cross" /> value.</param>
		/// <returns>Returns the space, in pixels, required for this axis (between the
		/// rect and ChartRect)</returns>
		public float CalcSpace( Graphics g, GraphPane pane, float scaleFactor, out float fixedSpace )
		{
			//fixedSpace = 0;

			//Typical character height for the scale font
			float charHeight = _scale._fontSpec.GetHeight( scaleFactor );
			// Scaled size (pixels) of a tic
			float ticSize = _majorTic.ScaledTic( scaleFactor );
			// Scaled size (pixels) of the axis gap
			float axisGap = _axisGap * scaleFactor;
			float scaledLabelGap = _scale._labelGap * charHeight;
			float scaledTitleGap = _title.GetScaledGap( scaleFactor );

			// The minimum amount of space to reserve for the NORMAL position of the axis.  This would
			// be the left side of the chart rect for the Y axis, the right side for the Y2 axis, etc.
			// This amount of space is based on the need to reserve space for tics, etc., even if the
			// Axis.Cross property causes the axis to be in a different location.
			fixedSpace = 0;

			// The actual space needed for this axis (ignoring the setting of Axis.Cross)
			_tmpSpace = 0;

			// Account for the Axis
			if ( _isVisible )
			{
				bool hasTic = this.MajorTic.IsOutside || this.MajorTic._isCrossOutside ||
									this.MinorTic.IsOutside || this.MinorTic._isCrossOutside;

				// account for the tic space.  Leave the tic space for any type of outside tic (Outside Tic Space)
				if ( hasTic )
					_tmpSpace += ticSize;

				// if this is not the primary axis
				if ( !IsPrimary( pane ) )
				{
					// always leave an extra tic space for the space between the multi-axes (Axis Gap)
					_tmpSpace += axisGap;

					// if it has inside tics, leave another tic space (Inside Tic Space)
					if ( this.MajorTic._isInside || this.MajorTic._isCrossInside ||
							this.MinorTic._isInside || this.MinorTic._isCrossInside )
						_tmpSpace += ticSize;
				}

				// tic takes up 1x tic
				// space between tic and scale label is 0.5 tic
				// scale label is GetScaleMaxSpace()
				// space between scale label and axis label is 0.5 tic

				// account for the tic labels + 'LabelGap' tic gap between the tic and the label
				_tmpSpace += _scale.GetScaleMaxSpace( g, pane, scaleFactor, true ).Height +
						scaledLabelGap;

				string str = MakeTitle();

				// Only add space for the title if there is one
				// Axis Title gets actual height
				// if ( str.Length > 0 && _title._isVisible )
				if ( !string.IsNullOrEmpty( str ) && _title._isVisible )
				{
					//tmpSpace += this.TitleFontSpec.BoundingBox( g, str, scaleFactor ).Height;
					fixedSpace = this.Title.FontSpec.BoundingBox( g, str, scaleFactor ).Height +
							scaledTitleGap;
					_tmpSpace += fixedSpace;

					fixedSpace += scaledTitleGap;
				}

				if ( hasTic )
					fixedSpace += ticSize;
			}

			// for the Y axes, make sure that enough space is left to fit the first
			// and last X axis scale label
			if ( this.IsPrimary( pane ) && ( (
					( this is YAxis && (
						( !pane.XAxis._scale._isSkipFirstLabel && !pane.XAxis._scale._isReverse ) ||
						( !pane.XAxis._scale._isSkipLastLabel && pane.XAxis._scale._isReverse ) ) ) ||
					( this is Y2Axis && (
						( !pane.XAxis._scale._isSkipFirstLabel && pane.XAxis._scale._isReverse ) ||
						( !pane.XAxis._scale._isSkipLastLabel && !pane.XAxis._scale._isReverse ) ) ) ) &&
					pane.XAxis.IsVisible && pane.XAxis._scale._isVisible ) )
			{
				// half the width of the widest item, plus a gap of 1/2 the charheight
				float tmp = pane.XAxis._scale.GetScaleMaxSpace( g, pane, scaleFactor, true ).Width / 2.0F;
				//+ charHeight / 2.0F;
				//if ( tmp > tmpSpace )
				//	tmpSpace = tmp;

				fixedSpace = Math.Max( tmp, fixedSpace );
			}

			// Verify that the minSpace property was satisfied
			_tmpSpace = Math.Max( _tmpSpace, _minSpace * (float)scaleFactor );

			fixedSpace = Math.Max( fixedSpace, _minSpace * (float)scaleFactor );

			return _tmpSpace;
		}

		/// <summary>
		/// Determines if this <see cref="Axis" /> object is a "primary" one.
		/// </summary>
		/// <remarks>
		/// The primary axes are the <see cref="XAxis" /> (always), the first
		/// <see cref="YAxis" /> in the <see cref="GraphPane.YAxisList" /> 
		/// (<see cref="CurveItem.YAxisIndex" /> = 0),  and the first
		/// <see cref="Y2Axis" /> in the <see cref="GraphPane.Y2AxisList" /> 
		/// (<see cref="CurveItem.YAxisIndex" /> = 0).  Note that
		/// <see cref="GraphPane.YAxis" /> and <see cref="GraphPane.Y2Axis" />
		/// always reference the primary axes.
		/// </remarks>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <returns>true for a primary <see cref="Axis" /> (for the <see cref="XAxis" />,
		/// this is always true), false otherwise</returns>
		abstract internal bool IsPrimary( GraphPane pane );

		internal void FixZeroLine( Graphics g, GraphPane pane, float scaleFactor,
				float left, float right )
		{
			// restore the zero line if needed (since the fill tends to cover it up)
			if ( _isVisible && _majorGrid._isZeroLine &&
					_scale._min < 0.0 && _scale._max > 0.0 )
			{
				float zeroPix = _scale.Transform( 0.0 );

				using ( Pen zeroPen = new Pen( _color,
						pane.ScaledPenWidth( _majorGrid._penWidth, scaleFactor ) ) )
				{
					g.DrawLine( zeroPen, left, zeroPix, right, zeroPix );
					//zeroPen.Dispose();
				}
			}
		}

		/// <summary>
		/// Draw the minor tic marks as required for this <see cref="Axis"/>.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="baseVal">
		/// The scale value for the first major tic position.  This is the reference point
		/// for all other tic marks.
		/// </param>
		/// <param name="shift">The number of pixels to shift this axis, based on the
		/// value of <see cref="Cross"/>.  A positive value is into the ChartRect relative to
		/// the default axis position.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <param name="topPix">
		/// The pixel location of the far side of the ChartRect from this axis.
		/// This value is the ChartRect.Height for the XAxis, or the ChartRect.Width
		/// for the YAxis and Y2Axis.
		/// </param>
		public void DrawMinorTics( Graphics g, GraphPane pane, double baseVal, float shift,
								float scaleFactor, float topPix )
		{
			if ( ( this.MinorTic.IsOutside || this.MinorTic.IsOpposite || this.MinorTic.IsInside ||
					this.MinorTic._isCrossOutside || this.MinorTic._isCrossInside || _minorGrid._isVisible )
					&& _isVisible )
			{
				double	tMajor = _scale._majorStep * _scale.MajorUnitMultiplier,
							tMinor = _scale._minorStep * _scale.MinorUnitMultiplier;

				if ( _scale.IsLog || tMinor < tMajor )
				{
					float minorScaledTic = this.MinorTic.ScaledTic( scaleFactor );

					// Minor tics start at the minimum value and step all the way thru
					// the full scale.  This means that if the minor step size is not
					// an even division of the major step size, the minor tics won't
					// line up with all of the scale labels and major tics.
					double	first = _scale._minLinTemp,
								last = _scale._maxLinTemp;

					double dVal = first;
					float pixVal;

					int iTic = _scale.CalcMinorStart( baseVal );
					int MajorTic = 0;
					double majorVal = _scale.CalcMajorTicValue( baseVal, MajorTic );

					using ( Pen	pen = new Pen( _minorTic._color,
										pane.ScaledPenWidth( MinorTic._penWidth, scaleFactor ) ) )
					using ( Pen minorGridPen = _minorGrid.GetPen( pane, scaleFactor ) )
					{

						// Draw the minor tic marks
						while ( dVal < last && iTic < 5000 )
						{
							// Calculate the scale value for the current tic
							dVal = _scale.CalcMinorTicValue( baseVal, iTic );
							// Maintain a value for the current major tic
							if ( dVal > majorVal )
								majorVal = _scale.CalcMajorTicValue( baseVal, ++MajorTic );

							// Make sure that the current value does not match up with a major tic
							if ( ( ( Math.Abs( dVal ) < 1e-20 && Math.Abs( dVal - majorVal ) > 1e-20 ) ||
								( Math.Abs( dVal ) > 1e-20 && Math.Abs( ( dVal - majorVal ) / dVal ) > 1e-10 ) ) &&
								( dVal >= first && dVal <= last ) )
							{
								pixVal = _scale.LocalTransform( dVal );

								_minorGrid.Draw( g, minorGridPen, pixVal, topPix );

								_minorTic.Draw( g, pane, pen, pixVal, topPix, shift, minorScaledTic );
							}

							iTic++;
						}
					}
				}
			}
		}

		/// <summary>
		/// Draw the title for this <see cref="Axis"/>.
		/// </summary>
		/// <remarks>On entry, it is assumed that the
		/// graphics transform has been configured so that the origin is at the left side
		/// of this axis, and the axis is aligned along the X coordinate direction.
		/// </remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="shiftPos">The number of pixels to shift this axis, based on the
		/// value of <see cref="Cross"/>.  A positive value is into the ChartRect relative to
		/// the default axis position.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void DrawTitle( Graphics g, GraphPane pane, float shiftPos, float scaleFactor )
		{
			string str = MakeTitle();

			// If the Axis is visible, draw the title
			//if ( _isVisible && _title._isVisible && str.Length > 0 )
			if ( _isVisible && _title._isVisible && !string.IsNullOrEmpty( str ) )
			{
				bool hasTic = ( _scale._isLabelsInside ?
						( this.MajorTic.IsInside || this.MajorTic._isCrossInside ||
								this.MinorTic.IsInside || this.MinorTic._isCrossInside ) :
						( this.MajorTic.IsOutside || this.MajorTic._isCrossOutside || this.MinorTic.IsOutside || this.MinorTic._isCrossOutside ) );

				// Calculate the title position in screen coordinates
				float x = ( _scale._maxPix - _scale._minPix ) / 2;

				float scaledTic = MajorTic.ScaledTic( scaleFactor );
				float scaledLabelGap = _scale._fontSpec.GetHeight( scaleFactor ) * _scale._labelGap;
				float scaledTitleGap = _title.GetScaledGap( scaleFactor );

				// The space for the scale labels is only reserved if the axis is not shifted due to the
				// cross value.  Note that this could be a problem if the axis is only shifted slightly,
				// since the scale value labels may overlap the axis title.  However, it's not possible to
				// calculate that actual shift amount at this point, because the ChartRect rect has not yet been
				// calculated, and the cross value is determined using a transform of scale values (which
				// rely on ChartRect).

				float gap = scaledTic * ( hasTic ? 1.0f : 0.0f ) +
							this.Title.FontSpec.BoundingBox( g, str, scaleFactor ).Height / 2.0F;
				float y = ( _scale._isVisible ? _scale.GetScaleMaxSpace( g, pane, scaleFactor, true ).Height
							+ scaledLabelGap : 0 );

				if ( _scale._isLabelsInside )
					y = shiftPos - y - gap;
				else
					y = shiftPos + y + gap;

				if ( !_crossAuto && !_title._isTitleAtCross )
					y = Math.Max( y, gap );

				AlignV alignV = AlignV.Center;

				// Add in the TitleGap space
				y += scaledTitleGap;

				// Draw the title
				this.Title.FontSpec.Draw( g, pane, str, x, y,
							AlignH.Center, alignV, scaleFactor );
			}
		}

		private string MakeTitle()
		{
			if ( _title._text == null )
				_title._text = "";

			// Revision: JCarpenter 10/06
			// Allow customization of the modified title when the scale is very large
			// The event handler can edit the full label.  If the handler returns
			// null, then the title will be the default.
			if ( ScaleTitleEvent != null )
			{
				string label = ScaleTitleEvent( this );
				if ( label != null )
					return label;
			}

			// If the Mag is non-zero and IsOmitMag == false, and IsLog == false,
			// then add the mag indicator to the title.
			if ( _scale._mag != 0 && !_title._isOmitMag && !_scale.IsLog )
				return _title._text + String.Format( " (10^{0})", _scale._mag );
			else
				return _title._text;

		}

		/// <summary>
		/// Make a value label for the axis at the specified ordinal position.
		/// </summary>
		/// <remarks>
		/// This method properly accounts for <see cref="ZedGraph.Scale.IsLog"/>,
		/// <see cref="ZedGraph.Scale.IsText"/>,
		/// and other axis format settings.  It also implements the ScaleFormatEvent such that
		/// custom labels can be created.
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
		/// The numeric value associated with the label.  This value is ignored for log
		/// (<see cref="ZedGraph.Scale.IsLog"/>)
		/// and text (<see cref="ZedGraph.Scale.IsText"/>) type axes.
		/// </param>
		/// <returns>The resulting value label as a <see cref="string" /></returns>
		internal string MakeLabelEventWorks( GraphPane pane, int index, double dVal )
		{
			// if there is a valid ScaleFormatEvent, then try to use it to create the label
			// the label will be non-null if it's to be used
			if ( this.ScaleFormatEvent != null )
			{
				string label;

				label = this.ScaleFormatEvent( pane, this, dVal, index );
				if ( label != null )
					return label;
			}

			// second try.  If there's no custom ScaleFormatEvent, then just call
			// _scale.MakeLabel according to the type of scale
			if ( this.Scale != null )
				return _scale.MakeLabel( pane, index, dVal );
			else
				return "?";
		}

	#endregion

	}
}

