using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Text;
using AltitudeAngel.IsolatedPlugin.Common.Maps;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Messaging;
using DotNetOpenAuth.OAuth2;
using GMap.NET;

namespace AltitudeAngelWings
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //AltitudeAngelHttpHandlerFactory handlerFactory = new AltitudeAngelHttpHandlerFactory(ConfigurationManager.AppSettings["AuthURL"], null);

            //var t1 = handlerFactory.CreateMessageHandler();

            //var t2 = new AltitudeAngelClient(ConfigurationManager.AppSettings["AuthURL"],ConfigurationManager.AppSettings["APIURL"], null, HandlerFactory);

            var t1 = new MessagesService();
            var t2 = new MissionPlanner();

            AltitudeAngelService service = new AltitudeAngelService(t1, t2,
                new FlightDataService(new ObservableProperty<long>(), new MissionPlannerFlightDataProvider(null)), AaClientFactory);

            //if (!service.IsSignedIn)
                //service.SignInAsync();

            //var t3 = service.UpdateMapData(t2.FlightDataMap);

            var t4 = service.Client.GetMapData(t2.FlightDataMap.GetViewArea());

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
            //service.UpdateMapData();
        }

        private static AltitudeAngelClient AaClientFactory(string authUrl, string apiUrl, AuthorizationState existingState)
        {
            return new AltitudeAngelClient(authUrl, apiUrl, existingState, HandlerFactory);
        }

        private static AltitudeAngelHttpHandlerFactory HandlerFactory(string authUrl, AuthorizationState existingState)
        {
            return new AltitudeAngelHttpHandlerFactory(authUrl, existingState);
        }
    }

    public class FMap : IMap
    {
        public PointLatLng GetCenter()
        {
            return new PointLatLng(-35,117.89);
        }

        public RectLatLng GetViewArea()
        {
            return new RectLatLng(new PointLatLng(-35.001759,117.860954), new SizeLatLng(.03,.03));
        }

        public void AddOverlay(string name)
        {
           
        }

        public void DeleteOverlay(string name)
        {
           
        }

        public IOverlay GetOverlay(string name, bool createIfNotExists = false)
        {
            return new overlay();
        }

        public IObservable<Unit> MapChanged { get; }
    }

    public class overlay : IOverlay
    {
        public void AddPolygon(string name, List<PointLatLng> points, ColorInfo colorInfo)
        {
            
        }

        public void AddLine(string name, List<PointLatLng> points, ColorInfo colorInfo)
        {
            
        }

        public bool LineExists(string name)
        {
            return true;
        }

        public bool PolygonExists(string name)
        {
           return true;
        }

        public bool IsVisible { get; set; }
    }
}
