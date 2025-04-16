using log4net;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.Swarm
{
    abstract class Swarm
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal MAVState Leader = null;

        public void setLeader(MAVState lead)
        {
            Leader = lead;
        }

        public MAVState getLeader()
        {
            return Leader;
        }

        public void Arm()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    port.doARM(mav.sysid, mav.compid, true);
                }
            }
        }

        public void Disarm()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    port.doARM(mav.sysid, mav.compid, false);
                }
            }
        }

        public void Takeoff()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    port.setMode(mav.sysid, mav.compid, "GUIDED");

                    port.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 5);
                }
            }
        }

        public void Land()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    port.setMode(mav.sysid, mav.compid, "Land");
                }
            }
        }

        public void Stop()
        {
        }

        public void GuidedMode()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    port.setMode(mav.sysid, mav.compid, "GUIDED");
                }
            }
        }

        public void AutoMode()
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == Leader)
                        continue;

                    port.setMode(mav.sysid, mav.compid, "AUTO");
                }
            }
        }


        public void Arm_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical)
                    {
                        if (mav == Leader)
                            continue;
                    }

                    port.doARM(mav.sysid, mav.compid, true);
                }
            }
        }

        public void Disarm_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical)
                    {
                        if (mav == Leader)
                            continue;
                    }

                    port.doARM(mav.sysid, mav.compid, false);
                }
            }
        }

        public void Takeoff_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical)
                    {
                        if (mav == Leader)
                            continue;
                    }

                    port.setMode(mav.sysid, mav.compid, "GUIDED");

                    port.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 5);
                }
            }
        }

        public void Land_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    port.setMode(mav.sysid, mav.compid, "Land");
                }
            }
        }

        public void Rtl_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    port.setMode(mav.sysid, mav.compid, "RTL");
                }
            }
        }
        public async void Rtl_successively_ALL(bool vertical,int sleep_time)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                   
                   
                    port.setMode(mav.sysid, mav.compid, "RTL");
                    await Task.Delay(sleep_time*1000);  // 延迟3000毫秒（即3秒）
                }
            }
        }
        

        public void GuidedMode_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical)
                    {
                        if (mav == Leader)
                            continue;
                    }

                    port.setMode(mav.sysid, mav.compid, "GUIDED");
                }
            }
        }

        public void AutoMode_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical) {
                        if (mav == Leader)
                            continue;
                    }
                    port.setMode(mav.sysid, mav.compid, "AUTO");
                }
            }
        }
        public void Brake_ALL(bool vertical)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (!vertical)
                    {
                        if (mav == Leader)
                            continue;
                    }
                    port.setMode(mav.sysid, mav.compid, "Brake");
                }
            }
        }

        public abstract void Update();

        public abstract void SendCommand();
    }
}