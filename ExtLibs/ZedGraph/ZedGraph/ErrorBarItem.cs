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
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

#region Using directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace ZedGraph
{
	/// <summary>
	/// Encapsulates an "Error Bar" curve type that displays a vertical or horizontal
	/// line with a symbol at each end.
	/// </summary>
	/// <remarks>The <see cref="ErrorBarItem"/> type is intended for displaying
	/// confidence intervals, candlesticks, stock High-Low charts, etc.  It is
	/// technically not a bar, since it is drawn as a vertical or horizontal line.
	/// The default symbol at each end of the "bar" is <see cref="SymbolType.HDash"/>,
	/// which creates an "I-Beam".  For horizontal bars
	/// (<see cref="ZedGraph.BarBase.Y"/> or
	/// <see cref="ZedGraph.BarBase.Y2"/>), you will need to change the symbol to
	/// <see cref="SymbolType.VDash"/> to get horizontal "I-Beams".
	/// Since the horizontal segments are actually symbols, their widths are
	/// controlled by the symbol size in <see cref="ZedGraph.ErrorBar.Symbol"/>,
	/// specified in points (1/72nd inch).  The position of each "I-Beam" is set
	/// according to the <see cref="PointPair"/> values.  The independent axis
	/// is assigned with <see cref="BarSettings.Base"/>, and is a
	/// <see cref="ZedGraph.BarBase"/> enum type.</remarks>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.19 $ $Date: 2007-04-16 00:03:01 $ </version>
	[Serializable]
	public class ErrorBarItem : CurveItem, ICloneable, ISerializable
	{
	#region Fields
		/// <summary>
		/// Private field that stores a reference to the <see cref="ZedGraph.ErrorBar"/>
		/// class defined for this <see cref="ErrorBarItem"/>.  Use the public
		/// property <see cref="ErrorBar"/> to access this value.
		/// </summary>
		private ErrorBar _bar;

	#endregion

	#region Properties
		/// <summary>
		/// Gets a reference to the <see cref="ZedGraph.ErrorBar"/> class defined
		/// for this <see cref="ErrorBarItem"/>.
		/// </summary>
		public ErrorBar Bar
		{
			get { return _bar; }
		}

		/// <summary>
		/// Gets a flag indicating if the Z data range should be included in the axis scaling calculations.
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
		/// </param>
		/// <value>true if the Z data are included, false otherwise</value>
		override internal bool IsZIncluded( GraphPane pane )
		{
			return true;
		}

		/// <summary>
		/// Gets a flag indicating if the X axis is the independent axis for this <see cref="CurveItem" />
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
		/// </param>
		/// <value>true if the X axis is independent, false otherwise</value>
		override internal bool IsXIndependent( GraphPane pane )
		{
			return pane._barSettings.Base == BarBase.X;
		}

	#endregion

	#region Constructors
		/// <summary>
		/// Create a new <see cref="ErrorBarItem"/>, specifying only the legend label.
		/// </summary>
		/// <param name="label">The label that will appear in the legend.</param>
		public ErrorBarItem( string label ) : base( label )
		{
			_bar = new ErrorBar();
		}
		
		/// <summary>
		/// Create a new <see cref="ErrorBarItem"/> using the specified properties.
		/// </summary>
		/// <param name="label">The label that will appear in the legend.</param>
		/// <param name="x">An array of double precision values that define
		/// the X axis values for this curve</param>
		/// <param name="y">An array of double precision values that define
		/// the Y axis values for this curve</param>
		/// <param name="lowValue">An array of double precision values that define
		/// the lower dependent values for this curve</param>
		/// <param name="color">A <see cref="Color"/> value that will be applied to
		/// the <see cref="Line"/> properties.
		/// </param>
		public ErrorBarItem( string label, double[] x, double[] y, double[] lowValue,
							System.Drawing.Color color )
			: this( label, new PointPairList( x, y, lowValue ), color )
		{
		}

		/// <summary>
		/// Create a new <see cref="ErrorBarItem"/> using the specified properties.
		/// </summary>
		/// <param name="label">The label that will appear in the legend.</param>
		/// <param name="points">A <see cref="IPointList"/> of double precision values that define
		/// the X, Y and lower dependent values for this curve</param>
		/// <param name="color">A <see cref="Color"/> value that will be applied to
		/// the <see cref="Line"/> properties.
		/// </param>
		public ErrorBarItem( string label, IPointList points, Color color )
			: base( label, points )
		{
			_bar = new ErrorBar( color );
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ErrorBarItem"/> object from which to copy</param>
		public ErrorBarItem( ErrorBarItem rhs ) : base( rhs )
		{
			_bar = new ErrorBar( rhs.Bar );
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
		public ErrorBarItem Clone()
		{
			return new ErrorBarItem( this );
		}

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
		protected ErrorBarItem( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

			_bar = (ErrorBar) info.GetValue( "bar", typeof(ErrorBar) );

			// This is now just a dummy variable, since barBase was removed
			BarBase barBase = (BarBase) info.GetValue( "barBase", typeof(BarBase) );
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
			info.AddValue( "bar", _bar );

			// BarBase is now just a dummy value, since the GraphPane.BarBase is used exclusively
			info.AddValue( "barBase", BarBase.X );
		}
	#endregion

	#region Methods
		/// <summary>
		/// Do all rendering associated with this <see cref="ErrorBarItem"/> to the specified
		/// <see cref="Graphics"/> device.  This method is normally only
		/// called by the Draw method of the parent <see cref="ZedGraph.CurveList"/>
		/// collection object.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="pos">The ordinal position of the current <see cref="ErrorBarItem"/>
		/// curve.</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		override public void Draw( Graphics g, GraphPane pane, int pos, float scaleFactor  )
		{
			if ( _isVisible )
			{
				_bar.Draw( g, pane, this, this.BaseAxis( pane ),
								this.ValueAxis( pane ), scaleFactor );
			}
		}		

		/// <summary>
		/// Draw a legend key entry for this <see cref="ErrorBarItem"/> at the specified location
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
        /// <param name="pane">
        /// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
        /// owner of this object.
        /// </param>
        /// <param name="rect">The <see cref="RectangleF"/> struct that specifies the
        /// location for the legend key</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		override public void DrawLegendKey( Graphics g, GraphPane pane, RectangleF rect,
									float scaleFactor )
		{
			float pixBase, pixValue, pixLowValue;

			if ( pane._barSettings.Base == BarBase.X )
			{
				pixBase = rect.Left + rect.Width / 2.0F;
				pixValue = rect.Top;
				pixLowValue = rect.Bottom;
			}
			else
			{
				pixBase = rect.Top + rect.Height / 2.0F;
				pixValue = rect.Right;
				pixLowValue = rect.Left;
			}

			using ( Pen pen = new Pen( _bar.Color, _bar.PenWidth ) )
			{
				this.Bar.Draw( g, pane, pane._barSettings.Base == BarBase.X, pixBase, pixValue,
									pixLowValue, scaleFactor, pen, false, null );
			}
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
		override public bool GetCoords( GraphPane pane, int i, out string coords )
		{
			coords = string.Empty;

			if ( i < 0 || i >= _points.Count )
				return false;

			Axis valueAxis = ValueAxis( pane );
			Axis baseAxis = BaseAxis( pane );

			float scaledSize = _bar.Symbol.Size * pane.CalcScaleFactor();

			// pixBase = pixel value for the bar center on the base axis
			// pixHiVal = pixel value for the bar top on the value axis
			// pixLowVal = pixel value for the bar bottom on the value axis
			float pixBase, pixHiVal, pixLowVal;

			float clusterWidth = pane.BarSettings.GetClusterWidth();
			float barWidth = GetBarWidth( pane );
			float clusterGap = pane._barSettings.MinClusterGap * barWidth;
			float barGap = barWidth * pane._barSettings.MinBarGap;

			// curBase = the scale value on the base axis of the current bar
			// curHiVal = the scale value on the value axis of the current bar
			// curLowVal = the scale value of the bottom of the bar
			double curBase, curLowVal, curHiVal;
			ValueHandler valueHandler = new ValueHandler( pane, false );
			valueHandler.GetValues( this, i, out curBase, out curLowVal, out curHiVal );

			// Any value set to double max is invalid and should be skipped
			// This is used for calculated values that are out of range, divide
			//   by zero, etc.
			// Also, any value <= zero on a log scale is invalid

			if ( !_points[i].IsInvalid3D )
			{
				// calculate a pixel value for the top of the bar on value axis
				pixLowVal = valueAxis.Scale.Transform( _isOverrideOrdinal, i, curLowVal );
				pixHiVal = valueAxis.Scale.Transform( _isOverrideOrdinal, i, curHiVal );
				// calculate a pixel value for the center of the bar on the base axis
				pixBase = baseAxis.Scale.Transform( _isOverrideOrdinal, i, curBase );

				// Calculate the pixel location for the side of the bar (on the base axis)
				float pixSide = pixBase - scaledSize / 2.0F;

				// Draw the bar
				if ( baseAxis is XAxis || baseAxis is X2Axis )
					coords = String.Format( "{0:f0},{1:f0},{2:f0},{3:f0}",
								pixSide, pixLowVal,
								pixSide + scaledSize, pixHiVal );
				else
					coords = String.Format( "{0:f0},{1:f0},{2:f0},{3:f0}",
								pixLowVal, pixSide,
								pixHiVal, pixSide + scaledSize );

				return true;
			}

			return false;
		}

	#endregion

	}
}
