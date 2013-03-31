//============================================================================ 
//DataSourcePointList Class 
//Copyright © 2006 John Champion, Jerry Vos 
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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Data;

namespace ZedGraph
{
	/// <summary> 
	///  
	/// </summary> 
	/// <seealso cref="IPointList" /> 
	/// <seealso cref="IPointListEdit" /> 
	///  
	/// <author>John Champion</author> 
	/// <version> $Revision: 3.7 $ $Date: 2007-11-05 04:33:26 $ </version> 
	[Serializable]
	public class DataSourcePointList : IPointList
	{
		private BindingSource _bindingSource;

		//private object _dataSource = null; 
		private string _xDataMember = null;
		private string _yDataMember = null;
		private string _zDataMember = null;
		private string _tagDataMember = null;

		#region Properties

		/// <summary> 
		/// Indexer to access the specified <see cref="PointPair"/> object by 
		/// its ordinal position in the list. 
		/// </summary> 
		/// <param name="index">The ordinal position (zero-based) of the 
		/// <see cref="PointPair"/> object to be accessed.</param> 
		/// <value>A <see cref="PointPair"/> object reference.</value> 
		public PointPair this[int index]
		{
			get
			{
				if ( index < 0 || index >= _bindingSource.Count )
					throw new System.ArgumentOutOfRangeException( "Error: Index out of range" );

				object row = _bindingSource[index];

				double x = GetDouble( row, _xDataMember, index );
				double y = GetDouble( row, _yDataMember, index );
				double z = GetDouble( row, _zDataMember, index );
				object tag = GetObject( row, _tagDataMember );

				PointPair pt = new PointPair( x, y, z );
				pt.Tag = tag;
				return pt;
			}
		}

		/// <summary> 
		/// gets the number of points available in the list 
		/// </summary> 
		public int Count
		{
			get
			{
				if ( _bindingSource != null )
					return _bindingSource.Count;
				else
					return 0;
			}
		}

		/// <summary> 
		/// The <see cref="BindingSource" /> object from which to get the bound data 
		/// </summary> 
		/// <remarks> 
		/// Typically, you set the <see cref="System.Windows.Forms.BindingSource.DataSource" /> 
		/// property to a reference to your database, table or list object. The 
		/// <see cref="System.Windows.Forms.BindingSource.DataMember" /> property would be set 
		/// to the name of the datatable within the 
		/// <see cref="System.Windows.Forms.BindingSource.DataSource" />, 
		/// if applicable.</remarks> 
		public BindingSource BindingSource
		{
			get { return _bindingSource; }
		}

		/// <summary> 
		/// The table or list object from which to extract the data values. 
		/// </summary> 
		/// <remarks> 
		/// This property is just an alias for 
		/// <see cref="System.Windows.Forms.BindingSource.DataSource" />. 
		/// </remarks> 
		public object DataSource
		{
			get { return _bindingSource.DataSource; }
			set { _bindingSource.DataSource = value; }
		}

		/// <summary> 
		/// The <see cref="string" /> name of the property or column from which to obtain the 
		/// X data values for the chart. 
		/// </summary> 
		/// <remarks>Set this to null leave the X data values set to <see cref="PointPairBase.Missing" /> 
		/// </remarks> 
		public string XDataMember
		{
			get { return _xDataMember; }
			set { _xDataMember = value; }
		}

		/// <summary> 
		/// The <see cref="string" /> name of the property or column from which to obtain the 
		/// Y data values for the chart. 
		/// </summary> 
		/// <remarks>Set this to null leave the Y data values set to <see cref="PointPairBase.Missing" /> 
		/// </remarks> 
		public string YDataMember
		{
			get { return _yDataMember; }
			set { _yDataMember = value; }
		}

		/// <summary> 
		/// The <see cref="string" /> name of the property or column from which to obtain the 
		/// Z data values for the chart. 
		/// </summary> 
		/// <remarks>Set this to null leave the Z data values set to <see cref="PointPairBase.Missing" /> 
		/// </remarks> 
		public string ZDataMember
		{
			get { return _zDataMember; }
			set { _zDataMember = value; }
		}

