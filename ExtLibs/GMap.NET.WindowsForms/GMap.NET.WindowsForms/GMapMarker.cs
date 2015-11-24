
namespace GMap.NET.WindowsForms
{
   using System;
   using System.Drawing;
   using System.Runtime.Serialization;
   using System.Windows.Forms;
   using GMap.NET.WindowsForms.ToolTips;

   /// <summary>
   /// GMap.NET marker
   /// </summary>
   [Serializable]
#if !PocketPC
   public abstract class GMapMarker : ISerializable, IDisposable
#else
   public class GMapMarker: IDisposable
#endif
   {
#if PocketPC
      static readonly System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes();

      static GMapMarker()
      {
         attr.SetColorKey(Color.White, Color.White);
      }
#endif
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

      private PointLatLng position;
      public virtual PointLatLng Position
      {
         get
         {
            return position;
         }
         set
         {
            if(position != value)
            {
               position = value;

               if(IsVisible)
               {
                  if(Overlay != null && Overlay.Control != null)
                  {
                     Overlay.Control.UpdateMarkerLocalPosition(this);
                  }
               }
            }
         }
      }

      public object Tag;

      Point offset;
      public Point Offset
      {
         get
         {
            return offset;
         }
         set
         {
            if(offset != value)
            {
               offset = value;

               if(IsVisible)
               {
                  if(Overlay != null && Overlay.Control != null)
                  {
                     Overlay.Control.UpdateMarkerLocalPosition(this);
                  }
               }
            }
         }
      }

      Rectangle area;

      /// <summary>
      /// marker position in local coordinates, internal only, do not set it manualy
      /// </summary>
      public Point LocalPosition
      {
         get
         {
            return area.Location;
         }
         set
         {
            if(area.Location != value)
            {
               area.Location = value;
               {
                  if(Overlay != null && Overlay.Control != null)
                  {
                     if(!Overlay.Control.HoldInvalidation)
                     {
                        Overlay.Control.Core.Refresh.Set();
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// ToolTip position in local coordinates
      /// </summary>
      public Point ToolTipPosition
      {
         get
         {
            Point ret = area.Location;
            ret.Offset(-Offset.X, -Offset.Y);
            return ret;
         }
      }

      public Size Size
      {
         get
         {
            return area.Size;
         }
         set
         {
            area.Size = value;
         }
      }

      public Rectangle LocalArea
      {
         get
         {
            return area;
         }
      }

      internal Rectangle LocalAreaInControlSpace
      {
         get
         {
            Rectangle r = area;
            if(Overlay != null && Overlay.Control != null)
            {
               r.Offset((int)Overlay.Control.Core.renderOffset.X, (int)overlay.Control.Core.renderOffset.Y);
            }
            return r;
         }
      }

      public GMapToolTip ToolTip;

      public MarkerTooltipMode ToolTipMode = MarkerTooltipMode.OnMouseOver;

      string toolTipText;
      public string ToolTipText
      {
         get
         {
            return toolTipText;
         }

         set
         {
            if(ToolTip == null && !string.IsNullOrEmpty(value))
            {
#if !PocketPC
               ToolTip = new GMapRoundedToolTip(this);
#else
               ToolTip = new GMapToolTip(this);
#endif
            }
            toolTipText = value;
         }
      }

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
                     Overlay.Control.UpdateMarkerLocalPosition(this);
                  }
                  else
                  {
                      if (Overlay.Control.IsMouseOverMarker)
                      {
                          Overlay.Control.IsMouseOverMarker = false;
#if !PocketPC
                          Overlay.Control.RestoreCursorOnLeave();
#endif
                      }
                  }

                  {
                     if(!Overlay.Control.HoldInvalidation)
                     {
                        Overlay.Control.Core.Refresh.Set();
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// if true, marker will be rendered even if it's outside current view
      /// </summary>
      public bool DisableRegionCheck = false;

      /// <summary>
      /// can maker receive input
      /// </summary>
      public bool IsHitTestVisible = true;

      private bool isMouseOver = false;

      /// <summary>
      /// is mouse over marker
      /// </summary>
      public bool IsMouseOver
      {
         get
         {
            return isMouseOver;
         }
         internal set
         {
            isMouseOver = value;
         }
      }

      public GMapMarker(PointLatLng pos)
      {
         this.Position = pos;
      }

      public virtual void OnRender(Graphics g)
      {
         //
      }

#if PocketPC
      protected void DrawImageUnscaled(Graphics g, Bitmap inBmp, int x, int y)
      {
         g.DrawImage(inBmp, new Rectangle(x, y, inBmp.Width, inBmp.Height), 0, 0, inBmp.Width, inBmp.Height, GraphicsUnit.Pixel, attr);
      }
#endif

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
         info.AddValue("Position", this.Position);
         info.AddValue("Tag", this.Tag);
         info.AddValue("Offset", this.Offset);
         info.AddValue("Area", this.area);
         info.AddValue("ToolTip", this.ToolTip);
         info.AddValue("ToolTipMode", this.ToolTipMode);
         info.AddValue("ToolTipText", this.ToolTipText);
         info.AddValue("Visible", this.IsVisible);
         info.AddValue("DisableregionCheck", this.DisableRegionCheck);
         info.AddValue("IsHitTestVisible", this.IsHitTestVisible);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="GMapMarker"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected GMapMarker(SerializationInfo info, StreamingContext context)
      {
         this.Position = Extensions.GetStruct<PointLatLng>(info, "Position", PointLatLng.Empty);
         this.Tag = Extensions.GetValue<object>(info, "Tag", null);
         this.Offset = Extensions.GetStruct<Point>(info, "Offset", Point.Empty);
         this.area = Extensions.GetStruct<Rectangle>(info, "Area", Rectangle.Empty);
         
         this.ToolTip = Extensions.GetValue<GMapToolTip>(info, "ToolTip", null);
         if (this.ToolTip != null) this.ToolTip.Marker = this;

         this.ToolTipMode = Extensions.GetStruct<MarkerTooltipMode>(info, "ToolTipMode", MarkerTooltipMode.OnMouseOver);
         this.ToolTipText = info.GetString("ToolTipText");
         this.IsVisible = info.GetBoolean("Visible");
         this.DisableRegionCheck = info.GetBoolean("DisableregionCheck");
         this.IsHitTestVisible = info.GetBoolean("IsHitTestVisible");
      }

      #endregion
#endif

      #region IDisposable Members

      bool disposed = false;

      public virtual void Dispose()
      {
         if(!disposed)
         {
            disposed = true;

            Tag = null;

            if(ToolTip != null)
            {
               toolTipText = null;
               ToolTip.Dispose();
               ToolTip = null;
            }
         }
      }

      #endregion
   }

   public delegate void MarkerClick(GMapMarker item, MouseEventArgs e);
   public delegate void MarkerEnter(GMapMarker item);
   public delegate void MarkerLeave(GMapMarker item);

   /// <summary>
   /// modeof tooltip
   /// </summary>
   public enum MarkerTooltipMode
   {
      OnMouseOver,
      Never,
      Always,
   }
}
