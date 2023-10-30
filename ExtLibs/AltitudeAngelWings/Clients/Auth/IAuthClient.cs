using System;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Auth.Model;

namespace AltitudeAngelWings.Clients.Auth
{
    public interface IAuthClient : IDisposable
    {
        Task<TokenResponse> GetTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<TokenResponse> GetTokenFromAuthorizationCode(string authorizationCode, CancellationToken cancellationToken);
        Task<TokenResponse> GetTokenFromClientCredentials(CancellationToken cancellationToken);
        Task<string> GetAuthorizationCode(string accessToken, string pollId, CancellationToken cancellationToken);
        Task<UserProfileInfo> GetUserProfile(string accessToken, CancellationToken cancellationToken);
    }
}