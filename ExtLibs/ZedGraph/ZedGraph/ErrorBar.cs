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
	/// This class handles the drawing of the curve <see cref="ErrorBar"/> objects.
	/// The Error Bars are the vertical lines with a symbol at each end.
	/// </summary>
	/// <remarks>To draw "I-Beam" bars, the symbol type defaults to
	/// <see cref="SymbolType.HDash"/>, which is just a horizontal line.
	/// If <see cref="BarBase"/> is Y-oriented, then the symbol type should be
	/// set to <see cref="SymbolType.VDash"/> to get the same effect.
	/// </remarks>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.21 $ $Date: 2007-08-10 16:22:54 $ </version>
	[Serializable]
	public class ErrorBar : ICloneable, ISerializable
	{
	#region Fields
		/// <summary>
		/// Private field that stores the visibility of this
		/// <see cref="ErrorBar"/>.  Use the public
		/// property <see cref="IsVisible"/> to access this value.  If this value is
		/// false, the symbols will not be shown.
		/// </summary>
		private bool		_isVisible;
		/// <summary>
		/// Private field that stores the error bar color.  Use the public
		/// property <see cref="Color"/> to access this value.
		/// </summary>
		private Color		_color;
		/// <summary>
		/// Private field that stores the pen width for this error bar.  Use the public
		/// property <see cref="PenWidth"/> to access this value.
		/// </summary>
		private float		_penWidth;
		/// <summary>
		/// private field that contains the symbol element that will be drawn
		/// at the top and bottom of the error bar.  Use the public property
		/// <see cref="Symbol"/> to access this value.
		/// </summary>
		private Symbol		_symbol;
	#endregion

	#region Defaults
		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="ZedGraph.ErrorBar"/> class.
		/// </summary>
		public struct Default
		{
			// Default Symbol properties
			/// <summary>
			/// The default size for curve symbols
			/// (<see cref="ZedGraph.Symbol.Size"/> property),
			/// in units of points.
			/// </summary>
			public static float Size = 7;
			/// <summary>
			/// The default pen width to be used for drawing error bars
			/// (<see cref="ErrorBar.PenWidth"/> property).  Units are points.
			/// </summary>
			public static float PenWidth = 1.0F;
			/// <summary>
			/// The default display mode for symbols (<see cref="ErrorBar.IsVisible"/> property).
			/// true to display symbols, false to hide them.
			/// </summary>
			public static bool IsVisible = true;
			/// <summary>
			/// The default color for drawing error bars (<see cref="ErrorBar.Color"/> property).
			/// </summary>
			public static Color Color = Color.Red;
			/// <summary>
			/// The default symbol for drawing at the top and bottom of the
			/// error bar (see <see cref="ErrorBar.Symbol"/>).
			/// </summary>
			public static SymbolType Type = SymbolType.HDash;
		}
	#endregion

	#region Properties
		/// <summary>
		/// Gets or sets a property that shows or hides the <see cref="ErrorBar"/>.
		/// </summary>
		/// <value>true to show the error bar, false to hide it</value>
		/// <seealso cref="Default.IsVisible"/>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}
		
		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Color"/> data for this
		/// <see cref="ErrorBar"/>.
		/// </summary>
		/// <remarks>This property only controls the color of
		/// the vertical line.  The symbol color is controlled separately in
		/// the <see cref="Symbol"/> property.
		/// </remarks>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}
		/// <summary>
		/// The pen width to be used for drawing error bars
		/// Units are points.
		/// </summary>
		/// <remarks>This property only controls the pen width for the
		/// vertical line.  The pen width for the symbol outline is
		/// controlled separately by the <see cref="Symbol"/> property.
		/// </remarks>
		public float PenWidth
		{
			get { return _penWidth; }
			set { _penWidth = value; }
		}
		/// <summary>
		/// Contains the symbol element that will be drawn
		/// at the top and bottom of the error bar.
		/// </summary>
		public Symbol Symbol
		{
			get { return _symbol; }
			set { _symbol = value; }
		}

	#endregion
	
	#region Constructors
		/// <summary>
		/// Default constructor that sets all <see cref="ErrorBar"/> properties to
		/// default values as defined in the <see cref="Default"/> class.
		/// </summary>
		public ErrorBar() : this( Default.Color )
		{
		}

		/// <summary>
		/// Default constructor that sets the
		/// <see cref="Color"/> as specified, and the remaining
		/// <see cref="ErrorBar"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <param name="color">A <see cref="Color"/> value indicating
		/// the color of the symbol
		/// </param>
		public ErrorBar( Color color )
		{
			_symbol = new Symbol( Default.Type, color );
			_symbol.Size = Default.Size;
			_color = color;
			_penWidth = Default.PenWidth;
			_isVisible = Default.IsVisible;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ErrorBar"/> object from which to copy</param>
		public ErrorBar( ErrorBar rhs )
		{
			_color = rhs.Color;
			_isVisible = rhs.IsVisible;
			_penWidth = rhs.PenWidth;
			_symbol = rhs.Symbol.Clone();
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
		public ErrorBar Clone()
		{
			return new ErrorBar( this );
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
		protected ErrorBar( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_isVisible = info.GetBoolean( "isVisible" );
			_color = (Color) info.GetValue( "color", typeof(Color) );
			_penWidth = info.GetSingle( "penWidth" );
			_symbol = (Symbol) info.GetValue( "symbol", typeof(Symbol) );
		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
		[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "schema", schema );
			info.AddValue( "isVisible", _isVisible );
			info.AddValue( "color", _color );
			info.AddValue( "penWidth", _penWidth );
			info.AddValue( "symbol", _symbol );
		}
	#endregion

	#region Rendering Methods
		/// <summary>
		/// Draw the <see cref="ErrorBar"/> to the specified <see cref="Graphics"/>
		/// device at the specified location.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="isXBase">boolean value that indicates if the "base" axis for this
		/// <see cref="ErrorBar"/> is the X axis.  True for an <see cref="XAxis"/> base,
		/// false for a <see cref="YAxis"/> or <see cref="Y2Axis"/> base.</param>
		/// <param name="pixBase">The independent axis position of the center of the error bar in
		/// pixel units</param>
		/// <param name="pixValue">The dependent axis position of the top of the error bar in
		/// pixel units</param>
		/// <param name="pixLowValue">The dependent axis position of the bottom of the error bar in
		/// pixel units</param>
		/// <param name="scaleFactor">
		/// The scaling factor for the features of the graph based on the <see cref="PaneBase.BaseDimension"/>.  This
		/// scaling factor is calculated by the <see cref="PaneBase.CalcScaleFactor"/> method.  The scale factor
		/// represents a linear multiple to be applied to font sizes, symbol sizes, etc.</param>
		/// <param name="pen">A pen with attributes of <see cref="Color"/> and
		/// <see cref="PenWidth"/> for this <see cref="ErrorBar"/></param>
		/// <param name="dataValue">The data value to be used for a value-based
		/// color gradient.  This is only applicable for <see cref="FillType.GradientByX"/>,
		/// <see cref="FillType.GradientByY"/> or <see cref="FillType.GradientByZ"/>.</param>
		/// <param name="isSelected">Indicates that the <see cref="ErrorBar" /> should be drawn
		/// with attributes from the <see cref="Selection" /> class.
		/// </param>
		public void Draw( Graphics g, GraphPane pane, bool isXBase,
								float pixBase, float pixValue,
								float pixLowValue, float scaleFactor, Pen pen, bool isSelected,
								PointPair dataValue )
		{
			if ( isXBase )
			{
				g.DrawLine( pen, pixBase, pixValue, pixBase, pixLowValue );
				_symbol.DrawSymbol( g, pane, (int)pixBase, (int)pixValue,
							scaleFactor, isSelected, dataValue );
				_symbol.DrawSymbol( g, pane, (int)pixBase, (int)pixLowValue,
							scaleFactor, isSelected, dataValue );
			}
			else
			{
				g.DrawLine( pen, pixValue, pixBase, pixLowValue, pixBase );
				_symbol.DrawSymbol( g, pane, (int)pixValue, (int)pixBase,
							scaleFactor, isSelected, dataValue );
				_symbol.DrawSymbol( g, pane, (int)pixLowValue, (int)pixBase,
							scaleFactor, isSelected, dataValue );
			}
		}


		/// <summary>
		/// Draw all the <see cref="ErrorBar"/>'s to the specified <see cref="Graphics"/>
		/// device as a an error bar at each defined point.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="CurveItem"/> object representing the
		/// <see cref="Bar"/>'s to be drawn.</param>
		/// <param name="baseAxis">The <see cref="Axis"/> class instance that defines the base (independent)
		/// axis for the <see cref="Bar"/></param>
		/// <param name="valueAxis">The <see cref="Axis"/> class instance that defines the value (dependent)
		/// axis for the <see cref="Bar"/></param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void Draw( Graphics g, GraphPane pane, ErrorBarItem curve,
							Axis baseAxis, Axis valueAxis, float scaleFactor )
		{
			ValueHandler valueHandler = new ValueHandler( pane, false );

			float	pixBase, pixValue, pixLowValue;
			double	scaleBase, scaleValue, scaleLowValue;
		
			if ( curve.Points != null && this.IsVisible )
			{
				using ( Pen pen = !curve.IsSelected ? new Pen( _color, _penWidth ) :
						new Pen( Selection.Border.Color, Selection.Border.Width ) )
				{
					// Loop over each defined point							
					for ( int i = 0; i < curve.Points.Count; i++ )
					{
						valueHandler.GetValues( curve, i, out scaleBase,
									out scaleLowValue, out scaleValue );

						// Any value set to double max is invalid and should be skipped
						// This is used for calculated values that are out of range, divide
						//   by zero, etc.
						// Also, any value <= zero on a log scale is invalid

						if ( !curve.Points[i].IsInvalid3D &&
								( scaleBase > 0 || !baseAxis._scale.IsLog ) &&
								( ( scaleValue > 0 && scaleLowValue > 0 ) || !valueAxis._scale.IsLog ) )
						{
							pixBase = baseAxis.Scale.Transform( curve.IsOverrideOrdinal, i, scaleBase );
							pixValue = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, scaleValue );
							pixLowValue = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, scaleLowValue );

							//if ( this.fill.IsGradientValueType )
							//	brush = fill.MakeBrush( _rect, _points[i] );

							this.Draw( g, pane, baseAxis is XAxis || baseAxis is X2Axis, pixBase, pixValue,
											pixLowValue, scaleFactor, pen, curve.IsSelected,
											curve.Points[i] );
						}
					}
				}
			}
		}
	#endregion
	
	}
}
