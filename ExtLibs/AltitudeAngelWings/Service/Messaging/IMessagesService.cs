using System.Threading.Tasks;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public interface IMessagesService
    {
        ObservableProperty<Message> Messages { get; }
        Task AddMessageAsync(Message message);
    }
}
