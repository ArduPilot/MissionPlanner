using System;
using System.Threading.Tasks;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MessagesService : IMessagesService
    {
        private readonly IMessageDisplay _messageDisplay;

        public MessagesService(IMessageDisplay messageDisplay)
        {
            _messageDisplay = messageDisplay;
        }

        public Task AddMessageAsync(Message message) => Task.Factory.StartNew(async () =>
        {
            try
            {
                _messageDisplay.AddMessage(message);
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
                } while (!message.HasExpired());
            }
            finally
            {
                _messageDisplay.RemoveMessage(message);
            }
        });
    }
}
