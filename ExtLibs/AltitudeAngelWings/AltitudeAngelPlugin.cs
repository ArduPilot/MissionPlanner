using System;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Messaging;

namespace AltitudeAngelWings
{
    public static class AltitudeAngelPlugin
    {
        public static void Configure()
        {
            ServiceLocator.Register<ISettings>(l => new Settings(
                l.Resolve<IMissionPlanner>()));
            ServiceLocator.Register<IMessagesService>(l => new MessagesService());
            ServiceLocator.Register<IFlightDataProvider>(l => new MissionPlannerFlightDataProvider(
                l.Resolve<IMissionPlannerState>()));
            ServiceLocator.Register<IFlightDataService>(l => new FlightDataService(
                TimeSpan.FromMilliseconds(500),
                l.Resolve<IFlightDataProvider>()));
            ServiceLocator.Register<IAltitudeAngelClient>(l => new AltitudeAngelClient(
                l.Resolve<ISettings>().AuthenticationUrl,
                l.Resolve<ISettings>().ApiUrl,
                l.Resolve<ISettings>().AuthToken,
                (a, s) => new AltitudeAngelHttpHandlerFactory(a, s,
                    l.Resolve<ISettings>().ClientId,
                    l.Resolve<ISettings>().ClientSecret)));
            ServiceLocator.Register<IAltitudeAngelService>(l => new AltitudeAngelService(
                l.Resolve<IMessagesService>(),
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<IFlightDataService>(),
                l.Resolve<IAltitudeAngelClient>()));
        }
    }
}