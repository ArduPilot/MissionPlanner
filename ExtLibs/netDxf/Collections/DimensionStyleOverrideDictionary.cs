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
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a dictionary of <see cref="DimensionStyleOverride">DimensionStyleOverrides</see>.
    /// </summary>
    public sealed class DimensionStyleOverrideDictionary :
        IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>
    {
        #region delegates and events

        public delegate void BeforeAddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event BeforeAddItemEventHandler BeforeAddItem;

        private bool OnBeforeAddItemEvent(DimensionStyleOverride item)
        {
            BeforeAddItemEventHandler ae = this.BeforeAddItem;
            if (ae != null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void AddItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event AddItemEventHandler AddItem;

        private void OnAddItemEvent(DimensionStyleOverride item)
        {
            AddItemEventHandler ae = this.AddItem;
            if (ae != null)
                ae(this, new DimensionStyleOverrideDictionaryEventArgs(item));
        }

        public delegate void BeforeRemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event BeforeRemoveItemEventHandler BeforeRemoveItem;

        private bool OnBeforeRemoveItemEvent(DimensionStyleOverride item)
        {
            BeforeRemoveItemEventHandler ae = this.BeforeRemoveItem;
            if (ae != null)
            {
                DimensionStyleOverrideDictionaryEventArgs e = new DimensionStyleOverrideDictionaryEventArgs(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        public delegate void RemoveItemEventHandler(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e);

        public event RemoveItemEventHandler RemoveItem;

        private void OnRemoveItemEvent(DimensionStyleOverride item)
        {
            RemoveItemEventHandler ae = this.RemoveItem;
            if (ae != null)
                ae(this, new DimensionStyleOverrideDictionaryEventArgs(item));
        }

        #endregion

        #region private fields

        private readonly Dictionary<DimensionStyleOverrideType, DimensionStyleOverride> innerDictionary;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>DimensionStyleOverrideDictionary</c>.
        /// </summary>
        public DimensionStyleOverrideDictionary()
        {
            this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>();
        }

        /// <summary>
        /// Initializes a new instance of <c>DimensionStyleOverrideDictionary</c> and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of items the collection can initially store.</param>
        public DimensionStyleOverrideDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<DimensionStyleOverrideType, DimensionStyleOverride>(capacity);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> with the specified type.
        /// </summary>
        /// <param name="type">The type of the DimensionStyleOverride to get or set.</param>
        /// <returns>The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> with the specified type.</returns>
        public DimensionStyleOverride this[DimensionStyleOverrideType type]
        {
            get { return this.innerDictionary[type]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (type != value.Type)
                    throw new ArgumentException(string.Format("The dictionary type: {0}, and the DimensionStyleOverride type: {1}, must be the same", type, value.Type));

                DimensionStyleOverride remove = this.innerDictionary[type];
                if (this.OnBeforeRemoveItemEvent(remove))
                    return;
                if (this.OnBeforeAddItemEvent(value))
                    return;
                this.innerDictionary[type] = value;
                this.OnAddItemEvent(value);
                this.OnRemoveItemEvent(remove);
            }
        }

        /// <summary>
        /// Gets an ICollection containing the types of the current dictionary.
        /// </summary>
        public ICollection<DimensionStyleOverrideType> Types
        {
            get { return this.innerDictionary.Keys; }
        }

        /// <summary>
        /// Gets an ICollection containing the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> list of the current dictionary.
        /// </summary>
        public ICollection<DimensionStyleOverride> Values
        {
            get { return this.innerDictionary.Values; }
        }

        /// <summary>
        /// Gets the number of <see cref="DimensionStyleOverride">DimensionStyleOverride</see> contained in the current dictionary.
        /// </summary>
        public int Count
        {
            get { return this.innerDictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the actual dictionary is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds a <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to the dictionary from its type and value.
        /// </summary>
        /// <param name="type">Dimension style override type.</param>
        /// <param name="value">Dimension style override value.</param>
        /// <remarks>A new DimensionStyleOverride will be created from the specified arguments.</remarks>
        public void Add(DimensionStyleOverrideType type, object value)
        {
            this.Add(new DimensionStyleOverride(type, value));
        }

        /// <summary>
        /// Adds an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to the dictionary.
        /// </summary>
        /// <param name="item">The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to add.</param>
        public void Add(DimensionStyleOverride item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (this.OnBeforeAddItemEvent(item))
                throw new ArgumentException(string.Format("The DimensionStyleOverride {0} cannot be added to the collection.", item), nameof(item));
            this.innerDictionary.Add(item.Type, item);
            this.OnAddItemEvent(item);
        }

        /// <summary>
        /// Adds an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> list to the dictionary.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added.</param>
        public void AddRange(IEnumerable<DimensionStyleOverride> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            // we will make room for so the collection will fit without having to resize the internal array during the Add method
            foreach (DimensionStyleOverride item in collection)
                this.Add(item);
        }

        /// <summary>
        /// Removes an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type from the current dictionary.
        /// </summary>
        /// <param name="type">The type of the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to remove.</param>
        /// <returns>True if the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> is successfully removed; otherwise, false.</returns>
        public bool Remove(DimensionStyleOverrideType type)
        {
            DimensionStyleOverride remove;
            if (!this.innerDictionary.TryGetValue(type, out remove))
                return false;
            if (this.OnBeforeRemoveItemEvent(remove))
                return false;
            this.innerDictionary.Remove(type);
            this.OnRemoveItemEvent(remove);
            return true;
        }

        /// <summary>
        /// Removes all <see cref="DimensionStyleOverride">DimensionStyleOverride</see> from the current dictionary.
        /// </summary>
        public void Clear()
        {
            DimensionStyleOverrideType[] types = new DimensionStyleOverrideType[this.innerDictionary.Count];
            this.innerDictionary.Keys.CopyTo(types, 0);
            foreach (DimensionStyleOverrideType tag in types)
            {
                this.Remove(tag);
            }
        }

        /// <summary>
        /// Determines whether current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type.
        /// </summary>
        /// <param name="type">The type to locate in the current dictionary.</param>
        /// <returns>True if the current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the type; otherwise, false.</returns>
        public bool ContainsType(DimensionStyleOverrideType type)
        {
            return this.innerDictionary.ContainsKey(type);
        }

        /// <summary>
        /// Determines whether current dictionary contains a specified <see cref="DimensionStyleOverride">DimensionStyleOverride</see>.
        /// </summary>
        /// <param name="value">The <see cref="DimensionStyleOverride">DimensionStyleOverride</see> to locate in the current dictionary.</param>
        /// <returns>True if the current dictionary contains the <see cref="DimensionStyleOverride">DimensionStyleOverride</see>; otherwise, false.</returns>
        public bool ContainsValue(DimensionStyleOverride value)
        {
            return this.innerDictionary.ContainsValue(value);
        }

        /// <summary>
        /// Gets the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> associated of the specified type.
        /// </summary>
        /// <param name="type">The type whose value to get.</param>
        /// <param name="value">When this method returns, the <see cref="DimensionStyleOverride">DimensionStyleOverride</see> associated of the specified type,
        /// if the tag is found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>True if the current dictionary contains an <see cref="DimensionStyleOverride">DimensionStyleOverride</see> of the specified type; otherwise, false.</returns>
        public bool TryGetValue(DimensionStyleOverrideType type, out DimensionStyleOverride value)
        {
            return this.innerDictionary.TryGetValue(type, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
        public IEnumerator<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>> GetEnumerator()
        {
            return this.innerDictionary.GetEnumerator();
        }

        #endregion

        #region private properties

        ICollection<DimensionStyleOverrideType> IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Keys
        {
            get { return this.innerDictionary.Keys; }
        }

        #endregion

        #region private methods

        bool IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.ContainsKey(DimensionStyleOverrideType tag)
        {
            return this.innerDictionary.ContainsKey(tag);
        }

        void IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>.Add(DimensionStyleOverrideType key, DimensionStyleOverride value)
        {
            this.Add(value);
        }

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Add(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            this.Add(item.Value);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Remove(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            if (!ReferenceEquals(item.Value, this.innerDictionary[item.Key]))
                return false;
            return this.Remove(item.Key);
        }

        bool ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.Contains(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride> item)
        {
            return ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>) this.innerDictionary).Contains(item);
        }

        void ICollection<KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>>.CopyTo(KeyValuePair<DimensionStyleOverrideType, DimensionStyleOverride>[] array, int arrayIndex)
        {
            ((IDictionary<DimensionStyleOverrideType, DimensionStyleOverride>) this.innerDictionary).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}