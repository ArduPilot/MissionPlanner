using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface IAuthClient
    {
        Task<TokenResponse> GetTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<TokenResponse> GetTokenFromAuthorizationCode(string authorizationCode, CancellationToken cancellationToken);
        Task<TokenResponse> GetTokenFromClientCredentials(CancellationToken cancellationToken);
        Task<string> GetAuthorizationCode(string accessToken, string pollId, CancellationToken cancellationToken);
    }
}