
import clr
import MissionPlanner
clr.AddReference("MAVLink")
from System import Byte
import MAVLink
from MAVLink import mavlink_command_long_t

import MAVLink

print 'Start Script'

MAV.doCommand(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 0, 0, 0, 0, 0, 0);

MAV.doCommand(MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, MAVLink.MAV_MOUNT_MODE.NEUTRAL.value__, 0, 0, 0, 0, 0, 0);

MAV.setDigicamControl(True);

MAV.doARM(True);

#MAV.doReboot(False);

MAV.setWPCurrent(1);

MAV.GetParam("PARAMNAME");

MAV.setParam("PARM",1.0);

MAV.getWPCount();

MAV.getWP(1);

MAV.getParamList();

commandlong = mavlink_command_long_t()
mavlink_command_long_t.target_system.SetValue(commandlong,71)
mavlink_command_long_t.target_component.SetValue(commandlong,67)
mavlink_command_long_t.param1.SetValue(commandlong, MAVLink.MAV_MOUNT_MODE.NEUTRAL.value__)
mavlink_command_long_t.command.SetValue(commandlong, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL.value__)

# command , target sysid, target compid    used to keep track of the remote state
MAV.sendPacket(commandlong, 71, 67)
