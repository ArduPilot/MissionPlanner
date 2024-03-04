using System;
using MissionPlanner.Comms;

namespace MissionPlanner.Radio
{
    public static class ComPort
    {
        static ICommsSerial _Port;

        public static ICommsSerial GetComPortForSiKRadio()
        {
            if (_Port == null)
            {
                MainV2.comPort.Close();
                MissionPlanner.Radio.Sikradio.Connect(ref _Port);
            }

            return _Port;
        }

        public static void FinishedWithComPortForSiKRadio()
        {
            if (_Port != null)
            {
                _Port.Dispose();
                _Port = null;
            }
        }
    }

}