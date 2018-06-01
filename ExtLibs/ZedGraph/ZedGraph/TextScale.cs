//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2005  John Champion
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
using System.Collections;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// The TextScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.Text" />.
	/// </summary>
	/// <remarks>
	/// TextScale is an ordinal axis with user-defined text labels.  An ordinal axis means that
	/// all data points are evenly spaced at integral values, and the actual coordinate values
	/// for points corresponding to that axis are ignored.  That is, if the X axis is an
	/// ordinal type, then all X values associated with the curves are ignored.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.8 $ $Date: 2006-08-25 05:19:09 $ </version>
	[Serializable]
	class TextScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		public TextScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="TextScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="TextScale" /></param>
		public TextScale( Scale rhs, Axis owner )
			: base( rhs, owner )
		{
		}

		/// <summary>
		/// Create a new clone of the current item, with a new owner assignment
		/// </summary>
		/// <param name="owner">The new <see cref="Axis" /> instance that will be
		/// the owner of the new Scale</param>
		/// <returns>A new <see cref="Scale" /> clone.</returns>
		public override Scale Clone( Axis owner )
		{
			return new TextScale( this, owner );
		}

	#endregion

	#region properties

		public override AxisType Type
		{
			get { return AxisType.Text; }
		}

	#endregion

	#region methods

		/// <summary>
		/// Internal routine to determine the ordinals of the first minor tic mark
		/// </summary>
		/// <param name="baseVal">
		/// The value of the first major tic for the axis.
		/// </param>
		/// <returns>
		/// The ordinal position of the first minor tic, relative to the first major tic.
		/// This value can be negative (e.g., -3 means the first minor tic is 3 minor step
		/// increments before the first major tic.
		/// </returns>
		override internal int CalcMinorStart( double baseVal )
		{
			// This should never happen (no minor tics for text labels)
			return 0;
		}

		/// <summary>
		/// Determine the value for the first major tic.
		/// </summary>
		/// <remarks>
		/// This is done by finding the first possible value that is an integral multiple of
		/// the step size, taking into account the date/time units if appropriate.
		/// This method properly accounts for <see cref="Scale.IsLog"/>, <see cref="Scale.IsText"/>,
		/// and other axis format settings.
		/// </remarks>
		/// <returns>
		/// First major tic value (floating point double).
		/// </returns>
		override internal double CalcBaseTic()
		{
			if ( _baseTic != PointPair.Missing )
				return _baseTic;
			else
				return 1.0;

		}
		
		/// <summary>
		/// Internal routine to determine the ordinals of the first and last major axis label.
		/// </summary>
		/// <returns>
		/// This is the total number of major tics for this axis.
		/// </returns>
		override internal int CalcNumTics()
		{
			int nTics = 1;

			// If no array of labels is available, just assume 10 labels so we don't blow up.
			if ( _textLabels == null )
				nTics = 10;
			else
				nTics = _textLabels.Length;

			if ( nTics < 1 )
				nTics = 1;
			else if ( nTics > 1000 )
				nTics = 1000;

			return nTics;
		}

		/// <summary>
		/// Select a reasonable text axis scale given a range of data values.
		/// </summary>
		/// <remarks>
		/// This method only applies to <see cref="AxisType.Text"/> type axes, and it
		/// is called by the general <see cref="PickScale"/> method.  This is an ordinal
		/// type, such that the labeled values start at 1.0 and increment by 1.0 for
		/// each successive label.  The maximum number of labels on the graph is
		/// determined by <see cref="Scale.Default.MaxTextLabels"/>.  If necessary, this method will
		/// set the <see cref="Scale.MajorStep"/> value to greater than 1.0 in order to keep the total
		/// labels displayed below <see cref="Scale.Default.MaxTextLabels"/>.  For example, a
		/// <see cref="Scale.MajorStep"/> size of 2.0 would only display every other label on the
		/// axis.  The <see cref="Scale.MajorStep"/> value calculated by this routine is always
		/// an integral value.  This
		/// method honors the <see cref="Scale.MinAuto"/>, <see cref="Scale.MaxAuto"/>,
		/// and <see cref="Scale.MajorStepAuto"/> autorange settings.
		/// In the event that any of the autorange settings are false, the
		/// corresponding <see cref="Scale.Min"/>, <see cref="Scale.Max"/>, or <see cref="Scale.MajorStep"/>
		/// setting is explicitly honored, and the remaining autorange settings (if any) will
		/// be calculated to accomodate the non-autoranged values.
		/// <para>On Exit:</para>
		/// <para><see cref="Scale.Min"/> is set to scale minimum (if <see cref="Scale.MinAuto"/> = true)</para>
		/// <para><see cref="Scale.Max"/> is set to scale maximum (if <see cref="Scale.MaxAuto"/> = true)</para>
		/// <para><see cref="Scale.MajorStep"/> is set to scale step size (if <see cref="Scale.MajorStepAuto"/> = true)</para>
		/// <para><see cref="Scale.MinorStep"/> is set to scale minor step size (if <see cref="Scale.MinorStepAuto"/> = true)</para>
		/// <para><see cref="Scale.Mag"/> is set to a magnitude multiplier according to the data</para>
		/// <para><see cref="Scale.Format"/> is set to the display format for the values (this controls the
		/// number of decimal places, whether there are thousands separators, currency types, etc.)</para>
		/// </remarks>
		/// <param name="pane">A reference to the <see cref="GraphPane"/> object
		/// associated with this <see cref="Axis"/></param>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="scaleFactor">
		/// The scaling factor to be used for rendering objects.  This is calculated and
		/// passed down by the parent <see cref="GraphPane"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <seealso cref="PickScale"/>
		/// <seealso cref="AxisType.Text"/>
		override public void PickScale( GraphPane pane, Graphics g, float scaleFactor )
		{
			// call the base class first
			base.PickScale( pane, g, scaleFactor );

			// if text labels are provided, then autorange to the number of labels
			if ( _textLabels != null )
			{
				if ( _minAuto )
					_min = 0.5;
				if ( _maxAuto )
					_max = _textLabels.Length + 0.5;
			}
			else
			{
				if ( _minAuto )
					_min -= 0.5;
				if ( _maxAuto )
					_max += 0.5;
			}
			// Test for trivial condition of range = 0 and pick a suitable default
			if ( _max - _min < .1 )
			{
				if ( _maxAuto )
					_max = _min + 10.0;
				else
					_min = _max - 10.0;
			}

			if ( _majorStepAuto )
			{
				if ( !_isPreventLabelOverlap )
				{
					_majorStep = 1;
				}
				else if ( _textLabels != null )
				{
					// Calculate the maximum number of labels
					double maxLabels = (double) this.CalcMaxLabels( g, pane, scaleFactor );

					// Calculate a step size based on the width of the labels
					double tmpStep = Math.Ceiling( ( _max - _min ) / maxLabels );

					// Use the lesser of the two step sizes
					//if ( tmpStep < this.majorStep )
					_majorStep = tmpStep;
				}
				else
					_majorStep = (int) ( ( _max - _min - 1.0 ) / Default.MaxTextLabels ) + 1.0;

			}
			else
			{
				_majorStep = (int) _majorStep;
				if ( _majorStep <= 0 )
					_majorStep = 1.0;
			}

			if ( _minorStepAuto )
			{
				_minorStep = _majorStep / 10;

				//_minorStep = CalcStepSize( _majorStep, 10 );
				if ( _minorStep < 1 )
					_minorStep = 1;
			}

			_mag = 0;
		}

		/// <summary>
		/// Make a value label for an <see cref="AxisType.Text" /> <see cref="Axis" />.
		/// </summary>
		/// <param name="pane">
		/// A reference to the <see cref="GraphPane"/> object that is the parent or
		/// owner of this object.
		/// </param>
		/// <param name="index">
		/// The zero-based, ordinal index of the label to be generated.  For example, a value of 2 would
		/// cause the third value label on the axis to be generated.
		/// </param>
		/// <param name="dVal">
		/// The numeric value associated with the label.  This value is ignored for log (<see cref="Scale.IsLog"/>)
		/// and text (<see cref="Scale.IsText"/>) type axes.
		/// </param>
		/// <returns>The resulting value label as a <see cref="string" /></returns>
		override internal string MakeLabel( GraphPane pane, int index, double dVal )
		{
			if ( _format == null )
				_format = Scale.Default.Format;

			index *= (int) _majorStep;
			if ( _textLabels == null || index < 0 || index >= _textLabels.Length )
				return string.Empty;
			else
				return _textLabels[index];
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
		protected TextScale( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

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
		}
	#endregion

	}
}
