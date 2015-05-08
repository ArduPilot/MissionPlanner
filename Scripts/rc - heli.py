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

print 'Start Script'

Script.SendRC(3,1600,False)
Script.SendRC(8,1000,True)
print 'sent throttle down'
MAV.doARM(True);
print 'sent arm'
Script.SendRC(8,2000,True)
print 'sent throtle up'
Script.ChangeMode("Guided")
print 'sent guided'
MAV.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 100);
print 'sent takeoff'

