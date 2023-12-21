using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace AltitudeAngelWings.Plugin
{
    internal class OverlayAdapter : IOverlay
    {
        private readonly GMapOverlay _overlay;
        private readonly SynchronizationContext _context;

        public OverlayAdapter(GMapOverlay overlay)
        {
            _context = new WindowsFormsSynchronizationContext();
            _overlay = overlay;
        }

        public void SetFeatures(IReadOnlyList<OverlayFeature> features)
        {
            _context.Send(_ =>
            {
                var existing = _overlay.Polygons.Union(_overlay.Routes.Cast<MapRoute>()).ToDictionary(i => i.Name, i => i);
                var index = features.ToDictionary(f => f.Name, f => f);

                // Remove polygons and routes not in features
                foreach (var remove in existing.Keys.Except(index.Keys))
                {
                    var item = existing[remove];
                    switch (item)
                    {
                        case GMapPolygon polygon:
                            _overlay.Polygons.Remove(polygon);
                            break;
                        case GMapRoute route:
                            _overlay.Routes.Remove(route);
                            break;
                    }
                }

                // Update polygons and routes already in features and remove from index as updated
                foreach (var update in existing.Keys.Intersect(index.Keys))
                {
                    var item = existing[update];
                    var feature = index[update];
                    switch (item)
                    {
                        case GMapPolygon polygon:
                            polygon.Points.Clear();
                            polygon.Points.AddRange(feature.Points.Select(p => new PointLatLng(p.Latitude, p.Longitude)));
                            SetPolygon(polygon, feature);
                            break;
                        case GMapRoute route:
                            route.Points.Clear();
                            route.Points.AddRange(feature.Points.Select(p => new PointLatLng(p.Latitude, p.Longitude)));
                            SetRoute(route, feature);
                            break;
                    }

                    index.Remove(update);
                }

                // Add polygons and routes that are left in the index
                foreach (var item in index.Values)
                {
                    switch (item.Type)
                    {
                        case OverlayFeatureType.Polygon:
                            var polygon = new GMapPolygon(
                                    item.Points.Select(p => new PointLatLng(p.Latitude, p.Longitude)).ToList(),
                                    item.Name);
                            SetPolygon(polygon, item);
                            _overlay.Polygons.Add(polygon);
                            break;
                        case OverlayFeatureType.Line:
                            var route = new GMapRoute(
                                item.Points.Select(p => new PointLatLng(p.Latitude, p.Longitude)).ToList(),
                                item.Name);
                            SetRoute(route, item);
                            _overlay.Routes.Add(route);
                            break;
                    }
                }
            }, null);
        }

        private static void SetRoute(GMapRoute route, OverlayFeature feature)
        {
            route.Stroke = new Pen(Color.FromArgb((int)feature.ColorInfo.StrokeColor), feature.ColorInfo.StrokeWidth);
            route.IsHitTestVisible = true;
            route.Tag = feature.FeatureInfo;
        }

        private static void SetPolygon(GMapPolygon polygon, OverlayFeature feature)
        {
            polygon.Fill = new SolidBrush(Color.FromArgb((int)feature.ColorInfo.FillColor));
            polygon.Stroke = new Pen(Color.FromArgb((int)feature.ColorInfo.StrokeColor), feature.ColorInfo.StrokeWidth);
            polygon.IsHitTestVisible = true;
            polygon.Tag = feature.FeatureInfo;
        }
    }
}