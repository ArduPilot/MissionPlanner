using System.Collections.Generic;
using AltitudeAngelWings.ApiClient.Models;
using GeoJSON.Net.Feature;

namespace AltitudeAngelWings.Extra
{
    public interface IOverlay
    {
        void AddOrUpdatePolygon(string name, List<LatLong> points, ColorInfo colorInfo, Feature featureInfo);
        void AddOrUpdateLine(string name, List<LatLong> points, ColorInfo colorInfo, Feature featureInfo);
        bool LineExists(string name);
        bool PolygonExists(string name);
        bool IsVisible { get; set; }
        void RemovePolygonsExcept(List<string> names);
        void RemoveLinesExcept(List<string> names);
    }
}
