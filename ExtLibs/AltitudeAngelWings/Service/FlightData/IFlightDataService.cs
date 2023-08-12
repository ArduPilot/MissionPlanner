using System;

namespace AltitudeAngelWings.Service.FlightData
{
    public interface IFlightDataService : IDisposable
    {
        IObservable<Model.FlightData> FlightArmed { get; }
        IObservable<Model.FlightData> FlightDisarmed { get; }
        IObservable<Model.FlightData> ArmedFlightData { get; }
        IObservable<Model.FlightData> RawFlightData { get; }
    }
}