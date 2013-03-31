//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
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
	/// This class handles the drawing of the curve <see cref="OHLCBar"/> objects.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.5 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	public class OHLCBar : LineBase, ICloneable, ISerializable
	{
	#region Fields

		/// <summary>
		/// Private field that stores the visibility of the <see cref="OHLCBar"/> open and
		/// close line segments ("wings").  Use the public
		/// property <see cref="IsOpenCloseVisible"/> to access this value.  If this value is
		/// false, the wings will not be shown.
		/// </summary>
		protected bool _isOpenCloseVisible;

		/// <summary>
		/// Private field that stores the total width for the Opening/Closing line
		/// segments.  Use the public property <see cref="Size"/> to access this value.
		/// </summary>
		protected float _size;

		/// <summary>
		/// Private field that determines if the <see cref="Size" /> property will be
		/// calculated automatically based on the minimum axis scale step size between
		/// bars.  Use the public property <see cref="IsAutoSize" /> to access this value.
		/// </summary>
		protected Boolean _isAutoSize;

		/// <summary>
		/// The result of the autosize calculation, which is the size of the bars in
		/// user scale units.  This is converted to pixels at draw time.
		/// </summary>
		internal double _userScaleSize = 1.0;

	#endregion

	#region Defaults

		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="ZedGraph.OHLCBar"/> class.
		/// </summary>
		new public struct Default
		{
			// Default Symbol properties
			/// <summary>
			/// The default width for the candlesticks (see <see cref="OHLCBar.Size" />),
			/// in units of points.
			/// </summary>
			public static float Size = 12;
			/// <summary>
			/// The default display mode for symbols (<see cref="OHLCBar.IsOpenCloseVisible"/> property).
			/// true to display symbols, false to hide them.
			/// </summary>
			public static bool IsOpenCloseVisible = true;

			/// <summary>
			/// The default value for the <see cref="ZedGraph.OHLCBar.IsAutoSize" /> property.
			/// </summary>
			public static Boolean IsAutoSize = true;
		}

	#endregion

	#region Properties

		/// <summary>
		/// Gets or sets a property that shows or hides the <see cref="OHLCBar"/> open/close "wings".
		/// </summary>
		/// <value>true to show the CandleStick wings, false to hide them</value>
		/// <seealso cref="Default.IsOpenCloseVisible"/>
		public bool IsOpenCloseVisible
		{
			get { return _isOpenCloseVisible; }
			set { _isOpenCloseVisible = value; }
		}

		/// <summary>
		/// Gets or sets the total width to be used for drawing the opening/closing line
		/// segments ("wings") of the <see cref="OHLCBar" /> items. Units are points.
		/// </summary>
		/// <remarks>The size of the candlesticks can be set by this value, which
		/// is then scaled according to the scaleFactor (see
		/// <see cref="PaneBase.CalcScaleFactor"/>).  Alternatively,
		/// if <see cref="IsAutoSize"/> is true, the bar width will
		/// be set according to the maximum available cluster width less
		/// the cluster gap (see <see cref="BarSettings.GetClusterWidth"/>
		/// and <see cref="BarSettings.MinClusterGap"/>).  That is, if
		/// <see cref="IsAutoSize"/> is true, then the value of
		/// <see cref="Size"/> will be ignored.  If you modify the value of Size,
		/// then <see cref="IsAutoSize" /> will be automatically set to false.
		/// </remarks>
		/// <value>Size in points (1/72 inch)</value>
		/// <seealso cref="Default.Size"/>
		public float Size
		{
			get { return _size; }
			set { _size = value; _isAutoSize = false; }
		}

		/// <summary>
		/// Gets or sets a value that determines if the <see cref="Size" /> property will be
		/// calculated automatically based on the minimum axis scale step size between
		/// bars.
		/// </summary>
		public Boolean IsAutoSize
		{
			get { return _isAutoSize; }
			set { _isAutoSize = value; }
		}

	#endregion

	#region Constructors

		/// <summary>
		/// Default constructor that sets all <see cref="OHLCBar"/> properties to
		/// default values as defined in the <see cref="Default"/> class.
		/// </summary>
		public OHLCBar() : this( LineBase.Default.Color )
		{
		}

		/// <summary>
		/// Default constructor that sets the
		/// <see cref="Color"/> as specified, and the remaining
		/// <see cref="OHLCBar"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <param name="color">A <see cref="Color"/> value indicating
		/// the color of the symbol
		/// </param>
		public OHLCBar( Color color ) : base( color )
		{
			_size = Default.Size;
			_isAutoSize = Default.IsAutoSize;
			_isOpenCloseVisible = Default.IsOpenCloseVisible;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="OHLCBar"/> object from which to copy</param>
		public OHLCBar( OHLCBar rhs ) : base( rhs )
		{
			_isOpenCloseVisible = rhs._isOpenCloseVisible;
			_size = rhs._size;
			_isAutoSize = rhs._isAutoSize;
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
		public OHLCBar Clone()
		{
			return new OHLCBar( this );
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
		protected OHLCBar( SerializationInfo info, StreamingContext context ) :
			base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_isOpenCloseVisible = info.GetBoolean( "isOpenCloseVisible" );
			_size = info.GetSingle( "size" );
			_isAutoSize = info.GetBoolean( "isAutoSize" );
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
			info.AddValue( "schema", schema );
			info.AddValue( "isOpenCloseVisible", _isOpenCloseVisible );
			info.AddValue( "size", _size );
			info.AddValue( "isAutoSize", _isAutoSize );
		}

	#endregion

	#region Rendering Methods


		/// <summary>
		/// Draw the <see cref="OHLCBar"/> to the specified <see cref="Graphics"/>
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
		/// <see cref="OHLCBar"/> is the X axis.  True for an <see cref="XAxis"/> base,
		/// false for a <see cref="YAxis"/> or <see cref="Y2Axis"/> base.</param>
		/// <param name="pixBase">The independent axis position of the center of the candlestick in
		/// pixel units</param>
		/// <param name="pixHigh">The dependent axis position of the top of the candlestick in
		/// pixel units</param>
		/// <param name="pixLow">The dependent axis position of the bottom of the candlestick in
		/// pixel units</param>
		/// <param name="pixOpen">The dependent axis position of the opening value of the candlestick in
		/// pixel units</param>
		/// <param name="pixClose">The dependent axis position of the closing value of the candlestick in
		/// pixel units</param>
		/// <param name="halfSize">
		/// The scaled width of the candlesticks, pixels</param>
		/// <param name="pen">A pen with attributes of <see cref="Color"/> and
		/// <see cref="LineBase.Width"/> for this <see cref="OHLCBar"/></param>
		public void Draw( Graphics g, GraphPane pane, bool isXBase,
								float pixBase, float pixHigh, float pixLow,
								float pixOpen, float pixClose,
								float halfSize, Pen pen  )
		{
			if ( pixBase != PointPair.Missing )
			{
				if ( isXBase )
				{
					if ( Math.Abs( pixLow ) < 1000000 && Math.Abs( pixHigh ) < 1000000 )
						g.DrawLine( pen, pixBase, pixHigh, pixBase, pixLow );
					if ( _isOpenCloseVisible && Math.Abs( pixOpen ) < 1000000 )
						g.DrawLine( pen, pixBase - halfSize, pixOpen, pixBase, pixOpen );
					if ( _isOpenCloseVisible && Math.Abs( pixClose ) < 1000000 )
						g.DrawLine( pen, pixBase, pixClose, pixBase + halfSize, pixClose );
				}
				else
				{
					if ( Math.Abs( pixLow ) < 1000000 && Math.Abs( pixHigh ) < 1000000 )
						g.DrawLine( pen, pixHigh, pixBase, pixLow, pixBase );
					if ( _isOpenCloseVisible && Math.Abs( pixOpen ) < 1000000 )
						g.DrawLine( pen, pixOpen, pixBase - halfSize, pixOpen, pixBase );
					if ( _isOpenCloseVisible && Math.Abs( pixClose ) < 1000000 )
						g.DrawLine( pen, pixClose, pixBase, pixClose, pixBase + halfSize );
				}
			}
		}


		/// <summary>
		/// Draw all the <see cref="OHLCBar"/>'s to the specified <see cref="Graphics"/>
		/// device as a candlestick at each defined point.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="curve">A <see cref="OHLCBarItem"/> object representing the
		/// <see cref="OHLCBar"/>'s to be drawn.</param>
		/// <param name="baseAxis">The <see cref="Axis"/> class instance that defines the base (independent)
		/// axis for the <see cref="OHLCBar"/></param>
		/// <param name="valueAxis">The <see cref="Axis"/> class instance that defines the value (dependent)
		/// axis for the <see cref="OHLCBar"/></param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		public void Draw( Graphics g, GraphPane pane, OHLCBarItem curve,
							Axis baseAxis, Axis valueAxis, float scaleFactor )
		{
			//ValueHandler valueHandler = new ValueHandler( pane, false );

			float pixBase, pixHigh, pixLow, pixOpen, pixClose;

			if ( curve.Points != null )
			{
				//float halfSize = _size * scaleFactor;
				float halfSize = GetBarWidth( pane, baseAxis, scaleFactor );

				using ( Pen pen = !curve.IsSelected ? new Pen( _color, _width ) :
						new Pen( Selection.Border.Color, Selection.Border.Width ) )
//				using ( Pen pen = new Pen( _color, _penWidth ) )
				{
					// Loop over each defined point							
					for ( int i = 0; i < curve.Points.Count; i++ )
					{
						PointPair pt = curve.Points[i];
						double date = pt.X;
						double high = pt.Y;
						double low = pt.Z;
						double open = PointPair.Missing;
						double close = PointPair.Missing;
						if ( pt is StockPt )
						{
							open = ( pt as StockPt ).Open;
							close = ( pt as StockPt ).Close;
						}

						// Any value set to double max is invalid and should be skipped
						// This is used for calculated values that are out of range, divide
						//   by zero, etc.
						// Also, any value <= zero on a log scale is invalid

						if ( !curve.Points[i].IsInvalid3D &&
								( date > 0 || !baseAxis._scale.IsLog ) &&
								( ( high > 0 && low > 0 ) || !valueAxis._scale.IsLog ) )
						{
							pixBase = (int)( baseAxis.Scale.Transform( curve.IsOverrideOrdinal, i, date ) + 0.5 );
							//pixBase = baseAxis.Scale.Transform( curve.IsOverrideOrdinal, i, date );
							pixHigh = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, high );
							pixLow = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, low );
							if ( PointPair.IsValueInvalid( open ) )
								pixOpen = Single.MaxValue;
							else
								pixOpen = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, open );

							if ( PointPair.IsValueInvalid( close ) )
								pixClose = Single.MaxValue;
							else
								pixClose = valueAxis.Scale.Transform( curve.IsOverrideOrdinal, i, close );

							if ( !curve.IsSelected && this._gradientFill.IsGradientValueType )
							{
								using ( Pen tPen = GetPen( pane, scaleFactor, pt ) )
									Draw( g, pane, baseAxis is XAxis || baseAxis is X2Axis,
											pixBase, pixHigh, pixLow, pixOpen,
											pixClose, halfSize, tPen );
							}
							else
								Draw( g, pane, baseAxis is XAxis || baseAxis is X2Axis,
										pixBase, pixHigh, pixLow, pixOpen,
										pixClose, halfSize, pen );
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the width of the candleStick, in pixels, based on the settings for
		/// <see cref="Size"/> and <see cref="IsAutoSize"/>.
		/// </summary>
		/// <param name="pane">The parent <see cref="GraphPane"/> object.</param>
		/// <param name="baseAxis">The <see cref="Axis"/> object that
		/// represents the bar base (independent axis).</param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>The width of each bar, in pixel units</returns>
		public float GetBarWidth( GraphPane pane, Axis baseAxis, float scaleFactor )
		{
			float width;
			if ( _isAutoSize )
				width = baseAxis._scale.GetClusterWidth( _userScaleSize ) /
								( 1.0F + pane._barSettings.MinClusterGap ) / 2.0f;
			else
				width = (float)( _size * scaleFactor ) / 2.0f;

			// use integral size
			return (int)(width + 0.5f);
		}
		
	#endregion

	}
}
