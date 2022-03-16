
import clr
import MissionPlanner
clr.AddReference("MAVLink")
from System import Byte
from System import Func
import MAVLink
from MAVLink import mavlink_command_long_t

import MAVLink

print 'Start Script'

MAV.setParam("INS_ACC_ID",	0, True);
MAV.setParam("INS_ACC2_ID",	0, True);
MAV.setParam("INS_ACC3_ID",	0, True);

MAV.setParam("INS_ACC_ID",	3081250, True);
MAV.setParam("INS_ACC2_ID",	2883874, True);
MAV.setParam("INS_ACC3_ID",	3015690, True);

print 'Done'
