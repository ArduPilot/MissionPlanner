
namespace GMap.NET.WindowsForms
{
   using System;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Runtime.Serialization;
   using System.Windows.Forms;
   using GMap.NET.ObjectModel;

   /// <summary>
   /// GMap.NET overlay
   /// </summary>
   [Serializable]
#if !PocketPC
   public class GMapOverlay : ISerializable, IDeserializationCallback
#else
   public class GMapOverlay
#endif
   {
      bool isVisibile = true;

      /// <summary>
      /// is overlay visible
      /// </summary>
      public bool IsVisibile
      {
         get
         {
            return isVisibile;
         }
         set
         {
            if(value != isVisibile)
            {
               isVisibile = value;
               if(isVisibile)
               {
                  Control.HoldInvalidation = true;
                  ForceUpdate();
                  Control.Refresh();
               }
               else
               {
                  if(!Control.HoldInvalidation)
                  {
                     Control.Invalidate();
                  }
               }
            }
         }
      }

      /// <summary>
      /// overlay Id
      /// </summary>
      public string Id;

      /// <summary>
      /// list of markers, should be thread safe
      /// </summary>
      public readonly ObservableCollectionThreadSafe<GMapMarker> Markers = new ObservableCollectionThreadSafe<GMapMarker>();

      /// <summary>
      /// list of routes, should be thread safe
      /// </summary>
      public readonly ObservableCollectionThreadSafe<GMapRoute> Routes = new ObservableCollectionThreadSafe<GMapRoute>();

      /// <summary>
      /// list of polygons, should be thread safe
      /// </summary>
      public readonly ObservableCollectionThreadSafe<GMapPolygon> Polygons = new ObservableCollectionThreadSafe<GMapPolygon>();

      internal GMapControl Control;

      public GMapOverlay(GMapControl control, string id)
      {
         if(control == null)
         {
            throw new Exception("GMapControl in GMapOverlay can't be null");
         }

         Control = control;
         Id = id;
         Markers.CollectionChanged += new NotifyCollectionChangedEventHandler(Markers_CollectionChanged);
         Routes.CollectionChanged += new NotifyCollectionChangedEventHandler(Routes_CollectionChanged);
         Polygons.CollectionChanged += new NotifyCollectionChangedEventHandler(Polygons_CollectionChanged);
      }

      void Polygons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.NewItems != null)
         {
            foreach(GMapPolygon obj in e.NewItems)
            {
               if(obj != null)
               {
                  obj.Overlay = this;
                  Control.UpdatePolygonLocalPosition(obj);
               }
            }
         }

