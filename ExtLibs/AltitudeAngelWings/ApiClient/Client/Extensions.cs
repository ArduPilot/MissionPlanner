using AltitudeAngelWings.ApiClient.Models;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class Extensions
    {
        public static IEnumerable<string> GetFiltersForFeature(this Feature aaFeature)
        {
            if (!aaFeature.Properties.ContainsKey("filters")) { yield return null; }
            else
            {
                var filters = JsonConvert.DeserializeObject<List<JObject>>(aaFeature.Properties["filters"].ToString());
                foreach (var f in filters)
                {
                    yield return f.SelectToken("name").Value<string>();
                }
            }
        }

        public static List<string> FiltersSeen = new List<string>();

        public static IEnumerable<string> GetFilters(this AAFeatureCollection mapData)
        {
            var availableFilters = mapData.Features.SelectMany(x => GetFiltersForFeature(x)).Where(x => x != null).Distinct();

            FiltersSeen.AddRange(availableFilters);

            FiltersSeen = FiltersSeen.Distinct().ToList();

            return availableFilters;
        }

        public static bool IsFilterOutItem(this Feature aaFeature, IEnumerable<string> filterout)
        {
            var featureFilters = aaFeature.GetFiltersForFeature();

            int matchs = 0;

            foreach (var item in featureFilters)
            {
                if (filterout.Contains(item))
                    matchs++;
            }

            if (matchs == featureFilters.Count())
            {
                return false;
            }

            return true;
        }

        public static bool IsEnabledByDefault(this Feature aaFeature)
        {
            var filters = JsonConvert.DeserializeObject<List<JObject>>(aaFeature.Properties["filters"].ToString());
            foreach (var f in filters)
            {
                var isActive = f.SelectToken("active").Value<bool>();
                var isShowHideFilter = f.SelectToken("property").Value<string>() == "show";
                if (!isActive && isShowHideFilter)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
