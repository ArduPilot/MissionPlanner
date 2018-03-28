using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AltitudeAngel.IsolatedPlugin.Common;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Properties;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.Messaging;
using DotNetOpenAuth.OAuth2;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GMap.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Service
{
    public class AltitudeAngelService : IDisposable
    {
        public ObservableProperty<bool> IsSignedIn { get; }
        public ObservableProperty<WeatherInfo> WeatherReport { get; }
        public ObservableProperty<Unit> SentTelemetry { get; }
        public UserProfileInfo CurrentUser { get; private set; }
        public readonly List<string> FilteredOut = new List<string>();

        private bool _grounddata = true;
        public bool GroundDataDisplay
        {
            get
            {
                if (String.IsNullOrEmpty(_missionPlanner.LoadSetting("AA.Ground")))
                    return _grounddata;
                _grounddata = bool.Parse(_missionPlanner.LoadSetting("AA.Ground"));
                return _grounddata;
            }
            set
            {
                _grounddata = value;
                _missionPlanner.SaveSetting("AA.Ground", _grounddata.ToString());
            }
        }

        private bool _airdata = true;
        public bool AirDataDisplay
        {
            get
            {
                if (String.IsNullOrEmpty(_missionPlanner.LoadSetting("AA.Air")))
                    return _airdata;
                _airdata = bool.Parse(_missionPlanner.LoadSetting("AA.Air"));
                return _airdata;
            }
            set
            {
                _airdata = value;
                _missionPlanner.SaveSetting("AA.Air", _airdata.ToString());
            }
        }

        public AltitudeAngelService(
            IMessagesService messagesService,
            IMissionPlanner missionPlanner,
            FlightDataService flightDataService
        )
        {
            _messagesService = messagesService;
            _missionPlanner = missionPlanner;
            _flightDataService = flightDataService;
            IsSignedIn = new ObservableProperty<bool>(false);
            WeatherReport = new ObservableProperty<WeatherInfo>();
            SentTelemetry = new ObservableProperty<Unit>();

            CreateClient((url, apiUrl, state) =>
                new AltitudeAngelClient(url, apiUrl, state,
                    (authUrl, existingState) => new AltitudeAngelHttpHandlerFactory(authUrl, existingState)));

            try
            {
                _disposer.Add(_missionPlanner.FlightDataMap
                    .MapChanged
                    .Throttle(TimeSpan.FromSeconds(1))
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
                    .Subscribe(async i => await UpdateMapData(_missionPlanner.FlightPlanningMap)));
            }
            catch
            {
            }

            try
            {
                var list = JsonConvert.DeserializeObject<List<string>>(_missionPlanner.LoadSetting("AAWings.Filters"));

                FilteredOut.AddRange(list.Distinct());
            }
            catch
            {

            }

            TryConnect();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SignInAsync()
        {
            try
            {
                // Load the user's profile, will trigger auth
                await LoadUserProfile();

                // Save the token from the auth process
                _missionPlanner.SaveSetting("AAWings.Token", JsonConvert.SerializeObject(_aaClient.AuthorizationState));

                SignedIn(true);
            }
            catch (Exception ex)
            {
                await _messagesService.AddMessageAsync($"There was a problem signing you in.\r\n{ex.Message}");
            }
        }

        public Task DisconnectAsync()
        {
            _missionPlanner.ClearSetting("AAWings.Token");
            _aaClient.Disconnect();
            SignedOut();

            return null;
        }

        public void RemoveOverlays()
        {
            _missionPlanner.FlightDataMap.DeleteOverlay("AAMapData.Air");
            _missionPlanner.FlightDataMap.DeleteOverlay("AAMapData.Ground");

            _missionPlanner.FlightPlanningMap.DeleteOverlay("AAMapData.Air");
            _missionPlanner.FlightPlanningMap.DeleteOverlay("AAMapData.Ground");
        }

        /// <summary>
        ///     Updates a map with the latest ground / air data
        /// </summary>
        /// <param name="map">The map to update</param>
        /// <returns></returns>
        public async Task UpdateMapData(IMap map)
        {
            if (!IsSignedIn)
                return;

            try
            {
                RectLatLng area = map.GetViewArea();
                await _messagesService.AddMessageAsync($"Map area {area.Top}, {area.Bottom}, {area.Left}, {area.Right}");

                AAFeatureCollection mapData = await _aaClient.GetMapData(area);

                // build the filter list
                mapData.GetFilters();

                // this ensures the user sees the results before its saved
                _missionPlanner.SaveSetting("AAWings.Filters", JsonConvert.SerializeObject(FilteredOut));

                await _messagesService.AddMessageAsync($"Map area Loaded {area.Top}, {area.Bottom}, {area.Left}, {area.Right}");

                // add all items to cache
                mapData.Features.ForEach(feature => cache[feature.Id] = feature);

                // Only get the features that are enabled by default, and have not been filtered out
                IEnumerable<Feature> features = mapData.Features.Where(feature => feature.IsEnabledByDefault() && feature.IsFilterOutItem(FilteredOut)).ToList();

                ProcessFeatures(map, features);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static Dictionary<string, Feature> cache = new Dictionary<string, Feature>();

        public void ProcessAllFromCache(IMap map)
        {
            map.DeleteOverlay("AAMapData.Air");
            map.DeleteOverlay("AAMapData.Ground");
            ProcessFeatures(map, cache.Values.Where(feature => feature.IsEnabledByDefault() && feature.IsFilterOutItem(FilteredOut)).ToList());
            map.Invalidate();
        }

        public void ProcessFeatures(IMap map, IEnumerable<Feature> features)
        {
            IOverlay airOverlay = map.GetOverlay("AAMapData.Air", true);
            IOverlay groundOverlay = map.GetOverlay("AAMapData.Ground", true);

            groundOverlay.IsVisible = GroundDataDisplay;
            airOverlay.IsVisible = AirDataDisplay;

            foreach (Feature feature in features)
            {
                IOverlay overlay = string.Equals((string) feature.Properties.Get("category"), "airspace")
                    ? airOverlay
                    : groundOverlay;

                var altitude = ((JObject) feature.Properties.Get("altitudeFloor"))?.ToObject<Altitude>();

                if (altitude == null || altitude.Meters <= 152)
                {
                    if (!GroundDataDisplay)
                    {
                        if (overlay.PolygonExists(feature.Id))
                            continue;
                    }
                }
                else
                {
                    if (!AirDataDisplay)
                    {
                        continue;
                    }
                }

                switch (feature.Geometry.Type)
                {
                    case GeoJSONObjectType.Point:
                    {
                        if (!overlay.PolygonExists(feature.Id))
                        {
                            var pnt = (Point) feature.Geometry;

                            List<PointLatLng> coordinates = new List<PointLatLng>();

                            if (feature.Properties.ContainsKey("radius"))
                            {
                                var rad = double.Parse(feature.Properties["radius"].ToString());

                                for (int i = 0; i <= 360; i+=10)
                                {
                                    coordinates.Add(
                                        newpos(new PointLatLng(((Position) pnt.Coordinates).Latitude,
                                            ((Position) pnt.Coordinates).Longitude), i, rad));
                                }
                            }

                            ColorInfo colorInfo = feature.ToColorInfo();
                            colorInfo.StrokeColor = 0xFFFF0000;
                            overlay.AddPolygon(feature.Id, coordinates, colorInfo, feature);
                        }
                    }
                        break;
                    case GeoJSONObjectType.MultiPoint:
                        break;
                    case GeoJSONObjectType.LineString:
                    {
                        if (!overlay.LineExists(feature.Id))
                        {
                            var line = (LineString) feature.Geometry;
                            List<PointLatLng> coordinates = line.Coordinates.OfType<Position>()
                                .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                .ToList();
                            overlay.AddLine(feature.Id, coordinates, new ColorInfo {StrokeColor = 0xFFFF0000}, feature);
                        }
                    }
                        break;

                    case GeoJSONObjectType.MultiLineString:
                        break;
                    case GeoJSONObjectType.Polygon:
                    {
                        if (!overlay.PolygonExists(feature.Id))
                        {
                            var poly = (Polygon) feature.Geometry;
                            List<PointLatLng> coordinates =
                                poly.Coordinates[0].Coordinates.OfType<Position>()
                                    .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                    .ToList();

                            ColorInfo colorInfo = feature.ToColorInfo();
                            colorInfo.StrokeColor = 0xFFFF0000;
                            overlay.AddPolygon(feature.Id, coordinates, colorInfo, feature);
                        }
                    }
                        break;
                    case GeoJSONObjectType.MultiPolygon:
                        if (!overlay.PolygonExists(feature.Id))
                        {
                            foreach (var poly in ((MultiPolygon) feature.Geometry).Coordinates)
                            {
                                List<PointLatLng> coordinates =
                                    poly.Coordinates[0].Coordinates.OfType<Position>()
                                        .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                        .ToList();

                                ColorInfo colorInfo = feature.ToColorInfo();
                                colorInfo.StrokeColor = 0xFFFF0000;
                                overlay.AddPolygon(feature.Id, coordinates, colorInfo, feature);
                            }
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
        }


        public PointLatLng newpos(PointLatLng input, double bearing, double distance)
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
        public async Task UpdateWeatherData(PointLatLng center)
        {
            WeatherReport.Value = await _aaClient.GetWeather(center);
        }

        private void CreateClient(AltitudeAngelClient.Create aaClientFactory)
        {
            string stateString = _missionPlanner.LoadSetting("AAWings.Token");
            AuthorizationState existingState = null;
            if (stateString != null)
            {
                existingState = JsonConvert.DeserializeObject<AuthorizationState>(stateString);
            }

            _aaClient = aaClientFactory(ConfigurationManager.AppSettings["AuthURL"],
                ConfigurationManager.AppSettings["APIURL"], existingState);
        }

        private async void TryConnect()
        {
            if (_missionPlanner.LoadSetting("AAWings.Token") != null)
            {
                await SignInAsync();
            }
        }

        private async Task LoadUserProfile()
        {
            CurrentUser = await _aaClient.GetUserProfile();
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

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _disposer?.Dispose();
                _disposer = null;
            }
        }

        private readonly FlightDataService _flightDataService;
        private readonly IMessagesService _messagesService;
        private readonly IMissionPlanner _missionPlanner;
        private AltitudeAngelClient _aaClient;
        private CompositeDisposable _disposer = new CompositeDisposable();

        public AltitudeAngelClient Client
        {
            get { return _aaClient; }
        }
    }
}
