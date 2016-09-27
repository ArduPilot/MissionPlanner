using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using AltitudeAngelWings;
using GMap.NET;
using GMap.NET.WindowsForms;
using SharpKml.Dom;
using Feature = GeoJSON.Net.Feature.Feature;
using Timer = System.Windows.Forms.Timer;
using Unit = System.Reactive.Unit;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    class MapAdapter : IMap, IDisposable
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
            mapControl.OnPolygonClick += Control_OnPolygonClick;
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
                    var prop = ((Feature) item.Tag).Properties;

                    var st = String.Format("{0} is categorised as {1}", prop["name"], prop["detailedCategory"]);

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

            _context.Send(_ => pointLatLng = _mapControl.Position, null);

            return pointLatLng;
        }

        public RectLatLng GetViewArea()
        {
            RectLatLng rectLatLng = default(RectLatLng);

            _context.Send(_ => rectLatLng = _mapControl.ViewArea, null);

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
    }
}