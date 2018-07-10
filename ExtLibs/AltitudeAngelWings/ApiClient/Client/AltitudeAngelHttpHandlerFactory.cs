using System;
using System.Net.Http;
using System.Security;
using AltitudeAngelWings.ApiClient.CodeProvider;
using DotNetOpenAuth.OAuth2;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client
{
    /// <summary>
    ///     Used to create an OAuth aware HTTP client.
    /// </summary>
    public class AltitudeAngelHttpHandlerFactory : DefaultHttpClientFactory, IDisposable
    {
        /// <summary>
        ///     The current authorization state containing the permitted scopes, the access and refresh tokens.
        ///     May be persisted for session continuation across app termination.
        /// </summary>
        public IAuthorizationState AuthorizationState => _handlerInfo?.AuthorizationState;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authUrl">The base auth URL (scheme and host).</param>
        /// <param name="existingState">Any existing state from a previous session.</param>
        public AltitudeAngelHttpHandlerFactory(string authUrl, IAuthorizationState existingState, string clientId, string clientSecret)
        {
            _authUrl = authUrl;
            _existingState = existingState;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        ///     Create the message handler. Modify this to support different scopes.
        /// </summary>
        /// <returns></returns>
        public override HttpMessageHandler CreateMessageHandler()
        {
            _handlerInfo = ApiOAuthClientHandler.Create(
                _authUrl, _clientId, _clientSecret,
                new[] { "query_mapdata", "query_mapairdata", "talk_tower", "query_userinfo", "manage_flightreports" }, _existingState, true, "https://aawings.com/", 
				new WpfAuthorizeDisplay());
                
            return _handlerInfo.ClientHandler;
        }
        
        /// <summary>
        ///     Clear any pre-existing auth state. Will force log on the next time this handler is used for a request.
        /// </summary>
        public void ClearAuthState()
        {
            _existingState = null;
        }

        private readonly string _authUrl;
        private IAuthorizationState _existingState;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private ClientHandlerInfo _handlerInfo;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _handlerInfo?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
