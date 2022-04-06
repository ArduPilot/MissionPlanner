using AltitudeAngelWings.ApiClient.Models;
using System;

namespace AltitudeAngelWings.Service.FlightService
{
    public interface IFlightService: IDisposable
    {
        UserProfileInfo CurrentUser { get; }
    }
}
