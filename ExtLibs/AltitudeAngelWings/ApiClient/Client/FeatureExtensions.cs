using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class FeatureExtensions
    {
        private static readonly char[] DefaultWhitespaceChars = { ' ', '\r', '\n', '\t' };

        public static FeatureProperties GetFeatureProperties(this Feature feature)
            => JsonConvert.DeserializeObject<FeatureProperties>(new JObject(feature.Properties.Select(p => new JProperty(p.Key, p.Value))).ToString());

        public static IEnumerable<FilterInfoDisplay> GetFilterInfo(this Feature feature)
        {
            if (!feature.Properties.ContainsKey("filters"))
            {
                yield break;
            }

            var filters = JsonConvert.DeserializeObject<List<FilterInfo>>(feature.Properties["filters"].ToString());
            var visible = filters.All(f => f.Active);
            foreach (var f in filters)
            {
                yield return new FilterInfoDisplay
                {
                    Name = f.Name,
                    Property = f.Property,
                    Active = f.Active,
                    ParentName = f.ParentName,
                    Visible = visible
                };
            }
        }

        public static void UpdateFilterInfo(this IEnumerable<Feature> features, IList<FilterInfoDisplay> filterInfoToUpdate, bool resetFilters = false)
        {
            var comparer = new FilterInfoDisplayEqualityComparer();
            foreach (var change in features
                .SelectMany(GetFilterInfo)
                .Distinct(comparer)
                .Select(f => new
                {
                    Update = filterInfoToUpdate.Intersect(new [] { f }, comparer).FirstOrDefault(),
                    Item = f
                }))
            {
                if (change.Update == null)
                {
                    filterInfoToUpdate.Add(change.Item);
                    if (change.Item.Name == "Airspace")
                    {
                        change.Item.Visible = false;
                    }
                }
                else
                {
                    change.Update.Name = change.Item.Name;
                    change.Update.ParentName = change.Item.ParentName;
                    change.Update.Active = change.Item.Active;
                    change.Update.Property = change.Item.Property;
                    if (resetFilters)
                    {
                        change.Update.Visible = change.Item.Name != "Airspace" && change.Item.Visible;
                    }
                }
            }
        }

        public static DisplayInfo GetDisplayInfo(this Feature feature)
            => JsonConvert.DeserializeObject<DisplayInfo>(feature.Properties["display"].ToString());
    }
}
