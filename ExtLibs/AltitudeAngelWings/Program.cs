using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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

            //var t3 = t2.GetMapData(new RectLatLng(new PointLatLng(-34.8, 117.89), new SizeLatLng(0.1, 0.1)));

            AltitudeAngelService service = new AltitudeAngelService(new MessagesService(), new MissionPlanner(),
                new FlightDataService(new ObservableProperty<long>(), new MissionPlannerFlightDataProvider(null)), AaClientFactory);

            service.SignInAsync();


            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
            //service.UpdateMapData();
        }

        private static AltitudeAngelClient AaClientFactory(string authUrl, string apiUrl, AuthorizationState existingState)
        {
            return new AltitudeAngelClient(authUrl, apiUrl, null, HandlerFactory);
        }

        private static AltitudeAngelHttpHandlerFactory HandlerFactory(string authUrl, AuthorizationState existingState)
        {
            return new AltitudeAngelHttpHandlerFactory(authUrl, existingState);
        }
    }
}
