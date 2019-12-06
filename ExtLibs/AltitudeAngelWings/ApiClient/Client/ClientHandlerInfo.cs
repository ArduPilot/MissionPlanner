using System;
using System.Net.Http;
using DotNetOpenAuth.OAuth2;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class ClientHandlerInfo : IDisposable
    {
        public HttpMessageHandler ClientHandler { get; private set; }
        public IAuthorizationState AuthorizationState { get; }

        public ClientHandlerInfo(HttpMessageHandler clientHandler, IAuthorizationState authorizationState)
        {
            ClientHandler = clientHandler;
            AuthorizationState = authorizationState;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                ClientHandler?.Dispose();
                ClientHandler = null;
            }
        }
    }
}
