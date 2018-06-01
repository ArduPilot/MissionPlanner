//============================================================================
//GasGaugeRegion Class
//Copyright © 2006 Jay Mistry
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//=============================================================================
using System.Runtime.Serialization;
using System.Drawing.Drawing2D;
using System;
using System.Text;
using System.Drawing;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// A class representing a region on the GasGuage chart
	/// <see cref="GasGaugeRegion"/>s.
	/// </summary>
	/// <author> Jay Mistry </author>
	/// <version> $Revision: 1.2 $ $Date: 2007-07-30 05:26:23 $ </version>
	[Serializable]
	public class GasGaugeRegion : CurveItem, ICloneable, ISerializable
	{
		#region Fields

		/// <summary>
		/// Defines the minimum value of this <see cref="GasGaugeRegion"/>
		/// </summary>
		private double _minValue;

		/// <summary>
		/// Defines the maximum value of this <see cref="GasGaugeRegion"/>
		/// </summary>
		private double _maxValue;

		/// <summary>
		/// Defines the Color of this <see cref="GasGaugeRegion"/>
		/// </summary>
		private Color _color;

		/// <summary>
		/// Internally calculated; Start angle of this pie that defines this <see cref="GasGaugeRegion"/>
		/// </summary>
		private float _startAngle;

		/// <summary>
		/// Internally calculated; Sweep angle of this pie that defines this <see cref="GasGaugeRegion"/>
		/// </summary>
		private float _sweepAngle;

		/// <summary>
		/// Private	field	that stores the	<see cref="ZedGraph.Fill"/> data for this
		/// <see cref="GasGaugeRegion"/>.	 Use the public property <see	cref="Fill"/> to
		/// access this value.
		/// </summary>
		private Fill _fill;

		/// <summary>
		/// A <see cref="ZedGraph.TextObj"/> which will customize the label display of this
		/// <see cref="GasGaugeRegion"/>
		/// </summary>
		private TextObj _labelDetail;

		/// <summary>
		/// Private	field	that stores the	<see cref="Border"/> class that defines	the
		/// properties of the	border around	this <see cref="GasGaugeRegion"/>. Use the public
		/// property	<see cref="Border"/> to access this value.
		/// </summary>
		private Border _border;

		/// <summary>
		/// The bounding rectangle for this <see cref="GasGaugeRegion"/>.
		/// </summary>
		private RectangleF _boundingRectangle;

		/// <summary>
		/// Private field to hold the GraphicsPath of this <see cref="GasGaugeRegion"/> to be
		/// used for 'hit testing'.
		/// </summary>
		private GraphicsPath _slicePath;

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
		protected GasGaugeRegion( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			// The schema value is just a file version parameter. You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			_labelDetail = (TextObj)info.GetValue( "labelDetail", typeof( TextObj ) );
			_fill = (Fill)info.GetValue( "fill", typeof( Fill ) );
			_border = (Border)info.GetValue( "border", typeof( Border ) );
			_color = (Color)info.GetValue( "color", typeof( Color ) );
			_minValue = info.GetDouble( "minValue" );
			_maxValue = info.GetDouble( "maxValue" );
			_startAngle = (float)info.GetDouble( "startAngle" );
			_sweepAngle = (float)info.GetDouble( "sweepAngle" );
			_boundingRectangle = (RectangleF)info.GetValue( "boundingRectangle", typeof( RectangleF ) );
			_slicePath = (GraphicsPath)info.GetValue( "slicePath", typeof( GraphicsPath ) );
		}

		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "schema2", schema2 );
			info.AddValue( "labelDetail", _labelDetail );
			info.AddValue( "fill", _fill );
			info.AddValue( "color", _color );
			info.AddValue( "border", _border );
			info.AddValue( "minVal", _minValue );
			info.AddValue( "maxVal", _maxValue );
			info.AddValue( "startAngle", _startAngle );
			info.AddValue( "sweepAngle", _sweepAngle );
			info.AddValue( "boundingRectangle", _boundingRectangle );
			info.AddValue( "slicePath", _slicePath );
		}
		#endregion

		#region Constructors

		/// <summary>
		/// Create a new <see cref="GasGaugeRegion"/>
		/// </summary>
		/// <param name="label">The value associated with this <see cref="GasGaugeRegion"/> instance.</param>
		/// <param name="color">The display color for this <see cref="GasGaugeRegion"/> instance.</param>
		/// <param name="minVal">The minimum value of this <see cref="GasGaugeNeedle"/>.</param>
		/// <param name="maxVal">The maximum value of this <see cref="GasGaugeNeedle"/>.</param>
		public GasGaugeRegion( string label, double minVal, double maxVal, Color color )
			: base( label )
		{
			MinValue = minVal;
			MaxValue = maxVal;
			RegionColor = color;
			StartAngle = 0f;
			SweepAngle = 0f;
			_border = new Border( Default.BorderColor, Default.BorderWidth );
			_labelDetail = new TextObj();
			_labelDetail.FontSpec.Size = Default.FontSize;
			_slicePath = null;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="ggr">The <see cref="GasGaugeRegion"/> object from which to copy</param>
		public GasGaugeRegion( GasGaugeRegion ggr )
			: base( ggr )
		{
			_minValue = ggr._minValue;
			_maxValue = ggr._maxValue;
			_color = ggr._color;
			_startAngle = ggr._startAngle;
			_sweepAngle = ggr._sweepAngle;
			_border = ggr._border.Clone();
			_labelDetail = ggr._labelDetail.Clone();
		}

		/// <summary>
		/// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
		/// calling the typed version of <see cref="Clone" />
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
		public GasGaugeRegion Clone()
		{
			return new GasGaugeRegion( this );
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the SlicePath of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public GraphicsPath SlicePath
		{
			get { return _slicePath; }
		}

		/// <summary>
		/// Gets or sets the LabelDetail of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public TextObj LabelDetail
		{
			get { return _labelDetail; }
			set { _labelDetail = value; }
		}

		/// <summary>
		/// Gets or sets the Border of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public Border Border
		{
			get { return ( _border ); }
			set { _border = value; }
		}

		/// <summary>
		/// Gets or sets the RegionColor of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public Color RegionColor
		{
			get { return _color; }
			set
			{
				_color = value;
				Fill = new Fill( _color );
			}
		}

		/// <summary>
		/// Gets or sets the Fill of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public Fill Fill
		{
			get { return _fill; }
			set { _fill = value; }
		}

		/// <summary>
		/// Gets or sets the SweepAngle of this <see cref="GasGaugeRegion"/>
		/// </summary>
		private float SweepAngle
		{
			get { return _sweepAngle; }
			set { _sweepAngle = value; }
		}

		/// <summary>
		/// Gets or sets the StartAngle of this <see cref="GasGaugeRegion"/>
		/// </summary>
		private float StartAngle
		{
			get { return ( _startAngle ); }
			set { _startAngle = value; }
		}

		/// <summary>
		/// Gets or sets the MinValue of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public double MinValue
		{
			get { return ( _minValue ); }
			set { _minValue = value > 0 ? value : 0; }
		}

		/// <summary>
		/// Gets or sets the MaxValue of this <see cref="GasGaugeRegion"/>
		/// </summary>
		public double MaxValue
		{
			get { return ( _maxValue ); }
			set { _maxValue = value > 0 ? value : 0; }
		}

		/// <summary>
		/// Gets a flag indicating if the Z data range should be included in the axis scaling calculations.
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
		/// </param>
		/// <value>true if the Z data are included, false otherwise</value>
		override internal bool IsZIncluded( GraphPane pane )
		{
			return false;
		}

		/// <summary>
		/// Gets a flag indicating if the X axis is the independent axis for this <see cref="CurveItem" />
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
		/// </param>
		/// <value>true if the X axis is independent, false otherwise</value>
		override internal bool IsXIndependent( GraphPane pane )
		{
			return true;
		}

	#endregion

	#region Defaults

		/// <summary>
		/// Specify the default property values for the <see cref="GasGaugeRegion"/> class.
		/// </summary>
		public struct Default
		{
			/// <summary>
			/// The default border pen width for the <see cref="GasGaugeRegion"/>
			/// </summary>
			public static float BorderWidth = 1.0F;

			/// <summary>
			/// The default fill type for the <see cref="GasGaugeRegion"/>
			/// </summary>
			public static FillType FillType = FillType.Brush;

			/// <summary>
			/// The default value for the visibility of the <see cref="GasGaugeRegion"/> border.
			/// </summary>
			public static bool IsBorderVisible = true;

			/// <summary>
			/// The default value for the color of the <see cref="GasGaugeRegion"/> border
			/// </summary>
			public static Color BorderColor = Color.Gray;

			/// <summary>
			/// The default value for the color of the <see cref="GasGaugeRegion"/> fill
			/// </summary>
			public static Color FillColor = Color.Empty;

			/// <summary>
			/// The default value for the fill brush of the <see cref="GasGaugeRegion"/>
			/// </summary>
			public static Brush FillBrush = null;

			/// <summary>
			/// The default value for the visibility of the <see cref="GasGaugeRegion"/> fill.
			/// </summary>
			public static bool isVisible = true;

//			public static PieLabelType LabelType = PieLabelType.Name;

			/// <summary>
			/// The default value for the font size of the <see cref="GasGaugeRegion"/> labels.
			/// </summary>
			public static float FontSize = 10;
		}

	#endregion Defaults

	#region Methods

		/// <summary>
		/// Do all rendering associated with this <see cref="GasGaugeRegion"/> item to the specified
		/// <see cref="Graphics"/> device. This method is normally only
		/// called by the Draw method of the parent <see cref="ZedGraph.CurveList"/>
		/// collection object.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into. This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="pos">Not used for rendering GasGaugeNeedle</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects. This is calculated and
		/// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>	
		public override void Draw( Graphics g, GraphPane pane, int pos, float scaleFactor )
		{
			if ( pane.Chart._rect.Width <= 0 && pane.Chart._rect.Height <= 0 )
			{
				_slicePath = null;
			}
			else
			{
				CalcRectangle( g, pane, scaleFactor, pane.Chart._rect );

				_slicePath = new GraphicsPath();

				if ( !_isVisible )
					return;

				RectangleF tRect = _boundingRectangle;

				if ( tRect.Width >= 1 && tRect.Height >= 1 )
				{
					SmoothingMode sMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					_slicePath.AddPie( tRect.X, tRect.Y, tRect.Width, tRect.Height,
															 -0.0f, -180.0f );

					g.FillPie( Fill.MakeBrush( _boundingRectangle ), tRect.X, tRect.Y, tRect.Width, tRect.Height, -StartAngle, -SweepAngle );

					if ( this.Border.IsVisible )
					{
						Pen borderPen = _border.GetPen( pane, scaleFactor );
						g.DrawPie( borderPen, tRect.X, tRect.Y, tRect.Width, tRect.Height,
							 -0.0f, -180.0f );
						borderPen.Dispose();
					}

					g.SmoothingMode = sMode;
				}
			}
		}

		/// <summary>
		/// Render the label for this <see cref="GasGaugeRegion"/>.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into. This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A graphic device object to be drawn into. This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="rect">Bounding rectangle for this <see cref="GasGaugeRegion"/>.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects. This is calculated and
		/// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>		
		public override void DrawLegendKey( Graphics g, GraphPane pane, RectangleF rect, float scaleFactor )
		{
			if ( !_isVisible )
				return;

			// Fill the slice
			if ( _fill.IsVisible )
			{
				// just avoid height/width being less than 0.1 so GDI+ doesn't cry
				using ( Brush brush = _fill.MakeBrush( rect ) )
				{
					g.FillRectangle( brush, rect );
					//brush.Dispose();
				}
			}

			// Border the bar
			if ( !_border.Color.IsEmpty )
				_border.Draw( g, pane, scaleFactor, rect );
		}

		/// <summary>
		/// Determine the coords for the rectangle associated with a specified point for 
		/// this <see cref="CurveItem" />
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane" /> to which this curve belongs</param>
		/// <param name="i">The index of the point of interest</param>
		/// <param name="coords">A list of coordinates that represents the "rect" for
		/// this point (used in an html AREA tag)</param>
		/// <returns>true if it's a valid point, false otherwise</returns>
		public override bool GetCoords( GraphPane pane, int i, out string coords )
		{
			coords = String.Empty;
			return false;
		}

		/// <summary>
		/// Calculate the values needed to properly display this <see cref="GasGaugeRegion"/>.
		/// </summary>
		/// <param name="pane">
		/// A graphic device object to be drawn into. This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		public static void CalculateGasGuageParameters( GraphPane pane )
		{
			//loop thru slices and get total value and maxDisplacement
			double minVal = double.MaxValue;
			double maxVal = double.MinValue;
			foreach ( CurveItem curve in pane.CurveList )
				if ( curve is GasGaugeRegion )
				{
					GasGaugeRegion ggr = (GasGaugeRegion)curve;
					if ( maxVal < ggr.MaxValue )
						maxVal = ggr.MaxValue;

					if ( minVal > ggr.MinValue )
						minVal = ggr.MinValue;
				}

			//Calculate start and sweep angles for each of the GasGaugeRegion based on teh min and max value
			foreach ( CurveItem curve in pane.CurveList )
			{
				if ( curve is GasGaugeRegion )
				{
					GasGaugeRegion ggr = (GasGaugeRegion)curve;
					float start = ( (float)ggr.MinValue - (float)minVal ) / ( (float)maxVal - (float)minVal ) * 180.0f;
					float sweep = ( (float)ggr.MaxValue - (float)minVal ) / ( (float)maxVal - (float)minVal ) * 180.0f;
					sweep = sweep - start;

					Fill f = new Fill( Color.White, ggr.RegionColor, -( sweep / 2f ) );
					ggr.Fill = f;

					ggr.StartAngle = start;
					ggr.SweepAngle = sweep;
				}
			}
		}

		/// <summary>
		/// Calculate the <see cref="RectangleF"/> that will be used to define the bounding rectangle of
		/// the GasGaugeNeedle.
		/// </summary>
		/// <remarks>This rectangle always lies inside of the <see cref="Chart.Rect"/>, and it is
		/// normally a square so that the pie itself is not oval-shaped.</remarks>
		/// <param name="g">
		/// A graphic device object to be drawn into. This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects. This is calculated and
		/// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>				
		/// <param name="chartRect">The <see cref="RectangleF"/> (normally the <see cref="Chart.Rect"/>)
		/// that bounds this pie.</param>
		/// <returns></returns>
		public static RectangleF CalcRectangle( Graphics g, GraphPane pane, float scaleFactor, RectangleF chartRect )
		{
			RectangleF nonExpRect = chartRect;

			if ( ( 2 * nonExpRect.Height ) > nonExpRect.Width )
			{
				//Scale based on width
				float percentS = ( ( nonExpRect.Height * 2 ) - nonExpRect.Width ) / ( nonExpRect.Height * 2 );
				nonExpRect.Height = ( ( nonExpRect.Height * 2 ) - ( ( nonExpRect.Height * 2 ) * percentS ) );
			}
			else
			{
				nonExpRect.Height = nonExpRect.Height * 2;
			}

			nonExpRect.Width = nonExpRect.Height;

			float xDelta = ( chartRect.Width / 2 ) - ( nonExpRect.Width / 2 );

			//Align Horizontally
			nonExpRect.X += xDelta;
			//nonExpRect.Y += -(float)0.025F * nonExpRect.Height;
			//nonExpRect.Y += ((chartRect.Height) - (nonExpRect.Height / 2)) - 10.0f;

			nonExpRect.Inflate( -(float)0.05F * nonExpRect.Height, -(float)0.05 * nonExpRect.Width );

			GasGaugeRegion.CalculateGasGuageParameters( pane );

			foreach ( CurveItem curve in pane.CurveList )
			{
				if ( curve is GasGaugeRegion )
				{
					GasGaugeRegion gg = (GasGaugeRegion)curve;
					gg._boundingRectangle = nonExpRect;
				}
			}

			return nonExpRect;

		}

		#endregion
	}
}
