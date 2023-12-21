using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AltitudeAngelWings.Clients.Api.Model;
using AltitudeAngelWings.Plugin.Properties;
using AltitudeAngelWings.Service.Messaging;
using GMap.NET;
using GMap.NET.WindowsForms;
using Feature = GeoJSON.Net.Feature.Feature;
using Message = AltitudeAngelWings.Model.Message;
using Unit = System.Reactive.Unit;

namespace AltitudeAngelWings.Plugin
{
    internal class MapAdapter : IMap, IDisposable
    {
        private static readonly TimeSpan CtrlClickMessageInterval = TimeSpan.FromSeconds(30);

        private readonly GMapControl _mapControl;
        private readonly Func<bool> _enabled;
        private readonly IMapInfoDockPanel _mapInfoDockPanel;
        private readonly IMessagesService _messages;
        private readonly bool _ctrlForPanel;
        private CompositeDisposable _disposer = new CompositeDisposable();
        private readonly SynchronizationContext _context;
        private Point? _mouseDown;
        private DateTimeOffset _lastShownCtrlClickMessage = DateTimeOffset.MinValue;

        public IObservable<Unit> MapChanged { get; }
        public ObservableProperty<Feature> FeatureClicked { get; } = new ObservableProperty<Feature>(0);

        public MapAdapter(GMapControl mapControl, Func<bool> enabled, IMapInfoDockPanel mapInfoDockPanel, ISettings settings, IMessagesService messages, bool ctrlForPanel)
        {
            _context = new WindowsFormsSynchronizationContext();
            _mapControl = mapControl;
            _enabled = enabled;
            _mapInfoDockPanel = mapInfoDockPanel;
            _messages = messages;
            _ctrlForPanel = ctrlForPanel;

            var positionChanged = Observable
                .FromEvent<PositionChanged, PointLatLng>(
                    action => point => action(point),
                    handler => _mapControl.OnPositionChanged += handler,
                    handler => _mapControl.OnPositionChanged -= handler)
                .Select(i => Unit.Default);

            var mapZoom = Observable
                .FromEvent<MapZoomChanged, Unit>(
                    action => () => action(Unit.Default),
                    handler => _mapControl.OnMapZoomChanged += handler,
                    handler => _mapControl.OnMapZoomChanged -= handler);

            var visible = Observable
                .FromEvent<EventHandler, Unit>(
                    action => (s, e) => action(Unit.Default),
            handler => _mapControl.VisibleChanged += handler,
                    handler => _mapControl.VisibleChanged -= handler)
                .Where(i => _mapControl.Visible);

            var interval = Observable
                .Interval(TimeSpan.FromSeconds(settings.MapUpdateRefresh))
                .Select(_ => mapControl.Position)
                .Select(i => Unit.Default);

            MapChanged = visible
                .Merge(positionChanged)
                .Merge(interval)
                .Merge(mapZoom)
                .Throttle(TimeSpan.FromSeconds(settings.MapUpdateThrottle))
                .Where(i => _mapControl.Visible)
                .ObserveOn(ThreadPoolScheduler.Instance);

            mapControl.MouseDown += OnMouseDown;
            mapControl.MouseUp += OnMouseUp;
        }

        private void OnMouseDown(object s, MouseEventArgs e) => _mouseDown = e.Button == MouseButtons.Left ? e.Location : (Point?)null;

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (e.Location != _mouseDown)
            {
                _mouseDown = null;
                return;
            }

            if (_ctrlForPanel && Control.ModifierKeys == Keys.None)
            {
                if (DateTimeOffset.UtcNow.Subtract(_lastShownCtrlClickMessage) >= CtrlClickMessageInterval)
                {
                    _messages.AddMessageAsync(Message.ForInfo(Resources.MessageCtrlClickPlannerMap, CtrlClickMessageInterval));
                    _lastShownCtrlClickMessage = DateTimeOffset.UtcNow;
                }
                return;
            }

            var mapItems = GetMapItemsOnMouseClick(e.Location);
            if (mapItems.Length == 0)
            {
                _mapInfoDockPanel.Hide();
                return;
            }
            _mapInfoDockPanel.Show(mapItems.Select(i => i.GetFeatureProperties()).ToArray());
            foreach (var item in mapItems)
            {
                FeatureClicked.Value = item;
            }
        }

        public bool Enabled => _enabled();

        private Feature[] GetMapItemsOnMouseClick(Point point)
        {
            var mapItems = new List<Feature>();

            var polygons = _mapControl.Overlays
                .SelectMany(o => o.Polygons)
                .Where(p => p.IsVisible && p.IsHitTestVisible)
                .Where(p => p.IsInside(_mapControl.FromLocalToLatLng(point.X, point.Y)))
                .Select(p=> (Feature)p.Tag);
            mapItems.AddRange(polygons);

            var routes = _mapControl.Overlays
                .SelectMany(o => o.Routes)
                .Where(r => r.IsVisible && r.IsHitTestVisible)
                .Where(r =>
                {
                    var rp = new GPoint(point.X, point.Y);
                    rp.OffsetNegative(_mapControl.Core.renderOffset);
                    return r.IsInside((int)rp.X, (int)rp.Y);
                })
                .Select(r => (Feature)r.Tag);
            mapItems.AddRange(routes);

            return mapItems.ToArray();
        }

        public BoundingLatLong GetViewArea()
        {
            var rectLatLng = default(RectLatLng);
            _context.Send(_ => rectLatLng = _mapControl.ViewArea, null);
            if (rectLatLng.WidthLng < 0.03)
                rectLatLng.Inflate(0, (0.03 - rectLatLng.WidthLng) / 2);
            if (rectLatLng.HeightLat < 0.03)
                rectLatLng.Inflate((0.03 - rectLatLng.HeightLat) / 2, 0);

            return new BoundingLatLong
            {
                NorthEast = new LatLong(rectLatLng.Top, rectLatLng.Right),
                SouthWest = new LatLong(rectLatLng.Bottom, rectLatLng.Left)
            };
        }

        public void DeleteOverlay(string name)
            => _context.Send(_ =>
            {
                var overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);
                if (overlay == null) return;
                _mapControl.Overlays.Remove(overlay);
                overlay.Dispose();
            }, null);

        public IOverlay GetOverlay(string name, bool createIfNotExists = false)
        {
            IOverlay result = null;
            _context.Send(_ =>
            {
                var overlay = _mapControl.Overlays.FirstOrDefault(i => i.Id == name);

                if (overlay == null)
                {
                    if (!createIfNotExists) throw new ArgumentException($"Overlay {name} not found.");
                    overlay = new GMapOverlay(name);
                    _mapControl.Overlays.Add(overlay);
                }

                result = new OverlayAdapter(overlay);
            }, null);
            return result;
        }

        public void Invalidate()
        {
            _mapControl.Invalidate();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
                _disposer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}