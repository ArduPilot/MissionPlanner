using System;

namespace AltitudeAngelWings.Service.FlightData
{
    public interface IFlightDataService : IDisposable
    {
        IObservable<Models.FlightData> FlightArmed { get; }
        IObservable<Models.FlightData> FlightDisarmed { get; }
        IObservable<Models.FlightData> ArmedFlightData { get; }
        IObservable<Models.FlightData> RawFlightData { get; }
    }
}