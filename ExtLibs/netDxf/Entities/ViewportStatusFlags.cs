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
    /// viewport status flags
    /// </summary>
    [Flags]
    public enum ViewportStatusFlags
    {
        /// <summary>
        /// Enables perspective mode.
        /// </summary>
        PerspectiveMode = 1,

        /// <summary>
        /// Enables front clipping.
        /// </summary>
        FrontClipping = 2,

        /// <summary>
        /// Enables back clipping.
        /// </summary>
        BackClipping = 4,

        /// <summary>
        /// Enables UCS follow.
        /// </summary>
        UcsFollow = 8,

        /// <summary>
        /// Enables front clip not at eye.
        /// </summary>
        FrontClipNotAtEye = 16,

        /// <summary>
        /// Enables UCS icon visibility.
        /// </summary>
        UcsIconVisibility = 32,

        /// <summary>
        /// Enables UCS icon at origin.
        /// </summary>
        UcsIconAtOrigin = 64,

        /// <summary>
        /// Enables fast zoom.
        /// </summary>
        FastZoom = 128,

        /// <summary>
        /// Enables snap mode.
        /// </summary>
        SnapMode = 256,

        /// <summary>
        /// Enables grid mode.
        /// </summary>
        GridMode = 512,

        /// <summary>
        /// Enables isometric snap style.
        /// </summary>
        IsometricSnapStyle = 1024,

        /// <summary>
        /// Enables hide plot mode.
        /// </summary>
        HidePlotMode = 2048,

        /// <summary>
        /// If set and IsoPairRight is not set, then isopair top is enabled. If both IsoPairTop and IsoPairRight are set, then isopair left is enabled
        /// </summary>
        IsoPairTop = 4096,

        /// <summary>
        /// If set and IsoPairTop is not set, then isopair right is enabled.
        /// </summary>
        IsoPairRight = 8192,

        /// <summary>
        /// Enables viewport zoom locking.
        /// </summary>
        ViewportZoomLocking = 16384,

        /// <summary>
        /// Currently always enabled.
        /// </summary>
        CurrentlyAlwaysEnabled = 32768,

        /// <summary>
        /// Enables non-rectangular clipping.
        /// </summary>
        NonRectangularClipping = 65536,

        /// <summary>
        /// Turns the viewport off.
        /// </summary>
        ViewportOff = 131072,

        /// <summary>
        /// Enables the display of the grid beyond the drawing limits.
        /// </summary>
        DisplayGridBeyondDrawingLimits = 262144,

        /// <summary>
        /// Enable adaptive grid display.
        /// </summary>
        AdaptiveGridDisplay = 524288,

        /// <summary>
        /// Enables subdivision of the grid below the set grid spacing when the grid display is adaptive.
        /// </summary>
        SubdivisionGridBelowSpacing = 1048576
    }
}