using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.ApiClient.Client
{
    public interface ITokenProvider
    {
        Task<string> GetToken(CancellationToken cancellationToken);
    }
}