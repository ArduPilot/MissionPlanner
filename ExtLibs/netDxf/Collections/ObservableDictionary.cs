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

using System.Collections;
using System.Collections.Generic;

namespace netDxf.Collections
{
    public sealed class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>
    {
        #region delegates and events

        public delegate void AddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeAddItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void RemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public delegate void BeforeRemoveItemEventHandler(ObservableDictionary<TKey, TValue> sender, ObservableDictionaryEventArgs<TKey, TValue> e);

        public event BeforeAddItemEventHandler BeforeAddItem;
        public event AddItemEventHandler AddItem;
        public event BeforeRemoveItemEventHandler BeforeRemoveItem;
        public event RemoveItemEventHandler RemoveItem;

        private bool BeforeAddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeAddItemEventHandler ae = this.BeforeAddItem;
            if (ae != null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void AddItemEvent(KeyValuePair<TKey, TValue> item)
        {
            AddItemEventHandler ae = this.AddItem;
            if (ae != null)
                ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
        }

        private bool BeforeRemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            BeforeRemoveItemEventHandler ae = this.BeforeRemoveItem;
            if (ae != null)
            {
                ObservableDictionaryEventArgs<TKey, TValue> e = new ObservableDictionaryEventArgs<TKey, TValue>(item);
                ae(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void RemoveItemEvent(KeyValuePair<TKey, TValue> item)
        {
            RemoveItemEventHandler ae = this.RemoveItem;
            if (ae != null)
                ae(this, new ObservableDictionaryEventArgs<TKey, TValue>(item));
        }

        #endregion

        #region private fields

        private readonly Dictionary<TKey, TValue> innerDictionary;

        #endregion

        #region constructor

        public ObservableDictionary()
        {
            this.innerDictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(int capacity)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.innerDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        #endregion

        #region public properties

        public TValue this[TKey key]
        {
            get { return this.innerDictionary[key]; }
            set
            {
                KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
                KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);

                if (this.BeforeRemoveItemEvent(remove))
                    return;
                if (this.BeforeAddItemEvent(add))
                    return;
                this.innerDictionary[key] = value;
                this.AddItemEvent(add);
                this.RemoveItemEvent(remove);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return this.innerDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return this.innerDictionary.Values; }
        }

        public int Count
        {
            get { return this.innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region public methods

        public void Add(TKey key, TValue value)
        {
            KeyValuePair<TKey, TValue> add = new KeyValuePair<TKey, TValue>(key, value);
            if (this.BeforeAddItemEvent(add))
                return;
            this.innerDictionary.Add(key, value);
            this.AddItemEvent(add);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            if (!this.innerDictionary.ContainsKey(key))
                return false;

            KeyValuePair<TKey, TValue> remove = new KeyValuePair<TKey, TValue>(key, this.innerDictionary[key]);
            if (this.BeforeRemoveItemEvent(remove))
                return false;
            this.innerDictionary.Remove(key);
            this.RemoveItemEvent(remove);

            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!ReferenceEquals(item.Value, this.innerDictionary[item.Key]))
                return false;
            return this.Remove(item.Key);
        }

        public void Clear()
        {
            TKey[] keys = new TKey[this.innerDictionary.Count];
            this.innerDictionary.Keys.CopyTo(keys, 0);
            foreach (TKey key in keys)
            {
                this.Remove(key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) this.innerDictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return this.innerDictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return this.innerDictionary.ContainsValue(value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.innerDictionary.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>) this.innerDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}