		/// <summary> 
		/// The <see cref="string" /> name of the property or column from which to obtain the 
		/// tag values for the chart. 
		/// </summary> 
		/// <remarks>Set this to null leave the tag values set to null. If this references string 
		/// data, then the tags may be used as tooltips using the 
		/// <see cref="ZedGraphControl.IsShowPointValues" /> option. 
		/// </remarks> 
		public string TagDataMember
		{
			get { return _tagDataMember; }
			set { _tagDataMember = value; }
		}

		#endregion

		#region Constructors

		/// <summary> 
		/// Default Constructor 
		/// </summary> 
		public DataSourcePointList()
		{
			_bindingSource = new BindingSource();
			_xDataMember = string.Empty;
			_yDataMember = string.Empty;
			_zDataMember = string.Empty;
			_tagDataMember = string.Empty;
		}

		/// <summary> 
		/// Constructor to initialize the DataSourcePointList from an 
		/// existing <see cref="DataSourcePointList" /> 
		/// </summary> 
		public DataSourcePointList( DataSourcePointList rhs )
			: this()
		{
			_bindingSource.DataSource = rhs._bindingSource.DataSource;
			if ( rhs._xDataMember != null )
				_xDataMember = (string)rhs._xDataMember.Clone();
			if ( rhs._yDataMember != null )
				_yDataMember = (string)rhs._yDataMember.Clone();
			if ( rhs._zDataMember != null )
				_zDataMember = (string)rhs._zDataMember.Clone();
			if ( rhs._tagDataMember != null )
				_tagDataMember = (string)rhs._tagDataMember.Clone();
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
		public DataSourcePointList Clone()
		{
			return new DataSourcePointList( this );
		}


		#endregion

		#region Methods

		/// <summary> 
		/// Extract a double value from the specified table row or data object with the 
		/// specified column name. 
		/// </summary> 
		/// <param name="row">The data object from which to extract the value</param> 
		/// <param name="dataMember">The property name or column name of the value 
		/// to be extracted</param> 
		/// <param name="index">The zero-based index of the point to be extracted. 
		/// </param> 
		private double GetDouble( object row, string dataMember, int index )
		{
			if ( dataMember == null || dataMember == string.Empty )
				return index + 1;

			//Type myType = row.GetType();
			DataRowView drv = row as DataRowView;
			PropertyInfo pInfo = null;
			if ( drv == null )
				pInfo = row.GetType().GetProperty( dataMember );

			object val = null;

			if ( pInfo != null )
				val = pInfo.GetValue( row, null );
			else if ( drv != null )
				val = drv[dataMember];
			else if ( pInfo == null )
				throw new System.Exception( "Can't find DataMember '" + dataMember + "' in DataSource" );

			// if ( val == null ) 
			// throw new System.Exception( "Can't find DataMember '" + dataMember + "' in DataSource" ); 

			double x;
			if ( val == null || val == DBNull.Value )
				x = PointPair.Missing;
			else if ( val.GetType() == typeof( DateTime ) )
				x = ( (DateTime)val ).ToOADate();
			else if ( val.GetType() == typeof( string ) )
				x = index + 1;
			else
				x = Convert.ToDouble( val );

			return x;
		}

		/// <summary> 
		/// Extract an object from the specified table row or data object with the 
		/// specified column name. 
		/// </summary> 
		/// <param name="row">The data object from which to extract the object</param> 
		/// <param name="dataMember">The property name or column name of the object 
		/// to be extracted</param> 
		private object GetObject( object row, string dataMember )
		{
			if ( dataMember == null || dataMember == string.Empty )
				return null;

			PropertyInfo pInfo = row.GetType().GetProperty( dataMember );
			DataRowView drv = row as DataRowView;

			object val = null;

			if ( pInfo != null )
				val = pInfo.GetValue( row, null );
			else if ( drv != null )
				val = drv[dataMember];

			if ( val == null )
				throw new System.Exception( "Can't find DataMember '" + dataMember + "' in DataSource" );

			return val;
		}


		#endregion
	}
}