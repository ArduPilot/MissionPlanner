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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of dimension styles.
    /// </summary>
    public sealed class DimensionStyles :
        TableObjects<DimensionStyle>
    {
        #region constructor

        internal DimensionStyles(DxfDocument document)
            : this(document, null)
        {
        }

        internal DimensionStyles(DxfDocument document, string handle)
            : base(document, DxfObjectCode.DimensionStyleTable, handle)
        {
            this.MaxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a dimension style to the list.
        /// </summary>
        /// <param name="style"><see cref="DimensionStyle">DimensionStyle</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the dimension style parameter.</param>
        /// <returns>
        /// If a dimension style already exists with the same name as the instance that is being added the method returns the existing dimension style,
        /// if not it will return the new dimension style.
        /// </returns>
        internal override DimensionStyle Add(DimensionStyle style, bool assignHandle)
        {
            if (this.list.Count >= this.MaxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.CodeName, this.MaxCapacity));
            if (style == null)
                throw new ArgumentNullException(nameof(style));

            DimensionStyle add;
            if (this.list.TryGetValue(style.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(style.Handle))
                this.Owner.NumHandles = style.AsignHandle(this.Owner.NumHandles);

            this.list.Add(style.Name, style);
            this.references.Add(style.Name, new List<DxfObject>());

            // add referenced text style
            style.TextStyle = this.Owner.TextStyles.Add(style.TextStyle, assignHandle);
            this.Owner.TextStyles.References[style.TextStyle.Name].Add(style);

            // add referenced blocks
            if (style.LeaderArrow != null)
            {
                style.LeaderArrow = this.Owner.Blocks.Add(style.LeaderArrow, assignHandle);
                this.Owner.Blocks.References[style.LeaderArrow.Name].Add(style);
            }

            if (style.DimArrow1 != null)
            {
                style.DimArrow1 = this.Owner.Blocks.Add(style.DimArrow1, assignHandle);
                this.Owner.Blocks.References[style.DimArrow1.Name].Add(style);
            }
            if (style.DimArrow2 != null)
            {
                style.DimArrow2 = this.Owner.Blocks.Add(style.DimArrow2, assignHandle);
                this.Owner.Blocks.References[style.DimArrow2.Name].Add(style);
            }

            // add referenced line types
            style.DimLineLinetype = this.Owner.Linetypes.Add(style.DimLineLinetype, assignHandle);
            this.Owner.Linetypes.References[style.DimLineLinetype.Name].Add(style);

            style.ExtLine1Linetype = this.Owner.Linetypes.Add(style.ExtLine1Linetype, assignHandle);
            this.Owner.Linetypes.References[style.ExtLine1Linetype.Name].Add(style);

            style.ExtLine2Linetype = this.Owner.Linetypes.Add(style.ExtLine2Linetype, assignHandle);
            this.Owner.Linetypes.References[style.ExtLine2Linetype.Name].Add(style);

            style.Owner = this;

            style.NameChanged += this.Item_NameChanged;
            style.LinetypeChanged += this.DimensionStyleLinetypeChanged;
            style.TextStyleChanged += this.DimensionStyleTextStyleChanged;
            style.BlockChanged += this.DimensionStyleBlockChanged;

            this.Owner.AddedObjects.Add(style.Handle, style);

            return style;
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="name"><see cref="DimensionStyle">DimensionStyle</see> name to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="item"><see cref="DimensionStyle">DimensionStyle</see> to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(DimensionStyle item)
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


            // remove referenced text style
            this.Owner.TextStyles.References[item.TextStyle.Name].Remove(item);

            // remove referenced blocks
            if (item.DimArrow1 != null)
                this.Owner.Blocks.References[item.DimArrow1.Name].Remove(item);
            if (item.DimArrow2 != null)
                this.Owner.Blocks.References[item.DimArrow2.Name].Remove(item);

            // remove referenced line types
            this.Owner.Linetypes.References[item.DimLineLinetype.Name].Remove(item);
            this.Owner.Linetypes.References[item.ExtLine1Linetype.Name].Remove(item);
            this.Owner.Linetypes.References[item.ExtLine2Linetype.Name].Remove(item);

            this.references.Remove(item.Name);
            this.list.Remove(item.Name);

            item.Handle = null;
            item.Owner = null;

            item.NameChanged -= this.Item_NameChanged;
            item.LinetypeChanged -= this.DimensionStyleLinetypeChanged;
            item.TextStyleChanged -= this.DimensionStyleTextStyleChanged;
            item.BlockChanged -= this.DimensionStyleBlockChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another dimension style with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (DimensionStyle) sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        private void DimensionStyleLinetypeChanged(TableObject sender, TableObjectChangedEventArgs<Linetype> e)
        {
            this.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.Owner.Linetypes.Add(e.NewValue);
            this.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleTextStyleChanged(TableObject sender, TableObjectChangedEventArgs<TextStyle> e)
        {
            this.Owner.TextStyles.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.Owner.TextStyles.Add(e.NewValue);
            this.Owner.TextStyles.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleBlockChanged(TableObject sender, TableObjectChangedEventArgs<Block> e)
        {
            if (e.OldValue != null)
                this.Owner.Blocks.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.Owner.Blocks.Add(e.NewValue);
            if (e.NewValue != null)
                this.Owner.Blocks.References[e.NewValue.Name].Add(sender);
        }

        #endregion
    }
}