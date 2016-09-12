using System;

namespace AltitudeAngelWings.Views
{
    public interface IView<T> where T : class
    {
        T ViewModel { get; set; }
        void ViewInitialized();
    }
}
