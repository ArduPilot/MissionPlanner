using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AltitudeAngelWings.Commands
{
    internal class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> executeAction)
        {
            _executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _executeAction((T) parameter);
        }

        public event EventHandler CanExecuteChanged;
        private readonly Action<T> _executeAction;
    }

    internal class DelegateCommandAsync<T> : ICommand
    {
        public DelegateCommandAsync(Func<T, Task> executeFunc, bool disableOnWait = false)
        {
            _executeFunc = executeFunc;
            _disableOnWait = disableOnWait;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            Task task = _executeFunc((T) parameter);

            if (_disableOnWait)
            {
                _canExecute = false;
                OnCanExecuteChanged();

                task.ContinueWith(r =>
                {
                    if (r.IsFaulted)
                    {
                        Debug.WriteLine(r.Exception.Message);
                    }

                    _canExecute = true;
                    OnCanExecuteChanged();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }


        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private readonly bool _disableOnWait;
        private readonly Func<T, Task> _executeFunc;
        private bool _canExecute = true;
    }
}
