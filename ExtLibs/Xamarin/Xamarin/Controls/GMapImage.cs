
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;

namespace GMap.NET.WindowsForms
{
    using GMap.NET.Internals;
    using GMap.NET.MapProviders;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// image abstraction
    /// </summary>
    public class GMapImage : PureImage
   {
      public Image Img;

      public override void Dispose()
      {
         if(Img != null)
         {
            Img.Dispose();
            Img = null;
         }

         if(Data != null)
         {
            Data.Dispose();
            Data = null;
         }
      }
   }

   /// <summary>
   /// image abstraction proxy
   /// </summary>
   public class GMapImageProxy : PureImageProxy
   {
      GMapImageProxy()
      {

      }

      public static void Enable()
      {
          GMapProvider.TileImageProxy = Instance;
      }

      public static readonly GMapImageProxy Instance = new GMapImageProxy();

#if !PocketPC
       public ColorMatrix ColorMatrix;
#endif

      static readonly bool Win7OrLater = Stuff.IsRunningOnWin7orLater();

      public override PureImage FromStream(Stream stream)
      {
         GMapImage ret = null;
         try
         {
#if !PocketPC
            Image m = Image.FromStream(stream, true, Win7OrLater ? false : true);
#else
            Image m = new Bitmap(stream);
#endif
            if(m != null)
            {
               ret = new GMapImage();
#if !PocketPC
               ret.Img = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m;
#else
               ret.Img = m;
#endif
            }

         }
         catch(Exception ex)
         {
            ret = null;
            Debug.WriteLine("FromStream: " + ex.ToString());
         }

         return ret;
      }

      public override bool Save(Stream stream, GMap.NET.PureImage image)
      {
         GMapImage ret = image as GMapImage;
         bool ok = true;

         if(ret.Img != null)
         {
            // try png
            try
            {
               ret.Img.Save(stream, SKEncodedImageFormat.Png);
            }
            catch
            {
               // try jpeg
               try
               {
                  stream.Seek(0, SeekOrigin.Begin);
                  ret.Img.Save(stream, SKEncodedImageFormat.Jpeg);
               }
               catch
               {
                  ok = false;
               }
            }
         }
         else
         {
            ok = false;
         }

         return ok;
      }

#if !PocketPC
      Bitmap ApplyColorMatrix(Image original, ColorMatrix matrix)
      {
         // create a blank bitmap the same size as original
         Bitmap newBitmap = new Bitmap(original.Width, original.Height);

         using(original) // destroy original
         {
            // get a graphics object from the new image
            using(Graphics g = Graphics.FromImage(newBitmap))
            {
               // set the color matrix attribute
               using(ImageAttributes attributes = new ImageAttributes())
               {
                  attributes.SetColorMatrix(matrix);
                  g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
               }
            }
         }

         return newBitmap;
      }
#endif
   }
}
