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

using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems.Transformations
{

    /// <summary>
    /// Adjusts target Prime Meridian
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable]
#endif
    internal class PrimeMeridianTransform : MathTransform
    {
        #region class variables
        private bool _isInverted = false;
        private IPrimeMeridian _source;
        private IPrimeMeridian _target;
        #endregion class variables

        #region constructors & finalizers
        /// <summary>
        /// Creates instance prime meridian transform
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public PrimeMeridianTransform(IPrimeMeridian source, IPrimeMeridian target)
            : base()
        {
            if (!source.AngularUnit.EqualParams(target.AngularUnit))
            {
                throw new NotImplementedException("The method or operation is not implemented.");  
            }
            _source = source;
            _target = target;            
        }


        #endregion constructors & finalizers

        #region public properties
        /// <summary>
        /// Gets a Well-Known text representation of this affine math transformation.
        /// </summary>
        /// <value></value>
        public override string WKT
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }
        /// <summary>
        /// Gets an XML representation of this affine transformation.
        /// </summary>
        /// <value></value>
        public override string XML
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Gets the dimension of input points.
        /// </summary>
        public override int DimSource { get { return 3; } }

        /// <summary>
        /// Gets the dimension of output points.
        /// </summary>
        public override int DimTarget { get { return 3; } }
        #endregion public properties

        #region public methods
        /// <summary>
        /// Returns the inverse of this affine transformation.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current affine transformation.</returns>
        public override IMathTransform Inverse()
        {
            return new PrimeMeridianTransform(_target, _source);
        }

        /// <summary>
        /// Transforms a coordinate point. The passed parameter point should not be modified.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double[] Transform(double[] point)
        {
            double[] transformed = new double[point.Length];
            
            if (!_isInverted)
                transformed[0] = point[0] + _source.Longitude - _target.Longitude;
            else
                transformed[0] = point[0] + _target.Longitude - _source.Longitude;
            transformed[1] = point[1];
            if (point.Length > 2)
                transformed[2] = point[2];
            return transformed;
        }

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert()
        {
            this._isInverted = !this._isInverted;
        }

        #endregion public methods
    }
}
