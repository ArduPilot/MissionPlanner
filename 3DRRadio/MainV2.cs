using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Comms;

namespace ArdupilotMega
{
    class MainV2
    {
        public static portproxy comPort = new portproxy();
    }

    class portproxy
    {
        public SerialPort BaseStream = new SerialPort();
    }
}
