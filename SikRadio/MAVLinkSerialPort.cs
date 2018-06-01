namespace MissionPlanner.Comms
{
    internal class MAVLinkSerialPort : SerialPort
    {
        private int p;
        private portproxy portproxy;

        public MAVLinkSerialPort(portproxy portproxy, int p)
        {
            // TODO: Complete member initialization
            this.portproxy = portproxy;
            this.p = p;
        }
    }
}