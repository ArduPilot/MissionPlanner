using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MessagesService : IMessagesService, IDisposable
    {
        private readonly CompositeDisposable _disposer = new CompositeDisposable();

        public MessagesService(IMessageDisplay messageDisplay)
        {
            Messages = new ObservableProperty<Message>(0);
            _disposer.Add(Messages);
            _disposer.Add(Messages
                .Do(messageDisplay.AddMessage)
                .SelectMany(m => Observable.Interval(TimeSpan.FromMilliseconds(100))
                    .SkipWhile(i => !m.HasExpired())
                    .Select(i => m))
                .Subscribe(messageDisplay.RemoveMessage));
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
                _disposer?.Dispose();
            }
        }
    }
}
