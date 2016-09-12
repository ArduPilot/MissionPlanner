using System;
using System.Collections.Generic;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class ApiOAuthClientHandler
    {

        /// <summary>
        ///     Create an HTTP Handler for client only auth.
        /// </summary>
        /// <param name="authBaseUri">The base auth URI e.g. https://auth.alitudeangel.com</param>
        /// <param name="clientId">Your client ID</param>
        /// <param name="clientSecret">Your client secret</param>
        /// <param name="scopes">Requested scopes</param>
        /// <param name="existingState">(optional) An existing state object from a previous session. May be null.</param>
        public static ClientHandlerInfo Create(string authBaseUri,
        string clientId,
            string clientSecret,
            IEnumerable<string> scopes,
            AuthorizationState existingState = null
            )
        {
            return Create(authBaseUri, clientId, clientSecret, scopes, existingState, false, null, null);
        }

        /// <summary>
        ///     Create an HTTP Handler that supports OAuth user authentication.
        /// </summary>
        /// <param name="authBaseUri">The base auth URI e.g. https://auth.alitudeangel.com</param>
        /// <param name="clientId">Your client ID</param>
        /// <param name="clientSecret">Your client secret</param>
        /// <param name="scopes">Requested scopes</param>
        /// <param name="existingState">(optional) An existing state object from a previous session. May be null.</param>
        /// <param name="requireUserToken">true to aquire a user token, false to get a client only token.</param>
        /// <param name="redirectUri">The redirect URI to use for user token auth. Must match the registered URI for your client ID.</param>
        /// <param name="codeProvider">Implementation to use to get an authorization code URI from an auth login URI.</param>
        /// <returns>
        ///     A <see cref="ClientHandlerInfo"/> object that contains the auth state and the handler. The auth state may be persisted and passed
        ///     back in on future runs of the application to save login state.
        /// </returns>
        public static ClientHandlerInfo Create(
            string authBaseUri,
            string clientId,
            string clientSecret,
            IEnumerable<string> scopes,
            AuthorizationState existingState,
            bool requireUserToken,
            string redirectUri,
            IAuthorizeCodeProvider codeProvider)
        {
            AuthorizationServerDescription serverDescription = GetServerDescription(authBaseUri);
            ClientBase client;
            IAuthorizationState state = existingState;

            if (requireUserToken)
            {
                if (codeProvider == null || string.IsNullOrEmpty(redirectUri))
                {
                    throw new ArgumentNullException(nameof(codeProvider),
                        $"{nameof(codeProvider)} or {nameof(redirectUri)} cannot be null if {nameof(requireUserToken)} is true.");
                }

                var userClient = new UserAgentClient(serverDescription, clientId, ClientCredentialApplicator.PostParameter(clientSecret));
                if (state == null)
                {
                    // Open browser here
                    var returnTo = new Uri(redirectUri);
                    Uri uri = userClient.RequestUserAuthorization(scopes, returnTo: returnTo);
                    Uri result = codeProvider.GetCodeUri(uri, returnTo).Result;

                    state = new AuthorizationState {Callback = returnTo};
                    state.Scope.AddRange(scopes);
                    state = userClient.ProcessUserAuthorization(result, state);
                }

                client = userClient;
            }
            else
            {
                client = new WebServerClient(serverDescription, clientId, ClientCredentialApplicator.PostParameter(clientSecret));
                state = state ?? client.GetClientAccessToken(scopes);
            }

            return new ClientHandlerInfo(client.CreateAuthorizingHandler(state), state);
        }

        private static AuthorizationServerDescription GetServerDescription(string authBaseUri)
        {
            authBaseUri = authBaseUri.TrimEnd('/');

            return new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri($"{authBaseUri}/oauth/v2/authorize"),
                TokenEndpoint = new Uri($"{authBaseUri}/oauth/v2/token")
            };
        }
    }
}
