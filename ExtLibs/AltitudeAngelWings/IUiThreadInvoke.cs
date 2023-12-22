using System;
using System.Threading.Tasks;

namespace AltitudeAngelWings
{
    public interface IUiThreadInvoke
    {
        void Invoke(Action action);
        Task<T> Invoke<T>(Func<T> action);
    }
}