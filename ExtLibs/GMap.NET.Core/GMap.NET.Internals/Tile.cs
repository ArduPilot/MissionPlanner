
namespace GMap.NET.Internals
{
   using System.Collections.Generic;

   /// <summary>
   /// represent tile
   /// </summary>
   internal class Tile
   {
      GPoint pos;
      int zoom;
      public readonly List<PureImage> Overlays = new List<PureImage>(1);

      public Tile(int zoom, GPoint pos)
      {
         this.Zoom = zoom;
         this.Pos = pos;
      }

      public void Clear()
      {
         lock(Overlays)
         {
            foreach(PureImage i in Overlays)
            {
               i.Dispose();
            }

            Overlays.Clear();
         }
      }

      public int Zoom
      {
         get
         {
            return zoom;
         }
         private set
         {
            zoom = value;
         }
      }

      public GPoint Pos
      {
         get
         {
            return pos;
         }
         private set
         {
            pos= value;
         }
      }
   }
}
