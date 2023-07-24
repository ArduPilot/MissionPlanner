using System.Collections.Generic;
using AltitudeAngelWings.Clients.Api.Model;
using GeoJSON.Net.Feature;

namespace AltitudeAngelWings
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
