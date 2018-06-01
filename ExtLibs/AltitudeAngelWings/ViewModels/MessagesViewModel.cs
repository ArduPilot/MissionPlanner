using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using AltitudeAngelWings.Service.Messaging;

namespace AltitudeAngelWings.ViewModels
{
    public class MessagesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set
            {
                if (value != _messages)
                {
                    _messages = value;
                    OnPropertyChanged();
                }
            }
        }

        public MessagesViewModel()
        {
            CreateSampleData();
        }

        public MessagesViewModel(IMessagesService messagesService)
        {
            Messages = new ObservableCollection<string>();

            messagesService.Messages
                            .ObserveOnDispatcher()
                            .Subscribe(message => Messages.Add(message.Content));
        }

        private void CreateSampleData()
        {
            Messages = new ObservableCollection<string>
            {
                "Message 1",
                "Message 2",
                "Message 3"
            };
        }

        private ObservableCollection<string> _messages;

        #region INotifyPropertyChanged/Changing

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
