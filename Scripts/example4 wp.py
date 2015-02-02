import sys
import math
import clr
import time
import System
from System import Byte

clr.AddReference("MissionPlanner")
import MissionPlanner
clr.AddReference("MissionPlanner.Utilities") # includes the Utilities class
from MissionPlanner.Utilities import Locationwp
clr.AddReference("MAVLink") # includes the Utilities class
import MAVLink

idmavcmd = MAVLink.MAV_CMD.WAYPOINT
id = int(idmavcmd)

home = Locationwp().Set(-34.9805,117.8518,0, id)
to = Locationwp()
Locationwp.id.SetValue(to, int(MAVLink.MAV_CMD.TAKEOFF))
Locationwp.p1.SetValue(to, 15)
Locationwp.alt.SetValue(to, 50)
wp1 = Locationwp().Set(-35,117.8,50, id)
wp2 = Locationwp().Set(-35,117.89,50, id)
wp3 = Locationwp().Set(-35,117.85,20, id)

print "set wp total"
MAV.setWPTotal(5)
print "upload home - reset on arm"
MAV.setWP(home,0,MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);
print "upload to"
MAV.setWP(to,1,MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);
print "upload wp1"
MAV.setWP(wp1,2,MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);
print "upload wp2"
MAV.setWP(wp2,3,MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);
print "upload wp3"
MAV.setWP(wp3,4,MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);
print "final ack"
MAV.setWPACK();

print "done"



