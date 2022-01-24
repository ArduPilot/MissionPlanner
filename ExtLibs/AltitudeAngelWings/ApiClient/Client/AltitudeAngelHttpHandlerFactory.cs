using System.Net;
using System.Net.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class AltitudeAngelHttpHandlerFactory : DefaultHttpClientFactory
    {
        private readonly ITokenProvider _tokenProvider;

        public AltitudeAngelHttpHandlerFactory(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public override HttpMessageHandler CreateMessageHandler()
        {
            return new BearerTokenHttpMessageHandler(
                _tokenProvider,
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });
        }
    }
}
