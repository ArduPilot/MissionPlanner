using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace AltitudeAngelWings.Clients.Auth.Model
{
    public static class TokenResponseExtensions
    {
        private static JwtSecurityToken GetJwt(string accessToken)
        {
            try
            {
                return new JwtSecurityToken(accessToken);
            }
            catch (Exception)
            {
                // Ignore
                return null;
            }
        }

        public static string[] AccessTokenScopes(this TokenResponse tokenResponse)
        {
            if (!tokenResponse.HasAccessToken())
            {
                return Array.Empty<string>();
            }

            var token = GetJwt(tokenResponse.AccessToken);
            if (token == null || !token.Payload.TryGetValue("urn:oauth:scope", out var value))
            {
                return Array.Empty<string>();
            }

            var scopes = new List<string>();
            switch (value)
            {
                case string str:
                    scopes.Add(str);
                    break;
                case IEnumerable<object> values:
                    scopes.AddRange(values.Select(item => item.ToString()));
                    break;
                default:
                    scopes.Add(JsonConvert.SerializeObject(value));
                    break;
            }

            scopes.Sort();
            return scopes.ToArray();
        }

        public static bool IsValidForAuth(this TokenResponse tokenResponse)
        {
            if (!tokenResponse.HasAccessToken()) return false;

            var token = GetJwt(tokenResponse.AccessToken);
            if (token?.Payload.Exp == null)
            {
                return false;
            }

            // Only valid if >= 1 minute of expiry time left
            var expires = DateTimeOffset.FromUnixTimeSeconds(token.Payload.Exp.Value);
            return expires > DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(1));
        }

        private static bool HasAccessToken(this TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(tokenResponse.AccessToken);
        }

        public static bool CanBeRefreshed(this TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(tokenResponse.RefreshToken);
        }

        public static bool HasScopes(this TokenResponse tokenResponse, params string[] scopesToCheck)
        {
            if (tokenResponse == null)
            {
                return false;
            }

            var scopes = tokenResponse.AccessTokenScopes();
            return scopesToCheck.All(c => scopes.Contains(c));
        }
    }
}