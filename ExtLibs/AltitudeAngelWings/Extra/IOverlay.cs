using System.Collections.Generic;
using GeoJSON.Net.Feature;
using GMap.NET;

namespace AltitudeAngelWings.Extra
{
    public interface IOverlay
    {
        void AddOrUpdatePolygon(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo);
        void AddOrUpdateLine(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo);
        bool LineExists(string name);
        bool PolygonExists(string name);
        bool IsVisible { get; set; }
        void RemovePolygonsExcept(List<string> names);
        void RemoveLinesExcept(List<string> names);
    }
}
