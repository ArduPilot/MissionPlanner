using System;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    internal class AltitudeAngel : IDisposable
    {
        public static MissionPlannerAdaptor MP = new MissionPlannerAdaptor(
            () => FlightPlanner.instance.GetFlightPlanLocations());
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