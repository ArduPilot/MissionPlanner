using MissionPlanner.Comms;

namespace Xamarin
{
    public class MonoSerialPort : System.IO.Ports.MonoSerialPort, ICommsSerial
    {
        public MonoSerialPort()
        {
        }

        public MonoSerialPort(string portName) : base(portName)
        {
        }

        public MonoSerialPort(string portName, int baudRate) : base(portName, baudRate)
        {
        }

        public void toggleDTR()
        {
            
        }
    }
}