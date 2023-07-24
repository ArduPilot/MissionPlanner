using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Clients
{
    public class UserAgentHandler : DelegatingHandler
    {
        private readonly ProductInfoHeaderValue _version;

        public UserAgentHandler(ProductInfoHeaderValue version)
        {
            _version = version;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Add(_version);
            return base.SendAsync(request, cancellationToken);
        }
    }
}