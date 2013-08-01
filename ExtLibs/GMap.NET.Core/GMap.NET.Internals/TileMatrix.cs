
namespace GMap.NET.Internals
{
   using System.Collections.Generic;
   using System.Threading;
   using System.Diagnostics;
   using System;

   /// <summary>
   /// matrix for tiles
   /// </summary>
   internal class TileMatrix
   {
      readonly List<Dictionary<GPoint, Tile>> Levels = new List<Dictionary<GPoint, Tile>>(33);
      readonly FastReaderWriterLock Lock = new FastReaderWriterLock();

      public TileMatrix()
      {
         for(int i = 0; i < Levels.Capacity; i++)
         {
            Levels.Add(new Dictionary<GPoint, Tile>(55));
         }
      }

      public void ClearAllLevels()
      {
         Lock.AcquireWriterLock();
         try
         {
            foreach(var matrix in Levels)
            {
               foreach(var t in matrix)
               {
                  t.Value.Clear();
               }
               matrix.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevel(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom < Levels.Count)
            {
               var l = Levels[zoom];

               foreach(var t in l)
               {
                  t.Value.Clear();
               }

               l.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      readonly List<KeyValuePair<GPoint, Tile>> tmp = new List<KeyValuePair<GPoint, Tile>>(44);

      public void ClearLevelAndPointsNotIn(int zoom, List<GPoint> list)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom < Levels.Count)
            {
               var l = Levels[zoom];

               tmp.Clear();

               foreach(var t in l)
               {
                  if(!list.Contains(t.Key))
                  {
                     tmp.Add(t);
                  }
               }

               foreach(var r in tmp)
               {
                  l.Remove(r.Key);
                  r.Value.Clear();
               }

               tmp.Clear();
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevelsBelove(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom-1 < Levels.Count)
            {
               for(int i = zoom-1; i >= 0; i--)
               {
                  var l = Levels[i];

                  foreach(var t in l)
                  {
                     t.Value.Clear();
                  }

                  l.Clear();
               }
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void ClearLevelsAbove(int zoom)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(zoom+1 < Levels.Count)
            {
               for(int i = zoom+1; i < Levels.Count; i++)
               {
                  var l = Levels[i];

                  foreach(var t in l)
                  {
                     t.Value.Clear();
                  }

                  l.Clear();
               }
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }

      public void EnterReadLock()
      {
         Lock.AcquireReaderLock();
      }

      public void LeaveReadLock()
      {
         Lock.ReleaseReaderLock();
      }

      public Tile GetTileWithNoLock(int zoom, GPoint p)
      {
         Tile ret = null;

         //if(zoom < Levels.Count)
         {
            Levels[zoom].TryGetValue(p, out ret);
         }

         return ret;
      }

      public Tile GetTileWithReadLock(int zoom, GPoint p)
      {
         Tile ret = null;

         Lock.AcquireReaderLock();
         try
         {
            ret = GetTileWithNoLock(zoom, p);
         }
         finally
         {
            Lock.ReleaseReaderLock();
         }

         return ret;
      }

      public void SetTile(Tile t)
      {
         Lock.AcquireWriterLock();
         try
         {
            if(t.Zoom < Levels.Count)
            {
               Levels[t.Zoom][t.Pos] = t;
            }
         }
         finally
         {
            Lock.ReleaseWriterLock();
         }
      }
   }
}
