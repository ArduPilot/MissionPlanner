using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GMap.NET;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Service
{
    public class AltitudeAngelService : IAltitudeAngelService
    {
        public ObservableProperty<bool> IsSignedIn { get; }
        public ObservableProperty<WeatherInfo> WeatherReport { get; }
        public ObservableProperty<Unit> SentTelemetry { get; }
        public UserProfileInfo CurrentUser { get; private set; }
        public IList<string> FilteredOut { get; } = new List<string>();

        public AltitudeAngelService(
            IMessagesService messagesService,
            IMissionPlanner missionPlanner,
            ISettings settings,
            IFlightDataService flightDataService,
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
            SentTelemetry = new ObservableProperty<Unit>();
            _disposer.Add(SentTelemetry);

            try
            {
                _disposer.Add(_missionPlanner.FlightDataMap
                    .MapChanged
                    .Throttle(TimeSpan.FromSeconds(10))
                    .RepeatLastValue(TimeSpan.FromSeconds(60))
                    .Subscribe(async i => await UpdateMapData(_missionPlanner.FlightDataMap)));
            }
            catch
            {
            }

            try
            {
                _disposer.Add(_missionPlanner.FlightPlanningMap
                    .MapChanged
                    .Throttle(TimeSpan.FromSeconds(1))
                    .RepeatLastValue(TimeSpan.FromSeconds(60))
                    .Subscribe(async i => await UpdateMapData(_missionPlanner.FlightPlanningMap)));
            }
            catch
            {
            }

            try
            {
                _disposer.Add(flightDataService.FlightArmed
                    .Subscribe(async i => await SubmitFlightReport(i)));
                _disposer.Add(flightDataService.FlightDisarmed
                    .Subscribe(async i => await CompleteFlightReport(i)));
            }
            catch
            {
            }

            try
            {
                FilteredOut = _settings.MapFilters;
            }
            catch
            {

            }
        }

        private async Task SubmitFlightReport(Models.FlightData flightData)
        {
            await _messagesService.AddMessageAsync(new Message($"ARMED: {flightData.CurrentPosition.Latitude},{flightData.CurrentPosition.Longitude}"));
            if (!_settings.FlightReportEnable || _settings.CurrentFlightReportId != null)
            {
                return;
            }

            try
            {
                var centerPoint = new PointLatLng(flightData.CurrentPosition.Latitude,
                    flightData.CurrentPosition.Longitude);
                var bufferedBoundingRadius = 500;
                var flightPlan = _missionPlanner.GetFlightPlan();
                if (flightPlan != null)
                {
                    centerPoint.Lat = flightPlan.CenterLatitude;
                    centerPoint.Lng = flightPlan.CenterLongitude;
                    bufferedBoundingRadius = Math.Max(flightPlan.BoundingRadius + 50, bufferedBoundingRadius);
                }

                _settings.CurrentFlightReportId = await _client.CreateFlightReport(
                    _settings.FlightReportName,
                    _settings.FlightReportCommercial,
                    DateTime.Now,
                    DateTime.Now.Add(_settings.FlightReportTimeSpan),
                    centerPoint,
                    bufferedBoundingRadius);
                await _messagesService.AddMessageAsync(new Message($"Flight plan {_settings.CurrentFlightReportId} created"));
                await UpdateMapData(_missionPlanner.FlightDataMap);
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(new Message($"Creating flight plan failed. {ex}"));
            }
        }

        private async Task CompleteFlightReport(Models.FlightData flightData)
        {
            await _messagesService.AddMessageAsync(new Message($"DISARMED: {flightData.CurrentPosition.Latitude},{flightData.CurrentPosition.Longitude}"));
            if (_settings.CurrentFlightReportId == null)
            {
                return;
            }
            try
            {
                await _client.CompleteFlightReport(_settings.CurrentFlightReportId);
                _settings.CurrentFlightReportId = null;
                await _messagesService.AddMessageAsync(new Message($"Flight plan {_settings.CurrentFlightReportId} marked as complete"));
                await UpdateMapData(_missionPlanner.FlightDataMap);
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync(new Message($"Marking flight plan {_settings.CurrentFlightReportId} as complete failed. {ex}"));
            }
        }

        public async Task SignInAsync()
        {
            try
            {
                // Load the user's profile, will trigger auth
                await LoadUserProfile();

                // Save the token from the auth process
                _settings.AuthToken = _client.AuthorizationState;

                SignedIn(true);
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync($"There was a problem signing you in.\r\n{ex.Message}");
            }
        }

        public Task DisconnectAsync()
        {
            _settings.AuthToken = null;
            _client.Disconnect();
            SignedOut();
            return null;
        }

        /// <summary>
        ///     Updates a map with the latest ground / air data
        /// </summary>
        /// <param name="map">The map to update</param>
        /// <returns></returns>
        private async Task UpdateMapData(IMap map)
        {
            if (!IsSignedIn)
                return;

            try
            {
                RectLatLng area = map.GetViewArea();
                await _messagesService.AddMessageAsync($"Map area {area.Top}, {area.Bottom}, {area.Left}, {area.Right}");

                AAFeatureCollection mapData = await _client.GetMapData(area);

                // build the filter list
                mapData.GetFilters();

                // this ensures the user sees the results before its saved
                _settings.MapFilters = FilteredOut;

                await _messagesService.AddMessageAsync($"Map area Loaded {area.Top}, {area.Bottom}, {area.Left}, {area.Right}");

                // add all items to cache
                MapFeatureCache.Clear();
                mapData.Features.ForEach(feature => MapFeatureCache[feature.Id] = feature);

                // Only get the features that are enabled by default, and have not been filtered out
                IEnumerable<Feature> features = mapData.Features.Where(feature => feature.IsEnabledByDefault() && feature.IsFilterOutItem(FilteredOut)).ToList();

                ProcessFeatures(map, features);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static readonly Dictionary<string, Feature> MapFeatureCache = new Dictionary<string, Feature>();

        public void ProcessAllFromCache(IMap map)
        {
            map.DeleteOverlay("AAMapData.Air");
            map.DeleteOverlay("AAMapData.Ground");
            ProcessFeatures(map, MapFeatureCache.Values.Where(feature => feature.IsEnabledByDefault() && feature.IsFilterOutItem(FilteredOut)).ToList());
            map.Invalidate();
        }

        private void ProcessFeatures(IMap map, IEnumerable<Feature> features)
        {
            IOverlay airOverlay = map.GetOverlay("AAMapData.Air", true);
            IOverlay groundOverlay = map.GetOverlay("AAMapData.Ground", true);

            groundOverlay.IsVisible = _settings.GroundDataDisplay;
            airOverlay.IsVisible = _settings.AirDataDisplay;

            var polygons = new List<string>();
            var lines = new List<string>();

            foreach (Feature feature in features)
            {
                IOverlay overlay = string.Equals((string)feature.Properties.Get("category"), "airspace")
                    ? airOverlay
                    : groundOverlay;

                var altitude = ((JObject)feature.Properties.Get("altitudeFloor"))?.ToObject<Altitude>();

                if (altitude == null || altitude.Meters <= 152)
                {
                    if (!_settings.GroundDataDisplay)
                    {
                        if (overlay.PolygonExists(feature.Id))
                            continue;
                    }
                }
                else
                {
                    if (!_settings.AirDataDisplay)
                    {
                        continue;
                    }
                }

                switch (feature.Geometry.Type)
                {
                    case GeoJSONObjectType.Point:
                        {
                            var pnt = (Point)feature.Geometry;

                            List<PointLatLng> coordinates = new List<PointLatLng>();

                            if (feature.Properties.ContainsKey("radius"))
                            {
                                var rad = double.Parse(feature.Properties["radius"].ToString());

                                for (int i = 0; i <= 360; i += 10)
                                {
                                    coordinates.Add(
                                        newpos(new PointLatLng(((Position)pnt.Coordinates).Latitude,
                                            ((Position)pnt.Coordinates).Longitude), i, rad));
                                }
                            }

                            ColorInfo colorInfo = feature.ToColorInfo();
                            colorInfo.StrokeColor = 0xFFFF0000;
                            overlay.AddOrUpdatePolygon(feature.Id, coordinates, colorInfo, feature);
                            polygons.Add(feature.Id);
                        }
                        break;
                    case GeoJSONObjectType.MultiPoint:
                        break;
                    case GeoJSONObjectType.LineString:
                        {
                            var line = (LineString)feature.Geometry;
                            List<PointLatLng> coordinates = line.Coordinates.OfType<Position>()
                                .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                .ToList();
                            overlay.AddOrUpdateLine(feature.Id, coordinates, new ColorInfo { StrokeColor = 0xFFFF0000 }, feature);
                            lines.Add(feature.Id);
                        }
                        break;

                    case GeoJSONObjectType.MultiLineString:
                        break;
                    case GeoJSONObjectType.Polygon:
                        {
                            var poly = (Polygon)feature.Geometry;
                            List<PointLatLng> coordinates =
                                poly.Coordinates[0].Coordinates.OfType<Position>()
                                    .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                    .ToList();

                            ColorInfo colorInfo = feature.ToColorInfo();
                            colorInfo.StrokeColor = 0xFFFF0000;
                            overlay.AddOrUpdatePolygon(feature.Id, coordinates, colorInfo, feature);
                            polygons.Add(feature.Id);
                        }
                        break;
                    case GeoJSONObjectType.MultiPolygon:
                        foreach (var poly in ((MultiPolygon)feature.Geometry).Coordinates)
                        {
                            List<PointLatLng> coordinates =
                                poly.Coordinates[0].Coordinates.OfType<Position>()
                                    .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                    .ToList();

                            ColorInfo colorInfo = feature.ToColorInfo();
                            colorInfo.StrokeColor = 0xFFFF0000;
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

            airOverlay.RemovePolygonsExcept(polygons);
            groundOverlay.RemovePolygonsExcept(polygons);
            airOverlay.RemoveLinesExcept(lines);
            groundOverlay.RemoveLinesExcept(lines);
        }

        private PointLatLng newpos(PointLatLng input, double bearing, double distance)
        {
            const double rad2deg = (180 / Math.PI);
            const double deg2rad = (1.0 / rad2deg);

            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            double radius_of_earth = 6378100.0;//# in meters

            double lat1 = deg2rad * (input.Lat);
            double lon1 = deg2rad * (input.Lng);
            double brng = deg2rad * (bearing);
            double dr = distance / radius_of_earth;

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dr) +
                        Math.Cos(lat1) * Math.Sin(dr) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dr) * Math.Cos(lat1),
                                Math.Cos(dr) - Math.Sin(lat1) * Math.Sin(lat2));

            double latout = rad2deg * (lat2);
            double lngout = rad2deg * (lon2);

            return new PointLatLng(latout, lngout);
        }


        /// <summary>
        ///     Update the AA UI with the latest weather
        /// </summary>
        /// <param name="center">The point to get weather for</param>
        /// <returns></returns>
        private async Task UpdateWeatherData(PointLatLng center)
        {
            WeatherReport.Value = await _client.GetWeather(center);
        }

        public async Task SignInIfAuthenticated()
        {
            if (_settings.AuthToken != null)
            {
                await SignInAsync();
            }
        }

        private async Task LoadUserProfile()
        {
            CurrentUser = await _client.GetUserProfile();
        }

        private void SignedIn(bool isSignedIn)
        {
            IsSignedIn.Value = isSignedIn;
            _messagesService.AddMessageAsync("Connected to Altitude Angel.");

            if (isSignedIn)
            {
                _messagesService.AddMessageAsync("Loading map data...")
                    .ContinueWith(async i =>
                    {
                        try
                        {
                            await UpdateMapData(_missionPlanner.FlightDataMap);
                            await _messagesService.AddMessageAsync("Map data loaded");
                        }
                        catch
                        {
                        }
                    });

                // Should really move this to a manual trigger or on arm as the map might not be in the correct position
                // And we only want to do it occasionally
                /*
                _messagesService.AddMessageAsync("Loading weather info...")
                    .ContinueWith(async i =>
                    {
                        try
                        {
                            await UpdateWeatherData(_missionPlanner.FlightDataMap.GetCenter());
                            await _messagesService.AddMessageAsync("Weather loaded");
                        }
                        catch
                        {
                        }
                    });*/
            }
        }

        private void SignedOut()
        {
            IsSignedIn.Value = false;
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

        private readonly IMessagesService _messagesService;
        private readonly IMissionPlanner _missionPlanner;
        private readonly CompositeDisposable _disposer = new CompositeDisposable();
        private readonly IAltitudeAngelClient _client;
        private readonly ISettings _settings;
    }
}
