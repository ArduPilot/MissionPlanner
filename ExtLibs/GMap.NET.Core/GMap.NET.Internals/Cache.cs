
namespace GMap.NET.Internals
{
   using System.Collections.Generic;
   using System.IO;
   using System.Text;
   using System;
   using System.Diagnostics;
   using GMap.NET.CacheProviders;
   using System.Globalization;

   /// <summary>
   /// cache system for tiles, geocoding, etc...
   /// </summary>
   internal class Cache : Singleton<Cache>
   {
      string cache;
      string routeCache;
      string geoCache;
      string placemarkCache;

      /// <summary>
      /// abstract image cache
      /// </summary>
      public PureImageCache ImageCache;

      /// <summary>
      /// second level abstract image cache
      /// </summary>
      public PureImageCache ImageCacheSecond;

      /// <summary>
      /// local cache location
      /// </summary>
      public string CacheLocation
      {
         get
         {
            return cache;
         }
         set
         {
            cache = value;
            routeCache = cache + "RouteCache" + Path.DirectorySeparatorChar;
            geoCache = cache + "GeocoderCache" + Path.DirectorySeparatorChar;
            placemarkCache = cache + "PlacemarkCache" + Path.DirectorySeparatorChar;

#if SQLite
            if(ImageCache is myPureImageCache)
            {
               (ImageCache as myPureImageCache).CacheLocation = value;
            }
#else
            if(ImageCache is MsSQLCePureImageCache)
            {
               (ImageCache as MsSQLCePureImageCache).CacheLocation = value;
            }
#endif
         }
      }

      public Cache()
      {
         #region singleton check
         if(Instance != null)
         {
            throw (new System.Exception("You have tried to create a new singleton class where you should have instanced it. Replace your \"new class()\" with \"class.Instance\""));
         }
         #endregion

#if SQLite
         ImageCache = new myPureImageCache();
#else
         // you can use $ms stuff if you like too ;}
         ImageCache = new MsSQLCePureImageCache();
#endif

         if(string.IsNullOrEmpty(CacheLocation))
         {
#if PocketPC
            // use sd card if exist for cache
            string sd = Native.GetRemovableStorageDirectory();
            if(!string.IsNullOrEmpty(sd))
            {
               CacheLocation = sd + Path.DirectorySeparatorChar +  "GMap.NET" + Path.DirectorySeparatorChar;
            }
            else
#endif
            {
               string oldCache = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "GMap.NET" + Path.DirectorySeparatorChar;
#if PocketPC
               CacheLocation = oldCache;
#else
               string newCache = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "GMap.NET" + Path.DirectorySeparatorChar;

               // move database to non-roaming user directory
               if(Directory.Exists(oldCache))
               {
                  try
                  {
                     if(Directory.Exists(newCache))
                     {
                        Directory.Delete(oldCache, true);
                     }
                     else
                     {
                        Directory.Move(oldCache, newCache);
                     }
                     CacheLocation = newCache;
                  }
                  catch(Exception ex)
                  {
                     CacheLocation = oldCache;
                     Trace.WriteLine("SQLitePureImageCache, moving data: " + ex.ToString());
                  }
               }
               else
               {
                  CacheLocation = newCache;
               }
#endif
            }
         }
      }

      #region -- etc cache --
      public void CacheGeocoder(string urlEnd, string content)
      {
         try
         {
            // precrete dir
            if(!Directory.Exists(geoCache))
            {
               Directory.CreateDirectory(geoCache);
            }

            StringBuilder file = new StringBuilder(geoCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.geo", urlEnd);

            using(StreamWriter writer = new StreamWriter(file.ToString(), false, Encoding.UTF8))
            {
               writer.Write(content);
            }
         }
         catch
         {
         }
      }

      public string GetGeocoderFromCache(string urlEnd)
      {
         string ret = null;

         try
         {
            StringBuilder file = new StringBuilder(geoCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.geo", urlEnd);

            if(File.Exists(file.ToString()))
            {
               using(StreamReader r = new StreamReader(file.ToString(), Encoding.UTF8))
               {
                  ret = r.ReadToEnd();
               }
            }
         }
         catch
         {
            ret = null;
         }

         return ret;
      }

      public void CachePlacemark(string urlEnd, string content)
      {
         try
         {
            // precrete dir
            if(!Directory.Exists(placemarkCache))
            {
               Directory.CreateDirectory(placemarkCache);
            }

            StringBuilder file = new StringBuilder(placemarkCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.plc", urlEnd);

            using(StreamWriter writer = new StreamWriter(file.ToString(), false, Encoding.UTF8))
            {
               writer.Write(content);
            }
         }
         catch
         {
         }
      }

      public string GetPlacemarkFromCache(string urlEnd)
      {
         string ret = null;

         try
         {
            StringBuilder file = new StringBuilder(placemarkCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.plc", urlEnd);

            if(File.Exists(file.ToString()))
            {
               using(StreamReader r = new StreamReader(file.ToString(), Encoding.UTF8))
               {
                  ret= r.ReadToEnd();
               }
            }
         }
         catch
         {
            ret = null;
         }

         return ret;
      }

      public void CacheRoute(string urlEnd, string content)
      {
         try
         {
            // precrete dir
            if(!Directory.Exists(routeCache))
            {
               Directory.CreateDirectory(routeCache);
            }

            StringBuilder file = new StringBuilder(routeCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.dragdir", urlEnd);

            using(StreamWriter writer = new StreamWriter(file.ToString(), false, Encoding.UTF8))
            {
               writer.Write(content);
            }
         }
         catch
         {
         }
      }

      public string GetRouteFromCache(string urlEnd)
      {
         string ret = null;

         try
         {
            StringBuilder file = new StringBuilder(routeCache);
            file.AppendFormat(CultureInfo.InvariantCulture, "{0}.dragdir", urlEnd);

            if(File.Exists(file.ToString()))
            {
               using(StreamReader r = new StreamReader(file.ToString(), Encoding.UTF8))
               {
                  ret= r.ReadToEnd();
               }
            }
         }
         catch
         {
            ret = null;
         }

         return ret;
      }
      #endregion
   }
}
