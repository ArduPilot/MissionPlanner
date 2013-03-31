using System;

namespace GMap.NET.ObjectModel
{
   public class ObservableCollectionThreadSafe<T> : ObservableCollection<T>
   {
      NotifyCollectionChangedEventHandler collectionChanged;
      public override event NotifyCollectionChangedEventHandler CollectionChanged
      {
         add
         {
            collectionChanged += value;
         }
         remove
         {
            collectionChanged -= value;
         }
      }

      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         // Be nice - use BlockReentrancy like MSDN said
         using(BlockReentrancy())
         {
            if(collectionChanged != null)
            {
               Delegate[] delegates = collectionChanged.GetInvocationList();

               // Walk thru invocation list
               foreach(NotifyCollectionChangedEventHandler handler in delegates)
               {
#if !PocketPC
                  System.Windows.Forms.Control dispatcherObject = handler.Target as System.Windows.Forms.Control;

                  // If the subscriber is a DispatcherObject and different thread
                  if(dispatcherObject != null && dispatcherObject.InvokeRequired)
                  {
                     // Invoke handler in the target dispatcher's thread
                     dispatcherObject.Invoke(handler, this, e);
                  }
                  else // Execute handler as is 
                  {
                     collectionChanged(this, e);
                  }
#else
                  // If the subscriber is a DispatcherObject and different thread
                  if(handler != null)
                  {
                     // Invoke handler in the target dispatcher's thread
                     handler.Invoke(handler, e);
                  }
                  else // Execute handler as is 
                  {
                     collectionChanged(this, e);
                  }
#endif
               }
            }
         }
      }
   }
}
