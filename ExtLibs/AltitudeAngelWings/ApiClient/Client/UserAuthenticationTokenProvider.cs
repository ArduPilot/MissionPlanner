using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class UserAuthenticationTokenProvider : ITokenProvider
    {
        private readonly ISettings _settings;
        private readonly Lazy<IAltitudeAngelService> _service;
        private readonly IAuthorizeCodeProvider _provider;
        private readonly IMessagesService _messagesService;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly FlurlClient _client;

        public UserAuthenticationTokenProvider(ISettings settings, IHttpClientFactory clientFactory, Lazy<IAltitudeAngelService> service, IAuthorizeCodeProvider provider, IMessagesService messagesService)
        {
            _settings = settings;
            _service = service;
            _provider = provider;
            _messagesService = messagesService;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                }
            };
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
                        await _messagesService.AddMessageAsync(Message.ForInfo("Refreshing Altitude Angel access token."));
                        return await RefreshAccessToken(cancellationToken);
                    }
                    catch (Exception)
                    {
                        // Ignore and try asking user
                    }
                }

                //_settings.TokenResponse = null;
                if (_service.Value.SigningIn)
                {
                    return await AskUserForAccessToken(cancellationToken);
                }
                else
                {
                    await _messagesService.AddMessageAsync(Message.ForAction(
                        "AskToSignIn",
                        "You need to sign into Altitude Angel. Click here to sign in.",
                        () => Task.Factory.StartNew(() => AskUserForAccessToken(CancellationToken.None), cancellationToken),
                        () => _settings.TokenResponse.IsValidForAuth()));
                    return null;
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<string> RefreshAccessToken(CancellationToken cancellationToken)
        {
            _settings.TokenResponse = await _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "refresh_token",
                        refresh_token = _settings.TokenResponse.RefreshToken,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();
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
            
            var authCode = await _provider.GetAuthorizeCode(
                FormatCodeAuthorizeUri(
                    new Uri($"{_settings.AuthenticationUrl}/oauth/v2/authorize"),
                    parameters));

            _settings.TokenResponse = await _settings.AuthenticationUrl
                .AppendPathSegments("oauth", "v2", "token")
                .WithClient(_client)
                .PostUrlEncodedAsync(
                    new
                    {
                        client_id = _settings.ClientId,
                        client_secret = _settings.ClientSecret,
                        redirect_uri = _settings.RedirectUri,
                        grant_type = "authorization_code",
                        code = authCode,
                        token_format = "jwt"
                    },
                    cancellationToken)
                .ReceiveJson<TokenResponse>();
            
            _service.Value.IsSignedIn.Value = _settings.TokenResponse.IsValidForAuth();
            return _settings.TokenResponse.AccessToken;
        }

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