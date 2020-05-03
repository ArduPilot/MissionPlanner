// Copyright 2005 - 2009 - Morten Nielsen (www.sharpgis.net)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace ProjNet.CoordinateSystems.Transformations
{
	/// <summary>
	/// The GeographicTransform class is implemented on geographic transformation objects and
	/// implements datum transformations between geographic coordinate systems.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    public class GeographicTransform : MathTransform
	{
		internal GeographicTransform(IGeographicCoordinateSystem sourceGCS, IGeographicCoordinateSystem targetGCS)
		{
			_SourceGCS = sourceGCS;
			_TargetGCS = targetGCS;
		}

		#region IGeographicTransform Members

		private IGeographicCoordinateSystem _SourceGCS;

		/// <summary>
		/// Gets or sets the source geographic coordinate system for the transformation.
		/// </summary>
		public IGeographicCoordinateSystem SourceGCS
		{
			get { return _SourceGCS; }
			set { _SourceGCS = value; }
		}

		private IGeographicCoordinateSystem _TargetGCS;

		/// <summary>
		/// Gets or sets the target geographic coordinate system for the transformation.
		/// </summary>
		public IGeographicCoordinateSystem TargetGCS
		{
			get { return _TargetGCS; }
			set { _TargetGCS = value; }
		}

		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification. [NOT IMPLEMENTED].
		/// </summary>
		public override string WKT
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets an XML representation of this object [NOT IMPLEMENTED].
		/// </summary>
		public override string XML
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

        public override int DimSource
        {
            get { return _SourceGCS.Dimension; }
        }

        public override int DimTarget
        {
            get { return _TargetGCS.Dimension; }
        }
        
        /// <summary>
		/// Creates the inverse transform of this object.
		/// </summary>
		/// <remarks>This method may fail if the transform is not one to one. However, all cartographic projections should succeed.</remarks>
		/// <returns></returns>
		public override IMathTransform Inverse()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Transforms a coordinate point. The passed parameter point should not be modified.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
        public override double[] Transform(double[] point)
		{
            double[] pOut = (double[]) point.Clone();
            pOut[0] /= SourceGCS.AngularUnit.RadiansPerUnit;
            pOut[0] -= SourceGCS.PrimeMeridian.Longitude / SourceGCS.PrimeMeridian.AngularUnit.RadiansPerUnit;
            pOut[0] += TargetGCS.PrimeMeridian.Longitude / TargetGCS.PrimeMeridian.AngularUnit.RadiansPerUnit;
            pOut[0] *= SourceGCS.AngularUnit.RadiansPerUnit;
			return pOut;
		}

		/// <summary>
		/// Transforms a list of coordinate point ordinal values.
		/// </summary>
		/// <remarks>
		/// This method is provided for efficiently transforming many points. The supplied array 
		/// of ordinal values will contain packed ordinal values. For example, if the source 
		/// dimension is 3, then the ordinals will be packed in this order (x0,y0,z0,x1,y1,z1 ...).
		/// The size of the passed array must be an integer multiple of DimSource. The returned 
		/// ordinal values are packed in a similar way. In some DCPs. the ordinals may be 
		/// transformed in-place, and the returned array may be the same as the passed array.
		/// So any client code should not attempt to reuse the passed ordinal values (although
		/// they can certainly reuse the passed array). If there is any problem then the server
		/// implementation will throw an exception. If this happens then the client should not
		/// make any assumptions about the state of the ordinal values.
		/// </remarks>
		/// <param name="points"></param>
		/// <returns></returns>
        public override IList<double[]> TransformList(IList<double[]> points)
		{
            var trans = new List<double[]>(points.Count);
            foreach (var p in points)
				trans.Add(Transform(p));
			return trans;
		}

	    public override IList<Coordinate> TransformList(IList<Coordinate> points)
	    {
	        var trans = new List<Coordinate>(points.Count);
            foreach (var coordinate in points)
	        {
	            trans.Add(Transform(coordinate));
	        }
	        return trans;
	    }

	    /// <summary>
		/// Reverses the transformation
		/// </summary>
		public override void Invert()
		{
			throw new NotImplementedException();
		}
	}
}
