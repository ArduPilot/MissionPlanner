using System;
using System.Threading.Tasks;

namespace AltitudeAngelWings
{
    public class UiThreadInvoke : IUiThreadInvoke
    {
        private readonly Func<Func<object>, Task<object>> _invokeOnUiThread;

        public UiThreadInvoke(Func<Func<object>, Task<object>> invokeOnUiThread)
        {
            _invokeOnUiThread = invokeOnUiThread;
        }

        public void FireAndForget(Action action)
        {
            Invoke<object>(() =>
            {
                action();
                return null;
            });
        }

        public Task<T> Invoke<T>(Func<T> action)
        {
            return _invokeOnUiThread(() => action())
                .ContinueWith(task => (T)task.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}