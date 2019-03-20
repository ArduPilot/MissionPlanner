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
    /// <summary>
    /// Event data for changes or substitutions of table objects in entities or other tables.
    /// </summary>
    /// <typeparam name="T">A table object</typeparam>
    public class TableObjectChangedEventArgs<T> :
        EventArgs
    {
        #region private fields

        private readonly T oldValue;
        private T newValue;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>TableObjectModifiedEventArgs</c>.
        /// </summary>
        /// <param name="oldTable">The previous table object.</param>
        /// <param name="newTable">The new table object.</param>
        public TableObjectChangedEventArgs(T oldTable, T newTable)
        {
            this.oldValue = oldTable;
            this.newValue = newTable;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the previous property value.
        /// </summary>
        public T OldValue
        {
            get { return this.oldValue; }
        }

        /// <summary>
        /// Gets or sets the new property value.
        /// </summary>
        public T NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }

        #endregion
    }
}