using AltitudeAngelWings.Clients.Flight.Model;

namespace AltitudeAngelWings.Clients.OutboundNotifications.Model
{
    public class LoiterNotificationProperties
    {
        public double Latitude
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }

        public Altitude Altitude
        {
            get;
            set;
        }
    }
}
