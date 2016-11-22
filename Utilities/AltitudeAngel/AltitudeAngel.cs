using System;
using System.Text;
using System.Windows.Forms;
using AltitudeAngelWings;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Service;
using AltitudeAngelWings.Service.FlightData;
using AltitudeAngelWings.Service.FlightData.Providers;
using AltitudeAngelWings.Service.Messaging;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities.AltitudeAngel
{
    public class AltitudeAngel : IDisposable
    {
        public static MissionPlannerAdaptor MP = new MissionPlannerAdaptor();
        private static MessagesService Message = new MessagesService();

        public static AltitudeAngelService service = null;

        static AltitudeAngel()
        {
            service = new AltitudeAngelService(Message, MP,
                new FlightDataService(new ObservableProperty<long>(3), new MissionPlannerFlightDataProvider(null)));
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}