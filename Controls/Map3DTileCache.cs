using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Disk cache for Map3D tile altitude data.
    /// Stores pre-computed altitude grids to avoid repeated SRTM lookups.
    /// </summary>
    public class Map3DTileCache
    {
        private static readonly object _lock = new object();
        private static string _cacheDir;

        /// <summary>
        /// Gets or sets the cache directory. Defaults to LocalApplicationData/MissionPlanner/Map3DCache
        /// </summary>
        public static string CacheDirectory
        {
            get
            {
                if (_cacheDir == null)
                {
                    _cacheDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "MissionPlanner", "Map3DCache");
                }
                return _cacheDir;
            }
            set { _cacheDir = value; }
        }

        /// <summary>
        /// Cached tile altitude data
        /// </summary>
        public class CachedTileData
        {
            public int Zoom { get; set; }
            public long X { get; set; }
            public long Y { get; set; }
            public int GridWidth { get; set; }
            public int GridHeight { get; set; }
            public int PxStep { get; set; }
            public double[] Altitudes { get; set; }  // Flattened altitude grid [gridWidth * gridHeight]
            public byte[] ImageData { get; set; }    // PNG-encoded satellite image
        }

        /// <summary>
        /// Gets the cache file path for a tile
        /// </summary>
        private static string GetCachePath(long x, long y, int zoom)
        {
            // Organize by zoom level for easier management
            var zoomDir = Path.Combine(CacheDirectory, $"z{zoom}");
            return Path.Combine(zoomDir, $"{x}_{y}.bin");
        }

        /// <summary>
        /// Saves tile altitude data to disk cache
        /// </summary>
        public static void SaveTile(long x, long y, int zoom, int gridWidth, int gridHeight, int pxStep,
            double[,] altitudes, Image image)
        {
            try
            {
                var path = GetCachePath(x, y, zoom);
                var dir = Path.GetDirectoryName(path);

                lock (_lock)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }

                // Convert image to PNG bytes
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                // Write binary file
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(zoom);
                    bw.Write(x);
                    bw.Write(y);
                    bw.Write(gridWidth);
                    bw.Write(gridHeight);
                    bw.Write(pxStep);

                    // Write altitudes
                    for (int gx = 0; gx < gridWidth; gx++)
                        for (int gy = 0; gy < gridHeight; gy++)
                            bw.Write(altitudes[gx, gy]);

                    bw.Write(imageBytes.Length);
                    bw.Write(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Map3DTileCache.SaveTile error: {ex.Message}");
            }
        }

        /// <summary>
        /// Tries to load a tile from disk cache at the exact zoom level
        /// </summary>
        public static CachedTileData LoadTile(long x, long y, int zoom)
        {
            try
            {
                var path = GetCachePath(x, y, zoom);
                if (!File.Exists(path))
                    return null;

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var br = new BinaryReader(fs))
                {
                    var cached = new CachedTileData();
                    cached.Zoom = br.ReadInt32();
                    cached.X = br.ReadInt64();
                    cached.Y = br.ReadInt64();
                    cached.GridWidth = br.ReadInt32();
                    cached.GridHeight = br.ReadInt32();
                    cached.PxStep = br.ReadInt32();

                    // Read altitudes
                    int altCount = cached.GridWidth * cached.GridHeight;
                    cached.Altitudes = new double[altCount];
                    for (int i = 0; i < altCount; i++)
                        cached.Altitudes[i] = br.ReadDouble();

                    int imageLen = br.ReadInt32();
                    cached.ImageData = br.ReadBytes(imageLen);

                    return cached;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Map3DTileCache.LoadTile error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tries to load a tile from disk cache, checking the requested zoom or better (higher) zoom levels.
        /// Returns the best available cached tile data.
        /// </summary>
        public static CachedTileData LoadTileOrBetter(long x, long y, int requestedZoom, int maxZoom)
        {
            // First, try exact zoom
            var cached = LoadTile(x, y, requestedZoom);
            if (cached != null)
                return cached;

            // Try higher zoom levels (better resolution)
            for (int z = requestedZoom + 1; z <= maxZoom; z++)
            {
                int zoomDiff = z - requestedZoom;
                long scale = 1L << zoomDiff;
                long higherX = x * scale + scale / 2;
                long higherY = y * scale + scale / 2;

                cached = LoadTile(higherX, higherY, z);
                if (cached != null)
                    return cached;
            }

            return null;
        }

        /// <summary>
        /// Clears all cached tiles
        /// </summary>
        public static void ClearCache()
        {
            try
            {
                if (Directory.Exists(CacheDirectory))
                {
                    Directory.Delete(CacheDirectory, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Map3DTileCache.ClearCache error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the total size of the cache in bytes
        /// </summary>
        public static long GetCacheSize()
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                    return 0;

                return new DirectoryInfo(CacheDirectory)
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(f => f.Length);
            }
            catch
            {
                return 0;
            }
        }
    }
}
