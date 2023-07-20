using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MessagesService : IMessagesService, IDisposable
    {
        public MessagesService(IMessageDisplay messageDisplay)
        {
            Messages = new ObservableProperty<Message>(0);
            Messages
                .Do(messageDisplay.AddMessage)
                .SelectMany(m => Observable.Interval(TimeSpan.FromMilliseconds(100))
                    .SkipWhile(i => !m.HasExpired())
                    .Select(i => m))
                .Subscribe(messageDisplay.RemoveMessage);
        }

        public ObservableProperty<Message> Messages { get; }

        public Task AddMessageAsync(Message message) => Task.Factory.StartNew(() => Messages.Value = message);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Messages?.Dispose();
            }
        }
    }
}
