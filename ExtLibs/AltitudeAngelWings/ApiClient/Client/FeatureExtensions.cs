using System;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                }
                else
                {
                    change.Update.Name = change.Item.Name;
                    change.Update.ParentName = change.Item.ParentName;
                    change.Update.Active = change.Item.Active;
                    change.Update.Property = change.Item.Property;
                    if (resetFilters)
                    {
                        change.Update.Visible = change.Item.Visible;
                    }
                }
            }
        }

        public static DisplayInfo GetDisplayInfo(this Feature feature)
            => JsonConvert.DeserializeObject<DisplayInfo>(feature.Properties["display"].ToString());

        public static string GetMapInfoMessage(this DisplayInfo displayInfo, int wrapPoint = 60, ICollection<char> whitespaceChars = null)
        {
            if (whitespaceChars == null)
            {
                whitespaceChars = DefaultWhitespaceChars;
            }

            var builder = new StringBuilder();
            builder.AppendLine(displayInfo.DetailedCategory);
            builder.AppendLine(displayInfo.Title);
            builder.AppendLine();
            foreach (var section in displayInfo.Sections)
            {
                builder.AppendLine(section.DisplayTitle);
                builder.AppendLine(section.Text);
                builder.AppendLine();
            }

            WrapText(builder, wrapPoint, whitespaceChars);
            return builder.ToString().Trim(whitespaceChars.ToArray());
        }

        private static void WrapText(StringBuilder builder, int wrap, ICollection<char> whitespaceChars)
        {
            var pos = wrap;
            while (pos < builder.Length)
            {
                var closest = FindClosest(builder, pos, whitespaceChars);
                if (closest < 0)
                {
                    break;
                }

                builder.Insert(closest, Environment.NewLine);
                closest += Environment.NewLine.Length;
                builder.Remove(closest, 1);
                pos = closest + wrap;
            }
        }

        private static int FindClosest(StringBuilder builder, int pos, ICollection<char> options)
        {
            if (options.Contains(builder[pos]))
            {
                return pos;
            }

            var offset = 1;
            while (pos - offset > 0 && pos + offset < builder.Length)
            {
                if (options.Contains(builder[pos - offset]))
                {
                    return pos - offset;
                }
                if (options.Contains(builder[pos + offset]))
                {
                    return pos + offset;
                }

                offset++;
            }

            return -1;
        }
    }
}
