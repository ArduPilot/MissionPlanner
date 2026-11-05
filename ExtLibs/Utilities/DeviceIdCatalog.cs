using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Downloads ArduPilot's generated device catalog and keeps a validated disk cache.
    /// Lookups only use local data; refreshes run separately in the background.
    /// </summary>
    public sealed class DeviceIdCatalog
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeviceIdCatalog));
        private const string SourceUrl = "https://autotest.ardupilot.org/LogMessages/devid.json";
        // Match the existing LogMetaData and parameter metadata cache lifetime.
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromDays(7);
        private static readonly Lazy<DeviceIdCatalog> Default =
            new Lazy<DeviceIdCatalog>(() => new DeviceIdCatalog());

        private readonly string _cacheFile;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private volatile Dictionary<string, Dictionary<int, string>> _deviceTypes;

        private DeviceIdCatalog()
        {
            _cacheFile = Path.Combine(Settings.GetDataDirectory(), "devid.json");
            try
            {
                if (File.Exists(_cacheFile))
                    _deviceTypes = Parse(File.ReadAllText(_cacheFile));
            }
            catch (JsonException ex)
            {
                log.Warn("Unable to parse cached device catalog", ex);
            }
            catch (IOException ex)
            {
                log.Warn("Unable to read cached device catalog", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Warn("Unable to access cached device catalog", ex);
            }
        }

        /// <summary>
        /// Looks up a category/type pair, falling back to the caller's enum/numeric name.
        /// This never downloads data or waits for a refresh.
        /// </summary>
        public static string GetName(string category, int value, string fallback)
        {
            return Default.Value.Lookup(category, value, fallback);
        }

        /// <summary>
        /// Refreshes a missing, invalid or older-than-seven-days cache. An unsuccessful
        /// download or parse leaves the last valid cache available, including offline.
        /// Call from a background task to keep disk operations off the UI thread.
        /// </summary>
        public static Task UpdateAsync()
        {
            return Default.Value.RefreshAsync();
        }

        private string Lookup(string category, int value, string fallback)
        {
            var deviceTypes = _deviceTypes;
            if (category != null && deviceTypes != null &&
                deviceTypes.TryGetValue(category, out var types) && types.TryGetValue(value, out var name))
                return name;

            return fallback;
        }

        private async Task RefreshAsync()
        {
            await _refreshLock.WaitAsync().ConfigureAwait(false);
            string pendingFile = null;
            try
            {
                if (_deviceTypes != null && File.Exists(_cacheFile) &&
                    DateTime.UtcNow - File.GetLastWriteTimeUtc(_cacheFile) < CacheLifetime)
                    return;

                Directory.CreateDirectory(Path.GetDirectoryName(_cacheFile));
                pendingFile = _cacheFile + "." + Guid.NewGuid().ToString("N") + ".tmp";
                if (!await Download.getFilefromNetAsync(SourceUrl, pendingFile).ConfigureAwait(false))
                    return;

                // Validate before replacing anything. A failed refresh must not discard
                // a working cache. Use the same directory so replacement is atomic.
                var deviceTypes = Parse(File.ReadAllText(pendingFile));
                File.SetLastWriteTimeUtc(pendingFile, DateTime.UtcNow);
                if (File.Exists(_cacheFile))
                    File.Replace(pendingFile, _cacheFile, null);
                else
                    File.Move(pendingFile, _cacheFile);

                _deviceTypes = deviceTypes;
            }
            catch (JsonException ex)
            {
                log.Warn("Unable to parse downloaded device catalog", ex);
            }
            catch (HttpRequestException ex)
            {
                log.Warn("Unable to download device catalog", ex);
            }
            catch (OperationCanceledException ex)
            {
                log.Warn("Device catalog download was cancelled", ex);
            }
            catch (IOException ex)
            {
                log.Warn("Unable to cache device catalog", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Warn("Unable to write device catalog cache", ex);
            }
            finally
            {
                if (pendingFile != null)
                {
                    DeletePendingFile(pendingFile);
                    // Download.getFilefromNetAsync writes its partial download here.
                    DeletePendingFile(pendingFile + ".new");
                }
                _refreshLock.Release();
            }
        }

        private static void DeletePendingFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException ex)
            {
                log.Warn("Unable to remove device catalog download", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Warn("Unable to remove device catalog download", ex);
            }
        }

        private static Dictionary<string, Dictionary<int, string>> Parse(string json)
        {
            var catalog = JsonConvert.DeserializeObject<Catalog>(json);
            if (catalog == null || catalog.FormatVersion != 1 || catalog.DeviceTypes == null ||
                catalog.DeviceTypes.Count == 0)
                throw new JsonSerializationException("Unsupported or empty device catalog.");

            var result = new Dictionary<string, Dictionary<int, string>>();
            int typeCount = 0;
            foreach (var category in catalog.DeviceTypes)
            {
                if (category.Value == null)
                    throw new JsonSerializationException("Missing device types.");

                var types = new Dictionary<int, string>();
                foreach (var type in category.Value)
                {
                    if (type == null || type.Value < 0 || type.Value > byte.MaxValue ||
                        string.IsNullOrWhiteSpace(type.Name) || types.ContainsKey(type.Value))
                        throw new JsonSerializationException("Invalid device type.");

                    types.Add(type.Value, type.Name);
                    typeCount++;
                }
                result.Add(category.Key, types);
            }
            if (typeCount == 0)
                throw new JsonSerializationException("Empty device catalog.");
            return result;
        }

        private class Catalog
        {
            [JsonProperty("format_version", Required = Required.Always)]
            public int FormatVersion { get; set; }

            [JsonProperty("device_types", Required = Required.Always)]
            public Dictionary<string, List<DeviceType>> DeviceTypes { get; set; }
        }

        private class DeviceType
        {
            [JsonProperty("value", Required = Required.Always)]
            public int Value { get; set; }

            [JsonProperty("name", Required = Required.Always)]
            public string Name { get; set; }
        }
    }
}
