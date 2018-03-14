using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GMap.NET.ObjectModel
{
   public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

   public interface INotifyCollectionChanged
   {
      // Events
      event NotifyCollectionChangedEventHandler CollectionChanged;
   }

   public interface INotifyPropertyChanged
   {
      // Events
      event PropertyChangedEventHandler PropertyChanged;
   }

   public enum NotifyCollectionChangedAction
   {
      Add,
      Remove,
      Replace,
      Move,
      Reset
   }

   public class NotifyCollectionChangedEventArgs : EventArgs
   {
      // Fields
      private NotifyCollectionChangedAction _action;
      private IList _newItems;
      private int _newStartingIndex;
      private IList _oldItems;
      private int _oldStartingIndex;

      // Methods
      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Reset)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         this.InitializeAdd(action, null, -1);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
         {
            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
         }
         if(action == NotifyCollectionChangedAction.Reset)
         {
            if(changedItems != null)
            {
               throw new ArgumentException("ResetActionRequiresNullItem", "action");
            }
            this.InitializeAdd(action, null, -1);
         }
         else
         {
            if(changedItems == null)
            {
               throw new ArgumentNullException("changedItems");
            }
            this.InitializeAddOrRemove(action, changedItems, -1);
         }
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
         {
            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
         }
         if(action == NotifyCollectionChangedAction.Reset)
         {
            if(changedItem != null)
            {
               throw new ArgumentException("ResetActionRequiresNullItem", "action");
            }
            this.InitializeAdd(action, null, -1);
         }
         else
         {
            this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
         }
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Replace)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         if(newItems == null)
         {
            throw new ArgumentNullException("newItems");
         }
         if(oldItems == null)
         {
            throw new ArgumentNullException("oldItems");
         }
         this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
         {
            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
         }
         if(action == NotifyCollectionChangedAction.Reset)
         {
            if(changedItems != null)
            {
               throw new ArgumentException("ResetActionRequiresNullItem", "action");
            }
            if(startingIndex != -1)
            {
               throw new ArgumentException("ResetActionRequiresIndexMinus1", "action");
            }
            this.InitializeAdd(action, null, -1);
         }
         else
         {
            if(changedItems == null)
            {
               throw new ArgumentNullException("changedItems");
            }
            if(startingIndex < -1)
            {
               throw new ArgumentException("IndexCannotBeNegative", "startingIndex");
            }
            this.InitializeAddOrRemove(action, changedItems, startingIndex);
         }
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
         {
            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
         }
         if(action == NotifyCollectionChangedAction.Reset)
         {
            if(changedItem != null)
            {
               throw new ArgumentException("ResetActionRequiresNullItem", "action");
            }
            if(index != -1)
            {
               throw new ArgumentException("ResetActionRequiresIndexMinus1", "action");
            }
            this.InitializeAdd(action, null, -1);
         }
         else
         {
            this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
         }
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Replace)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Replace)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         if(newItems == null)
         {
            throw new ArgumentNullException("newItems");
         }
         if(oldItems == null)
         {
            throw new ArgumentNullException("oldItems");
         }
         this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Move)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         if(index < 0)
         {
            throw new ArgumentException("IndexCannotBeNegative", "index");
         }
         this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Move)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         if(index < 0)
         {
            throw new ArgumentException("IndexCannotBeNegative", "index");
         }
         object[] newItems = new object[] { changedItem };
         this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
      }

      public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
      {
         this._newStartingIndex = -1;
         this._oldStartingIndex = -1;
         if(action != NotifyCollectionChangedAction.Replace)
         {
            throw new ArgumentException("WrongActionForCtor", "action");
         }
         this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
      }

      private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
      {
         this._action = action;
#if !PocketPC
				 this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
#else
         this._newItems = (newItems == null) ? null : newItems;
#endif
         this._newStartingIndex = newStartingIndex;
      }

      private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
      {
         if(action == NotifyCollectionChangedAction.Add)
         {
            this.InitializeAdd(action, changedItems, startingIndex);
         }
         else if(action == NotifyCollectionChangedAction.Remove)
         {
            this.InitializeRemove(action, changedItems, startingIndex);
         }
         else
         {
            throw new ArgumentException(string.Format("InvariantFailure, Unsupported action: {0}", action.ToString()));
         }
      }

      private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
      {
         this.InitializeAdd(action, newItems, startingIndex);
         this.InitializeRemove(action, oldItems, oldStartingIndex);
      }

      private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
      {
         this._action = action;
#if !PocketPC
				 this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
#else
         this._oldItems = (oldItems == null) ? null : oldItems;
#endif
         this._oldStartingIndex = oldStartingIndex;
      }

      // Properties
      public NotifyCollectionChangedAction Action
      {
         get
         {
            return this._action;
         }
      }

      public IList NewItems
      {
         get
         {
            return this._newItems;
         }
      }

      public int NewStartingIndex
      {
         get
         {
            return this._newStartingIndex;
         }
      }

      public IList OldItems
      {
         get
         {
            return this._oldItems;
         }
      }

      public int OldStartingIndex
      {
         get
         {
            return this._oldStartingIndex;
         }
      }
   }

   [Serializable]
   public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
   {
      // Fields
      private SimpleMonitor _monitor;
      private const string CountString = "Count";
      private const string IndexerName = "Item[]";

      // Events
      [field: NonSerialized]
      public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

      [field: NonSerialized]
      protected event PropertyChangedEventHandler PropertyChanged;

      event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
      {
         add
         {
            PropertyChanged += value;
         }
         remove
         {
            PropertyChanged -= value;
         }
      }

      // Methods
      public ObservableCollection()
      {
         this._monitor = new SimpleMonitor();
      }

      public ObservableCollection(IEnumerable<T> collection)
      {
         this._monitor = new SimpleMonitor();
         if(collection == null)
         {
            throw new ArgumentNullException("collection");
         }
         this.CopyFrom(collection);
      }

      public ObservableCollection(List<T> list)
         : base((list != null) ? new List<T>(list.Count) : list)
      {
         this._monitor = new SimpleMonitor();
         this.CopyFrom(list);
      }

      protected IDisposable BlockReentrancy()
      {
         this._monitor.Enter();
         return this._monitor;
      }

      protected void CheckReentrancy()
      {
         if((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
         {
            throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
         }
      }

      protected override void ClearItems()
      {
         this.CheckReentrancy();
         base.ClearItems();
         this.OnPropertyChanged(CountString);
         this.OnPropertyChanged(IndexerName);
         this.OnCollectionReset();
      }

      private void CopyFrom(IEnumerable<T> collection)
      {
         IList<T> items = base.Items;
         if((collection != null) && (items != null))
         {
            using(IEnumerator<T> enumerator = collection.GetEnumerator())
            {
               while(enumerator.MoveNext())
               {
                  items.Add(enumerator.Current);
               }
            }
         }
      }

      protected override void InsertItem(int index, T item)
      {
         this.CheckReentrancy();
         base.InsertItem(index, item);
         this.OnPropertyChanged(CountString);
         this.OnPropertyChanged(IndexerName);
         this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
      }

      public void Move(int oldIndex, int newIndex)
      {
         this.MoveItem(oldIndex, newIndex);
      }

      protected virtual void MoveItem(int oldIndex, int newIndex)
      {
         this.CheckReentrancy();
         T item = base[oldIndex];
         base.RemoveItem(oldIndex);
         base.InsertItem(newIndex, item);
         this.OnPropertyChanged(IndexerName);
         this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
      }

      protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         if(this.CollectionChanged != null)
         {
            using(this.BlockReentrancy())
            {
               this.CollectionChanged(this, e);
            }
         }
      }

      private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
      {
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
      }

      private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
      {
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
      }

      private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
      {
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
      }

      private void OnCollectionReset()
      {
         this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
      {
         if(this.PropertyChanged != null)
         {
            this.PropertyChanged(this, e);
         }
      }

      private void OnPropertyChanged(string propertyName)
      {
         this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
      }

      protected override void RemoveItem(int index)
      {
         this.CheckReentrancy();
         T item = base[index];
         base.RemoveItem(index);
         this.OnPropertyChanged(CountString);
         this.OnPropertyChanged(IndexerName);
         this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
      }

      protected override void SetItem(int index, T item)
      {
         this.CheckReentrancy();
         T oldItem = base[index];
         base.SetItem(index, item);
         this.OnPropertyChanged(IndexerName);
         this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
      }

      // Nested Types
      [Serializable]
      private class SimpleMonitor : IDisposable
      {
         // Fields
         private int _busyCount;

         // Methods
         public void Dispose()
         {
            this._busyCount--;
         }

         public void Enter()
         {
            this._busyCount++;
         }

         // Properties
         public bool Busy
         {
            get
            {
               return (this._busyCount > 0);
            }
         }
      }
   }
}
