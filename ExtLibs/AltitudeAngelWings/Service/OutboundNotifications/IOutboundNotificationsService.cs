using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.OutboundNotifications
{
    public interface IOutboundNotificationsService
    {
        Task StartWebSocket(CancellationToken cancellationToken = default);

        Task StopWebSocket(CancellationToken cancellationToken = default);
    }
}
