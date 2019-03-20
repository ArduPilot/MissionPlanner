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
    /// Defines the vertex type.
    /// </summary>
    [Flags]
    internal enum VertexTypeFlags
    {
        /// <summary>
        /// 2d polyline vertex.
        /// </summary>
        PolylineVertex = 0,

        /// <summary>
        /// Extra vertex created by curve-fitting.
        /// </summary>
        CurveFittingExtraVertex = 1,

        /// <summary>
        /// Curve-fit tangent defined for this vertex.
        /// A curve-fit tangent direction of 0 may be omitted from DXF output but is significant if this bit is set.
        /// </summary>
        CurveFitTangent = 2,

        /// <summary>
        /// Not used.
        /// </summary>
        NotUsed = 4,

        /// <summary>
        /// Spline vertex created by spline-fitting.
        /// </summary>
        SplineVertexFromSplineFitting = 8,

        /// <summary>
        /// Spline frame control point.
        /// </summary>
        SplineFrameControlPoint = 16,

        /// <summary>
        /// 3D polyline vertex.
        /// </summary>
        Polyline3dVertex = 32,

        /// <summary>
        /// 3D polygon mesh.
        /// </summary>
        Polygon3dMesh = 64,

        /// <summary>
        /// Polyface mesh vertex.
        /// </summary>
        PolyfaceMeshVertex = 128
    }
}