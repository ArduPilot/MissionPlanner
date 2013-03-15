using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace ArdupilotMega.Swarm
{
    abstract class Swarm
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal MAVLink Leader = null;

        public void setLeader(MAVLink lead)
        {
            Leader = lead;
        }

        public MAVLink getLeader()
        {
            return Leader;
        }

        public void Arm()
        {
            foreach (var port in MainV2.Comports)
            {
                port.doARM(true);
            }
        }

        public void Disarm()
        {
            foreach (var port in MainV2.Comports)
            {
                port.doARM(false);
            }
        }

        public void Takeoff()
        {

        }

        public void Land()
        {
            foreach (var port in MainV2.Comports)
            {
                port.setMode("Land");
            }
        }

        public void Stop()
        {

        }

        public abstract void Update();

        public abstract void SendCommand();
    }
}
