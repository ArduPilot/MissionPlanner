using AltitudeAngelWings.Extra;
using GeoJSON.Net.Feature;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Models;

namespace MissionPlanner.Utilities.AltitudeAngel
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

        public bool IsVisible
        {
            get
            {
                var value = false;
                _context.Send(state =>
                {
                    value = _overlay.IsVisibile;
                }, null);
                return value;
            }
            set
            {
                _context.Send(state =>
                {
                    _overlay.IsVisibile = value;
                }, null);
            }
        }

        public void AddOrUpdatePolygon(string name, List<LatLong> points, ColorInfo colorInfo, Feature featureInfo)
        {
            _context.Send(_ =>
            {
                var polygon = _overlay.Polygons.FirstOrDefault(p => p.Name == name);
                if (polygon == null)
                {
                    polygon = new GMapPolygon(points.ConvertAll(p => new PointLatLng(p.Latitude, p.Longitude)), name);
                    _overlay.Polygons.Add(polygon);
                }
                polygon.Fill = new SolidBrush(Color.FromArgb((int)colorInfo.FillColor));
                polygon.Stroke = new Pen(Color.FromArgb((int)colorInfo.StrokeColor), colorInfo.StrokeWidth);
                polygon.IsHitTestVisible = true;
                polygon.Tag = featureInfo;
            }, null);
        }

        public void RemovePolygonsExcept(List<string> names)
        {
            _context.Send(_ =>
            {
                var remove = _overlay.Polygons
                    .Where(p => !names.Contains(p.Name))
                    .ToArray();
                foreach (var polygon in remove)
                {
                    _overlay.Polygons.Remove(polygon);
                }
            }, null);
        }

        public bool PolygonExists(string name)
        {
            var polygonExists = false;
            _context.Send(_ => polygonExists = _overlay.Polygons.Any(i => i.Name == name), null);
            return polygonExists;
        }

        public void AddOrUpdateLine(string name, List<LatLong> points, ColorInfo colorInfo, Feature featureInfo)
        {
            _context.Send(_ =>
            {
                var route = _overlay.Routes.FirstOrDefault(r => r.Name == name);
                if (route == null)
                {
                    route = new GMapRoute(points.ConvertAll(p => new PointLatLng(p.Latitude, p.Longitude)), name);
                    _overlay.Routes.Add(route);
                }
                route.Stroke = new Pen(Color.FromArgb((int)colorInfo.StrokeColor), colorInfo.StrokeWidth + 2);
                route.IsHitTestVisible = true;
                route.Tag = featureInfo;
            }, null);
        }

        public void RemoveLinesExcept(List<string> names)
        {
            _context.Send(_ =>
            {
                var remove = _overlay.Routes
                    .Where(p => !names.Contains(p.Name))
                    .ToArray();
                foreach (var route in remove)
                {
                    _overlay.Routes.Remove(route);
                }
            }, null);
        }

        public bool LineExists(string name)
        {
            var exists = false;
            _context.Send(_ => exists = _overlay.Routes.Any(i => i.Name == name), null);
            return exists;
        }
    }
}