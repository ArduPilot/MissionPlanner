
namespace GMap.NET.WindowsForms.Markers
{
   using System.Drawing;

#if !PocketPC
    using GMap.NET.Properties;
#else
   using GMap.NET.WindowsMobile.Properties;
#endif

   public class GMapMarkerGoogleGreen : GMapMarker
   {
      public float? Bearing;

      static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(Resources.bigMarkerGreen.Width, Resources.bigMarkerGreen.Height);

       static Bitmap localcache1 = Resources.shadow50;
       static Bitmap localcache2 = Resources.bigMarkerGreen;

      public GMapMarkerGoogleGreen(PointLatLng p)
         : base(p)
      {
         Size = SizeSt;
         Offset = new Point(-10, -34);
      }

      static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -22), new Point(7, 7), new Point(0, 2) };

      public override void OnRender(Graphics g)
      {
#if !PocketPC
         if(!Bearing.HasValue)
         {
             g.DrawImageUnscaled(localcache1, LocalPosition.X, LocalPosition.Y);
         }
         g.TranslateTransform(ToolTipPosition.X, ToolTipPosition.Y);

         if(Bearing.HasValue)
         {
            g.RotateTransform(Bearing.Value - Overlay.Control.Bearing);
            g.FillPolygon(Brushes.Lime, Arrow);
         }

         g.ResetTransform();

         if(!Bearing.HasValue)
         {
             g.DrawImageUnscaled(localcache2, LocalPosition.X, LocalPosition.Y);
         }
#else
            DrawImageUnscaled(g, Resources.shadow50, LocalPosition.X, LocalPosition.Y);
            DrawImageUnscaled(g, Resources.marker, LocalPosition.X, LocalPosition.Y);
#endif
      }
   }
}
