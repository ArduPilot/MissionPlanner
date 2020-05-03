// Copyright 2006 - Morten Nielsen (www.iter.dk)
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
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// 
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable] 
#endif
    internal class ConcatenatedTransform : MathTransform
	{
        /// <summary>
        /// 
        /// </summary>
		protected IMathTransform _inverse;

        /// <summary>
        /// 
        /// </summary>
		public ConcatenatedTransform() : 
            this(new List<ICoordinateTransformation>()) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformlist"></param>
		public ConcatenatedTransform(List<ICoordinateTransformation> transformlist)
		{
			_coordinateTransformationList = transformlist;
		}

		private List<ICoordinateTransformation> _coordinateTransformationList;

        /// <summary>
        /// 
        /// </summary>
		public List<ICoordinateTransformation> CoordinateTransformationList
		{
			get { return _coordinateTransformationList; }
			set
			{
				_coordinateTransformationList = value;
				_inverse = null;
			}
		}

        public override int DimSource
        {
            get { return _coordinateTransformationList[0].SourceCS.Dimension; }
        }

        public override int DimTarget
        {
            get { return _coordinateTransformationList[_coordinateTransformationList.Count-1].TargetCS.Dimension; }
        }

        /// <summary>
        /// Transforms a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double[] Transform(double[] point)
		{
            foreach (ICoordinateTransformation ct in _coordinateTransformationList)
				point = ct.MathTransform.Transform(point);            
			return point;			
		}

        /// <summary>
		/// Transforms a list point
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public override IList<double[]> TransformList(IList<double[]> points)
		{
			IList<double[]> pnts = new List<double[]>(points);
			foreach (var ct in _coordinateTransformationList)
			{
			    pnts = ct.MathTransform.TransformList(pnts);
			}
			return pnts;
		}

        public override IList<Coordinate> TransformList(IList<Coordinate> points)
        {
            IList<Coordinate> pnts = new List<Coordinate>(points);
            foreach (var ct in _coordinateTransformationList)
            {
                pnts = ct.MathTransform.TransformList(pnts);
            }
            return pnts;
        }

        public override ICoordinateSequence Transform(ICoordinateSequence coordinateSequence)
        {
            var res = (ICoordinateSequence)coordinateSequence.Clone();
            foreach (var ct in _coordinateTransformationList)
            {
                res = ct.MathTransform.Transform(res);
            }
            return res;
        }

		/// <summary>
		/// Returns the inverse of this conversion.
		/// </summary>
		/// <returns>IMathTransform that is the reverse of the current conversion.</returns>
		public override IMathTransform Inverse()
		{
			if (_inverse == null)
			{
				_inverse = Clone();
				_inverse.Invert();
			}
			return _inverse;
		}
		
		/// <summary>
		/// Reverses the transformation
		/// </summary>
		public override void Invert()
		{
			_coordinateTransformationList.Reverse();
			foreach (ICoordinateTransformation ic in _coordinateTransformationList)
				ic.MathTransform.Invert();
		}

		public ConcatenatedTransform Clone()
		{
			var clonedList = new List<ICoordinateTransformation>(_coordinateTransformationList.Count);
			foreach (ICoordinateTransformation ct in _coordinateTransformationList)
				clonedList.Add(ct);
			return new ConcatenatedTransform(clonedList);
		}

        /// <summary>
        /// Gets a Well-Known text representation of this object.
        /// </summary>
        /// <value></value>
        public override string WKT
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        /// <value></value>
		public override string XML
		{
			get { throw new NotImplementedException(); }
		}
	}
}
