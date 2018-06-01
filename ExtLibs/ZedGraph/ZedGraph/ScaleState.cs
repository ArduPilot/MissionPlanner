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
	/// A class that captures an <see cref="Axis"/> scale range.
	/// </summary>
	/// <remarks>This structure is used by the <see cref="ZoomState"/> class to store
	/// <see cref="Axis"/> scale range settings in a collection for later retrieval.
	/// The class stores the <see cref="Scale.Min"/>, <see cref="Scale.Max"/>,
	/// <see cref="Scale.MinorStep"/>, and <see cref="Scale.MajorStep"/> properties, along with
	/// the corresponding auto-scale settings: <see cref="Scale.MinAuto"/>,
	/// <see cref="Scale.MaxAuto"/>, <see cref="Scale.MinorStepAuto"/>,
	/// and <see cref="Scale.MajorStepAuto"/>.</remarks>
	/// <author> John Champion </author>
	/// <version> $Revision: 3.2 $ $Date: 2007-02-19 08:05:24 $ </version>
	public class ScaleState : ICloneable
	{
		/// <summary>
		/// The axis range data for <see cref="Scale.Min"/>, <see cref="Scale.Max"/>,
		/// <see cref="Scale.MinorStep"/>, and <see cref="Scale.MajorStep"/>
		/// </summary>
		private double _min, _minorStep, _majorStep, _max;
		/// <summary>
		/// The status of <see cref="Scale.MinAuto"/>,
		/// <see cref="Scale.MaxAuto"/>, <see cref="Scale.MinorStepAuto"/>,
		/// and <see cref="Scale.MajorStepAuto"/>
		/// </summary>
		private bool _minAuto, _minorStepAuto,
							_majorStepAuto, _maxAuto,
							_formatAuto, _magAuto;

		/// <summary>
		/// The status of <see cref="Scale.MajorUnit"/> and <see cref="Scale.MinorUnit"/>
		/// </summary>
		private DateUnit _minorUnit, _majorUnit;

		private string _format;
		private int _mag;

		/// <summary>
		/// Construct a <see cref="ScaleState"/> from the specified <see cref="Axis"/>
		/// </summary>
		/// <param name="axis">The <see cref="Axis"/> from which to collect the scale
		/// range settings.</param>
		public ScaleState( Axis axis )
		{
			_min = axis._scale._min;
			_minorStep = axis._scale._minorStep;
			_majorStep = axis._scale._majorStep;
			_max = axis._scale._max;
			_majorUnit = axis._scale._majorUnit;
			_minorUnit = axis._scale._minorUnit;

			_format = axis._scale._format;
			_mag = axis._scale._mag;
			//this.numDec = axis.NumDec;

			_minAuto = axis._scale._minAuto;
			_majorStepAuto = axis._scale._majorStepAuto;
			_minorStepAuto = axis._scale._minorStepAuto;
			_maxAuto = axis._scale._maxAuto;

			_formatAuto = axis._scale._formatAuto;
			_magAuto = axis._scale._magAuto;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="ScaleState"/> object from which to copy</param>
		public ScaleState( ScaleState rhs )
		{
			_min = rhs._min;
			_majorStep = rhs._majorStep;
			_minorStep = rhs._minorStep;
			_max = rhs._max;
			_majorUnit = rhs._majorUnit;
			_minorUnit = rhs._minorUnit;

			_format = rhs._format;
			_mag = rhs._mag;

			_minAuto = rhs._minAuto;
			_majorStepAuto = rhs._majorStepAuto;
			_minorStepAuto = rhs._minorStepAuto;
			_maxAuto = rhs._maxAuto;

			_formatAuto = rhs._formatAuto;
			_magAuto = rhs._magAuto;
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
		public ScaleState Clone()
		{
			return new ScaleState( this );
		}

		/// <summary>
		/// Copy the properties from this <see cref="ScaleState"/> out to the specified <see cref="Axis"/>.
		/// </summary>
		/// <param name="axis">The <see cref="Axis"/> reference to which the properties should be
		/// copied</param>
		public void ApplyScale( Axis axis )
		{
			axis._scale._min = _min;
			axis._scale._majorStep = _majorStep;
			axis._scale._minorStep = _minorStep;
			axis._scale._max = _max;
			axis._scale._majorUnit = _majorUnit;
			axis._scale._minorUnit = _minorUnit;

			axis._scale._format = _format;
			axis._scale._mag = _mag;

			// The auto settings must be made after the min/step/max settings, since setting those
			// properties actually affects the auto settings.
			axis._scale._minAuto = _minAuto;
			axis._scale._minorStepAuto = _minorStepAuto;
			axis._scale._majorStepAuto = _majorStepAuto;
			axis._scale._maxAuto = _maxAuto;

			axis._scale._formatAuto = _formatAuto;
			axis._scale._magAuto = _magAuto;

		}

		/// <summary>
		/// Determine if the state contained in this <see cref="ScaleState"/> object is different from
		/// the state of the specified <see cref="Axis"/>.
		/// </summary>
		/// <param name="axis">The <see cref="Axis"/> object with which to compare states.</param>
		/// <returns>true if the states are different, false otherwise</returns>
		public bool IsChanged( Axis axis )
		{
			return axis._scale._min != _min ||
					axis._scale._majorStep != _majorStep ||
					axis._scale._minorStep != _minorStep ||
					axis._scale._max != _max ||
					axis._scale._minorUnit != _minorUnit ||
					axis._scale._majorUnit != _majorUnit ||
					axis._scale._minAuto != _minAuto ||
					axis._scale._minorStepAuto != _minorStepAuto ||
					axis._scale._majorStepAuto != _majorStepAuto ||
					axis._scale._maxAuto != _maxAuto;
		}

	}
}