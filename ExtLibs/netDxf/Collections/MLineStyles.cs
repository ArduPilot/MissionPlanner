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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of multiline styles.
    /// </summary>
    public sealed class MLineStyles :
        TableObjects<MLineStyle>
    {
        #region constructor

        internal MLineStyles(DxfDocument document)
            : this(document, null)
        {
        }

        internal MLineStyles(DxfDocument document, string handle)
            : base(document, DxfObjectCode.MLineStyleDictionary, handle)
        {
            this.MaxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a multiline style to the list.
        /// </summary>
        /// <param name="style"><see cref="MLineStyle">MLineStyle</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the multiline style parameter.</param>
        /// <returns>
        /// If a multiline style already exists with the same name as the instance that is being added the method returns the existing multiline style,
        /// if not it will return the new multiline style.
        /// </returns>
        internal override MLineStyle Add(MLineStyle style, bool assignHandle)
        {
            if (this.list.Count >= this.MaxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.CodeName, this.MaxCapacity));
            if (style == null)
                throw new ArgumentNullException(nameof(style));

            MLineStyle add;
            if (this.list.TryGetValue(style.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(style.Handle))
                this.Owner.NumHandles = style.AsignHandle(this.Owner.NumHandles);

            this.list.Add(style.Name, style);
            this.references.Add(style.Name, new List<DxfObject>());
            foreach (MLineStyleElement element in style.Elements)
            {
                element.Linetype = this.Owner.Linetypes.Add(element.Linetype);
                this.Owner.Linetypes.References[element.Linetype.Name].Add(style);
            }

            style.Owner = this;

            style.NameChanged += this.Item_NameChanged;
            style.MLineStyleElementAdded += this.MLineStyle_ElementAdded;
            style.MLineStyleElementRemoved += this.MLineStyle_ElementRemoved;
            style.MLineStyleElementLinetypeChanged += this.MLineStyle_ElementLinetypeChanged;

            this.Owner.AddedObjects.Add(style.Handle, style);

            return style;
        }

        /// <summary>
        /// Removes a multiline style.
        /// </summary>
        /// <param name="name"><see cref="MLineStyle">MLineStyle</see> name to remove from the document.</param>
        /// <returns>True if the multiline style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved multiline styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a multiline style.
        /// </summary>
        /// <param name="item"><see cref="MLineStyle">MLineStyle</see> to remove from the document.</param>
        /// <returns>True if the multiline style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved multiline styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(MLineStyle item)
        {
            if (item == null)
                return false;

            if (!this.Contains(item))
                return false;

            if (item.IsReserved)
                return false;

            if (this.references[item.Name].Count != 0)
                return false;

            foreach (MLineStyleElement element in item.Elements)
            {
                this.Owner.Linetypes.References[element.Linetype.Name].Remove(item);
            }

            this.Owner.AddedObjects.Remove(item.Handle);
            this.references.Remove(item.Name);
            this.list.Remove(item.Name);

            item.Handle = null;
            item.Owner = null;

            item.NameChanged -= this.Item_NameChanged;
            item.MLineStyleElementAdded -= this.MLineStyle_ElementAdded;
            item.MLineStyleElementRemoved -= this.MLineStyle_ElementRemoved;
            item.MLineStyleElementLinetypeChanged -= this.MLineStyle_ElementLinetypeChanged;

            return true;
        }

        #endregion

        #region MLineStyle events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another multiline style with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (MLineStyle) sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        private void MLineStyle_ElementLinetypeChanged(MLineStyle sender, TableObjectChangedEventArgs<Linetype> e)
        {
            this.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
            this.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        private void MLineStyle_ElementAdded(MLineStyle sender, MLineStyleElementChangeEventArgs e)
        {
            e.Item.Linetype = this.Owner.Linetypes.Add(e.Item.Linetype);
            //this.Owner.Linetypes.References[e.Item.Linetype.Name].Add(sender);
        }

        private void MLineStyle_ElementRemoved(MLineStyle sender, MLineStyleElementChangeEventArgs e)
        {
            this.Owner.Linetypes.References[e.Item.Linetype.Name].Remove(sender);
        }

        #endregion
    }
}