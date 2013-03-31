
namespace GMap.NET.Internals
{
   /// <summary>
   /// tile load task
   /// </summary>
   internal struct LoadTask
   {
      public GPoint Pos;
      public int Zoom;

      public LoadTask(GPoint pos, int zoom)
      {
         Pos = pos;
         Zoom = zoom;
      }

      public override string ToString()
      {
         return Zoom + " - " + Pos.ToString();
      }
   }
}
