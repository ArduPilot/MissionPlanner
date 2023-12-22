using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Auth.Model
{
    /// <summary>
    /// A token response for an OAuth endpoint
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// The access token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = "";

        /// <summary>
        /// The refresh token that can be used to request 
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = "";

        /// <summary>
        /// the number of seconds that the Access Token expires in
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The type of access token, eg 'Bearer' or 'Basic'
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "";
    }
}