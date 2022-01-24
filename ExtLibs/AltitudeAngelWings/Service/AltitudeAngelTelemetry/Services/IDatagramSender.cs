
namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services
{
    public interface IDatagramSender
    {
        void SendToServer(byte[] data);


        string Host
        {
            get; set;
        }

        int PortNumber
        {
            get; set;
        }

    }
}
