using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Polly;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class UserAuthenticationTokenProvider : ITokenProvider
    {
        private readonly ISettings _settings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAsyncPolicy _asyncPolicy;
        private readonly IAuthorizeCodeProvider _provider;
        private readonly IMessagesService _messagesService;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly ProductInfoHeaderValue _version;

        public UserAuthenticationTokenProvider(ISettings settings, IHttpClientFactory clientFactory, IAsyncPolicy asyncPolicy, IAuthorizeCodeProvider provider, IMessagesService messagesService, ProductInfoHeaderValue version)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _asyncPolicy = asyncPolicy;
            _provider = provider;
            _messagesService = messagesService;
            _version = version;
        }

        public async Task<string> GetToken(CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_settings.TokenResponse.IsValidForAuth())
                {
                    return _settings.TokenResponse.AccessToken;
                }

                if (_settings.TokenResponse.CanBeRefreshed())
                {
                    try
                    {
                        await _messagesService.AddMessageAsync("Refreshing AA access token.");
                        return await RefreshAccessToken(cancellationToken);
                    }
                    catch (Exception)
                    {
                        // Ignore and try asking user
                    }
                }

                await _messagesService.AddMessageAsync("Asking user for AA access token.");
                return await AskUserForAccessToken(cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<string> RefreshAccessToken(CancellationToken cancellationToken)
        {
            _settings.TokenResponse = await MakeTokenRequest(
                () => GetTokenRequestBody("refresh_token", "refresh_token", _settings.TokenResponse.RefreshToken),
                cancellationToken);
            return _settings.TokenResponse.AccessToken;
        }

        private async Task<string> AskUserForAccessToken(CancellationToken cancellationToken)
        {
            var redirectUri = new Uri(_settings.RedirectUri);

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("client_id", _settings.ClientId);
            parameters.Add("redirect_uri", redirectUri.ToString());
            parameters.Add("scope", string.Join(" ", _settings.ClientScopes));
            parameters.Add("response_type", "code");
            _provider.GetAuthorizeParameters(parameters);
            
            var code = await _provider.GetAuthorizeCode(
                FormatCodeAuthorizeUri(
                    new Uri($"{_settings.AuthenticationUrl}/oauth/v2/authorize"),
                    parameters));
            _settings.TokenResponse = await MakeTokenRequest(
                () => GetTokenRequestBody("authorization_code", "code", code),
                cancellationToken);
            return _settings.TokenResponse.AccessToken;
        }

        private async Task<TokenResponse> MakeTokenRequest(Func<HttpContent> postBody, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateHttpClient(new HttpClientHandler());
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.AuthenticationUrl}/oauth/v2/token")
            {
                Content = postBody(),
                Headers =
                {
                    UserAgent = { _version }
                }
            };
            using (var response = await _asyncPolicy.ExecuteAsync(() => client.SendAsync(request, cancellationToken)))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Failed to get authentication token. {response.StatusCode}: {content}");
                }

                return JsonConvert.DeserializeObject<TokenResponse>(content);
            }
        }

        private HttpContent GetTokenRequestBody(string grantType, string tokenName, string tokenValue)
            => new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["redirect_uri"] = _settings.RedirectUri,
                ["grant_type"] = grantType,
                [tokenName] = tokenValue,
                ["token_format"] = "jwt"
            });

        private static Uri FormatCodeAuthorizeUri(Uri baseUri, NameValueCollection parameters)
        {
            var builder = new UriBuilder(baseUri)
            {
                Query = parameters.ToString()
            };
            return builder.Uri;
        }
    }
}