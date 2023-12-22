using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Clients
{
    public interface ITokenProvider
    {
        Task<string> GetToken(CancellationToken cancellationToken);
    }
}