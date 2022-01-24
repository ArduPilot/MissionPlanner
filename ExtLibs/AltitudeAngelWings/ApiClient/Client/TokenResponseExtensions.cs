using System;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class TokenResponseExtensions
    {
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