using System;

namespace AltitudeAngelWings.Models
{
    public class FlightData
    {
        public FlightData()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public FlightData(FlightData flightData)
        {
            Armed = flightData.Armed;
            CurrentPosition = flightData.CurrentPosition;
            HomePosition = flightData.HomePosition;

            TimeStamp = DateTime.UtcNow;
        }

        public bool Armed { get; set; }
        public FlightDataPosition CurrentPosition { get; set; }
        public FlightDataPosition HomePosition { get; set; }
        public DateTime TimeStamp { get; private set; }
    }
}
