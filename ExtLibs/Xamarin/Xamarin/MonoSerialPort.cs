using MissionPlanner.Comms;

namespace Xamarin
{
    public class MonoSerialPort : System.IO.Ports.MonoSerialPort, ICommsSerial
    {
        public void toggleDTR()
        {
            
        }
    }
}