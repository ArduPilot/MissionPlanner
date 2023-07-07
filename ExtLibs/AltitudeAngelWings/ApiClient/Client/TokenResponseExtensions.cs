using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class TokenResponseExtensions
    {
        public static string[] AccessTokenScopes(this TokenResponse tokenResponse)
        {
            var token = new JwtSecurityToken(tokenResponse.AccessToken);
            if (!token.Payload.TryGetValue("urn:oauth:scope", out var value))
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
                    scopes.Add(JsonExtensions.SerializeToJson(value));
                    break;
            }

            scopes.Sort();
            return scopes.ToArray();
        }

        public static bool IsValidForAuth(this TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(tokenResponse.AccessToken)
                || tokenResponse.ExpiresIn <= 0)
            {
                return false;
            }

            // Only valid if >= 1 minute of expiry time left
            return tokenResponse.ExpiresAt >= DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(1));
        }

        public static bool CanBeRefreshed(this TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(tokenResponse.RefreshToken);
        }

    }
}