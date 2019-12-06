using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngelWings.Extra;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Maps;
using Feature = GeoJSON.Net.Feature.Feature;
using Unit = System.Reactive.Unit;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal class MapAdapter : IMap, IDisposable
    {
        public MapAdapter(GMapControl mapControl)
        {
            _context = new WindowsFormsSynchronizationContext();

            _mapControl = mapControl;

            IObservable<Unit> positionChanged = Observable
                .FromEvent<PositionChanged, PointLatLng>(
                    action => point => action(point),
                    handler => _mapControl.OnPositionChanged += handler,
                    handler => _mapControl.OnPositionChanged -= handler)
                .Select(i => Unit.Default);


            IObservable<Unit> mapZoom = Observable
                .FromEvent<MapZoomChanged, Unit>(
                    action => () => action(Unit.Default),
                    handler => _mapControl.OnMapZoomChanged += handler,
                    handler => _mapControl.OnMapZoomChanged -= handler);


            MapChanged = positionChanged
                .Merge(mapZoom)
                .ObserveOn(ThreadPoolScheduler.Instance);

            mapControl.OnMapDrag += MapControl_OnMapDrag;
            //mapControl.OnPolygonClick += Control_OnPolygonClick;

            mapControl.OnPolygonEnter += MapControl_OnPolygonEnter;
            mapControl.OnPolygonLeave += MapControl_OnPolygonLeave;

            //mapControl.OnRouteClick += MapControl_OnRouteClick;
            mapControl.OnRouteEnter += MapControl_OnRouteEnter;
            mapControl.OnRouteLeave += MapControl_OnRouteLeave;
        }

        private void MapControl_OnPolygonLeave(GMapPolygon item)
        {
            item.Overlay.Markers.Remove(marker);
            marker = null;
        }

        private void MapControl_OnPolygonEnter(GMapPolygon item)
        {
            item.Overlay.Markers.Clear();

            if (marker != null)
                item.Overlay.Markers.Remove(marker);

            var point = item.Overlay.Control.PointToClient(Control.MousePosition);
            var pos = item.Overlay.Control.FromLocalToLatLng(point.X, point.Y);

            marker = new GMapMarkerRect(pos)
            {
                ToolTipMode = MarkerTooltipMode.Always,
                ToolTipText = createMessage(item.Tag),
                IsHitTestVisible = false
            };
            item.Overlay.Markers.Add(marker);
        }

        GMapMarkerRect marker = null;

        private void MapControl_OnRouteLeave(GMapRoute item)
        {
            item.Overlay.Markers.Remove(marker);
            marker = null;
        }

        private void MapControl_OnRouteEnter(GMapRoute item)
        {
            if (marker != null)
                item.Overlay.Markers.Remove(marker);

            var point = item.Overlay.Control.PointToClient(Control.MousePosition);
            var pos = item.Overlay.Control.FromLocalToLatLng(point.X, point.Y);

            marker = new GMapMarkerRect(pos)
            {
                ToolTipMode = MarkerTooltipMode.Always,
                ToolTipText = createMessage(item.Tag),
                IsHitTestVisible = false
            };
            item.Overlay.Markers.Add(marker);
        }

        private void MapControl_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            CustomMessageBox.Show(createMessage(item.Tag), "Info", MessageBoxButtons.OK);
        }

        string createMessage(object item)
        {
            if (item is Feature)
            {
                var prop = ((Feature) item).Properties;

                var display = prop["display"] as Newtonsoft.Json.Linq.JObject;

                var sections = display["sections"];

                string title;
                string text;

                if (sections.Count() == 0)
                {
                    title = prop["detailedCategory"].ToString();
                    text = "";
                }
                else
                {
                    var section1 = sections.Last();

                    var iconURL = section1["iconUrl"].ToString();
                    title = display["category"].ToString();
                    text = section1["text"].ToString();
                }

                var st = String.Format("{0} is categorised as a {1}\n\n{2}", display["title"], title, text);

                return st;
            }

            return "";
        }

        DateTime lastmapdrag = DateTime.MinValue;

        private void MapControl_OnMapDrag()
        {
            lastmapdrag = DateTime.Now;
        }

        private void Control_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            if (item.Overlay.Control.IsDragging)
                return;

            if (_mapControl.Overlays.First(x => x.Polygons.Any(i => i.Name == item.Name)) != null)
            {
                if (item.Tag is Feature)
                {
                    var st = createMessage(item.Tag);

                    CustomMessageBox.Show(st, "Info", MessageBoxButtons.OK);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public PointLatLng GetCenter()
        {
            PointLatLng pointLatLng = default(PointLatLng);

            try
            {
                _context.Send(_ => pointLatLng = _mapControl.Position, null);
            }
            catch
            {
            }

            return pointLatLng;
        }

        public RectLatLng GetViewArea()
        {
            RectLatLng rectLatLng = default(RectLatLng);
            try
            {
                _context.Send(_ => rectLatLng = _mapControl.ViewArea, null);
            }
            catch
            {
            }
            if (rectLatLng.WidthLng < 0.03)
                rectLatLng.Inflate(0, (0.03 - rectLatLng.WidthLng) / 2);
            if (rectLatLng.HeightLat < 0.03)
                rectLatLng.Inflate((0.03 - rectLatLng.HeightLat) / 2, 0);

            return rectLatLng;
        }

        public void AddOverlay(string name)
        {
            _context.Send(state =>
            {
                _mapControl.Overlays.Add(new GMapOverlay(name));
            }, null);
        }

        public void DeleteOverlay(string name)
        {
            _context.Send(_ =>
            {
                GMapOverlay overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);

                if (overlay != null)
                {
                    _mapControl.Overlays.Remove(overlay);
                    overlay.Dispose();
                }
            }, null);
        }

        public IOverlay GetOverlay(string name, bool createIfNotExists = false)
        {
            IOverlay result = null;
            try
            {
                _context.Send(_ =>
                {
                    GMapOverlay overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);

                    if (overlay == null)
                    {
                        if (createIfNotExists)
                        {
                            AddOverlay(name);
                            result = GetOverlay(name);
                            return;
                        }

                        throw new ArgumentException($"Overlay {name} not found.");
                    }

                    result = new OverlayAdapter(overlay);
                }, null);
            }
            catch
            {
            }

            return result;
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
                _disposer = null;
            }
        }

        private readonly GMapControl _mapControl;

        private CompositeDisposable _disposer = new CompositeDisposable();
        private readonly SynchronizationContext _context;
        public IObservable<Unit> MapChanged { get; }

        public void Invalidate()
        {
            _mapControl.Invalidate();
        }
    }
}