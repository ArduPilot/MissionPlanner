using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.OutboundNotifications
{
    public interface IOutboundNotificationsService
    {
        Task StartWebSocket();

        Task StopWebSocket();
    }
}
