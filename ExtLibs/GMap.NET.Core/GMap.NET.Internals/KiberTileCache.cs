
namespace GMap.NET.Internals
{
   using System.Collections.Generic;
   using System.IO;
   using System;

   /// <summary>
   /// kiber speed memory cache for tiles with history support ;}
   /// </summary>
   internal class KiberTileCache : Dictionary<RawTile, MemoryStream>
   {
      readonly Queue<RawTile> Queue = new Queue<RawTile>();

      /// <summary>
      /// the amount of tiles in MB to keep in memmory, default: 22MB, if each ~100Kb it's ~222 tiles
      /// </summary>
#if !PocketPC
      public int MemoryCacheCapacity = 22;
#else
      public int MemoryCacheCapacity = 3;
#endif

      long memoryCacheSize = 0;

      /// <summary>
      /// current memmory cache size in MB
      /// </summary>
      public double MemoryCacheSize
      {
         get
         {
            return memoryCacheSize/1048576.0;
         }
      }

      public new void Add(RawTile key, MemoryStream value)
      {
         Queue.Enqueue(key);
         base.Add(key, value);

         memoryCacheSize += value.Length;
      }

      // do not allow directly removal of elements
      private new void Remove(RawTile key)
      {

      }

      internal void RemoveMemoryOverload()
      {
         while(MemoryCacheSize > MemoryCacheCapacity)
         {
            if(Keys.Count > 0 && Queue.Count > 0)
            {
               RawTile first = Queue.Dequeue();
               try
               {
                  using(MemoryStream m = base[first])
                  {
                     base.Remove(first);
                     memoryCacheSize -= m.Length;
                  }
               }
               catch
               {
               }
            }
            else
            {
               break;
            }
         }
      }
   }
}
