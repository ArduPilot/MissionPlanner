using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Offline snapshot of https://autotest.ardupilot.org/LogMessages/devid.json.
    /// Refresh devid.json from that URL without editing the generated contents.
    /// </summary>
    public static class DeviceIdCatalog
    {
        private static readonly Lazy<Dictionary<string, Dictionary<int, string>>> DeviceTypes =
            new Lazy<Dictionary<string, Dictionary<int, string>>>(Load);

        /// <summary>
        /// Looks up a type within compass, imu, baro or airspeed. The caller supplies
        /// its existing enum/numeric representation for unknown types or an unavailable catalog.
        /// </summary>
        public static string GetName(string category, int value, string fallback)
        {
            if (category != null && DeviceTypes.Value.TryGetValue(category, out var types) &&
                types.TryGetValue(value, out var name))
                return name;

            return fallback;
        }

        private static Dictionary<string, Dictionary<int, string>> Load()
        {
            try
            {
                using (var stream = typeof(DeviceIdCatalog).Assembly.GetManifestResourceStream(
                    "MissionPlanner.Utilities.devid.json"))
                {
                    if (stream == null)
                        throw new IOException("Embedded devid.json was not found.");

                    using (var reader = new StreamReader(stream))
                    {
                        var catalog = JsonConvert.DeserializeObject<Catalog>(reader.ReadToEnd());
                        if (catalog == null || catalog.FormatVersion != 1 || catalog.DeviceTypes == null)
                            throw new JsonSerializationException("Unsupported device catalog format.");

                        var result = new Dictionary<string, Dictionary<int, string>>();
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
                            }

                            result.Add(category.Key, types);
                        }

                        return result;
                    }
                }
            }
            catch (JsonException ex)
            {
                Trace.TraceWarning("Unable to read device catalog: {0}", ex.Message);
            }
            catch (IOException ex)
            {
                Trace.TraceWarning("Unable to load device catalog: {0}", ex.Message);
            }

            return new Dictionary<string, Dictionary<int, string>>();
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
