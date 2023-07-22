using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using MissionPlanner.Plugin;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.Plugin
{
    public class ExternalWebBrowserAuthorizeCodeProvider : IAuthorizeCodeProvider
    {
        private readonly ISettings _settings;
        private readonly IMessagesService _messages;
        private readonly PluginHost _host;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private string _pollId;
        private readonly FlurlClient _client;

        public ExternalWebBrowserAuthorizeCodeProvider(ISettings settings, IHttpClientFactory clientFactory, IMessagesService messages, PluginHost host, IUiThreadInvoke uiThreadInvoke)
        {
            _settings = settings;
            _messages = messages;
            _host = host;
            _uiThreadInvoke = uiThreadInvoke;
            _client = new FlurlClient
            {
                Settings =
                {
                    HttpClientFactory = clientFactory,
                }
            };
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
                    var token = await GetClientTokenForLoginPoll(_client, cancellationToken);
                    Process.Start(authorizeUri.ToString());

                    string code;
                    do
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                        code = await PollForAuthenticationCode(_client, token, cancellationToken);
                    } while (!cancellationToken.IsCancellationRequested && code == null);

                    return code;
                },
                "Opening a browser to sign in to Altitude Angel. Please sign in using the browser."));
        }

        private async Task<string> GetClientTokenForLoginPoll(IFlurlClient client, CancellationToken cancellationToken)
        {
            using (var response = await _settings.AuthenticationUrl
                           .AppendPathSegments("oauth", "v2", "token")
                           .WithClient(_client)
                           .PostUrlEncodedAsync(new
                           {
                               client_id = _settings.ClientId,
                               client_secret = _settings.ClientSecret,
                               grant_type = "client_credentials",
                               token_type = "jwt"
                           }, cancellationToken))
            {
                return (string)JObject.Parse(await response.GetStringAsync())["access_token"];
            };
        }

        private async Task<string> PollForAuthenticationCode(IFlurlClient client, string token, CancellationToken cancellationToken)
        {
            using (var response = await _settings.AuthenticationUrl
                           .AppendPathSegments("api", "v1", "security", "get-login")
                           .SetQueryParam("id", _pollId)
                           .WithHeader("Authorization", $"Bearer {token}")
                           .AllowHttpStatus(HttpStatusCode.NotFound)
                           .WithClient(_client)
                           .GetAsync(cancellationToken))
            {
                if (response.StatusCode != (int)HttpStatusCode.OK)
                {
                    return null;
                }

                return (string)JObject.Parse(await response.GetStringAsync())["code"];
            }
        }
    }
}