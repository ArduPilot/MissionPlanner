using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace AltitudeAngelWings.Clients
{
    public class PolicyHandler : DelegatingHandler
    {
        private readonly IAsyncPolicy _asyncPolicy;

        public PolicyHandler(IAsyncPolicy asyncPolicy)
        {
            _asyncPolicy = asyncPolicy;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _asyncPolicy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}