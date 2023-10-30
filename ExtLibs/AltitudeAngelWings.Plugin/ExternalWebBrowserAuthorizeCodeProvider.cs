using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients;
using AltitudeAngelWings.Clients.Auth;
using AltitudeAngelWings.Clients.Auth.Model;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.Plugin;

namespace AltitudeAngelWings.Plugin
{
    public class ExternalWebBrowserAuthorizeCodeProvider : IAuthorizeCodeProvider
    {
        private readonly ISettings _settings;
        private readonly IAuthClient _authClient;
        private readonly IMessagesService _messages;
        private readonly PluginHost _host;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private string _pollId;

        public ExternalWebBrowserAuthorizeCodeProvider(ISettings settings, IAuthClient authClient, IMessagesService messages, PluginHost host, IUiThreadInvoke uiThreadInvoke)
        {
            _settings = settings;
            _authClient = authClient;
            _messages = messages;
            _host = host;
            _uiThreadInvoke = uiThreadInvoke;
        }

        public void GetAuthorizeParameters(NameValueCollection parameters)
        {
            // Generate a random poll id
            _pollId = Guid.NewGuid().ToString("N");

            // Add poll id to the parameters
            parameters.Add("poll_id", _pollId);
        }

        public async Task<string> GetAuthorizeCode(Uri authorizeUri)
        {
            if (string.IsNullOrWhiteSpace(_settings.ClientId) || string.IsNullOrWhiteSpace(_settings.ClientSecret))
            {
                await _messages.AddMessageAsync(Message.ForAction(
                    "BadClientCredentials",
                    "Client ID and Client Secret are not set correctly. Click here to open settings.",
                    () => AASettings.Instance.Show(_host.MainForm),
                    () => _settings.TokenResponse.IsValidForAuth()));
                return null;
            }

            return await _uiThreadInvoke.Invoke(() => UiTask.ShowDialog(async cancellationToken =>
                {
                    var tokens = await _authClient.GetTokenFromClientCredentials(cancellationToken);
                    Process.Start(authorizeUri.ToString());

                    string code;
                    do
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                        code = await _authClient.GetAuthorizationCode(tokens.AccessToken, _pollId, cancellationToken);
                    } while (!cancellationToken.IsCancellationRequested && code == null);

                    return code;
                },
                "Opening a browser to sign in to Altitude Angel. Please sign in using the browser."));
        }
    }
}
