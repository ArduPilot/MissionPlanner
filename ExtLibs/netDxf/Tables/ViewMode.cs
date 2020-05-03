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

namespace netDxf.Tables
{
    [Flags]
    public enum ViewModeFlags
    {
        /// <summary>
        /// Turned off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Perspective view active.
        /// </summary>
        Perspective = 1,

        /// <summary>
        /// Front clipping on.
        /// </summary>
        FrontClippingPlane = 2,

        /// <summary>
        /// Back clipping on.
        /// </summary>
        BackClippingPlane = 4,

        /// <summary>
        /// UCS Follow mode on.
        /// </summary>
        UCSFollow = 8,

        /// <summary>
        /// Front clip not at eye. If on, the front clip distance (FRONTZ) determines the front clipping plane.
        /// If off, FRONTZ is ignored, and the front clipping plane is set to pass through the camera point (vectors behind the camera are not displayed).
        /// This flag is ignored if the front-clipping bit (2) is off.
        /// </summary>
        FrontClipNotAtEye = 16
    }
}