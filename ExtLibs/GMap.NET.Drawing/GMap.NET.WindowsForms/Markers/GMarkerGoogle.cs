
using System.ComponentModel;
using System.IO;
using SvgNet.SvgGdi;

namespace GMap.NET.WindowsForms.Markers
{
   using System.Drawing;
   using System.Collections.Generic;

#if !PocketPC
    using GMap.NET.Drawing.Properties;
   using System;
   using System.Runtime.Serialization;
#else
   using GMap.NET.WindowsMobile.Properties;
#endif

   public enum GMarkerGoogleType
   {
      none = 0,
      arrow,
      blue,
      blue_small,
      blue_dot,
      blue_pushpin,
      brown_small,
      gray_small,
      green,
      green_small,
      green_dot,
      green_pushpin,
      green_big_go,
      yellow,
      yellow_small,
      yellow_dot,
      yellow_big_pause,
      yellow_pushpin,
      lightblue,
      lightblue_dot,
      lightblue_pushpin,
      orange,
      orange_small,
      orange_dot,
      pink,
      pink_dot,
      pink_pushpin,
      purple,
      purple_small,
      purple_dot,
      purple_pushpin,
      red,
      red_small,
      red_dot,
      red_pushpin,
      red_big_stop,
      black_small,
      white_small,
   }

#if !PocketPC
   [Serializable]
   public class GMarkerGoogle : GMapMarker, ISerializable, IDeserializationCallback
#else
   public class GMarkerGoogle : GMapMarker
#endif
   {
      public float? Bearing;
      Image Bitmap;
      Image BitmapShadow;

      static Image arrowshadow;
      static Image msmarker_shadow;
      static Image shadow_small;
      static Image pushpin_shadow;

      public readonly GMarkerGoogleType Type;

      public GMarkerGoogle(PointLatLng p, GMarkerGoogleType type)
         : base(p)
      {
         this.Type = type;

         if(type != GMarkerGoogleType.none)
         {
            LoadBitmap();
         }
      }

      void LoadBitmap()
      {
         Bitmap = GetIcon(Type.ToString());
        lock(Bitmap)
            Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);

         switch(Type)
         {
            case GMarkerGoogleType.arrow:
            {
               Offset = new Point(-11, -Size.Height);

               if(arrowshadow == null)
               {
                  arrowshadow = Resources.arrowshadow;
               }
               BitmapShadow = arrowshadow;
            }
            break;

            case GMarkerGoogleType.blue:
            case GMarkerGoogleType.blue_dot:
            case GMarkerGoogleType.green:
            case GMarkerGoogleType.green_dot:
            case GMarkerGoogleType.yellow:
            case GMarkerGoogleType.yellow_dot:
            case GMarkerGoogleType.lightblue:
            case GMarkerGoogleType.lightblue_dot:
            case GMarkerGoogleType.orange:
            case GMarkerGoogleType.orange_dot:
            case GMarkerGoogleType.pink:
            case GMarkerGoogleType.pink_dot:
            case GMarkerGoogleType.purple:
            case GMarkerGoogleType.purple_dot:
            case GMarkerGoogleType.red:
            case GMarkerGoogleType.red_dot:
            {
               Offset = new Point(-Size.Width / 2 + 1, -Size.Height + 1);

               if(msmarker_shadow == null)
               {
                  msmarker_shadow = Resources.msmarker_shadow;
               }
               BitmapShadow = msmarker_shadow;
            }
            break;

            case GMarkerGoogleType.black_small:
            case GMarkerGoogleType.blue_small:
            case GMarkerGoogleType.brown_small:
            case GMarkerGoogleType.gray_small:
            case GMarkerGoogleType.green_small:
            case GMarkerGoogleType.yellow_small:
            case GMarkerGoogleType.orange_small:
            case GMarkerGoogleType.purple_small:
            case GMarkerGoogleType.red_small:
            case GMarkerGoogleType.white_small:
            {
               Offset = new Point(-Size.Width / 2, -Size.Height + 1);

               if(shadow_small == null)
               {
                  shadow_small = Resources.shadow_small;
               }
               BitmapShadow = shadow_small;
            }
            break;

            case GMarkerGoogleType.green_big_go:
            case GMarkerGoogleType.yellow_big_pause:
            case GMarkerGoogleType.red_big_stop:
            {
               Offset = new Point(-Size.Width / 2, -Size.Height + 1);
               if(msmarker_shadow == null)
               {
                  msmarker_shadow = Resources.msmarker_shadow;
               }
               BitmapShadow = msmarker_shadow;
            }
            break;

            case GMarkerGoogleType.blue_pushpin:
            case GMarkerGoogleType.green_pushpin:
            case GMarkerGoogleType.yellow_pushpin:
            case GMarkerGoogleType.lightblue_pushpin:
            case GMarkerGoogleType.pink_pushpin:
            case GMarkerGoogleType.purple_pushpin:
            case GMarkerGoogleType.red_pushpin:
            {
               Offset = new Point(-9, -Size.Height + 1);

               if(pushpin_shadow == null)
               {
                  pushpin_shadow = Resources.pushpin_shadow;
               }
               BitmapShadow = pushpin_shadow;
            }
            break;
         }
      }

