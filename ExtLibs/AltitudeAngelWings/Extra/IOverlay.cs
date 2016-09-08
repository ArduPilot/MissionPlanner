using System;
using System.Collections.Generic;
using GMap.NET;

namespace AltitudeAngel.IsolatedPlugin.Common.Maps
{
    public interface IOverlay
    {
        void AddPolygon(string name, List<PointLatLng> points, ColorInfo colorInfo);
        void AddLine(string name, List<PointLatLng> points, ColorInfo colorInfo);
        bool LineExists(string name);
        bool PolygonExists(string name);
        bool IsVisible { get; set; }
    }
}
