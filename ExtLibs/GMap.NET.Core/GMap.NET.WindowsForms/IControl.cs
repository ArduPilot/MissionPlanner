using System;
using System.Drawing;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using GMap.NET.ObjectModel;

namespace GMap.NET.WindowsForms
{
    public interface IControl
    {
        void UpdateRouteLocalPosition(GMapRoute obj);
        void UpdatePolygonLocalPosition(GMapPolygon gMapPolygon);
        void UpdateMarkerLocalPosition(GMapMarker gMapMarker);
        RectLatLng ViewArea { get; }
        bool PolygonsEnabled { get; set; }
        bool RoutesEnabled { get; set; }
        PointLatLng Position { get; set; }
        bool MarkersEnabled { get; set; }
        bool IsMouseOverMarker { get; set; }
        bool HoldInvalidation { get; set; }
        Internals.Core Core { get; }
        bool IsMouseOverPolygon { get; set; }
        bool IsMouseOverRoute { get; set; }
        int Width { get; set; }
        GMapProvider MapProvider { get; set; }
        int Height { get; set; }
        double Zoom { get; set; }
        float Bearing { get; set; }
        bool IsDragging { get; }
        ObservableCollectionThreadSafe<GMapOverlay> Overlays { get; }
        void RestoreCursorOnLeave();
        void Refresh();
        void Invalidate();
        GPoint FromLatLngToLocal(PointLatLng rectLocationTopLeft);
        PointLatLng FromLocalToLatLng(int p0, int p1);
        Point PointToClient(Point mousePosition);
        object Invoke(Delegate p0);
    }
}