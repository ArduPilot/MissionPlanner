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
using netDxf.Entities;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of blocks.
    /// </summary>
    public sealed class BlockRecords :
        TableObjects<Block>
    {
        #region constructor

        internal BlockRecords(DxfDocument document)
            : this(document, null)
        {
        }

        internal BlockRecords(DxfDocument document, string handle)
            : base(document, DxfObjectCode.BlockRecordTable, handle)
        {
            this.MaxCapacity = short.MaxValue;
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a block to the list.
        /// </summary>
        /// <param name="block"><see cref="Block">Block</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the block parameter.</param>
        /// <returns>
        /// If a block already exists with the same name as the instance that is being added the method returns the existing block,
        /// if not it will return the new block.
        /// </returns>
        internal override Block Add(Block block, bool assignHandle)
        {
            if (this.list.Count >= this.MaxCapacity)
                throw new OverflowException(string.Format("Table overflow. The maximum number of elements the table {0} can have is {1}", this.CodeName, this.MaxCapacity));

            if (block == null)
                throw new ArgumentNullException(nameof(block));

            Block add;
            if (this.list.TryGetValue(block.Name, out add))
                return add;

            if (assignHandle || string.IsNullOrEmpty(block.Handle))
                this.Owner.NumHandles = block.AsignHandle(this.Owner.NumHandles);

            this.list.Add(block.Name, block);
            this.references.Add(block.Name, new List<DxfObject>());

            block.Layer = this.Owner.Layers.Add(block.Layer);
            this.Owner.Layers.References[block.Layer.Name].Add(block);

            //for new block definitions configure its entities
            foreach (EntityObject entity in block.Entities)
                this.Owner.AddEntityToDocument(entity, block, assignHandle);

            //for new block definitions configure its attributes
            foreach (AttributeDefinition attDef in block.AttributeDefinitions.Values)
                this.Owner.AddAttributeDefinitionToDocument(attDef, assignHandle);

            block.Record.Owner = this;

            block.NameChanged += this.Item_NameChanged;
            block.LayerChanged += this.Block_LayerChanged;
            block.EntityAdded += this.Block_EntityAdded;
            block.EntityRemoved += this.Block_EntityRemoved;
            block.AttributeDefinitionAdded += this.Block_AttributeDefinitionAdded; 
            block.AttributeDefinitionRemoved += this.Block_AttributeDefinitionRemoved;

            this.Owner.AddedObjects.Add(block.Handle, block);
            this.Owner.AddedObjects.Add(block.Owner.Handle, block.Owner);

            return block;
        }

        /// <summary>
        /// Removes a block.
        /// </summary>
        /// <param name="name"><see cref="Block">Block</see> name to remove from the document.</param>
        /// <returns>True if the block has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved blocks or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return this.Remove(this[name]);
        }

        /// <summary>
        /// Removes a block.
        /// </summary>
        /// <param name="item"><see cref="Block">Block</see> to remove from the document.</param>
        /// <returns>True if the block has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved blocks or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(Block item)
        {
            if (item == null)
                return false;

            if (!this.Contains(item))
                return false;

            if (item.IsReserved)
                return false;

            if (this.references[item.Name].Count != 0)
                return false;

            // remove the block from the associated layer
            this.Owner.Layers.References[item.Layer.Name].Remove(item);

            // we will remove all entities from the block definition
            foreach (EntityObject entity in item.Entities)
                this.Owner.RemoveEntityFromDocument(entity);

            // remove all attribute definitions from the associated layers
            foreach (AttributeDefinition attDef in item.AttributeDefinitions.Values)
                this.Owner.RemoveAttributeDefinitionFromDocument(attDef);

            this.Owner.AddedObjects.Remove(item.Handle);
            this.references.Remove(item.Name);
            this.list.Remove(item.Name);

            item.Record.Handle = null;
            item.Record.Owner = null;

            item.Handle = null;
            item.Owner = null;

            item.NameChanged -= this.Item_NameChanged;
            item.LayerChanged -= this.Block_LayerChanged;
            item.EntityAdded -= this.Block_EntityAdded;
            item.EntityRemoved -= this.Block_EntityRemoved;
            item.AttributeDefinitionAdded -= this.Block_AttributeDefinitionAdded;
            item.AttributeDefinitionRemoved -= this.Block_AttributeDefinitionRemoved;

            return true;
        }

        #endregion

        #region Block events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (this.Contains(e.NewValue))
                throw new ArgumentException("There is already another block with the same name.");

            this.list.Remove(sender.Name);
            this.list.Add(e.NewValue, (Block) sender);

            List<DxfObject> refs = this.references[sender.Name];
            this.references.Remove(sender.Name);
            this.references.Add(e.NewValue, refs);
        }

        private void Block_LayerChanged(Block sender, TableObjectChangedEventArgs<Layer> e)
        {
            this.Owner.Layers.References[e.OldValue.Name].Remove(sender);

            e.NewValue = this.Owner.Layers.Add(e.NewValue);
            this.Owner.Layers.References[e.NewValue.Name].Add(sender);
        }

        private void Block_EntityAdded(TableObject sender, BlockEntityChangeEventArgs e)
        {
            if (e.Item.Owner != null)
            {
                // the block and its entities must belong to the same document
                if (!ReferenceEquals(e.Item.Owner.Record.Owner.Owner, this.Owner))
                    throw new ArgumentException("The block and the entity must belong to the same document. Clone it instead.");

                // the entity cannot belong to another block
                if (e.Item.Owner.Record.Layout == null)
                    throw new ArgumentException("The entity cannot belong to another block. Clone it instead.");

                // we will exchange the owner of the entity
                this.Owner.RemoveEntity(e.Item);
            }
            this.Owner.AddEntityToDocument(e.Item, (Block) sender, string.IsNullOrEmpty(e.Item.Handle));
        }

        private void Block_EntityRemoved(TableObject sender, BlockEntityChangeEventArgs e)
        {
            this.Owner.RemoveEntityFromDocument(e.Item);
        }

        private void Block_AttributeDefinitionAdded(Block sender, BlockAttributeDefinitionChangeEventArgs e)
        {
            if (e.Item.Owner != null)
            {
                // the block and its entities must belong to the same document
                if (!ReferenceEquals(e.Item.Owner.Record.Owner.Owner, this.Owner))
                    throw new ArgumentException("The block and the entity must belong to the same document. Clone it instead.");

                // the entity cannot belong to another block
                if (e.Item.Owner.Record.Layout == null)
                    throw new ArgumentException("The entity cannot belong to another block. Clone it instead.");

                // we will exchange the owner of the entity
                this.Owner.RemoveAttributeDefinitionFromDocument(e.Item);
            }
            this.Owner.AddAttributeDefinitionToDocument(e.Item, string.IsNullOrEmpty(e.Item.Handle));
        }

        private void Block_AttributeDefinitionRemoved(Block sender, BlockAttributeDefinitionChangeEventArgs e)
        {
            this.Owner.RemoveAttributeDefinitionFromDocument(e.Item);
        }

        #endregion
    }
}