      /// <summary>
      /// marker using manual bitmap, NonSerialized
      /// </summary>
      /// <param name="p"></param>
      /// <param name="Bitmap"></param>
      public GMarkerGoogle(PointLatLng p, Bitmap Bitmap)
         : base(p)
      {
         this.Bitmap = Bitmap;
         Size = new System.Drawing.Size(Bitmap.Width, Bitmap.Height);
         Offset = new Point(-Size.Width / 2, -Size.Height);
      }

      static readonly Dictionary<string, Image> iconCache = new Dictionary<string, Image>();

      internal static Image GetIcon(string name)
      {
          Image ret;
         if(!iconCache.TryGetValue(name, out ret))
         {
            ret = Resources.ResourceManager.GetObject(name, Resources.Culture) as Bitmap;
            iconCache.Add(name, ret);
         }
         return ret;
      }

      static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -22), new Point(7, 7), new Point(0, 2) };

      public override void OnRender(IGraphics g)
      {
          if (Math.Abs(LocalPosition.X) > 100000 || Math.Abs(LocalPosition.Y) > 100000)
                return;

#if !PocketPC
         //if(!Bearing.HasValue)
         {
            if(BitmapShadow != null)
            {
               g.DrawImage(BitmapShadow, LocalPosition.X, LocalPosition.Y, BitmapShadow.Width, BitmapShadow.Height);
            }
         }

         //if(Bearing.HasValue)
         //{
         //   g.RotateTransform(Bearing.Value - Overlay.Control.Bearing);
         //   g.FillPolygon(Brushes.Red, Arrow);
         //}

         //if(!Bearing.HasValue)
         {
            g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
         }
#else
         if(BitmapShadow != null)
         {
            DrawImageUnscaled(g, BitmapShadow, LocalPosition.X, LocalPosition.Y);
         }
         DrawImageUnscaled(g, Bitmap, LocalPosition.X, LocalPosition.Y);
#endif
      }

      public override void Dispose()
      {
         if(Bitmap != null)
         {
            if(!iconCache.ContainsValue(Bitmap))
            {
               Bitmap.Dispose();
               Bitmap = null;
            }
         }

         base.Dispose();
      }

#if !PocketPC

      #region ISerializable Members

      void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("type", this.Type);
         info.AddValue("Bearing", this.Bearing);

         base.GetObjectData(info, context);
      }

      protected GMarkerGoogle(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
         this.Type = Extensions.GetStruct<GMarkerGoogleType>(info, "type", GMarkerGoogleType.none);
         this.Bearing = Extensions.GetStruct<float>(info, "Bearing", null);
      }

      #endregion

      #region IDeserializationCallback Members

      public void OnDeserialization(object sender)
      {
         if(Type != GMarkerGoogleType.none)
         {
            LoadBitmap();
         }
      }

      #endregion

#endif
   }
}
