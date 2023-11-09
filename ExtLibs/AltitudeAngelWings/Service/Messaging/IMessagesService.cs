using System.Threading.Tasks;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public interface IMessagesService
    {
        Task AddMessageAsync(Message message);
    }
}
