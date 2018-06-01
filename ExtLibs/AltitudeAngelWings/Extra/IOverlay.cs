using System;
using System.Collections.Generic;
using GeoJSON.Net.Feature;
using GMap.NET;

namespace AltitudeAngel.IsolatedPlugin.Common.Maps
{
    public interface IOverlay
    {
        void AddPolygon(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo);
        void AddLine(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo);
        bool LineExists(string name);
        bool PolygonExists(string name);
        bool IsVisible { get; set; }
    }
}
