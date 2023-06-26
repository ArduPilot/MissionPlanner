using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.OutboundNotifs
{
    public interface IOutboundNotifsService
    {
        Task StartWebSocket();

        Task StopWebSocket();

    }
}
