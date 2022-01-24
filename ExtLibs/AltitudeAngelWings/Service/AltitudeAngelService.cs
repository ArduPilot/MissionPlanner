using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.Messaging;
using Flurl.Http;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace AltitudeAngelWings.Service
{
    public class AltitudeAngelService : IAltitudeAngelService
    {
        private const string MapOverlayName = "AAMapData";
        private static readonly Dictionary<string, Feature> MapFeatureCache = new Dictionary<string, Feature>();

        private readonly IMessagesService _messagesService;
        private readonly IMissionPlanner _missionPlanner;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly IAltitudeAngelClient _client;
        private readonly ISettings _settings;
        private readonly SemaphoreSlim _processLock = new SemaphoreSlim(1);

        public ObservableProperty<bool> IsSignedIn { get; }
        public ObservableProperty<WeatherInfo> WeatherReport { get; }
        public UserProfileInfo CurrentUser { get; private set; }
        public IList<FilterInfoDisplay> FilterInfoDisplay { get; }

        public AltitudeAngelService(
            IMessagesService messagesService,
            IMissionPlanner missionPlanner,
            ISettings settings,
            IAltitudeAngelClient client)
        {
            _messagesService = messagesService;
            _missionPlanner = missionPlanner;
            _settings = settings;
            _client = client;

            IsSignedIn = new ObservableProperty<bool>(false);
            _disposer.Add(IsSignedIn);
            WeatherReport = new ObservableProperty<WeatherInfo>();
            _disposer.Add(WeatherReport);
            FilterInfoDisplay = _settings.MapFilters;

            _disposer.Add(_missionPlanner.FlightDataMap
                .MapChanged
                .SubscribeWithAsync((i, ct) => UpdateMapData(_missionPlanner.FlightDataMap, ct)));
            _disposer.Add(_missionPlanner.FlightPlanningMap
                .MapChanged
                .SubscribeWithAsync((i, ct) => UpdateMapData(_missionPlanner.FlightPlanningMap, ct)));

            _disposer.Add(_missionPlanner.FlightDataMap
                .FeatureClicked
                .Select(f => new { Feature = f, Properties = f.GetFeatureProperties()})
                .Where(i => i.Properties.DetailedCategory == "user:flight_plan_report" && i.Properties.IsOwner)
                .SubscribeWithAsync((i, ct) => OnFlightReportClicked(i.Feature)));
            _disposer.Add(_missionPlanner.FlightPlanningMap
                .FeatureClicked
                .Select(f => new { Feature = f, Properties = f.GetFeatureProperties()})
                .Where(i => i.Properties.DetailedCategory == "user:flight_plan_report" && i.Properties.IsOwner)
                .SubscribeWithAsync((i, ct) => OnFlightReportClicked(i.Feature)));
        }

        public async Task SignInAsync()
        {
            try
            {
                // Load the user's profile, will trigger auth
                CurrentUser = await _client.GetUserProfile();
                IsSignedIn.Value = true;
                await UpdateMapData(_missionPlanner.FlightDataMap, CancellationToken.None);
                await UpdateMapData(_missionPlanner.FlightPlanningMap, CancellationToken.None);
            }
            catch (Exception)
            {
                await _messagesService.AddMessageAsync("There was a problem signing you in.");
            }
        }

        public Task DisconnectAsync()
        {
            _client.Disconnect(true);
            IsSignedIn.Value = false;
            ProcessAllFromCache(_missionPlanner.FlightDataMap);
            ProcessAllFromCache(_missionPlanner.FlightPlanningMap);
            return Task.CompletedTask;
        }

        private async Task UpdateMapData(IMap map, CancellationToken cancellationToken)
        {
            if (!IsSignedIn)
            {
                MapFeatureCache.Clear();
            }

            if (!map.Enabled)
            {
                map.DeleteOverlay(MapOverlayName);
                map.Invalidate();
                return;
            }

            try
            {
                var area = map.GetViewArea().RoundExpand(4);
                var sw = new Stopwatch();
                sw.Start();
                var mapData = await _client.GetMapData(area, cancellationToken);
                sw.Stop();

                mapData.Features.UpdateFilterInfo(FilterInfoDisplay);
                _settings.MapFilters = FilterInfoDisplay;

                await _messagesService.AddMessageAsync(
                    $"Map area loaded {area.NorthEast.Latitude:F4}, {area.SouthWest.Latitude:F4}, {area.SouthWest.Longitude:F4}, {area.NorthEast.Longitude:F4} in {sw.Elapsed.TotalMilliseconds:N2}ms");

                // add all items to cache
                MapFeatureCache.Clear();
                mapData.Features.ForEach(feature => MapFeatureCache[feature.Id] = feature);

                // Only get the features that are enabled by default, and have not been filtered out
                ProcessFeatures(map, mapData.Features);
            }
            catch (Exception ex) when (!(ex is FlurlHttpException) && !(ex.InnerException is TaskCanceledException))
            {
                await _messagesService.AddMessageAsync("Failed to update map data.");
            }
        }

        public void ProcessAllFromCache(IMap map, bool resetFilters = false)
        {
            map.DeleteOverlay(MapOverlayName);
            if (!IsSignedIn)
            {
                MapFeatureCache.Clear();
            }

            if (!map.Enabled)
            {
                map.DeleteOverlay(MapOverlayName);
                map.Invalidate();
                return;
            }

            if (resetFilters)
            {
                MapFeatureCache.Values.UpdateFilterInfo(FilterInfoDisplay, resetFilters);
            }

            ProcessFeatures(map, MapFeatureCache.Values);
            map.Invalidate();
        }

        private void ProcessFeatures(IMap map, IEnumerable<Feature> features)
        {
            try
            {
                if (!_processLock.Wait(TimeSpan.FromSeconds(1))) return;
                var overlay = map.GetOverlay(MapOverlayName, true);
                var polygons = new List<string>();
                var lines = new List<string>();

                foreach (var feature in features)
                {
                    if (!FilterInfoDisplay
                            .Intersect(feature.GetFilterInfo(), new FilterInfoDisplayEqualityComparer())
                            .Any(i => i.Visible))
                    {
                        continue;
                    }

                    var properties = feature.GetFeatureProperties();

                    switch (feature.Geometry.Type)
                    {
                        case GeoJSONObjectType.Point:
                        {
                            var pnt = (Point)feature.Geometry;

                            var coordinates = new List<LatLong>();

                            if (!string.IsNullOrEmpty(properties.Radius))
                            {
                                var rad = double.Parse(properties.Radius);
                                for (var i = 0; i <= 360; i += 10)
                                {
                                    coordinates.Add(
                                        PositionFromBearingAndDistance(new LatLong(((Position)pnt.Coordinates).Latitude,
                                            ((Position)pnt.Coordinates).Longitude), i, rad));
                                }
                            }

                            var colorInfo = properties.ToColorInfo(_settings.MapOpacityAdjust);
                            overlay.AddOrUpdatePolygon(feature.Id, coordinates, colorInfo, feature);
                            polygons.Add(feature.Id);
                        }
                            break;
                        case GeoJSONObjectType.MultiPoint:
                            break;
                        case GeoJSONObjectType.LineString:
                        {
                            var line = (LineString)feature.Geometry;
                            var coordinates = line.Coordinates.OfType<Position>()
                                .Select(c => new LatLong(c.Latitude, c.Longitude))
                                .ToList();
                            var colorInfo = properties.ToColorInfo(_settings.MapOpacityAdjust);
                            overlay.AddOrUpdateLine(feature.Id, coordinates, colorInfo, feature);
                            lines.Add(feature.Id);
                        }
                            break;

                        case GeoJSONObjectType.MultiLineString:
                            break;
                        case GeoJSONObjectType.Polygon:
                        {
                            var poly = (Polygon)feature.Geometry;
                            var coordinates =
                                poly.Coordinates[0].Coordinates.OfType<Position>()
                                    .Select(c => new LatLong(c.Latitude, c.Longitude))
                                    .ToList();

                            var colorInfo = properties.ToColorInfo(_settings.MapOpacityAdjust);
                            overlay.AddOrUpdatePolygon(feature.Id, coordinates, colorInfo, feature);
                            polygons.Add(feature.Id);
                        }
                            break;
                        case GeoJSONObjectType.MultiPolygon:
                            foreach (var poly in ((MultiPolygon)feature.Geometry).Coordinates)
                            {
                                var coordinates =
                                    poly.Coordinates[0].Coordinates.OfType<Position>()
                                        .Select(c => new LatLong(c.Latitude, c.Longitude))
                                        .ToList();

                                var colorInfo = properties.ToColorInfo(_settings.MapOpacityAdjust);
                                overlay.AddOrUpdatePolygon(feature.Id, coordinates, colorInfo, feature);
                                polygons.Add(feature.Id);
                            }

                            break;
                        case GeoJSONObjectType.GeometryCollection:
                            break;
                        case GeoJSONObjectType.Feature:
                            break;
                        case GeoJSONObjectType.FeatureCollection:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                overlay.RemovePolygonsExcept(polygons);
                overlay.RemoveLinesExcept(lines);
            }
            finally
            {
                _processLock.Release();
            }
        }

        private static LatLong PositionFromBearingAndDistance(LatLong input, double bearing, double distance)
        {
            const double rad2deg = 180 / Math.PI;
            const double deg2rad = 1.0 / rad2deg;

            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            var radiusOfEarth = 6378100.0;//# in meters

            var lat1 = deg2rad * input.Latitude;
            var lon1 = deg2rad * input.Longitude;
            var brng = deg2rad * bearing;
            var dr = distance / radiusOfEarth;

            var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dr) +
                                 Math.Cos(lat1) * Math.Sin(dr) * Math.Cos(brng));
            var lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dr) * Math.Cos(lat1),
                                Math.Cos(dr) - Math.Sin(lat1) * Math.Sin(lat2));

            return new LatLong(rad2deg * lat2, rad2deg * lon2);
        }

        private async Task OnFlightReportClicked(Feature feature)
        {
            if (!await _missionPlanner.ShowYesNoMessageBox(
                $"You have clicked your flight plan '{feature.GetDisplayInfo().Title}'.{Environment.NewLine}Would you like to set the current flight plan to this one?",
                "Flight Plan")) return;
            _settings.ExistingFlightPlanId = Guid.Parse(feature.Id);
            _settings.UseExistingFlightPlanId = true;
            await _messagesService.AddMessageAsync(new Message($"Current flight plan ID set to {feature.Id}")
                { TimeToLive = TimeSpan.FromSeconds(10) });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
            }
        }
    }
}
