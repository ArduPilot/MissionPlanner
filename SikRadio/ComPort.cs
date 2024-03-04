using System;
using MissionPlanner.Comms;

namespace MissionPlanner.Radio
{
    public static class ComPort
    {
        public static ICommsSerial GetComPortForSiKRadio()
        {
            return SikRadio.Config.comPort;
        }

        public static void FinishedWithComPortForSiKRadio()
        {
        }
    }

}