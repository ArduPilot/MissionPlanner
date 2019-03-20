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
    /// Represents a collection of text styles.
    /// </summary>
    public sealed class TextStyles :
        TableObjects<TextStyle>
    {
        #region constructor

        internal TextStyles(DxfDocument document)
            : this(document, null)
        {
        }

        internal TextStyles(DxfDocument document, string handle)
            : base(document, DxfObjectCode.TextStyleTable, handle)
        {
            this.MaxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a text style to the list.
        /// </summary>
        /// <param name="style"><see cref="TextStyle">TextStyle</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the text style parameter.</param>
        /// <returns>
        /// If a text style already exists with the same name as the instance that is being added the method returns the existing text style,
        /// if not it will return the new text style.
        /// </returns>
        internal override TextStyle Add(TextStyle style, bool assignHandle)
        {
            if (this.list.Count >= this.MaxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.CodeName, this.MaxCapacity));
            if (style == null)
                throw new ArgumentNullException(nameof(style));

            TextStyle add;
            if (this.list.TryGetValue(style.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(style.Handle))
                this.Owner.NumHandles = style.AsignHandle(this.Owner.NumHandles);

            this.list.Add(style.Name, style);
            this.references.Add(style.Name, new List<DxfObject>());

            style.Owner = this;

            style.NameChanged += this.Item_NameChanged;

            this.Owner.AddedObjects.Add(style.Handle, style);

            return style;
        }

        /// <summary>
        /// Removes a text style.
        /// </summary>
        /// <param name="name"><see cref="TextStyle">TextStyle</see> name to remove from the document.</param>
        /// <returns>True if the text style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved text styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a text style.
        /// </summary>
        /// <param name="item"><see cref="TextStyle">TextStyle</see> to remove from the document.</param>
        /// <returns>True if the text style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved text styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(TextStyle item)
        {
            if (item == null)
                return false;

            if (!this.Contains(item))
                return false;

            if (item.IsReserved)
                return false;

            if (this.references[item.Name].Count != 0)
                return false;

            this.Owner.AddedObjects.Remove(item.Handle);
            this.references.Remove(item.Name);
            this.list.Remove(item.Name);

            item.Handle = null;
            item.Owner = null;

            item.NameChanged -= this.Item_NameChanged;

            return true;
        }

        #endregion

        #region TextStyle events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another text style with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (TextStyle) sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        #endregion
    }
}