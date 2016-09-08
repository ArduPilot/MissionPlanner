using System;
using System.Threading.Tasks;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MessagesService : IMessagesService
    {
        public MessagesService()
        {
            Messages = new ObservableProperty<Message>(0);
        }

        public ObservableProperty<Message> Messages { get; }

        public Task AddMessageAsync(Message message)
        {
            return Task.Factory.StartNew(() => Messages.Value = message);
        }
    }
}
