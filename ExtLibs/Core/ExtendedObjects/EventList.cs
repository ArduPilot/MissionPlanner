using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ExtendedObjects
{
    public class EventList<T> : List<T>
    {
        public event GenericEventHandler<T> ObjectRemoved;
        public event GenericEventHandler<T> ObjectAdded;
        public event GenericEventHandler<List<T>> Clearing;

        public EventList() : base() { }
        public EventList(IEnumerable<T> collection) : base(collection) { }
        public EventList(int capacity) : base(capacity) { }

        public new void Add(T item)
        {
            base.Add(item);
            Call_ObjectAdded(item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            Call_ObjectAdded(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Add(item);
            }
        }
        
        public new void Remove(T item)
        {
            base.Remove(item);
            Call_ObjectRemoved(item);
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (T item in collection) {
                Remove(item);
            }
        }

        public new void Clear()
        {
            Call_Clearing(this);
            base.Clear();
        }

        protected void Call_ObjectAdded(T item)
        {
            if (ObjectAdded != null) {
                ObjectAdded(null, new GenericEventArgs<T>(item));
            }
        }

        protected void Call_ObjectRemoved(T item)
        {
            if (ObjectRemoved != null) {
                ObjectRemoved(null, new GenericEventArgs<T>(item));
            }
        }        

        protected void Call_Clearing(List<T> items)
        {
            if (Clearing != null) {
                Clearing(null, new GenericEventArgs<List<T>>(items));
            }
        }
    }    
}
