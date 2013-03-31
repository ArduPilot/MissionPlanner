
namespace GMap.NET.CacheProviders
{
   using System.IO;
   using System.Text;
   using System;
   using System.Diagnostics;
   using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;

   /// <summary>
   /// ultra fast cache system for tiles
   /// </summary>
   public class SQLitePureImageCache : PureImageCache
   {
      string cache;
      string gtileCache;
      string dir;
      string db;
      bool Created = false;

      public string GtileCache
      {
         get
         {
            return gtileCache;
         }
      }

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
            gtileCache = cache + "TileDBv3" + Path.DirectorySeparatorChar;

            dir = gtileCache + GMaps.Instance.LanguageStr + Path.DirectorySeparatorChar;

            cache = dir;

            // precreate dir
            if(!Directory.Exists(dir))
            {
               Directory.CreateDirectory(dir);
            }

            Created = true;
         }
      }

      #region -- import / export --
      public static bool CreateEmptyDB(string file)
      {
         bool ret = true;

         try
         {
            string dir = Path.GetDirectoryName(file);
            if(!Directory.Exists(dir))
            {
               Directory.CreateDirectory(dir);
            }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("CreateEmptyDB: " + ex.ToString());
#endif
            Debug.WriteLine("CreateEmptyDB: " + ex.ToString());
            ret = false;
         }
         return ret;
      }

      private static bool AlterDBAddTimeColumn(string file)
      {
         bool ret = true;

         return ret;
      }

      public static bool VacuumDb(string file)
      {
         bool ret = true;

         return ret;
      }

      public static bool ExportMapDataToDB(string sourceFile, string destFile)
      {
         bool ret = true;

         return ret;
      }
      #endregion

      static readonly string singleSqlSelect = "SELECT Tile FROM main.TilesData WHERE id = (SELECT id FROM main.Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3})";
      static readonly string singleSqlInsert = "INSERT INTO main.Tiles(X, Y, Zoom, Type, CacheTime) VALUES(@p1, @p2, @p3, @p4, @p5)";
      static readonly string singleSqlInsertLast = "INSERT INTO main.TilesData(id, Tile) VALUES((SELECT last_insert_rowid()), @p1)";

      string ConnectionString;

      readonly List<string> AttachedCaches = new List<string>();
      string finnalSqlSelect = singleSqlSelect;
      string attachSqlQuery = string.Empty;
      string detachSqlQuery = string.Empty;

      void RebuildFinnalSelect()
      {

      }

      public void Attach(string db)
      {
         if(!AttachedCaches.Contains(db))
         {
            AttachedCaches.Add(db);
            RebuildFinnalSelect();
         }
      }

      public void Detach(string db)
      {
         if(AttachedCaches.Contains(db))
         {
            AttachedCaches.Remove(db);
            RebuildFinnalSelect();
         }
      }

      #region PureImageCache Members

      bool PureImageCache.PutImageToCache(MemoryStream tile, MapType type, GPoint pos, int zoom)
      {
         bool ret = true;
         if(Created)
         {
            try
            {
                string file = CacheLocation + Path.DirectorySeparatorChar + type.ToString() + Path.DirectorySeparatorChar + zoom + Path.DirectorySeparatorChar + pos.Y + Path.DirectorySeparatorChar + pos.X + ".jpg";
                string dir = Path.GetDirectoryName(file);
                Directory.CreateDirectory(dir);
                using (BinaryWriter sw = new BinaryWriter(File.OpenWrite(file)))
                {
                    sw.Write(tile.ToArray());

                    sw.Close();
                }
            }
            catch(Exception ex)
            {
#if MONO
            Console.WriteLine("PutImageToCache: " + ex.ToString());
#endif
               Debug.WriteLine("PutImageToCache: " + ex.ToString());
               ret = false;
            }
         }
         return ret;
      }

      PureImage PureImageCache.GetImageFromCache(MapType type, GPoint pos, int zoom)
      {
         PureImage ret = null;
         try
         {
             string file = CacheLocation + Path.DirectorySeparatorChar + type.ToString() + Path.DirectorySeparatorChar + zoom + Path.DirectorySeparatorChar + pos.Y + Path.DirectorySeparatorChar + pos.X + ".jpg";
             if (File.Exists(file))
             {

                 BinaryReader sr = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read));

                 {
                     byte[] tile = sr.ReadBytes((int)sr.BaseStream.Length);

                                 MemoryStream stm = new MemoryStream(tile, 0, tile.Length, false, true);

                                 ret = GMaps.Instance.ImageProxy.FromStream(stm);
                                 if(ret != null)
                                 {
                                    ret.Data = stm;
                                 }
                 }
             }
         }
         catch(Exception ex)
         {
#if MONO
            Console.WriteLine("GetImageFromCache: " + ex.ToString());
#endif
            Debug.WriteLine("GetImageFromCache: " + ex.ToString());
            ret = null;
         }

         return ret;
      }

      int PureImageCache.DeleteOlderThan(DateTime date)
      {
         int affectedRows = 0;


         return affectedRows;
      }

      public int DeleteOlderThan(DateTime date,MapType type)
      {
          int affectedRows = 0;


          return affectedRows;
      }

      #endregion
   }
}
