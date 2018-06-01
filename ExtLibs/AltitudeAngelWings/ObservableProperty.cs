using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace AltitudeAngelWings
{
    public class ObservableProperty<T> : INotifyPropertyChanged, IObservable<T>
    {
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SendChangeNotifications();
            }
        }

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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public static implicit operator T(ObservableProperty<T> obj)
        {
            return obj.Value;
        }

        private bool HasChanged(T oldValue, T newValue)
        {
            return !(((oldValue != null) && oldValue.Equals(newValue)) ||
                     (oldValue == null) && (newValue == null));
        }

        private void SendChangeNotifications()
        {
            OnPropertyChanged(string.Empty);
            _subject.OnNext(_value);
        }


        private readonly ISubject<T> _subject;
        private T _value;

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
