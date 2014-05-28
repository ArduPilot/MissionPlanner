using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Comms
{
    class MAVLinkSerialPort: MissionPlanner.Comms.SerialPort
    {
        private MissionPlanner.portproxy portproxy;
        private int p;

        public MAVLinkSerialPort(MissionPlanner.portproxy portproxy, int p)
        {
            // TODO: Complete member initialization
            this.portproxy = portproxy;
            this.p = p;
        }
    }
}
