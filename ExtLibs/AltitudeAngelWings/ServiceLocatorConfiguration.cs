using System;
using System.Net;
using System.Net.Http;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Api;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Clients.Flight;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests;
using AltitudeAngelWings.Clients.Flight.Model.ServiceRequests.ProtocolConfiguration;
using AltitudeAngelWings.Clients.Surveillance;
using AltitudeAngelWings.Clients.Telemetry;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.FlightService;
using AltitudeAngelWings.Service.Messaging;
using AltitudeAngelWings.Service.OutboundNotifications;
using Flurl.Http;
using Flurl.Http.Configuration;
using Markdig;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Polly;

namespace AltitudeAngelWings
{
    public class ServiceLocatorConfiguration : IServiceLocatorConfiguration
    {
        public void Configure()
        {
            ServiceLocator.Register<IAsyncPolicy>(l => Policy.WrapAsync(
                Policy
                    .Handle<FlurlHttpException>(e => e.StatusCode == 401 && !l.Resolve<IAltitudeAngelService>().SigningIn && l.Resolve<ISettings>().TokenResponse.CanBeRefreshed())
                    .RetryAsync((e, i) => ResetAccessToken(l.Resolve<ISettings>())),
                Policy
                    .Handle<FlurlHttpException>(e => e.StatusCode >= 500)
                    .WaitAndRetryAsync(5,  i => TimeSpan.FromSeconds(Math.Pow(2, i) / 2)),
                Policy
                    .TimeoutAsync(TimeSpan.FromSeconds(30))));
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
                l.Resolve<IAuthClient>(),
                new Lazy<IAltitudeAngelService>(() => l.Resolve<IAltitudeAngelService>()),
                l.Resolve<IAuthorizeCodeProvider>(),
                l.Resolve<IMessagesService>()));
            ServiceLocator.Register<IHttpClientFactory>("Auth",
                l => new DelegatingHttpHandlerFactory(() => new PolicyHandler(l.Resolve<IAsyncPolicy>())
                {
                    InnerHandler = new UserAgentHandler(l.Resolve<IMissionPlanner>().VersionHeader)
                    {
                        InnerHandler = new HttpClientHandler
                        {
                            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                        }
                    }
                }));
            ServiceLocator.Register<IHttpClientFactory>(
                l => new DelegatingHttpHandlerFactory(() => new PolicyHandler(l.Resolve<IAsyncPolicy>())
                {
                    InnerHandler = new BearerTokenHttpMessageHandler(l.Resolve<ITokenProvider>())
                    {
                        InnerHandler = new UserAgentHandler(l.Resolve<IMissionPlanner>().VersionHeader)
                        {
                            InnerHandler = new HttpClientHandler
                            {
                                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                            }
                        }
                    }
                }));
            ServiceLocator.Register<ITelemetryClient>(l => new TelemetryClient(l.Resolve<IAutpService>()));
            ServiceLocator.Register<ISerializer>(l =>
            {
                var settings = new JsonSerializerSettings
                {
                    DateParseHandling = DateParseHandling.None
                };
                settings.Converters.Add(new BaseNotificationProtocolConfigurationConverter());
                settings.Converters.Add(new BaseTelemetryProtocolConfigurationConverter());
                settings.Converters.Add(new FlightServiceResponseConverter());
                settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return new NewtonsoftJsonSerializer(settings);
            });
            ServiceLocator.Register<IFlightClient>(l => new FlightClient(
                l.Resolve<ISettings>(),
                l.Resolve<IHttpClientFactory>(),
                l.Resolve<ISerializer>()));
            ServiceLocator.Register<ISurveillanceClient>(l => new SurveillanceClient(
                l.Resolve<ISettings>(),
                l.Resolve<IHttpClientFactory>(),
                l.Resolve<ISerializer>()));
            ServiceLocator.Register<IApiClient>(l => new ApiClient(
                l.Resolve<ISettings>(),
                l.Resolve<IHttpClientFactory>(),
                l.Resolve<ISerializer>()));
            ServiceLocator.Register<IAuthClient>(l => new AuthClient(
                l.Resolve<ISettings>(),
                l.Resolve<IHttpClientFactory>("Auth"),
                l.Resolve<ISerializer>()));
            ServiceLocator.Register<IOutboundNotificationsService>(l => new OutboundNotificationsService(
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<IMessagesService>(),
                l.Resolve<IFlightClient>(),
                l.Resolve<IMissionPlannerState>()));
            ServiceLocator.Register<IFlightService>(l => new FlightService(
                l.Resolve<IMessagesService>(),
                l.Resolve<IMissionPlannerState>(),
                l.Resolve<ISettings>(),
                l.Resolve<IFlightDataService>(),
                l.Resolve<IFlightClient>(),
                l.Resolve<IAuthClient>(),
                l.Resolve<IOutboundNotificationsService>()));
            ServiceLocator.Register<ITelemetryService>(l => new TelemetryService(
                l.Resolve<IMessagesService>(),
                l.Resolve<ISettings>(),
                l.Resolve<IFlightDataService>(),
                l.Resolve<ITelemetryClient>()));
            ServiceLocator.Register<IAltitudeAngelService>(l => new AltitudeAngelService(
                l.Resolve<IMessagesService>(),
                l.Resolve<IMissionPlanner>(),
                l.Resolve<ISettings>(),
                l.Resolve<ITokenProvider>(),
                    l.Resolve<IApiClient>(),
                l.Resolve<ITelemetryService>(),
                l.Resolve<IFlightService>()));
            ServiceLocator.Register(l => new MarkdownPipelineBuilder()
                    .UseAbbreviations()
                    .UseAutoIdentifiers()
                    .UseCitations()
                    .UseDefinitionLists()
                    .UseEmphasisExtras()
                    .UseFooters()
                    .UseFootnotes()
                    .UseGridTables()
                    .UsePipeTables()
                    .UseListExtras()
                    .UseTaskLists()
                    .DisableHtml()
                    .UseSmartyPants()
                    .Build());
        }

        private static void ResetAccessToken(ISettings settings)
        {
            if (settings.TokenResponse == null) return;
            var token = settings.TokenResponse;
            token.AccessToken = "";
            settings.TokenResponse = token;
        }
    }
}