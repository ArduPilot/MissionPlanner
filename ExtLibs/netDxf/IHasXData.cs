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

using netDxf.Collections;
using netDxf.Tables;

namespace netDxf
{
    public delegate void XDataAddAppRegEventHandler(IHasXData sender, ObservableCollectionEventArgs<ApplicationRegistry> e);
    public delegate void XDataRemoveAppRegEventHandler(IHasXData sender, ObservableCollectionEventArgs<ApplicationRegistry> e);

    /// <summary>
    /// Supports <see cref="DxfObject">DxfObjects</see> that contain extended data information.
    /// </summary>
    public interface IHasXData
    {
        event XDataAddAppRegEventHandler XDataAddAppReg;
        event XDataRemoveAppRegEventHandler XDataRemoveAppReg;

        /// <summary>
        /// Gets the object <see cref="XDataDictionary">extended data</see>.
        /// </summary>
        XDataDictionary XData { get; }
    }
}