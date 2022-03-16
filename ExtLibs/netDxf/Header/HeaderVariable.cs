﻿#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

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

namespace netDxf.Header
{
    /// <summary>
    /// Defines a header variable.
    /// </summary>
    internal class HeaderVariable
    {
        #region private fields

        private readonly string name;
        private object variable;

        #endregion

        #region constructors

        public HeaderVariable(string name, object value)
        {
            this.name = name;
            this.variable = value;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the header variable name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the header variable stored value.
        /// </summary>
        public object Value
        {
            get { return this.variable; }
            set { this.variable = value; }
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.name, this.variable);
        }

        #endregion
    }
}