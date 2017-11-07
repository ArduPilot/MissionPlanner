using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using MissionPlanner.Utilities;
using MissionPlanner;

namespace MissionPlanner.Swarm
{
    abstract class Swarm
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal MAVLinkInterface Leader = null;

        public void setLeader(MAVLinkInterface lead)
        {
            Leader = lead;
        }

        public MAVLinkInterface getLeader()
        {
            return Leader;
        }

        public void Arm()
        {
            foreach (var port in MainV2.Comports)
            {
                if (port == Leader)
                    continue;

                port.doARM(true);
            }
        }

        public void Disarm()
        {
            foreach (var port in MainV2.Comports)
            {
                if (port == Leader)
                    continue;

                port.doARM(false);
            }
        }

        public void Takeoff()
        {
            foreach (var port in MainV2.Comports)
            {
                if (port == Leader)
                    continue;

                port.setMode("GUIDED");

                port.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 5);

                /*if (port.MAV.cs.lat != 0 && port.MAV.cs.lng != 0)
                    port.setGuidedModeWP(new Locationwp() {alt = 5, lat = port.MAV.cs.lat, lng = port.MAV.cs.lng});
                 */
            }
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