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
using System.Collections.Generic;
using netDxf.Tables;

namespace netDxf
{
    /// <summary>
    /// Represents the extended data information.
    /// </summary>
    /// <remarks>
    /// Do not store your own data under the ACAD application registry it is used by some entities to store special data,
    /// it might be overwritten when the file is saved. Instead, create a new application registry and store your data there.
    /// </remarks>
    public class XData :
        ICloneable
    {
        #region private fields

        private ApplicationRegistry appReg;
        private readonly List<XDataRecord> xData;

        #endregion

        #region constructors

        /// <summary>
        /// Initialize a new instance of the <c>XData</c> class .
        /// </summary>
        /// <param name="appReg">Name of the application associated with the list of extended data records.</param>
        public XData(ApplicationRegistry appReg)
        {
            if(appReg == null)
                throw new ArgumentNullException(nameof(appReg));
            this.appReg = appReg;
            this.xData = new List<XDataRecord>();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the name of the application associated with the list of extended data records.
        /// </summary>
        public ApplicationRegistry ApplicationRegistry
        {
            get { return this.appReg; }
            internal set { this.appReg = value; }
        }

        /// <summary>
        /// Gets or sets the list of extended data records.
        /// </summary>
        /// <remarks>
        /// This list cannot contain a XDataRecord with a XDataCode of AppReg, this code is reserved to register the name of the application.
        /// Any record with this code will be omitted.
        /// </remarks>
        public List<XDataRecord> XDataRecord
        {
            get { return this.xData; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.appReg.Name;
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new XData that is a copy of the current instance.
        /// </summary>
        /// <returns>A new XData that is a copy of this instance.</returns>
        public object Clone()
        {
            XData xdata = new XData((ApplicationRegistry) this.appReg.Clone());
            foreach (XDataRecord record in this.xData)
                xdata.XDataRecord.Add(new XDataRecord(record.Code, record.Value));

            return xdata;
        }

        #endregion
    }
}