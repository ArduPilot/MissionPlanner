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
using System.Collections;
using System.Collections.Generic;
using netDxf.Entities;

namespace netDxf.Collections
{
    /// <summary>
    /// Represent a collection of <see cref="EntityObject">entities</see> that fire events when it is modified. 
    /// </summary>
    public class EntityCollection :
        IList<EntityObject>
    {
        #region delegates and events

        public delegate void BeforeAddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public event BeforeAddItemEventHandler BeforeAddItem;

        protected virtual bool OnBeforeAddItemEvent(EntityObject item)
        {
            BeforeAddItemEventHandler ae = this.BeforeAddItem;
            if (ae != null)
            {
                EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void AddItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public event AddItemEventHandler AddItem;

        protected virtual void OnAddItemEvent(EntityObject item)
        {
            AddItemEventHandler ae = this.AddItem;
            if (ae != null)
                ae(this, new EntityCollectionEventArgs(item));
        }

        public delegate void RemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        protected virtual bool OnBeforeRemoveItemEvent(EntityObject item)
        {
            BeforeRemoveItemEventHandler ae = this.BeforeRemoveItem;
            if (ae != null)
            {
                EntityCollectionEventArgs e = new EntityCollectionEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void BeforeRemoveItemEventHandler(EntityCollection sender, EntityCollectionEventArgs e);

        public event RemoveItemEventHandler RemoveItem;

        protected virtual void OnRemoveItemEvent(EntityObject item)
        {
            RemoveItemEventHandler ae = this.RemoveItem;
            if (ae != null)
                ae(this, new EntityCollectionEventArgs(item));
        }

        #endregion

        #region private fields

        private readonly List<EntityObject> innerArray;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>EntityCollection</c>.
        /// </summary>
        public EntityCollection()
        {
            this.innerArray = new List<EntityObject>();
        }

        /// <summary>
        /// Initializes a new instance of <c>EntityCollection</c> and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of items the collection can initially store.</param>
        public EntityCollection(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "The collection capacity cannot be negative.");
            this.innerArray = new List<EntityObject>(capacity);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the <see cref="EntityObject">entity</see> at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The <see cref="EntityObject">entity</see> at the specified index.</returns>
        public EntityObject this[int index]
        {
            get { return this.innerArray[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                EntityObject remove = this.innerArray[index];

                if (this.OnBeforeRemoveItemEvent(remove))
                    return;
                if (this.OnBeforeAddItemEvent(value))
                    return;
                this.innerArray[index] = value;
                this.OnAddItemEvent(value);
                this.OnRemoveItemEvent(remove);
            }
        }

        /// <summary>
        /// Gets the number of <see cref="EntityObject">entities</see> contained in the collection.
        /// </summary>
        public int Count
        {
            get { return this.innerArray.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds an <see cref="EntityObject">entity</see> to the collection.
        /// </summary>
        /// <param name="item"> The <see cref="EntityObject">entity</see> to add to the collection.</param>
        /// <returns>True if the <see cref="EntityObject">entity</see> has been added to the collection, or false otherwise.</returns>
        public void Add(EntityObject item)
        {
            if (this.OnBeforeAddItemEvent(item))
                throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
            this.innerArray.Add(item);
            this.OnAddItemEvent(item);
        }

        /// <summary>
        /// Adds an <see cref="EntityObject">entity</see> list to the end of the collection.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        public void AddRange(IEnumerable<EntityObject> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            foreach (EntityObject item in collection)
                this.Add(item);
        }

        /// <summary>
        /// Inserts an <see cref="EntityObject">entity</see> into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The <see cref="EntityObject">entity</see> to insert. The value can not be null.</param>
        public void Insert(int index, EntityObject item)
        {
            if (index < 0 || index >= this.innerArray.Count)
                throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, this.innerArray.Count));
            if (this.OnBeforeRemoveItemEvent(this.innerArray[index]))
                return;
            if (this.OnBeforeAddItemEvent(item))
                throw new ArgumentException("The entity cannot be added to the collection.", nameof(item));
            this.OnRemoveItemEvent(this.innerArray[index]);
            this.innerArray.Insert(index, item);
            this.OnAddItemEvent(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="EntityObject">entity</see> from the collection
        /// </summary>
        /// <param name="item">The <see cref="EntityObject">entity</see> to remove from the collection.</param>
        /// <returns>True if <see cref="EntityObject">entity</see> is successfully removed; otherwise, false.</returns>
        public bool Remove(EntityObject item)
        {
            if (!this.innerArray.Contains(item))
                return false;
            if (this.OnBeforeRemoveItemEvent(item))
                return false;
            this.innerArray.Remove(item);
            this.OnRemoveItemEvent(item);
            return true;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="items">The list of objects to remove from the collection.</param>
        /// <returns>True if object is successfully removed; otherwise, false.</returns>
        public void Remove(IEnumerable<EntityObject> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (EntityObject item in items)
                this.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="EntityObject">entity</see> at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="EntityObject">entity</see> to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.innerArray.Count)
                throw new ArgumentOutOfRangeException(string.Format("The parameter index {0} must be in between {1} and {2}.", index, 0, this.innerArray.Count));
            EntityObject remove = this.innerArray[index];
            if (this.OnBeforeRemoveItemEvent(remove))
                return;
            this.innerArray.RemoveAt(index);
            this.OnRemoveItemEvent(remove);
        }

        /// <summary>
        /// Removes all <see cref="EntityObject">entities</see> from the collection.
        /// </summary>
        public void Clear()
        {
            EntityObject[] entities = new EntityObject[this.innerArray.Count];
            this.innerArray.CopyTo(entities, 0);
            foreach (EntityObject item in entities)
                this.Remove(item);
        }

        /// <summary>
        /// Searches for the specified <see cref="EntityObject">entity</see> and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The <see cref="EntityObject">entity</see> to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, –1.</returns>
        public int IndexOf(EntityObject item)
        {
            return this.innerArray.IndexOf(item);
        }

        /// <summary>
        /// Determines whether an <see cref="EntityObject">entity</see> is in the collection.
        /// </summary>
        /// <param name="item">The <see cref="EntityObject">entity</see> to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, false.</returns>
        public bool Contains(EntityObject item)
        {
            return this.innerArray.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array"> The one-dimensional System.Array that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(EntityObject[] array, int arrayIndex)
        {
            this.innerArray.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<EntityObject> GetEnumerator()
        {
            return this.innerArray.GetEnumerator();
        }

        #endregion

        #region private methods

        void ICollection<EntityObject>.Add(EntityObject item)
        {
            this.Add(item);
        }

        void IList<EntityObject>.Insert(int index, EntityObject item)
        {
            this.Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}