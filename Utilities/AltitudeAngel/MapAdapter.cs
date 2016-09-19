using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using GMap.NET;
using GMap.NET.WindowsForms;
using Timer = System.Windows.Forms.Timer;

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