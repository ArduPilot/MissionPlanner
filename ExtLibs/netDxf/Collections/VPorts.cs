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

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of viewports.
    /// </summary>
    public sealed class VPorts :
        TableObjects<VPort>
    {
        #region constructor

        internal VPorts(DxfDocument document)
            : this(document, null)
        {
        }

        internal VPorts(DxfDocument document, string handle)
            : base(document, DxfObjectCode.VportTable, handle)
        {
            this.MaxCapacity = short.MaxValue;

            if (this.list.Count >= this.MaxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.CodeName, this.MaxCapacity));

            // add the current document viewport, it is always present
            VPort active = VPort.Active;
            this.Owner.NumHandles = active.AsignHandle(this.Owner.NumHandles);

            this.Owner.AddedObjects.Add(active.Handle, active);
            this.list.Add(active.Name, active);
            this.references.Add(active.Name, new List<DxfObject>());
            active.Owner = this;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds an viewports to the list.
        /// </summary>
        /// <param name="vport"><see cref="VPort">VPort</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the viewport parameter.</param>
        /// <returns>
        /// If a viewports already exists with the same name as the instance that is being added the method returns the existing viewports,
        /// if not it will return the new viewports.
        /// </returns>
        internal override VPort Add(VPort vport, bool assignHandle)
        {
            throw new ArgumentException("VPorts cannot be added to the collection. There is only one VPort in the list the \"*Active\".", nameof(vport));

            //if (this.list.Count >= this.maxCapacity)
            //    throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.codeName, this.maxCapacity));

            //VPort add;
            //if (this.list.TryGetValue(vport.Name, out add))
            //    return add;

            //if (assignHandle || string.IsNullOrEmpty(vport.Handle))
            //    this.document.NumHandles = vport.AsignHandle(this.document.NumHandles);

            //this.list.Add(vport.Name, vport);
            //this.references.Add(vport.Name, new List<DxfObject>());
            //vport.Owner = this;
            //this.document.AddedObjects.Add(vport.Handle, vport);
            //return vport;
        }

        /// <summary>
        /// Removes a viewports.
        /// </summary>
        /// <param name="name"><see cref="VPort">VPort</see> name to remove from the document.</param>
        /// <returns>True if the viewports has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved viewports or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            throw new ArgumentException("VPorts cannot be removed from the collection.", nameof(name));

            //return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a viewports.
        /// </summary>
        /// <param name="item"><see cref="VPort">VPort</see> to remove from the document.</param>
        /// <returns>True if the viewports has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved viewports or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(VPort item)
        {
            throw new ArgumentException("VPorts cannot be removed from the collection.", nameof(item));

            //if (vport == null)
            //    return false;

            //if (!this.Contains(vport))
            //    return false;

            //if (vport.IsReserved)
            //    return false;

            //if (this.references[vport.Name].Count != 0)
            //    return false;

            //vport.Owner = null;
            //this.document.AddedObjects.Remove(vport.Handle);
            //this.references.Remove(vport.Name);
            //this.list.Remove(vport.Name);

            //return true;
        }

        #endregion
    }
}