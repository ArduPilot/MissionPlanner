using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class AltitudeAngelClient : IAltitudeAngelClient
    {
        private FlurlClient _client;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettings _settings;

        public AltitudeAngelClient(
            ISettings settings,
            IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _clientFactory = clientFactory;
        }

        protected FlurlClient Client
            => _client ?? (_client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = _clientFactory
                }
            });

        /// <summary>
        ///     Disconnect the client from AA. Will force logon on the next request if required.
        /// </summary>
        public void Disconnect(bool resetAuth = false)
        {
            _client?.Dispose();
            _client = null;
            if (resetAuth)
            {
                _settings.TokenResponse = null;
            }
        }

        /// <summary>
        ///     Get the user profile for the current user. Must be using user auth. Required scopes: query_userinfo
        /// </summary>
        /// <returns>The user profile.</returns>
        public Task<UserProfileInfo> GetUserProfile(CancellationToken cancellationToken = default)
            => _settings.AuthenticationUrl
                .AppendPathSegment("userProfile")
                .WithClient(Client)
                .GetJsonAsync<UserProfileInfo>(cancellationToken: cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
                _client = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
