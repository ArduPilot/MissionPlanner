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

#endregion

namespace ZedGraph
{
	/// <summary>
	/// A class that captures all the scale range settings for a <see cref="GraphPane"/>.
	/// </summary>
	/// <remarks>
	/// This class is used to store scale ranges in order to allow zooming out to
	/// prior scale range states.  <see cref="ZoomState"/> objects are maintained in the
	/// <see cref="ZoomStateStack"/> collection.  The <see cref="ZoomState"/> object holds
	/// a <see cref="ScaleState"/> object for each of the three axes; the <see cref="XAxis"/>,
	/// the <see cref="YAxis"/>, and the <see cref="Y2Axis"/>.
	/// </remarks>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.15 $ $Date: 2007-04-16 00:03:07 $ </version>
	public class ZoomState : ICloneable
	{
		/// <summary>
		/// An enumeration that describes whether a given state is the result of a Pan or Zoom
		/// operation.
		/// </summary>
		public enum StateType
		{
			/// <summary>
			/// Indicates the <see cref="ZoomState"/> object is from a Zoom operation
			/// </summary>
			Zoom,
			/// <summary>
			/// Indicates the <see cref="ZoomState"/> object is from a Wheel Zoom operation
			/// </summary>
			WheelZoom,
			/// <summary>
			/// Indicates the <see cref="ZoomState"/> object is from a Pan operation
			/// </summary>
			Pan,
			/// <summary>
			/// Indicates the <see cref="ZoomState"/> object is from a Scroll operation
			/// </summary>
			Scroll
		}

		/// <summary>
		/// <see cref="ScaleState"/> objects to store the state data from the axes.
		/// </summary>
		private ScaleState	_xAxis, _x2Axis;
		private ScaleStateList _yAxis, _y2Axis;
		/// <summary>
		/// An enum value indicating the type of adjustment being made to the
		/// scale range state.
		/// </summary>
		private StateType	_type;

		/// <summary>
		/// Gets a <see cref="StateType" /> value indicating the type of action (zoom or pan)
		/// saved by this <see cref="ZoomState" />.
		/// </summary>
		public StateType Type
		{
			get { return _type; }
		}

		/// <summary>
		/// Gets a string representing the type of adjustment that was made when this scale
		/// state was saved.
		/// </summary>
		/// <value>A string representation for the state change type; typically
		/// "Pan", "Zoom", or "Scroll".</value>
		public string TypeString
		{
			get
			{
				switch ( _type )
				{
					case StateType.Pan:
						return "Pan";
					case StateType.WheelZoom:
						return "WheelZoom";
					case StateType.Zoom:
					default:
						return "Zoom";
					case StateType.Scroll:
						return "Scroll";
				}
			}
		}

		/// <summary>
		/// Construct a <see cref="ZoomState"/> object from the scale ranges settings contained
		/// in the specified <see cref="GraphPane"/>.
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane"/> from which to obtain the scale
		/// range values.
		/// </param>
		/// <param name="type">A <see cref="StateType"/> enumeration that indicates whether
		/// this saved state is from a pan or zoom.</param>
		public ZoomState( GraphPane pane, StateType type )
		{

			_xAxis = new ScaleState( pane.XAxis );
			_x2Axis = new ScaleState( pane.X2Axis );
			_yAxis = new ScaleStateList( pane.YAxisList );
			_y2Axis = new ScaleStateList( pane.Y2AxisList );
			_type = type;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ZoomState"/> object from which to copy</param>
		public ZoomState( ZoomState rhs )
		{
			_xAxis = new ScaleState( rhs._xAxis );
			_x2Axis = new ScaleState( rhs._x2Axis );
			_yAxis = new ScaleStateList( rhs._yAxis );
			_y2Axis = new ScaleStateList( rhs._y2Axis );
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
		public ZoomState Clone()
		{
			return new ZoomState( this );
		}


		/// <summary>
		/// Copy the properties from this <see cref="ZoomState"/> out to the specified <see cref="GraphPane"/>.
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane"/> to which the scale range properties should be
		/// copied.</param>
		public void ApplyState( GraphPane pane )
		{
			_xAxis.ApplyScale( pane.XAxis );
			_x2Axis.ApplyScale( pane.X2Axis );
			_yAxis.ApplyScale( pane.YAxisList );
			_y2Axis.ApplyScale( pane.Y2AxisList );
		}

		/// <summary>
		/// Determine if the state contained in this <see cref="ZoomState"/> object is different from
		/// the state of the specified <see cref="GraphPane"/>.
		/// </summary>
		/// <param name="pane">The <see cref="GraphPane"/> object with which to compare states.</param>
		/// <returns>true if the states are different, false otherwise</returns>
		public bool IsChanged( GraphPane pane )
		{
			return _xAxis.IsChanged( pane.XAxis ) ||
					_x2Axis.IsChanged( pane.X2Axis ) ||
					_yAxis.IsChanged( pane.YAxisList ) ||
					_y2Axis.IsChanged( pane.Y2AxisList );
		}

	}
}
