using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.Messaging;
using Flurl;
using Flurl.Http;
using MissionPlanner.Plugin;
using Newtonsoft.Json.Linq;
using Polly;

namespace AltitudeAngelWings.Plugin
{
    public class ExternalWebBrowserAuthorizeCodeProvider : IAuthorizeCodeProvider
    {
        private readonly ISettings _settings;
        private readonly IAsyncPolicy _asyncPolicy;
        private readonly IMessagesService _messages;
        private readonly PluginHost _host;
        private readonly IUiThreadInvoke _uiThreadInvoke;
        private readonly ProductInfoHeaderValue _version;
        private string _pollId;

        public ExternalWebBrowserAuthorizeCodeProvider(ISettings settings, IAsyncPolicy asyncPolicy, IMessagesService messages, PluginHost host, IUiThreadInvoke uiThreadInvoke, ProductInfoHeaderValue version)
        {
            _settings = settings;
            _asyncPolicy = asyncPolicy;
            _messages = messages;
            _host = host;
            _uiThreadInvoke = uiThreadInvoke;
            _version = version;
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
                var message = new Message("Client ID and Client Secret are not set correctly. Click here to open settings.")
                {
                    Key = "BadClientCredentials",
                    OnClick = () => AASettings.Instance.Show(_host.MainForm)
                };
                message.HasExpired = () => message.Clicked || _settings.TokenResponse.IsValidForAuth();
                await _messages.AddMessageAsync(message);
                return null;
            }

            return await _uiThreadInvoke.Invoke(() => UiTask.ShowDialog(async cancellationToken =>
                {
                    using (var client = new FlurlClient
                           {
                               Settings =
                               {
                                   BeforeCall = call => call.HttpRequestMessage.Headers.UserAgent.Add(_version)
                               }
                           })
                    {
                        var token = await GetClientTokenForLoginPoll(client, cancellationToken);
                        Process.Start(authorizeUri.ToString());

                        string code;
                        do
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                            code = await PollForAuthenticationCode(client, token, cancellationToken);
                        } while (!cancellationToken.IsCancellationRequested && code == null);

                        return code;
                    }
                },
                "Opening a browser to sign in to Altitude Angel. Please sign in using the browser."));
        }

        private async Task<string> GetClientTokenForLoginPoll(IFlurlClient client, CancellationToken cancellationToken)
        {
            using (var response = await _asyncPolicy.ExecuteAsync(() =>
                       _settings.AuthenticationUrl
                           .AppendPathSegments("oauth", "v2", "token")
                           .PostUrlEncodedAsync(new
                           {
                               client_id = _settings.ClientId,
                               client_secret = _settings.ClientSecret,
                               grant_type = "client_credentials",
                               token_type = "jwt"
                           }, cancellationToken)))
            {
                return (string)JObject.Parse(await response.GetStringAsync())["access_token"];
            };
        }

        private async Task<string> PollForAuthenticationCode(IFlurlClient client, string token, CancellationToken cancellationToken)
        {
            using (var response = await _asyncPolicy.ExecuteAsync(() =>
                       _settings.AuthenticationUrl
                           .AppendPathSegments("api", "v1", "security", "get-login")
                           .SetQueryParam("id", _pollId)
                           .WithHeader("Authorization", $"Bearer {token}")
                           .AllowHttpStatus(HttpStatusCode.NotFound)
                           .GetAsync(cancellationToken)))
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