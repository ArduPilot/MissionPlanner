using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;

namespace AltitudeAngelWings.Clients
{
    public class UserAuthenticationTokenProvider : ITokenProvider
    {
        private readonly ISettings _settings;
        private readonly IAuthClient _authClient;
        private readonly Lazy<IAltitudeAngelService> _service;
        private readonly IAuthorizeCodeProvider _provider;
        private readonly IMessagesService _messagesService;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public UserAuthenticationTokenProvider(ISettings settings, IAuthClient authClient, Lazy<IAltitudeAngelService> service, IAuthorizeCodeProvider provider, IMessagesService messagesService)
        {
            _settings = settings;
            _authClient = authClient;
            _service = service;
            _provider = provider;
            _messagesService = messagesService;
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
                        _settings.TokenResponse = await _authClient.GetTokenFromRefreshToken(_settings.TokenResponse.RefreshToken, cancellationToken);
                        return _settings.TokenResponse.AccessToken;
                    }
                    catch (Exception)
                    {
                        // Ignore and try asking user
                    }
                }

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

            _settings.TokenResponse = await _authClient.GetTokenFromAuthorizationCode(authCode, cancellationToken);
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