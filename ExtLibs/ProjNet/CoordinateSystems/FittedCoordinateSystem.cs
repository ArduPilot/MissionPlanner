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
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// A coordinate system which sits inside another coordinate system. The fitted 
    /// coordinate system can be rotated and shifted, or use any other math transform
    /// to inject itself into the base coordinate system.
    /// </summary>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable]
#endif
    public class FittedCoordinateSystem : CoordinateSystem, IFittedCoordinateSystem
    {
		/// <summary>
		/// Creates an instance of FittedCoordinateSystem using the specified parameters
		/// </summary>
        /// <param name="baseSystem">Underlying coordinate system.</param>
        /// <param name="transform">Transformation from fitted coordinate system to the base one</param>
		/// <param name="name">Name</param>
		/// <param name="authority">Authority name</param>
		/// <param name="code">Authority-specific identification code.</param>
		/// <param name="alias">Alias</param>
		/// <param name="abbreviation">Abbreviation</param>
		/// <param name="remarks">Provider-supplied remarks</param>
        protected internal FittedCoordinateSystem (ICoordinateSystem baseSystem, IMathTransform transform,
            string name, string authority, long code, string alias, string remarks, string abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
            _BaseCoordinateSystem = baseSystem;
            _ToBaseTransform = transform;
            //get axis infos from the source
            base.AxisInfo = new List<AxisInfo> (baseSystem.Dimension);
            for (int dim = 0; dim < baseSystem.Dimension; dim++)
            {
                base.AxisInfo.Add (baseSystem.GetAxis (dim));
            }
		}

        #region public properties

        private IMathTransform _ToBaseTransform;

        /// <summary>
        /// Represents math transform that injects itself into the base coordinate system.
        /// </summary>
        public IMathTransform ToBaseTransform
        {
            get { return _ToBaseTransform; }
        }
        #endregion public properties

        #region IFittedCoordinateSystem Members

        private ICoordinateSystem _BaseCoordinateSystem;

        /// <summary>
        /// Gets underlying coordinate system.
        /// </summary>
        public ICoordinateSystem BaseCoordinateSystem
        {
            get { return _BaseCoordinateSystem; }
        }

        /// <summary>
        /// Gets Well-Known Text of a math transform to the base coordinate system. 
        /// The dimension of this fitted coordinate system is determined by the source 
        /// dimension of the math transform. The transform should be one-to-one within 
        /// this coordinate system's domain, and the base coordinate system dimension 
        /// must be at least as big as the dimension of this coordinate system.
        /// </summary>
        /// <returns></returns>
        public string ToBase ()
        {
            return _ToBaseTransform.WKT;
        }
        #endregion IFittedCoordinateSystem Members

        #region ICoordinateSystem Members

        /// <summary>
        /// Returns the Well-known text for this object as defined in the simple features specification.
        /// </summary>
        public override string WKT
        {
            get
            {
                //<fitted cs>          = FITTED_CS["<name>", <to base>, <base cs>]

				StringBuilder sb = new StringBuilder();
                sb.AppendFormat ("FITTED_CS[\"{0}\", {1}, {2}]", Name, this._ToBaseTransform.WKT, this._BaseCoordinateSystem.WKT);
				return sb.ToString();
            }
        }

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        public override string XML
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// Only parameters used for coordinate system are used for comparison.
        /// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal</returns>
        public override bool EqualParams (object obj)
        {
            IFittedCoordinateSystem fcs = obj as IFittedCoordinateSystem;
            if (fcs != null)
            {
                if (fcs.BaseCoordinateSystem.EqualParams (this.BaseCoordinateSystem))
                {
                    string fcsToBase = fcs.ToBase ();
                    string thisToBase = this.ToBase ();
                    if (string.Equals (fcsToBase, thisToBase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the units for the dimension within coordinate system. 
        /// Each dimension in the coordinate system has corresponding units.
        /// </summary>
        public override IUnit GetUnits (int dimension)
        {
            return _BaseCoordinateSystem.GetUnits (dimension);
        }

        #endregion
    }
}
