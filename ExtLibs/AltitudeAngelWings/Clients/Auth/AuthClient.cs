using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Service;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.Clients.Auth
{
    public class AuthClient : IAuthClient
    {
        private readonly ISettings _settings;
        private readonly IFlurlClient _client;

        public AuthClient(ISettings settings, IHttpClientFactory clientFactory, ISerializer serializer)
        {
            _settings = settings;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                    JsonSerializer = serializer
                }
            };
        }

        public Task<TokenResponse> GetTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "refresh_token",
                        refresh_token = refreshToken,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public Task<TokenResponse> GetTokenFromAuthorizationCode(string authorizationCode, CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "authorization_code",
                        code = authorizationCode,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public Task<TokenResponse> GetTokenFromClientCredentials(CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        grant_type = "client_credentials",
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();

        public async Task<string> GetAuthorizationCode(string accessToken, string pollId, CancellationToken cancellationToken)
        {
            var response = await _settings.AuthenticationUrl
                .AppendPathSegments("api", "v1", "security", "get-login")
                .SetQueryParam("id", pollId)
                .WithOAuthBearerToken(accessToken)
                .AllowHttpStatus(HttpStatusCode.NotFound)
                .WithClient(_client)
                .GetAsync(cancellationToken);

            if (response.StatusCode != (int)HttpStatusCode.OK)
            {
                return null;
            }

            var data = await response.GetJsonAsync();
            return data.code;
        }

        public Task<UserProfileInfo> GetUserProfile(string accessToken, CancellationToken cancellationToken)
            => _settings.AuthenticationUrl
                .AppendPathSegment("userProfile")
                .WithOAuthBearerToken(accessToken)
                .WithClient(_client)
                .GetJsonAsync<UserProfileInfo>(cancellationToken);

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