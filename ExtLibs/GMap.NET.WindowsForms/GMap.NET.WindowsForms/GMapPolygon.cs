
namespace GMap.NET.WindowsForms
{
   using System.Collections.Generic;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Runtime.Serialization;
   using GMap.NET;

   /// <summary>
   /// GMap.NET polygon
   /// </summary>
   [System.Serializable]
#if !PocketPC
   public class GMapPolygon : MapRoute, ISerializable, IDeserializationCallback
#else
   public class GMapPolygon : MapRoute
#endif
   {
      private bool visible = true;

      /// <summary>
      /// is marker visible
      /// </summary>
      public bool IsVisible
      {
         get
         {
            return visible;
         }
         set
         {
            if(value != visible)
            {
               visible = value;

               if(Overlay != null && Overlay.Control != null)
               {
                  if(visible)
                  {
                     Overlay.Control.UpdatePolygonLocalPosition(this);
                  }

                  {
                     if(!Overlay.Control.HoldInvalidation)
                     {
                        Overlay.Control.Invalidate();
                     }
                  }
               }
            }
         }
      }

      GMapOverlay overlay;
      public GMapOverlay Overlay
      {
         get
         {
            return overlay;
         }
         internal set
         {
            overlay = value;
         }
      }

      //public double Area
      //{
      //   get
      //   {
      //      return 0;
      //   }
      //}

      /// <summary>
      /// specifies how the outline is painted
      /// </summary>
#if !PocketPC
      public Pen Stroke = new Pen(Color.FromArgb(155, Color.MidnightBlue));
#else
      public Pen Stroke = new Pen(Color.MidnightBlue);
#endif

      /// <summary>
      /// background color
      /// </summary>
#if !PocketPC
      public Brush Fill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));
#else
      public Brush Fill = new System.Drawing.SolidBrush(Color.AliceBlue);
#endif

      public readonly List<GPoint> LocalPoints = new List<GPoint>();

      public GMapPolygon(List<PointLatLng> points, string name)
         : base(points, name)
      {
         LocalPoints.Capacity = Points.Count;

#if !PocketPC
         Stroke.LineJoin = LineJoin.Round;
#endif
         Stroke.Width = 5;
      }

#if !PocketPC
      #region ISerializable Members

      /// <summary>
      /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
      /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
      /// <exception cref="T:System.Security.SecurityException">
      /// The caller does not have the required permission.
      /// </exception>
      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         base.GetObjectData(info, context);
         info.AddValue("Stroke", this.Stroke);
         info.AddValue("Fill", this.Fill);
         info.AddValue("LocalPoints", this.LocalPoints.ToArray());
      }

      // Temp store for de-serialization.
      private GPoint[] deserializedLocalPoints;

      /// <summary>
      /// Initializes a new instance of the <see cref="MapRoute"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected GMapPolygon(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
         this.Stroke = Extensions.GetValue<Pen>(info, "Stroke", new Pen(Color.FromArgb(155, Color.MidnightBlue)));
         this.Fill = Extensions.GetValue<Brush>(info, "Fill", new SolidBrush(Color.FromArgb(155, Color.AliceBlue)));
         this.deserializedLocalPoints = Extensions.GetValue<GPoint[]>(info, "LocalPoints");
      }

      #endregion

      #region IDeserializationCallback Members

      /// <summary>
      /// Runs when the entire object graph has been de-serialized.
      /// </summary>
      /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
      public override void OnDeserialization(object sender)
      {
         base.OnDeserialization(sender);

         // Accounts for the de-serialization being breadth first rather than depth first.
         LocalPoints.AddRange(deserializedLocalPoints);
         LocalPoints.Capacity = Points.Count;
      }

      #endregion
#endif
   }
}
