using System;
using System.IO;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.ApiClient.Client.FlightClient;
using AltitudeAngelWings.ApiClient.Client.TelemetryClient;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.FlightService;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifs;
using Flurl.Http;
using Flurl.Http.Configuration;
using Polly;

namespace AltitudeAngelWings
{
    public static class AltitudeAngelPlugin
    {
        public static class Resources
        {
            public static Stream Logo => new MemoryStream((byte[])GetObject("AALogo"));

            public static string MapInfoDockPanel => (string)GetObject("MapInfoDockPanel");

            public static string MapInfoDockPanelReplace => "###MapInfoDockPanel###";

            private static object GetObject(string name)
            {
                return Properties.Resources.ResourceManager.GetObject(name);
            }
        }

        public static void Configure()
        {
            ServiceLocator.Register<IAsyncPolicy>(l => Policy.WrapAsync(
                Policy
                    .TimeoutAsync(TimeSpan.FromSeconds(30)),
                Policy
                    .Handle<FlurlHttpException>(e => e.StatusCode == 401)
                    .RetryAsync(2, (exception, i) => l.Resolve<ISettings>().TokenResponse.AccessToken = ""),
                Policy
                    .Handle<FlurlHttpException>(e => e.StatusCode >= 500)
                    .WaitAndRetryAsync(5,  i => TimeSpan.FromSeconds(Math.Pow(2, i) / 10)),
                Policy
                    .TimeoutAsync(TimeSpan.FromSeconds(5))));
            ServiceLocator.Register<IEncryptionKeyGenerator>(l => new HmacKeyGenerator(
                l.Resolve<ISettings>().EncryptionHashType,
                l.Resolve<ISettings>().EncryptionKeySecret));
            ServiceLocator.Register<IEncryptionAlgorithm>(l => new SymmetricEncryptionAlgorithm(
                l.Resolve<ISettings>().EncryptionType,
                l.Resolve<IEncryptionKeyGenerator>()));
            ServiceLocator.Register<IAutpService>(l => new AutpService(
                l.Resolve<IEncryptionAlgorithm>()));
            ServiceLocator.Register<IMessagesService>(l => new MessagesService(
                l.Resolve<IMessageDisplay>()));
            ServiceLocator.Register<IFlightDataProvider>(l => new MissionPlannerFlightDataProvider(
                l.Resolve<IMissionPlannerState>()));
            ServiceLocator.Register<IFlightDataService>(l => new FlightDataService(
                l.Resolve<ISettings>().MinimumPollInterval,
                l.Resolve<IFlightDataProvider>()));
            ServiceLocator.Register<ITokenProvider>(l => new UserAuthenticationTokenProvider(
                l.Resolve<ISettings>(),
                new DefaultHttpClientFactory(),
                l.Resolve<IAuthorizeCodeProvider>(),
                l.Resolve<IMessagesService>()));
            ServiceLocator.Register<IHttpClientFactory>(l => new AltitudeAngelHttpHandlerFactory(
                l.Resolve<ITokenProvider>()));
            ServiceLocator.Register<ITelemetryClient>(l => new TelemetryClient(l.Resolve<IAutpService>()));
            ServiceLocator.Register<IFlightClient>(l => new FlightClient(
                l.Resolve<ISettings>().FlightServiceUrl,
                l.Resolve<IHttpClientFactory>(),
                l.Resolve<IAsyncPolicy>()));
            ServiceLocator.Register<IAltitudeAngelClient>(l => new AltitudeAngelClient(
                l.Resolve<ISettings>(),
                l.Resolve<IHttpClientFactory>(),
                l.Resolve<IAsyncPolicy>()));
            ServiceLocator.Register<IOutboundNotifsService>(l => new OutboundNotifsService(
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<IMessagesService>(),
                l.Resolve<IFlightClient>(),
                l.Resolve<IMissionPlannerState>()));
            ServiceLocator.Register<IFlightService>(l => new FlightService(
                l.Resolve<IMessagesService>(),
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<IFlightDataService>(),
                l.Resolve<IAltitudeAngelClient>(),
                l.Resolve<IOutboundNotifsService>()));
            ServiceLocator.Register<ITelemetryService>(l => new TelemetryService(
                l.Resolve<IMessagesService>(),
                l.Resolve<ISettings>(),
                l.Resolve<IFlightDataService>(),
                l.Resolve<ITelemetryClient>()));
            ServiceLocator.Register<IAltitudeAngelService>(l => new AltitudeAngelService(
                l.Resolve<IMessagesService>(),
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<IAltitudeAngelClient>()));
        }
    }
}