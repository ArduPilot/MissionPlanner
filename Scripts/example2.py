
import clr
import MissionPlanner
clr.AddReference("MAVLink")

import MAVLink

print 'Start Script'

MAV.doCommand(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 0, 0);

MAV.doARM(true);

MAV.doReboot(false);

MAV.setWPCurrent(1);

MAV.GetParam("PARAMNAME");

MAV.setParam("PARM",1.0);

MAV.getWPCount();

MAV.getWP(1);

MAV.getParamList();
