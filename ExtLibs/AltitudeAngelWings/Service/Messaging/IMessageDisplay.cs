using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public interface IMessageDisplay
    {
        void AddMessage(Message message);
        void RemoveMessage(Message message);
    }
}