using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDConfigurator
{
    public static class ConfigFactory
    {
        public static OSDConfiguration Create(IList<IOSDSetting> settings)
        {
            var screens = Enumerable.Range(1, 4)
                                    .Select(i => MakeScreen(i, settings))
                                    .Where(o => o != null)
                                    .ToArray();

            var options = settings.Where(o => o.Name.StartsWith("OSD_"))
                                  .ToArray();

            return new OSDConfiguration(options, screens);
        }
        
        private static OSDScreen MakeScreen(int screen, IEnumerable<IOSDSetting> source)
        {
            var settingPrefix = $"OSD{screen}_";

            var settings = source.Where(o => o.Name.StartsWith(settingPrefix)).ToList();

            if (!settings.Any())
                return null;

            var itemMap = new Dictionary<string, List<IOSDSetting>>();

            // Search by primary settings
            foreach (var setting in settings)
            {
                if (SplitIntoThree(setting.Name, out var prefix, out var name, out var suffix))
                {
                    if (prefix.Equals(settingPrefix, StringComparison.OrdinalIgnoreCase)
                        && (suffix.Equals("_X", StringComparison.OrdinalIgnoreCase)
                            || suffix.Equals("_Y", StringComparison.OrdinalIgnoreCase)
                            || suffix.Equals("_EN", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!itemMap.ContainsKey(name))
                            itemMap.Add(name, new List<IOSDSetting>());

                        itemMap[name].Add(setting);
                    }
                }
            }
            
            // Remove incomplete
            foreach(var key in itemMap.Keys.ToArray())
            {
                if (itemMap[key].Count != 3)
                    itemMap.Remove(key);
            }

            // Remove settings of the found items from source list
            foreach (var list in itemMap.Values)
                foreach (var setting in list)
                    settings.Remove(setting);

            // Search for extra settings (Not implemented in AP at the moment)
            foreach (var setting in settings)
            {
                if (SplitIntoThree(setting.Name, out var prefix, out var name, out var suffix)
                    && itemMap.ContainsKey(name))
                {
                    itemMap[name].Add(setting);
                }
            }

            // Remove settings of the found items from source list (once again after extra setting search)
            foreach (var list in itemMap.Values)
                foreach (var setting in list)
                    settings.Remove(setting);
            
            return new OSDScreen($"Screen {screen}", settings,
                itemMap.Select(o => new OSDItem(name: o.Key, options: o.Value)).ToArray());
        }
        
        private static bool SplitIntoThree(string settingName, out string prefix, out string name, out string suffix)
        {
            prefix = name = suffix = null;

            var parts = settingName.Split('_');

            if (parts.Length < 3)
                return false;

            prefix = string.Concat(parts.First(), "_");
            suffix = string.Concat("_", parts.Last());

            name = string.Join("_", parts.Skip(1).Take(parts.Length - 2));

            return true;
        }
    }
}
