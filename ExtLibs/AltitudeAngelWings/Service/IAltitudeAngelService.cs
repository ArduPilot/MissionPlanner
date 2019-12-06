using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Models;
using AltitudeAngelWings.Extra;

namespace AltitudeAngelWings.Service
{
    public interface IAltitudeAngelService : IDisposable
    {
        ObservableProperty<bool> IsSignedIn { get; }
        ObservableProperty<WeatherInfo> WeatherReport { get; }
        ObservableProperty<Unit> SentTelemetry { get; }
        UserProfileInfo CurrentUser { get; }
        IList<string> FilteredOut { get; }
        Task SignInAsync();
        Task DisconnectAsync();
        void ProcessAllFromCache(IMap map);
        Task SignInIfAuthenticated();
    }
}