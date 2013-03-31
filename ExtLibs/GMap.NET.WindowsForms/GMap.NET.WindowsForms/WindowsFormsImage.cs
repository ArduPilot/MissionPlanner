
namespace GMap.NET.WindowsForms
{
   using System.Drawing;
   using System.IO;
   using System.Drawing.Imaging;
   using System;
   using System.Diagnostics;

   /// <summary>
   /// image abstraction
   /// </summary>
   public class WindowsFormsImage : PureImage
   {
      public System.Drawing.Image Img;

      public override void Dispose()
      {
         if(Img != null)
         {
            Img.Dispose();
            Img = null;
         }
      }
   }

   /// <summary>
   /// image abstraction proxy
   /// </summary>
   public class WindowsFormsImageProxy : PureImageProxy
   {
#if !PocketPC
      internal ColorMatrix ColorMatrix;
#endif

      public override PureImage FromStream(Stream stream)
      {
         WindowsFormsImage ret = null;
         try
         {
            if(!GMaps.Instance.IsRunningOnMono)
            {
#if !PocketPC
               Image m = Image.FromStream(stream, true, true);
#else
               Image m = new Bitmap(stream);
#endif
               if(m != null)
               {
                  ret = new WindowsFormsImage();
#if !PocketPC
                  ret.Img = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m;
#else
                  ret.Img = m;
#endif
               }
            }
            else // mono yet do not support validation
            {
#if !PocketPC
               Image m = Image.FromStream(stream);
#else
               Image m = new Bitmap(stream);
#endif
               if(m != null)
               {
                  ret = new WindowsFormsImage();
#if !PocketPC
                  ret.Img = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m;
#else
                  ret.Img = m;
#endif
               }
            }
         }
         catch(Exception ex)
         {
            ret = null;
            Debug.WriteLine("FromStream: " + ex.ToString());
         }
         finally
         {
            try
            {
               stream.Seek(0, System.IO.SeekOrigin.Begin);

               if(ret == null)
               {
#if !PocketPC
                  stream.Dispose();
#else
                  IDisposable s = stream as IDisposable;
                  if(s != null)
                  {
                     s.Dispose();
                  }
#endif
               }
            }
            catch
            {
            }
         }
         return ret;
      }

      public override bool Save(Stream stream, GMap.NET.PureImage image)
      {
         WindowsFormsImage ret = image as WindowsFormsImage;
         bool ok = true;

         if(ret.Img != null)
         {
            // try png
            try
            {
               ret.Img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch
            {
               // try jpeg
               try
               {
                  stream.Seek(0, SeekOrigin.Begin);
                  ret.Img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
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
