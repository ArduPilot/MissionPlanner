using System;
using System.Net.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.Clients
{
    public class DelegatingHttpHandlerFactory : DefaultHttpClientFactory
    {
        private readonly Func<HttpMessageHandler> _handlerFactory;

        public DelegatingHttpHandlerFactory(Func<HttpMessageHandler> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public override HttpMessageHandler CreateMessageHandler() => _handlerFactory();
    }
}
