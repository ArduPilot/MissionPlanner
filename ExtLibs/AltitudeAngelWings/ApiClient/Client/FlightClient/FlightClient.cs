using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public class FlightClient : IFlightClient
    {
        private readonly string _flightServiceUrl;
        private readonly FlurlClient _client;

        public FlightClient(string flightServiceUrl, IHttpClientFactory clientFactory)
        {
            _flightServiceUrl = flightServiceUrl;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                }
            };
        }

        /// <summary>
        /// Starts a flight via FlightService API
        /// </summary>
        /// <param name="notificationProtocolUrl"></param>
        /// <param name="flightPlanId"></param>
        /// <param name="deconflictionService">deconfliction service required</param>
        /// <returns></returns>
        public Task<JObject> StartFlight(string notificationProtocolUrl, string flightPlanId, string deconflictionService)
            => _flightServiceUrl
                .AppendPathSegments("flight", "start")
                .WithClient(_client)
                .PostJsonAsync(new
                {
                    flightPlanId,
                    serviceRequired = deconflictionService,
                    telemetryProtocols = new List<object> { new { type = "Udp" } },
                    notificationProtocols = new List<object> { new { type = "Webhook", properties = new { url = notificationProtocolUrl } } }
                })
                .ReceiveJson<JObject>();

        /// <summary>
        /// Completes a flight via FlightService API
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public Task CompleteFlight(string flightId)
            => _flightServiceUrl
                .AppendPathSegments("flight", "complete")
                .SetQueryParam("id", flightId)
                .WithClient(_client)
                .DeleteAsync();

        public Task AcceptInstruction(string instructionId) => ProcessInstruction(instructionId, true);

        public Task RejectInstruction(string instructionId) => ProcessInstruction(instructionId, false);

        private Task ProcessInstruction(string instructionId, bool accept)
            => _flightServiceUrl
                .AppendPathSegments("flight", "v2", "instructions", instructionId, accept ? "accept" : "reject")
                .WithClient(_client)
                .PutAsync();

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
