using System;
using System.Threading.Tasks;

namespace AltitudeAngelWings
{
    public interface IUiThreadInvoke
    {
        void FireAndForget(Action action);
        Task<T> Invoke<T>(Func<T> action);
    }
}