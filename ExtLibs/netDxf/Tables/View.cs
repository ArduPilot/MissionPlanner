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
    public class View :
        TableObject
    {
        #region private fields

        private Vector3 target;
        private Vector3 camera;
        private double height;
        private double width;
        private double rotation;
        private ViewModeFlags viewmode;
        private double fov;
        private double frontClippingPlane;
        private double backClippingPlane;

        #endregion

        #region constants

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>View</c> class.
        /// </summary>
        public View(string name)
            : this(name, true)
        {
        }

        internal View(string name, bool checkName)
            : base(name, DxfObjectCode.View, checkName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The view name should be at least one character long.");

            this.IsReserved = false;
            this.target = Vector3.Zero;
            this.camera = Vector3.UnitZ;
            this.height = 1.0;
            this.width = 1.0;
            this.rotation = 0.0;
            this.viewmode = ViewModeFlags.Off;
            this.fov = 40.0;
            this.frontClippingPlane = 0.0;
            this.backClippingPlane = 0.0;
        }

        #endregion

        #region public properties

        public Vector3 Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        public Vector3 Camera
        {
            get { return this.camera; }
            set { this.camera = value; }
        }

        public double Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public double Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        public ViewModeFlags Viewmode
        {
            get { return this.viewmode; }
            set { this.viewmode = value; }
        }

        public double Fov
        {
            get { return this.fov; }
            set { this.fov = value; }
        }

        public double FrontClippingPlane
        {
            get { return this.frontClippingPlane; }
            set { this.frontClippingPlane = value; }
        }

        public double BackClippingPlane
        {
            get { return this.backClippingPlane; }
            set { this.backClippingPlane = value; }
        }

        /// <summary>
        /// Gets the owner of the actual view.
        /// </summary>
        public new Views Owner
        {
            get { return (Views) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new View that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">View name of the copy.</param>
        /// <returns>A new View that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            View copy = new View(newName)
            {
                Target = this.target,
                Camera = this.camera,
                Height = this.height,
                Width = this.width,
                Rotation = this.rotation,
                Viewmode = this.viewmode,
                Fov = this.fov,
                FrontClippingPlane = this.frontClippingPlane,
                BackClippingPlane = this.backClippingPlane
            };

            foreach (XData data in this.XData.Values)
                copy.XData.Add((XData)data.Clone());

            return copy;
        }

        /// <summary>
        /// Creates a new View that is a copy of the current instance.
        /// </summary>
        /// <returns>A new View that is a copy of this instance.</returns>
        public override object Clone()
        {
            return this.Clone(this.Name);
        }

        #endregion
    }
}