         if(!Control.HoldInvalidation)
         {
            Control.Core_OnNeedInvalidation();
         }
      }

      void Routes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.NewItems != null)
         {
            foreach(GMapRoute obj in e.NewItems)
            {
               if(obj != null)
               {
                  obj.Overlay = this;
                  Control.UpdateRouteLocalPosition(obj);
               }
            }
         }

         if(!Control.HoldInvalidation)
         {
            Control.Core_OnNeedInvalidation();
         }
      }

      void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.NewItems != null)
         {
            foreach(GMapMarker obj in e.NewItems)
            {
               if(obj != null)
               {
                  obj.Overlay = this;
                  Control.UpdateMarkerLocalPosition(obj);
               }
            }
         }

         if(e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
         {
#if !PocketPC
            if(Control.IsMouseOverMarker)
            {
               Control.IsMouseOverMarker = false;
               Control.Cursor = Cursors.Default;
            }
#endif
         }

         if(!Control.HoldInvalidation)
         {
            Control.Core_OnNeedInvalidation();
         }
      }

      /// <summary>
      /// updates local positions of objects
      /// </summary>
      internal void ForceUpdate()
      {
         foreach(GMapMarker obj in Markers)
         {
            if(obj.IsVisible)
            {
               Control.UpdateMarkerLocalPosition(obj);
            }
         }

         foreach(GMapPolygon obj in Polygons)
         {
            if(obj.IsVisible)
            {
               Control.UpdatePolygonLocalPosition(obj);
            }
         }

         foreach(GMapRoute obj in Routes)
         {
            if(obj.IsVisible)
            {
               Control.UpdateRouteLocalPosition(obj);
            }
         }
      }

      /// <summary>
      /// draw routes, override to draw custom
      /// </summary>
      /// <param name="g"></param>
      protected virtual void DrawRoutes(Graphics g)
      {
#if !PocketPC
         foreach(GMapRoute r in Routes)
         {
            if(r.IsVisible)
            {
               using(GraphicsPath rp = new GraphicsPath())
               {
                  for(int i = 0; i < r.LocalPoints.Count; i++)
                  {
                     GPoint p2 = r.LocalPoints[i];

                     if(i == 0)
                     {
                        rp.AddLine(p2.X, p2.Y, p2.X, p2.Y);
                     }
                     else
                     {
                        System.Drawing.PointF p = rp.GetLastPoint();
                        rp.AddLine(p.X, p.Y, p2.X, p2.Y);
                     }
                  }

                  if(rp.PointCount > 0)
                  {
                     g.DrawPath(r.Stroke, rp);
                  }
               }
            }
         }
#else
         foreach(GMapRoute r in Routes)
         {
            if(r.IsVisible)
            {
               Point[] pnts = new Point[r.LocalPoints.Count];
               for(int i = 0; i < r.LocalPoints.Count; i++)
               {
                  Point p2 = new Point(r.LocalPoints[i].X, r.LocalPoints[i].Y);
                  pnts[pnts.Length - 1 - i] = p2;
               }

               if(pnts.Length > 0)
               {
                  g.DrawLines(r.Stroke, pnts);
               }
            }
         }
#endif
      }

      /// <summary>
      /// draw polygons, override to draw custom
      /// </summary>
      /// <param name="g"></param>
      protected virtual void DrawPolygons(Graphics g)
      {
#if !PocketPC
         foreach(GMapPolygon r in Polygons)
         {
            if(r.IsVisible)
            {
               using(GraphicsPath rp = new GraphicsPath())
               {
                  for(int i = 0; i < r.LocalPoints.Count; i++)
                  {
                     GPoint p2 = r.LocalPoints[i];

                     if(i == 0)
                     {
                        rp.AddLine(p2.X, p2.Y, p2.X, p2.Y);
                     }
                     else
                     {
                        System.Drawing.PointF p = rp.GetLastPoint();
                        rp.AddLine(p.X, p.Y, p2.X, p2.Y);
                     }
                  }

                  if(rp.PointCount > 0)
                  {
                     rp.CloseFigure();

                     if (!(((SolidBrush)r.Fill).Color.A == 155 && ((SolidBrush)r.Fill).Color.B == 255 && ((SolidBrush)r.Fill).Color.G == 248 && ((SolidBrush)r.Fill).Color.R == 240))
                        g.FillPath(r.Fill, rp);

                     g.DrawPath(r.Stroke, rp);
                  }
               }
            }
         }
#else
         foreach(GMapPolygon r in Polygons)
         {
            if(r.IsVisible)
            {
               Point[] pnts = new Point[r.LocalPoints.Count];
               for(int i = 0; i < r.LocalPoints.Count; i++)
               {
                  Point p2 = new Point(r.LocalPoints[i].X, r.LocalPoints[i].Y);
                  pnts[pnts.Length - 1 - i] = p2;
               }

               if(pnts.Length > 0)
               {
                  g.FillPolygon(r.Fill, pnts);
                  g.DrawPolygon(r.Stroke, pnts);
               }
            }
         }
#endif
      }

      /// <summary>
      /// renders objects and routes
      /// </summary>
      /// <param name="g"></param>
      public virtual void Render(Graphics g)
      {
         if(Control != null)
         {
            if(Control.RoutesEnabled)
            {
               DrawRoutes(g);
            }

            if(Control.PolygonsEnabled)
            {
               DrawPolygons(g);
            }

            if(Control.MarkersEnabled)
            {
               // markers
               foreach(GMapMarker m in Markers)
               {
                  if(m.IsVisible && (m.DisableRegionCheck || Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y)))
                  {
                     m.OnRender(g);
                  }
               }

               // tooltips above
               foreach(GMapMarker m in Markers)
               {
                  if(m.ToolTip != null && m.IsVisible && Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y))
                  {
                     if(!string.IsNullOrEmpty(m.ToolTipText) && (m.ToolTipMode == MarkerTooltipMode.Always || (m.ToolTipMode == MarkerTooltipMode.OnMouseOver && m.IsMouseOver)))
                     {
                        m.ToolTip.Draw(g);
                     }
                  }
               }
            }
         }
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
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("Id", this.Id);
         info.AddValue("IsVisible", this.IsVisibile);

         GMapMarker[] markerArray = new GMapMarker[this.Markers.Count];
         this.Markers.CopyTo(markerArray, 0);
         info.AddValue("Markers", markerArray);

         GMapRoute[] routeArray = new GMapRoute[this.Routes.Count];
         this.Routes.CopyTo(routeArray, 0);
         info.AddValue("Routes", routeArray);

         GMapPolygon[] polygonArray = new GMapPolygon[this.Polygons.Count];
         this.Polygons.CopyTo(polygonArray, 0);
         info.AddValue("Polygons", polygonArray);
      }

      private GMapMarker[] deserializedMarkerArray;
      private GMapRoute[] deserializedRouteArray;
      private GMapPolygon[] deserializedPolygonArray;

      /// <summary>
      /// Initializes a new instance of the <see cref="GMapOverlay"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected GMapOverlay(SerializationInfo info, StreamingContext context)
      {
         this.Id = info.GetString("Id");
         this.IsVisibile = info.GetBoolean("IsVisible");

         this.deserializedMarkerArray = Extensions.GetValue<GMapMarker[]>(info, "Markers", new GMapMarker[0]);
         this.deserializedRouteArray = Extensions.GetValue<GMapRoute[]>(info, "Routes", new GMapRoute[0]);
         this.deserializedPolygonArray = Extensions.GetValue<GMapPolygon[]>(info, "Polygons", new GMapPolygon[0]);
      }

      #endregion

      #region IDeserializationCallback Members

      /// <summary>
      /// Runs when the entire object graph has been deserialized.
      /// </summary>
      /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
      public void OnDeserialization(object sender)
      {
         // Populate Markers
         foreach(GMapMarker marker in deserializedMarkerArray)
         {
            marker.Overlay = this;
            this.Markers.Add(marker);
         }

         // Populate Routes
         foreach(GMapRoute route in deserializedRouteArray)
         {
            route.Overlay = this;
            this.Routes.Add(route);
         }

         // Populate Polygons
         foreach(GMapPolygon polygon in deserializedPolygonArray)
         {
            polygon.Overlay = this;
            this.Polygons.Add(polygon);
         }
      }

      #endregion
#endif
   }
}