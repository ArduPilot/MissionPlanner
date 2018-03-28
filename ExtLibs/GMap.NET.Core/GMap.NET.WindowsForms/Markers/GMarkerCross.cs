
namespace GMap.NET.WindowsForms.Markers
{
   using System.Drawing;
   using System.Runtime.Serialization;
   using System;

#if !PocketPC
   [Serializable]
   public class GMarkerCross : GMapMarker, ISerializable
#else
   public class GMarkerCross : GMapMarker
#endif
   {
#if !PocketPC
         public static readonly Pen DefaultPen = new Pen(Brushes.Red, 1);
#else
         public static readonly Pen DefaultPen = new Pen(Color.Red, 1);
#endif

      [NonSerialized]
      public Pen Pen = DefaultPen;

      public GMarkerCross(PointLatLng p)
         : base(p)
      {
         IsHitTestVisible = false;
      }

      public override void OnRender(Graphics g)
      {
         System.Drawing.Point p1 = new System.Drawing.Point(LocalPosition.X, LocalPosition.Y);
         p1.Offset(0, -10);
         System.Drawing.Point p2 = new System.Drawing.Point(LocalPosition.X, LocalPosition.Y);
         p2.Offset(0, 10);

         System.Drawing.Point p3 = new System.Drawing.Point(LocalPosition.X, LocalPosition.Y);
         p3.Offset(-10, 0);
         System.Drawing.Point p4 = new System.Drawing.Point(LocalPosition.X, LocalPosition.Y);
         p4.Offset(10, 0);

         g.DrawLine(Pen, p1.X, p1.Y, p2.X, p2.Y);
         g.DrawLine(Pen, p3.X, p3.Y, p4.X, p4.Y);
      }

      public override void Dispose()
      {
         base.Dispose();
      }

#if !PocketPC

      #region ISerializable Members

      void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
      {
         base.GetObjectData(info, context);
      }

      protected GMarkerCross(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }

      #endregion

#endif
   }
}
