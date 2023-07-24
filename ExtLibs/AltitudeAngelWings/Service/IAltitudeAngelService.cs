using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Api.Model;

namespace AltitudeAngelWings.Service
{
    public interface IAltitudeAngelService : IDisposable
    {
        ObservableProperty<bool> IsSignedIn { get; }
        bool SigningIn { get; }
        ObservableProperty<WeatherInfo> WeatherReport { get; }
        IList<FilterInfoDisplay> FilterInfoDisplay { get; }
        Task SignInAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        void ProcessAllFromCache(IMap map, bool resetFilters = false);
    }
}