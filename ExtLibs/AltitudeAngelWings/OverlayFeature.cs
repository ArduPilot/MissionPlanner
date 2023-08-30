using System.Collections.Generic;
using AltitudeAngelWings.Clients.Api.Model;
using GeoJSON.Net.Feature;

namespace AltitudeAngelWings
{
    public class OverlayFeature
    {
        public string Name { get; }
        public OverlayFeatureType Type { get; }
        public IReadOnlyList<LatLong> Points { get; }
        public ColorInfo ColorInfo { get; }
        public Feature FeatureInfo { get; }

        public OverlayFeature(string name, OverlayFeatureType type, IReadOnlyList<LatLong> points, ColorInfo colorInfo, Feature featureInfo)
        {
            Name = name;
            Type = type;
            Points = points;
            ColorInfo = colorInfo;
            FeatureInfo = featureInfo;
        }
    }
}