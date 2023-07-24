using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Surveillance.Model;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.Clients.Surveillance
{
    public class SurveillanceClient : ISurveillanceClient
    {
        private readonly ISettings _settings;
        private readonly IFlurlClient _client;

        public SurveillanceClient(ISettings settings, IHttpClientFactory clientFactory, ISerializer serializer)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings = new ClientFlurlHttpSettings
                {
                    HttpClientFactory = clientFactory,
                    JsonSerializer = serializer
                }
            };
        }

        public Task SendReport(SurveillanceReport surveillanceReport, CancellationToken cancellationToken = default)
            => _settings.SurveillanceUrl
                .AppendPathSegments("v2", "reports")
                .WithClient(_client)
                .PostJsonAsync(surveillanceReport, cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}