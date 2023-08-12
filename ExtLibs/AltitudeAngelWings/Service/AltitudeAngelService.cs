using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Api;
using AltitudeAngelWings.Clients.Api.Model;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry;
using AltitudeAngelWings.Service.FlightService;
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
        private readonly IApiClient _apiClient;
        private readonly ITelemetryService _telemetryService;
        private readonly IFlightService _flightService;
        private readonly ISettings _settings;
        private readonly ITokenProvider _tokenProvider;
        private readonly SemaphoreSlim _signInLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _processLock = new SemaphoreSlim(1);

        public ObservableProperty<bool> IsSignedIn { get; }
        public ObservableProperty<WeatherInfo> WeatherReport { get; }
        public IList<FilterInfoDisplay> FilterInfoDisplay { get; }

        public bool SigningIn => _signInLock.CurrentCount == 0;

        public AltitudeAngelService(
            IMessagesService messagesService,
            IMissionPlanner missionPlanner,
            ISettings settings,
            ITokenProvider tokenProvider,
            IApiClient apiClient,
            ITelemetryService telemetryService,
            IFlightService flightService)
        {
            _messagesService = messagesService;
            _missionPlanner = missionPlanner;
            _settings = settings;
            _tokenProvider = tokenProvider;
            _apiClient = apiClient;
            _telemetryService = telemetryService;
            _flightService = flightService;

            IsSignedIn = new ObservableProperty<bool>(_settings.TokenResponse.IsValidForAuth());
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
                .Select(f => new { Feature = f, Properties = f.GetFeatureProperties() })
                .Where(i => i.Properties.DetailedCategory == "user:flight_plan_report" && i.Properties.IsOwner)
                .SubscribeWithAsync((i, ct) => OnFlightReportClicked(i.Feature)));
            _disposer.Add(_missionPlanner.FlightPlanningMap
                .FeatureClicked
                .Select(f => new { Feature = f, Properties = f.GetFeatureProperties() })
                .Where(i => i.Properties.DetailedCategory == "user:flight_plan_report" && i.Properties.IsOwner)
                .SubscribeWithAsync((i, ct) => OnFlightReportClicked(i.Feature)));
        }

        public async Task SignInAsync(CancellationToken cancellationToken = default)
        {
            if (!_settings.CheckEnableAltitudeAngel)
            {
                return;
            }

            try
            {
                await _signInLock.WaitAsync(cancellationToken);
                
                // Attempt to get a token
                var token = await _tokenProvider.GetToken(cancellationToken);
            }
            catch (FlurlHttpException ex) when (ex.StatusCode == 401)
            {
                // Ignore these as they'll be messaged by the sign in components
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(
                    Message.ForError("There was a problem signing you in to Altitude Angel.", ex));
                _settings.TokenResponse = null;
            }
            finally
            {
                _signInLock.Release();
            }

            if (_settings.TokenResponse.IsValidForAuth())
            {
                IsSignedIn.Value = true;
                await UpdateMapData(_missionPlanner.FlightDataMap, CancellationToken.None);
                await UpdateMapData(_missionPlanner.FlightPlanningMap, CancellationToken.None);
            }
        }

        public Task DisconnectAsync()
        {
            _settings.TokenResponse = null;
            IsSignedIn.Value = false;
            ProcessAllFromCache(_missionPlanner.FlightDataMap);
            ProcessAllFromCache(_missionPlanner.FlightPlanningMap);
            return Task.CompletedTask;
        }

        private async Task UpdateMapData(IMap map, CancellationToken cancellationToken)
        {
            if (!(IsSignedIn.Value || _settings.CheckEnableAltitudeAngel))
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
                var mapData = await _apiClient.GetMapData(area, cancellationToken);
                sw.Stop();

                foreach (var errorReason in mapData.ExcludedData.Select(e => e.ErrorReason).Distinct())
                {
                    var reasons = mapData.ExcludedData.Where(e => e.ErrorReason == errorReason).ToList();
                    if (reasons.Count <= 0) continue;
                    string message;
                    switch (errorReason)
                    {
                        case "QueryAreaTooLarge":
                            message = $"Zoom in to see {reasons.Select(d => d.Detail.DisplayName).AsReadableList()} from Altitude Angel.";
                            break;

                        default:
                            message = $"Warning: {reasons.Select(d => d.Detail.DisplayName).AsReadableList()} have been excluded from the Altitude Angel data.";
                            break;
                    }
                    await _messagesService.AddMessageAsync(Message.ForInfo(errorReason, message, TimeSpan.FromSeconds(_settings.MapUpdateRefresh)));
                }

                mapData.Features.UpdateFilterInfo(FilterInfoDisplay);
                _settings.MapFilters = FilterInfoDisplay;

                await _messagesService.AddMessageAsync(Message.ForInfo(
                    "UpdateMapData",
                    $"Map area loaded {area.NorthEast.Latitude:F4}, {area.SouthWest.Latitude:F4}, {area.SouthWest.Longitude:F4}, {area.NorthEast.Longitude:F4} in {sw.Elapsed.TotalMilliseconds:N2}ms",
                    TimeSpan.FromSeconds(1)));

                // add all items to cache
                MapFeatureCache.Clear();
                mapData.Features.ForEach(feature => MapFeatureCache[feature.Id] = feature);

                // Only get the features that are enabled by default, and have not been filtered out
                ProcessFeatures(map, mapData.Features);
            }
            catch (Exception ex) when (!(ex is FlurlHttpException) && !(ex.InnerException is TaskCanceledException))
            {
                await _messagesService.AddMessageAsync(Message.ForError("UpdateMapData", "Failed to update map data.", ex));
            }
        }

        public void ProcessAllFromCache(IMap map, bool resetFilters = false)
        {
            map.DeleteOverlay(MapOverlayName);
            if (!(IsSignedIn || _settings.CheckEnableAltitudeAngel))
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
                MapFeatureCache.Values.UpdateFilterInfo(FilterInfoDisplay, true);
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
                    if (properties.AltitudeFloor != null)
                    {
                        // TODO: Ignoring datum for now
                        if (properties.AltitudeFloor.Meters > _settings.AltitudeFilter)
                        {
                            continue;
                        }
                    }

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
            if (!(_settings.UseFlightPlans && _settings.TokenResponse.HasScopes(Scopes.ManageFlightReports)))
            {
                return;
            }

            if (_settings.CurrentFlightPlanId == null && _settings.ExistingFlightPlanId != Guid.Parse(feature.Id))
            {
                if (!await _missionPlanner.ShowYesNoMessageBox(
                        $"You have clicked your flight plan '{feature.GetDisplayInfo().Title}'.{Environment.NewLine}Would you like to use this flight plan when you arm your drone?",
                        "Flight Plan")) return;
                _settings.ExistingFlightPlanId = Guid.Parse(feature.Id);
                _settings.UseExistingFlightPlanId = true;
                await _messagesService.AddMessageAsync(
                    Message.ForInfo($"Use existing flight plan ID set to {feature.Id}", TimeSpan.FromSeconds(10)));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;
            _telemetryService.Dispose();
            _flightService.Dispose();
            _disposer?.Dispose();
        }
    }
}
