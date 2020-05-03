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

namespace netDxf.Entities
{
    /// <summary>
    /// Defines the entity type.
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// Arc entity.
        /// </summary>
        Arc,

        /// <summary>
        /// Circle entity.
        /// </summary>
        Circle,

        /// <summary>
        /// Dimension entity.
        /// </summary>
        Dimension,

        /// <summary>
        /// Ellipse entity.
        /// </summary>
        Ellipse,

        /// <summary>
        /// 3d face entity.
        /// </summary>
        Face3D,

        /// <summary>
        /// Hatch entity.
        /// </summary>
        Hatch,

        /// <summary>
        /// A raster image entity.
        /// </summary>
        Image,

        /// <summary>
        /// Block insertion entity.
        /// </summary>
        Insert,

        /// <summary>
        /// Leader entity.
        /// </summary>
        Leader,

        /// <summary>
        /// Lightweight polyline entity.
        /// </summary>
        LightWeightPolyline,

        /// <summary>
        /// Line entity.
        /// </summary>
        Line,

        /// <summary>
        /// Mesh entity.
        /// </summary>
        Mesh,

        /// <summary>
        /// Multiline entity.
        /// </summary>
        MLine,

        /// <summary>
        /// Multiline text string entity.
        /// </summary>
        MText,

        /// <summary>
        /// Point entity.
        /// </summary>
        Point,

        /// <summary>
        /// Polyface mesh entity.
        /// </summary>
        PolyfaceMesh,

        /// <summary>
        /// 3d polyline entity.
        /// </summary>
        Polyline,

        /// <summary>
        /// Ray entity.
        /// </summary>
        Ray,

        /// <summary>
        /// Shape entity.
        /// </summary>
        Shape,

        /// <summary>
        /// Solid entity.
        /// </summary>
        Solid,

        /// <summary>
        /// Spline (nonuniform rational B-splines NURBS).
        /// </summary>
        Spline,

        /// <summary>
        /// Text string entity.
        /// </summary>
        Text,

        /// <summary>
        /// Tolerance entity.
        /// </summary>
        Tolerance,

        /// <summary>
        /// Trace entity.
        /// </summary>
        Trace,

        /// <summary>
        /// Underlay entity.
        /// </summary>
        Underlay,

        /// <summary>
        /// Viewport entity.
        /// </summary>
        Viewport,

        /// <summary>
        /// Wipeout entity.
        /// </summary>
        Wipeout,

        /// <summary>
        /// XLine entity.
        /// </summary>
        XLine
    }
}