using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.FlightData.Providers;

namespace AltitudeAngelWings.Service.FlightData
{
    public class FlightDataService : IDisposable
    {
        public IObservable<Models.FlightData> FlightArmed { get; }
        public IObservable<Models.FlightData> FlightDisarmed { get; }
        public IObservable<Models.FlightData> ArmedFlightData { get; }
        public IObservable<Models.FlightData> RawFlightData => _rawFlightData;

        public FlightDataService(
            IObservable<long> pollInterval,
            IFlightDataProvider flightDataProvider)
        {
            _flightDataProvider = flightDataProvider;

            _rawFlightData = pollInterval
                .Select(i => _flightDataProvider.GetCurrentFlightData())
                .Publish();

            FlightArmed = _rawFlightData
                .DistinctUntilChanged(i => i.Armed)
                .Where(i => i.Armed)
                .Select(flightData => new Models.FlightData(flightData) {HomePosition = flightData.CurrentPosition});

            FlightDisarmed = _rawFlightData
                .DistinctUntilChanged(i => i.Armed)
                .Where(i => !i.Armed);

            ArmedFlightData = _rawFlightData
                .Where(i => i.Armed)
                .Select(flightData => new Models.FlightData(flightData) {HomePosition = _homePosition});
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Initialize()
        {
            FlightArmed
                .Subscribe(i => _homePosition = i.CurrentPosition);

            FlightDisarmed
                .Subscribe(i => _homePosition = null, () => _homePosition = null);

            _rawFlightDataSubscription = _rawFlightData.Connect();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _rawFlightDataSubscription?.Dispose();
                _rawFlightDataSubscription = null;
            }
        }

        private readonly IFlightDataProvider _flightDataProvider;
        private readonly IConnectableObservable<Models.FlightData> _rawFlightData;
        private FlightDataPosition _homePosition;
        private IDisposable _rawFlightDataSubscription;
    }
}
