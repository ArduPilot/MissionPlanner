//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright (C) 2006  John Champion
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
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// internal class to store pane layout details for the <see cref="MasterPane" />
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	public class PaneLayoutMgr : ICloneable, ISerializable
	{
		// =========== PANE LAYOUT STUFF ================

	#region Fields

		/// <summary>
		/// private field that saves the paneLayout format specified when
		/// <see cref="SetLayout(PaneLayout)"/> was called. This value will
		/// default to <see cref="MasterPane.Default.PaneLayout"/> if
		/// <see cref="SetLayout(PaneLayout)"/> (or an overload) was never called.
		/// </summary>
		internal PaneLayout _paneLayout;

		/// <summary>
		/// Private field that stores the boolean value that determines whether
		/// <see cref="_countList"/> is specifying rows or columns.
		/// </summary>
		internal bool _isColumnSpecified;
		/// <summary>
		/// private field that stores the row/column item count that was specified to the
		/// <see cref="SetLayout(bool,int[],float[])"/> method.  This values will be
		/// null if <see cref="SetLayout(bool,int[],float[])"/> was never called.
		/// </summary>
		internal int[] _countList;

		/// <summary>
		/// private field that stores the row/column size proportional values as specified
		/// to the <see cref="SetLayout(bool,int[],float[])"/> method.  This
		/// value will be null if <see cref="SetLayout(bool,int[],float[])"/>
		/// was never called.  
		/// </summary>
		internal float[] _prop;

	#endregion

	#region Constructors

		internal void Init()
		{
			_paneLayout = MasterPane.Default.PaneLayout;
			_countList = null;
			_isColumnSpecified = false;
			_prop = null;
		}

		internal PaneLayoutMgr()
		{
			Init();
		}

		internal PaneLayoutMgr( PaneLayoutMgr rhs )
		{
			_paneLayout = rhs._paneLayout;
			_countList = rhs._countList;
			_isColumnSpecified = rhs._isColumnSpecified;
			_prop = rhs._prop;
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
		public PaneLayoutMgr Clone()
		{
			return new PaneLayoutMgr( this );
		}

	#endregion

	#region Serialization

		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		internal const int schema = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		public PaneLayoutMgr( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_paneLayout = (PaneLayout)info.GetValue( "paneLayout", typeof( PaneLayout ) );
			_countList = (int[])info.GetValue( "countList", typeof(int[]) );

			_isColumnSpecified = info.GetBoolean( "isColumnSpecified" );
			_prop = (float[]) info.GetValue( "prop", typeof( float[] ) );
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

			info.AddValue( "paneLayout", _paneLayout );
			info.AddValue( "countList", _countList );
			info.AddValue( "isColumnSpecified", _isColumnSpecified );
			info.AddValue( "prop", _prop );
		}
	#endregion

	#region Methods

		/// <overloads>The SetLayout() methods setup the desired layout of the
		/// <see cref="GraphPane" /> objects within a <see cref="MasterPane" />.  These functions
		/// do not make any changes, they merely set the parameters so that future calls
		/// to <see cref="PaneBase.ReSize" /> or <see cref="DoLayout(Graphics,MasterPane)" />
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
		/// <seealso cref="SetLayout(int,int)" />
		/// <seealso cref="SetLayout(bool,int[])" />
		/// <seealso cref="SetLayout(bool,int[],float[])" />
		public void SetLayout( PaneLayout paneLayout )
		{
			Init();

			_paneLayout = paneLayout;
		}

		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to a reasonable configuration.
		/// </summary>
		/// <remarks>This method explicitly specifies the number of rows and columns to use
		/// in the layout, and all <see cref="GraphPane" /> objects will have the same size.
		/// Overloads are available that provide other layout options</remarks>
		/// <param name="rows">The number of rows of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		/// <param name="columns">The number of columns of <see cref="GraphPane"/> objects
		/// to include in the layout</param>
		/// <seealso cref="SetLayout(PaneLayout)" />
		/// <seealso cref="SetLayout(bool,int[])" />
		/// <seealso cref="SetLayout(bool,int[],float[])" />
		public void SetLayout( int rows, int columns )
		{
			Init();

			if ( rows < 1 )
				rows = 1;
			if ( columns < 1 )
				columns = 1;

			int[] countList = new int[rows];

			for (int i=0; i<rows; i++ )
				countList[i] = columns;

			SetLayout( true, countList, null );
		}

		/// <summary>
		/// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
		/// the list to the specified configuration.
		/// </summary>
		/// <remarks>This method specifies the number of rows in each column, or the number of
		/// columns in each row, allowing for irregular layouts.  Overloads are available that
		/// provide other layout options.
		/// </remarks>
		/// <param name="isColumnSpecified">Specifies whether the number of columns in each row, or
		/// the number of rows in each column will be specified.  A value of true indicates the
		/// number of columns in each row are specified in <see paramref="countList"/>.</param>
		/// <param name="countList">An integer array specifying either the number of columns in
		/// each row or the number of rows in each column, depending on the value of
		/// <see paramref="isColumnSpecified"/>.</param>
		/// <seealso cref="SetLayout(PaneLayout)" />
		/// <seealso cref="SetLayout(int,int)" />
		/// <seealso cref="SetLayout(bool,int[],float[])" />
		public void SetLayout( bool isColumnSpecified, int[] countList )
		{
			SetLayout( isColumnSpecified, countList, null );
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
		/// <seealso cref="SetLayout(PaneLayout)" />
		/// <seealso cref="SetLayout(int,int)" />
		/// <seealso cref="SetLayout(bool,int[])" />
		public void SetLayout( bool isColumnSpecified, int[] countList, float[] proportion )
		{
			Init();

			// use defaults if the parameters are invalid
			if ( countList != null && countList.Length > 0 )
			{
				this._prop = new float[countList.Length];

				// Sum up the total proportional factors
				float sumProp = 0.0f;
				for ( int i = 0; i < countList.Length; i++ )
				{
					this._prop[i] = ( proportion == null || proportion.Length <= i || proportion[i] < 1e-10 ) ?
												1.0f : proportion[i];
					sumProp += this._prop[i];
				}

				// Make prop sum to 1.0
				for ( int i=0; i<countList.Length; i++ )
					this._prop[i] /= sumProp;

				_isColumnSpecified = isColumnSpecified;
				_countList = countList;
			}
		}

		/// <summary>
		/// Modify the <see cref="GraphPane" /> <see cref="PaneBase.Rect" /> sizes of each
		/// <see cref="GraphPane" /> such that they fit within the <see cref="MasterPane" />
		/// in a pre-configured layout.
		/// </summary>
		/// <remarks>The <see cref="SetLayout(PaneLayout)" /> method (and overloads) is
		/// used for setting the layout configuration.</remarks>
		/// <param name="g">A <see cref="Graphics" /> instance to be used for font sizing,
		/// etc. in determining the layout configuration.</param>
		/// <param name="master">The <see cref="MasterPane" /> instance which is to
		/// be resized.</param>
		/// <seealso cref="SetLayout(PaneLayout)" />
		/// <seealso cref="SetLayout(int,int)" />
		/// <seealso cref="SetLayout(bool,int[])" />
		/// <seealso cref="SetLayout(bool,int[],float[])" />
		public void DoLayout( Graphics g, MasterPane master )
		{
			if ( this._countList != null )
				DoLayout( g, master, this._isColumnSpecified, this._countList, this._prop );
			else
			{
				int count = master.PaneList.Count;
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
						DoLayout( g, master, rows, cols );
						break;
					case PaneLayout.SingleColumn:
						rows = count;
						cols = 1;
						DoLayout( g, master, rows, cols );
						break;
					case PaneLayout.SingleRow:
						rows = 1;
						cols = count;
						DoLayout( g, master, rows, cols );
						break;
					default:
					case PaneLayout.SquareColPreferred:
						rows = root;
						cols = root;
						if ( count <= root * ( root - 1 ) )
							rows--;
						DoLayout( g, master, rows, cols );
						break;
					case PaneLayout.SquareRowPreferred:
						rows = root;
						cols = root;
						if ( count <= root * ( root - 1 ) )
							cols--;
						DoLayout( g, master, rows, cols );
						break;
					case PaneLayout.ExplicitCol12:
						DoLayout( g, master, true, new int[2] { 1, 2 }, null );
						break;
					case PaneLayout.ExplicitCol21:
						DoLayout( g, master, true, new int[2] { 2, 1 }, null );
						break;
					case PaneLayout.ExplicitCol23:
						DoLayout( g, master, true, new int[2] { 2, 3 }, null );
						break;
					case PaneLayout.ExplicitCol32:
						DoLayout( g, master, true, new int[2] { 3, 2 }, null );
						break;
					case PaneLayout.ExplicitRow12:
						DoLayout( g, master, false, new int[2] { 1, 2 }, null );
						break;
					case PaneLayout.ExplicitRow21:
						DoLayout( g, master, false, new int[2] { 2, 1 }, null );
						break;
					case PaneLayout.ExplicitRow23:
						DoLayout( g, master, false, new int[2] { 2, 3 }, null );
						break;
					case PaneLayout.ExplicitRow32:
						DoLayout( g, master, false, new int[2] { 3, 2 }, null );
						break;
				}
			}
		}

		/// <summary>
		/// Internal method that applies a previously set layout with a specific
		/// row and column count.  This method is only called by
		/// <see cref="DoLayout(Graphics,MasterPane)" />.
		/// </summary>
		internal void DoLayout( Graphics g, MasterPane master, int rows, int columns )
		{
			if ( rows < 1 )
				rows = 1;
			if ( columns < 1 )
				columns = 1;

			int[] countList = new int[rows];

			for (int i=0; i<rows; i++ )
				countList[i] = columns;

			DoLayout( g, master, true, countList, null );
		}

		/// <summary>
		/// Internal method that applies a previously set layout with a rows per column or
		/// columns per row configuration.  This method is only called by
		/// <see cref="DoLayout(Graphics,MasterPane)" />.
		/// </summary>
		internal void DoLayout( Graphics g, MasterPane master, bool isColumnSpecified, int[] countList,
					float[] proportion )
		{

			// calculate scaleFactor on "normal" pane size (BaseDimension)
			float scaleFactor = master.CalcScaleFactor();

			// innerRect is the area for the GraphPane's
			RectangleF innerRect = master.CalcClientRect( g, scaleFactor );
			master.Legend.CalcRect( g, master, scaleFactor, ref innerRect );

			// scaled InnerGap is the area between the GraphPane.Rect's
			float scaledInnerGap = (float)( master._innerPaneGap * scaleFactor );

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

					if ( iPane >= master._paneList.Count )
						return;

					for ( int colNum = 0; colNum < columns; colNum++ )
					{
						master[iPane].Rect = new RectangleF(
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
						if ( iPane >= master._paneList.Count )
							return;

						master[iPane].Rect = new RectangleF(
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
			this._countList = null;
			this._rows = rows;
			this._columns = columns;

			// calculate scaleFactor on "normal" pane size (BaseDimension)
			float scaleFactor = this.CalcScaleFactor();

			// innerRect is the area for the GraphPane's
			RectangleF innerRect = CalcClientRect( g, scaleFactor );
			this._legend.CalcRect( g, this, scaleFactor, ref innerRect );

			// scaled InnerGap is the area between the GraphPane.Rect's
			float scaledInnerGap = (float)( this._innerPaneGap * scaleFactor );

			float width = ( innerRect.Width - (float)( columns - 1 ) * scaledInnerGap ) / (float)columns;
			float height = ( innerRect.Height - (float)( rows - 1 ) * scaledInnerGap ) / (float)rows;

			int i = 0;
			foreach ( GraphPane pane in this._paneList )
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
