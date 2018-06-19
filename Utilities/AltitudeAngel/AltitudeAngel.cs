using System;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Messaging;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class AltitudeAngel : IDisposable
    {
        public static MissionPlannerAdaptor MP = new MissionPlannerAdaptor();
        private static MessagesService Message = new MessagesService();

        public static AltitudeAngelService service = null;

        static AltitudeAngel()
        {
            service = new AltitudeAngelService(
                Message,
                MP,
                new FlightDataService(
                    TimeSpan.FromMilliseconds(500),
                    new MissionPlannerFlightDataProvider(
                        new MissionPlannerStateAdapter(() => MainV2.comPort.MAV.cs))));
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}