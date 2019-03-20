using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace AltitudeAngelWings
{
    public class ObservableProperty<T> : INotifyPropertyChanged, IObservable<T>, IDisposable
    {
        private readonly ISubject<T> _subject;
        private T _value;

        public ObservableProperty()
            : this(1)
        {
        }

        public ObservableProperty(int replayCount)
            : this(default(T), replayCount)
        {
        }

        public ObservableProperty(T initialValue)
            : this(initialValue, 1)
        {
        }

        public ObservableProperty(T initialValue, int replayCount)
        {
            _subject = new ReplaySubject<T>(replayCount);
            Value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                SendChangeNotifications();
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public static implicit operator T(ObservableProperty<T> obj)
        {
            return obj.Value;
        }

        private void SendChangeNotifications()
        {
            OnPropertyChanged(string.Empty);
            _subject.OnNext(_value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)_subject)?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
