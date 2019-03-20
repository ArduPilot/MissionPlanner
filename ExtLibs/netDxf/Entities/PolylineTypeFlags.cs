#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
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

namespace netDxf.Entities
{
    /// <summary>
    /// Defines the polyline type.
    /// </summary>
    /// <remarks>Bit flag.</remarks>
    [Flags]
    internal enum PolylinetypeFlags
    {
        /// <summary>
        /// Default, open polyline.
        /// </summary>
        OpenPolyline = 0,

        /// <summary>
        /// This is a closed polyline (or a polygon mesh closed in the M direction).
        /// </summary>
        ClosedPolylineOrClosedPolygonMeshInM = 1,

        /// <summary>
        /// Curve-fit vertexes have been added.
        /// </summary>
        CurveFit = 2,

        /// <summary>
        /// Spline-fit vertexes have been added.
        /// </summary>
        SplineFit = 4,

        /// <summary>
        /// This is a 3D polyline.
        /// </summary>
        Polyline3D = 8,

        /// <summary>
        /// This is a 3D polygon mesh.
        /// </summary>
        PolygonMesh = 16,

        /// <summary>
        /// The polygon mesh is closed in the N direction.
        /// </summary>
        ClosedPolygonMeshInN = 32,

        /// <summary>
        /// The polyline is a polyface mesh.
        /// </summary>
        PolyfaceMesh = 64,

        /// <summary>
        /// The line type pattern is generated continuously around the vertexes of this polyline.
        /// </summary>
        ContinuousLinetypePattern = 128
    }
}