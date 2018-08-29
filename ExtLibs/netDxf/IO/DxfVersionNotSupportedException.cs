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

using netDxf.Header;
using System;

namespace netDxf.IO
{
    /// <summary>
    /// Represents an error that occur when trying to load a DXF file which <see cref="DxfVersion">version</see> is not supported.
    /// </summary>
    /// <remarks>netDxf only supports DXF file versions AutoCad2000 and higher.</remarks>
    public class DxfVersionNotSupportedException :
        Exception
    {
        #region private fields

        private readonly DxfVersion version;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of <c>DxfVersionNotSupportedException</c>
        /// </summary>
        /// <param name="version">DXF file version.</param>
        public DxfVersionNotSupportedException(DxfVersion version)
        {
            this.version = version;
        }

        /// <summary>
        /// Initializes a new instance of <c>DxfVersionNotSupportedException</c>
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="version">DXF file version.</param>
        public DxfVersionNotSupportedException(string message, DxfVersion version)
            :base(message)
        {
            this.version = version;
        }


        #endregion

        #region public properties

        /// <summary>
        /// Gets the DXF file version that generated the exception.
        /// </summary>
        public DxfVersion Version
        {
            get { return this.version; }
        }

        #endregion
    }
}