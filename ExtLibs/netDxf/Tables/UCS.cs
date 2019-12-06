#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a User Coordinate System.
    /// </summary>
    public class UCS :
        TableObject
    {
        #region private fields

        private Vector3 origin;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private double elevation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>UCS</c> class.
        /// </summary>
        /// <param name="name">User coordinate system name.</param>
        public UCS(string name)
            : this(name, true)
        {
        }

        internal UCS(string name, bool checkName)
            : base(name, DxfObjectCode.Ucs, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The UCS name should be at least one character long.");

            this.origin = Vector3.Zero;
            this.xAxis = Vector3.UnitX;
            this.yAxis = Vector3.UnitY;
            this.zAxis = Vector3.UnitZ;
            this.elevation = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <c>UCS</c> class.
        /// </summary>
        /// <param name="name">User coordinate system name.</param>
        /// <param name="origin">Origin in WCS.</param>
        /// <param name="xDirection">X-axis direction in WCS.</param>
        /// <param name="yDirection">Y-axis direction in WCS.</param>
        /// <remarks>
        /// The x-axis direction and y-axis direction must be perpendicular.
        /// </remarks>
        public UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection)
            : this(name, origin, xDirection, yDirection, true)
        {
        }

        internal UCS(string name, Vector3 origin, Vector3 xDirection, Vector3 yDirection, bool checkName)
            : base(name, DxfObjectCode.Ucs, checkName)
        {
            if (!Vector3.ArePerpendicular(xDirection, yDirection))
                throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
            this.origin = origin;
            this.xAxis = xDirection;
            this.xAxis.Normalize();
            this.yAxis = yDirection;
            this.yAxis.Normalize();
            this.zAxis = Vector3.CrossProduct(this.xAxis, this.yAxis);
            this.elevation = 0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the user coordinate system origin in WCS.
        /// </summary>
        public Vector3 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets the user coordinate system x-axis direction in WCS.
        /// </summary>
        public Vector3 XAxis
        {
            get { return this.xAxis; }
        }

        /// <summary>
        /// Gets the user coordinate system y-axis direction in WCS.
        /// </summary>
        public Vector3 YAxis
        {
            get { return this.yAxis; }
        }

        /// <summary>
        /// Gets the user coordinate system z-axis direction in WCS.
        /// </summary>
        public Vector3 ZAxis
        {
            get { return this.zAxis; }
        }

        /// <summary>
        /// Gets or sets the user coordinate system elevation.
        /// </summary>
        public double Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        /// <summary>
        /// Gets the owner of the actual user coordinate system.
        /// </summary>
        public new UCSs Owner
        {
            get { return (UCSs) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sets the user coordinate system x-axis and y-axis direction.
        /// </summary>
        /// <param name="xDirection">X-axis direction in WCS.</param>
        /// <param name="yDirection">Y-axis direction in WCS.</param>
        public void SetAxis(Vector3 xDirection, Vector3 yDirection)
        {
            if (!Vector3.ArePerpendicular(xDirection, yDirection))
                throw new ArgumentException("X-axis direction and Y-axis direction must be perpendicular.");
            this.xAxis = xDirection;
            this.xAxis.Normalize();
            this.yAxis = yDirection;
            this.yAxis.Normalize();
            this.zAxis = Vector3.CrossProduct(this.xAxis, this.yAxis);
        }

        /// <summary>
        /// Creates a new user coordinate system from the x-axis and a point on XY plane.
        /// </summary>
        /// <param name="name">User coordinate system name.</param>
        /// <param name="origin">Origin in WCS.</param>
        /// <param name="xDirection">X-axis direction in WCS.</param>
        /// <param name="pointOnPlaneXY">Point on the XYplane.</param>
        /// <returns>A new user coordinate system.</returns>
        public static UCS FromXAxisAndPointOnXYplane(string name, Vector3 origin, Vector3 xDirection, Vector3 pointOnPlaneXY)
        {
            UCS ucs = new UCS(name);
            ucs.origin = origin;
            ucs.xAxis = xDirection;
            ucs.xAxis.Normalize();
            ucs.zAxis = Vector3.CrossProduct(xDirection, pointOnPlaneXY);
            ucs.zAxis.Normalize();
            ucs.yAxis = Vector3.CrossProduct(ucs.zAxis, ucs.xAxis);
            return ucs;
        }

        /// <summary>
        /// Creates a new user coordinate system from the XY plane normal (z-axis).
        /// </summary>
        /// <param name="name">User coordinate system name.</param>
        /// <param name="origin">Origin in WCS.</param>
        /// <param name="normal">XY plane normal (z-axis).</param>
        /// <param name="rotation">The counter-clockwise angle in radians along the normal (z-axis).</param>
        /// <returns>A new user coordinate system.</returns>
        /// <remarks>This method uses the ArbitraryAxis algorithm to obtain the user coordinate system x-axis and y-axis.</remarks>
        public static UCS FromNormal(string name, Vector3 origin, Vector3 normal, double rotation)
        {
            Matrix3 mat = MathHelper.ArbitraryAxis(normal);
            Matrix3 rot = Matrix3.RotationZ(rotation);
            mat *= rot;
            UCS ucs = new UCS(name);
            ucs.origin = origin;
            ucs.xAxis = new Vector3(mat.M11, mat.M21, mat.M31);
            ucs.yAxis = new Vector3(mat.M12, mat.M22, mat.M32);
            ucs.zAxis = new Vector3(mat.M13, mat.M23, mat.M33);
            return ucs;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new UCS that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">UCS name of the copy.</param>
        /// <returns>A new UCS that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            UCS copy = new UCS(newName)
            {
                Origin = this.origin,
                xAxis = this.xAxis,
                yAxis = this.yAxis,
                zAxis = this.zAxis,
                Elevation = this.elevation
            };

            foreach (XData data in this.XData.Values)
                copy.XData.Add((XData)data.Clone());

            return copy;
        }

        /// <summary>
        /// Creates a new UCS that is a copy of the current instance.
        /// </summary>
        /// <returns>A new UCS that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.Name);
        }

        #endregion
    }
}