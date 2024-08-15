using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Maps
{
    public class MyImageCache : PureImageCache
    {
        string cache;
        string gtileCache;
        string dir;
        bool Created = false;

        public static MyImageCache Instance;

        public MyImageCache()
        {
            Instance = this;

            CacheLocation = Settings.GetDataDirectory() +
                            "gmapcache" + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// local cache location
        /// </summary>
        public string CacheLocation
        {
            get { return cache; }
            set
            {
                cache = value;
                gtileCache = cache + "TileDBv3" + Path.DirectorySeparatorChar;

                dir = gtileCache + GMapProvider.LanguageStr + Path.DirectorySeparatorChar;

                cache = dir;

                // precreate dir
                if (!Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                    }
                    catch
                    {
                        throw;
                    }
                }

                Created = true;
            }
        }

        #region PureImageCache Members

        bool PureImageCache.PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
        {
            bool ret = true;
            if (Created)
            {
                try
                {
                    string file = CacheLocation + Path.DirectorySeparatorChar + GMapProviders.TryGetProvider(type).Name +
                                  Path.DirectorySeparatorChar + zoom + Path.DirectorySeparatorChar + pos.Y +
                                  Path.DirectorySeparatorChar + pos.X + ".jpg";
                    string dir = Path.GetDirectoryName(file);
                    Directory.CreateDirectory(dir);
                    using (BinaryWriter sw = new BinaryWriter(File.OpenWrite(file)))
                    {
                        sw.Write(tile.ToArray());
                    }
                }
                catch (Exception ex)
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

        PureImage PureImageCache.GetImageFromCache(int type, GPoint pos, int zoom)
        {
            PureImage ret = null;
            try
            {
                string file = CacheLocation + Path.DirectorySeparatorChar + GMapProviders.TryGetProvider(type).Name +
                              Path.DirectorySeparatorChar + zoom + Path.DirectorySeparatorChar + pos.Y +
                              Path.DirectorySeparatorChar + pos.X + ".jpg";
                if (File.Exists(file))
                {
                    using (
                        BinaryReader sr =
                            new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        byte[] tile = sr.ReadBytes((int) sr.BaseStream.Length);

                        MemoryStream stm = new MemoryStream(tile, 0, tile.Length, false, true);

                        ret = GMapImageProxy.Instance.FromStream(stm);
                        if (ret != null)
                        {
                            ret.Data = stm;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if MONO
            Console.WriteLine("GetImageFromCache: " + ex.ToString());
#endif
                Debug.WriteLine("GetImageFromCache: " + ex.ToString());
                ret = null;
            }

            return ret;
        }

        int PureImageCache.DeleteOlderThan(DateTime date, int? type)
        {
            int affectedRows = 0;

            if (!type.HasValue)
                return 0;

            string file = CacheLocation + Path.DirectorySeparatorChar + GMapProviders.TryGetProvider(type.Value).Name +
                          Path.DirectorySeparatorChar;

            if (!Directory.Exists(file))
            {
                return 0;
            }

            string[] files = Directory.GetFiles(file, "*.jpg", SearchOption.AllDirectories);

            foreach (var filen in files)
            {
                try
                {
                    var fi = new FileInfo(filen);
                    if (fi.CreationTime < date)
                    {
                        File.Delete(filen);
                        affectedRows++;
                    }
                }
                catch
                {
                }
            }

            files = Directory.GetFiles(file, "*.png", SearchOption.AllDirectories);

            foreach (var filen in files)
            {
                try
                {
                    var fi = new FileInfo(filen);
                    if (fi.CreationTime < date)
                    {
                        File.Delete(filen);
                        affectedRows++;
                    }
                }
                catch
                {
                }
            }

            return affectedRows;
        }

        public bool CheckImageFromCache(int type, GPoint pos, int zoom)
        {
            bool ret = false;
            if (Created)
            {
                try
                {
                    string file = CacheLocation + Path.DirectorySeparatorChar + GMapProviders.TryGetProvider(type).Name +
                                  Path.DirectorySeparatorChar + zoom + Path.DirectorySeparatorChar + pos.Y +
                                  Path.DirectorySeparatorChar + pos.X + ".jpg";
                    if (File.Exists(file))
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
#if MONO
            Console.WriteLine("CheckImageFromCache: " + ex.ToString());
#endif
                    Debug.WriteLine("CheckImageFromCache: " + ex.ToString());
                    ret = false;
                }
            }
            return ret;
        }

        #endregion
    }
}