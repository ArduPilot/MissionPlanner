using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngelWings.Extra;
using GeoJSON.Net.Feature;
using GMap.NET;
using GMap.NET.WindowsForms;

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
                bool value = false;
                try
                {
                    _context.Send(state =>
                    {
                        value = _overlay.IsVisibile;
                    }, null);
                }
                catch (InvalidAsynchronousStateException)
                {

                }
                return value;
            }
            set
            {
                try
                {
                    _context.Send(state =>
                    {
                        _overlay.IsVisibile = value;
                    }, null);
                }
                catch (InvalidAsynchronousStateException)
                {

                }
            }
        }

        public void AddOrUpdatePolygon(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo)
        {
            try
            {
                _context.Send(_ =>
                {
                    var polygon = _overlay.Polygons.FirstOrDefault(p => p.Name == name);
                    if (polygon == null)
                    {
                        polygon = new GMapPolygon(points, name);
                        _overlay.Polygons.Add(polygon);
                    }
                    polygon.Fill = new SolidBrush(Color.FromArgb((int) colorInfo.FillColor));
                    polygon.Stroke = new Pen(Color.FromArgb((int) colorInfo.StrokeColor), colorInfo.StrokeWidth);
                    polygon.IsHitTestVisible = true;
                    polygon.Tag = featureInfo;
               }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public void RemovePolygonsExcept(List<string> names)
        {
            try
            {
                _context.Send(_ =>
                {
                    for (var pos = 0; pos < _overlay.Polygons.Count; pos++)
                    {
                        var polygon = _overlay.Polygons[pos];
                        if (!names.Contains(polygon.Name))
                        {
                            _overlay.Polygons.Remove(polygon);
                        }
                    }
                }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public bool PolygonExists(string name)
        {
            bool polygonExists = false;

            try
            {
                _context.Send(_ => polygonExists = _overlay.Polygons.Any(i => i.Name == name), null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }

            return polygonExists;
        }

        public void AddOrUpdateLine(string name, List<PointLatLng> points, ColorInfo colorInfo, Feature featureInfo)
        {
            try
            {
                _context.Send(_ =>
                {
                    var route = _overlay.Routes.FirstOrDefault(r => r.Name == name);
                    if (route == null)
                    {
                        route = new GMapRoute(points, name);
                        _overlay.Routes.Add(route);
                    }
                    route.Stroke = new Pen(Color.FromArgb((int) colorInfo.StrokeColor), colorInfo.StrokeWidth + 2);
                    route.IsHitTestVisible = true;
                    route.Tag = featureInfo;
                }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public void RemoveLinesExcept(List<string> names)
        {
            try
            {
                _context.Send(_ =>
                {
                    for (var pos = 0; pos < _overlay.Routes.Count; pos++)
                    {
                        var route = _overlay.Routes[pos];
                        if (!names.Contains(route.Name))
                        {
                            _overlay.Routes.Remove(route);
                        }
                    }
                }, null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
        }

        public bool LineExists(string name)
        {
            bool exists = false;
            try
            {
                _context.Send(_ => exists = _overlay.Routes.Any(i => i.Name == name), null);
            }
            catch (InvalidAsynchronousStateException)
            {

            }
            return exists;
        }
    }
}